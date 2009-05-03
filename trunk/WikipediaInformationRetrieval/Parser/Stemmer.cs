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

            foreach (string suffix in msRules)
            {
                if ( word.EndsWith(suffix) )
                {
                    stem = word.Substring(0, word.Length - suffix.Length);

                    if (IsNotProperStem(stem))
                        return word;
                    else return stem;
                }
            }

            if (msVowels.Contains<char>(word[word.Length - 1]))
            {
                stem2 = word.Substring(0, word.Length - 1);

                foreach (string ending in msEnding)
                {
                    if ( stem2.EndsWith(ending) )
                    {
                        stem = stem2.Substring(0, stem2.Length - ending.Length);

                        if (IsNotProperStem(stem))
                            return stem2;
                        else return stem;
                    }
                }

                return stem2;
            }

            return word;
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
