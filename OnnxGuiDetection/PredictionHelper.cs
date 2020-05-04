using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML;
using OnnxGuiDetection.ML;
using OnnxGuiDetection.ML.DataModels;
using SharedKernel;

namespace OnnxGuiDetection
{
    public class PredictionHelper
    {
        private static readonly string modelsDirectory = Path.Combine(Environment.CurrentDirectory, @"ML\OnnxModels");
        private OnnxOutputParser outputParser;
        private PredictionEngine<ImageInputData, CustomVisionPrediction> customVisionPredictionEngine;
        public PredictionHelper()
        {
            // Check for an Onnx model exported from Custom Vision
            var customVisionExport = Directory.GetFiles(modelsDirectory, "*.zip").FirstOrDefault();

            // If there is one, use it.
            if (customVisionExport != null)
            {
                var customVisionModel = new CustomVisionModel(customVisionExport);
                var modelConfigurator = new OnnxModelConfigurator(customVisionModel);

                outputParser = new OnnxOutputParser(customVisionModel);
                customVisionPredictionEngine = modelConfigurator.GetMlNetPredictionEngine<CustomVisionPrediction>();
            }
        }


        public List<BoundingBox> GetBoundingBoxes(System.Drawing.Bitmap bitmap)
        {
            var frame = new ImageInputData { Image = bitmap };
            return (DetectObjectsUsingModel(frame));

        }

     

        private List<BoundingBox> DetectObjectsUsingModel(ImageInputData imageInputData)
        {
            var labels = customVisionPredictionEngine?.Predict(imageInputData).PredictedLabels;
            var boundingBoxes = outputParser.ParseOutputs(labels);
            var filteredBoxes = outputParser.FilterBoundingBoxes(boundingBoxes, 5, 0.5f);
            return filteredBoxes;
        }

    }
}
