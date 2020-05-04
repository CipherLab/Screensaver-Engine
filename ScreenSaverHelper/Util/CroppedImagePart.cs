using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ScreenSaverHelper.Util
{
    public class CroppedImagePart
    {
        public Rectangle ImageProperties { get; set; }
        public byte[] ImageData { get; set; }
    }
}
