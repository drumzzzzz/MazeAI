using System.Collections.Generic;
using System.Windows.Forms;

namespace MouseAI.UI
{
    public partial class ModelLoad : Form
    {
        public ModelLoad(IEnumerable<string> starttimes, string starttime)
        {
            InitializeComponent();

            foreach (string s in starttimes)
                lbxModels.Items.Add(s);

            if (!string.IsNullOrEmpty(starttime))
            {
                lbxModels.SelectedItem = starttime;
                btnPredict.Enabled = true;
                btnRun.Enabled = true;
            }
        }
    }
}
