using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parser;
using InversedIndex;

namespace WikipediaSearchEngine
{
    /// <summary>
    /// Class representing and handling user queries. 
    /// To act properly morphologic dictionary needs to be initialized.
    /// </summary>
    public abstract class Query
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="query">User query</param>
        public Query(string query)
        {
            mUserQuery = query;
            mIndex = InversedPositionalIndex.Get();
            ParseQuery(mIndex.PerformedLematization, mIndex.PerformedStemming, mIndex.PerformedStopWordsRemoval);
        }

        /// <summary>
        /// Gets the normalized form of query. Queries can be compared with their normal form.
        /// </summary>
        public abstract string QueryNormalForm
        {
            get;
        }

        /// <summary>
        /// Query can be represent as conjunction (AND) of one or more disjunction (OR) with one or more words.
        /// </summary>
        public List<List<string>> QueryStructure
        {
            get { return mQueryStructure; }
        }

        /// <summary>
        /// Process query using inverted index.
        /// </summary>
        /// <returns>PositionalPostingList contains id of documents which satisfy the query.
        /// Return null when query contains only stop words and option 'stop words removal' was enabled</returns>
        public PositionalPostingList ProcessQuery()
        {
            //it's possibly query with stop words only
            if (this.mNormalizedQuery.Length < 1)
                return null;

            if (mQueryAnswer != null)
                return mQueryAnswer;

            mIndex = InversedPositionalIndex.Get();
            List<PositionalPostingList> and_list = new List<PositionalPostingList>();
            List<PositionalPostingList> or_list = new List<PositionalPostingList>();
            PositionalPostingList word_posting;

            foreach (List<string> subquery in mQueryStructure)
            {
                or_list.Clear();
                foreach (string word in subquery)
                {
                    word_posting = mIndex.PostingList(word);
                   
                    if ( word_posting.Positions.Length > 0 )
                        or_list.Add(mIndex.PostingList(word));
                }
                
               
                and_list.Add(MergePostings(or_list));
            }

            mQueryAnswer = GetPostingsProduct(and_list);
            return mQueryAnswer;
        }

        /// <summary>
        /// Merge list of postings to one posting list, which is union of them.
        /// </summary>
        /// <param name="postings">List of posting list to merge</param>
        /// <returns>Result of merging</returns>
        protected abstract PositionalPostingList MergePostings(List<PositionalPostingList> postings);

        /// <summary>
        /// Merge list of postings to one posting list, which is intersection of them.
        /// </summary>
        /// <param name="postings">List of posting list to merge</param>
        /// <returns>Product of postings</returns>
        protected abstract PositionalPostingList GetPostingsProduct(List<PositionalPostingList> postings);

        /// <summary>
        /// Return order of increasing document frequency for list of postings
        /// </summary>
        /// <param name="postings">List of posting list to determine merging order</param>
        /// <returns></returns>
        protected int[] DetermineSequence(List<PositionalPostingList> postings)
        {
            if (postings.Count < 2)
                return null;
            if (postings.Count == 2)
                return new int[]{0, 1};

            List<Pair> sequence = new List<Pair>();
          
            int ind = 0;
            foreach (PositionalPostingList p in postings)
            {
                sequence.Add(new Pair( ind, p.DocumentIds.Length ));
                ind++;
            }

            sequence.Sort();
            int[] result = new int[postings.Count];
            for(int i=0; i<sequence.Count; i++)
                result[i] = sequence[i].First;

            return result;
        }

        /// <summary>
        /// Build structure of query from user query give as string.
        /// Tokens in query are retrieved like by creating inverted index.
        /// </summary>
        /// <param name="doLematization">Does Lematizer will be used to convert words</param>
        /// <param name="doStemming">Does Stemmer will be used to convert words</param>
        /// <param name="removeStopWords">Does stop words will be removed</param>
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

                        if(!result.Contains(word))
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
        protected PositionalPostingList mQueryAnswer;
        protected string mUserQuery;
        protected string mNormalizedQuery;
    }
}
