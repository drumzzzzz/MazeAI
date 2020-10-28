using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MouseAI.ML;

namespace MouseAI.BL
{
    public class MazeNn
    {
        public static void TestMnist()
        {
            NeuralNet nn = new NeuralNet(28,28);
            nn.TestMnist();
        }

    }
}
