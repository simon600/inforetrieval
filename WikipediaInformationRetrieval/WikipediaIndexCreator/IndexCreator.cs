﻿using System;
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

namespace WikipediaIndexCreator
{
    /// <summary>
    /// Used for creating inversed positional index.
    /// </summary>
    class IndexCreator
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
            SortedDictionary<string, SortedList<uint, List<uint>>> inverted_index;

            // documents accesible by their id;
            Dictionary<uint, Document> documents = new Dictionary<uint,Document>();

            WordsStream words_stream = new WordsStream(sourceStream);

            // find first article
            while (!words_stream.EndOfStream && words_stream.Read() != "##TITLE##")
            {
            }

            while (sourceStream.Position < sourceStream.Length)
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
                    documents,
                    out size);

                WriteIndexToStream(inverted_index, file_stream, size);
                file_stream.Flush();                
            }

            
            MergeIndices(file_streams, destinationStream);
                       
            WriteDocumentsToStream(documents, destinationStream);
            
            destinationStream.Close();

            foreach (string name in filenames)
            {
                File.Delete(name);
            }
        }

        /////////////////////////////// PRIVATE //////////////////////////////        

        /// <summary>
        /// Default constructor.
        /// </summary>
        private IndexCreator()
        {
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
                out SortedDictionary<string, SortedList<uint, List<uint>>>
                    inversedIndex,
                Dictionary<uint, Document> documents,
                out long size)
        {
            // size of block in bytes
            size = 0;

            inversedIndex =
                new SortedDictionary<string, SortedList<uint, List<uint>>>();            
            
            List<string> article = new List<string>();
            List<string> base_forms;
            Tokenizer tokenizer = new Tokenizer();
            Normalizer normalizer = new Normalizer();
            Lematizer lematizer = new Lematizer();
            Stemmer stemmer = new Stemmer();
            string word;
            uint index_in_article = 0;
            Document document;
                        
            while (!wordsStream.EndOfStream && size < blockSize)
            {
                article.Clear();
                index_in_article = 0;
                document = new Document(wordsStream.Position);
                documents.Add(document.Id, document);

                while (!wordsStream.EndOfStream &&
                    (word = wordsStream.Read()) != "##TITLE##")
                {
                    article.Add(word);
                }

                article = tokenizer.ConvertStrings(article);

                foreach (string token in article)
                {
                    word = normalizer.Normalize(token);
                    base_forms = lematizer.LematizeString(word);

                    foreach (string base_form in base_forms)
                    {
                        if (base_form.Length > 0 &&
                            !StopWords.IsStopWord(base_form))
                        {                            
                            word = stemmer.DoStemming(base_form);
                            if (!inversedIndex.ContainsKey(word))
                            {
                                size += (long)word.Length + 1;
                                inversedIndex.Add(word,
                                    new SortedList<uint, List<uint>>());
                            }

                            if (!inversedIndex[word].ContainsKey(
                                document.Id))
                            {
                                size += sizeof(uint); // size of uint
                                inversedIndex[word].Add(
                                    document.Id, new List<uint>());
                            }

                            size += sizeof(uint);
                            inversedIndex[word][document.Id].Add(
                                index_in_article);
                        }
                    }

                    index_in_article++;
                }
            }            
        }

        /// <summary>
        /// Writes index structure to stream.
        /// </summary>
        /// <param name="inversedIndex">Index structure.</param>
        /// <param name="stream">Stream to write to.</param>
        private void WriteIndexToStream(
            SortedDictionary<string, SortedList<uint, List<uint>>> inversedIndex,
            Stream stream,
            long size)
        {
            BinaryWriter binary_writer = new BinaryWriter(stream);

            stream.SetLength(size);

            // write size of the dictionary
            binary_writer.Write(inversedIndex.Count);

            // write entries
            foreach (KeyValuePair<string, SortedList<uint, List<uint>>> entry
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
            SortedList<uint, List<uint>> postingList)
        {
            // write size
            writer.Write(postingList.Count);

            foreach (KeyValuePair<uint, List<uint>> post_pair
                    in postingList)
            {
                writer.Write(post_pair.Key);  // write doc_id
                // length of positions list
                writer.Write(post_pair.Value.Count);

                // write positions list
                foreach (uint pos in post_pair.Value)
                {
                    writer.Write(pos);
                }
            }
        }

        /// <summary>
        /// Writes list of documents to stream.
        /// </summary>
        /// <param name="documents">List of documents.</param>
        /// <param name="stream">Stream to write to.</param>
        private void WriteDocumentsToStream(
            Dictionary<uint, Document> documents,
            Stream stream)
        {
            stream.SetLength(stream.Length +
                documents.Count * (sizeof(long) + sizeof(uint)));
            BinaryWriter writer = new BinaryWriter(stream);
            foreach (KeyValuePair<uint, Document> pair in documents)
            {
                writer.Write(pair.Key);
                writer.Write(pair.Value.FilePosition);
            }
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

            foreach (FileStream file_stream in sourceStreams)
            {
                file_stream.Seek(0, SeekOrigin.Begin);
                length += file_stream.Length;
            }

            outputStream.SetLength(length);

            List<BinaryReader> readers = new List<BinaryReader>(sourceStreams.Count);
            List<int> min_words = new List<int>();
            List<SortedList<uint, List<uint>>> postingLists =
                new List<SortedList<uint,List<uint>>>();
            List<int> sizes = new List<int>(sourceStreams.Count);

            for (int i = 0; i < sourceStreams.Count; i++)
            {
                readers.Add(new BinaryReader(sourceStreams[i]));
            }

            int size = 0; // total size of merged index

            for (int i = 0; i < readers.Count; i++)
            {
                sizes.Add(readers[i].ReadInt32());
            }
            // write number of entries
            writer.Write(size);

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
                    postingLists.Add(ReadPostingList(sourceStreams[num]));
                }

                size++;
                writer.Write(words_in_streams[min_words[0]]);
                WritePostingListToStream(writer, MergePostingLists(postingLists));

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
        /// Reads posting list from stream.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <returns>Posting list read.</returns>
        private SortedList<uint, List<uint>> ReadPostingList(Stream stream)
        {
            SortedList<uint, List<uint>> posting_list =
                new SortedList<uint,List<uint>>();

            List<uint> positions_list;

            BinaryReader reader = new BinaryReader(stream);
            int posting_length;
            int positions_length;
            uint doc_id;

            posting_length = reader.ReadInt32();

            for (int i = 0; i < posting_length; i++)
            {
                doc_id = reader.ReadUInt32();
                positions_list = new List<uint>();
                posting_list.Add(doc_id, positions_list);

                positions_length = reader.ReadInt32();
                
                for (int j = 0; j < positions_length; j++)
                {
                    positions_list.Add(reader.ReadUInt32());
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
        private SortedList<uint, List<uint>> MergePostingLists(
            List<SortedList<uint, List<uint>>> postingLists)
        {
            SortedList<uint, List<uint>> merged_list;

            merged_list = new SortedList<uint,List<uint>>();
            
            foreach (SortedList<uint, List<uint>> posting_list
                in postingLists)
            {
                foreach (KeyValuePair<uint, List<uint>> pair
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


        #region old
        //private Dictionary<string, MemoryStream> CompressIndex(
        //    Dictionary<string, SortedList<uint, List<uint>>> inversedIndex)
        //{
        //    Dictionary<string, MemoryStream> compressed_index =
        //        new Dictionary<string, MemoryStream>();

        //    GZipStream gzip_stream;
        //    BinaryFormatter binary_formater = new BinaryFormatter();

        //    foreach (KeyValuePair<string, SortedList<uint, List<uint>>>
        //        pair in inversedIndex)
        //    {
        //        compressed_index[pair.Key] = new MemoryStream();
        //        gzip_stream = new GZipStream(
        //            compressed_index[pair.Key],
        //            CompressionMode.Compress,
        //            true);
        //        binary_formater.Serialize(gzip_stream, pair.Value);
        //    }

        //    return compressed_index;
        //} 
        #endregion

        /// <summary>
        /// Holds instance of index creator.
        /// </summary>
        private static IndexCreator msIndexCreator;
    }
}
