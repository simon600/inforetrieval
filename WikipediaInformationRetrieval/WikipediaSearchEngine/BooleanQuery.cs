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
        /// Compute union of postings list.
        /// Positions list are not relevant, in result posting list positions list are not sorted.
        /// </summary>
        /// <param name="postings">List of postings to merge</param>
        /// <returns>Union of postings</returns>
        protected override PositionalPostingList MergePostings(List<PositionalPostingList> postings)
        {
            if (postings.Count == 0)
                return new PositionalPostingList();

            if (postings.Count == 1)
                return postings[0];

            if (postings.Count == 2)
                return UnionPostings(postings[0], postings[1]);

            int[] sequence = DetermineSequence(postings);
            int ind = 0;

            List<PositionalPostingList> merge_postings = new List<PositionalPostingList>();
            PositionalPostingList new_posting;

            for (ind = 0; ind < postings.Count - 1; ind+= 2)
            {
                new_posting = UnionPostings( postings[ sequence[ind] ], postings[ sequence[ind+1] ]);  
                merge_postings.Add(new_posting);
            }

            if (postings.Count % 2 != 0)
                merge_postings.Add(postings[sequence[ind]]);

            while (merge_postings.Count > 1)
            {
                new_posting = UnionPostings(merge_postings[0], merge_postings[1]);
                merge_postings.RemoveRange(0, 2);
                merge_postings.Add(new_posting);
            }

            return merge_postings[0];
        }

        /// <summary>
        /// Compute intersection of postings list.
        /// Positions list are not relevant, in result posting list positions list are not sorted.
        /// </summary>
        /// <param name="postings">List of postings to intersect</param>
        /// <returns>Intersection of postings</returns>
        protected override PositionalPostingList GetPostingsProduct(List<PositionalPostingList> postings)
        {
            if (postings.Count == 0)
                return new PositionalPostingList();

            if (postings.Count == 1)
                return postings[0];

            int[] sequence = DetermineSequence(postings);

            PositionalPostingList new_posting = 
                ProductPostings(postings[ sequence[0] ], postings[ sequence[1] ]);
            
            int ind = 2;
            while( ind < sequence.Length )
            {
                new_posting = ProductPostings(new_posting, postings[ sequence[ind] ]);
                ind++;
            }
            return new_posting;
        }

        /// <summary>
        /// Union two posting list
        /// </summary>
        /// <param name="posting1">First posting list</param>
        /// <param name="posting2">Second posting list</param>
        /// <returns>Results of union</returns>
        private PositionalPostingList UnionPostings(PositionalPostingList posting1, PositionalPostingList posting2)
        {
            PositionalPostingList union_of_postings;
           
            List<uint> doc_ids = new List<uint>();
            List<uint[]> list_of_positions = new List<uint[]>();

            uint[] positions;

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
                    list_of_positions.Add(posting1.Positions[index1]);  //to nas nie obchodzi

                    index1++;
                }
                else if (key2 < key1)
                {
                    doc_ids.Add(key2);
                    list_of_positions.Add(posting2.Positions[index2]);  

                    index2++;
                }
                else
                {
                    doc_ids.Add(key1);
                    //niezachowana kolejnosc pozycji!!! to nas nie obchodzi
                    positions = posting1.Positions[index1].Concat(posting2.Positions[index2]).ToArray();

                    list_of_positions.Add(positions);

                    index1++;
                    index2++;
                }
            }

            while (index1 < size1)
            {
                doc_ids.Add(posting1.DocumentIds[index1]);
                list_of_positions.Add(posting1.Positions[index1]);

                index1++;
            }

            while (index2 < size2)
            {
                doc_ids.Add(posting2.DocumentIds[index2]);
                list_of_positions.Add(posting2.Positions[index2]);

                index2++;
            }

            //list_of_positions is not sorted 
            union_of_postings = new PositionalPostingList(doc_ids.ToArray(), list_of_positions.ToArray());
            return union_of_postings;
        }

        /// <summary>
        /// Compute intersection of two posting lists
        /// </summary>
        /// <param name="posting1">First posting list</param>
        /// <param name="posting2">Second posting list</param>
        /// <returns>Intersection of postings</returns>
        private PositionalPostingList ProductPostings(PositionalPostingList posting1, PositionalPostingList posting2)
        {
            PositionalPostingList product_of_postings;

            List<uint> doc_ids = new List<uint>();
            List<uint[]> list_of_positions = new List<uint[]>();

            uint[] positions;

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
                    //niezachowana kolejnosc pozycji!!!
                    positions = posting1.Positions[index1].Concat(posting2.Positions[index2]).ToArray();

                    list_of_positions.Add(positions);

                    index1++;
                    index2++;
                }
            }

            product_of_postings = new PositionalPostingList(doc_ids.ToArray(), list_of_positions.ToArray());
            return product_of_postings;
        }

    }
}
