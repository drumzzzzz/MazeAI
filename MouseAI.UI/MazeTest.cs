﻿using System;
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
    public partial class MazeTest : Form
    {
        public MazeTest(IEnumerable<string> starttimes, string starttime)
        {
            InitializeComponent();

            foreach (string s in starttimes)
                lbxModels.Items.Add(s);

            if (!string.IsNullOrEmpty(starttime))
                lbxModels.SelectedItem = starttime;
        }
    }
}