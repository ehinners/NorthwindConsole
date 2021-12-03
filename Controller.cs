using System;
using System.IO;
using NLog.Web;
using System.Collections.Generic;
using System.Linq;
using NorthwindConsole.Model;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace NorthwindConsole
{
    public static class Controller
    {
        private static string escapeValue = "q";

        public static string getEscapeValue()
        {
            return escapeValue;
        }
        public static void mainLoop()
        {
            Data.getLogger().Info("Program started");
            try
            {
                string choice;
                do
                {
                    View.displayPages();
                    choice = Console.ReadLine();
                    Data.getLogger().Info($"Option {choice} selected");
                    if(choice == "1" || choice.ToLower().Contains("product"))
                    {
                        productMenu();                       
                    }
                    if(choice == "2" || choice.ToLower().Contains("categor"))
                    {
                        categoryMenu();   
                    }
                    
                } while (choice.ToLower() != escapeValue);
            }
            catch (Exception ex)
            {
                Data.getLogger().Error(ex.Message);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        // ------------------------------------------MENUS------------------------------------------- //
        ////////////////////////////////////////////////////////////////////////////////////////////////

        private static void productMenu()
        {
            View.displayMainMenuProductOptions();
            string choice = Console.ReadLine();
            Data.getLogger().Info($"Option {choice} selected");
        
            if(choice == "1")
            {
                addProduct();
            }
            if(choice == "2")
            {
                editProduct();
            }
            if(choice == "3")
            {
                displayProducts();
            }
            if(choice == "4")
            {
                displayFullSpecificProduct();
            }
            if(choice == "5")
            {
                deleteProduct();
            }
        }

        private static void categoryMenu()
        {
            View.displayMainMenuCategoryOptions();
            string choice = Console.ReadLine();
            Data.getLogger().Info($"Option {choice} selected");
            
            if(choice == "1")
            {
                View.displayCategories(Data.GetNorthwindContext().Categories.OrderBy(p => p.CategoryName));
            }
            if(choice == "2")
            {
                addCategory();
            }
            if(choice == "3")
            {
                displayCategoryAndRelatedProducts();
            }
            if(choice == "4")
            {
                View.displayAllCategoriesAndRelatedProducts();
            }
            if(choice == "5")
            {
                editCategory();
            }
            if(choice == "6")
            {
                deleteCategory();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        // -----------------------------------------PRODUCTS----------------------------------------- //
        ////////////////////////////////////////////////////////////////////////////////////////////////

        private static void deleteProduct()
        {
            View.deleteProdPrompt();
            View.displayProductSelect(Data.GetNorthwindContext().Products.OrderBy(p => p.ProductId));
            int userChoice = 0;
            string userInput =  Console.ReadLine();
            if(!Int32.TryParse(userInput, out userChoice))
            {
                Data.getLogger().Error("Not Valid Int");
            }
            else if(verifyProductID(userChoice))
            {     
                var query = Data.GetNorthwindContext().Products.Where(p => p.ProductId == userChoice);
                Product product = query.First();
                Data.getLogger().Info("Product {0} Selected", product.ProductName);
                View.deleteProdConfirmation(product);

                userInput = Console.ReadLine();
                string yesOrNo = parseBool(userInput);
                if(yesOrNo == "true")
                {
                    Data.getLogger().Info("Product {0} Deleted", product.ProductName);
                    //Data.GetNorthwindContext().Remove(product);
                    Northwind_88_EHContext dbContext = Data.GetNorthwindContext();
                    dbContext.DeleteProduct(product);
                }
                else
                {
                    Data.getLogger().Info("Product {0} Deletion Aborted", product.ProductName);
                }
            }
        }

        private static void displayFullSpecificProduct()
        {
            int userChoice = 0;
            View.viewSpecificProductPrompt();
            View.displayProductSelect(Data.GetNorthwindContext().Products.OrderBy(p => p.ProductId));

            string userInput =  Console.ReadLine();
            if(!Int32.TryParse(userInput, out userChoice))
            {
                Data.getLogger().Error("Not Valid Int");
            }
            else if(verifyProductID(userChoice))
            {     
                var query = Data.GetNorthwindContext().Products.Where(p => p.ProductId == userChoice);
                Data.getLogger().Info("Product {0} Selected", query.First().ProductName);
                View.displaySpecificProduct(query);
            }
        }

        private static void displayProducts()
        {
            View.productsViewPrompt();

            string choice = Console.ReadLine();
                Data.getLogger().Info($"Option {choice} selected");

                if(choice == "1")
                {
                    View.displayAllProducts(Data.GetNorthwindContext().Products.OrderBy(p => p.ProductId));
                }
                if(choice == "2")
                {
                    View.displayActiveProducts(Data.GetNorthwindContext().Products.Where(p => p.Discontinued == false));
                }
                if(choice == "3")
                {
                    View.displayDiscontinuedProducts(Data.GetNorthwindContext().Products.Where(p => p.Discontinued == true));
                }
        }

        private static void editProduct()
        {
            View.editProductSelectionPrompt();
            View.displayProductSelect(Data.GetNorthwindContext().Products.OrderBy(p => p.ProductId));
            string userInput =  Console.ReadLine();
            int tempInt;
            short tempUnits;
            int selectedProductID;
            IEnumerable<Product> query;
            Product product;
            if(!Int32.TryParse(userInput, out tempInt))
            {
                Data.getLogger().Error("Not Valid Int");
            }
            else if(verifyProductID(tempInt))
            {
                selectedProductID = tempInt;
                Northwind_88_EHContext dbContext = Data.GetNorthwindContext();
                query = dbContext.Products.Where(p => p.ProductId == selectedProductID);
                product = query.First();
                bool editing = true;

                while(editing)
                {
                    View.editProductOptionsPrompt(product);
                    string editProductChoice = Console.ReadLine();

                    if(editProductChoice == "1")
                    {
                        // User Provides New Name
                        View.addProdProductNamePrompt();
                        product.ProductName = Console.ReadLine();
                         Data.getLogger().Info("Product {0} product name changed", product.ProductName);
                    }

                    if(editProductChoice == "2")
                    {
                        // User Provides Supplier ID
                        View.addProdSupplierIdPrompt();
                        View.displaySupplierSelect(dbContext.Suppliers.OrderBy(p => p.SupplierId));
                        tempInt = 0;
                        userInput =  Console.ReadLine();
                        if(!Int32.TryParse(userInput, out tempInt))
                        {
                            Data.getLogger().Error("Not Valid Int");
                        }
                        else if(verifySupplierID(tempInt))
                        {
                            product.SupplierId = tempInt;
                            Data.getLogger().Info("Product {0} Supplier ID changed", product.ProductName);
                        }     
                        else
                        {
                            Data.getLogger().Error("Supplier Not Found");
                        }                            
                    }

                    if(editProductChoice == "3")
                    {
                        // User Provides Category ID
                        View.addProdCategoryIdPrompt();
                        View.displayCategorySelect(dbContext.Categories.OrderBy(p => p.CategoryName));
                        tempInt = 0;
                        userInput =  Console.ReadLine();
                        if(!Int32.TryParse(userInput, out tempInt))
                        {
                            Data.getLogger().Error("Not Valid Int");
                        }
                        else if(verifyCategoryID(tempInt))
                        {
                            product.CategoryId = tempInt;
                            Data.getLogger().Info("Product {0} Category ID changed", product.ProductName);
                        }     
                        else
                        {
                            Data.getLogger().Error("Category Not Found");
                        }
                        
                    }

                    if(editProductChoice == "4")
                    {
                        // User Provides Quantity Per Unit
                        View.addProdQuantityPerUnitPrompt();
                        product.QuantityPerUnit = Console.ReadLine();
                        Data.getLogger().Info("Product {0} Quantity Per Unit changed", product.ProductName);
                    }

                    if(editProductChoice == "5")
                    {
                        // User Provides Unit Price
                        View.addProdUnitPricePrompt();
                        decimal tempPrice;
                        userInput =  Console.ReadLine();
                        if(!Decimal.TryParse(userInput, out tempPrice))
                        {
                            Data.getLogger().Error("Not Valid Decimal");                            
                        }
                        else
                        {
                            product.UnitPrice = tempPrice;
                            Data.getLogger().Info("Product {0} Unit Price changed", product.ProductName);
                        }
                    }

                    if(editProductChoice == "6")
                    {
                        // User Provides Units In Stock
                        View.addProdUnitsInStockPrompt();
                        tempUnits = 0;
                        userInput =  Console.ReadLine();
                        if(!short.TryParse(userInput, out tempUnits))
                        {
                            Data.getLogger().Error("Not Valid Number");
                        }
                        else
                        {
                            product.UnitsInStock = tempUnits;
                            Data.getLogger().Info("Product {0} Units In Stock changed", product.ProductName);
                        }                        
                    }

                    if(editProductChoice == "7")
                    {
                        // User Provides Units On Order
                        View.addProdUnitsOnOrderPrompt();
                        tempUnits = 0;
                        userInput =  Console.ReadLine();
                        if(!short.TryParse(userInput, out tempUnits))
                        {
                            Data.getLogger().Error("Not Valid Number");
                        }
                        else
                        {
                            product.UnitsOnOrder = tempUnits;
                            Data.getLogger().Info("Product {0} Units On Order changed", product.ProductName);
                        }                        
                    }

                    if(editProductChoice == "8")
                    {
                        // User Provides Reorder Level
                        View.addProdReorderLevelPrompt();
                        tempUnits = 0;
                        userInput =  Console.ReadLine();
                        if(!short.TryParse(userInput, out tempUnits))
                        {
                            Data.getLogger().Error("Not Valid Number");
                        }
                        else
                        {
                            product.ReorderLevel = tempUnits;
                            Data.getLogger().Info("Product {0} Reorder changed", product.ProductName);
                        }                        
                    }

                    if(editProductChoice == "9")
                    {
                        // User Provides Discontinued Status
                        View.addProdDiscontinuedPrompt();
                        userInput = Console.ReadLine();
                        string yesOrNo = parseBool(userInput);
                        if(yesOrNo == "true")
                        {
                            product.Discontinued = true;
                            Data.getLogger().Info("Product {0} Discontinued Status changed", product.ProductName);
                        }
                        else if(yesOrNo == "false")
                        {
                            product.Discontinued = false;
                            Data.getLogger().Info("Product {0} Discontinued Status changed", product.ProductName);
                        }
                    }

                    if(editProductChoice.ToLower() == "q")
                    {
                        editing = false;
                    }
                }

                ValidationContext context = new ValidationContext(product, null, null);
                List<ValidationResult> results = new List<ValidationResult>();

                var isValid = Validator.TryValidateObject(product, context, results, true);
                if (isValid)
                {
                    // check for unique name
                    
                    if(dbContext.Products.Any(p => p.ProductName == product.ProductName && p.ProductId != product.ProductId))
                    {
                        // generate validation error
                        isValid = false;
                        results.Add(new ValidationResult("Name exists", new string[] { "ProductName" }));
                    }
                    else
                    {
                        Data.getLogger().Info("Validation passed");
                        try
                        {
                            // Update Database      
                            dbContext.SaveChanges();
                            Data.getLogger().Info("Product {0} edited and saved", product.ProductName);
                        }
                        catch (Exception ex)
                        {
                            Model.Data.getLogger().Error(ex.Message);
                        }
                    }
                }
                if (!isValid)
                {
                    Data.getLogger().Error("Item Not Saved");
                    foreach (var result in results)
                    {
                        Data.getLogger().Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                    }
                }

            }            
        }

        private static void addProduct()
        {          
            Product product = new Product();
            bool noNulls = true;

            // User Provides New Name
            View.addProdProductNamePrompt();
            product.ProductName = Console.ReadLine();

            // User Provides Supplier ID
            View.addProdSupplierIdPrompt();
            View.displaySupplierSelect(Data.GetNorthwindContext().Suppliers.OrderBy(p => p.SupplierId));
            int tempInt = 0;
            string userInput =  Console.ReadLine();
            if(!Int32.TryParse(userInput, out tempInt))
            {
                Data.getLogger().Error("Not Valid Int");
                noNulls = false;
            }
            else if(verifySupplierID(tempInt))
            {
                product.SupplierId = tempInt;
            }     
            else
            {
                Data.getLogger().Error("Supplier Not Found");
                noNulls = false;
            }       

            // User Provides Category ID
            View.addProdCategoryIdPrompt();
            View.displayCategorySelect(Data.GetNorthwindContext().Categories.OrderBy(p => p.CategoryName));
            tempInt = 0;
            userInput =  Console.ReadLine();
            if(!Int32.TryParse(userInput, out tempInt))
            {
                Data.getLogger().Error("Not Valid Int");
                noNulls = false;
            }
            else if(verifyCategoryID(tempInt))
            {
                product.CategoryId = tempInt;
            }     
            else
            {
                Data.getLogger().Error("Category Not Found");
                noNulls = false;
            }

            // User Provides Quantity Per Unit
            View.addProdQuantityPerUnitPrompt();
            product.QuantityPerUnit = Console.ReadLine();
            if(product.QuantityPerUnit.Length == 0)
            {
                noNulls = false;
            }

            // User Provides Unit Price
            View.addProdUnitPricePrompt();
            decimal tempPrice;
            userInput =  Console.ReadLine();
            if(!Decimal.TryParse(userInput, out tempPrice))
            {
                Data.getLogger().Error("Not Valid Decimal");
                noNulls = false;
            }
            else
            {
                product.UnitPrice = tempPrice;
            }

            // User Provides Units In Stock
            View.addProdUnitsInStockPrompt();
            short tempUnits;
            userInput =  Console.ReadLine();
            if(!short.TryParse(userInput, out tempUnits))
            {
                Data.getLogger().Error("Not Valid Number");
                noNulls = false;
            }
            else
            {
                product.UnitsInStock = tempUnits;
            }

            // User Provides Units On Order
            View.addProdUnitsOnOrderPrompt();
            tempUnits = 0;
            userInput =  Console.ReadLine();
            if(!short.TryParse(userInput, out tempUnits))
            {
                Data.getLogger().Error("Not Valid Number");
                noNulls = false;
            }
            else
            {
                product.UnitsOnOrder = tempUnits;
            }

            // User Provides Reorder Level
            View.addProdReorderLevelPrompt();
            tempUnits = 0;
            userInput =  Console.ReadLine();
            if(!short.TryParse(userInput, out tempUnits))
            {
                Data.getLogger().Error("Not Valid Number");
                noNulls = false;
            }
            else
            {
                product.ReorderLevel = tempUnits;
            }

            // User Provides Discontinued Status
            View.addProdDiscontinuedPrompt();
            userInput = Console.ReadLine();
            string yesOrNo = parseBool(userInput);
            if(yesOrNo == "true")
            {
                product.Discontinued = true;
            }
            else if(yesOrNo == "false")
            {
                product.Discontinued = false;
            }
            else
            {
                product.Discontinued = false;
                noNulls = false;
            }
            bool permissed = false;
            if(!noNulls)
            {
                View.nullCheck();
                userInput = Console.ReadLine();
                yesOrNo = parseBool(userInput);
                if(yesOrNo == "true")
                {
                    permissed = true;
                }
                else if(yesOrNo == "false")
                {
                    permissed = false;
                }
                else
                {
                    Data.getLogger().Error("Input Not Recognized");
                }
            }
            else
            {
                permissed = true;
            }
            if(permissed)
            {
                ValidationContext context = new ValidationContext(product, null, null);
                List<ValidationResult> results = new List<ValidationResult>();

                var isValid = Validator.TryValidateObject(product, context, results, true);
                if (isValid)
                {
                    // check for unique name
                    if (Data.GetNorthwindContext().Products.Any(p => p.ProductName == product.ProductName))
                    {
                        // generate validation error
                        isValid = false;
                        results.Add(new ValidationResult("Name exists", new string[] { "ProductName" }));
                    }
                    else
                    {
                        Data.getLogger().Info("Validation passed");
                        try
                        {
                            // Create and save a new Product        
                            
                            Model.Data.GetNorthwindContext().AddProduct(product);
                            Model.Data.getLogger().Info("Product added - {name}", product.ProductName);
                        }
                        catch (Exception ex)
                        {
                            Model.Data.getLogger().Error(ex.Message);
                        }
                    }
                }
                if (!isValid)
                {
                    foreach (var result in results)
                    {
                        Data.getLogger().Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                    }
                }
            }
               
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////
        // ---------------------------------------VALIDATORS----------------------------------------- //
        ////////////////////////////////////////////////////////////////////////////////////////////////

        private static bool verifySupplierID(int selectedID) //TODO: Make these Validation Result Methods
        {
            bool verified = false;
            var results = Data.GetNorthwindContext().Suppliers.Where(p => p.SupplierId == selectedID);
            if(results.Count()!=0)
            {
                verified = true;
            }
            return verified;
        }

        private static bool verifyProductID(int selectedID) //TODO: Make these Validation Result Methods
        {
            bool verified = false;
            var results = Data.GetNorthwindContext().Products.Where(p => p.ProductId == selectedID);
            if(results.Count()!=0)
            {
                verified = true;
            }
            else{
                Model.Data.getLogger().Error("Product Not Found");
            }
            return verified;
        }

        private static bool verifyCategoryID(int selectedID) //TODO: Make these Validation Result Methods
        {
            bool verified = false;
            var results = Data.GetNorthwindContext().Categories.Where(p => p.CategoryId == selectedID);
            if(results.Count()!=0)
            {
                verified = true;
            }
            else{
                Model.Data.getLogger().Error("Category Not Found");
            }
            return verified;
        }

        private static string parseBool(string input)
        {
            string result;
            if(input.Length >= 1)
            {
                if(input.ToLower()[0]=='y')
                {
                    result = "true";
                }
                else if(input.ToLower()[0]=='n')
                {
                    result = "false";
                }
                else
                {
                    result = "neither";
                    Model.Data.getLogger().Error("Not Valid Selection");
                }
            }
            else
            {
                result = "neither";
                Model.Data.getLogger().Error("Null Entry Not Accepted");
            }           
            
            return result;
        }

       

        ////////////////////////////////////////////////////////////////////////////////////////////////
        // ----------------------------------------CATEGORIES---------------------------------------- //
        ////////////////////////////////////////////////////////////////////////////////////////////////

        private static void deleteCategory()
        {
            System.Console.WriteLine("Not Yet Implemented");
        }

        private static void editCategory()
        {
            View.editCategorySelectionPrompt();
            View.displayCategorySelect(Data.GetNorthwindContext().Categories.OrderBy(c => c.CategoryId));
            string userInput =  Console.ReadLine();
            int selectedCategoryID;
            int tempInt;
            IEnumerable<Category> query;
            Category category;
            if(!Int32.TryParse(userInput, out tempInt))
            {
                Data.getLogger().Error("Not Valid Int");
            }
            else if(verifyProductID(tempInt))
            {
                selectedCategoryID = tempInt;
                Northwind_88_EHContext dbContext = Data.GetNorthwindContext();
                query = dbContext.Categories.Where(c => c.CategoryId == selectedCategoryID);
                category = query.First();
                bool editing = true;

                while(editing)
                {
                    View.editCategoryOptionsPrompt(category);
                    string editCategoryChoice = Console.ReadLine();

                    if(editCategoryChoice == "1")
                    {
                        // User Provides New Name
                        View.addCategoryNamePrompt();
                        category.CategoryName = Console.ReadLine();
                        Data.getLogger().Info("Category {0} product name changed", category.CategoryName);
                    }

                    if(editCategoryChoice == "2")
                    {
                        // User Provides Description
                        View.addCategoryDescriptionPrompt();
                        category.Description = Console.ReadLine();
                        Data.getLogger().Info("Category {0} Description changed", category.Description);
                    }

                    if(editCategoryChoice.ToLower() == "q")
                    {
                        editing = false;
                    }
                }

                ValidationContext context = new ValidationContext(category, null, null);
                List<ValidationResult> results = new List<ValidationResult>();

                var isValid = Validator.TryValidateObject(category, context, results, true);
                if (isValid)
                {
                    // check for unique name
                    
                    if(dbContext.Categories.Any(c => c.CategoryName == category.CategoryName && c.CategoryId != category.CategoryId))
                    {
                        // generate validation error
                        isValid = false;
                        results.Add(new ValidationResult("Name exists", new string[] { "CategoryName" }));
                    }
                    else
                    {
                        Data.getLogger().Info("Validation passed");
                        try
                        {
                            // Update Database      
                            dbContext.SaveChanges();
                            Data.getLogger().Info("Category {0} edited and saved", category.CategoryName);
                        }
                        catch (Exception ex)
                        {
                            Model.Data.getLogger().Error(ex.Message);
                        }
                    }
                }
                if (!isValid)
                {
                    Data.getLogger().Error("Item Not Saved");
                    foreach (var result in results)
                    {
                        Data.getLogger().Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                    }
                }

            }            
        }

        private static void displayCategoryAndRelatedProducts()
        {
            View.promptCategorySelection();
            View.displayCategorySelect(Data.GetNorthwindContext().Categories.OrderBy(p => p.CategoryId));
            int tempInt;
            string userInput = Console.ReadLine();
            if(!Int32.TryParse(userInput, out tempInt))
            {
                Data.getLogger().Error("Not Valid Int");
            }
            else
            {
                Data.getLogger().Info($"CategoryId {tempInt} selected");
                Category category = Data.GetNorthwindContext().Categories.Include("Products").FirstOrDefault(c => c.CategoryId == tempInt);
                View.displayCategoryAndRelatedProducts(category);
            }
        }

        private static void addCategory()
        {
            Category category = new Category();
            View.addCategoryNamePrompt();
            category.CategoryName = Console.ReadLine();
            View.addCategoryDescriptionPrompt();
            category.Description = Console.ReadLine();
                        
                        
            ValidationContext context = new ValidationContext(category, null, null);
            List<ValidationResult> results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(category, context, results, true);
            if (isValid)
            {
                // check for unique name
                if (Data.GetNorthwindContext().Categories.Any(c => c.CategoryName == category.CategoryName))
                {
                    // generate validation error
                    isValid = false;
                    results.Add(new ValidationResult("Name exists", new string[] { "CategoryName" }));
                }
                else
                {
                    Data.getLogger().Info("Validation passed");                    
                    
                    try
                    {
                        // Create and save a new Category        
                        
                        Model.Data.GetNorthwindContext().AddCategory(category);
                        Model.Data.getLogger().Info("Category added - {name}", category.CategoryName);
                    }
                    catch (Exception ex)
                    {
                        Model.Data.getLogger().Error(ex.Message);
                    }
                }
            }
            if (!isValid)
            {
                foreach (var result in results)
                {
                    Data.getLogger().Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                }
            }
        }
    }
}