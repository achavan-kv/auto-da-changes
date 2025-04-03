using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MergeJsonToArray
{
    class Program
    {
        static void Main(string[] args)
        {
            var execFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Modules.FirstOrDefault().FullyQualifiedName);

            if (Directory.Exists(execFolder))
            {
                var dir = new DirectoryInfo(execFolder);
                var allJsonFiles = dir.GetFiles("*.json");
                var allJsonFileName = "all.json";

                var allJsonFileString = string.Empty;
                for (var i = 0; i < allJsonFiles.Length; i++)
                {
                    if (allJsonFiles[i].FullName.EndsWith(allJsonFileName))
                    {
                        continue;
                    }

                    var tmpFile = File.OpenText(allJsonFiles[i].FullName);
                    var tmpFileString = tmpFile.ReadToEnd();
                    tmpFile.Close();

                    tmpFileString = AddIndent(tmpFileString);
                    allJsonFileString += tmpFileString;
                    allJsonFileString += (i + 2 > allJsonFiles.Length) ? "\r\n" : ",\r\n";
                }

                var jsonMergeFile = new StreamWriter(Path.Combine(execFolder, "all.json"), false, Encoding.UTF8, 10000);
                jsonMergeFile.WriteLine("{\r\n  \"files\": [\r\n" + allJsonFileString + "  ]\r\n}");
                jsonMergeFile.Close();
            }
        }

        private static string AddIndent(string str)
        {
            var allLines = str.Split(new char[] { '\r', '\n' }, StringSplitOptions.None);

            var indent = "  ";
            var indentedStr = string.Empty;
            foreach (var l in allLines)
            {
                if (l.Trim().Length > 0)
                {
                    indentedStr += indent + l + "\r\n";
                }
                else
                {
                    indentedStr += "\r\n";
                }

            }

            return indentedStr;
        }

    }
}
