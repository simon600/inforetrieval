using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parser
{
    /// <summary>
    /// Normalizes strings.
    /// </summary>
    public class Normalizer
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Normalizer()
        {
            string str = ":-+=-_";

            remove_chars = str.ToCharArray();
        }

        /// <summary>
        /// Performs normalization.
        /// </summary>
        /// <param name="word">A string to normalize.</param>
        /// <returns>Normalized string.</returns>
        public string Normalize(string word)
        {
            string str = word;
            foreach (char c in remove_chars)
            {
                word = word.Replace(c.ToString(),"");
            }
            return word.ToLower();
        }

        private char[] remove_chars;
    }
}
