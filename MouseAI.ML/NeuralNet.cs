#region Using Statements

using System;
using System.Collections.Generic;
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
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;
using Keras.Callbacks;
using Keras.PreProcessing.Image;
using MouseAI.SH;
using MouseAI.PL;
using static Keras.Models.Sequential;

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
        private BaseModel model_loaded;

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
            K.DisableEager();
            K.ClearSession();
            K.ResetUids();
        }

        public void InitDataSets(ImageDatas imageDatas, double split, Random r)
        {
            dataSets = new DataSets(width, height, imageDatas, split, r);
            BuildDataSets();
        }

        public void InitDataSets(ImageDatas imageDatas)
        {
            dataSets = new DataSets(width, height, imageDatas);
        }

        public void BuildDataSets()
        {
            ((x_train, y_train), (x_test, y_test)) = dataSets.BuildDataSets();
            Console.WriteLine("X_Train:{0} Y_Train:{1} X_Test:{2} Y_Test:{3}", x_train.shape, y_train.shape, x_test.shape, y_test.shape);
        }

        #endregion

        #region Training

        public void Process(Config _config, int num_classes)
        {
            if (x_train == null || y_train == null || x_test == null || y_test == null)
                throw new Exception("Dataset was null!");

            dtStart = DateTime.Now;
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

            starttime = Utils.GetDateTime_Formatted();
            log_file = log_dir + @"\" + starttime + "." + log_ext;

            model = ProcessModel(x_train, y_train, x_test, y_test, num_classes, log_file, config);
            dtEnd = DateTime.Now;

            // Score the model for performance
            score = model.Evaluate(x_test, y_test, verbose: 0);
            TimeSpan ts = dtEnd - dtStart;
            model.Summary();
            Console.WriteLine("Test End: {0}  Duration: {1}:{2}.{3}", dtEnd, ts.Hours,ts.Minutes, ts.Seconds);
            Console.WriteLine("Loss: {0} Accuracy: {1}", score[0], score[1]);
        }

        #endregion

        #region Predicition

        public void Predict()
        {
            if (model_loaded == null)
                throw new Exception("Invalid Model!");
            if (dataSets == null || !dataSets.isImageDatas())
                throw new Exception("Invalid Dataset!");

            ImageDatas ids = dataSets.GetImageDatas();
            int index;

            NDarray x_data;
            NDarray y_labels;

            (x_data, y_labels) = dataSets.BuildDataSet();

            List<string> labels = ids.GetLabels();
            Console.WriteLine("Predicting {0} Images", ids.Count);
            NDarray y = model_loaded.Predict(x_data);

            int correct = 0;
            int incorrect = 0;

            for(int i=0;i<y.len;i++)
            {
                NDarray result = y[i];
                result = result.argmax();
                index = result.asscalar<int>();

                if (ids[i].Label == labels[index])
                    correct++;
                else
                    incorrect++;
            }
            Console.WriteLine("Correct:{0} Incorrect:{1} Tested:{2}", correct, incorrect, incorrect + correct);
        }

        #endregion

        #region Models

        private static Sequential ProcessModel(NDarray x_train, NDarray y_train, NDarray x_test, NDarray y_test,
            int num_classes, string logname, Config config)
        {
            // Build model
            Sequential model = new Sequential();
             
            double dropout_increment = GetDropOutRate(config);

            model.Add(new Flatten());

            for (int i = config.Layers; i > -1;i--)
            {
                if (config.Amount != 0 && i < config.Amount)
                {
                    model.Add(new Dropout(config.DropOut - (dropout_increment * i)));
                }
                model.Add(new Dense(config.Nodes, activation: "relu"));
            }
            
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

        private static double GetDropOutRate(Config config)
        {
            if (config.Amount <= 0)
            {
                config.Amount = 0;
                return 0;
            }
            if (config.Amount > config.Layers)
                    config.Amount = config.Layers;

            return config.DropOut / config.Amount;
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

        public void LoadModel(string stime)
        {
            K.DisableEager();
            K.ClearSession();
            K.ResetUids();

            string filename = model_dir + @"\" + stime + ".";
            Config cfg = (Config)FileIO.DeSerializeXml(typeof(Config), filename + config_ext);

            if (cfg == null || string.IsNullOrEmpty(cfg.Model))
                throw new Exception("Invalid config file");

            model_loaded = BaseModel.ModelFromJson(cfg.Model);
            model_loaded.LoadWeight(filename + model_ext);
            model_loaded.Summary();
        }

        public void SaveFiles()
        {
            if (model == null || string.IsNullOrEmpty(starttime) || string.IsNullOrEmpty(config.Guid))
                throw new Exception("Model Save Error");

            config.StartTime = starttime;
            config.Model = model.ToJson();
            string filename = model_dir + @"\" + starttime + ".";
            FileIO.SerializeXml(config, filename + config_ext);
            model.SaveWeight(filename + model_ext);
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
