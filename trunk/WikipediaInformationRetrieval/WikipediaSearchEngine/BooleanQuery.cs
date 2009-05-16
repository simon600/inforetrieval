using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InversedIndex;

namespace WikipediaSearchEngine
{
    public class BooleanQuery : Query
    {
        public BooleanQuery(string query)
            :base(query)
        {}

        /// <summary>
        /// Normal form for boolean query:
        /// word1 or word2 = word1|word2  ("|" is separator)
        /// word1 and word2 = word1 word2 (" " is separator)
        /// Words in disjunction are sorted and parts of conjunction also.
        /// </summary>
        public override string QueryNormalForm
        {
            get
            {
                if (mNormalizedQuery == null)
                {
                    List<string> sorted_clause = new List<string>();
                   
                    string clause;
                    foreach (List<string> or_list in mQueryStructure)
                    {
                        if (or_list.Count == 0)     //tu bylo stop words
                            continue;

                        clause = "";
                        or_list.Sort();
                        foreach (string w in or_list)
                            clause += w + "|";

                        clause = clause.Remove(clause.Length - 1);
                        sorted_clause.Add(clause);
                    }

                    mNormalizedQuery = "";

                    if (sorted_clause.Count == 0)
                        return mNormalizedQuery;

                            
                    sorted_clause.Sort();
                    
                    foreach (string w in sorted_clause)
                        mNormalizedQuery += w + " ";

                    mNormalizedQuery = mNormalizedQuery.Remove(mNormalizedQuery.Length - 1);
                }

                return mNormalizedQuery;
            }
        }

        /// <summary>
        /// Finishing building mQueryStructure for query
        /// </summary>
        protected override void FinalizeParsing()
        {
            //usuniecie list zawierajacych stop words
            int i = 0;
            while (i < mQueryStructure.Count)
            {
                if (mQueryStructure[i].Count == 0)
                    mQueryStructure.RemoveAt(i);
                else i++;
            }
        }

        /// <summary>
        /// Union two posting list
        /// Positions list are not relevant, in result posting list positions are empty.
        /// </summary>
        /// <param name="posting1">First posting list</param>
        /// <param name="posting2">Second posting list</param>
        /// <returns>Results of union</returns>
        protected override PositionalPostingList UnionPostings(PositionalPostingList posting1, PositionalPostingList posting2)
        {
            PositionalPostingList union_of_postings;
           
            List<uint> doc_ids = new List<uint>();
          
            int index1 = 0;
            int index2 = 0;

            int size1 = posting1.DocumentIds.Length;
            int size2 = posting2.DocumentIds.Length;

            uint key1;
            uint key2;

            while (index1 < size1 && index2 < size2)
            {
                key1 = posting1.DocumentIds[index1];
                key2 = posting2.DocumentIds[index2];

                if (key1 < key2)
                {
                    doc_ids.Add(key1);
                  
                    index1++;
                }
                else if (key2 < key1)
                {
                    doc_ids.Add(key2);
                 
                    index2++;
                }
                else
                {
                    doc_ids.Add(key1);
                   
                    index1++;
                    index2++;
                }
            }

            while (index1 < size1)
            {
                doc_ids.Add(posting1.DocumentIds[index1]);
                index1++;
            }

            while (index2 < size2)
            {
                doc_ids.Add(posting2.DocumentIds[index2]);
                index2++;
            }

            //list_of_positions is not sorted 
            union_of_postings = new PositionalPostingList(doc_ids.ToArray(), new ushort[0][]);
            return union_of_postings;
        }

        /// <summary>
        /// Compute intersection of two posting lists
        /// Positions list are not relevant, in result posting list positions are empty.
        /// </summary>
        /// <param name="posting1">First posting list</param>
        /// <param name="posting2">Second posting list</param>
        /// <returns>Intersection of postings</returns>
        protected override PositionalPostingList IntersectPostings(PositionalPostingList posting1, PositionalPostingList posting2)
        {
            PositionalPostingList product_of_postings;

            List<uint> doc_ids = new List<uint>();
         
            int index1 = 0;
            int index2 = 0;

            int size1 = posting1.DocumentIds.Length;
            int size2 = posting2.DocumentIds.Length;

            uint key1;
            uint key2;

            while (index1 < size1 && index2 < size2)
            {
                key1 = posting1.DocumentIds[index1];
                key2 = posting2.DocumentIds[index2];

                if (key1 < key2)
                    index1++;

                else if (key2 < key1)
                    index2++;

                else
                {
                    doc_ids.Add(key1);
                    
                    index1++;
                    index2++;
                }
            }

            product_of_postings = new PositionalPostingList(doc_ids.ToArray(), new ushort[0][]);
            return product_of_postings;
        }

    }
}
