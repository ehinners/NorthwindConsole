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
                    View.displayMainMenu();
                    /*
                    "1) Display Categories",
                    "2) Add Category",
                    "3) Display Category and related products",
                    "4) Display all Categories and their related products",
                    "5) Add Product",
                    "6) Edit Product",
                    "7) Display Full Specific Product",
                    "8) Edit Category",
                    "9) Display Full Specific Category",
                    "10) Delete Product",
                    "11) Delete Category"
                    */
                    choice = Console.ReadLine();

                    Data.getLogger().Info($"Option {choice} selected");
                    if (choice == "1")
                    {
                        View.displayCategories(Data.GetNorthwindContext().Categories.OrderBy(p => p.CategoryName));
                    }
                    else if (choice == "2")
                    {
                        addCategory();
                    }
                    else if (choice == "3")
                    {
                        View.promptCategorySelection();
                        View.displayCategoryAndRelatedProducts(int.Parse(Console.ReadLine()));
                    }
                    else if (choice == "4")
                    {
                        View.displayAllCategoriesAndRelatedProducts();
                    }
                    else if (choice == "5")
                    {
                        addProduct();
                    }
                    

                } while (choice.ToLower() != escapeValue);
            }
            catch (Exception ex)
            {
                Data.getLogger().Error(ex.Message);
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

        private static bool verifyCategoryID(int selectedID) //TODO: Make these Validation Result Methods
        {
            bool verified = false;
            var results = Data.GetNorthwindContext().Categories.Where(p => p.CategoryId == selectedID);
            if(results.Count()!=0)
            {
                verified = true;
            }
            return verified;
        }

        private static void addProduct()
        {
            /*
            int ProductId // not null
            string ProductName  // not null max 40 char
            int? SupplierId // not null
            int? CategoryId // not null
            string QuantityPerUnit // max 20 char
            decimal? UnitPrice
            short? UnitsInStock
            short? UnitsOnOrder
            short? ReorderLevel
            bool Discontinued // not null
            */
            
            Product product = new Product();

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
            }
            else if(verifySupplierID(tempInt))
            {
                product.SupplierId = tempInt;
            }     
            else
            {
                Data.getLogger().Error("Supplier Not Found");
            }       

            // User Provides Category ID
            View.addProdCategoryIdPrompt();
            View.displayCategorySelect(Data.GetNorthwindContext().Categories.OrderBy(p => p.CategoryName));
            tempInt = 0;
            userInput =  Console.ReadLine();
            if(!Int32.TryParse(userInput, out tempInt))
            {
                Data.getLogger().Error("Not Valid Int");
            }
            else if(verifyCategoryID(tempInt))
            {
                product.CategoryId = tempInt;
            }     
            else
            {
                Data.getLogger().Error("Category Not Found");
            }

            View.addProdQuantityPerUnitPrompt();
            product.QuantityPerUnit = Console.ReadLine();

            View.addProdUnitPricePrompt();
            //product.UnitPrice = Console.ReadLine();

            View.addProdUnitsInStockPrompt();
            //product.UnitsInStock = Console.ReadLine();

            View.addProdUnitsOnOrderPrompt();
            //product.UnitsOnOrder = Console.ReadLine();

            View.addProdReorderLevelPrompt();
            //product.ReorderLevel = Console.ReadLine();

            View.addProdDiscontinuedPrompt();
            //product.Discontinued = Console.ReadLine();    
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
                    // TODO: save category to db
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