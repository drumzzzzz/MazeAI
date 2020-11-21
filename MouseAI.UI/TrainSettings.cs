using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MouseAI.ML;

namespace MouseAI.UI
{
    public partial class TrainSettings : Form
    {
        private readonly Config config;
        private const int EPOCHS = 50;
        private const int BATCH = 10;
        private const int LAYERS = 1;
        private readonly int[] NODES = {100,100,100,100,100};
        private const bool CNN = true;
        private const bool NORMALIZE = true;
        private const bool EARLYSTOP = false;
        private readonly double[] DROPOUT = {0.050,0.125, 0.250, 0.400,0.500};
        private const int SEED = 60;
        private const double SPLIT = 0.70;
        private const double LEARN_RATE = 0.001;
        private const double LEARN_DECAY = 0.00;
        private const int MAX_LAYERS = 5;
        private List<NeuralLayer> neurallayers;

        public TrainSettings(Config config)
        {
            InitializeComponent();

            if (config == null)
            {
                this.config = new Config
                {
                    Epochs = EPOCHS,
                    Batch = BATCH,
                    Layers = LAYERS,
                    Nodes = NODES,
                    isCNN = CNN,
                    isNormalize = NORMALIZE,
                    isEarlyStop = EARLYSTOP,
                    DropOut = DROPOUT,
                    Split = SPLIT,
                    Seed = SEED,
                    LearnRate = LEARN_RATE,
                    LearnDecay = LEARN_DECAY
                };
            }
            else
                this.config = config;

            UpdateControls();
        }

        public Config GetConfig()
        {
            UpdateConfig();
            return config;
        }

        private void UpdateControls()
        {
            neurallayers = new List<NeuralLayer>();
            nudEpochs.Value = config.Epochs;
            nudBatch.Value = config.Batch;
            nudLayers.Maximum = MAX_LAYERS;
            nudLayers.Value = config.Layers;
            nudSeed.Value = config.Seed;
            nudSplit.Value = Convert.ToDecimal(config.Split);
            nudLearnRate.Value = Convert.ToDecimal(config.LearnRate);
            nudLearnDecay.Value = Convert.ToDecimal(config.LearnDecay);
            chkCNN.Checked = config.isCNN;
            chkNormalize.Checked = config.isNormalize;
            chkEarlyStop.Checked = config.isEarlyStop;
            SetNeuralLayers();
        }

        private void SetNeuralLayers()
        {
            int layers = config.Layers;

            if (config.Nodes.Length != MAX_LAYERS || config.DropOut.Length != MAX_LAYERS)
                throw new Exception("Invalid network config!");

            int index = 0;
            for (int i = MAX_LAYERS - 1; i > -1; i--)
            {
                NeuralLayer nl = new NeuralLayer();
                nl.nudNodes.Value = Convert.ToDecimal(config.Nodes[index]);
                nl.nudDropout.Value = Convert.ToDecimal(config.DropOut[index]);
                nl.Enabled = (index < layers);
                pnlLayers.Controls.Add(nl);
                nl.Location = new Point(1, (i * nl.Height) + 1);
                neurallayers.Add(nl);
                index++;
            }
        }

        private void UpdateConfig()
        {
            config.Epochs = (int)nudEpochs.Value;
            config.Batch = (int) nudBatch.Value;
            config.Layers = (int) nudLayers.Value;
            config.Seed = (int) nudSeed.Value;
            config.Split = Convert.ToDouble(nudSplit.Value);
            config.LearnRate = Convert.ToDouble(nudLearnRate.Value);
            config.LearnDecay = Convert.ToDouble(nudLearnDecay.Value);
            config.isCNN = chkCNN.Checked;
            config.isNormalize = chkNormalize.Checked;
            config.isEarlyStop = chkEarlyStop.Checked;

            int index = 0;
            foreach (NeuralLayer nl in neurallayers)
            {
                config.Nodes[index] = (int)nl.nudNodes.Value;
                config.DropOut[index] = Convert.ToDouble(nl.nudDropout.Value);
                index++;
            }
        }

        private void btnTrain_Click(object sender, EventArgs e)
        {
            UpdateConfig();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void nudLayers_ValueChanged(object sender, EventArgs e)
        {
            int index = 0;
            foreach (NeuralLayer nl in neurallayers)
            {
                nl.Enabled = (index++ < nudLayers.Value);
            }
        }
    }
}
