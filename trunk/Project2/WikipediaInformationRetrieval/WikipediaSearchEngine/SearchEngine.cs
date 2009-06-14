using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Morphologic;
using InversedIndex;
using System.Runtime.InteropServices;
using Parser;

namespace WikipediaSearchEngine
{
    public class SearchEngine
    {
        private class DocumentComparer : System.Collections.IComparer
        {
            public int Compare(Object x, Object y)
            {
                float val_x = (float)x;
                float val_y = (float)y;

                if (val_y < val_x)
                {
                    return -1;
                }
                else if (val_y == val_x)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
        }

        public SearchEngine(string source_path, string morphologic_path, string index_path, bool readTitles)
        {
            mTextSource = new StreamReader(new FileStream(source_path, FileMode.Open));

            mMorphologic = MorphologicDictionary.Get();
            mMorphologic.ReadFromFile(morphologic_path);

            mIndex = InversedPositionalIndex.Get();
            mIndex.ReadFromStream(new FileStream(index_path, FileMode.Open));

            mDocumentScores = new float[mIndex.Positions.Length];
            mDocumentRanking = new uint[mIndex.Positions.Length];

            mAnswers = new List<string>();

            if (readTitles)
                ReadTitles();

            if (QueryPerformanceFrequency(out mFrequency) == false)
                throw new Exception("blad");
        }


        public void Close()
        {
            mTextSource.Close();
        }

        public List<string> SearchFor(string query_string)
        {
            if (query_string.StartsWith("\"") && query_string.EndsWith("\""))
                mLastQuery = new PhraseQuery(query_string);

            else mLastQuery = new BooleanQuery(query_string);

            //response time start
            QueryPerformanceCounter(out mStart);
            mStartTime = DateTime.Now;            
            //PositionalPostingList query_result = mLastQuery.ProcessQuery();
            ProcessQuery(mLastQuery);

            PrepareAnswerList();

            //response time stop
            QueryPerformanceCounter(out mStop);
            mStopTime = DateTime.Now;
            return mAnswers;
        }

        public void QueriesFromFile(string queries_path, string results_path)
        {
            CharReader reader = new CharReader(new FileStream(queries_path, FileMode.Open));
            StreamWriter writer = new StreamWriter(new FileStream(results_path, FileMode.Create), Encoding.UTF8);

            string query_string;
          
            BooleanQuery boolean_query = new BooleanQuery("");
            PhraseQuery phrase_query = new PhraseQuery("");

            //PositionalPostingList postings;
            mTotalTime = new TimeSpan(0);

            while (!reader.EndOfStream)
            {
                query_string = reader.ReadLine();
                Console.WriteLine(query_string);

                if (query_string.StartsWith("\"") && query_string.EndsWith("\""))
                {
                    //phrase_query.NewUserQuery(query_string);
                   
                    ////mierzymy czas
                    //mStartTime = DateTime.Now;

                    //postings = phrase_query.ProcessQuery();
                    //PrepareAnswerList(postings);

                    //mStopTime = DateTime.Now;

                    //mTotalTime += (mStopTime - mStartTime);
                }
                else
                {
                    boolean_query.NewUserQuery(query_string);
                   
                    //mierzymy czas
                    mStartTime = DateTime.Now;

                    ProcessQuery(boolean_query);
                    PrepareAnswerList();

                    mStopTime = DateTime.Now;

                    mTotalTime += (mStopTime - mStartTime);
                }

                writer.Write("QUERY: " + query_string);
                writer.WriteLine(" TOTAL: " + mAnswers.Count.ToString());

                foreach (string title in mAnswers)
                    writer.WriteLine(title);

                mAnswers.Clear();

                //ograniczenie zuzycia pamieci
                //if (GC.GetTotalMemory(false) > 1500000000)
                //    mIndex.CompressPostings();

            }

            reader.Close();
            writer.Close();
        }

        public TimeSpan ResponseTime
        {
            get
            {
                return (mStopTime - mStartTime);
            }
        }

        public double ResponseTimeInSeconds
        {
            get
            {
                return (double)(mStop - mStart) / (double) mFrequency;
            }
        }

        public TimeSpan TotalResponseTime
        {
            get
            {
                return mTotalTime;
            }
        }

        private void ProcessQuery(Query query)
        {
            List<string> words = new List<string>();
            foreach (List<string> or_list in query.QueryStructure)
            {
                if (or_list.Count > 0)
                {
                    words.Add(or_list[0]);
                }
            }

            for (int i = 0; i < mDocumentScores.Length; i++)
            {
                mDocumentScores[i] = 0.0f;
            }

            // counting document scores
            foreach (string word in words)
            {                
                PositionalPostingList posting_list = mIndex.PostingList(word);
                float idf = (float)Math.Log((float)mIndex.VocabularySize / posting_list.DocumentIds.Length);
                for (uint i = 0; i < posting_list.DocumentIds.Length; i++)
                {
                    uint documentId = posting_list.DocumentIds[i];
                    int termFrequency = posting_list.Positions[i].Length;
                    mDocumentScores[documentId] += termFrequency * idf;
                }
            }

            // TODO:
            // dzielenie przez długość dokumentów

            // sorting documents
            for (uint i = 0; i < mDocumentRanking.Length; i++)
            {
                mDocumentRanking[i] = i;
            }
            
            Array.Sort(mDocumentScores, mDocumentRanking);
            uint temp;
            for (uint i = 0; i < mDocumentRanking.Length / 2; i++)
            {
                temp = mDocumentRanking[i];
                mDocumentRanking[i] = mDocumentRanking[mDocumentRanking.Length - 1 - i];
                mDocumentRanking[mDocumentRanking.Length - 1 - i] = temp;
            }

            // mDocumentScores unusable now, have to recalculate
        }

        private void PrepareAnswerList()
        {
            long position;
            string title;

            mAnswers.Clear();

            //if (query_result == null)
            //    return;

            if (hasTitles)
            {
                foreach (uint docId in mDocumentRanking)
                    mAnswers.Add(mTitles[(int)docId]);

                return;
            }

            foreach (uint docId in mDocumentRanking)
            {
                position = mIndex.Positions[docId];
                mTextSource.BaseStream.Position = position;
                mTextSource.DiscardBufferedData();

                title = mTextSource.ReadLine();
                mAnswers.Add(title);
            }
        }

        public void ReadTitles()
        {
            mTitles = new string[mIndex.Positions.Length];
            string title;

            for(int i= 0; i<mIndex.Positions.Length; i++)
            {
                long position = mIndex.Positions[i];
           
                mTextSource.BaseStream.Position = position;
             
                mTextSource.DiscardBufferedData();
              
                title = mTextSource.ReadLine();
              
                mTitles[i] = title;
            }

            mTextSource.Close();
            hasTitles = true;
        }

        public void StartTimer()
        {
            QueryPerformanceCounter(out mStart);
            mStartTime = DateTime.Now;
        }

        public void StopTimer()
        {
            QueryPerformanceCounter(out mStop);
            mStopTime = DateTime.Now;
        }

        private Query mLastQuery;
        private List<string> mAnswers;
        private string[] mTitles;
        private bool hasTitles = false;

        private StreamReader mTextSource;
        private MorphologicDictionary mMorphologic;
        private InversedPositionalIndex mIndex;


        [DllImport("kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);
        private long mStart;
        private long mStop;
        private long mFrequency;
        private float[] mDocumentScores;        
        private uint[] mDocumentRanking;
        private TimeSpan mTotalTime;

        private DateTime mStartTime;
        private DateTime mStopTime;
       
    }
}
