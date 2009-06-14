using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Morphologic;

namespace Parser
{
    /// <summary>
    /// Finds base forms for a word. Used for lematization.
    /// MorphologicDictionary needs to be initialized before usage
    /// of this class.
    /// </summary>
    public class Lematizer
    {        
        /// <summary>
        /// Finds base forms of a word. If cannot find any returns that word.
        /// </summary>
        /// <param name="word">A word to find base forms for.</param>
        /// <returns>A list of base forms or containing the word
        /// if no base forms found.</returns>
        public List<string> LematizeString(string word)
        {
            List<string> base_forms;            

            base_forms = MorphologicDictionary.Get()[word];

            if (base_forms.Count == 0)
            {
                base_forms.Add(word);
            }

            return base_forms;
        }
    }
}
