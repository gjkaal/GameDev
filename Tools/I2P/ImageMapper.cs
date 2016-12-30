using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;

namespace I2P
{
    public class ImageMapper
    {

        private string _filePath;
        private string _className;
        private int _xSize;
        private int _ySize;

        public static ImageMapper CreateConverter(string sourceFilePath, string className,int xSize,int ySize)
        {
            Mc.Core.Validate.NotNull("sourceFilePath", sourceFilePath);
            var f = new FileInfo(sourceFilePath);
            if (!f.Exists)
            {
                throw new Mc.Core.Exceptions.McException("The file does not exist:" + sourceFilePath);
            }

            return new ImageMapper()
            {
                _filePath = f.FullName,
                _className = className,
                _xSize = xSize,
                _ySize = ySize,
            };
        }

        public void WriteCodeFiles(string outputPath)
        {
            var headerFile = outputPath + "\\" + _className + ".h";
            var codeFile = outputPath + "\\" + _className + ".cpp";

            // get the file
            var source = Image.FromFile(_filePath);
            var img = new Bitmap(source, _xSize, _ySize);
            var mapSize = img.Width * img.Height; // 3 bytes per pixel
            byte[] fileData;

            var header = string.Format(@"
#pragma once
#include ""stdint.h""

class {0}
{{
public:
	const static unsigned int imgData[{1}];	
}};", _className, mapSize);
            // write header
            fileData = Encoding.ASCII.GetBytes(header);
            using (var fs = new FileStream(headerFile, FileMode.Create))
            {
                fs.Write(fileData, 0, fileData.Length);
            }
            // write code
            var fileContent = ImageMap(img);
            fileData = Encoding.ASCII.GetBytes(fileContent);
            using (var fs = new FileStream(codeFile, FileMode.Create))
            {
                fs.Write(fileData, 0, fileData.Length);
            }
        }

        private string ImageMap(Bitmap img)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("#include	\"{0}.h\"", _className);
            sb.AppendLine("");
            sb.AppendFormat("const unsigned int {0}::imgData[] = {{", _className);
            sb.AppendLine();
            sb.AppendFormat("// Image {0} columns x {1} rows pixels", img.Width, img.Height);
            sb.AppendLine();
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
            // end imgData
            sb.AppendLine("};");
            return sb.ToString();
        }
    }
}