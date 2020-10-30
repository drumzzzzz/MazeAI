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
        public NDarray Data { get; set; }
        public string Label { get; set; }    // Label label
    }

    public class DataSets
    {
        private int Width { get; set; }
        private int Height { get; set; }
        private string Label { get; set; }

        private List<DataSet> TrainData { get; set; }
        private List<DataSet> TestData { get; set; }

        public DataSets(int Width, int Height, string Label)
        {
            this.Width = Width;
            this.Height = Height;
            this.Label = Label;

            TrainData = new List<DataSet>();
            TestData = new List<DataSet>();
        }
        
        public void AddTrainData(byte[] imagedata, string Label)
        {
            DataSet ds = new DataSet();
            ds.Data = new NDarray(imagedata);
            ds.Label = Label;

            TrainData.Add(ds);
        }

        public void AddTestData(byte[] imagedata, string Label)
        {
            DataSet ds = new DataSet();
            ds.Data = new NDarray(imagedata);
            ds.Label = Label;

            TestData.Add(ds);
        }

        public void Clear()
        {
            TrainData.Clear();
            TestData.Clear();
        }
    }
}
