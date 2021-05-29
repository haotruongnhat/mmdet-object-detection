using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Microsoft.ML;
using DLib;

namespace ObjectDetection
{
    class Program
    {
        public static void Main()
        {
            // If there is one, use it.
            var modelPath = "/home/mmdet-object-detection/model/det.zip";
            Bitmap bitmap = new Bitmap("/home/vstech-data/COCO/lab/images/45.jpg");

            // Load model
            var model = new DLModule(modelPath);

            var boxes = model.Inspect(bitmap);
            Bitmap overlay = model.DrawBoundingBox(bitmap, boxes, Color.Red);
            overlay.Save("/home/mmdet-object-detection/output.jpeg");
        }
    }
}



