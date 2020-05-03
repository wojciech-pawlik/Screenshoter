using System;
using System.Drawing;
using System.Windows;

namespace Google_Drive_Screenshoter
{
    class Screenshoter
    {
        public static string MakeScreenshot(
            string filename,
            bool date
            )
        {
            double screenLeft = SystemParameters.VirtualScreenLeft;
            double screenTop = SystemParameters.VirtualScreenTop;
            double screenWidth = SystemParameters.VirtualScreenWidth;
            double screenHeight = SystemParameters.VirtualScreenHeight;

            using (
                var bitmap = new Bitmap(
                    (int)screenWidth,
                    (int)screenHeight)
                )
            {
                using (
                    var graphics = Graphics.FromImage(bitmap)
                )
                {
                    if (date)
                        filename += DateTime.Now.ToString("_dd-MM-yyyy_hh-mm-ss");
                    filename += ".png";
                    graphics.CopyFromScreen(
                        (int)screenLeft,
                        (int)screenTop,
                        0,
                        0,
                        bitmap.Size);
                    bitmap.Save(filename);
                }
            }
            return filename;
        }
    }
}
