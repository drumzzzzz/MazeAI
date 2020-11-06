using System;
using System.Configuration;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using MouseAI.BL;
using MouseAI.SH;

namespace MouseAI.UI
{
    public partial class TrainSettings : Form
    {
        private readonly Config config;
        private const int EPOCHS = 50;
        private const int BATCH = 10;
        private const int LAYERS = 1;
        private const int NODES = 100;
        private const bool NORMALIZE = true;
        private const bool EARLYSTOP = true;
        private const bool DROPOUT = true;
        private const int SEED = 0;
        private const double SPLIT = 0.70;

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
                    isNormalize = NORMALIZE,
                    isEarlyStop = EARLYSTOP,
                    isDropOut = DROPOUT,
                    Split = SPLIT,
                    Seed = SEED
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
            nudEpochs.Value = config.Epochs;
            nudBatch.Value = config.Batch;
            nudLayers.Value = config.Layers;
            nudNodes.Value = config.Nodes;
            nudSeed.Value = config.Seed;
            nudSplit.Value = Convert.ToDecimal(config.Split);
            chkNormalize.Checked = config.isNormalize;
            chkEarlyStop.Checked = config.isEarlyStop;
            chkDropOut.Checked = config.isDropOut;
        }

        private void UpdateConfig()
        {
            config.Epochs = (int)nudEpochs.Value;
            config.Batch = (int) nudBatch.Value;
            config.Layers = (int) nudLayers.Value;
            config.Nodes = (int) nudNodes.Value;
            config.Seed = (int) nudSeed.Value;
            config.Split = Convert.ToDouble(nudSplit.Value);
            config.isNormalize = chkNormalize.Checked;
            config.isEarlyStop = chkEarlyStop.Checked;
            config.isDropOut = chkDropOut.Checked;
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
    }
}
