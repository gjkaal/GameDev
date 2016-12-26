using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Mc.Core;
using System.Drawing.Imaging;

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
        //private System.Drawing.Bitmap _bmp;
        private string _filePath;
        private string _className;

        //public string FileName { get; private set; }
        public RGB IgnoreColor { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageConverter"/> class.
        /// </summary>
        /// <param name="bmp">The BMP.</param>
        /// <remarks>no comments</remarks>
        public ImageConverter()
        {
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
        public static ImageConverter CreateConverter(string filePath, string className)
        {
            Mc.Core.Validate.NotNull("filePath", filePath);
            var f = new FileInfo(filePath);
            if (!f.Exists)
            {
                throw new Mc.Core.Exceptions.McException("The file does not exist:" + filePath);
            }

            return new ImageConverter()
            {
                _filePath = filePath,
                _className = className
            };
        }

        public void WriteFile(string name, string outputPath, int xSize, int ySize)
        {
            var fileContent = Sprite(name, xSize, ySize);
            var fileData = UTF8Encoding.ASCII.GetBytes(fileContent);
            using (var fs = new FileStream(outputPath, FileMode.Create))
            {
                fs.Write(fileData, 0, fileData.Length);
            }
        }

        public void CreateRotatedImage(string outputPath, string fileName, int xSize, int ySize, int steps)
        {
            var fName = outputPath + "\\" + fileName;
            for (var a = 0; a < 360; a += (360 / steps))
            {
                var angle = (float)a;
                var rotated = Rotate(xSize, ySize, angle);
                var newName = fName + a + ".png";
                if (File.Exists(newName))
                {
                    File.Delete(newName);
                }
                rotated.Save(newName, ImageFormat.Png);
            }

        }

        public string Sprite(string name, int xSize, int ySize)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("# include \"{0}.h\"", _className);
            sb.AppendLine();
            sb.AppendFormat("void {1}::{0}(int x, int y, Graphics & gfx)", name.Replace("-", ""), _className);
            sb.AppendLine("{");
            AddPixelMapReSized(sb, xSize, ySize);
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
            var img = Image.FromFile(_filePath);
            if (img == null)
            {
                sb.AppendLine("// No image");
                return;
            }

            // display info
            var bmp = new Bitmap(img);
            sb.AppendFormat("// original file : " + _filePath);
            sb.AppendLine();
            AddPixelMap(IgnoreColor, sb, bmp);
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
            var img = Image.FromFile(_filePath);
            if (img == null)
            {
                sb.AppendLine("// No image");
                return;
            }
            var bmp = new Bitmap(img);
            // display info
            sb.AppendFormat("// original file : " + _filePath);
            sb.AppendLine();
            sb.AppendFormat("// original width={0};", bmp.Width);
            sb.AppendLine();
            sb.AppendFormat("// original height={0};", bmp.Height);
            sb.AppendLine();
            Bitmap resized = new Bitmap(bmp, new Size(xSize, ySize));

            AddPixelMap(IgnoreColor, sb, resized);
            return;
        }

        private Bitmap Rotate(int xSize, int ySize, float angle)
        {
            var img = Image.FromFile(_filePath);
            var e = Graphics.FromImage(img);
            float moveX = img.Width / 2f;
            float moveY = img.Height / 2f;
            e.TranslateTransform(moveX, moveY);
            e.RotateTransform(angle);
            e.TranslateTransform(-moveX, -moveY);
            e.DrawImage(img, 0, 0);
            return new Bitmap(img, new Size(xSize, ySize));
        }

        private static void AddPixelMap(RGB ignoreColor, StringBuilder sb, Bitmap bitmap)
        {
            sb.AppendFormat("// width={0};", bitmap.Width);
            sb.AppendLine();
            sb.AppendFormat("// height={0};", bitmap.Height);
            sb.AppendLine();
            for (var y = 0; y < bitmap.Height; y++)
            {
                sb.AppendFormat("// Row {0}", y);
                sb.AppendLine();
                for (var x = 0; x < bitmap.Width; x++)
                {
                    // add putpixel per color,
                    // starting from the origin
                    var c = bitmap.GetPixel(x, y);
                    var testIgnore = ColorExtenstions.Equals(c, ignoreColor);
                    if (!testIgnore)
                    {
                        if (x == 0)
                        {
                            sb.AppendFormat("gfx.PutPixel(x, {0} + y, {1},{2},{3});", y, c.R, c.G, c.B);
                        }
                        else if (y == 0)
                        {
                            sb.AppendFormat("gfx.PutPixel({0} + x, y,  {1},{2},{3});", x, c.R, c.G, c.B);
                        }
                        else
                        {
                            sb.AppendFormat("gfx.PutPixel({0} + x, {1} + y, {2},{3},{4});", x, y, c.R, c.G, c.B);
                        }

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
            var dif = 0;
            dif += Math.Abs(c.R - rgb.R);
            dif += Math.Abs(c.G - rgb.G);
            dif += Math.Abs(c.B - rgb.B);

            if (dif < 5) return true;
            return false;
        }
    }
}
