using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using NLog.Web;

namespace NorthwindConsole.Model
{  
    public static class Data
    {
        //////////////////////////////
        //      NLOG Instantiation  //
        //////////////////////////////

        // static instance of logger held so multiple instances don't have to be created
        private static NLog.Logger logger;
        private static Northwind_88_EHContext db;

        public static NLog.Logger getLogger()
        {
            if(logger == null)
            {
               // create instance of Logger
                logger = NLogBuilder.ConfigureNLog(Directory.GetCurrentDirectory() + "\\nlog.config").GetCurrentClassLogger();
            }
            return logger;
        }       

        public static Northwind_88_EHContext GetNorthwindContext()
        {
            return new Northwind_88_EHContext();
        }

    }
}