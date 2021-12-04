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
                        productMenu();  // sub menu for products                     
                    }
                    if(choice == "2" || choice.ToLower().Contains("categor"))
                    {
                        categoryMenu();  // sub menu for categories
                    }
                    
                } while (choice.ToLower() != escapeValue); // ends program
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
            Data.getLogger().Info($"Products Menu - {choice} selected");
        
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
            Data.getLogger().Info($"Categories Menu -  {choice} selected");
            
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
            // User is asked to select a product to delete
            View.deleteProdPrompt();
            // User is given a list of products to delete
            View.displayProductSelect(Data.GetNorthwindContext().Products.OrderBy(p => p.ProductId));
            int userChoice = 0;
            string userInput =  Console.ReadLine();
            // User should enter the id of the product they wish to delete
            if(!Int32.TryParse(userInput, out userChoice))
            {
                Data.getLogger().Error("Not Valid Int");
            }
            else if(verifyProductID(userChoice))// existance of product with chosen product id is verified
            {     
                // product is the product with the selected productId
                var query = Data.GetNorthwindContext().Products.Where(p => p.ProductId == userChoice);
                Product product = query.First();
                Data.getLogger().Info("Product {0} Selected", product.ProductName);
                // User is given option to choose NOT to delete (includes warning about number of orphans)
                View.deleteProdConfirmation(product);

                userInput = Console.ReadLine();
                string yesOrNo = parseBool(userInput);
                if(yesOrNo == "true")
                {
                    try
                    {
                        Data.getLogger().Info("Product {0} Deleted", product.ProductName);
                        //Data.GetNorthwindContext().Remove(product);
                        Northwind_88_EHContext dbContext = Data.GetNorthwindContext();
                        dbContext.DeleteProduct(product);
                        // delete product handles orphans
                    }
                    catch(Exception e)
                    {
                        Data.getLogger().Error(e.Message);
                    }
                    
                }
                else
                {
                    Data.getLogger().Info("Product {0} Deletion Aborted", product.ProductName);
                }
            }
            else
            {
                Data.getLogger().Error("Product Not Found");
            }
        }

        private static void displayFullSpecificProduct()
        {
            int userChoice = 0;
            View.viewSpecificProductPrompt();
            // user is given list of products to choose an id from
            View.displayProductSelect(Data.GetNorthwindContext().Products.OrderBy(p => p.ProductId));

            string userInput =  Console.ReadLine();
            if(!Int32.TryParse(userInput, out userChoice))
            {
                Data.getLogger().Error("Not Valid Int");
            }
            else if(verifyProductID(userChoice))// verifies a product exists with selected productID
            {     
                var query = Data.GetNorthwindContext().Products.Where(p => p.ProductId == userChoice);
                Data.getLogger().Info("Product {0} Selected", query.First().ProductName);
                View.displaySpecificProduct(query);
            }
            else
            {
                Data.getLogger().Error("Product Not Found");
            }
        }

        private static void displayProducts()
        {
            View.productsViewPrompt();

            string choice = Console.ReadLine();
                Data.getLogger().Info($"Display Products - {choice} selected");

                // user is given an option of which set of products to display
                // all, active (not discontinued), or ONLY discontinued

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
            // user may change properties of a product directly, once they've indicated they are done
            // changing values, the resulting product is verified, and only if it passes verification again
            // is it saved to the database, otherwise the context is refreshed and changes are lost

            View.editProductSelectionPrompt();
            // user is given list of products to select from, including their product id
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
            else if(verifyProductID(tempInt))// product existance is verified with selected productID
            {
                selectedProductID = tempInt;
                Northwind_88_EHContext dbContext = Data.GetNorthwindContext();
                query = dbContext.Products.Where(p => p.ProductId == selectedProductID);
                product = query.First(); // product is the selected product from list with selected productID
                Data.getLogger().Info("Edit Product - {0} Selected", product.ProductName);
                bool editing = true; 
                // allows the user to make several edits in a row, only a few, redundant edits, or none at all
                while(editing)
                {
                    View.editProductOptionsPrompt(product);
                    string editProductChoice = Console.ReadLine();
                    // user is given a list of all mutable product properties in which they may select one to edit
                    Data.getLogger().Info("Edit Product Choice - {0}", editProductChoice);
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
                        // user is given a list of all potential/existing suppliers (supplier name + supplierID)
                        tempInt = 0;
                        userInput =  Console.ReadLine();
                        if(!Int32.TryParse(userInput, out tempInt))
                        {
                            Data.getLogger().Error("Not Valid Int");
                        }
                        else
                        {
                            product.SupplierId = tempInt;
                            Data.getLogger().Info("Supplier ID Changed");
                        }
                                             
                    }

                    if(editProductChoice == "3")
                    {
                        // User Provides Category ID
                        View.addProdCategoryIdPrompt();
                        View.displayCategorySelect(dbContext.Categories.OrderBy(p => p.CategoryName));
                        // user is given full list of existing categories (to create a new one, that's a different menu)
                        tempInt = 0;
                        userInput =  Console.ReadLine();
                        if(!Int32.TryParse(userInput, out tempInt))
                        {
                            Data.getLogger().Error("Not Valid Int");
                        }
                        else
                        {
                            product.CategoryId = tempInt;
                            Data.getLogger().Info("Category ID changed");
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
                
                // check for unique name, taking into account that the item already exists, 
                // so it ignores products with the same name AND id
                
                if(dbContext.Products.Any(p => p.ProductName == product.ProductName && p.ProductId != product.ProductId))
                {
                    // generate validation error
                    isValid = false;
                    results.Add(new ValidationResult("Name exists", new string[] { "ProductName" }));
                } 
                if(!verifyCategoryID((int)product.CategoryId))
                {
                    // generate validation error
                    isValid = false;
                    results.Add(new ValidationResult("Category Does Not exist", new string[] { "ProductName" }));
                }
                if(!verifySupplierID((int)product.SupplierId))
                {
                    // generate validation error
                    isValid = false;
                    results.Add(new ValidationResult("Supplier Does Not exist", new string[] { "ProductName" }));
                }
                

                if(isValid)
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
                else
                {
                    Data.getLogger().Error("Item Not Saved");
                    foreach (var result in results)
                    {
                        Data.getLogger().Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                    }
                }

            }  
            else{
                Data.getLogger().Error("Product Not Found");
            }          
        }

        private static void addProduct()
        {          
            // User is given a set of prompts to propagate fields for a new product
            // User is given option to allow most fields to be NULL, database is configured to allow some
            // overrides others with defaults
            Product product = new Product();
            bool noNulls = true; // Any unvalidated or null values entered by the user flags this as false
            // A false value will warn the user before the product is saved, and allows them to cancel

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
            else
            {
                 product.SupplierId = tempInt;
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
            else
            {
                 product.CategoryId = tempInt;
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
            // If the user failed any validations, or provided a null value
            // They are requred to confirm their addition as an extra step
            // And are warned about the object having null values
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
            } // if the user allowed null values, or all input was validated, final validation can proceed
            if(permissed)
            {
                
                ValidationContext context = new ValidationContext(product, null, null);
                List<ValidationResult> results = new List<ValidationResult>();

                var isValid = Validator.TryValidateObject(product, context, results, true);
                
                // check for unique name
                if (Data.GetNorthwindContext().Products.Any(p => p.ProductName == product.ProductName))
                {
                    // generate validation error
                    isValid = false;
                    results.Add(new ValidationResult("Name exists", new string[] { "ProductName" }));
                }
                try{
                    if(product.CategoryId != null)
                    {
                        if(!verifyCategoryID((int)product.CategoryId))
                        {
                            // generate validation error
                            isValid = false;
                            results.Add(new ValidationResult("Category Does Not exist", new string[] { "ProductName" }));
                        }
                    }
                    if(product.SupplierId != null)
                    {
                        if(!verifySupplierID((int)product.SupplierId))
                        {
                            // generate validation error
                            isValid = false;
                            results.Add(new ValidationResult("Supplier Does Not exist", new string[] { "ProductName" }));
                        }
                    }
                    
                    
                }
                catch(Exception yyyy)
                {
                    Data.getLogger().Error(yyyy.Message);
                }
                    

                if(isValid)
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
                else
                {
                    Data.getLogger().Error("Item Not Saved");
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

        // given an id, these will return a bool value based on whether any relative objects with that id
        // exist in the database

        private static bool verifySupplierID(int selectedID) 
        {
            bool verified = false;
            var results = Data.GetNorthwindContext().Suppliers.Where(p => p.SupplierId == selectedID);
            if(results.Count()!=0)
            {
                verified = true;
            }
            return verified;
        }

        private static bool verifyProductID(int selectedID) 
        {
            bool verified = false;
            var results = Data.GetNorthwindContext().Products.Where(p => p.ProductId == selectedID);
            if(results.Count()!=0)
            {
                verified = true;
            }
            return verified;
        }

        private static bool verifyCategoryID(int selectedID) 
        {
            bool verified = false;
            var results = Data.GetNorthwindContext().Categories.Where(p => p.CategoryId == selectedID);
            if(results.Count()!=0)
            {
                verified = true;
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
            View.deleteCatePrompt();
            // user is given list of categories to select from
            View.displayCategorySelect(Data.GetNorthwindContext().Categories.OrderBy(c => c.CategoryId));
            int userChoice = 0;
            string userInput =  Console.ReadLine();
            if(!Int32.TryParse(userInput, out userChoice))
            {
                Data.getLogger().Error("Not Valid Int");
            }
            else if(verifyCategoryID(userChoice)) // selected category's existance is verified
            {     
                var query = Data.GetNorthwindContext().Categories.Where(c => c.CategoryId == userChoice);
                Category category = query.First();
                Data.getLogger().Info("Category {0} Selected", category.CategoryName);
                View.deleteCateConfirmation(category);

                userInput = Console.ReadLine();
                string yesOrNo = parseBool(userInput);
                if(yesOrNo == "true")
                {
                    try
                    {
                        Data.getLogger().Info("Category {0} Deleted", category.CategoryName);                    
                        Northwind_88_EHContext dbContext = Data.GetNorthwindContext();
                        dbContext.DeleteCategory(category);
                    }
                    catch(Exception e)
                    {
                        Data.getLogger().Error(e.Message);
                    }
                    
                }
                else
                {
                    Data.getLogger().Info("Category {0} Deletion Aborted", category.CategoryName);
                }
            }
            else
            {
                Data.getLogger().Error("Category Not Found");
            }
        }

        private static void editCategory()
        {
            View.editCategorySelectionPrompt();
            // user is given a list of categories to select from
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
            else if(verifyCategoryID(tempInt))// existance of selected category is verified
            {
                selectedCategoryID = tempInt;
                Northwind_88_EHContext dbContext = Data.GetNorthwindContext();
                query = dbContext.Categories.Where(c => c.CategoryId == selectedCategoryID);
                category = query.First();
                Data.getLogger().Info("Category Selected - {0}", category.CategoryName);
                bool editing = true;
                // user is given option to perform several edits, redundant edits, a single edit, or none at all
                while(editing)
                {
                    View.editCategoryOptionsPrompt(category);
                    string editCategoryChoice = Console.ReadLine();
                    Data.getLogger().Info("Category Editing Choice - {0}", editCategoryChoice);
                    // user is given option of which property they would like to edit
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
                    // check for unique name, takes into account item already exists
                    
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
                    // Reasons why item did not pass validation are logged
                    Data.getLogger().Error("Item Not Saved");
                    foreach (var result in results)
                    {
                        Data.getLogger().Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                    }
                }

            } 
            else
            {
                Data.getLogger().Error("Category Not Found");
            }           
        }

        private static void displayCategoryAndRelatedProducts()
        {
            View.promptCategorySelection();
            // user is given a list of categories to choose from
            View.displayCategorySelect(Data.GetNorthwindContext().Categories.OrderBy(p => p.CategoryId));
            int tempInt;
            string userInput = Console.ReadLine();
            if(!Int32.TryParse(userInput, out tempInt))
            {
                Data.getLogger().Error("Not Valid Int");
            }
            else if(verifyCategoryID(tempInt))// category's existance is verified
            {
                Data.getLogger().Info($"CategoryId {tempInt} selected");
                Category category = Data.GetNorthwindContext().Categories.Include("Products").FirstOrDefault(c => c.CategoryId == tempInt);
                View.displayCategoryAndRelatedProducts(category);
            }
            else
            {
                Data.getLogger().Error("Category Not Found");
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
                Data.getLogger().Info("Item not Added");
                // Reasons why a category failed validation are logged
                foreach (var result in results)
                {
                    Data.getLogger().Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                }
            }
        }
    }
}