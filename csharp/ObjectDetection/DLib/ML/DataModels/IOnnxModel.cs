using System;

namespace DLib
{
    internal interface IModel
    {
        string ModelPath { get; }

        // To check Model input and output parameter names, you can
        // use tools like Netron: https://github.com/lutzroeder/netron
        string ModelInput { get; }
        string[] ModelOutput { get; }

        string[] Labels { get; }
    }

    internal interface IObjectPrediction
    {
        float[] PredictedDets { get; set; }

        long[] PredictedLabels { get; set; }
    }
}
