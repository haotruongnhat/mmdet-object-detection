using System;
using Microsoft.ML.Data;

namespace DLib
{
    internal class CustomVisionPrediction : IObjectPrediction
    {
        [ColumnName("dets")]
        public float[] PredictedDets { get; set; }

        [ColumnName("labels")]
        public long[] PredictedLabels { get; set; }
    }
}
