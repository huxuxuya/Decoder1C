using Elisy.MdInternals.Cil;
using Elisy.MdInternals.CfProject;
using Elisy.MdInternals.Cf;
using System;
using System.Collections.Generic;
using System.IO;
using Elisy.MdInternals;
using Decoder;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;

namespace Decoder
{ 
internal class Program
{
        private static void Main(string[] args)
        {


            var fileMask = @"Module.bin";
            var fileArray = ExpandFilePaths(args, fileMask);
            var DeleteBin = false;
            var DeleteTxt = true;

            foreach (var filePath in fileArray)
            {

                BinFile binFile = new BinFile();
                binFile.filePath = filePath;
                binFile.ReadFile(binFile);
                binFile.CreateFilteredFile(binFile);


                Console.WriteLine(filePath);
                try
                {
                    var bslFile = filePath.Replace("bin", "bsl");
                    DecompileFile(binFile.filteredFilePath, bslFile);

                    if (DeleteBin == true) { System.IO.File.Delete(binFile.filePath); }
                    if (DeleteTxt == true) { System.IO.File.Delete(binFile.filteredFilePath); }
 
                    var reader = new StreamReader(bslFile);
                    var content = reader.ReadToEnd();
                    reader.Close();
                    content = Regex.Replace(content, @"[\r\n\n]{2,}", "\r\n", RegexOptions.Multiline);
                    content = Regex.Replace(content, @"\nПроцедура", "\n\nПроцедура", RegexOptions.Multiline);
                    content = Regex.Replace(content, @"\nФункция", "\n\nФункция", RegexOptions.Multiline);

                    var writer = new StreamWriter(bslFile);
                    writer.Write(content);
                    writer.Close();

                }
                catch
                {
                    Console.WriteLine(filePath + " - ERROR");
                }

            }
        }

    public static string[] ExpandFilePaths(string[] args, string filePart)
    {
        var fileList = new List<string>();

        foreach (var arg in args)
        {
            Console.WriteLine(" arg - " + arg);

            var substitutedArg = System.Environment.ExpandEnvironmentVariables(arg);

            var dirPart = Path.GetDirectoryName(substitutedArg);
            if (dirPart.Length == 0)
                dirPart = ".";

                //var filePart = Path.GetFileName(substitutedArg);
                Console.WriteLine(" find " + filePart + " in " + dirPart);

                foreach (var filepath in Directory.GetFiles(dirPart, filePart, SearchOption.AllDirectories))
                fileList.Add(filepath);
        }

        return fileList.ToArray();
    }

    private static void DecompileFile(string filePathIn, string filePathOut)
    {
        string opCodeString = System.IO.File.ReadAllText(filePathIn);
        CodeReader reader = new CodeReader(opCodeString, false);
        string decompiledString = reader.GetSourceCode();
        using (StreamWriter outfile = new StreamWriter(filePathOut))
        {
            outfile.Write(decompiledString);
        }
    }
}
}