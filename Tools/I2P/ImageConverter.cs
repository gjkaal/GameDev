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
        public RGB IgnoreColor { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageConverter"/> class.
        /// </summary>
        /// <param name="bmp">The BMP.</param>
        /// <remarks>no comments</remarks>
        public ImageConverter(Bitmap bmp)
        {
            _bmp = bmp;
            // Ignore black
            IgnoreColor = new RGB { R = 0, G = 0, B = 0 };
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

        public void WriteFile(string name, string outputPath, int xSize, int ySize)
        {
            var fileContent = Sprite(name,  xSize,  ySize);
            var fileData = UTF8Encoding.ASCII.GetBytes(fileContent);
            using (var fs = new FileStream(outputPath, FileMode.Create))
            {
                fs.Write(fileData, 0, fileData.Length);
            }
        }

        public string Sprite(string name, int xSize, int ySize)
        {
            var sb = new StringBuilder();
            sb.AppendLine("# include \"Sprites.h\"");
            sb.AppendFormat("void Sprites::{0}(int x, int y, Graphics & gfx)",name);
            sb.AppendLine("{");
            AddPixelMapReSized(sb,xSize, ySize);
            sb.AppendLine("};");
            return sb.ToString();
        }

        /// <summary>
        /// Return the current bitmap as a pixel map for the ChiliFramework
        /// </summary>
        /// <returns>System.String.</returns>
        /// <remarks>no comments</remarks>
        public void AddPixelMap(StringBuilder sb)
        {
            Validate.NotNull("sb", sb);
            if (_bmp == null)
            {
                sb.AppendLine("// Bitmap is empty");
                return;
            }
            
            // display info
            sb.AppendFormat("// original file : " + FileName);
            sb.AppendLine();
            AddPixelMap(IgnoreColor, sb, _bmp);
            return;
        }

        /// <summary>
        /// Return the current bitmap as a pixel map for the ChiliFramework
        /// </summary>
        /// <returns>System.String.</returns>
        /// <remarks>no comments</remarks>
        public void AddPixelMapReSized(StringBuilder sb, int xSize, int ySize)
        {
            Validate.NotNull("sb", sb);
            if (_bmp == null)
            {
                sb.AppendLine("// Bitmap is empty");
                return;
            }
            // display info
            sb.AppendFormat("// original file : " + FileName);
            sb.AppendLine();
            sb.AppendFormat("// original width={0};", _bmp.Width);
            sb.AppendLine();
            sb.AppendFormat("// original height={0};", _bmp.Height);
            sb.AppendLine();
            Bitmap resized = new Bitmap(_bmp, new Size(xSize, ySize));
            AddPixelMap(IgnoreColor, sb, resized);
            return;
        }

        private static void AddPixelMap(RGB ignoreColor, StringBuilder sb, Bitmap bitmap)
        {
            sb.AppendFormat("// width={0};", bitmap.Width);
            sb.AppendLine();
            sb.AppendFormat("// height={0};", bitmap.Height);
            sb.AppendLine();
            for (var y = 0; y < bitmap.Height; y++)
            {
                for (var x = 0; x < bitmap.Width; x++)
                {
                    // add putpixel per color,
                    // starting from the origin
                    var c = bitmap.GetPixel(x, y);
                    var testIgnore = ColorExtenstions.Equals(c, ignoreColor);
                    if (!testIgnore)
                    {
                        sb.AppendFormat("gfx.PutPixel({0} + x, {1} + y, {2},{3},{4});", x, y, c.R, c.G, c.B);
                        sb.AppendLine();
                    }

                }
            }
        }
    }

    public static class ColorExtenstions
    {
        public static bool Equals(this Color c, RGB rgb)
        {
            if (c.R != rgb.R) return false;
            if (c.G != rgb.G) return false;
            if (c.B != rgb.B) return false;
            return true;
        }
    }
}
