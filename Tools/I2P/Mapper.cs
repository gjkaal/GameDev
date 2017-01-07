using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace I2P
{
    public class ImageInfo
    {
        public string Name { get; set; }
        public Image Image { get; set; }
    }

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

        protected IEnumerable<ImageInfo> GetImages()
        {
            if (!IsFolderPath) yield break;
            var d = new DirectoryInfo(FilePath);
            if (!d.Exists) yield break;
            foreach (var fileInfo in d.GetFiles())
            {
                // files that start with underscores are not added by design
                Image source;
                if (fileInfo.Name.StartsWith("_")) continue;
                // check extensions
                try
                {
                    source = Image.FromFile(fileInfo.FullName);
                }
                catch (Exception)
                {
                    // not an image
                    source = null;
                }
                if (source != null)
                {
                    var name = fileInfo.Name;
                    name = name.Substring(0, name.IndexOf('.'));
                    yield return new ImageInfo
                    {
                        Image = source,
                        Name = name
                    };
                }
            }
        }

        public abstract void WriteCodeFiles(string outputPath);
    }
}