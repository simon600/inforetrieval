using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Parser;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using InversedIndex;
using GammaCompression;

namespace WikipediaIndexCreator
{
    /// <summary>
    /// Used for creating inversed positional index.
    /// </summary>
    public class IndexCreator
    {
        /// <summary>
        /// Gets only instance of index creator.
        /// </summary>
        /// <returns>Instance of index creator.</returns>
        public static IndexCreator Get()
        {
            if (msIndexCreator == null)
            {
                msIndexCreator = new IndexCreator();
            }

            return msIndexCreator;
        }

        /// <summary>
        /// Creates index reading data from specified stream.
        /// </summary>
        /// <param name="sourceStream">Stream to read data from.</param>
        /// <param name="destinationStream">Stream to write index to.</param>
        public void CreateIndexFromStream(
            Stream sourceStream,
            Stream destinationStream)
        {
            const long BLOCK_SIZE = 50000000;
            long size;

            int file_number = 0;   // number of temporary file
            string filename;
            FileStream file_stream;            

            List<string> filenames = new List<string>();
            List<FileStream> file_streams = new List<FileStream>();            

            // inverted index to create
            SortedDictionary<string, SortedList<uint, List<ushort>>> inverted_index;

            //length of documents
            List<int> documents_length = new List<int>();

            WordsStream words_stream = new WordsStream(sourceStream);

            // find first article
            while (!words_stream.EndOfStream && !words_stream.Read().Contains("##TITLE##"))
            {
            }

            while (!words_stream.EndOfStream)
            {
                filename = "temp" + file_number.ToString() + ".tmp";
                while (File.Exists(filename))
                {
                    file_number++;
                    filename = "temp" + file_number.ToString() + ".tmp";
                }
                filenames.Add(filename);
                file_stream = new FileStream(filename, FileMode.OpenOrCreate);
                file_streams.Add(file_stream);

                CreateInversedIndex(
                    words_stream,
                    BLOCK_SIZE,
                    out inverted_index,
                    out size,
                    documents_length);

                WriteIndexToStream(inverted_index, file_stream, size);
                file_stream.Flush();                
            }

            
            MergeIndices(file_streams, destinationStream);

            List<long> positions = ReadTitlePosition(sourceStream);

            WriteDocumentsToStream(positions, destinationStream);
            WriteDocumentsLengthToStream(documents_length, destinationStream);

            destinationStream.Close();

            foreach (string name in filenames)
            {
                File.Delete(name);
            }
        }


        /// <summary>
        /// Gets or sets string separating articles in file.
        /// </summary>
        public string ArticleSeparator
        {
            get
            {
                return mArticleSeparator;
            }
            set
            {
                mArticleSeparator = value.Trim();
            }
        }

        public bool PerformStemming
        {
            get
            {
                return mPerformStemming;
            }
            set
            {
                mPerformStemming = value;
            }
        }

        public bool PerformStopWordsRemoval
        {
            get
            {
                return mPerformStopWordsRemoval;
            }
            set
            {
                mPerformStopWordsRemoval = value;
            }
        }

        public bool PerformLematization
        {
            get
            {
                return mPerformLematization;
            }
            set
            {
                mPerformLematization = value;
            }
        }

        public bool PerformCompression
        {
            get
            {
                return mPerformCompression;
            }
            set
            {
                mPerformCompression = value;
            }
        }


        /////////////////////////////// PRIVATE //////////////////////////////        

        /// <summary>
        /// Default constructor.
        /// </summary>
        private IndexCreator()
        {
            mArticleSeparator = "##TITLE##";
            mPerformLematization = true;
            mPerformStemming = true;
            mPerformStopWordsRemoval = true;

            mBitStreamWriter = new BitStreamWriter();

            tokenizer = new Tokenizer();
            normalizer = new Normalizer();
            lematizer = new Lematizer();
            stemmer = new Stemmer();
        }

        /// <summary>
        /// Creates index structure.
        /// </summary>
        /// <param name="wordsStream">Stream of words for creating
        /// index.</param>
        /// <param name="blockSize">Size of memory block. Function will finish
        /// index creating if exceedes.</param>
        /// <param name="inversedIndex">Function writes inversed index
        /// structure here.</param>
        /// <param name="documents">List of documents written here.</param>
        /// <param name="size">Size of index in bytes written here.</param>
        private void
            CreateInversedIndex(
                WordsStream wordsStream,
                long blockSize,
                out SortedDictionary<string, SortedList<uint, List<ushort>>>
                    inversedIndex,
                out long size, 
                List<int> documents_len)
        {
            // size of block in bytes
            size = 0;

            inversedIndex =
                new SortedDictionary<string, SortedList<uint, List<ushort>>>();            
            
            List<string> article = new List<string>();            
                        
            string word;
            ushort index_in_article = 0;
        
                        
            while (!wordsStream.EndOfStream && size < blockSize)
            {
                article.Clear();
                index_in_article = 0;
        
                mDocumentIndex++;

                while (!wordsStream.EndOfStream &&
                    !(word = wordsStream.Read()).Contains("##TITLE##"))
                {
                    article.Add(word);
                }

                article = tokenizer.ConvertStrings(article);

                foreach (string token in article)
                {
                    size += InsertTokenToIndex(token,
                        inversedIndex,
                        mDocumentIndex-1,
                        index_in_article);

                    index_in_article++;
                }

                documents_len.Add(index_in_article);
            }            
        }

        private long InsertTokenToIndex(
            string token,
            SortedDictionary<string, SortedList<uint, List<ushort>>>
                inversedIndex,
            uint document,
            ushort indexInArticle)
        {
            long size = 0;
            bool add = true;
            List<string> base_forms;
            List<string> base_forms_after_stemming = new List<string>();

            string word;

            word = normalizer.Normalize(token);

            if (mPerformLematization)
            {
                base_forms = lematizer.LematizeString(word);
            }
            else
            {
                base_forms = new List<string>();
                base_forms.Add(word);
            }

            foreach (string base_form in base_forms)
            {
                word = base_form;
               
                if (mPerformStopWordsRemoval &&
                    StopWords.IsStopWord(base_form))
                {
                    add = false;
                }

                if (base_form.Length > 0 && add)
                {
                    if (mPerformStemming)
                    {
                        word = stemmer.DoStemming(word);

                        //the word is already in inverted index with this indexInArticle
                        if (base_forms_after_stemming.Contains(word))
                            continue;

                        else 
                        {
                            base_forms_after_stemming.Add(word);
                        }
                    }

                    if (word.Length > 0)
                    {

                        if (!inversedIndex.ContainsKey(word))
                        {
                            size += (long)word.Length + 1;
                            inversedIndex.Add(word,
                                new SortedList<uint, List<ushort>>());
                        }

                        if (!inversedIndex[word].ContainsKey(
                            document))
                        {
                            size += sizeof(uint); // size of uint
                            inversedIndex[word].Add(
                                document, new List<ushort>());
                        }

                        size += sizeof(uint);
                        inversedIndex[word][document].Add(
                            indexInArticle);
                    }
                }
            }

            return size;
        }

        /// <summary>
        /// Writes index structure to stream.
        /// </summary>
        /// <param name="inversedIndex">Index structure.</param>
        /// <param name="stream">Stream to write to.</param>
        private void WriteIndexToStream(
            SortedDictionary<string, SortedList<uint, List<ushort>>> inversedIndex,
            Stream stream,
            long size)
        {
            BinaryWriter binary_writer = new BinaryWriter(stream);

            stream.SetLength(size);

            // write size of the dictionary
            binary_writer.Write(inversedIndex.Count);

            // write entries
            foreach (KeyValuePair<string, SortedList<uint, List<ushort>>> entry
                in inversedIndex)
            {
                binary_writer.Write(entry.Key);     // write key
                // length of posting list                
                
                // write posting list
                WritePostingListToStream(binary_writer, entry.Value);
            }            
        }

        /// <summary>
        /// Writes posting list to stream.
        /// </summary>
        /// <param name="writer">Binary writer to use.</param>
        /// <param name="postingList">Posting list to write.</param>
        private void WritePostingListToStream(
            BinaryWriter writer,
            SortedList<uint, List<ushort>> postingList)
        {
            // write size
            writer.Write(postingList.Count);

            foreach (KeyValuePair<uint, List<ushort>> post_pair
                    in postingList)
            {
                writer.Write(post_pair.Key);  // write doc_id
                // length of positions list
                writer.Write(post_pair.Value.Count);

                // write positions list
                foreach (ushort pos in post_pair.Value)
                {
                    writer.Write(pos);
                }
            }
        }

        private void CompressAndWritePostingListToStream(
            BinaryWriter writer,
            SortedList<uint, List<ushort>> postingList)
        {
            //write size
            writer.Write(postingList.Count);

            byte[] bitstream = CompressPostingList(postingList);

            //write size of bitstream
            writer.Write(bitstream.Length);     //bitstream.StreamSize
            //write bitstream
            writer.Write(bitstream);      //bitStream.Bytes
        }

        private byte[] CompressPostingList(SortedList<uint, List<ushort>> postingList)
        {
            mBitStreamWriter.ResetStream();

            uint gap = 0;
            uint current_id = 0;
            ushort current_position = 0;

            foreach (KeyValuePair<uint, List<ushort>> pair in postingList)
            {
                gap = pair.Key - current_id;
                current_id = pair.Key;

                GammaEncoding.CodeInt(gap, mBitStreamWriter);
                GammaEncoding.CodeInt((uint)pair.Value.Count, mBitStreamWriter);

                current_position = 0;

                foreach (ushort pos in pair.Value)
                {
                    gap = (uint)pos - current_position;
                    current_position += (ushort)gap;

                    GammaEncoding.CodeInt(gap, mBitStreamWriter);
                }
            }

           // return compressed_posting;
            return mBitStreamWriter.Bytes;
        }

        private List<long> ReadTitlePosition(Stream source)
        {
            List<long> positions = new List<long>();

            source.Position = 0;
            WordsStream words_stream = new WordsStream(source);

            while (!words_stream.EndOfStream && !words_stream.Read().Contains("##TITLE##"))
            {
            }

            while (!words_stream.EndOfStream)
            {
                positions.Add(words_stream.Position);
                while (!words_stream.EndOfStream && !words_stream.Read().Contains("##TITLE##"))
                {
                }
            }

            return positions;
        }

        /// <summary>
        /// Writes list of documents to stream.
        /// </summary>
        /// <param name="documents">List of documents.</param>
        /// <param name="stream">Stream to write to.</param>
        private void WriteDocumentsToStream(
            List<long> documents,
            Stream stream)
        {
            stream.SetLength(stream.Length + documents.Count * (sizeof(long)) + sizeof(int));
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write(documents.Count);

            foreach (long position in documents)
                writer.Write(position);
        }


        private void WriteDocumentsLengthToStream(List<int> documents_length, Stream stream)
        {
            stream.SetLength(stream.Length + documents_length.Count * (sizeof(int)) );
            BinaryWriter writer = new BinaryWriter(stream);

            foreach (int len in documents_length)
                writer.Write(len);
        }

        /// <summary>
        /// Merges inversed indices writen in separate streams.
        /// </summary>
        /// <param name="sourceStreams">Streams, one for each
        /// inversed index.</param>
        /// <param name="outputStream">Here output inversed
        /// index will be written.</param>
        private void MergeIndices(
            List<FileStream> sourceStreams,
            Stream outputStream)
        {            
            CaseInsensitiveComparer comparer = new CaseInsensitiveComparer();
            int cmp;
            BinaryWriter writer = new BinaryWriter(outputStream);
            long length = 0; // length of the output stream   
            int size = 0; // total size of merged index

            // write number of entries, will be changed later
            writer.Write(size);

            WriteSettings(writer);

            foreach (FileStream file_stream in sourceStreams)
            {
                file_stream.Seek(0, SeekOrigin.Begin);
                length += file_stream.Length;
            }

            outputStream.SetLength(length);

            List<BinaryReader> readers = new List<BinaryReader>(sourceStreams.Count);
            List<int> min_words = new List<int>();
            List<SortedList<uint, List<ushort>>> postingLists =
                new List<SortedList<uint, List<ushort>>>();
            List<int> sizes = new List<int>(sourceStreams.Count);

            for (int i = 0; i < sourceStreams.Count; i++)
            {
                readers.Add(new BinaryReader(sourceStreams[i]));
            }            

            for (int i = 0; i < readers.Count; i++)
            {
                sizes.Add(readers[i].ReadInt32());
            }            

            List<string> words_in_streams = new List<string>(sourceStreams.Count);

            for (int i = 0; i < readers.Count; i++)
            {
                words_in_streams.Add(readers[i].ReadString());
                sizes[i]--;
            }

            while (sourceStreams.Count > 0)
            {
                min_words.Clear();
                min_words.Add(0);
                for (int i = 1; i < words_in_streams.Count; i++)
                {
                    cmp = comparer.Compare(words_in_streams[i], words_in_streams[min_words[0]]);
                    if (cmp < 0)
                    {
                        min_words.Clear();
                        min_words.Add(i);
                    }
                    else if (cmp == 0)
                    {
                        min_words.Add(i);
                    }
                }
                postingLists.Clear();
                foreach (int num in min_words)
                {
                    postingLists.Add(ReadPostingList(readers[num]));
                }

                size++;
                writer.Write(words_in_streams[min_words[0]]);
                SortedList<uint, List<ushort>> mergedPostingList = MergePostingLists(postingLists);

                //if compress index
                if (mPerformCompression)
                    CompressAndWritePostingListToStream(writer, mergedPostingList);
                else WritePostingListToStream(writer, mergedPostingList);

                foreach (int num in min_words)
                {
                    if (sizes[num] > 0)
                    {
                        words_in_streams[num] = readers[num].ReadString();
                        sizes[num]--;
                    }
                }                

                for (int s_num = 0; s_num < sourceStreams.Count; s_num++)
                {
                    if (sizes[s_num] <= 0)
                    {
                        sourceStreams[s_num].Close();
                        sourceStreams.RemoveAt(s_num);
                        words_in_streams.RemoveAt(s_num);
                        sizes.RemoveAt(s_num);
                        readers.RemoveAt(s_num);
                        s_num--;
                    }
                }
            }

            outputStream.SetLength(outputStream.Position);
            outputStream.Seek(0, SeekOrigin.Begin);
            writer.Write(size);
            outputStream.Seek(0, SeekOrigin.End);
        }

        /// <summary>
        /// Writes settings to stream.
        /// </summary>
        /// <param name="writer">Binary writter to use.</param>
        private void WriteSettings(BinaryWriter writer)
        {
            writer.Write(mPerformLematization);
            writer.Write(mPerformStemming);
            writer.Write(mPerformStopWordsRemoval);
            writer.Write(mPerformCompression);
        }

        /// <summary>
        /// Reads posting list from stream.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <returns>Posting list read.</returns>
        private SortedList<uint, List<ushort>> ReadPostingList(BinaryReader reader)
        {
            SortedList<uint, List<ushort>> posting_list =
                new SortedList<uint, List<ushort>>();

            List<ushort> positions_list;

            // BinaryReader reader = new BinaryReader(stream);
            int posting_length;
            int positions_length;
            uint doc_id;

            posting_length = reader.ReadInt32();

            for (int i = 0; i < posting_length; i++)
            {
                doc_id = reader.ReadUInt32();
                positions_list = new List<ushort>();
                posting_list.Add(doc_id, positions_list);

                positions_length = reader.ReadInt32();
                
                for (int j = 0; j < positions_length; j++)
                {
                    positions_list.Add(reader.ReadUInt16());
                }
            }

            return posting_list;
        }

        /// <summary>
        /// Merges few posting lists to one.
        /// 
        /// WARNING
        /// Postings lists in argument must be in order
        /// they were created.
        /// </summary>
        /// <param name="postingLists">Posting lists to merge.</param>
        /// <returns>Merged posting list.</returns>
        private SortedList<uint, List<ushort>> MergePostingLists(
            List<SortedList<uint, List<ushort>>> postingLists)
        {
            SortedList<uint, List<ushort>> merged_list;

            merged_list = new SortedList<uint, List<ushort>>();

            foreach (SortedList<uint, List<ushort>> posting_list
                in postingLists)
            {
                foreach (KeyValuePair<uint, List<ushort>> pair
                    in posting_list)
                {
                    merged_list.Add(pair.Key, pair.Value);
                }
            }

            return merged_list;
            
            #region old
            //int index0;
            //int index1;

            //uint key0;
            //uint key1;

            //if (postingLists.Count < 1)
            //{
            //    throw new Exception("Not enough posting lists.");
            //}

            //while (postingLists.Count > 1)
            //{
            //    merged_list = new SortedList<uint, List<uint>>();

            //    index0 = index1 = 0;

            //    while (index0 < postingLists[0].Count
            //        && index1 < postingLists[1].Count)
            //    {
            //        key0 = postingLists[0].Keys[index0];
            //        key1 = postingLists[1].Keys[index1];

            //        if (key0 < key1)
            //        {
            //            merged_list.Add(key0, postingLists[0][key0]);
            //            index0++;
            //        }
            //        else if (key0 > key1)
            //        {
            //            merged_list.Add(key1, postingLists[1][key1]);
            //            index1++;
            //        }
            //        else
            //        {
            //            merged_list.Add(
            //                key0,
            //                MergePositionsLists(
            //                    postingLists[0][key0],
            //                    postingLists[1][key1]));
            //            index0++;
            //            index1++;
            //        }
            //    }
            //    while (index0 < postingLists[0].Count)
            //    {
            //        key0 = postingLists[0].Keys[index0];
            //        merged_list.Add(key0, postingLists[0][key0]);
            //        index0++;
            //    }
            //    while (index1 < postingLists[1].Count)
            //    {
            //        key1 = postingLists[1].Keys[index1];
            //        merged_list.Add(key1, postingLists[1][key1]);
            //        index1++;
            //    }

            //    postingLists.RemoveAt(0);
            //    postingLists.RemoveAt(0);
            //    postingLists.Add(merged_list);
            //}

            //return postingLists[0]; 
            #endregion
        }

        #region old
        //private List<uint> MergePositionsLists(
        //    List<uint> list0, List<uint> list1)
        //{
        //    List<uint> merged_list = new List<uint>();

        //    int index0 = 0;
        //    int index1 = 0;

        //    uint pos0;
        //    uint pos1;

        //    while (index0 < list0.Count
        //        && index1 < list1.Count)
        //    {
        //        pos0 = list0[index0];
        //        pos1 = list1[index1];

        //        if (pos0 < pos1)
        //        {
        //            merged_list.Add(pos0);
        //            index0++;
        //        }
        //        else if (pos0 > pos1)
        //        {
        //            merged_list.Add(pos1);
        //            index1++;
        //        }
        //        else
        //        {
        //            merged_list.Add(pos0);
        //            index0++;
        //            index1++;
        //        }
        //    }
        //    while (index0 < list0.Count)
        //    {
        //        merged_list.Add(list0[index0]);
        //        index0++;
        //    }
        //    while (index1 < list1.Count)
        //    {
        //        merged_list.Add(list1[index1]);
        //        index1++;
        //    }

        //    return merged_list;
        //} 
        #endregion




        /// <summary>
        /// Holds instance of index creator.
        /// </summary>
        private static IndexCreator msIndexCreator;

        /// <summary>
        /// Separates articles in files.
        /// </summary>
        private string mArticleSeparator;

        private bool mPerformStemming;
        private bool mPerformStopWordsRemoval;
        private bool mPerformLematization;
        private bool mPerformCompression;

        private uint mDocumentIndex = 0;

        private BitStreamWriter mBitStreamWriter;       //used to compress postings

        Tokenizer tokenizer;
        Normalizer normalizer;
        Lematizer lematizer;
        Stemmer stemmer;
    }
}
