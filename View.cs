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
            "7) Display Products",
            "8) Display Full Specific Product",
            "9) Edit Category",
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

        public static void nullCheck()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine("It Seems One Or More Values Are Not Valid");
            System.Console.WriteLine("Entering In This Object Will Result In One Or More Null Values");
            System.Console.WriteLine("Proceed To Enter In Object With Null Values? (Y/N)");
            Console.ForegroundColor = ConsoleColor.White;
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

        public static void displayProductSelect(IEnumerable<Product> query)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            foreach (var item in query)
            {
                Console.WriteLine($"{item.ProductId}) {item.ProductName}");
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

        // EDITING A PRODUCT

        public static void editProductSelectionPrompt()
        {
            System.Console.WriteLine("Please Select The ID Of The Product You'd Like To Edit: ");
        }

        public static void editProductOptionsPrompt(Product product)
        {
            System.Console.WriteLine("Please Enter The Number Of Which Property To Be Edited:");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            System.Console.WriteLine("1 - Product Name ({0})", product.ProductName);
            System.Console.WriteLine("2 - Supplier ID ({0})", product.SupplierId);
            System.Console.WriteLine("3 - Category ID ({0})", product.CategoryId);
            System.Console.WriteLine("4 - Quantity Per Unit ({0})", product.QuantityPerUnit);;
            System.Console.WriteLine("5 - Unit Price ({0})", product.UnitPrice);
            System.Console.WriteLine("6 - Units In Stock ({0})", product.UnitsInStock);
            System.Console.WriteLine("7 - Units On Order ({0})", product.UnitsOnOrder);
            System.Console.WriteLine("8 - Reorder Level ({0})", product.ReorderLevel);
            System.Console.WriteLine("9 - Discontinued ({0})", product.Discontinued ? "True":"False");
            Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine("Or Enter '{0}' to finish editing", Controller.getEscapeValue());
            Console.ForegroundColor = ConsoleColor.White;
        }

        // CREATING A PRODUCT

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

        // VIEWING PRODUCTS

        public static void viewSpecificProductPrompt()
        {
            Console.WriteLine("Select the Product which you want to display in full:");
        }

        public static void displaySpecificProduct(IEnumerable<Product> query)
        {
            foreach (var item in query)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                System.Console.WriteLine($"Product ID: {item.ProductId}");
                System.Console.WriteLine($"Product Name: {item.ProductName}");
                System.Console.WriteLine($"CategoryID: {item.CategoryId}");
                System.Console.WriteLine($"SupplierID: {item.SupplierId}");
                System.Console.WriteLine($"Quantity Per Unit: {item.QuantityPerUnit}");
                System.Console.WriteLine($"Unit Price: {item.UnitPrice}");
                System.Console.WriteLine($"Units In Stock: {item.UnitsInStock}");
                System.Console.WriteLine($"Units On Order: {item.UnitsOnOrder}");
                System.Console.WriteLine($"Reorder Level: {item.ReorderLevel}");
                if(item.Discontinued)
                {
                    System.Console.WriteLine("Item Discontinued");
                }
                else
                {
                    System.Console.WriteLine("Item Active (Not Discontinued)");
                }
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public static void productsViewPrompt()
        {
            System.Console.WriteLine("Please Select One Of The Following Options:");
            System.Console.WriteLine("1. View All Products");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            System.Console.WriteLine("2. View Only Active (Not Discontinued) Products");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            System.Console.WriteLine("3. View Only Discontinued Products");
            Console.ForegroundColor = ConsoleColor.White;
        }

        

        public static void displayAllProducts(IEnumerable<Product> query)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{query.Count()} records returned");
            Console.ForegroundColor = ConsoleColor.White;
            foreach (var item in query)
            {
                if(item.Discontinued)
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                }
                else if(!item.Discontinued)
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.WriteLine($"{item.ProductName}");
            }
            
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void displayActiveProducts(IEnumerable<Product> query)
        {
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{query.Count()} records returned");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            foreach (var item in query)
            {
                Console.WriteLine($"{item.ProductName}");
            }

            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void displayDiscontinuedProducts(IEnumerable<Product> query)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{query.Count()} records returned");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            foreach (var item in query)
            {
                Console.WriteLine($"{item.ProductName}");
            }           

            Console.ForegroundColor = ConsoleColor.White;
        }

                   
                    
                    
    }
}