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
            string str = ".,;|_()[]{}";

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
                foreach (string str in word.Split(split_chars))
                {
                    modified_word = TrimWord(str);
                    if (modified_word.Length > 0 &&
                        ContainsLetters(modified_word))
                    {
                        list.Add(modified_word);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// Trims word.
        /// </summary>
        /// <param name="word">Word to trim.</param>
        /// <returns>Modiefied word.</returns>
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

        /// <summary>
        /// Checks if word contains letters.
        /// </summary>
        /// <param name="word">Word to check.</param>
        /// <returns>True if has letters, false otherwise.</returns>
        private bool ContainsLetters(string word)
        {
            bool has_letters = false;
            foreach (char c in word)
            {
                if (char.IsLetter(c))
                {
                    has_letters = true;
                    break;
                }
            }

            return has_letters;
        }
        
        private char[] split_chars;
    }
}
