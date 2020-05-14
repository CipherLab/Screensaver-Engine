using System.Drawing;
using Rectangle = System.Drawing.Rectangle;

namespace SharedKernel.Interfaces
{
    public interface ICroppedImagePart
    {
        Rectangle ImageProperties { get; set; }
        byte[] ImageData { get; set; }
        PointF Position { get; }
    }
}