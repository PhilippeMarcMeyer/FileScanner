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

            //string resFilePath = @"C:\Scanner\resources2.resx";
            //string keysToDeleteFilePath = @"C:\Scanner\missing__20180927T113008.txt";
            //Utils.DeleteKeysInResourceFile(resFilePath, keysToDeleteFilePath);

            string resFilePath = @"C:\Scanner\resources2.resx";
            string keysToExtractFilePath = @"C:\Scanner\enum_reintegrer.txt";

            Utils.ExtractKeysInResourceFile(resFilePath, keysToExtractFilePath);

            //string resFilePath = @"C:\Scanner\tests\extract_20181002T124835.xml";
            //string keysToDeleteFilePath = @"C:\Scanner\resall.txt";
            //Utils.DeleteKeysInResourceFile(resFilePath, keysToDeleteFilePath);

        }

    }
}


