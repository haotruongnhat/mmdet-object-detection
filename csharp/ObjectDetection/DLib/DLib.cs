using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Microsoft.ML;

namespace DLib
{
    public class DLModule
    {   
        private PredictionEngine<ImageInputData, CustomVisionPrediction> predictionEngine;
        private OutputParser outputParser;

        public DLModule(string modelPath) {
            var customVisionModel = new CustomVisionModel(modelPath);
            var modelConfigurator = new ModelConfigurator(customVisionModel);
            
            outputParser = new OutputParser(customVisionModel);
            predictionEngine = modelConfigurator.GetMlNetPredictionEngine<CustomVisionPrediction>();
        }
        
        public List<BoundingBox> Inspect(Bitmap image) {
            var frame = new ImageInputData { Image = image };
            var boxes = Inspect_(frame);
            return boxes;
        }

        private List<BoundingBox> Inspect_(ImageInputData input) {
            var outputs = predictionEngine.Predict(input);
            var labels = outputs.PredictedLabels;
            var dets = outputs.PredictedDets;
            var boxes = outputParser.ParseOutputs(dets, labels);

            return boxes;
        }

        public Bitmap DrawBoundingBox(Bitmap image, IList<BoundingBox> boundingBoxes, 
                                        Color boxColor, int boxThickness = 3, 
                                        bool drawText = false, int fontSize = 3)
        {
            Bitmap editableImg = new Bitmap(image);

            var originalImageHeight = image.Height;
            var originalImageWidth = image.Width;

            foreach (var box in boundingBoxes)
            {
                // Get Bounding Box Dimensions
                var x = (uint)Math.Max(box.Dimensions.X, 0);
                var y = (uint)Math.Max(box.Dimensions.Y, 0);
                var width = (uint)Math.Min(originalImageWidth - x, box.Dimensions.Width);
                var height = (uint)Math.Min(originalImageHeight - y, box.Dimensions.Height);

                // Resize To Image
                x = (uint)originalImageWidth * x / ImageSettings.imageWidth;
                y = (uint)originalImageHeight * y / ImageSettings.imageHeight;
                width = (uint)originalImageWidth * width / ImageSettings.imageWidth;
                height = (uint)originalImageHeight * height / ImageSettings.imageHeight;

                // Bounding Box Text
                string text = box.Description;

                using (Graphics thumbnailGraphic = Graphics.FromImage(editableImg))
                {
                    thumbnailGraphic.CompositingQuality = CompositingQuality.HighQuality;
                    thumbnailGraphic.SmoothingMode = SmoothingMode.HighQuality;
                    thumbnailGraphic.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    // Define BoundingBox options
                    Pen pen = new Pen(boxColor, boxThickness);
                    SolidBrush colorBrush = new SolidBrush(boxColor);
                    thumbnailGraphic.DrawRectangle(pen, x, y, width, height);

                    // Draw text on image 
                    if (drawText) {
                        Font drawFont = new Font("Arial", fontSize, FontStyle.Bold);
                        SizeF size = thumbnailGraphic.MeasureString(text, drawFont);
                        SolidBrush fontBrush = new SolidBrush(Color.Black);
                        Point atPoint = new Point((int)x, (int)y - (int)size.Height - 1);
                        thumbnailGraphic.FillRectangle(colorBrush, (int)x, (int)(y - size.Height - 1), (int)size.Width, (int)size.Height);
                        thumbnailGraphic.DrawString(text, drawFont, fontBrush, atPoint);
                    }
                }
            }
            return editableImg;
        }

    }
}