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
    public static class View
    {
        private static List<string> mainMenuOptions = new List<string>()
        {
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
        };
        
        public static void displayMainMenu()
        {
            System.Console.WriteLine("Enter your selection:");
            foreach(string option in mainMenuOptions)
            {
                System.Console.WriteLine(option);
            } 
            System.Console.WriteLine("Enter {0} to quit",Controller.getEscapeValue());           
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        // ----------------------------------------SELECTORS----------------------------------------- //
        ////////////////////////////////////////////////////////////////////////////////////////////////

        public static void displaySupplierSelect(IEnumerable<Supplier> query)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            foreach (var item in query)
            {
                Console.WriteLine($"{item.SupplierId} - {item.CompanyName}");
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void displayCategorySelect(IEnumerable<Category> query)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            foreach (var item in query)
            {
                Console.WriteLine($"{item.CategoryId} - {item.CategoryName}");
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        // ----------------------------------------CATEGORIES---------------------------------------- //
        ////////////////////////////////////////////////////////////////////////////////////////////////

        public static void displayCategories(IEnumerable<Category> query)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{query.Count()} records returned");
            Console.ForegroundColor = ConsoleColor.Magenta;
            foreach (var item in query)
            {
                Console.WriteLine($"{item.CategoryName} - {item.Description}");
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void promptCategorySelection()
        {
            Console.WriteLine("Select the category whose products you want to display:");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            foreach (var item in Data.GetNorthwindContext().Categories.OrderBy(p => p.CategoryId))
            {
                Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
            }
        }

        public static void displayCategoryAndRelatedProducts(int id)
        {     
            Console.ForegroundColor = ConsoleColor.White;
            
            Console.Clear();
            Data.getLogger().Info($"CategoryId {id} selected");

            Category category = Data.GetNorthwindContext().Categories.Include("Products").FirstOrDefault(c => c.CategoryId == id);
            Console.WriteLine($"{category.CategoryName} - {category.Description}");

            foreach (Product p in category.Products)
            {
                Console.WriteLine(p.ProductName);
            }
        }

        public static void displayAllCategoriesAndRelatedProducts()
        {
            foreach (var item in Data.GetNorthwindContext().Categories.Include("Products").OrderBy(p => p.CategoryId))
            {
                Console.WriteLine($"{item.CategoryName}");
                foreach (Product p in item.Products)
                {
                    Console.WriteLine($"\t{p.ProductName}");
                }
            }
        }

        public static void addCategoryNamePrompt()
        {
            Console.WriteLine("Enter Category Name:");
        }

        public static void addCategoryDescriptionPrompt()
        {
            Console.WriteLine("Enter the Category Description:");
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        // -----------------------------------------PRODUCTS----------------------------------------- //
        ////////////////////////////////////////////////////////////////////////////////////////////////

        public static void addProdProductNamePrompt()
        {
            Console.WriteLine("Enter The Product Name:");
        }

        public static void addProdSupplierIdPrompt()
        {
            System.Console.WriteLine("Enter The Supplier ID:");
        }

        public static void addProdCategoryIdPrompt()
        {
            System.Console.WriteLine("Enter The Category ID:");
        }

        public static void addProdQuantityPerUnitPrompt()
        {
            System.Console.WriteLine("Enter The Quantity Per Unit:");
        }

        public static void addProdUnitPricePrompt()
        {
            System.Console.WriteLine("Enter The Unit Price:");
        }

        public static void addProdUnitsInStockPrompt()
        {
            System.Console.WriteLine("Enter The Number of Units In Stock:");
        }

        public static void addProdUnitsOnOrderPrompt()
        {
            System.Console.WriteLine("Enter The Number of Units On Order:");
        }

        public static void addProdReorderLevelPrompt()
        {
            System.Console.WriteLine("Enter The Reorder Level:");
        }

        public static void addProdDiscontinuedPrompt()
        {
            System.Console.WriteLine("Is The Product Discontinued? (Y/N):");
        }

                   
                    
                    
    }
}