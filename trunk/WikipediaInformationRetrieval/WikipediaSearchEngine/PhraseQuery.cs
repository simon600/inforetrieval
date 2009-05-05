using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InversedIndex;

namespace WikipediaSearchEngine
{
    public class PhraseQuery : Query
    {

        public PhraseQuery(string query)
            :base(query)
        {
            mDistances = new List<uint>();
            mDistancesSequence = new List<int>();

            int k = 0;
            int i = 0;

            while(i < mQueryStructure.Count - 1)
            {
                k = i+1;
                while (mQueryStructure[k].Count == 0)
                    k++;

                mDistances.Add((uint)(k - i));
                i = k;
            }

            i = 0;
            while (i < mQueryStructure.Count)
            {
                if (mQueryStructure[i].Count == 0)
                    mQueryStructure.RemoveAt(i);
                else i++;
            }
        }

        public override string QueryNormalForm
        {
            get 
            {
                if (mNormalizedQuery == null)
                {
                    mNormalizedQuery = "";
                    string clause;

                    foreach (List<string> or_list in mQueryStructure)
                    {
                        or_list.Sort();
                        clause = "";
                        foreach (string w in or_list)
                            clause += w + "|";

                        if (clause.Length > 0)
                            clause = clause.Remove(clause.Length - 1);

                        mNormalizedQuery += clause + " ";
                    }

                    foreach (uint distance in mDistances)
                        mNormalizedQuery += distance.ToString() + " ";

                    mNormalizedQuery = mNormalizedQuery.Trim();
                }
                return mNormalizedQuery;
            
            }
        }

        protected override PositionalPostingList IntersectPostings(PositionalPostingList posting1, PositionalPostingList posting2)
        {
            if (mDistancesSequence.Count == 0)
                CountDistances();

            int diff = mDistancesSequence[0];
            
            PositionalPostingList product_of_postings;

            List<uint> doc_ids = new List<uint>();
            List<ushort[]> list_of_positions = new List<ushort[]>();

            ushort[] positions;

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
                    positions = IntersectPositions(posting1.Positions[index1], posting2.Positions[index2], diff);

                    if (positions.Length > 0)
                    {
                        doc_ids.Add(key1);
                        list_of_positions.Add(positions);
                    }

                    index1++;
                    index2++;
                }
            }

            mDistancesSequence.RemoveAt(0);


            product_of_postings = new PositionalPostingList(doc_ids.ToArray(), list_of_positions.ToArray());
            return product_of_postings;
        }

        protected override PositionalPostingList UnionPostings(PositionalPostingList posting1, PositionalPostingList posting2)
        {
            PositionalPostingList union_of_postings;

            List<uint> doc_ids = new List<uint>();
            List<ushort[]> list_of_positions = new List<ushort[]>();

            ushort[] positions;

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
                    list_of_positions.Add(posting1.Positions[index1]); 
                    
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

                    positions = UnionPositionsList(posting1.Positions[index1], posting2.Positions[index2]);
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

            union_of_postings = new PositionalPostingList(doc_ids.ToArray(), list_of_positions.ToArray());
            return union_of_postings;
        }

        private void CountDistances()
        {
            if (mSequence == null)
                throw new Exception("Nieustalona kolejność scalania");

            if (mSequence.Length != mDistances.Count + 1)
                throw new Exception("Za duzo list do scalenia");

            int x, y;
            uint distance;
            int multiply_by = 1;

            for (int i = 1; i < mSequence.Length; i++)
            {
                multiply_by = 1;

                x = mSequence[0];
                y = mSequence[i];

                if (x > y)
                {
                    x = mSequence[i];
                    y = mSequence[0];

                    multiply_by = -1;
                }

                distance = 0;
                for (int j = x; j < y; j++)
                    distance += mDistances[j];

                mDistancesSequence.Add((int)distance*multiply_by);
            }
        }

        private ushort[] IntersectPositions(ushort[] position_list1, ushort[] position_list2, int diff)
        {
            List<ushort> new_positions = new List<ushort>();

            int index1 = 0;
            int index2 = 0;

            int pos1;
            int pos2;

            while (index1 < position_list1.Length && index2 < position_list2.Length)
            {
                pos1 = (int)position_list1[index1];
                pos2 = (int)position_list2[index2] - diff;

                if (pos1 < pos2)
                    index1++;
                else if (pos1 > pos2)
                    index2++;

                else
                {
                    new_positions.Add((ushort)pos1);
                    index1++;
                    index2++;
                }

            }

            return new_positions.ToArray();
        }

        private ushort[] UnionPositionsList(ushort[] positions1, ushort[] positions2)
        {
            List<ushort> positions = new List<ushort>();
            int index1 = 0;
            int index2 = 0;

            while (index1 < positions1.Length && index2 < positions2.Length)
            {
                if (positions1[index1] < positions1[index2])
                {
                    positions.Add(positions1[index1]);
                    index1++;
                }
                else if (positions1[index1] > positions1[index2])
                {
                    positions.Add(positions2[index2]);
                    index2++;
                }
                else
                {
                    positions.Add(positions1[index1]);
                    index1++;
                    index2++;
                }
            }

            while (index1 < positions1.Length)
            {
                positions.Add(positions1[index1]);
                index1++;
            }
            while (index2 < positions2.Length)
            {
                positions.Add(positions2[index2]);
                index2++;
            }

            return positions.ToArray();
        }

        private List<uint> mDistances;
        private List<int> mDistancesSequence;
    }
}
