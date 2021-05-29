using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DLib
{
    internal class OutputParser
    {
        class BoundingBoxPrediction : BoundingBoxDimensions
        {
            public float Confidence { get; set; }
        }

        const int BBOX_COUNT = 100;
        const int DET_ATTRIBUTE_COUNT = 5;
        const int LABEL_ATTRIBUTE_COUNT = 1;

        // Labels corresponding to the classes the  model can predict. For example, the 
        // Tiny YOLOv2 model included with this sample is trained to predict 20 different classes.
        private readonly string[] classLabels;

        public OutputParser(IModel Model)
        {
            classLabels = Model.Labels;
        }

        // Applies the sigmoid function that outputs a number between 0 and 1.
        private float Sigmoid(float value)
        {
            var k = Math.Exp(value);
            return (float)(k / (1.0f + k));
        }

        // Normalizes an input vector into a probability distribution.
        private float[] Softmax(float[] classProbabilities)
        {
            var max = classProbabilities.Max();
            var exp = classProbabilities.Select(v => (float)Math.Exp(v - max));
            var sum = exp.Sum();
            return exp.Select(v => v / sum).ToArray();
        }

        // Extracts the bounding box features (x, y, height, width, confidence) method from the model
        // output. The confidence value states how sure the model is that it has detected an object. 
        // We use the Sigmoid function to turn it that confidence into a percentage.
        private BoundingBoxPrediction ExtractBoundingBoxPrediction(float[] modelOutput, int elemId)
        {
            return new BoundingBoxPrediction
            {
                X = modelOutput[elemId*DET_ATTRIBUTE_COUNT],
                Y = modelOutput[elemId*DET_ATTRIBUTE_COUNT + 1],
                Width = modelOutput[elemId*DET_ATTRIBUTE_COUNT + 2] - modelOutput[elemId*DET_ATTRIBUTE_COUNT],
                Height = modelOutput[elemId*DET_ATTRIBUTE_COUNT + 3] - modelOutput[elemId*DET_ATTRIBUTE_COUNT + 1],
                Confidence = modelOutput[elemId*DET_ATTRIBUTE_COUNT + 4],
            };
        }

        private Int64 ExtractClass(Int64[] modelOutput, int elemId)
        {
            return modelOutput[elemId*LABEL_ATTRIBUTE_COUNT];
        }

        // IoU (Intersection over union) measures the overlap between 2 boundaries. We use that to
        // measure how much our predicted boundary overlaps with the ground truth (the real object
        // boundary). In some datasets, we predefine an IoU threshold (say 0.5) in classifying
        // whether the prediction is a true positive or a false positive. This method filters
        // overlapping bounding boxes with lower probabilities.
        private float IntersectionOverUnion(RectangleF boundingBoxA, RectangleF boundingBoxB)
        {
            var areaA = boundingBoxA.Width * boundingBoxA.Height;
            var areaB = boundingBoxB.Width * boundingBoxB.Height;

            if (areaA <= 0 || areaB <= 0)
                return 0;

            var minX = Math.Max(boundingBoxA.Left, boundingBoxB.Left);
            var minY = Math.Max(boundingBoxA.Top, boundingBoxB.Top);
            var maxX = Math.Min(boundingBoxA.Right, boundingBoxB.Right);
            var maxY = Math.Min(boundingBoxA.Bottom, boundingBoxB.Bottom);

            var intersectionArea = Math.Max(maxY - minY, 0) * Math.Max(maxX - minX, 0);

            return intersectionArea / (areaA + areaB - intersectionArea);
        }

        public List<BoundingBox> ParseOutputs(float[] dets, Int64[] labels, float probabilityThreshold = .3f)
        {
            // var batchBoxes = new BatchBoundingBoxes();
            // result of one sample: det (100x5)
            // for(int batchId = 0; batchId < dets.Count(); batchId++) {
            //     float[] det = dets.ElementAt(batchId);
            //     Int64[] label = labels.ElementAt(batchId);

            var boxes = new List<BoundingBox>();
            for(int elemId = 0; elemId < BBOX_COUNT; elemId++) {
                
                var boxPrediction = ExtractBoundingBoxPrediction(dets, elemId);
                var classIndex = ExtractClass(labels, elemId);

                if (boxPrediction.Confidence < probabilityThreshold) continue;

                boxes.Add(new BoundingBox
                {
                    Dimensions = new BoundingBoxDimensions
                    {
                        X = boxPrediction.X,
                        Y = boxPrediction.Y,
                        Width = boxPrediction.Width,
                        Height = boxPrediction.Height
                    },
                    Confidence = boxPrediction.Confidence,
                    Label = classLabels[classIndex],
                    BoxColor = BoundingBox.GetColor((int)classIndex)
                });
            }
            return boxes;
        }

        public List<BoundingBox> FilterBoundingBoxes(List<BoundingBox> boxes, int limit, float iouThreshold)
        {
            var results = new List<BoundingBox>();
            var filteredBoxes = new bool[boxes.Count];
            var sortedBoxes = boxes.OrderByDescending(b => b.Confidence).ToArray();

            for (int i = 0; i < boxes.Count; i++)
            {
                if (filteredBoxes[i])
                    continue;

                results.Add(sortedBoxes[i]);

                if (results.Count >= limit)
                    break;

                for (var j = i + 1; j < boxes.Count; j++)
                {
                    if (filteredBoxes[j])
                        continue;

                    if (IntersectionOverUnion(sortedBoxes[i].Rect, sortedBoxes[j].Rect) > iouThreshold)
                        filteredBoxes[j] = true;

                    if (filteredBoxes.Count(b => b) <= 0)
                        break;
                }
            }
            return results;
        }
    }
}