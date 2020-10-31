using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Numpy;

namespace MouseAI.ML
{
    public class ImageData
    {
        public byte[] Data;
        public string Label;

        public ImageData(byte[] Data, string Label)
        {
            this.Data = Data;
            this.Label = Label;
        }
    }

    public class ImageDatas : List<ImageData>
    {
    }

    public class DataSet
    {
        public int[] Data;
        public string Label { get; set; }   
    }

    public class DataSets
    {
        private int Width;
        private int Height;
        private static int ImageSize;
        private const int HEADER_SIZE = 54;
        private const int BYTE_NUM = 4;
        private string Label { get; set; }

        private int[,] X_Train_Data;
        private int[,] Y_Train_Data;
        private int[,] X_Test_Data;
        private int[,] Y_Test_Data;
        private List<string> TrainLabels;
        private List<string> TestLabels;
        private int train_idx = 0;
        private int test_idx = 0;

        // X: length x width pixel data, y: n Images
        // X: 1 column, y: label per n images 
        public NDarray X_train; 
        public NDarray Y_train; 
        public NDarray X_test;
        public NDarray Y_test;

        public DataSets(int Width, int Height, int Train_Count, int Test_Count, string Label)
        {
            this.Width = Width;
            this.Height = Height;
            ImageSize = (Width * Height);
            this.Label = Label;

            // X: Width * Height in pixels, Count in nSamples
            // Y: nSamples, 1 column
            X_Train_Data = new int[Train_Count, ImageSize];
            Y_Train_Data = new int[Train_Count, 1];
            X_Test_Data = new int[Test_Count, ImageSize];
            Y_Test_Data = new int[Test_Count, 1];

            TrainLabels = new List<string>();
            TestLabels = new List<string>();

            for (int idx = 0; idx < Train_Count; idx++)
            {
                Y_Train_Data[idx, 0] = idx;
            }

            for (int idx = 0; idx < Test_Count; idx++)
            {
                Y_Test_Data[idx, 0] = idx;
            }
        }

        public ((NDarray, NDarray), (NDarray, NDarray)) GetData()
        {
            (NDarray, NDarray) tuple1 = (X_train, Y_train);
            (NDarray, NDarray) tuple2 = (X_test, Y_test);
            return (tuple1, tuple2);
        }

        public void BuildArrays()
        {
            X_train = X_Train_Data;
            Y_train = Y_Train_Data;
            X_test = X_Test_Data;
            Y_test = Y_Test_Data;

            Console.WriteLine("X_Train:{0} Y_Train:{1} X_Test:{2} Y_Test:{3}",X_train.shape, Y_train.shape, X_test.shape, Y_test.shape);
        }
        
        public void AddTrainData(byte[] imagedata, string label)
        {
            if (train_idx > X_Train_Data.Length)
                throw new Exception("Train Index Excedded!");

            TrainLabels.Add(label);

            for (int idx = 0; idx < ImageSize; idx++)
            {
                X_Train_Data[train_idx, idx] = imagedata[GetByteOffset(idx)];
            }

            train_idx++;
        }

        private static int GetByteOffset(int index)
        {
            return HEADER_SIZE + (index * BYTE_NUM);
        }

        public void AddTestData(byte[] imagedata, string label)
        {
            if (test_idx > X_Test_Data.Length)
                throw new Exception("Test Index Excedded!");

            TestLabels.Add(label);

            for (int idx = 0; idx < ImageSize; idx++)
                X_Test_Data[test_idx, idx] = imagedata[GetByteOffset(idx)];

            test_idx++;
        }
    }
}
