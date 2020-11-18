using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScottPlot;
using ScottPlot.Config;

namespace MouseAI.UI
{
    public partial class ModelRun : Form
    {
        private readonly List<Button> buttons;
        private FormsPlot plot;
        private int columns_length;
        private const int X_MARGIN = 10;
        private const int Y_MARGIN = 10;
        private const int X_POSITION = 5;
        private const int Y_POSITION = 5;
        private List<Plottable> plottables;

        public ModelRun(string[] columns)
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

            InitPlot(columns);
        }

        public void UpdatePlot(double[] data)
        {
            if (plot == null || data.Length != columns_length)
            {
                Console.WriteLine("Plot Error!");
                return;
            }

            double[] xs = DataGen.Consecutive(columns_length);
            //double[] ys = DataGen.Consecutive(columns_length, 10, 0);

            plot.plt.Clear();
            plot.plt.PlotBar(xs, data, valueColor: Color.RoyalBlue, fillColor:Color.RoyalBlue);
            plot.Render();
        }

        public void InitPlot(string[] columns)
        {
            plot = new FormsPlot();
            columns_length = columns.Length;
            double[] xs = DataGen.Consecutive(columns_length);
            double[] ys = DataGen.Consecutive(columns_length, 0, 0);

            try
            {
                plot.plt.PlotBar(xs, ys);
                plot.plt.Axis(y1: 0);
                plot.plt.Grid(enableVertical: false, lineStyle: LineStyle.Dot);
                plot.plt.XTicks(xs, columns);
                plot.Width = pnlPlot.Width - X_MARGIN - X_POSITION;
                plot.Height = pnlPlot.Height - Y_MARGIN - Y_POSITION;
                plot.Location = new Point(X_POSITION, Y_POSITION);
                plot.AutoSize = false;
                plot.plt.XLabel("Running Statistics");
                plot.plt.AxisAuto(horizontalMargin: .1, verticalMargin: 0);
                pnlPlot.Controls.Add(plot);
                plot.Render();
                plottables = plot.plt.GetPlottables();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error initializing plot: {0}", e.Message);
                plot = null;
            }
        }

        public void ResetButtons()
        {
            foreach (Button btn in buttons)
            {
                btn.Enabled = false;
            }
        }

        private void ModelRun_Shown(object sender, EventArgs e)
        {

        }
    }
}
