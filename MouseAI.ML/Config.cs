namespace MouseAI.ML
{
    public class Config
    {
        public int Epochs { get; set; }
        public int Batch { get; set; }
        public int[] Nodes { get; set; }
        public double[] DropOut { get; set; }
        public bool isCNN { get; set; }
        public bool isNormalize { get; set; }
        public bool isEarlyStop { get; set; }
        public int Seed { get; set; }
        public double Split { get; set; }
        public double LearnRate { get; set; }
        public double LearnDecay { get; set; }
        public string Guid { get; set; }
        public string StartTime { get; set; }
        public int Layers { get; set; }
        public string Model { get; set; }
    }
}
