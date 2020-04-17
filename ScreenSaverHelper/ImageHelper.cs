using ImageMagick;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;

namespace ScreenSaverHelper
{
    public class ImageHelper
    {
        private Rectangle Bounds { get; }
        public bool FancyTile { get; }

        public ImageHelper(Rectangle bounds, bool fancyTile)
        {
            Bounds = bounds;
            FancyTile = fancyTile;
        }
        public byte[] MirrorUpconvertImage(string i)
        {
            if (FancyTile)
            {
                using (MagickImage orig = new MagickImage(i))
                {
                    int origWidth = orig.Width;
                    int origHeight = orig.Height;

                    if (origHeight >= Bounds.Height && origWidth >= Bounds.Width)
                    {
                        Debug.WriteLine($"{new FileInfo(i).Name} - Original image is big enough");
                        return new MagickImage(i).ToByteArray();
                    }

                    if (origHeight >= Bounds.Height && origWidth < Bounds.Height)
                    {
                        Debug.WriteLine($"{new FileInfo(i).Name} - Not wide enough");
                        var mirroredWImage = MirrorLeftAndRight(orig.ToByteArray(), origWidth, origHeight);
                        return mirroredWImage;
                    }

                    if (origWidth >= Bounds.Width && origHeight < Bounds.Height)
                    {
                        Debug.WriteLine($"{new FileInfo(i).Name} - Not tall enough");
                        var mirroredHImage = MirrorUpAndDown(orig.ToByteArray(), origWidth, Bounds.Height);
                        return mirroredHImage;
                    }

                    if (origHeight < Bounds.Height && origWidth < Bounds.Width)
                    {
                        Debug.WriteLine($"{new FileInfo(i).Name} - Not tall or wide enough");
                        var mirroredWImage = MirrorLeftAndRight(orig.ToByteArray(), origWidth, origHeight);

                        //pass the niw extra wide one to be mirrored top and bottom
                        var mirroredHImage = MirrorUpAndDown(mirroredWImage.ToArray(), Bounds.Width, origHeight);
                        return mirroredHImage;
                    }

                }
            }
            return new MagickImage(i).ToByteArray();
        }

        private byte[] MirrorUpAndDown(byte[] orig, int origWidth, int origHeight)
        {
            using (MagickImage top = new MagickImage(orig.ToArray()))
            using (MagickImage bottom = new MagickImage(orig.ToArray()))
            {
                var hDif = (Bounds.Height - origHeight) / 2;

                var geom1 = new MagickGeometry(0, 0, origWidth, hDif);
                top.Crop(geom1);
                var geom2 = new MagickGeometry(0, origHeight - hDif, origWidth, hDif);
                bottom.Crop(geom2);

                using (var imageCol = new MagickImageCollection())
                {
                    top.Flip();
                    bottom.Flip();
                    imageCol.Add(top);
                    imageCol.Add(new MagickImage(orig));
                    imageCol.Add(bottom);

                    using (var result = imageCol.AppendVertically())
                    {
                        var size = new MagickGeometry(origWidth, Bounds.Height);
                        size.IgnoreAspectRatio = true;
                        result.Resize(size);

                        return result.ToByteArray();
                    }
                }
            }
        }

        private byte[] MirrorLeftAndRight(byte[] orig, int origWidth, int origHeight)
        {
            using (MagickImage left = new MagickImage(orig.ToArray()))
            using (MagickImage right = new MagickImage(orig.ToArray()))
            {
                var wDif = (Bounds.Width - origWidth) / 2;

                var geom1 = new MagickGeometry(origWidth - wDif, 0, wDif, origHeight);
                right.Crop(geom1);
                var geom2 = new MagickGeometry(0, 0, wDif, origHeight);
                left.Crop(geom2);

                using (var imageCol = new MagickImageCollection())
                {
                    left.Flop();
                    right.Flop();
                    imageCol.Add(left);
                    imageCol.Add(new MagickImage(orig));
                    imageCol.Add(right);

                    using (var result = imageCol.AppendHorizontally())
                    {
                        var size = new MagickGeometry(Bounds.Width, origHeight);
                        size.IgnoreAspectRatio = true;
                        result.Resize(size);

                        return result.ToByteArray();
                    }
                }
            }
        }


    }
}
