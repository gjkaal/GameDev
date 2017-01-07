using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;

namespace I2P
{
    public class AnimationMapper : Mapper
    {
        public AnimationMapper(string sourceFilePath, string className, int xSize, int ySize, Color ignoreColor)
            : base(sourceFilePath, className, xSize, ySize, ignoreColor)
        {
            // should have folder
            if (!IsFolderPath)
            {
                throw new Mc.Core.Exceptions.McException("AnimationMapper requires a folder but received a file:" + sourceFilePath);
            }
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
            var d = new DirectoryInfo(FilePath);
            if (!d.Exists) return "// Folder is not valid : " + FilePath;
            var sb = new StringBuilder();
            sb.AppendFormat(@"
#pragma once
#include ""stdint.h""

class {0}
{{
public:
", ClassName);
            var totalMapSize = 0;
            var frameCount = 0;
            var frameSize = 0;
            foreach (var source in GetImages())
            {
                var name = source.Name;
                name = name.Substring(0, name.IndexOf('.'));
                var img = new Bitmap(source.Image, XSize, YSize);
                frameSize = img.Width * img.Height;
                sb.AppendFormat("// frame n = {0} starting at position [{1}];", name, totalMapSize);
                sb.AppendLine();
                totalMapSize += frameSize;
                frameCount++;
                //sb.AppendFormat("const static unsigned int {0}[{1}];",name, mapSize);

            }
            // add frame map
            // add frame count
            sb.AppendFormat(@"const static unsigned int frameData[{0}];	", totalMapSize);
            sb.AppendLine();
            sb.AppendFormat(@"const static int frameCount={0};	", frameCount);
            sb.AppendLine();
            sb.AppendFormat(@"const static int frameSize={0};	", frameSize);
            sb.AppendLine();
            sb.AppendFormat(@"const static int XSize={0};	", XSize);
            sb.AppendLine();
            sb.AppendFormat(@"const static int YSize={0};	", YSize);
            sb.AppendLine();
            sb.AppendLine("};");
            return sb.ToString();
        }

        private string DefinitionFile()
        {
            var d = new DirectoryInfo(FilePath);
            if (!d.Exists) return "// Folder is not valid : " + FilePath;
            var sb = new StringBuilder();

            sb.AppendFormat("#include	\"{0}.h\"", ClassName);
            sb.AppendLine("");
            sb.AppendFormat("const unsigned int {0}::frameData[] = {{", ClassName);
            sb.AppendLine();

            var totalMapSize = 0;
            foreach (var source in GetImages())
            {
                var img = new Bitmap(source.Image, XSize, YSize);
                var mapSize = img.Width * img.Height;
                sb.AppendFormat("// frame n = {0} starting at position [{1}];", source.Name, totalMapSize);
                sb.AppendLine();
                totalMapSize += mapSize;
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
            return sb.ToString();
        }
    }
}