using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;

namespace I2P
{
    public class AsciiMapper
    {
        private string _folderPath;
        private string _className;

        public static AsciiMapper CreateConverter(string folderPath, string className)
        {
            Mc.Core.Validate.NotNull("folderPath", folderPath);
            var f = new DirectoryInfo(folderPath);
            if (!f.Exists)
            {
                throw new Mc.Core.Exceptions.McException("The folder does not exist:" + folderPath);
            }

            return new AsciiMapper()
            {
                _folderPath = f.FullName,
                _className = className
            };
        }

        public void WriteFile(string outputPath)
        {
            var fileContent = AsciiMap(_className);
            var fileData = UTF8Encoding.ASCII.GetBytes(fileContent);
            using (var fs = new FileStream(outputPath, FileMode.Create))
            {
                fs.Write(fileData, 0, fileData.Length);
            }
        }

        private string AsciiMap(string name)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("#include	\"{0}.h\"", name);
            sb.AppendLine("");
            sb.AppendFormat("{0}::{0}() {{", name);
            sb.AppendLine();
            sb.AppendLine("const uint8_t Ascii::cm[] = {");
            const byte start = 32;
            const byte end = 127;
            for (var c = start; c <= end; c++)
            {
                // try get file
                var fileName = UTF8Encoding.ASCII.GetString(new[] { c });
                // whitelist
                if (@" ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789/\#@?!.:".IndexOf(fileName, StringComparison.Ordinal) < 0)
                {
                    // add empty
                    AddPixels(sb, "Not in map : " + fileName, c);
                }
                else
                {
                    switch (fileName)
                    {
                        case " ":
                            fileName = "space";
                            break;
                        case "@":
                            fileName = "at";
                            break;
                        case "!":
                            fileName = "exclamation";
                            break;
                        case "?":
                            fileName = "questionMark";
                            break;
                        case ".":
                            fileName = "dot";
                            break;
                        case ":":
                            fileName = "colon";
                            break;
                        case "/":
                            fileName = "slash";
                            break;
                        case "\\":
                            fileName = "backslash";
                            break;
                        case "#":
                            fileName = "mc";
                            break;
                        case "0":
                        case "1":
                        case "2":
                        case "3":
                        case "4":
                        case "5":
                        case "6":
                        case "7":
                        case "8":
                        case "9":
                            fileName = "d" + fileName;
                            break;
                    }
                    var f = new FileInfo(_folderPath + "/" + fileName + ".png");
                    if (f.Exists)
                    {
                        // read file and add to stringbuilder
                        var img = Image.FromFile(f.FullName);
                        AddPixels(sb, fileName, c, new Bitmap(img));
                    }
                    else
                    {
                        AddPixels(sb, fileName, c);
                    }
                }
            }
            sb.AppendLine("};");
            return sb.ToString();
        }

        private static void AddPixels(StringBuilder sb, string name, int index, Bitmap bitmap)
        {
            sb.AppendFormat("// {0} = {1} ", name, index);
            sb.AppendLine();
            for (var y = 0; y < bitmap.Height; y++)
            {
                for (var x = 0; x < bitmap.Width && x < 16; x++)
                {
                    // add putpixel per color,
                    // starting from the origin
                    var c = bitmap.GetPixel(x, y);
                    sb.AppendFormat(string.Format(CultureInfo.InvariantCulture, "{0},{1},{2}, ", c.R, c.G, c.B).PadRight(15, ' '));
                }
                // fill until 16 pixels
                for (var x = bitmap.Width; x < 16; x++)
                {
                    sb.Append(@"0,0,0,   ".PadRight(15, ' '));
                }
                sb.AppendLine();
            }
            // empty lines
            for (var y = bitmap.Height; y < 16; y++)
            {
                for (var x = 0; x < 16; x++)
                {
                    sb.Append(@"0,0,0,   ".PadRight(15, ' '));
                }
                sb.AppendLine();
            }
        }

        private static void AddPixels(StringBuilder sb, string name, int index)
        {
            sb.AppendFormat("// {0} = {1} ", name, index);
            sb.AppendLine();
            // empty lines
            for (var y = 0; y < 16; y++)
            {
                for (var x = 0; x < 16; x++)
                {
                    sb.Append(@"0,0,0,   ".PadRight(15, ' '));
                }
                sb.AppendLine();
            }
        }
    }
}