using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FileScanner
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            string app = "";
            string root = @"C:\Git\EconomikV2\PlatForm\";
            if(app != "")
            {
                root += app + "\\";

            }
            string[] extensions = new string[] { "aspx", "asp", "cs", "ascx", "js" };

            List<Pattern> patterns = Utils.GetPatterns();

            // Utils.ScanAndReplace(root, app, patterns, extensions);

            // Utils.CheckIfExist(root, app, patterns);
            */
            //Console.WriteLine( Utils.GetHorodatedFile("myFile", "xml"));
         
            string resFilePath = @"D:\_Developpement\GITHUB\FileScanner\tests\ResourcesTest.resx";
            string keysToDeleteFilePath = @"D:\_Developpement\GITHUB\FileScanner\tests\missingTest.txt";
            Utils.DeleteKeysInResourceFile(resFilePath, keysToDeleteFilePath);
          


        }

    }
}


