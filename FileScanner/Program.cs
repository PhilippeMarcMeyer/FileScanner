using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileScanner
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime start = DateTime.Now;
            string tag = string.Format("{0:s}", start);
            tag = tag.Replace("-", "");
            tag = tag.Replace(":", "");
            string logFolderName = @"c:\Scanner\";
            string resourcesToFindFile = logFolderName + "words.txt";
            string resultFile = logFolderName+"results_" + tag+".txt";

            string[] words = System.IO.File.ReadAllLines(resourcesToFindFile);
            foreach (string word in words)
            {
             
            }
        }


    }

    public static string SearchWord(string word)
    {
        string result;

        return result;
    }
}
