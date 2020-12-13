// ModelRun Class:
// Form for running a mouse -> cheese session for a selected neural model and maze(s)
// Local methods perform plotting related functions

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ScottPlot;

namespace MouseAI.UI
{
    public partial class ModelRun : Form
    {
        private readonly Maze maze;
        private readonly List<Button> buttons;
        private FormsPlot maze_plot;
        private FormsPlot total_plot;
        private int columns_length;
        private const int X_MARGIN = 10;
        private const int Y_MARGIN = 10;
        private const int X_POSITION = 5;
        private const int Y_POSITION = 5;
        private readonly Color background;
        private readonly Color foreground;
        private readonly string[] columns;

        public ModelRun(Maze maze, Color background, Color foreground)
        {
            InitializeComponent();

            this.maze = maze;
            this.background = background;
            this.foreground = foreground;
            buttons = new List<Button>();

            foreach (Control ctl in Controls)
            {
                if (ctl is Button)
                {
                    buttons.Add((Button)ctl);
                }
            }

            this.columns = maze.GetMazeStatisticPlotColumns();
            InitPlot();
        }

        public void UpdatePlot(double[] data, bool isTotal)
        {
            if (maze_plot == null || total_plot == null || (data != null && data.Length != columns_length))
            {
                Console.WriteLine("Plot Error!");
                return;
            }

            double[] xs = DataGen.Consecutive(columns_length);

            FormsPlot plot = (isTotal) ? total_plot : maze_plot;

            plot.plt.Clear();
            plot.plt.PlotBar(xs, data, fillColor:background);
            plot.Render();
        }

        public void InitPlot()
        {
            columns_length = columns.Length;
            double[] xs = DataGen.Consecutive(columns_length);
            double[] ys = DataGen.Consecutive(columns_length, 0);

            try
            {
                maze_plot = GetPlot(xs, ys, columns, "Maze");
                maze_plot.Width = (pnlPlot.Width / 2)  - X_MARGIN - X_POSITION;
                maze_plot.Height = pnlPlot.Height - Y_MARGIN - Y_POSITION;
                maze_plot.Location = new Point(X_POSITION, Y_POSITION);
                
                
                total_plot = GetPlot(xs, ys, columns, "Total %");
                total_plot.Width = (pnlPlot.Width / 2) - X_MARGIN - X_POSITION;
                total_plot.Height = pnlPlot.Height - Y_MARGIN - Y_POSITION;
                total_plot.Location = new Point(X_POSITION + maze_plot.Width, Y_POSITION);

                pnlPlot.Controls.Add(maze_plot);
                pnlPlot.Controls.Add(total_plot);
      
                maze_plot.Render();
                total_plot.Render();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error initializing plot: {0}", e.Message);
                maze_plot = null;
            }
        }

        private FormsPlot GetPlot(double[] xs, double[] ys, string[] columns, string title)
        {
            FormsPlot plot = new FormsPlot();

            plot.plt.Style(Style.Blue1);
            plot.plt.PlotBar(xs, ys);
            plot.plt.Axis(y1: 0);
            plot.plt.Grid(enableVertical: false, lineStyle: LineStyle.Dot);
            plot.plt.XTicks(xs, columns);
            plot.plt.Ticks(fontName: "arial narrow", fontSize: 10);
            plot.plt.XLabel(title);
            plot.plt.AxisAuto(horizontalMargin: .1, verticalMargin: 0);
            plot.AutoSize = false;
            plot.plt.Style(dataBg: foreground, tick: Color.White);

            return plot;
        }

        public void ClearPlots()
        {
            if (maze_plot == null || total_plot == null)
                return;
            maze_plot.plt.Clear();
            total_plot.plt.Clear();
            maze_plot.Render();
            total_plot.Render();
        }

        public void ClearMazePlot()
        {
            if (maze_plot == null)
                return;

            maze_plot.plt.Clear();
            maze_plot.Render();
        }

        public void UpdateTimer()
        {
            tbxMoves.Text = maze.GetMazeStatisticMoves().ToString();
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
