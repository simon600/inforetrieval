using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MorphologicDictionary
{
    /// <summary>
    /// A class representing morhpologic dictionary.
    /// Allows getting base forms of words.
    /// </summary>
    public class MorphologicDictionary
    {
        /// <summary>
        /// Gets the only instance of MorphologicDictionary.
        /// </summary>
        /// <returns>An instance of MorphologicDictionary</returns>
        static public MorphologicDictionary Get()
        {
            if (mInstance == null)
            {
                mInstance = new MorphologicDictionary();
            }
            return mInstance;
        }

        /// <summary>
        /// Reads morhpologic dictionary form file.
        /// </summary>
        /// <param name="filename">Name of the file in which morphologic
        /// dictionary is stored.</param>
        public void ReadFromFile(string filename)
        {
            if (!File.Exists(filename))
            {
                throw new IOException("No file named " + filename + " found.");
            }

            StreamReader reader = new StreamReader(filename);
            string[] line;

            while (!reader.EndOfStream)
            {
                line = reader.ReadLine().Split(' ');
                if (line.Length < 2)
                {
                    throw new Exception("Wrong file format.");
                }                                
                for (int i = 1; i < line.Length; i++)
                {
                    if (!mDictionary.ContainsKey(line[i]))
                    {
                        // create new list of base forms for a word
                        mDictionary[line[i]] = new List<string>();
                    }
                    mDictionary[line[i]].Add(line[0]); // add base form
                }                
            }

            reader.Close();
        }

        /// <summary>
        /// Gets list of base forms of a word.
        /// </summary>
        /// <param name="word">Word, which base forms are to be found.</param>
        /// <returns>List of base forms of a word if found, null
        /// otherwise.</returns>
        public List<string> this[string word]
        {
            get
            {
                if (mDictionary.ContainsKey(word))
                {                    
                    return mDictionary[word];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Private default constructor.
        /// </summary>
        private MorphologicDictionary()
        {
            mDictionary = new Dictionary<string, List<string>>();
        }

        /// <summary>
        /// Private copy constructor.
        /// </summary>
        /// <param name="from"></param>
        private MorphologicDictionary(MorphologicDictionary from)
        {
        }

        private static MorphologicDictionary mInstance;
        private Dictionary<string, List<string>> mDictionary;
    }
}
