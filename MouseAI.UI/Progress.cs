
using System.Windows.Forms;

namespace MouseAI.UI
{
    public partial class Progress : Form
    {
        public Progress(string message, bool isCancelVisible)
        {
            InitializeComponent();

            btnProgressCancel.Visible = isCancelVisible;
            Text = message;
        }
    }
}
