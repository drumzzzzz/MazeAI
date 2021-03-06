﻿// DataSets Class:
// Prepares formatted datasets from supplied images for Neural network training and predictions
// Sets can be split upon a given ratio of training : test data and with a random seed
// Images are indexed with int 0->N labeling that references an associated maze and Guid

using System;
using System.Collections.Generic;
using System.Linq;
using Numpy;

namespace MouseAI.ML
{
    public class DataSets
    {
        private readonly Random r;
        private static int ImageSize;
        private const int HEADER_SIZE = 54;
        private const int BYTE_NUM = 4;
        
        private readonly ImageDatas imageDatas;
        private readonly double split;

        private List<int> GetRandomList(int min, int max)
        {
            List<int> Values = new List<int>();
            List<int> RandomValues = new List<int>();

            for (int i = min; i < max; i++)
            {
                Values.Add(i);
            }

            int index;
            while (Values.Count != 0)
            {
                index = r.Next(Values.Count - 1);
                RandomValues.Add(Values[index]);
                Values.RemoveAt(index);
            }
            return RandomValues;
        }

        public DataSets(int Width, int Height, ImageDatas imageDatas, double split, int seed)
        {
            if (split < 0 || split > 1.00)
                throw new Exception(string.Format("Invalid split parameter: {0}", split));
            
            ImageSize = (Width * Height);

            this.imageDatas = imageDatas;
            this.split = split;

            if (seed != 0)
                r = new Random(seed);
            else
                r = new Random();
        }

        public DataSets(int Width, int Height, ImageDatas imageDatas, int seed)
        {
            ImageSize = (Width * Height);
            this.imageDatas = imageDatas;
            imageDatas.InitLabelIndexes();
        }

        public bool isImageDatas()
        {
            return (imageDatas != null && imageDatas.Count != 0);
        }

        public ((NDarray, NDarray), (NDarray, NDarray)) BuildDataSets()
        {
            List<string> Labels = imageDatas.GetLabels();

            if (Labels == null || Labels.Count == 0)
                throw new Exception("Invalid Dataset Labels!");

            List<byte[]> train = new List<byte[]>();
            List<int> train_labels = new List<int>();
            List<byte[]> test = new List<byte[]>();
            List<int> test_labels = new List<int>();

            if (BuildSets(train, train_labels, test, test_labels, Labels) != imageDatas.Count)
                throw new Exception(string.Format("Error creating data sets: Expected:{0}", imageDatas.Count));

            (NDarray, NDarray) tuple1 = GetDataSets(train, train_labels);
            (NDarray, NDarray) tuple2 = GetDataSets(test, test_labels);
            return (tuple1, tuple2);
        }

        public NDarray BuildDataSet()
        {
            List<string> Labels = imageDatas.GetLabels();

            if (Labels == null || Labels.Count == 0)
                throw new Exception("Invalid Dataset Labels!");

            List<byte[]> x_data = new List<byte[]>();
            List<int> y_labels = new List<int>();

            if (BuildSet(x_data, y_labels, Labels) != imageDatas.Count)
                throw new Exception(string.Format("Error creating data sets: Expected:{0}", imageDatas.Count));

            return GetDataSet(x_data);
        }

        private int BuildSets(ICollection<byte[]> train, ICollection<int> train_labels, ICollection<byte[]> test, ICollection<int> test_labels, IEnumerable<string> Labels)
        {
            List<ImageData> id;
            List<int> indexes;
            int label_count = 0;
            int set_count = 0;
            int count, sum, train_count, index;

            Console.WriteLine("Splitting data sets by {0}%", split * 100);

            foreach (string label in Labels)
            {
                id = imageDatas.Where(o => o.Label == label).ToList();
                if (id.Count == 0)
                {
                    throw new Exception(string.Format("Image Data label for {0} not found!", label));
                }

                count = id.Count;
                sum = 0;
                // Get random index 1 -> n - 1
                indexes = GetRandomList(0, count);
                // Insert first and last into training
                //indexes.Insert(0, 0);
                //indexes.Insert(1, count - 1);
                train_count = (int)(count * split);

                for (int i = 0; i < train_count; i++)
                {
                    index = indexes[i];
                    train.Add(id[index].Data);
                    train_labels.Add(label_count);
                    id[index].isTrain = true;
                    id[index].Reference = train.Count - 1;
                    sum++;
                    set_count++;
                }
                //indexes.RemoveAt(0);
                //indexes.RemoveAt(0);
                indexes.RemoveRange(0, train_count);

                foreach (int idx in indexes)
                {
                    test.Add(id[idx].Data);
                    test_labels.Add(label_count);
                    id[idx].isTrain = false;
                    id[idx].Reference = test.Count - 1;
                    sum++;
                    set_count++;
                }

                if (sum != count)
                    throw new Exception("Invalid Data Selection!");

                label_count++;
            }
            return set_count;
        }

        private int BuildSet(ICollection<byte[]> x_data, ICollection<int> y_labels, IEnumerable<string> Labels)
        {
            List<ImageData> id;
            int label_count = 0;
            int set_count = 0;
            int count;

            Console.WriteLine("Creating data set ...");

            foreach (string label in Labels)
            {
                id = imageDatas.Where(o => o.Label == label).ToList();
                if (id.Count == 0)
                {
                    throw new Exception(string.Format("Image Data label for {0} not found!", label));
                }

                count = id.Count;

                for (int i = 0; i < count; i++)
                {
                    x_data.Add(id[i].Data);
                    y_labels.Add(label_count);
                    id[i].Reference = x_data.Count - 1;
                    set_count++;
                }
                label_count++;
            }
            return set_count;
        }

        private static (NDarray, NDarray) GetDataSets(IReadOnlyList<byte[]> data, IReadOnlyList<int> labels)
        {
            int count = data.Count;
            int[,] x_data = new int[count, ImageSize];
            int[] y_labels = new int[count];

            byte[] bytes;

            for (int i = 0; i < count; i++)
            {
                bytes = data[i];

                for (int byteIdx = 0; byteIdx < ImageSize; byteIdx++)
                {
                    x_data[i, byteIdx] =  bytes[GetByteOffset(byteIdx)];
                }
                y_labels[i] = labels[i];
            }
            return (x_data, y_labels);
        }

        public NDarray GetDataSet(IReadOnlyList<byte[]> data)
        {
            int count = data.Count;
            int[,] x_data = new int[count, ImageSize];
            byte[] bytes;

            for (int i = 0; i < count; i++)
            {
                bytes = data[i];

                for (int byteIdx = 0; byteIdx < ImageSize; byteIdx++)
                {
                    x_data[i, byteIdx] = bytes[GetByteOffset(byteIdx)];
                }
            }
            return x_data;
        }

        public ImageDatas GetImageDatas()
        {
            return imageDatas;
        }

        private static int GetByteOffset(int index)
        {
            return HEADER_SIZE + (index * BYTE_NUM);
        }
    }
}
