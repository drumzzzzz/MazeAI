﻿using System;
using System.Windows.Forms;
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
        private const bool CNN = false;
        private const bool NORMALIZE = true;
        private const bool EARLYSTOP = false;
        private const double DROPOUT = 0.05;
        private const int AMOUNT = 1;
        private const int SEED = 0;
        private const double SPLIT = 0.70;
        private const double LEARN_RATE = 0.001;
        private const double LEARN_DECAY = 0.00;

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
                    DropOutLayers = AMOUNT,
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
            nudEpochs.Value = config.Epochs;
            nudBatch.Value = config.Batch;
            nudLayers.Value = config.Layers;
            nudNodes.Value = config.Nodes;
            nudSeed.Value = config.Seed;
            nudSplit.Value = Convert.ToDecimal(config.Split);
            nudDropOut.Value = Convert.ToDecimal(config.DropOut);
            nudDropOutLayers.Value = config.DropOutLayers;
            nudLearnRate.Value = Convert.ToDecimal(config.LearnRate);
            nudLearnDecay.Value = Convert.ToDecimal(config.LearnDecay);
            chkCNN.Checked = config.isCNN;
            chkNormalize.Checked = config.isNormalize;
            chkEarlyStop.Checked = config.isEarlyStop;
        }

        private void UpdateConfig()
        {
            config.Epochs = (int)nudEpochs.Value;
            config.Batch = (int) nudBatch.Value;
            config.Layers = (int) nudLayers.Value;
            config.Nodes = (int) nudNodes.Value;
            config.Seed = (int) nudSeed.Value;
            config.Split = Convert.ToDouble(nudSplit.Value);
            config.DropOut = Convert.ToDouble(nudDropOut.Value);
            config.DropOutLayers = (int) nudDropOutLayers.Value;
            config.LearnRate = Convert.ToDouble(nudLearnRate.Value);
            config.LearnDecay = Convert.ToDouble(nudLearnDecay.Value);
            config.isCNN = chkCNN.Checked;
            config.isNormalize = chkNormalize.Checked;
            config.isEarlyStop = chkEarlyStop.Checked;
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
