﻿using System;
using System.Collections.Generic;

namespace MouseAI.BL
{
    public class MazeStatistic
    {
        public string maze_guid { get; set; }
        public string model_name { get; set; }
        private double predictions;
        private double predict_labels;
        private double predict_error;
        private double neural_moves;
        private double wander_moves;
        private DateTime dtStart { get; set; }
        private DateTime dtEnd { get; set; }
        private MazeStatistics.MOUSE_STATUS mouse_status;
        private bool isRunning;

        public MazeStatistic(string maze_guid, string model_name)
        {
            this.maze_guid = maze_guid;
            this.model_name = model_name;

            dtStart = DateTime.UtcNow;
            isRunning = true;
            mouse_status = MazeStatistics.MOUSE_STATUS.NONE;
        }

        public double[] GetData()
        {
            return new []{ predictions, predict_labels, predict_error, neural_moves, wander_moves };
        }

        public void IncrementNeuralMoves()
        {
            neural_moves++;
        }

        public void IncrementWanderMoves()
        {
            wander_moves++;
        }

        public void SetPredictions(int value)
        {
            predictions = value;
        }

        public void SetPredictedLabels(int value)
        {
            predict_labels = value;
        }

        public void SetPredictedErrors(int value)
        {
            predict_error = value;
        }

        public void End()
        {
            dtEnd = DateTime.UtcNow;
            isRunning = false;
        }

        public string GetMouseStatus()
        {
            switch (mouse_status)
            {
                case MazeStatistics.MOUSE_STATUS.NONE: return "Waiting";
                case MazeStatistics.MOUSE_STATUS.RECALLING: return "Moving Neurally";
                case MazeStatistics.MOUSE_STATUS.WANDERING: return "Wandering";
                case MazeStatistics.MOUSE_STATUS.LOOKING: return "Looking";
                case MazeStatistics.MOUSE_STATUS.FOUND: return "Found the cheese!";
                default: return string.Empty;
            }
        }

        public void SetMouseStatus(MazeStatistics.MOUSE_STATUS ms)
        {
            mouse_status = ms;
        }

        public string GetTime()
        {
            TimeSpan ts = (isRunning) ? DateTime.UtcNow - dtStart : dtEnd - dtStart;
            return string.Format("{0}.{1}.{2}", ts.Minutes, ts.Seconds, ts.Milliseconds);
        }
    }

    public class MazeStatistics : List<MazeStatistic>
    {
        private const int COLUMNS = 5;

        public enum MOUSE_STATUS
        {
            NONE,
            RECALLING,
            WANDERING,
            LOOKING,
            FOUND
        }

        private static readonly string[] Columns =
        {
            "Prediction\nCount", "Predicted\nLabels", "Predicted\nErrors", "Neural\nMoves", "Wander\nMoves"
        };

        public static string[] GetPlotColumns()
        {
            return Columns;
        }

        public double[] GetData()
        {
            double[] values = new double[COLUMNS];
            double[] result;

            foreach (MazeStatistic ms in this)
            {
                result = ms.GetData();
                for (int i = 0; i < COLUMNS; i++)
                {
                    values[i] += result[i];
                }
            }
            double total = values[3] + values[4];
            values[3] = (100 * values[3]) / total;
            values[4] = (100 * values[4]) / total;

            
            total = values[1] + values[2];
            values[0] = (100 * values[0]) / total;
            values[1] = (100 * values[1]) / total;
            values[2] = (100 * values[2]) / total;
            return values;
        }
    }
}
