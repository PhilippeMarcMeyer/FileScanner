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
            string root = @"C:\Git\EconomikV2\PlatForm\FleetManagement\";
          //  string root = @"C:\Scanner\tests\";

            

           List <Pattern> patterns = new List<Pattern> {
                new Pattern // .aspx
                {
                    begin = "<%=Common.App_GlobalResources.Resources.",
                    end = " %>",
                    ext = ".aspx"
                },
                new Pattern // .aspx
                {
                    begin = "<%=Common.App_GlobalResources.Resources.",
                    end = "%>",
                    ext = ".aspx"
                },
                new Pattern // .aspx
                {
                    begin = "<%=Resources.",
                    end = " %>",
                    ext = ".aspx"
                },
                new Pattern // .aspx
                {
                    begin = "<%=Resources.",
                    end = "%>",
                    ext = ".aspx"
                },
                new Pattern // .asp
                {
                    begin = "<%=",
                    end = " %>",
                    ext = ".asp"
                },
                new Pattern // .asp
                {
                    begin = "<%=",
                    end = "%>",
                    ext = ".asp"
                },
                new Pattern // .cs
                {
                    begin = "Resources.",
                    end = ";",
                    ext = ".cs"
                },
                new Pattern // .cs
                {
                    begin = "Resources.",
                    end = ",",
                    ext = ".cs"
                },
                new Pattern // .cs
                {
                    begin = "Resources",
                    end = " ",
                    ext = ".cs"
                },
                new Pattern // .js
                {
                    begin = "resources.get('",
                    end = "')",
                    ext = ".js"
                },
                  new Pattern // .js
                {
                    begin = "resources.get(\"",
                    end = "\")",
                    ext = ".js"
                },
            };

            string[] extensions = new string[] { "aspx", "asp","cs","ascx","js"};

            List<Replacement> replacements = new List<Replacement>();

            DateTime start = DateTime.Now;
            string tag = string.Format("{0:s}", start);
            tag = tag.Replace("-", "");
            tag = tag.Replace(":", "");
            string logFolderName = @"c:\Scanner\";
            string resourcesToFindFile = logFolderName + "exchanges.txt";
            string resourcesToFindFileVerif = logFolderName + "exchangesVerif.txt";
            string resultFile = logFolderName+"results_" + tag+".txt";

            string[] words = System.IO.File.ReadAllLines(resourcesToFindFile);
            foreach (string word in words)
            {
                int position = word.IndexOf(";");
                if (position != -1)
                {
                    replacements.Add(new Replacement
                    {
                        replaced = word.Substring(0, position),
                        by = word.Substring(position+1)
                    });
                }
            }

            using (System.IO.StreamWriter filePtr = new System.IO.StreamWriter(resourcesToFindFileVerif))
            {
                foreach (Replacement replacement in replacements)
                {
                    string ouput = string.Format("{0};{1}", replacement.replaced, replacement.by);
                    filePtr.WriteLine(ouput);
                }
            }

            using (System.IO.StreamWriter filePtr = new System.IO.StreamWriter(resultFile))
            {

                    var files = Directory.GetFiles(root, "*.*", SearchOption.AllDirectories)
                          .Where(s => s.EndsWith(".aspx") || s.EndsWith(".asp") || s.EndsWith(".cs") || s.EndsWith(".ascx") || s.EndsWith(".js"));

                    long nrFiles = files.LongCount();
                    int i = 0;
                    foreach (string file in files)
                    {
                        bool fileModified = false;
                        // filePtr.WriteLine(file);
                        i++;
                        string message = string.Format("{0}/{1} => {2}", i, nrFiles, file);
                        Console.WriteLine("reading file : " + message);
                        string[] lines = System.IO.File.ReadAllLines(file);
                    string ext = Path.GetExtension(file);

                    List<Pattern> pattersForExt = patterns.Where(x => x.ext == ext).ToList();
                    for (int lineOffset = 0; lineOffset < lines.Length; lineOffset++)
                        {
                            var line = lines[lineOffset];
                            bool found = false;
                            foreach (Pattern pattern in pattersForExt)
                            {
                                foreach (Replacement replacement in replacements)
                                {
                                    string oldString = pattern.begin + replacement.replaced + pattern.end;
                                    string newString = pattern.begin + replacement.by + pattern.end;
                                    string testLine = line;
                                    string oldStringTest = oldString;
                                    string newStringTest = newString;
                                    if (file.EndsWith("asp"))
                                    {
                                        testLine = testLine.ToUpper();
                                        oldStringTest = oldStringTest.ToUpper();
                                        newStringTest = newStringTest.ToUpper();
                                    }
                                    int position = testLine.IndexOf(oldStringTest);
                                    if (position != -1)
                                    {
                                        string startLine = "";
                                        if (position > 0)
                                        {
                                            startLine = line.Substring(0, position);

                                        }
                                        string endLine = line.Substring(position + oldString.Length);
                                        var prev = line;
                                        line = startLine + newString + endLine;

                                        filePtr.WriteLine(file);
                                        filePtr.WriteLine(prev);
                                        filePtr.WriteLine(line);
                                        filePtr.WriteLine("");
                                        found = true;
                                        fileModified = true;

                                    Console.WriteLine("found : " + oldString);
                                }

                                }
                            }

                            if (found)
                            {
                                lines[lineOffset] = line;
                            }
                        }

                       if(fileModified) {

                            Encoding enc = Utils.GetEncoding(file);
                            File.WriteAllLines(file, lines, enc);

                             Console.WriteLine("modified : " + file);
                    }

                    }

            }
        }
    }

   public class Pattern
    {
        public string begin { get; set; }
        public string end { get; set; }
        public string ext { get; set; }
    }

    public class Replacement
    {
        public string replaced { get; set; }
        public string by { get; set; }

    }


}


