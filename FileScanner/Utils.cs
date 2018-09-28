using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace FileScanner
{
    public class Utils
    {

        public static Encoding GetEncoding(string filename)
        {
            // This is a direct quote from MSDN:  
            // The CurrentEncoding value can be different after the first
            // call to any Read method of StreamReader, since encoding
            // autodetection is not done until the first call to a Read method.

            using (var reader = new StreamReader(filename, Encoding.Default, true))
            {
                if (reader.Peek() >= 0) // you need this!
                    reader.Read();

                return reader.CurrentEncoding;
            }
        }

        public static void DeleteKeysInResourceFile(string resFilePath,string keysToDeleteFilePath)
        {


            string logFolderName = @"c:\Scanner\";
            string logFile = logFolderName + GetHorodatedFile("deleteLogger_", "txt");
            string resultFile = logFolderName + GetHorodatedFile("result_", "resx");

            StreamWriter logPtr = new StreamWriter(logFile);

            string[] keysToDelete = File.ReadAllLines(keysToDeleteFilePath);

            XDocument originalFile = null;
            try
            {
                originalFile = XDocument.Load(resFilePath);
                List<XElement> datas = originalFile.Root.Elements("data").ToList();
                if (datas != null)
                {
                    foreach (XElement item in datas)
                    {
                        XName xName = null;
                        try
                        {
                            xName = item.Attribute("name").Value;
                            string key = xName.ToString();
                            if (Array.IndexOf(keysToDelete, key) > -1)
                            {
                                item.Remove();
                                logPtr.WriteLine(xName.ToString());
                                Console.WriteLine(xName.ToString());
                            }
                        }
                        catch (XmlException ex)
                        {
                            Console.Error.WriteLine(
                                string.Format("Error : ({0})", ex.ToString())
                            );
                            logPtr.WriteLine("Error : ({0})", ex.ToString());
                        }
                    }
                    // Saving the result

                    XDocument result = new XDocument(
                        new XElement(originalFile.Root.Name,
                            from comment in originalFile.Root.Nodes() where comment.NodeType == XmlNodeType.Comment select comment,
                            from schema in originalFile.Root.Elements() where schema.Name.LocalName == "schema" select schema,
                            from resheader in originalFile.Root.Elements("resheader") orderby (string)resheader.Attribute("name") select resheader,
                            from assembly in originalFile.Root.Elements("assembly") orderby (string)assembly.Attribute("name") select assembly,
                            from metadata in originalFile.Root.Elements("metadata") orderby (string)metadata.Attribute("name") select metadata,
                            from data in datas orderby (string)data.Attribute("name") select data
                        )
                    );

                    try
                    {
                        result.Save(resultFile);
                    }
                    catch
                    {
                        Console.Error.WriteLine(
                            string.Format("An error occured while writing the file ({0})", resultFile)
                        );
                    }

                }
            }
            catch (FileNotFoundException ex)
            {
                Console.Error.WriteLine("File not found ({0})", resFilePath);
            }
            catch (XmlException ex)
            {
                Console.Error.WriteLine(
                    string.Format("The file does not contain valid XML ({0})", resFilePath)
                );
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    string.Format("An error occured while reading the file ({0}) : {1}", resFilePath,ex.ToString())
                );
            }

           // 


           // StreamWriter resultPtr = new StreamWriter(resultFile);
 
           // originalFile.Save(resultPtr, SaveOptions.None);

            ConsoleKeyInfo info = Console.ReadKey();
        }

        public static string GetHorodatedFile(string shortFileName,string extension)
        {
            DateTime start = DateTime.Now;
            string horodatedFile = string.Format("{0:s}", start);
            horodatedFile = horodatedFile.Replace("-", "").Replace(":", "");
            extension = extension.Replace(".", "");
            horodatedFile = shortFileName + horodatedFile + "." + extension;
            return horodatedFile;
        }

        public static void ScanAndReplace(string root, string app, List<Pattern> patterns, string[] extensions)
        {

            List<Replacement> replacements = new List<Replacement>();
   
            string logFolderName = @"c:\Scanner\";
            string resourcesToFindFile = logFolderName + "exchanges.txt";
            string consoleLog = logFolderName + "console_" + app + ".txt";
            string resultFile = logFolderName + GetHorodatedFile("results_","txt");

            string[] words = System.IO.File.ReadAllLines(resourcesToFindFile);
            foreach (string word in words)
            {
                int position = word.IndexOf(";");
                if (position != -1)
                {
                    replacements.Add(new Replacement
                    {
                        replaced = word.Substring(0, position),
                        by = word.Substring(position + 1)
                    });
                }
            }

            StreamWriter consolePtr = new StreamWriter(consoleLog);


            Encoding utf8 = Encoding.UTF8;
            Encoding utf16 = Encoding.Unicode;
            Encoding ASCII = Encoding.ASCII;

            using (System.IO.StreamWriter filePtr = new System.IO.StreamWriter(resultFile))
            {

                var files = Directory.GetFiles(root, "*.*", SearchOption.AllDirectories)
                      .Where(s => s.EndsWith(".aspx") || s.EndsWith(".asp") || s.EndsWith(".cs") || s.EndsWith(".ascx") || s.EndsWith(".js"));

                long nrFiles = files.LongCount();

                filePtr.WriteLine(root + " : " + nrFiles.ToString() + " files");

                int i = 0;
                foreach (string file in files)
                {
                    bool fileModified = false;
                    // filePtr.WriteLine(file);
                    i++;
                    string message = string.Format("{0}/{1} => {2}", i, nrFiles, file);
                    Console.WriteLine("reading file : " + message);
                    consolePtr.WriteLine("reading file : " + message);
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
                                    consolePtr.WriteLine("found : " + oldString);
                                }

                            }
                        }

                        if (found)
                        {
                            lines[lineOffset] = line;
                        }
                    }

                    if (fileModified)
                    {
                        string info = "?";
                        Encoding enc = Utils.GetEncoding(file);
                        File.WriteAllLines(file, lines, enc);
                        if (enc.Equals(utf8))
                        {
                            info = "utf8";
                        }
                        else if (enc.Equals(utf16))
                        {
                            info = "utf16";

                        }
                        else if (enc.Equals(ASCII))
                        {
                            info = "ASCII";
                        }

                        Console.WriteLine("modified : " + file + " encode : " + info);
                        consolePtr.WriteLine("modified : " + file + " encode : " + info);
                    }
                }
            }
        }

        public static void CheckIfExist(string root, string app, List<Pattern> patterns)
        {

            List<Detection> detections = new List<Detection>();
            List<Detection> notFound = new List<Detection>();
    
            string logFolderName = @"c:\Scanner\";

            string resourcesToFindFile = logFolderName + "keys.txt";


            string LogFile = logFolderName + GetHorodatedFile("check_" + app + "_", "txt");
            string resultFile = logFolderName + GetHorodatedFile("missing_" + app + "_", "txt");

            string[] keys = System.IO.File.ReadAllLines(resourcesToFindFile);
            foreach (string key in keys)
            {
                detections.Add(new Detection
                {
                    Key = key,
                    Found = false,
                    Filename = "",
                    Line = 0
                });
            }
            StreamWriter missingsPtr = new System.IO.StreamWriter(resultFile);

            using (System.IO.StreamWriter filePtr = new System.IO.StreamWriter(LogFile))
            {

                var files = Directory.GetFiles(root, "*.*", SearchOption.AllDirectories)
                      .Where(s => s.EndsWith(".aspx") || s.EndsWith(".asp") || s.EndsWith(".cs") || s.EndsWith(".ascx") || s.EndsWith(".js"));

                long nrFiles = files.LongCount();

                filePtr.WriteLine(root + " : " + nrFiles.ToString() + " files");

                int i = 0;
                foreach (string file in files)
                {
                    i++;
                    string message = string.Format("{0}/{1} => {2}", i, nrFiles, file);
                    Console.WriteLine("reading file : " + message);
                    filePtr.WriteLine("reading file : " + message);
                    string[] lines = System.IO.File.ReadAllLines(file);
                    string ext = Path.GetExtension(file);
                    List<Pattern> pattersForExt = patterns.Where(x => x.ext == ext).ToList();
                    for (int lineOffset = 0; lineOffset < lines.Length; lineOffset++)
                    {
                        var line = lines[lineOffset];
                        foreach (Pattern pattern in pattersForExt)
                        {
                            foreach (Detection detection in detections)
                            {
                                if (!detection.Found)
                                {
                                    string keyToFind = pattern.begin + detection.Key + pattern.end;
                                    string testLine = line;
                                    string testkeyToFind = keyToFind;

                                    if (file.EndsWith(".asp"))
                                    {
                                        testLine = testLine.ToUpper();
                                        testkeyToFind = testkeyToFind.ToUpper();
                                    }
                                    int position = testLine.IndexOf(testkeyToFind);
                                    if (position != -1)
                                    {
                                        detection.Found = true;
                                        detection.Line = lineOffset + 1;
                                        detection.Filename = file;

                                        Console.WriteLine(keyToFind + ";" + detection.Line + " ;" + detection.Filename);
                                        filePtr.WriteLine(keyToFind + ";" + detection.Line + " ;" + detection.Filename);
                                    }
                                }
                            }
                        }

                    }

                }
            }
            notFound = detections.Where(x => x.Found == false).ToList();
            if(notFound != null)
            {
                foreach (Detection missing in notFound)
                {
                    missingsPtr.WriteLine(missing.Key);
                }
            }

        }

        public static List<Pattern> GetPatterns()
    {

        List<Pattern> patterns = new List<Pattern> {
                new Pattern // .aspx
                {
                    begin = "<% =Common.App_GlobalResources.Resources.",
                    end = " %>",
                    ext = ".aspx"
                },
                new Pattern // .aspx
                {
                    begin = "<% =Common.App_GlobalResources.Resources.",
                    end = "%>",
                    ext = ".aspx"
                },
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
                    begin = "Resources.",
                    end = ")",
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
        return patterns;
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

public class Detection
{
    public string Key { get; set; }
    public bool Found { get; set; }
    public string Filename { get; set; }
    public int Line { get; set; }
}
