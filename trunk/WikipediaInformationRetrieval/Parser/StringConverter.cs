using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parser
{
    /// <summary>
    /// An abstract class defining StringConverter.
    /// Dereived class get list of strings, convert it and
    /// return converted list. An example of convertion may be
    /// normalizing each string in a list.
    /// </summary>
    public abstract class StringConverter
    {
        /// <summary>
        /// Convertes list of strings to another list.
        /// </summary>
        /// <param name="strings">List of strings to convert.</param>
        /// <returns>Converted list of strings.</returns>
        public virtual List<string> ConvertStrings(List<string> strings)
        {
            return strings;
        }
    }
}
