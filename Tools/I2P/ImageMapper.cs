using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;

namespace I2P
{


    public class ImageMapper : Mapper
    {

        public ImageMapper(string sourceFilePath, string className, int xSize, int ySize, Color ignoreColor) : base(sourceFilePath, className, xSize, ySize, ignoreColor)
        {
            // should have folder
            if (!IsFilePath)
            {
                throw new Mc.Core.Exceptions.McException("AnimationMapper requires a file but received a folder:" + sourceFilePath);
            }
        }

        public override void WriteCodeFiles(string outputPath)
        {
            var headerFile = outputPath + "\\" + ClassName + ".h";
            var codeFile = outputPath + "\\" + ClassName + ".cpp";

            // get the file
            var source = Image.FromFile(FilePath);
            var img = new Bitmap(source, XSize, YSize);
            var mapSize = img.Width * img.Height; 
            byte[] fileData;
            var header = string.Format(@"
#pragma once
#include ""stdint.h""

class {0}
{{
public:
	const static unsigned int imgData[{1}];	
}};", ClassName, mapSize);
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
            sb.AppendFormat("#include	\"{0}.h\"", ClassName);
            sb.AppendLine("");
            sb.AppendFormat("const unsigned int {0}::imgData[] = {{", ClassName);
            sb.AppendLine();
            sb.AppendFormat("// Image {0} columns x {1} rows pixels", img.Width, img.Height);
            sb.AppendLine();
            for (var y = 0; y < img.Height; y++)
            {
                for (var x = 0; x < img.Width; x++)
                {
                    // add RGB value per color,
                    var c = (uint)img.GetPixel(x, y).ToArgb();
                    if (c == IgnoreColor.ToArgb())
                    {
                        sb.Append("0, ".PadRight(8, ' '));
                    }
                    else
                    {
                        sb.Append(string.Format(CultureInfo.InvariantCulture, "{0}, ", c).PadRight(8, ' '));
                    }
                    
                }
                sb.AppendLine();
            }
            // end imgData
            sb.AppendLine("};");
            return sb.ToString();
        }
    }
}