using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MouseAI.UI
{
    public partial class ModelInfo : Form
    {
        public ModelInfo(string model_info)
        {
            InitializeComponent();
            tbxModelInfo.Text = string.IsNullOrEmpty(model_info) ? "No Data" : model_info;
        }
    }
}
