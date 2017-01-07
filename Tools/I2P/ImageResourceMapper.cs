using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;

namespace I2P
{
    public class ImageResourceMapper : Mapper
    {

        private ResizeOption _resize;

        public ImageResourceMapper(string sourceFilePath, string className, int xSize, int ySize, Color ignoreColor, ResizeOption resizing = ResizeOption.Resize)
            : base(sourceFilePath, className, xSize, ySize, ignoreColor)
        {
            // should have folder
            if (!IsFolderPath)
            {
                throw new Mc.Core.Exceptions.McException("ImageResourceMapper requires a folder but received a file:" + sourceFilePath);
            }
            _resize = resizing;
        }


        public override void WriteCodeFiles(string outputPath)
        {
            var headerFile = outputPath + "\\" + ClassName + ".h";
            var codeFile = outputPath + "\\" + ClassName + ".cpp";
            // write header
            var fileData = Encoding.ASCII.GetBytes(HeaderFile());
            using (var fs = new FileStream(headerFile, FileMode.Create))
            {
                fs.Write(fileData, 0, fileData.Length);
            }
            // write code
            var fileContent = DefinitionFile();
            fileData = Encoding.ASCII.GetBytes(fileContent);
            using (var fs = new FileStream(codeFile, FileMode.Create))
            {
                fs.Write(fileData, 0, fileData.Length);
            }
        }


        private string HeaderFile()
        {
            var sb = new StringBuilder();
            sb.AppendFormat(@"
#pragma once
#include ""stdint.h""

class {0}
{{
public:
", ClassName);
            var totalMapSize = 0;
            var imageCount = 0;
            foreach (var source in GetImages())
            {
                var imgSize = 0;
                Bitmap img;
                switch (_resize)
                {
                    case ResizeOption.None:
                        img = new Bitmap(source.Image);
                        break;
                    case ResizeOption.Resize:
                        img = new Bitmap(source.Image, XSize, YSize);
                        break;
                    default:
                        throw new NotImplementedException();
                }
                imgSize = img.Width * img.Height;

                sb.AppendFormat("// frame n = {0} starting at position [{1}];", source.Name, totalMapSize);
                sb.AppendLine();
                totalMapSize += imgSize;
                imageCount++;

                sb.AppendFormat(@"const static unsigned int {0}={1};", source.Name.ToLowerInvariant(), totalMapSize);
                sb.AppendLine();
                sb.AppendFormat(@"const static unsigned int {0}XSize={1};", source.Name.ToLowerInvariant(), img.Width);
                sb.AppendLine();
                sb.AppendFormat(@"const static unsigned int {0}YSize={1};", source.Name.ToLowerInvariant(), img.Height);
                sb.AppendLine();
            }
            // add frame map
            // add frame count
            sb.AppendFormat(@"const static unsigned int imageData[{0}];	", totalMapSize);
            sb.AppendLine();
            sb.AppendFormat(@"const static int imageInfo[{0}];	", imageCount * 3);
            sb.AppendLine();
            sb.AppendFormat(@"const static int imageCount={0};	", imageCount);
            sb.AppendLine();

            sb.AppendLine("};");
            return sb.ToString();
        }

        private string DefinitionFile()
        {
            var d = new DirectoryInfo(FilePath);
            if (!d.Exists) return "// Folder is not valid : " + FilePath;
            var sb = new StringBuilder();
            var sbImgInfo = new StringBuilder();
            sb.AppendFormat("#include	\"{0}.h\"", ClassName);
            sb.AppendLine("");
            sb.AppendFormat("const unsigned int {0}::imageData[] = {{", ClassName);
            sb.AppendLine();

            var totalMapSize = 0;
            var imageCount = 0;
            foreach (var source in GetImages())
            {
                var imgSize = 0;
                Bitmap img;
                switch (_resize)
                {
                    case ResizeOption.None:
                        img = new Bitmap(source.Image);
                        break;
                    case ResizeOption.Resize:
                        img = new Bitmap(source.Image, XSize, YSize);
                        break;
                    default:
                        throw new NotImplementedException();
                }
                imgSize = img.Width * img.Height;

                sbImgInfo.AppendFormat("{0},{1},{2},", totalMapSize, img.Width, img.Height);
                sb.AppendFormat("// frame n = {0} starting at position [{1}];", source.Name, totalMapSize);
                sb.AppendLine();
                totalMapSize += imgSize;
                imageCount++;
                //sb.AppendFormat("const static unsigned int {0}[{1}];",name, mapSize);
                for (var y = 0; y < img.Height; y++)
                {
                    for (var x = 0; x < img.Width; x++)
                    {
                        // add RGB value per color,
                        var c = (uint)img.GetPixel(x, y).ToArgb();
                        sb.AppendFormat(string.Format(CultureInfo.InvariantCulture, "{0}, ", c).PadRight(8, ' '));
                    }
                    sb.AppendLine();
                }
            }

            // end imgData
            sb.AppendLine("};");
            sb.AppendFormat("const unsigned int {0}::imageInfo[] = {{{1}}};", ClassName, sbImgInfo);
            sb.AppendLine();
            return sb.ToString();

        }
    }
}
