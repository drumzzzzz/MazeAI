
using System.Collections.Generic;
using System.Windows.Forms;

namespace MouseAI.UI
{
    public partial class MazeManage : Form
    {
        public MazeManage(IEnumerable<string> project_files)
        {
            InitializeComponent();

            foreach (string project_file in project_files)
            {
                lbxProjects.Items.Add(project_file);
            }
        }
    }
}
