using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace WpfApp
{
    public class WindowsFormsHelper
    {
        public static byte[] ScreenGrab()
        {
            using (Bitmap bmpScreenCapture = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                Screen.PrimaryScreen.Bounds.Height))
            using (Graphics g = Graphics.FromImage(bmpScreenCapture))
            {
                g.CopyFromScreen(Screen.PrimaryScreen.Bounds.X,
                    Screen.PrimaryScreen.Bounds.Y,
                    0, 0,
                    bmpScreenCapture.Size,
                    CopyPixelOperation.SourceCopy);
                using (MemoryStream ms = new MemoryStream())
                {
                    bmpScreenCapture.Save(ms, ImageFormat.Jpeg);
                    return ms.ToArray();
                }
            }

        }
    }
}
