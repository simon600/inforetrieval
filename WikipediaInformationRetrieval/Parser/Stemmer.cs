using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parser
{
    /// <summary>
    /// Find the "root" of word using simple cut off rules.
    /// </summary>
    public class Stemmer
    {
        /// <summary>
        /// Cut off endings from word to get the "root".
        /// </summary>
        /// <param name="word">A word to find "root" for.</param>
        /// <returns>Root of a word or the same word if any of rules fit.</returns>
        public string DoStemming(string word)
        {
            string stem;
            string stem2;
          
            stem = word;
            //Step 1: cut off suffix according to rules
            //stem is a proper stem after that
            foreach (string suffix in msRules)
            {
                if (word.EndsWith(suffix))
                {
                    stem = word.Substring(0, word.Length - suffix.Length);

                    if (IsNotProperStem(stem))
                        stem = word;
                }
            }

            //Step 2: cut of last characters while it is a vowel
            //stem is a proper stem after that
            while (msVowels.Contains<char>(stem[stem.Length - 1]))
            {
                stem2 = stem.Substring(0, stem.Length - 1);
                if (IsNotProperStem(stem2))
                    break;

                else stem = stem2;
            }

            //Step 3: cut of endings
            foreach (string ending in msEnding)
            {
                if (stem.EndsWith(ending))
                {
                    stem2 = stem.Substring(0, stem.Length - ending.Length);

                    if (!IsNotProperStem(stem2))    //if stem2 is proper stem
                        stem = stem2;
                }
            }

            return stem;
        }

        private bool HasVowels(string word)
        {
            foreach(char v in msVowels)
                if( word.Contains(v) )
                    return true;

            return false;
        }


        private bool IsNotProperStem(string word)
        {
            if (word.Length < msMinLength)
                return true;

            foreach (char v in msVowels)
                if (word.Contains(v))
                    return false;

            return true;
        }
     
        public static char[] msVowels = { 'a', 'ą', 'e', 'ę', 'i', 'o', 'u', 'y'};
        public static string[] msRules = { "ów", "owej", "ie", "iej", "ego", "emu", "em", "iem",
                                             "om", "ami", "ach", "im", "imi", "ich", "ym", "ymi", "ych" };
        public static string[] msEnding = { "ówn", "ow" };
        public static short msMinLength = 3;
        
    }
}
