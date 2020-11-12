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
    public partial class ModelRun : Form
    {
        private List<Button> buttons;

        public ModelRun()
        {
            InitializeComponent();

            buttons = new List<Button>();

            foreach (Control ctl in Controls)
            {
                if (ctl is Button)
                {
                    buttons.Add((Button)ctl);
                }
            }
        }

        public void ResetButtons()
        {
            foreach (Button btn in buttons)
            {
                btn.Enabled = false;
            }
        }

    }
}
