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
    class Program
    {
        static void Main(string[] args)
        {
            /**
            Using the Northwind database, create a menu driven UI console application that allows maintenance of selected data tables.

            “C” (405):
                [X]1.Add a new product
                [X]2.Edit a specified Product
                [X]3.Display all Products (ProductName only) - user decides:
                    [X]-all
                    [X]-discontinued
                    [X]-active (not discontinued)
                [X]Discontinued products should be distinguished from active products. (Change Text Color?)
                [X]4.Display a specific Product (all product fields should be displayed)
                [ ]5.Use NLog to track user functions
            “B” (445):
                [X]1.Add new Category
                [X]2.Edit a specified Category
                [X]3.Display all Categories (CategoryName and Description)
                [X]4.Display all Categories and their (not discontinued) product data (CategoryName, ProductName)
                [X]5.Display a specific Category and its active product data (CategoryName, ProductName)
            “A” (475):
                [ ]1.Delete a specified existing Product  (account for Orphans)
                [ ]2.Delete a specified existing Category (account for Orphans)
                [ ]3.Use data annotations and handle ALL user errors gracefully & log all errors using NLog
            500 points:
                [ ]*.your application must do something exceptional, something we have not covered in class.

            [ ]The full functionality of your project must be demonstrated with a video. 
            [ ]A link to the video must be submitted. 
            [ ]A link to your Github repo must be submitted.

            Any feature not demonstrated in the video will not be applied to the project grade.

            */

            // TODO: MAKE ALL ''SELECTED'' FUNCTIONS THE SAME
            Console.Clear();
            Console.WriteLine("");

            // dotnet ef dbcontext scaffold "Server=bitsql.wctc.edu;Database=Northwind_88_EH;User ID=YYY;Password=ZZZ" Microsoft.EntityFrameworkCore.SqlServer -o Model

            Data.getLogger().Info("NLOG Loaded");

            Controller.mainLoop();
            
            Data.getLogger().Info("Program Ended");
        }
    }
}
