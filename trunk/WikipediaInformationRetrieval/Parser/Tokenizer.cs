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
            //string str = " \t\n\r.,'\";:-_=+(){}[]!@#$%^&*|\\/><";
            //trim_chars = str.ToCharArray();
            string str = ".,;_";

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
            string modified_word;
            
            foreach (string word in strings)
            {
                modified_word = TrimWord(word);
                if (modified_word.Length > 0)
                {
                    list.AddRange(modified_word.Split(split_chars));
                }
            }

            return list;
        }

        private string TrimWord(string word)
        {
            bool contains_bad_char = true;
            while (contains_bad_char && word.Length > 0)
            {
                contains_bad_char = false;
                if (!char.IsLetterOrDigit(word[0]))
                {
                    word = word.Remove(0, 1);
                    contains_bad_char = true;
                }
                else if (!char.IsLetterOrDigit(word[word.Length - 1]))
                {
                    word = word.Remove(word.Length - 1, 1);
                    contains_bad_char = true;
                }
            }

            return word;
        }
        
        private char[] split_chars;
    }
}
