using System;
using Microsoft.ML;
using Microsoft.ML.Transforms.Image;
using System.Collections.Generic;
using System.Linq;

namespace DLib
{
    internal class ModelConfigurator
    {
        private readonly MLContext mlContext;
        private readonly ITransformer mlModel;

        public ModelConfigurator(IModel Model)
        {
            mlContext = new MLContext();
            // Model creation and pipeline definition for images needs to run just once,
            // so calling it from the constructor:
            mlModel = SetupMlNetModel(Model);
        }

        public IDataView BatchInference(IEnumerable<ImageInputData> inputs) {
            var dataView = mlContext.Data.LoadFromEnumerable(inputs);
            var outputs = mlModel.Transform(dataView);

            return outputs;    
        }

        private ITransformer SetupMlNetModel(IModel Model)
        {
            var dataView = mlContext.Data.LoadFromEnumerable(new List<ImageInputData>());

            var pipeline = mlContext.Transforms.ResizeImages(
                                            outputColumnName: Model.ModelInput, 
                                            inputColumnName: nameof(ImageInputData.Image),
                                            imageWidth: ImageSettings.imageWidth, 
                                            imageHeight: ImageSettings.imageHeight,
                                            resizing: ImageResizingEstimator.ResizingKind.Fill)
                            .Append(mlContext.Transforms.ExtractPixels(outputColumnName: Model.ModelInput,
                                                                    inputColumnName: Model.ModelInput,
                                                                    offsetImage: 128f,
                                                                    scaleImage: 1f / 255f,
                                                                    outputAsFloatArray: true))
                            .Append(mlContext.Transforms.ApplyOnnxModel(modelFile: Model.ModelPath, 
                                                                        outputColumnNames: Model.ModelOutput, 
                                                                        inputColumnNames: new[] { Model.ModelInput },
                                                                        gpuDeviceId: 0,
                                                                        fallbackToCpu: true));

            var mlNetModel = pipeline.Fit(dataView);

            return mlNetModel;
        }

        public PredictionEngine<ImageInputData, T> GetMlNetPredictionEngine<T>()
            where T : class, IObjectPrediction, new()
        {
            return mlContext.Model.CreatePredictionEngine<ImageInputData, T>(mlModel);
        }

        public void SaveMLNetModel(string mlnetModelFilePath)
        {
            // Save/persist the model to a .ZIP file to be loaded by the PredictionEnginePool
            mlContext.Model.Save(mlModel, null, mlnetModelFilePath);
        }
    }
}
