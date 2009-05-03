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
        /// Default constructor.
        /// </summary>
        public Tokenizer()
        {
            string str = " \t\n\r.,'\";:-_=+(){}[]!@#$%^&*|\\/><";
            trim_chars = str.ToCharArray();

            str = ".,;";

            split_chars = str.ToCharArray();
        }

        /// <summary>
        /// Converts list of strings into another list of strings (tokens).
        /// </summary>
        /// <param name="strings">List of strings to convert</param>
        /// <returns>Converted list of strings</returns>
        public List<string> ConvertStrings(List<string> strings)
        {
            List<string> list = new List<string>();         
            
            foreach (string word in strings)
            {                
                list.AddRange(word.Trim(trim_chars).Split(split_chars));
            }

            return list;
        }

        private char[] trim_chars;
        private char[] split_chars;
    }
}
