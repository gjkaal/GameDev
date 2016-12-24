using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Mc.Core;

namespace I2P
{
    /// <summary>
    /// Class ImageConverter.
    /// </summary>
    /// <remarks>no comments</remarks>
    public class ImageConverter
    {
        /// <summary>
        /// The Bitmap source
        /// </summary>
        private System.Drawing.Bitmap _bmp;
        
        public string FileName { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageConverter"/> class.
        /// </summary>
        /// <param name="bmp">The BMP.</param>
        /// <remarks>no comments</remarks>
        public ImageConverter(Bitmap bmp)
        {
            _bmp = bmp;
        }

        /// <summary>
        /// Creates the converter.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>ImageConverter.</returns>
        /// <exception cref="Mc.Core.Exceptions.McException">The file does not exist:" + filePath</exception>
        /// <remarks>no comments</remarks>
        public static ImageConverter CreateConverter(string filePath)
        {
            Mc.Core.Validate.NotNull("filePath", filePath);
            var f = new FileInfo(filePath);
            if (!f.Exists)
            {
                throw new Mc.Core.Exceptions.McException("The file does not exist:" + filePath);
            }
            Bitmap bmp;
            using (var fs = f.OpenRead())
            {
                bmp = new System.Drawing.Bitmap(fs,true);
            }
            return new ImageConverter(bmp)
            {
                FileName = filePath
            };
        }

        /// <summary>
        /// Return the current bitmap as a pixel map for the ChiliFramework
        /// </summary>
        /// <returns>System.String.</returns>
        /// <remarks>no comments</remarks>
        public string PixelMap(Mc.Core.RGB ignoreColor)
        {
            if (_bmp == null) return "// Bitmap is empty";
            var sb = new StringBuilder();
            // display info
            sb.AppendFormat("// original file : " + FileName);
            sb.AppendLine();
            sb.AppendFormat("int width={0};", _bmp.Width);
            sb.AppendFormat("int height={0};", _bmp.Height);
            for (var y = 0; y < _bmp.Height; y++)
            {
                for (var x = 0; x < _bmp.Width; x++)
                {
                    // add putpixel per color,
                    // starting from the origin
                    var c=  _bmp.GetPixel(x, y);
                    
                }
            }
        }
    }

    public static class ColorExtenstions
    {
        public static bool Equals(this Color c, RGB rgb)
        {
            
        }
    }
}
