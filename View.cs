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

        ////////////////////////////////////////////////////////////////////////////////////////////////
        // ------------------------------------------MENUS------------------------------------------- //
        ////////////////////////////////////////////////////////////////////////////////////////////////
        private static List<string> productOptions = new List<string>()
        {
            "1) Add Product",
            "2) Edit Product",
            "3) Display Products",
            "4) Display Full Specific Product",
            "5) Delete Product"
        };

        private static List<string> categoryOptions = new List<string>()
        {
            "1) Display Categories",
            "2) Add Category",
            "3) Display Category and related products",
            "4) Display all Categories and their related products",
            "5) Edit Category",
            "6) Delete Category"
        };

        public static void displayPages()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            System.Console.WriteLine("Please Select:");
            Console.ForegroundColor = ConsoleColor.Blue;
            System.Console.WriteLine("1. Products");
            System.Console.WriteLine("2. Categories");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            System.Console.WriteLine("Enter {0} to quit",Controller.getEscapeValue());    
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void displayMainMenuProductOptions()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            System.Console.WriteLine("Enter your selection:");
            Console.ForegroundColor = ConsoleColor.Blue;
            foreach(string option in productOptions)
            {
                System.Console.WriteLine(option);
            } 
            Console.ForegroundColor = ConsoleColor.DarkRed;
            System.Console.WriteLine("Enter {0} to quit",Controller.getEscapeValue());   
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void displayMainMenuCategoryOptions()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            System.Console.WriteLine("Enter your selection:");
            Console.ForegroundColor = ConsoleColor.Blue;
            foreach(string option in categoryOptions)
            {
                System.Console.WriteLine(option);
            } 
            Console.ForegroundColor = ConsoleColor.DarkRed;
            System.Console.WriteLine("Enter {0} to quit",Controller.getEscapeValue());   
            Console.ForegroundColor = ConsoleColor.White;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        // -----------------------------------------WARNINGS----------------------------------------- //
        ////////////////////////////////////////////////////////////////////////////////////////////////
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
            Console.ForegroundColor = ConsoleColor.Blue;
            foreach (var item in query)
            {
                Console.WriteLine($"{item.SupplierId} - {item.CompanyName}");
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void displayCategorySelect(IEnumerable<Category> query)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            foreach (var item in query)
            {
                Console.WriteLine($"{item.CategoryId} - {item.CategoryName}");
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void displayProductSelect(IEnumerable<Product> query)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            foreach (var item in query)
            {
                if(item.Discontinued)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                }
                Console.WriteLine($"{item.ProductId}) {item.ProductName}");
            }
            Console.ForegroundColor = ConsoleColor.White;
        }            

        ////////////////////////////////////////////////////////////////////////////////////////////////
        // ----------------------------------------CATEGORIES---------------------------------------- //
        ////////////////////////////////////////////////////////////////////////////////////////////////

        public static void deleteCatePrompt()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            System.Console.WriteLine("Please Select The ID Of The Category To Be ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            System.Console.WriteLine("DELETED:");
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void categoryOrphanWarning(Category category)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Black;
            System.Console.Write("This Will Delete {0} Orphan Items", Data.GetNorthwindContext().Products.Where(p=>p.CategoryId == category.CategoryId).Count());
            Console.BackgroundColor = ConsoleColor.Black;
            System.Console.Write("      ");            
            Console.BackgroundColor = ConsoleColor.Black;
            System.Console.Write("  ");  
            System.Console.WriteLine("");  
        }

        public static void deleteCateConfirmation(Category category)
        {
            categoryOrphanWarning(category);
            Console.ForegroundColor = ConsoleColor.DarkRed;            
            System.Console.WriteLine("Are you SURE you want to DELETE Category {0}? (Y/N)", category.CategoryName);            
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void editCategorySelectionPrompt()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Select the category to edit:");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void editCategoryOptionsPrompt(Category category)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            System.Console.WriteLine("Please Enter The Number Of Which Property To Be Edited:");
            Console.ForegroundColor = ConsoleColor.Blue;
            System.Console.WriteLine("1 - Category Name ({0})", category.CategoryName);
            System.Console.WriteLine("2 - Category Description({0})", category.Description);
            Console.ForegroundColor = ConsoleColor.DarkRed;
            System.Console.WriteLine("Or Enter '{0}' to finish editing", Controller.getEscapeValue());
            Console.ForegroundColor = ConsoleColor.White;
        }


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
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Select the category whose products you want to display:");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void displayCategoryAndRelatedProducts(Category category)
        {     
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"{category.CategoryName} - {category.Description}");
            Console.ForegroundColor = ConsoleColor.DarkCyan;

            foreach (Product p in category.Products)
            {
                if(!p.Discontinued)
                {
                    Console.WriteLine(p.ProductName);
                }                
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void displayAllCategoriesAndRelatedProducts()
        {
            foreach (var item in Data.GetNorthwindContext().Categories.Include("Products").OrderBy(p => p.CategoryId))
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"{item.CategoryName}");
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                foreach (Product p in item.Products)
                {
                    if(!p.Discontinued)
                    {
                        Console.WriteLine($"\t{p.ProductName}");
                    }                    
                }
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public static void addCategoryNamePrompt()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Enter Category Name:");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void addCategoryDescriptionPrompt()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Enter the Category Description:");
            Console.ForegroundColor = ConsoleColor.White;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        // -----------------------------------------PRODUCTS----------------------------------------- //
        ////////////////////////////////////////////////////////////////////////////////////////////////

        // DELETING A PRODUCT

        public static void deleteProdPrompt()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            System.Console.WriteLine("Please Select The ID Of The Product To Be ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            System.Console.WriteLine("DELETED:");
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void productOrphanWarning(Product product)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Black;
            System.Console.Write("This Will Create {0} Orphan Items", Data.GetNorthwindContext().OrderDetails.Where(o=>o.ProductId == product.ProductId).Count());
            Console.BackgroundColor = ConsoleColor.Black;
            System.Console.Write("");            
            Console.BackgroundColor = ConsoleColor.Black;
            System.Console.WriteLine("");
        }

        public static void deleteProdConfirmation(Product product)
        {            
            productOrphanWarning(product);
            Console.ForegroundColor = ConsoleColor.DarkRed;
            System.Console.WriteLine("Are you SURE you want to DELETE Product {0}? (Y/N)", product.ProductName);            
            Console.ForegroundColor = ConsoleColor.White;            
        }

        // EDITING A PRODUCT

        public static void editProductSelectionPrompt()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            System.Console.WriteLine("Please Select The ID Of The Product You'd Like To Edit: ");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void editProductOptionsPrompt(Product product)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            System.Console.WriteLine("Please Enter The Number Of Which Property To Be Edited:");
            Console.ForegroundColor = ConsoleColor.Blue;
            System.Console.WriteLine("1 - Product Name ({0})", product.ProductName);
            System.Console.WriteLine("2 - Supplier ID ({0})", product.SupplierId);
            System.Console.WriteLine("3 - Category ID ({0})", product.CategoryId);
            System.Console.WriteLine("4 - Quantity Per Unit ({0})", product.QuantityPerUnit);;
            System.Console.WriteLine("5 - Unit Price ({0})", product.UnitPrice);
            System.Console.WriteLine("6 - Units In Stock ({0})", product.UnitsInStock);
            System.Console.WriteLine("7 - Units On Order ({0})", product.UnitsOnOrder);
            System.Console.WriteLine("8 - Reorder Level ({0})", product.ReorderLevel);
            System.Console.WriteLine("9 - Discontinued ({0})", product.Discontinued ? "True":"False");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            System.Console.WriteLine("Or Enter '{0}' to finish editing", Controller.getEscapeValue());
            Console.ForegroundColor = ConsoleColor.White;
        }

        // CREATING A PRODUCT

        public static void addProdProductNamePrompt()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Enter The Product Name:");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void addProdSupplierIdPrompt()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            System.Console.WriteLine("Enter The Supplier ID:");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void addProdCategoryIdPrompt()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            System.Console.WriteLine("Enter The Category ID:");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void addProdQuantityPerUnitPrompt()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            System.Console.WriteLine("Enter The Quantity Per Unit:");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void addProdUnitPricePrompt()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            System.Console.WriteLine("Enter The Unit Price:");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void addProdUnitsInStockPrompt()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            System.Console.WriteLine("Enter The Number of Units In Stock:");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void addProdUnitsOnOrderPrompt()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            System.Console.WriteLine("Enter The Number of Units On Order:");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void addProdReorderLevelPrompt()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            System.Console.WriteLine("Enter The Reorder Level:");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void addProdDiscontinuedPrompt()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            System.Console.WriteLine("Is The Product Discontinued? (Y/N):");
            Console.ForegroundColor = ConsoleColor.White;
        }

        // VIEWING PRODUCTS

        public static void viewSpecificProductPrompt()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Select the Product which you want to display in full:");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void displaySpecificProduct(IEnumerable<Product> query)
        {
            foreach (var item in query)
            {
                if(item.Discontinued)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                else if(!item.Discontinued)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }                
                System.Console.WriteLine($"Product ID: {item.ProductId}");
                System.Console.WriteLine($"Product Name: {item.ProductName}");
                System.Console.WriteLine($"CategoryID: {item.CategoryId}");
                System.Console.WriteLine($"Category Name: {(Data.GetNorthwindContext().Categories.Where(c => c.CategoryId == item.CategoryId)).First().CategoryName }");
                System.Console.WriteLine($"SupplierID: {item.SupplierId}");
                System.Console.WriteLine($"Supplier Name: {(Data.GetNorthwindContext().Suppliers.Where(s => s.SupplierId == item.SupplierId)).First().CompanyName }");
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
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            System.Console.WriteLine("Please Select One Of The Following Options:");
            Console.ForegroundColor = ConsoleColor.Blue;
            System.Console.WriteLine("1. View All Products");
            Console.ForegroundColor = ConsoleColor.Cyan;
            System.Console.WriteLine("2. View Only Active (Not Discontinued) Products");
            Console.ForegroundColor = ConsoleColor.Gray;
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
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                else if(!item.Discontinued)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
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
            Console.ForegroundColor = ConsoleColor.Cyan;
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
            Console.ForegroundColor = ConsoleColor.Gray;
            foreach (var item in query)
            {
                Console.WriteLine($"{item.ProductName}");
            }           

            Console.ForegroundColor = ConsoleColor.White;
        }

                   
                    
                    
    }
}