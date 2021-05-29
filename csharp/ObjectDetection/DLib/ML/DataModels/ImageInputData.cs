using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Image;
using System.Drawing;

namespace DLib
{
    internal struct ImageSettings
    {
        public const int imageHeight = 640;
        public const int imageWidth = 800;
    }

    internal class ImageInputData
    {
        [ImageType(ImageSettings.imageHeight, ImageSettings.imageWidth)]
        public Bitmap Image { get; set; }
    }
}
