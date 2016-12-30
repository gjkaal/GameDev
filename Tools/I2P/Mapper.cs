using System.Drawing;
using System.IO;

namespace I2P
{
    public abstract class Mapper
    {
        protected bool IsFilePath;
        protected bool IsFolderPath;
        protected string FilePath;
        protected string ClassName;
        protected int XSize;
        protected int YSize;
        protected Color IgnoreColor;

        public Mapper(string sourceFilePath, string className, int xSize, int ySize, Color ignoreColor)
        {
            IsFolderPath = false;
            IsFilePath = false;
            Mc.Core.Validate.NotNull("sourceFilePath", sourceFilePath);
            var f = new FileInfo(sourceFilePath);
            if (f.Exists)
            {
                IsFilePath = true;
                IsFolderPath = false;

            }

            var d = new DirectoryInfo(sourceFilePath);
            if (d.Exists)
            {
                IsFilePath = false;
                IsFolderPath = true;
            }

            if (!(IsFilePath || IsFolderPath))
            {
                throw new Mc.Core.Exceptions.McException("The file or folder does not exist:" + sourceFilePath);
            }

            FilePath = f.FullName;
            ClassName = className;
            XSize = xSize;
            YSize = ySize;
            IgnoreColor = ignoreColor;

        }

        public abstract void WriteCodeFiles(string outputPath);
    }
}