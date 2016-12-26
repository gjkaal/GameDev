using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mc.Core;

namespace McKahl
{
    class Program
    {
        static int Main(string[] args)
        {
            string target = @"D:\Marjolein\assignment\Bomber\Bomber\Engine";
            string source = @"D:\images";

            var f = new System.IO.DirectoryInfo(source);
            foreach(var file in f.GetFiles())
            {
                var fName = file.Name;
                fName = fName.Substring(0, fName.IndexOf('.'));
                var img = I2P.ImageConverter.CreateConverter(file.FullName);
                var newFile = target + "\\Sprites" + fName + ".cpp";
                img.WriteFile(fName, newFile, 40, 40);
            }
            // Create a new command line interpreter class
            // and use it to pass parameters to tools classes
            int result =0;
            var cli = new CLI(args);
            if (cli.HasInstruction("I2P"))
            {
                
            }
            return result;
        }
    }
}
