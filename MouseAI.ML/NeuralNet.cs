#region Using Statements

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Keras;
using Keras.Datasets;
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

#endregion

namespace MouseAI.ML
{
    public class NeuralNet
    {
        #region Declarations

        private int width; 
        private int height;
        private string label;

        private NDarray x_train;    // Training Data
        private NDarray y_train;    // Training Labels
        private NDarray x_test;     // Testing Data
        private NDarray y_test;     // Testing Labels

        private DataSets dataSets;

        #endregion

        #region Initialization

        public NeuralNet(int width, int height)
        {
            this.width = width;
            this.height = height;

            string paths = ConfigurationManager.AppSettings.Get("PythonPaths");
            if (!string.IsNullOrEmpty(paths))
            {
                string AppDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                PythonEngine.PythonPath = paths += AppDir + ";";
            }
        }

        public void InitDataSets(ImageDatas imageDatas, double split, string label, Random r)
        {
            this.label = label;
            dataSets = null;
            dataSets = new DataSets(width, height, imageDatas, split, r);
        }

        public void TestMnist()
        { 
            ((x_train, y_train), (x_test, y_test)) = MNIST.LoadData();
            width = 28;
            height = 28;
            Process(100, 10, 128, true, null, true);
        }

        public void BuildDataSets()
        {
            ((x_train, y_train), (x_test, y_test)) = dataSets.BuildDataSets();
            Console.WriteLine("X_Train:{0} Y_Train:{1} X_Test:{2} Y_Test:{3}", x_train.shape, y_train.shape, x_test.shape, y_test.shape);
        }

        #endregion

        #region Processing

        public void Process(int epochs, int num_classes, int batch_size, bool isNormalize, string Guid, bool isEarlyStop)
        {
            if (x_train == null || y_train == null || x_test == null || y_test == null)
                throw new Exception("Dataset was null!");

            DateTime dtStart = DateTime.UtcNow;
            Shape input_shape;

            if (K.ImageDataFormat() == "channels_first")
            {
                x_train = x_train.reshape(x_train.shape[0], 1, height, width);
                x_test = x_test.reshape(x_test.shape[0], 1, height, width);
                input_shape = (1, height, width);
            }
            else
            {
                x_train = x_train.reshape(x_train.shape[0], height, width, 1);
                x_test = x_test.reshape(x_test.shape[0], height, width, 1);
                input_shape = (height, width, 1);
            }

            if (isNormalize)
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

            Sequential model = ProcessModel(input_shape, x_train, y_train, x_test, y_test, epochs, num_classes, batch_size, isEarlyStop);

            DateTime dtEnd = DateTime.UtcNow;

            // Score the model for performance
            double[] score = model.Evaluate(x_test, y_test, verbose: 0);
            TimeSpan ts = dtEnd - dtStart;
            model.Summary();
            Console.WriteLine("Test End: {0}  Duration: {1}:{2}.{3}", dtEnd, ts.Hours,ts.Minutes, ts.Seconds);
            Console.WriteLine("Loss: {0} Accuracy: {1}", score[0], score[1]);

            //string json = model.ToJson();
            //File.WriteAllText("model.json", json);
            //model.Save("model.h5");
        }

        #endregion

        #region Models

        private static Sequential ProcessModel(Shape input_shape, NDarray x_train, NDarray y_train, NDarray x_test, NDarray y_test,
            int epochs, int num_classes, int batch_size, bool isEarlyStop)
        {
            // Build model
            Sequential model = new Sequential();
            model.Add(new Dropout(0.25));
            model.Add(new Flatten());
            model.Add(new Dense(1024, activation: "relu"));
            model.Add(new Dense(1024, activation: "relu"));
            model.Add(new Dropout(0.5));
            model.Add(new Dense(num_classes, activation: "softmax"));

            // Compile with loss, metrics and optimizer
            model.Compile(loss: "categorical_crossentropy", optimizer: new Adadelta(), metrics: new[] { "accuracy" });

            if (isEarlyStop)
            {
                EarlyStopping es = new EarlyStopping(monitor: "val_loss", 0, 0, 1, mode: "min", 1);

                // Train the model
                model.Fit(x_train, y_train, batch_size: batch_size, epochs: epochs, verbose: 1,
                    validation_data: new[] {x_test, y_test}, callbacks: new Callback[] {es});
            }
            else
            {
                // Train the model
                model.Fit(x_train, y_train, batch_size: batch_size, epochs: epochs, verbose: 1,
                    validation_data: new[] { x_test, y_test });
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
    }
}
