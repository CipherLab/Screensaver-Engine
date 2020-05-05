using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Rectangle = System.Drawing.Rectangle;

namespace ScreenSaverHelper.Util
{
    public class CroppedImagePart
    {
        public Rectangle ImageProperties { get; set; }
        public byte[] ImageData { get; set; }

        public Vector2 Vector2
        {
            get
            {
                float x = ImageProperties.X + ImageProperties.Width / 2;
                float y = ImageProperties.Y + ImageProperties.Height / 2;

                return new Vector2(x,y);
            }
        }
    }
}
