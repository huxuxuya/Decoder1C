

using System.IO;
using System.Text.RegularExpressions;

namespace Decoder
{
    public class BinFile 
    {
        public string content { get; set; }
        public string filteredContent { get; set; }
        public string filePath { get; set; }
        public string filteredFilePath { get; set; }

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

        public void CreateFilteredFile(BinFile _m)
        {
            _m.filteredFilePath = filePath.Replace("bin", "txt"); 
            var writer = new StreamWriter(_m.filteredFilePath);

            writer.Write(content);
            writer.Close();

        }
    }

}