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
    public class Tokenizer : StringConverter
    {
        /// <summary>
        /// Converts list of strings into another list of strings (tokens).
        /// </summary>
        /// <param name="strings">List of strings to convert</param>
        /// <returns>Converted list of strings</returns>
        public override List<string> ConvertStrings(List<string> strings)
        {
            return strings;
        }
    }
}
