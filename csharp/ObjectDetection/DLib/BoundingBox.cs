using System.Drawing;
using System.Collections.Generic;

namespace DLib
{
    /// <summary>
    /// Bounding Box's Dimensions
    /// </summary>
    public class BoundingBoxDimensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public float X { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public float Y { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public float Height { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public float Width { get; set; }
    }

    /// <summary>
    /// Bounding Box
    /// </summary>
    public class BoundingBox
    {
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public BoundingBoxDimensions Dimensions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public string Label { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public float Confidence { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public RectangleF Rect
        {
            get { return new RectangleF(Dimensions.X, Dimensions.Y, Dimensions.Width, Dimensions.Height); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public Color BoxColor { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string Description => $"{Label} ({(Confidence * 100).ToString("0")}%)";

        private static readonly Color[] classColors = new Color[]
        {
            Color.Khaki, Color.Fuchsia, Color.Silver, Color.RoyalBlue,
            Color.Green, Color.DarkOrange, Color.Purple, Color.Gold,
            Color.Red, Color.Aquamarine, Color.Lime, Color.AliceBlue,
            Color.Sienna, Color.Orchid, Color.Tan, Color.LightPink,
            Color.Yellow, Color.HotPink, Color.OliveDrab, Color.SandyBrown,
            Color.DarkTurquoise
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Color GetColor(int index) => index < classColors.Length ? classColors[index] : classColors[index % classColors.Length];
    
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Bbox - Label: {0} X: {1}, Y: {2}, Width: {3}, Height: {4}, Score: {5}", 
                                Label, Dimensions.X, Dimensions.Y, Dimensions.Width, Dimensions.Height, Confidence);   
        }
        public string GetSimplifiedString()
        {
            return string.Format("label:{0},x:{1},y:{2},width:{3},height:{4},score:{5}", 
                                Label, (int)Dimensions.X, (int)Dimensions.Y, (int)Dimensions.Width, (int)Dimensions.Height, Confidence);   
        }
    }
}