using System;
using System.IO;
// using System.IO.Compression;
using System.Linq;
using Ionic.Zip;

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
                ExtractArchive(modelPath, tmpExtractedPath);

            Labels = File.ReadAllLines(labelsPath);
        }

        void ExtractArchive(string modelPath, string extractedPath)
        {
            using (ZipFile archive = new ZipFile(modelPath))
            {
                //https://passwordsgenerator.net/sha256-hash-generator/ - SHA256: haotruong95
                archive.Password = "50834B4DD468EB59781986643E1FAF7A053B97D69C8DDBE26ED4DC545872008F" ; 
                archive.Encryption = EncryptionAlgorithm.PkzipWeak ; // the default: you might need to select the proper value here

                archive.ExtractAll(extractedPath, ExtractExistingFileAction.Throw) ;
            }
        }
    }
}
