using Microsoft.ML.Data;

namespace OnnxGuiDetection.ML.DataModels
{
    public class TinyYoloPrediction : IOnnxObjectPrediction
    {
        [ColumnName("grid")]
        public float[] PredictedLabels { get; set; }
    }
}
