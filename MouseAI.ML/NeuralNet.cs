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
using Keras.Callbacks;
using Keras.PreProcessing.Image;
using Microsoft.Win32;
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
        private readonly string plot_ext;
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
        private int predicted = 0;
        private int predictions = 0;

        private static readonly string[] PYTHON_PATHS =
            {@"Python38\", @"Python38\Lib\", @"Python38\DLLs\", @"Python38\Lib\site-packages"};
        private const string SOFTWARE = "SOFTWARE";

        // C:\Users\USER\AppData\Local\Programs\Python\Python38\python38.zip;
        // C:\Users\USER\AppData\Local\Programs\Python\Python38\Lib\;
        // C:\Users\USER\AppData\Local\Programs\Python\Python38\DLLs\;
        // C:\Users\USER\AppData\Local\Programs\Python\Python38\Lib\site-packages"

        #endregion

        #region Initialization

        public NeuralNet(int width, int height, string log_dir, string log_ext, string model_dir, string model_ext, string config_ext, string plot_ext)
        {
            this.width = width;
            this.height = height;
            this.log_dir = log_dir;
            this.log_ext = log_ext;
            this.model_dir = model_dir;
            this.model_ext = model_ext;
            this.config_ext = config_ext;
            this.plot_ext = plot_ext;

            //string paths = ConfigurationManager.AppSettings.Get("PythonPaths");
            //if (!string.IsNullOrEmpty(paths))
            //{
            //    string AppDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //    PythonEngine.PythonPath = paths += AppDir + ";";
            //}

            K.DisableEager();
            K.ClearSession();
            K.ResetUids();
        }

        public static string CheckPythonPath()
        {
            string result = PythonEngine.PythonPath;

            return string.Empty;
        }

        private static string GetPythonPath()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(SOFTWARE) ?? Registry.LocalMachine.OpenSubKey(SOFTWARE);
            if (key == null)
                return null;

            RegistryKey pythonkey = key.OpenSubKey(@"Python\PythonCore") ?? key.OpenSubKey(@"Wow6432Node\Python\PythonCore");
            if (pythonkey == null)
                return null;

            RegistryKey installPathKey = pythonkey.OpenSubKey(@"3.8\InstallPath");
            return installPathKey == null ? null : (string)installPathKey.GetValue("ExecutablePath");
        }

        public static string[] GetPythonPaths()
        {
            return PYTHON_PATHS;
        }

        public void InitDataSets(ImageDatas imageDatas, double split, int seed)
        {
            dataSets = new DataSets(width, height, imageDatas, split, seed);
            BuildDataSets();
        }

        public void InitDataSets(ImageDatas imageDatas)
        {
            dataSets = new DataSets(width, height, imageDatas, 0);
            ResetPredictions();
        }

        public void BuildDataSets()
        {
            ((x_train, y_train), (x_test, y_test)) = dataSets.BuildDataSets();
            Console.WriteLine("X_Train:{0} Y_Train:{1} X_Test:{2} Y_Test:{3}", x_train.shape, y_train.shape, x_test.shape, y_test.shape);
        }

        #endregion

        #region Training

        private Shape GetShape()
        {
            Shape shape;

            if (K.ImageDataFormat() == "channels_first")
            {
                x_train = x_train.reshape(x_train.shape[0], 1, height, width);
                x_test = x_test.reshape(x_test.shape[0], 1, height, width);
                shape = (1, height, width);
            }
            else
            {
                x_train = x_train.reshape(x_train.shape[0], height, width, 1);
                x_test = x_test.reshape(x_test.shape[0], height, width, 1);
                shape = (height, width, 1);
            }

            return shape;
        }

        public void Process(Config _config, int num_classes)
        {
            if (x_train == null || y_train == null || x_test == null || y_test == null)
                throw new Exception("Dataset was null!");

            dtStart = DateTime.Now;
            config = _config;

            Shape input_shape = null;

            if (config.isCNN)
            {
                input_shape = GetShape();
            }

            if (config.isNormalize)
            {
                x_train = x_train.astype(np.float32);
                x_test = x_test.astype(np.float32);
                x_train /= 255;
                x_test /= 255;
            }

            Console.WriteLine("Test Started {0}", dtStart);
            Console.WriteLine("Network Type: {0} Neural Network", (!config.isCNN) ? "Simple" : "Convolution");
            Console.WriteLine("Width: {0} Height: {1} Size:{2}", width, height, width * height);
            Console.WriteLine("x_train shape: {0} x_train samples: {1} x_test shape: {2} x_test samples: {3}", 
                                x_train.shape, x_train.shape[0], x_test.shape, x_test.shape[0]);

            y_train = Util.ToCategorical(y_train, num_classes);
            y_test = Util.ToCategorical(y_test, num_classes);

            starttime = Utils.GetDateTime_Formatted();
            //log_file = log_dir + @"\" + starttime + "." + log_ext;
            log_file = Utils.GetFileWithExtension(log_dir, starttime, log_ext);

            if (!config.isCNN)
                model = ProcessSnnModel(x_train, y_train, x_test, y_test, num_classes, log_file, config);
            else
                model = ProcessCnnModel(input_shape, x_train, y_train, x_test, y_test, num_classes, log_file, config);

            dtEnd = DateTime.Now;

            // Score the model for performance
            score = model.Evaluate(x_test, y_test, verbose: 0);
            TimeSpan ts = dtEnd - dtStart;
            model.Summary();
            Console.WriteLine("Test End: {0}  Duration: {1}:{2}.{3}", dtEnd, ts.Hours,ts.Minutes, ts.Seconds);
            Console.WriteLine("Loss: {0} Accuracy: {1}", score[0], score[1]);
        }

        #endregion

        #region Prediction

        public ImageDatas Predict(bool isCNN)
        {
            if (model_loaded == null)
                throw new Exception("Invalid Model!");
            if (dataSets == null || !dataSets.isImageDatas())
                throw new Exception("Invalid Dataset!");

            ImageDatas ids = dataSets.GetImageDatas();
            ImageDatas idf = new ImageDatas();
            NDarray x_data = dataSets.BuildDataSet();
            List<string> labels = ids.GetLabels();

            if (isCNN)
            {
                x_data = (K.ImageDataFormat() == "channels_first")
                    ? x_data.reshape(x_data.shape[0], 1, height, width)
                    : x_data.reshape(x_data.shape[0], height, width, 1);
            }

            Console.WriteLine("Predicting {0} Images", ids.Count);
            NDarray y = model_loaded.Predict(x_data, verbose: 2);

            int index;
            NDarray result;

            for (int i = 0; i < y.len; i++)
            {
                result = y[i];
                result = result.argmax();
                index = result.asscalar<int>();

                if (ids[i].Label != labels[index])
                {
                    ids[i].Index = labels.IndexOf(ids[i].Label) + 1;
                    idf.Add(ids[i]);
                }
            }
            double accuracy = Math.Round(((y.len - idf.Count) * 100) / (double)y.len, 2);

            idf.SetResults(string.Format("Predicted:{0} Correct: {1} Incorrect:{2} Accuracy:{3}", y.len, y.len - idf.Count, idf.Count, accuracy));
            return idf;
        }

        public int Predict(List<byte[]> image, string guid, bool isDebug)
        {
            if (model_loaded == null || dataSets == null)
                throw new Exception("Neural Network not initialized!");

            NDarray x_data = dataSets.GetDataSet(image);

            if (config.isCNN)
            {
                x_data = (K.ImageDataFormat() == "channels_first")
                    ? x_data.reshape(x_data.shape[0], 1, height, width)
                    : x_data.reshape(x_data.shape[0], height, width, 1);
            }

            NDarray y = model_loaded.Predict(x_data, verbose: 1);

            y = y.argmax();
            int index = y.asscalar<int>();
            bool result = dataSets.GetImageDatas().isLabelValid(index, guid, isDebug);
            predicted += (Convert.ToInt32(result));
            predictions++;

            return index;
        }

        public int GetPredicted()
        {
            return predicted;
        }

        public int GetPredictedErrors()
        {
            return predictions - predicted;
        }

        private void ResetPredictions()
        {
            predicted = 0;
            predictions = 0;
        }

        #endregion

        #region Models

        private static Callback[] GetCallbacks(bool isEarlyStop, string logname)
        {
            CSVLogger csv_logger = new CSVLogger(logname);

            return isEarlyStop 
                ? new Callback[] { csv_logger, new EarlyStopping(monitor: "val_accuracy", 0, 50, 1, mode: "max", 1)} 
                : new Callback[] { csv_logger };
        }

        private static Sequential ProcessSnnModel(NDarray x_train, NDarray y_train, NDarray x_test, NDarray y_test,
            int num_classes, string logname, Config config)
        {
            // Build model
            Sequential model = new Sequential();
            model.Add(new Flatten());

            AddNodes(model, config);
            
            model.Add(new Dense(num_classes, activation: "softmax"));

            // Compile with loss, metrics and optimizer
            model.Compile(loss: "categorical_crossentropy", 
                optimizer: new Adam(lr: (float)config.LearnRate, decay: (float)config.LearnDecay), metrics: new[] { "accuracy" });

            Callback[] callbacks = GetCallbacks(config.isEarlyStop, logname);

            // Train the model
            model.Fit(x_train, y_train, batch_size: config.Batch, epochs: config.Epochs, verbose: 1,
                    validation_data: new[] {x_test, y_test}, callbacks: callbacks);
            
            return model;
        }

        private static Sequential ProcessCnnModel(Shape input_shape, NDarray x_train, NDarray y_train, NDarray x_test, NDarray y_test,
            int num_classes, string logname, Config config)
        {
            // Build CNN model
            Sequential model = new Sequential();
            //model.Add(new Conv2D(32, kernel_size: (3, 3).ToTuple(), activation: "relu", input_shape: input_shape));
            //model.Add(new Conv2D(64, (3, 3).ToTuple(), activation: "relu"));
            model.Add(new Conv2D(16, kernel_size: (3, 3).ToTuple(), activation: "relu", input_shape: input_shape));
            model.Add(new Conv2D(32, (3, 3).ToTuple(), activation: "relu"));
            model.Add(new MaxPooling2D(pool_size: (2, 2).ToTuple()));
            model.Add(new Flatten());

            Callback[] callbacks = GetCallbacks(config.isEarlyStop, logname);

            AddNodes(model, config);

            model.Add(new Dense(num_classes, activation: "softmax"));

            // Compile with loss, metrics and optimizer
            model.Compile(loss: "categorical_crossentropy", 
                optimizer: new Adam(lr:(float)config.LearnRate, decay:(float)config.LearnDecay), metrics: new[] { "accuracy" });

            // Train the model
            model.Fit(x_train, y_train, batch_size: config.Batch, epochs: config.Epochs, verbose: 1,
                validation_data: new[] { x_test, y_test }, callbacks: callbacks);
  
            return model;
        }

        private static void AddNodes(Sequential model, Config config)
        {
            for (int i = config.Layers - 1; i > -1; i--)
            {
                model.Add(new Dense(config.Nodes[i], activation: "relu"));
                Console.WriteLine("Added nodes {0}", config.Nodes[i]);
                if (config.DropOut[i] != 0.00)
                {
                    model.Add(new Dropout(config.DropOut[i]));
                    Console.WriteLine("Added dropout {0}", config.DropOut[i]);
                }
            }
        }

        #endregion

        #region File Saving and Loading

        public void LoadModel(string stime)
        {
            K.DisableEager();
            K.ClearSession();
            K.ResetUids();

            //string filename = model_dir + @"\" + stime + ".";
            string filename = Utils.GetFileWithoutExtension(model_dir, stime);
            config = (Config)FileIO.DeSerializeXml(typeof(Config), filename + config_ext);

            if (config == null || string.IsNullOrEmpty(config.Model))
                throw new Exception("Invalid config file");

            model_loaded = BaseModel.ModelFromJson(config.Model);
            model_loaded.LoadWeight(filename + model_ext);
            model_loaded.Summary();
            starttime = config.StartTime;
        }

        public void SaveFiles()
        {
            if (model == null || string.IsNullOrEmpty(starttime) || string.IsNullOrEmpty(config.Guid))
                throw new Exception("Model Save Error");

            config.StartTime = starttime;
            config.Model = model.ToJson();

            //string filename = model_dir + @"\" + starttime + ".";
            string filename = Utils.GetFileWithoutExtension(model_dir, starttime);
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
