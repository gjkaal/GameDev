﻿using System.Drawing;
using Mc.Core;

namespace McKahl
{
    class Program
    {
        static int Main(string[] args)
        {
            //string target = @"D:\Marjolein\assignment\Bomber\Bomber\Engine";
            ////string spriteSource = @"D:\images";
            //string source = @"D:\images\building";

            //var f = new System.IO.DirectoryInfo(source);
            //foreach(var file in f.GetFiles())
            //{
            //    var fName = file.Name;
            //    fName = fName.Substring(0, fName.IndexOf('.'));
            //    var img = I2P.ImageConverter.CreateConverter(file.FullName, "Building");
            //    var newFile = target + "\\" + fName.Replace("-","") + ".cpp";
            //    img.WriteFile(fName, newFile, 40, 40);
            //}

            //
            //var bullet = I2P.ImageConverter.CreateConverter(@"c:\images\Untitled.png", "Sprites");
            //bullet.CreateRotatedImage(@"c:\images","Bullet", 80, 80, 24);
            //f = new System.IO.DirectoryInfo(source + @"\bullet");
            //foreach (var file in f.GetFiles())
            //{
            //    var fName = file.Name;
            //    fName = fName.Substring(0, fName.IndexOf('.'));
            //    var img = I2P.ImageConverter.CreateConverter(file.FullName, "Sprites");
            //    var newFile = target + "\\" + fName.Replace("-", "") + ".cpp";
            //    img.WriteFile(fName, newFile, 10, 10);
            //}

            // Create an ascii character map
            //var asciiMapFolder = @"C:\Images\ascii";
            //var asciiClassName = "Ascii";
            //var charMap = new I2P.AsciiMapper(asciiMapFolder, asciiClassName, 16, 16, Color.Magenta);
            //charMap.WriteCodeFiles(@"C:\Dev\GameDevelopment\GameDev\Chili Framework 2016\Engine");


            // Create an image map
            //var splashFile = @"c:\images\splash\Splashscreen.png";
            //var splashClassName = "SplashScreen";
            //var imageMap =new I2P.ImageMapper(splashFile, splashClassName, 800, 600, Color.Magenta);
            //imageMap.WriteCodeFiles(@"C:\Dev\GameDevelopment\GameDev\Chili Framework 2016\Engine");

            //// Create an animated map
            //var animateFolder = @"C:\Images\animated\explosion";
            //var animateClassName = "Explosion";
            //var animateMap = new I2P.AnimationMapper(animateFolder, animateClassName, 64, 64, Color.Magenta);
            //animateMap.WriteCodeFiles(@"C:\Dev\GameDevelopment\GameDev\Chili Framework 2016\Engine");

            // C:\Images\blocks
            // Create a bitmap resource class
            var bitmapFolder = @"C:\Images\blocks";
            var bitmapClassName = "Bitmaps";
            var bitmapMap = new I2P.ImageResourceMapper(bitmapFolder, bitmapClassName, 64, 64, Color.Magenta);
            bitmapMap.WriteCodeFiles(@"C:\Dev\GameDevelopment\GameDev\SquiX\Engine");
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

