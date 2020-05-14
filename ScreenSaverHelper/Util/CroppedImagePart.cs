using System.Drawing;
using SharedKernel.Interfaces;
using Rectangle = System.Drawing.Rectangle;

namespace ScreenSaverHelper.Util
{
    public class CroppedImagePart : ICroppedImagePart
    {
        public Rectangle ImageProperties { get; set; }
        public byte[] ImageData { get; set; }
        public PointF Position
        {
            get
            {
                float x = ImageProperties.X + ImageProperties.Width / 2f;
                float y = ImageProperties.Y + ImageProperties.Height / 2f;

                return new PointF(x, y);
            }
        }

    }
}
