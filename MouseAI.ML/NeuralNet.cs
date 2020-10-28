using System;
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

namespace MouseAI.ML
{
    public class NeuralNet
    {
        private readonly int width; 
        private readonly int height;

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

        public void TestMnist()
        { 
            var ((x_train, y_train), (x_test, y_test)) = MNIST.LoadData();
            Process(5, 10, 128, true, null, x_train, y_train,x_test, y_test);
        }

        public void Process(int epochs, int num_classes, int batch_size, bool isNormalize, string Guid, 
                            NDarray x_train, NDarray y_train, NDarray x_test, NDarray y_test)
        {
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
            Console.WriteLine("x_train shape: {0} train samples: {1} x_test shape: {2}", 
                                x_train.shape, x_train.shape[0], x_test.shape[0]);

            // Convert class vectors to binary class matrices
            y_train = Util.ToCategorical(y_train, num_classes);
            y_test = Util.ToCategorical(y_test, num_classes);

            Sequential model = ProcessCnnModel(input_shape, x_train, y_train, x_test, y_test, epochs, num_classes, batch_size);

            DateTime dtEnd = DateTime.UtcNow;

            // Score the model for performance
            double[] score = model.Evaluate(x_test, y_test, verbose: 0);
            TimeSpan ts = dtEnd - dtStart;
            Console.WriteLine("Test End: {0}  Duration: {1}:{2}.{3}", dtEnd, ts.Hours,ts.Minutes, ts.Seconds);
            Console.WriteLine("Loss: {0} Accuracy: {1}", score[0], score[1]);

            //string json = model.ToJson();
            //File.WriteAllText("model.json", json);
            //model.Save("model.h5");
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
    }
}
