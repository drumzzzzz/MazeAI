
using System.Collections.Generic;
using System.Windows.Forms;

namespace MouseAI.UI
{
    public partial class MazeManage : Form
    {
        public MazeManage()
        {
            InitializeComponent();
        }

        public void SetProjectFiles(IEnumerable<string> project_files)
        {
            lbxProjects.Items.Clear();
            foreach (string project_file in project_files)
            {
                lbxProjects.Items.Add(project_file);
            }
        }
    }
}
