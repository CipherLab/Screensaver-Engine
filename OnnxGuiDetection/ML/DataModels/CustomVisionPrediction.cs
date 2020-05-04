using Microsoft.ML.Data;

namespace OnnxGuiDetection.ML.DataModels
{
    public class CustomVisionPrediction : IOnnxObjectPrediction
    {
        [ColumnName("model_outputs0")]
        public float[] PredictedLabels { get; set; }
    }
}
