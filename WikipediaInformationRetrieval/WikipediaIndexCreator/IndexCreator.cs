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

namespace WikipediaIndexCreator
{
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

        public void CreateIndexFromStream(
            Stream sourceStream,
            Stream destinationStream)
        {
            const ulong BLOCK_SIZE = 1000000;

            uint file_number = 0;   // number of temporary file
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
                    documents);

                //WriteIndexToStream(inverted_index, file_stream);
                WriteIndexToStream(inverted_index, destinationStream);
                //file_stream.Flush();
                break;
            }

            
            //MergeIndices(file_streams, destinationStream);
            
            WriteDocumentsToStream(documents, destinationStream);

            destinationStream.Close();

            //foreach (string name in filenames)
            //{
            //    File.Delete(name);
            //}
        }

        /////////////////////////////// PRIVATE //////////////////////////////        

        /// <summary>
        /// Default constructor.
        /// </summary>
        private IndexCreator()
        {
        }

        private void
            CreateInversedIndex(
                WordsStream wordsStream,
                ulong blockSize,
                out SortedDictionary<string, SortedList<uint, List<uint>>>
                    inversedIndex,
                Dictionary<uint, Document> documents)
        {
            // size of block in bytes
            ulong size = 0;

            inversedIndex =
                new SortedDictionary<string, SortedList<uint, List<uint>>>();            
            
            List<string> article = new List<string>();
            List<string> base_forms;
            Tokenizer tokenizer = new Tokenizer();
            Lematizer lematizer = new Lematizer();
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
                    base_forms = lematizer.LematizeString(token);

                    foreach (string base_form in base_forms)
                    {
                        if (base_form.Length > 0)
                        {
                            if (!inversedIndex.ContainsKey(base_form))
                            {
                                size += (ulong)base_form.Length + 1;
                                inversedIndex.Add(base_form,
                                    new SortedList<uint, List<uint>>());
                            }

                            if (!inversedIndex[base_form].ContainsKey(
                                document.Id))
                            {
                                size += sizeof(uint); // size of uint
                                inversedIndex[base_form].Add(
                                    document.Id, new List<uint>());
                            }

                            size += sizeof(uint);
                            inversedIndex[base_form][document.Id].Add(
                                index_in_article);
                        }
                    }

                    index_in_article++;
                }
            }            
        }

        private void WriteIndexToStream(
            SortedDictionary<string, SortedList<uint, List<uint>>> inversedIndex,
            Stream stream)
        {
            BinaryWriter binary_writer = new BinaryWriter(stream);

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

        private void WriteDocumentsToStream(
            Dictionary<uint, Document> documents,
            Stream stream)
        {
            BinaryWriter writer = new BinaryWriter(stream);
            foreach (KeyValuePair<uint, Document> pair in documents)
            {
                writer.Write(pair.Key);
                writer.Write(pair.Value.FilePosition);
            }
        }

        private void MergeIndices(
            List<FileStream> sourceStreams,
            Stream outputStream)
        {
            CaseInsensitiveComparer comparer = new CaseInsensitiveComparer();
            int cmp;
            BinaryWriter writer = new BinaryWriter(outputStream);

            foreach (FileStream file_stream in sourceStreams)
            {
                file_stream.Seek(0, SeekOrigin.Begin);
            }

            List<BinaryReader> readers = new List<BinaryReader>(sourceStreams.Count);
            List<int> min_words = new List<int>();
            List<SortedList<uint, List<uint>>> postingLists =
                new List<SortedList<uint,List<uint>>>();

            for (int i = 0; i < sourceStreams.Count; i++)
            {
                readers.Add(new BinaryReader(sourceStreams[i]));
            }

            int size = 0; // total size of merged index

            foreach (BinaryReader reader in readers)
            {
                size += reader.ReadInt32();
            }
            // write number of entries
            writer.Write(size);

            List<string> words_in_streams = new List<string>(sourceStreams.Count);

            for (int i = 0; i < readers.Count; i++)
            {
                words_in_streams.Add(readers[i].ReadString());                
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
                if (words_in_streams[min_words[0]].Contains("\0\0\0"))
                {
                    int a = 0;
                }
                postingLists.Clear();
                foreach (int num in min_words)
                {
                    postingLists.Add(ReadPostingList(sourceStreams[num]));
                }

                writer.Write(words_in_streams[min_words[0]]);
                WritePostingListToStream(writer, MergePostingLists(postingLists));

                foreach (int num in min_words)
                {
                    if (sourceStreams[num].Position < sourceStreams[num].Length)
                    {
                        words_in_streams[num] = readers[num].ReadString();
                    }
                }

                for (int s_num = 0; s_num < sourceStreams.Count; s_num++)
                {
                    if (sourceStreams[s_num].Position >=
                        sourceStreams[s_num].Length)
                    {
                        sourceStreams[s_num].Close();
                        sourceStreams.RemoveAt(s_num);
                        words_in_streams.RemoveAt(s_num);
                        //min_words.RemoveAt(s_num);
                        readers.RemoveAt(s_num);
                        s_num--;
                    }
                }
            }

        }

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
                doc_id = (uint)reader.ReadInt32();
                positions_list = new List<uint>();
                posting_list.Add(doc_id, positions_list);

                positions_length = reader.ReadInt32();
                
                for (int j = 0; j < positions_length; j++)
                {
                    positions_list.Add((uint)reader.ReadInt32());
                }
            }

            return posting_list;
        }

        private SortedList<uint, List<uint>> MergePostingLists(
            List<SortedList<uint, List<uint>>> postingLists)
        {
            SortedList<uint, List<uint>> merged_list;

            int index0;
            int index1;

            uint key0;
            uint key1;

            if (postingLists.Count < 1)
            {
                throw new Exception("Not enough posting lists.");
            }

            while (postingLists.Count > 1)
            {
                merged_list = new SortedList<uint, List<uint>>();

                index0 = index1 = 0;

                while (index0 < postingLists[0].Count
                    && index1 < postingLists[1].Count)
                {
                    key0 = postingLists[0].Keys[index0];
                    key1 = postingLists[1].Keys[index1];

                    if (key0 < key1)
                    {
                        merged_list.Add(key0, postingLists[0][key0]);
                        index0++;
                    }
                    else if (key0 > key1)
                    {
                        merged_list.Add(key1, postingLists[1][key1]);
                        index1++;
                    }
                    else
                    {
                        merged_list.Add(
                            key0,
                            MergePositionsLists(
                                postingLists[0][key0],
                                postingLists[1][key1]));
                        index0++;
                        index1++;
                    }
                }
                while (index0 < postingLists[0].Count)
                {
                    key0 = postingLists[0].Keys[index0];
                    merged_list.Add(key0, postingLists[0][key0]);
                    index0++;
                }
                while (index1 < postingLists[1].Count)
                {
                    key1 = postingLists[1].Keys[index1];
                    merged_list.Add(key1, postingLists[1][key1]);
                    index1++;
                }

                postingLists.RemoveAt(0);
                postingLists.RemoveAt(0);
                postingLists.Add(merged_list);
            }

            return postingLists[0];
        }

        private List<uint> MergePositionsLists(
            List<uint> list0, List<uint> list1)
        {
            List<uint> merged_list = new List<uint>();

            int index0 = 0;
            int index1 = 0;

            uint pos0;
            uint pos1;

            while (index0 < list0.Count
                && index1 < list1.Count)
            {
                pos0 = list0[index0];
                pos1 = list1[index1];

                if (pos0 < pos1)
                {
                    merged_list.Add(pos0);
                    index0++;
                }
                else if (pos0 > pos1)
                {
                    merged_list.Add(pos1);
                    index1++;
                }
                else
                {
                    merged_list.Add(pos0);
                    index0++;
                    index1++;
                }
            }
            while (index0 < list0.Count)
            {
                merged_list.Add(list0[index0]);
                index0++;
            }
            while (index1 < list1.Count)
            {
                merged_list.Add(list1[index1]);
                index1++;
            }

            return merged_list;
        }

        private Dictionary<string, MemoryStream> CompressIndex(
            Dictionary<string, SortedList<uint, List<uint>>> inversedIndex)
        {
            Dictionary<string, MemoryStream> compressed_index =
                new Dictionary<string, MemoryStream>();
            
            GZipStream gzip_stream;
            BinaryFormatter binary_formater = new BinaryFormatter();

            foreach (KeyValuePair<string, SortedList<uint, List<uint>>>
                pair in inversedIndex)
            {
                compressed_index[pair.Key] = new MemoryStream();
                gzip_stream = new GZipStream(
                    compressed_index[pair.Key],
                    CompressionMode.Compress,
                    true);
                binary_formater.Serialize(gzip_stream, pair.Value);
            }

            return compressed_index;
        }

        private static IndexCreator msIndexCreator;
    }
}
