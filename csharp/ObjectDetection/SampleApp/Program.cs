using System;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using DLib;

namespace ObjectDetection
{
    class Program
    {
        public static void Main()
        {
            string rootFolder = "path/to/assets/folder";
            
            //Path to model
            var modelPath = Path.Combine(rootFolder, "model" , "det.zip");

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Console.WriteLine("Start loading model");

            //Initilize model
            var model = new DLModule(modelPath);

            Console.WriteLine("Load model: {0} ms", stopwatch.Elapsed.Milliseconds);

            //Path to image
            Bitmap bitmap1 = new Bitmap(Path.Combine(rootFolder, "images", "46.jpg"));
            Bitmap bitmap2 = new Bitmap(Path.Combine(rootFolder, "images", "1.jpg"));
            var bitmaps = new List<Bitmap>() {bitmap1, bitmap2};

            stopwatch.Restart();

            //Inspect images
            List<List<BoundingBox>> boxes = model.Inspect(bitmaps);

            //Execution time
            Console.WriteLine("Run image: {0} ms", stopwatch.Elapsed.Milliseconds);

            //Access boxes result from first image
            boxes.ElementAt(0).ForEach(box => Console.WriteLine(box));

            //Write bounding boxes on image
            Bitmap overlay1 = model.DrawBoundingBox(bitmap1, boxes.ElementAt(0), Color.Red);
            overlay1.Save(Path.Combine(rootFolder, "output1.jpg"));
            Bitmap overlay2 = model.DrawBoundingBox(bitmap2, boxes.ElementAt(1), Color.Red);
            overlay2.Save(Path.Combine(rootFolder, "output2.jpg"));
        }
    }
}



