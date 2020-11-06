using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MouseAI.SH
{
    public class Config
    {
        public int Epochs { get; set; }
        public int Batch { get; set; }
        public int Layers { get; set; }
        public int Nodes { get; set; }
        public bool isNormalize { get; set; }
        public bool isEarlyStop { get; set; }
        public bool isDropOut { get; set; }
    }
}
