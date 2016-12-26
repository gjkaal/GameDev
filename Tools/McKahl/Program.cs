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
            //string spriteSource = @"D:\images";
            string source = @"D:\images\building";

            var f = new System.IO.DirectoryInfo(source);
            foreach(var file in f.GetFiles())
            {
                var fName = file.Name;
                fName = fName.Substring(0, fName.IndexOf('.'));
                var img = I2P.ImageConverter.CreateConverter(file.FullName, "Building");
                var newFile = target + "\\" + fName.Replace("-","") + ".cpp";
                img.WriteFile(fName, newFile, 40, 40);
            }

            //
           // var bullet = I2P.ImageConverter.CreateConverter(source + @"\bullet.png");
           // bullet.CreateRotatedImage(source,"Bullet", 40, 40, 24);

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
