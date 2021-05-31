using System;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using Microsoft.ML;
using Microsoft.ML.Data;


namespace DLib
{
    /// <summary>
    /// DLModule class
    /// </summary>
    public class DLModule
    {   
        private PredictionEngine<ImageInputData, CustomVisionPrediction> predictionEngine;
        private OutputParser outputParser;
        private ModelConfigurator modelConfigurator;

        /// <summary>
        /// DLModule Constructor
        /// </summary>
        /// <param name="modelPath">Path to model file (zip file)</param>
        public DLModule(string modelPath) {
            var customVisionModel = new CustomVisionModel(modelPath);
            modelConfigurator = new ModelConfigurator(customVisionModel);
            
            outputParser = new OutputParser(customVisionModel);
            predictionEngine = modelConfigurator.GetMlNetPredictionEngine<CustomVisionPrediction>();
        }
        
        /// <summary>
        /// Inspect multiple images
        /// </summary>
        /// <param name="images">List of multiple images (Bitmap)</param>
        /// <returns></returns>
        public List<List<BoundingBox>> Inspect(List<Bitmap> images) {
            IEnumerable<ImageInputData> inputs = images.Select(image => new ImageInputData() {Image = image});
            var boxes = Inspect_(inputs);
            return boxes;
        }

        private List<List<BoundingBox>> Inspect_(IEnumerable<ImageInputData> inputs) {
            var outputs = modelConfigurator.BatchInference(inputs);

            // Output post-processing
            IEnumerable<float[]> dets = outputs.GetColumn<float[]>("dets");
            IEnumerable<long[]> labels = outputs.GetColumn<long[]>("labels");

            int batchSize = dets.Count();
            List<List<BoundingBox>> batchBoxes = new List<List<BoundingBox>>();
            
            for(int batchId = 0; batchId < batchSize; batchId++) {
                var boxes = outputParser.ParseOutputs(
                    dets.ElementAt(batchId), 
                    labels.ElementAt(batchId));
                
                batchBoxes.Add(boxes);
            }

            return batchBoxes;
        }

        /// <summary>
        /// Draw boxes on bitmap image
        /// </summary>
        /// <param name="image">Input bitmap image</param>
        /// <param name="boundingBoxes">Bounding boxes result from model</param>
        /// <param name="boxColor">Box color</param>
        /// <param name="boxThickness">Box thickness</param>
        /// <param name="drawText">Draw box description if true</param>
        /// <param name="fontSize">Font size of text description</param>
        /// <returns></returns>
        public Bitmap DrawBoundingBox(Bitmap image, List<BoundingBox> boundingBoxes, 
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