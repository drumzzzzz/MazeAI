// ModleInfo Class:
// Form for displaying the advanced information of a selected trained neural model

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
