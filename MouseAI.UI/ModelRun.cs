using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ScottPlot;

namespace MouseAI.UI
{
    public partial class ModelRun : Form
    {
        private readonly List<Button> buttons;
        private FormsPlot run_plot;
        private FormsPlot runall_plot;
        private int columns_length;
        private const int X_MARGIN = 10;
        private const int Y_MARGIN = 10;
        private const int X_POSITION = 5;
        private const int Y_POSITION = 5;

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

        public void UpdatePlot(double[] data, bool isRunAll)
        {
            if (run_plot == null || runall_plot == null || data.Length != columns_length)
            {
                Console.WriteLine("Plot Error!");
                return;
            }

            double[] xs = DataGen.Consecutive(columns_length);

            FormsPlot plot = (isRunAll) ? runall_plot : run_plot;

            plot.plt.Clear();
            plot.plt.PlotBar(xs, data, fillColor:Color.Blue);
            plot.Render();
        }

        public void InitPlot(string[] columns)
        {
            columns_length = columns.Length;
            double[] xs = DataGen.Consecutive(columns_length);
            double[] ys = DataGen.Consecutive(columns_length, 0, 0);

            try
            {
                run_plot = GetPlot(xs, ys, columns, "Maze");
                run_plot.Width = (pnlPlot.Width / 2)  - X_MARGIN - X_POSITION;
                run_plot.Height = pnlPlot.Height - Y_MARGIN - Y_POSITION;
                run_plot.Location = new Point(X_POSITION, Y_POSITION);
                
                runall_plot = GetPlot(xs, ys, columns, "All");
                runall_plot.Width = (pnlPlot.Width / 2) - X_MARGIN - X_POSITION;
                runall_plot.Height = pnlPlot.Height - Y_MARGIN - Y_POSITION;
                runall_plot.Location = new Point(X_POSITION + run_plot.Width, Y_POSITION);

                pnlPlot.Controls.Add(run_plot);
                pnlPlot.Controls.Add(runall_plot);

                run_plot.Render();
                runall_plot.Render();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error initializing plot: {0}", e.Message);
                run_plot = null;
            }
        }

        private static FormsPlot GetPlot(double[] xs, double[] ys, string[] columns, string title)
        {
            FormsPlot plot = new FormsPlot();

            plot.plt.PlotBar(xs, ys);
            plot.plt.Axis(y1: 0);
            plot.plt.Grid(enableVertical: false, lineStyle: LineStyle.Dot);
            plot.plt.XTicks(xs, columns);
            plot.plt.Ticks(fontName: "arial narrow", fontSize: 10, color: Color.DarkBlue);
            plot.plt.XLabel(title);
            plot.plt.AxisAuto(horizontalMargin: .1, verticalMargin: 0);
            plot.AutoSize = false;

            return plot;
        }

        public void ClearPlots()
        {
            if (run_plot == null || runall_plot == null)
                return;
            run_plot.plt.Clear();
            runall_plot.plt.Clear();
            run_plot.Render();
            runall_plot.Render();
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
