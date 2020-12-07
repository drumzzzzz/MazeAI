// ImageData and ImageDatas Class:
// Image array, associated maze label (Guid) and properties of a maze path segment instance
// ImageData object list containing the path segments associated to a maze instance

using System;
using System.Collections.Generic;
using System.Linq;

namespace MouseAI.ML
{
    public class ImageData
    {
        public byte[] Data { get; }
        public string Label { get; }
        public int Reference { get; set; }
        public bool isTrain { get; set; }
        public int Index { get; set; }

        public ImageData(byte[] Data, string Label)
        {
            this.Data = Data;
            this.Label = Label;
        }
    }

    public class ImageDatas : List<ImageData>
    {
        private readonly List<string> labels;
        private string results;

        public ImageDatas(List<string> labels)
        {
            this.labels = labels;
        }

        public ImageDatas()
        {
        }

        public List<string> GetLabels()
        {
            return labels;
        }

        public void SetResults(string value)
        {
            results = value;
        }

        public string GetResults()
        {
            return string.IsNullOrEmpty(results) ? string.Empty : results;
        }

        public bool isLabelValid(int predicted, string expected, bool isDebug)
        {
            if (predicted < 0 || predicted > Count)
            {
                throw new Exception(string.Format("Predicted was out of range index: {0} size: {1}", predicted, Count));
            }

            ImageData id = this.FirstOrDefault(o => o.Index == predicted);
            if (id == null)
            {
                throw new Exception(string.Format("Prediction failed for image data index: {0}", predicted));
            }

            if (isDebug)
                Console.WriteLine("Predicted:{0} Expected:{1}", id.Label, expected);

            return (id.Label.Equals(expected, StringComparison.OrdinalIgnoreCase));
        }

        public void InitLabelIndexes()
        {
            if (Count == 0)
                return;

            string lbl = this.ElementAt(0).Label;
            int index = 0;

            foreach (ImageData id in this)
            {
                if (!id.Label.Equals(lbl, StringComparison.OrdinalIgnoreCase))
                {
                    index++;
                    lbl = id.Label;
                }
                id.Index = index;
            }
        }
    }
}
