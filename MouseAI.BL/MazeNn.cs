#region Using statements

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;

#endregion

namespace MouseAI.BL
{
    public class MazeNn
    {
        #region Declarations

        private readonly MLContext mlContext;
        private ITransformer model;

        static readonly string _assetsPath = Path.Combine(Environment.CurrentDirectory, "assets");
        static readonly string _imagesFolder = Path.Combine(_assetsPath, "images");
        static readonly string _trainTagsTsv = Path.Combine(_imagesFolder, "tags.tsv");
        static readonly string _testTagsTsv = Path.Combine(_imagesFolder, "test-tags.tsv");
        static readonly string _predictSingleImage = Path.Combine(_imagesFolder, "toaster3.jpg");
        // static readonly string _inceptionTensorFlowModel = Path.Combine(_assetsPath, "inception", "tensorflow_inception_graph.pb");
        
        private NeuralSettings neuralSettings;
        public const float DEFAULT_MEAN = 117;
        public const float DEFAULT_SCALE = 1;
        public const bool DEFAULT_CHANNELS = true;

        public class NeuralSettings
        {
            public int maze_width { get; set; }
            public int maze_height { get; set; }
            public float Mean { get; set; }
            public float Scale { get; set; }
            public bool ChannelsLast { get; set; }
        }

        //public class ImageData
        //{
        //    [LoadColumn(0)]
        //    public string ImagePath;

        //    [LoadColumn(1)]
        //    public string Label;
        //}

        public class ImagePrediction : ImageClassificationData
        {
            public float[] Score;
            public string PredictedLabelValue;
        }

        public class ImageClassificationData
        {
            [LoadColumn(0)]
            public Bitmap bitmap;

            [LoadColumn(1)]
            public string Label;
        }

        public class ImageDataCollection : IEnumerable<ImageClassificationData>
        {
            private IEnumerable<string> files { get; set; }
            public Func<Bitmap, Bitmap> Handler { get; set; }

            public ImageDataCollection(IEnumerable<string> files)
            {
                this.files = files;
            }

            public IEnumerator<ImageClassificationData> GetEnumerator()
            {
                IEnumerator<string> iterator = files.GetEnumerator();
                while (iterator.MoveNext())
                {
                    string data = iterator.Current;
                    Bitmap image = new Bitmap(data);
                    if (Handler != null)
                    {
                        image = Handler(image);
                    }
                    string[] c = data.Split(new char[] { '\\' });
                    yield return new ImageClassificationData { Label = c[c.Length - 1], bitmap = image };
                    image.Dispose();
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        #endregion

        #region Initialization

        public MazeNn(int width, int height)
        {
            neuralSettings = new NeuralSettings()
            {
                maze_width = width,
                maze_height = height,
                Mean = DEFAULT_MEAN,
                Scale = DEFAULT_SCALE,
                ChannelsLast = DEFAULT_CHANNELS
            };
            mlContext = new MLContext();
        }

        public MazeNn(NeuralSettings neuralSettings)
        {
            this.neuralSettings = neuralSettings;
            mlContext = new MLContext();
        }

        public static NeuralSettings GetNeuralSettingsDefault(int width, int height)
        {
            return new NeuralSettings()
            {
                maze_width = width,
                maze_height = height,
                Mean = DEFAULT_MEAN,
                Scale = DEFAULT_SCALE,
                ChannelsLast = DEFAULT_CHANNELS
            };
        }

        public void SetNeuralSettings(NeuralSettings _neuralSettings)
        {
            neuralSettings = _neuralSettings;
        }

        public NeuralSettings GetNeuralSettings()
        {
            return neuralSettings;
        }

        #endregion

        #region Processing

        public bool ProcessNet()
        {
            try
            {
                List<string> files = new List<string>();
                ImageDataCollection idc = new ImageDataCollection(files);
                model = GenerateModel(mlContext, idc);
                ClassifySingleImage(mlContext, model);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        // Build and train model
        public ITransformer GenerateModel(MLContext mlcontext, ImageDataCollection idc)
        {
            Console.WriteLine("Training classification model");
            IEstimator<ITransformer> pipeline = GetPipeline(mlcontext, neuralSettings, idc);

            // Load Training Label "tags.tsv"
            //IDataView trainingData = mlcontext.Data.LoadFromTextFile<ImageData>(path: _trainTagsTsv, hasHeader: false);
            IDataView trainingData = mlcontext.Data.LoadFromTextFile<ImageClassificationData>(path: _trainTagsTsv, hasHeader: false);

            // Create and train the model
            ITransformer model = pipeline.Fit(trainingData);

            // Generate predictions from the test data, to be evaluated
            IDataView testData = mlcontext.Data.LoadFromTextFile<ImageClassificationData>(path: _testTagsTsv, hasHeader: false);
            IDataView predictions = model.Transform(testData);

            // Create an IEnumerable for the predictions for displaying results
            IEnumerable<ImagePrediction> imagePredictionData = mlcontext.Data.CreateEnumerable<ImagePrediction>(predictions, true);
            DisplayResults(imagePredictionData);

            // Get performance metrics on the model using training data
            Console.WriteLine("=============== Classification metrics ===============");

            MulticlassClassificationMetrics metrics = 
                mlcontext.MulticlassClassification.Evaluate(
                    predictions, labelColumnName: "LabelKey",
                    predictedLabelColumnName: "PredictedLabel");

            Console.WriteLine($"LogLoss is: {metrics.LogLoss}");
            Console.WriteLine($"PerClassLogLoss is: {string.Join(" , ", metrics.PerClassLogLoss.Select(c => c.ToString()))}");

            return model;
        }

        public static void ClassifySingleImage(MLContext mlContext, ITransformer model)
        {
            // load the fully qualified image file name into ImageData
            var imageData = new ImageClassificationData()
            {
                //ImagePath = _predictSingleImage
            };

            // Create prediction function (input = ImageData, output = ImagePrediction)
            PredictionEngine<ImageClassificationData, ImagePrediction> predictor = mlContext.Model.CreatePredictionEngine<ImageClassificationData, ImagePrediction>(model);
            ImagePrediction prediction = predictor.Predict(imageData);

            Console.WriteLine("=============== Making single image classification ===============");

            Console.WriteLine($"Image: {Path.GetFileName(imageData.bitmap.ToString())} predicted as: {prediction.PredictedLabelValue} with score: {prediction.Score.Max()} ");
        }

        private static IEstimator<ITransformer> GetPipeline(MLContext mlcontext, NeuralSettings neuralsettings, ImageDataCollection idc)
        {
            // The image transforms transform the images into the model's expected format.
            // The ScoreTensorFlowModel transform scores the TensorFlow model and allows communication

            return mlcontext.Transforms.LoadImages(outputColumnName: "input", imageFolder: idc, inputColumnName: nameof(ImageClassificationData.bitmap))
                .Append(mlcontext.Transforms.ResizeImages(outputColumnName: "input", imageWidth: neuralsettings.maze_width, imageHeight: neuralsettings.maze_height, inputColumnName: "input"))
                .Append(mlcontext.Transforms.ExtractPixels(outputColumnName: "input", interleavePixelColors: neuralsettings.ChannelsLast, offsetImage: neuralsettings.Mean))
                .Append(mlcontext.Model.LoadTensorFlowModel(Resources.tensorflow_inception_graph.ToString()).
                    ScoreTensorFlowModel(outputColumnNames: new[] { "softmax2_pre_activation" }, inputColumnNames: new[] { "input" }, addBatchDimensionInput: true))
                .Append(mlcontext.Transforms.Conversion.MapValueToKey(outputColumnName: "LabelKey", inputColumnName: "Label"))
                .Append(mlcontext.MulticlassClassification.Trainers.LbfgsMaximumEntropy(labelColumnName: "LabelKey", featureColumnName: "softmax2_pre_activation"))
                .Append(mlcontext.Transforms.Conversion.MapKeyToValue("PredictedLabelValue", "PredictedLabel"))
                .AppendCacheCheckpoint(mlcontext);
        }

        #endregion

        #region Misc

        private static void DisplayResults(IEnumerable<ImagePrediction> imagePredictionData)
        {
            foreach (ImagePrediction prediction in imagePredictionData)
            {
                Console.WriteLine($"Image: {Path.GetFileName(prediction.bitmap.ToString())} predicted as: {prediction.PredictedLabelValue} with score: {prediction.Score.Max()} ");
            }
        }

        //private static IEnumerable<ImageClassificationData> ReadFromTsv(string file, string folder)
        //{
        //    // Need to parse through the tags.tsv file to combine the file path to the
        //    // image name for the ImagePath property so that the image file can be found.
        //    return File.ReadAllLines(file).Select(line => line.Split('\t')).Select(line => new ImageData()
        //     {
        //         ImagePath = Path.Combine(folder, line[0])
        //     });
        //}
        #endregion
    }
}
