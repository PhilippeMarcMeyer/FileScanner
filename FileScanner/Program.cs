using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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

            string resFilePath = @"C:\Scanner\ResourcesTest.resx";
            string keysToDeleteFilePath = @"C:\Scanner\missingTest.txt";
            Utils.DeleteKeysInResourceFile(resFilePath, keysToDeleteFilePath);
        }

    }
}


