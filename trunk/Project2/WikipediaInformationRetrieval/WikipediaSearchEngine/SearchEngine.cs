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
        private class DocumentComparer : System.Collections.Generic.IComparer<float>
        {
            public int Compare(float x, float y)
            {                
                if (y < x)
                {
                    return -1;
                }
                else if (y == x)
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
            mDocumentWordsCount = new byte[mIndex.Positions.Length];

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
            float idf;
            float wf;
            uint document_id;

            PositionalPostingList posting_list;

            foreach (List<string> or_list in query.QueryStructure)
            {
                if (or_list.Count > 0)
                {
                    words.Add(or_list[0]);
                }
            }

            // set default values
            for (uint i = 0; i < mDocumentScores.Length; i++)
            {
                mDocumentScores[i] = 0.0f;
                mDocumentWordsCount[i] = 0;
                mDocumentRanking[i] = i;
            }

            // counting document scores
            foreach (string word in words)
            {                
                posting_list = mIndex.PostingList(word);
                idf = (float)Math.Log((float)mIndex.VocabularySize / posting_list.DocumentIds.Length);
                for (uint i = 0; i < posting_list.DocumentIds.Length; i++)
                {
                    document_id = posting_list.DocumentIds[i];
                    //int termFrequency = posting_list.Positions[i].Length;
                    wf = 1 + (float)Math.Log(posting_list.Positions[i].Length);
                    if (posting_list.Positions[i][0] < 10)
                    {
                        wf *= mBeginBonus;
                    }
                    mDocumentWordsCount[document_id] += 1;
                    mDocumentScores[document_id] += wf * idf;
                }
            }

            // for phrase
            for (int j = 0; j < words.Count - 1; j++)
            {
                posting_list = IntersectPostings(
                    mIndex.PostingList(words[j]), mIndex.PostingList(words[j+1]));

                idf = (float)Math.Log((float)mIndex.VocabularySize / posting_list.DocumentIds.Length);
                for (uint i = 0; i < posting_list.DocumentIds.Length; i++)
                {
                    document_id = posting_list.DocumentIds[i];
                    //int termFrequency = posting_list.Positions[i].Length;
                    mDocumentScores[document_id] *= mPhraseBonus;                    
                }
            }


            for (int i = 0; i < mDocumentScores.Length; i++)
            {
                if (mDocumentScores[i] > 0)
                {
                    mDocumentScores[i] /= mIndex.Lengths[i];
                    mDocumentScores[i] *= (float)Math.Pow(((float)mDocumentWordsCount[i] / words.Count), 2);
                }
            }
            
            Array.Sort<float, uint>(mDocumentScores, mDocumentRanking, new DocumentComparer());            

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

        PositionalPostingList IntersectPostings(PositionalPostingList posting1, PositionalPostingList posting2)
        {
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
                    positions = IntersectPositions(posting1.Positions[index1], posting2.Positions[index2], 1);

                    if (positions.Length > 0)
                    {
                        doc_ids.Add(key1);
                        list_of_positions.Add(positions);
                    }

                    index1++;
                    index2++;
                }
            }            

            product_of_postings = new PositionalPostingList(doc_ids.ToArray(), list_of_positions.ToArray());
            return product_of_postings;
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
        private byte[] mDocumentWordsCount;
        public float mBeginBonus;
        public float mPhraseBonus;
        private TimeSpan mTotalTime;

        private DateTime mStartTime;
        private DateTime mStopTime;
       
    }
}
