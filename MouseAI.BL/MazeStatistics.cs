using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MouseAI.BL
{
    public class MazeStatistics
    {
        public string maze_guid { get; set; }
        public string model_name { get; set; }
        public int predictions { get; set; }
        public int predicted { get; set; }
        public int fail { get; set; }
        public int good { get; set; }
        public DateTime dtStart { get; set; }
        public DateTime dtEnd { get; set; }
        private bool isRunning;

        public MazeStatistics(string maze_guid, string model_name)
        {
            this.maze_guid = maze_guid;
            this.model_name = model_name;

            dtStart = DateTime.UtcNow;
            isRunning = true;
        }

        public void End()
        {
            dtEnd = DateTime.UtcNow;
            isRunning = false;
        }

        public string GetStatus()
        {
            TimeSpan ts = (isRunning) ? DateTime.UtcNow - dtStart : dtEnd - dtStart;
            return string.Format("{0}\t{1}\t{2}\t{3}\t{4}.{5}", predictions,predicted, good, fail, ts.Minutes, ts.Seconds);
        }

        public string GetColumns()
        {
            return "Pred\tPred\tVision\tWander\tTime" + Environment.NewLine +
                   "Total\tLabel\tMoves\tMoves\tElapsed";
        }
    }
}
