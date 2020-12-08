// ModelLoad Class:
// Form for selecting and loading a trained neural model
// Provides a method for viewing a selected CSV file on users system

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace MouseAI.UI
{
    public partial class ModelLoad : Form
    {
        private readonly Maze maze;

        public ModelLoad(Maze maze, IEnumerable<string> starttimes)
        {
            InitializeComponent();

            this.maze = maze;

            foreach (string s in starttimes)
                lbxModels.Items.Add(s);

            string starttime = maze.GetProjectModelName();

            if (!string.IsNullOrEmpty(starttime))
            {
                lbxModels.SelectedItem = starttime;
            }
        }

        public bool isSelectedModel()
        {
            return (lbxModels.SelectedItem != null);
        }

        public string GetSelectedModel()
        {
            return lbxModels.SelectedItem.ToString();
        }

        public void SetButtonStates(bool state)
        {
            btnPredict.Enabled = state;
            btnRun.Enabled = state;
            btnDelete.Enabled = state;
            btnModel.Enabled = state;
            btnExport.Enabled = state;
        }

        private void ModelLoad_Shown(object sender, EventArgs e)
        {
            string modelast = maze.GetProjectLast(maze.GetModelProjectGuid());
            ListBox lbx = lbxModels;

            if (lbx.Items.Count != 0 && lbx.Items.Contains(modelast))
            {
                lbxModels.SelectedItem = modelast;
                lbx.SetSelected(lbx.SelectedIndex, true);
            }
            else
            { 
                lbxModels.SelectedItem = lbxModels.Items[0];
            }
        }

        private void btnModel_Click(object sender, EventArgs e)
        {
            ModelInfo model_info = new ModelInfo(Maze.GetModelInfo());
            model_info.ShowDialog();
        }

        private void llblLog_Click(object sender, EventArgs e)
        {
            string link = (string)llblLog.Tag;
            if (string.IsNullOrEmpty(link))
                return;

            Process process = new Process
            {
                StartInfo = { FileName = link }
            };

            try
            {
                process.Start();
            }
            catch (Exception ex)
            {
                string message = string.Format("Error opening log file '{0}': {1}", link, ex.Message);
                Console.WriteLine(message);
                MessageBox.Show(message);
            }
        }

        private void lbxModels_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isSelectedModel())
            {
                string starttime = GetSelectedModel();
                string summary = Maze.GetModelSummary(starttime);

                if (!string.IsNullOrEmpty(summary))
                {
                    tbxSummary.Text = summary;
                    SetButtonStates(true);

                    string plot = Maze.GetModelPlot(starttime);
                    pbxPlot.Image = (!string.IsNullOrEmpty(plot)) ? Image.FromFile(plot) : null;
                    llblLog.Tag = maze.GetLogPath(starttime);
                    llblLog.Text = string.Format("Log: {0}", maze.GetLogFileName(starttime));
                    return;
                }
            }
            SetButtonStates(false);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
