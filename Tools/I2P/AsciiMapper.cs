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
        private int _xSize;
        private int _ySize;
        private const int firstChar = 32; // space
        private const int lastChar = 127; // DEL


        public static AsciiMapper CreateConverter(string folderPath, string className, int xSize, int ySize)
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
                _className = className,
                _xSize = xSize,
                _ySize = ySize
            };
        }

        public void WriteCodeFiles(string outputPath)
        {
            var headerFile = outputPath + "\\" + _className + ".h";
            var codeFile = outputPath + "\\" + _className + ".cpp";
            var mapSize = (lastChar - firstChar + 1)*_xSize*_ySize*3; //73728 with 16 x 16 characters
            byte[] fileData;

            var header = string.Format(@"
#pragma once
#include ""stdint.h""

class {0}
{{
public:
	const static uint8_t cm[{1}];	
}};", _className, mapSize);
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
            sb.AppendFormat("#include	\"{0}.h\"", _className);
            sb.AppendLine("");
            sb.AppendFormat("const uint8_t {0}::cm[] = {{", _className);
            sb.AppendLine();
            const byte start = firstChar;
            const byte end = lastChar;
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
                    var f = new FileInfo(_folderPath + "/" + fileName + ".png");
                    if (f.Exists)
                    {
                        // read file and add to stringbuilder
                        var img = Image.FromFile(f.FullName);
                        AddPixels(sb, fileName, c, new Bitmap(img,_xSize, _ySize));
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
            for (var y = 0; y < bitmap.Height && y < _ySize; y++)
            {
                for (var x = 0; x < bitmap.Width && x < _xSize; x++)
                {
                    // add putpixel per color,
                    // starting from the origin
                    var c = bitmap.GetPixel(x, y);
                    sb.AppendFormat(string.Format(CultureInfo.InvariantCulture, "{0},{1},{2}, ", c.R, c.G, c.B).PadRight(15, ' '));
                }
                // fill until size 
                for (var x = bitmap.Width; x < _xSize; x++)
                {
                    sb.Append(@"255,0,255,".PadRight(15, ' '));
                }
                sb.AppendLine();
            }
            // empty lines
            for (var y = bitmap.Height; y < _ySize; y++)
            {
                for (var x = 0; x < _xSize; x++)
                {
                    sb.Append(@"255,0,255,".PadRight(15, ' '));
                }
                sb.AppendLine();
            }
        }

        private  void AddPixels(StringBuilder sb, string name, int index)
        {
            sb.AppendFormat("// {0} = {1} ", name, index);
            sb.AppendLine();
            // empty lines
            for (var y = 0; y < _xSize; y++)
            {
                for (var x = 0; x < _ySize; x++)
                {
                    sb.Append(@"255,0,255,".PadRight(15, ' '));
                }
                sb.AppendLine();
            }
        }
    }
}