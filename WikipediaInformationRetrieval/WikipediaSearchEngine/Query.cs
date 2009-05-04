using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parser;
using InversedIndex;

namespace WikipediaSearchEngine
{
    public abstract class Query
    {
        public Query(string query)
        {
            mUserQuery = query;
            mIndex = InversedPositionalIndex.Get();
            ParseQuery(mIndex.PerformedLematization, mIndex.PerformedStemming, mIndex.PerformedStopWordsRemoval);
        }

        public abstract string QueryNormalForm
        {
            get;
        }

        public List<List<string>> QueryStructure
        {
            get { return mQueryStructure; }
        }

        public PositionalPostingList ProcessQuery()
        {
            mIndex = InversedPositionalIndex.Get();
            List<PositionalPostingList> and_list = new List<PositionalPostingList>();
            List<PositionalPostingList> or_list = new List<PositionalPostingList>();

            foreach (List<string> subquery in mQueryStructure)
            {
                or_list.Clear();
                foreach (string word in subquery)
                    or_list.Add(mIndex.PostingList(word));

                and_list.Add(MergePostings(or_list));
            }

            return GetPostingsProduct(and_list);
        }


        protected abstract PositionalPostingList MergePostings(List<PositionalPostingList> postings);

        protected abstract PositionalPostingList GetPostingsProduct(List<PositionalPostingList> postings);

        protected int[] DetermineSequence(List<PositionalPostingList> postings)
        {
            if (postings.Count < 2)
                return null;

            SortedList<int, int> sequence = new SortedList<int, int>();
            
            int ind = 0;
            foreach (PositionalPostingList p in postings)
            {
                sequence.Add(p.DocumentIds.Length, ind);
                ind++;
            }

            return sequence.Values.ToArray();
        }

        private void ParseQuery(bool doLematization, bool doStemming, bool removeStopWords)
        {
            mQueryStructure = new List<List<string>>();
            string[] or_subtrees = mUserQuery.Split(new char[] { ' ' });
            string word;

            foreach (string clause in or_subtrees)
            {
                List<string> result = new List<string>();
                List<string> or_literals = msTokenizer.ConvertStrings(clause.Split(new char[] { '|' }).ToList());
                List<string> base_forms;

                foreach (string token in or_literals)
                {
                    word = msNormalizer.Normalize(token);

                    if (doLematization)
                        base_forms = msLematizer.LematizeString(word);

                    else
                    {
                        base_forms = new List<string>();
                        base_forms.Add(word);
                    }

                    foreach (string base_form in base_forms)
                    {
                        word = base_form;
                        if (removeStopWords && StopWords.IsStopWord(base_form))
                            continue;

                        if (doStemming)
                            word = msStemmer.DoStemming(base_form);

                        result.Add(word);
                    }
                }

                mQueryStructure.Add(result);
            }
        }


        private static Normalizer msNormalizer = new Normalizer();
        private static Tokenizer msTokenizer = new Tokenizer();
        private static Lematizer msLematizer = new Lematizer();
        private static Stemmer msStemmer = new Stemmer();
        private InversedPositionalIndex mIndex;
        
        protected List<List<string>> mQueryStructure;
        protected string mUserQuery;
        protected string mNormalizedQuery;
    }
}
