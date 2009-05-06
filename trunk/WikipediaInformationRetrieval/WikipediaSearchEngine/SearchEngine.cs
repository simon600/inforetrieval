using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Morphologic;
using InversedIndex;
using System.Runtime.InteropServices;

namespace WikipediaSearchEngine
{
    public class SearchEngine
    {
        public SearchEngine(string source_path, string morphologic_path, string index_path, bool readTitles)
        {
            mTextSource = new StreamReader(new FileStream(source_path, FileMode.Open));

            mMorphologic = MorphologicDictionary.Get();
            mMorphologic.ReadFromFile(morphologic_path);

            mIndex = InversedPositionalIndex.Get();
            mIndex.ReadFromStream(new FileStream(index_path, FileMode.Open));

            if (readTitles)
                ReadTitles();

            if (QueryPerformanceFrequency(out mFrequency) == false)
                throw new Exception("DUPA");
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
            start = DateTime.Now;
            PositionalPostingList query_result = mLastQuery.ProcessQuery();
            PrepareAnswerList(query_result);

            //response time stop
            QueryPerformanceCounter(out mStop);
            stop = DateTime.Now;
            return mAnswers;
        }

        public TimeSpan ResponseTime
        {
            get
            {
                return (stop - start);
            }
        }

        public double ResponseTimeInSeconds
        {
            get
            {
                return (double)(mStop - mStart) / (double) mFrequency;
            }
        }

        private void PrepareAnswerList(PositionalPostingList query_result)
        {
            long position;
            string title;

            mAnswers = new List<string>();
            if (hasTitles)
            {
                foreach (uint docId in query_result.DocumentIds)
                    mAnswers.Add(mTitles[(int)docId]);

                return;
            }

            foreach (uint docId in query_result.DocumentIds)
            {
                position = mIndex.Documents[docId].FilePosition;
                mTextSource.BaseStream.Seek(position, SeekOrigin.Begin);

                title = mTextSource.ReadLine();
                title = title.Replace("#", "").Trim();
                mAnswers.Add(title);

                mTextSource.DiscardBufferedData();
                
             }
        }

        public void ReadTitles()
        {
            mTitles = new List<string>();
            string title;

            foreach(Document d in mIndex.Documents.Values)
            {
                mTextSource.BaseStream.Seek(d.FilePosition, SeekOrigin.Begin);
               
                title = mTextSource.ReadLine();
                title = title.Replace("#", "").Trim();
               
                mTitles.Add(title);

                mTextSource.DiscardBufferedData();
            }

            mTextSource.Close();
            hasTitles = true;
        }

        public void StartTimer()
        {
            QueryPerformanceCounter(out mStart);
            start = DateTime.Now;
        }

        public void StopTimer()
        {
            QueryPerformanceCounter(out mStop);
            stop = DateTime.Now;
        }

        private Query mLastQuery;
        private List<string> mAnswers;
        private List<string> mTitles;
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

        private DateTime start;
        private DateTime stop;

        private long mElapsedTime;
    }
}
