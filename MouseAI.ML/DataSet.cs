using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Numpy;

namespace MouseAI.ML
{
    public class DataSet
    {
        public NDarray Data { get; set; }
        public string Guid { get; set; }    // Guid label
    }

    public class DataSets
    {
        private int Width { get; set; }
        private int Height { get; set; }
        private string Guid { get; set; }

        private List<DataSet> TrainData { get; set; }
        private List<DataSet> TestData { get; set; }

        public DataSets(int Width, int Height, string Guid)
        {
            this.Width = Width;
            this.Height = Height;
            this.Guid = Guid;
        }
        
        public void AddTrainData(byte[] imagedata, string Guid)
        {
            DataSet ds = new DataSet();
            ds.Data = new NDarray(imagedata);
            ds.Guid = Guid;

            TrainData.Add(ds);
        }

        public void AddTestData(byte[] imagedata, string Guid)
        {
            DataSet ds = new DataSet();
            ds.Data = new NDarray(imagedata);
            ds.Guid = Guid;

            TestData.Add(ds);
        }
    }
}
