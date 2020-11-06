
using System.Windows.Forms;

namespace MouseAI.UI
{
    public partial class Progress : Form
    {
        public Progress(string name, bool isCancelVisible)
        {
            InitializeComponent();

            btnProgressCancel.Visible = isCancelVisible;
            lblProgress.Text = name;
        }

        public void SetCancel(bool isCancel)
        {
            btnProgressCancel.Enabled = isCancel;
        }

        public bool isCancel()
        {
            return btnProgressCancel.Enabled;
        }
    }
}
