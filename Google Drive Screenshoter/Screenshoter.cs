using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.IO;
using System.Collections.Generic;

namespace Google_Drive_Screenshoter
{
    class Screenshoter
    {
        private static double SCREEN_LEFT = SystemParameters.VirtualScreenLeft;
        private static double SCREEN_TOP = SystemParameters.VirtualScreenTop;
        private static double SCREEN_WIDTH = SystemParameters.VirtualScreenWidth;
        private static double SCREEN_HEIGHT = SystemParameters.VirtualScreenHeight;

        public List<string> filesToUpload;

        public static string MakeScreenshot(string filename, bool date)
        {

            using (
                var bitmap = new Bitmap(
                    (int)SCREEN_WIDTH,
                    (int)SCREEN_HEIGHT)
                )
            {
                
                using (
                    var graphics = Graphics.FromImage(bitmap)
                )
                {
                    if (date)
                        filename += DateTime.Now.ToString("_yyyy-MM-dd_HH-mm-ss");
                    filename += ".png";

                    /*Check if the file exists - neccessary to avoid crash (screenshot without a date or captured a few within a second).*/
                    filename = CheckFilename(filename);

                    graphics.CopyFromScreen(
                        (int)SCREEN_LEFT,
                        (int)SCREEN_TOP,
                        0,
                        0,
                        bitmap.Size);


                    bitmap.Save(filename);
                }
            }
            return filename;
        }

        private static string CheckFilename(string filename)
        {
            /* If file exists, add (n) into the name */
            if (File.Exists(filename))
            {
                filename = filename.Replace(".png", "(1).png");
                Debug.WriteLine($"filename: {filename}");
            }
                
            int i = 1;
            while (File.Exists(filename))
            {
                filename = filename.Replace($"({i}).png", $"({i + 1}).png");
                Debug.WriteLine($"filename: {filename}");
                i++;
            }
            return filename;
        }
    }
}
