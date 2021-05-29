using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace DLib
{
    internal class CustomVisionModel : IModel
    {
        const string modelName = "model.onnx", labelsName = "labels.txt";

        private readonly string labelsPath;

        public string ModelPath { get; private set; }

        public string ModelInput { get; } = "input";
        public string[] ModelOutput { get; } = {"dets", "labels"};

        public string[] Labels { get; private set; }

        public CustomVisionModel(string modelPath, string labelPath) {
            ModelPath = modelPath;
            labelsPath = labelPath;
            
            Labels = File.ReadAllLines(labelsPath);
        }

        public CustomVisionModel(string modelPath)
        {
            string tmpPath = Path.GetTempPath();  
            var tmpExtractedPath = Path.Combine(tmpPath, Path.GetFileNameWithoutExtension(modelPath));

            if (!Directory.Exists(tmpExtractedPath))
                Directory.CreateDirectory(tmpExtractedPath);

            ModelPath = Path.GetFullPath(Path.Combine(tmpExtractedPath, modelName));
            labelsPath = Path.GetFullPath(Path.Combine(tmpExtractedPath, labelsName));

            if (!File.Exists(ModelPath) || !File.Exists(labelsPath))
                ExtractArchive(modelPath);

            Labels = File.ReadAllLines(labelsPath);
        }

        void ExtractArchive(string modelPath)
        {
            using (ZipArchive archive = ZipFile.OpenRead(modelPath))
            {
                var modelEntry = archive.Entries.FirstOrDefault(e => e.Name.Equals(modelName, StringComparison.OrdinalIgnoreCase))
                    ?? throw new FormatException("The exported .zip archive is missing the model.onnx file");

                modelEntry.ExtractToFile(ModelPath);

                var labelsEntry = archive.Entries.FirstOrDefault(e => e.Name.Equals(labelsName, StringComparison.OrdinalIgnoreCase))
                    ?? throw new FormatException("The exported .zip archive is missing the labels.txt file");

                labelsEntry.ExtractToFile(labelsPath);
            }
        }
    }
}
