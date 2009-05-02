using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Morphologic
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

            FileStream fstream = new FileStream(filename, FileMode.Open);
            BinaryReader reader = new BinaryReader(fstream);
            int length;
            int entries; // number of entries in dictionary

            length = reader.ReadInt32();

            mBaseWords = new string[length];

            for (int i = 0; i < length; i++)
            {
                mBaseWords[i] = reader.ReadString();
            }

            entries = reader.ReadInt32();
            mWords = new string[entries];
            mBaseFormNumbers = new uint[entries][];

            for (int entry_num = 0; entry_num < entries; entry_num++)
            {
                mWords[entry_num] = reader.ReadString();
                length = reader.ReadInt32();
                mBaseFormNumbers[entry_num] = new uint[length];
                for (int i = 0; i < length; i++)
                {
                    mBaseFormNumbers[entry_num][i] = (uint)reader.ReadInt32();                    
                }                
            }

            reader.Close();
        }

        /// <summary>
        /// Gets list of base forms of a word.
        /// 
        /// REQUIRES Dictionary must have already been read from file.
        /// </summary>
        /// <param name="word">Word, which base forms are to be found.</param>
        /// <returns>List of base forms of a word if found, null
        /// otherwise.</returns>
        public List<string> this[string word]
        {
            get
            {
                List<string> list = new List<string>();
                int index = BinarySearch(word);
                if (index > 0)
                {
                    foreach (uint base_form_num in mBaseFormNumbers[index])
                    {
                        list.Add(mBaseWords[base_form_num]);
                    }
                }
                return list;
            }
        }

        /// <summary>
        /// Private default constructor.
        /// </summary>
        private MorphologicDictionary()
        {
            mWords = null;
            mBaseFormNumbers = null;
            mBaseWords = null;
        }

        /// <summary>
        /// Binary search for a word in mWords array.
        /// </summary>
        /// <param name="word">Word to find.</param>
        /// <returns>Index of found word, or -1 if not found.</returns>
        private int BinarySearch(string word)
        {
            CaseInsensitiveComparer comparer = new CaseInsensitiveComparer();

            int begining = 0;
            int end = mWords.Length-1;
            int middle;
            int cmp;

            while (end > begining + 1)
            {
                middle = (end + begining) / 2;

                cmp = comparer.Compare(word, mWords[middle]);

                if (cmp < 0)
                {
                    end = middle;
                }
                else if (cmp > 0)
                {
                    begining = middle;
                }
                else
                {
                    return middle;
                }
            }

            if (word == mWords[end])
            {
                return end;
            }

            return -1;
        }

        private static MorphologicDictionary mInstance;
        private string[] mWords;
        private uint[][] mBaseFormNumbers;        
        private string[] mBaseWords;
    }
}
