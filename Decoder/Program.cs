using Elisy.MdInternals.Cil;
using Elisy.MdInternals.CfProject;
using Elisy.MdInternals.Cf;
using System;
using System.Collections.Generic;
using System.IO;
using Elisy.MdInternals;
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

        foreach (var filePath in fileArray)
        {
            Console.WriteLine(filePath);

            var filteredFile = filePath.Replace("bin", "txt");
            var content = string.Empty;
            var reader = new StreamReader(filePath);
            content = reader.ReadToEnd();
            reader.Close();

             

            string MatchPhrase = @"{1.*\n^{.Cmd.*$\n^{\d*,\d*},[\s|.|\S]+}.*$\n}.*$\n}";
            Match match = Regex.Match(content, MatchPhrase, RegexOptions.Multiline);
            if (match.Success)
            {
                content = match.Value;
                
                var writer = new StreamWriter(filteredFile);
                writer.Write(content);
                writer.Close();

                try
                {
                   
                    Console.WriteLine(filePath + " - Done");
                    var bslFile = filePath.Replace("bin", "bsl");

                    DecompileFile(filteredFile, bslFile);
                    }
                catch
                {
                    Console.WriteLine(filePath + " - ERROR");
                }

                }



            else
            {
                Console.WriteLine(filePath + " - False");
            }


            }

    }

    public static string[] ExpandFilePaths(string[] args, string filePart)
    {
        var fileList = new List<string>();

        foreach (var arg in args)
        {
            var substitutedArg = System.Environment.ExpandEnvironmentVariables(arg);

            var dirPart = Path.GetDirectoryName(substitutedArg);
            if (dirPart.Length == 0)
                dirPart = ".";

            //var filePart = Path.GetFileName(substitutedArg);

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