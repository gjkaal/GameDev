using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;

namespace I2P
{
    public class AsciiMapper : Mapper
    {
        private const int FirstChar = 32; // space
        private const int LastChar = 127; // DEL
        private string _ignoreColor;

        public AsciiMapper(string folderPath, string className, int xSize, int ySize, Color ignoreColor) : base(folderPath, className, xSize, ySize, ignoreColor)
        {

            _ignoreColor = ((uint)ignoreColor.ToArgb()).ToString(CultureInfo.InvariantCulture) + ",";
        }

        public override void WriteCodeFiles(string outputPath)
        {
            var headerFile = outputPath + "\\" + ClassName + ".h";
            var codeFile = outputPath + "\\" + ClassName + ".cpp";
            var mapSize = (LastChar - FirstChar + 1) * XSize * YSize * 3; //73728 with 16 x 16 characters
            byte[] fileData;

            var header = string.Format(@"
#pragma once
#include ""stdint.h""

class {0}
{{
public:
	const static unsigned int cm[{1}];	
}};", ClassName, mapSize);
            // write header
            fileData = Encoding.ASCII.GetBytes(header);
            using (var fs = new FileStream(headerFile, FileMode.Create))
            {
                fs.Write(fileData, 0, fileData.Length);
            }
            // write code
            var fileContent = AsciiMap();
            fileData = Encoding.ASCII.GetBytes(fileContent);
            using (var fs = new FileStream(codeFile, FileMode.Create))
            {
                fs.Write(fileData, 0, fileData.Length);
            }
        }

        private string AsciiMap()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("#include	\"{0}.h\"", ClassName);
            sb.AppendLine("");
            sb.AppendFormat("const unsigned int {0}::cm[] = {{", ClassName);
            sb.AppendLine();
            const byte start = FirstChar;
            const byte end = LastChar;
            for (var c = start; c <= end; c++)
            {
                // try get file
                var fileName = Encoding.ASCII.GetString(new[] { c });
                // TODO : name exotic characters whitelist
                if (@" abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789/\#@?!.:".IndexOf(fileName, StringComparison.Ordinal) < 0)
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
                    var f = new FileInfo(FilePath + "/" + fileName + ".png");
                    if (f.Exists)
                    {
                        // read file and add to stringbuilder
                        var img = Image.FromFile(f.FullName);
                        AddPixels(sb, fileName, c, new Bitmap(img, XSize, YSize));
                    }
                    else
                    {
                        AddPixels(sb, "No file : " + fileName, c);
                    }
                }
            }
            sb.AppendLine("};");
            return sb.ToString();
        }

        private void AddPixels(StringBuilder sb, string name, int index, Bitmap bitmap)
        {

            sb.AppendFormat("// {0} = {1} ", name, index);
            sb.AppendLine();
            for (var y = 0; y < bitmap.Height && y < YSize; y++)
            {
                for (var x = 0; x < bitmap.Width && x < XSize; x++)
                {
                    // add putpixel per color,
                    // starting from the origin
                    var c = (uint)bitmap.GetPixel(x, y).ToArgb();
                    sb.AppendFormat(string.Format(CultureInfo.InvariantCulture, "{0},", c).PadRight(10, ' '));
                }
                // fill until size 
                for (var x = bitmap.Width; x < XSize; x++)
                {
                    sb.Append(_ignoreColor);
                }
                sb.AppendLine();
            }
            // empty lines
            for (var y = bitmap.Height; y < YSize; y++)
            {
                for (var x = 0; x < XSize; x++)
                {
                    sb.Append(_ignoreColor);
                }
                sb.AppendLine();
            }
        }

        private void AddPixels(StringBuilder sb, string name, int index)
        {
            sb.AppendFormat("// {0} = {1} ", name, index);
            sb.AppendLine();
            // empty lines
            for (var y = 0; y < YSize; y++)
            {
                for (var x = 0; x < XSize; x++)
                {
                    sb.Append(_ignoreColor);
                }
                sb.AppendLine();
            }
        }
    }
}