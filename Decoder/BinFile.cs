

using System.IO;
using System.Text.RegularExpressions;

namespace Decoder
{
    public class BinFile 
    {
        public string content { get; set; }
        public string filteredContent { get; set; }
        public string filePath { get; set; }

        public void ReadFile(BinFile _m)
        {
            
            var content = string.Empty;
            var reader = new StreamReader(filePath);
            content = reader.ReadToEnd();
            reader.Close();

            string MatchPhrase = @"{1.*\n^{.Cmd.*$\n^{\d*,\d*},[\s|.|\S]+}.*$\n}.*$\n}";
            Match match = Regex.Match(content, MatchPhrase, RegexOptions.Multiline);

            if (match.Success)
            {
                _m.content = match.Value;

            }
        }

        public string CreateFilteredFile()
        {
            var filteredFile = filePath.Replace("bin", "txt"); 
            var writer = new StreamWriter(filteredFile);

            writer.Write(content);
            writer.Close();

            return filteredFile;

        }
    }

}