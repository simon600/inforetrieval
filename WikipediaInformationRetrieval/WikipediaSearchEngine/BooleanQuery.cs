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

        public override string QueryNormalForm
        {
            get
            {
                if (mNormalizedQuery == null)
                {
                    List<string> sorted_clause = new List<string>();
                   
                    string clause = "";
                    foreach (List<string> or_list in mQueryStructure)
                    {
                        clause = "";
                        or_list.Sort();
                        foreach (string w in or_list)
                            clause += w + "|";

                        clause = clause.Remove(clause.Length - 1);
                        sorted_clause.Add(clause);
                    }

                    sorted_clause.Sort();
                    mNormalizedQuery = "";
                    foreach (string w in sorted_clause)
                        mNormalizedQuery += w + " ";

                    mNormalizedQuery = mNormalizedQuery.Remove(mNormalizedQuery.Length - 1);
                }

                return mNormalizedQuery;
            }
        }
       

        protected override PositionalPostingList MergePostings(List<PositionalPostingList> postings)
        {
            if (postings.Count == 1)
                return postings[0];

            if (postings.Count == 2)
                return SumPostings(postings[0], postings[1]);

            int[] sequence = DetermineSequence(postings);
            int ind = 0;

            List<PositionalPostingList> merge_postings = new List<PositionalPostingList>();
            PositionalPostingList new_posting;

            for (ind = 0; ind < postings.Count - 1; ind+= 2)
            {
                new_posting = SumPostings( postings[ sequence[ind] ], postings[ sequence[ind+1] ]);  
                merge_postings.Add(new_posting);
            }

            if (postings.Count % 2 != 0)
                merge_postings.Add(postings[sequence[ind]]);

            while (merge_postings.Count > 1)
            {
                new_posting = SumPostings(merge_postings[0], merge_postings[1]);
                merge_postings.RemoveRange(0, 2);
                merge_postings.Add(new_posting);
            }

            return merge_postings[0];
        }

        protected override PositionalPostingList GetPostingsProduct(List<PositionalPostingList> postings)
        {
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

        private PositionalPostingList SumPostings(PositionalPostingList posting1, PositionalPostingList posting2)
        {
            PositionalPostingList sum_of_postings;
           
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

            //list_of_positions moze byc pusta
            sum_of_postings = new PositionalPostingList(doc_ids.ToArray(), list_of_positions.ToArray());
            return sum_of_postings;
        }

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
