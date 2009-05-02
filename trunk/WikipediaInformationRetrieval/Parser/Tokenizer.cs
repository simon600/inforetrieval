using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Parser
{
    /// <summary>
    /// Class representing a tokenizer.
    /// It turns list of strings into tokens, that is 
    /// different list of strings.
    /// </summary>
    public class Tokenizer
    {
        /// <summary>
        /// Converts list of strings into another list of strings (tokens).
        /// </summary>
        /// <param name="strings">List of strings to convert</param>
        /// <returns>Converted list of strings</returns>
        public List<string> ConvertStrings(List<string> strings)
        {
            List<string> list = new List<string>();
            char[] trim_chars = { ' ', '\t', '\n', '\r', '.', ',', '(',
                                  ')',  '[',  ']', '"', '\'', '/', '\\'};

            foreach (string word in strings)
            {
                list.Add(word.Trim(trim_chars).ToLower());
            }

            return list;
        }
    }
}
