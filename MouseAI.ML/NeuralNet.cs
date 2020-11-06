#region Using Statements

using System;
using System.IO;
using Keras;
using Keras.Layers;
using Keras.Models;
using Keras.Optimizers;
using Keras.Utils;
using K = Keras.Backend;
using Numpy;
using Python.Runtime;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using Keras.Callbacks;
using MouseAI.SH;
using MouseAI.PL;

#endregion

namespace MouseAI.ML
{
    public class NeuralNet
    {
        #region Declarations

        private readonly int width; 
        private readonly int height;
        private readonly string model_dir;
        private readonly string model_ext;
        private readonly string log_dir;
        private readonly string log_ext;
        private readonly string config_ext;
        private string starttime;
        private string log_file;
        private Config config;

        private NDarray x_train;    // Training Data
        private NDarray y_train;    // Training Labels
        private NDarray x_test;     // Testing Data
        private NDarray y_test;     // Testing Labels

        private DataSets dataSets;
        private Sequential model;
        private DateTime dtStart;
        private DateTime dtEnd;
        private double[] score;

        #endregion

        #region Initialization

        public NeuralNet(int width, int height, string log_dir, string log_ext, string model_dir, string model_ext, string config_ext)
        {
            this.width = width;
            this.height = height;
            this.log_dir = log_dir;
            this.log_ext = log_ext;
            this.model_dir = model_dir;
            this.model_ext = model_ext;
            this.config_ext = config_ext;

            string paths = ConfigurationManager.AppSettings.Get("PythonPaths");
            if (!string.IsNullOrEmpty(paths))
            {
                string AppDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                PythonEngine.PythonPath = paths += AppDir + ";";
            }
        }

        public void InitDataSets(ImageDatas imageDatas, double split, Random r)
        {
            dataSets = new DataSets(width, height, imageDatas, split, r);
            BuildDataSets();
        }

        public void BuildDataSets()
        {
            ((x_train, y_train), (x_test, y_test)) = dataSets.BuildDataSets();
            Console.WriteLine("X_Train:{0} Y_Train:{1} X_Test:{2} Y_Test:{3}", x_train.shape, y_train.shape, x_test.shape, y_test.shape);
        }

        #endregion

        #region Processing

        public void Process(Config _config, int num_classes)
        {
            if (x_train == null || y_train == null || x_test == null || y_test == null)
                throw new Exception("Dataset was null!");

            dtStart = DateTime.UtcNow;
            config = _config;

            if (K.ImageDataFormat() == "channels_first")
            {
                x_train = x_train.reshape(x_train.shape[0], 1, height, width);
                x_test = x_test.reshape(x_test.shape[0], 1, height, width);
            }
            else
            {
                x_train = x_train.reshape(x_train.shape[0], height, width, 1);
                x_test = x_test.reshape(x_test.shape[0], height, width, 1);
            }

            if (config.isNormalize)
            {
                x_train = x_train.astype(np.float32);
                x_test = x_test.astype(np.float32);
                x_train /= 255;
                x_test /= 255;
            }

            Console.WriteLine("Test Started {0}", dtStart);
            Console.WriteLine("Width: {0} Height: {1} Size:{2}", width, height, width * height);
            Console.WriteLine("x_train shape: {0} x_train samples: {1} x_test shape: {2} x_test samples: {3}", 
                                x_train.shape, x_train.shape[0], x_test.shape, x_test.shape[0]);

            y_train = Util.ToCategorical(y_train, num_classes);
            y_test = Util.ToCategorical(y_test, num_classes);

            starttime = DateTime.UtcNow.ToString("dd_MM_yyyy_hh_mm_ss");
            log_file = log_dir + @"\" + starttime + "." + log_ext;

            model = ProcessModel(x_train, y_train, x_test, y_test, num_classes, log_file, config);
            dtEnd = DateTime.UtcNow;

            // Score the model for performance
            score = model.Evaluate(x_test, y_test, verbose: 0);
            TimeSpan ts = dtEnd - dtStart;
            model.Summary();
            Console.WriteLine("Test End: {0}  Duration: {1}:{2}.{3}", dtEnd, ts.Hours,ts.Minutes, ts.Seconds);
            Console.WriteLine("Loss: {0} Accuracy: {1}", score[0], score[1]);
        }

        #endregion

        #region Models

        //private static Sequential ProcessModel(Shape input_shape, NDarray x_train, NDarray y_train, NDarray x_test, NDarray y_test,
        //    int epochs, int num_classes, int batch_size, bool isEarlyStop, string logname)
        private static Sequential ProcessModel(NDarray x_train, NDarray y_train, NDarray x_test, NDarray y_test,
            int num_classes, string logname, Config config)
        {
            // Build model
            Sequential model = new Sequential();
            model.Add(new Dropout(0.25));
            model.Add(new Flatten());

            for (int i = 0; i < config.Layers;i++)
            {
                model.Add(new Dense(config.Nodes, activation: "relu"));
            }

            if (config.isDropOut)
                model.Add(new Dropout(0.5));

            model.Add(new Dense(num_classes, activation: "softmax"));

            // Compile with loss, metrics and optimizer
            model.Compile(loss: "categorical_crossentropy", optimizer: new Adam(), metrics: new[] { "accuracy" });

            CSVLogger csv_logger = new CSVLogger(logname);

            if (config.isEarlyStop)
            {
                Callback[] callbacks = { csv_logger, new EarlyStopping(monitor: "val_loss", 0, 0, 1, mode: "min", 1) };
                
                // Train the model
                model.Fit(x_train, y_train, batch_size: config.Batch, epochs: config.Epochs, verbose: 1,
                    validation_data: new[] {x_test, y_test}, callbacks: callbacks);
            }
            else
            {
                Callback[] callbacks = { csv_logger };
                // Train the model
                model.Fit(x_train, y_train, batch_size: config.Batch, epochs: config.Epochs, verbose: 1,
                    validation_data: new[] { x_test, y_test }, callbacks: callbacks);
            }

            return model;
        }
        
        private static Sequential ProcessCnnModel(Shape input_shape, NDarray x_train, NDarray y_train, NDarray x_test, NDarray y_test, 
                                        int epochs, int num_classes, int batch_size)
        {
            // Build CNN model
            Sequential model = new Sequential();
            model.Add(new Conv2D(32, kernel_size: (3, 3).ToTuple(),
                activation: "relu",
                input_shape: input_shape));

            model.Add(new Conv2D(64, (3, 3).ToTuple(), activation: "relu"));
            model.Add(new MaxPooling2D(pool_size: (2, 2).ToTuple()));
            model.Add(new Dropout(0.25));
            model.Add(new Flatten());
            model.Add(new Dense(128, activation: "relu"));
            model.Add(new Dropout(0.5));
            model.Add(new Dense(num_classes, activation: "softmax"));

            // Compile with loss, metrics and optimizer
            model.Compile(loss: "categorical_crossentropy", optimizer: new Adadelta(), metrics: new[] { "accuracy" });

            // Train the model
            model.Fit(x_train, y_train, batch_size: batch_size, epochs: epochs, verbose: 1,
                validation_data: new[] { x_test, y_test });

            return model;
        }

        #endregion

        #region File Saving and Loading

        public void SaveFiles()
        {
            if (model == null || string.IsNullOrEmpty(starttime) || string.IsNullOrEmpty(config.Guid))
                throw new Exception("Model Save Error");

            config.StartTime = starttime;
            config.Model = model.ToJson();
            string filename = model_dir + @"\" + starttime + ".";
            FileIO.SerializeXml(config, filename + config_ext);
            model.Save(filename + model_ext);
        }

        public string GetLogName()
        {
            return !string.IsNullOrEmpty(starttime) ? starttime : string.Empty;
        }

        public double[] GetScore()
        {
            return score;
        }

        public string GetStartTime()
        {
            return dtStart.ToString();
        }

        public string GetEndTime()
        {
            return dtEnd.ToString();
        }

        public int GetEpochs()
        {
            if (score == null || score.Length != 2)
                return -1;
            return (int) score[0];
        }

        public double GetAccuracy()
        {
            if (score == null || score.Length != 2)
                return -1;

            return score[1];
        }

        #endregion
    }
}
