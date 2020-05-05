using System;
using System.Collections.Generic;
using ImageMagick;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ScreenSaverHelper.Util;
using Zavolokas.ImageProcessing.Inpainting;
using Zavolokas.Structures;
using Rectangle = System.Drawing.Rectangle;
using Zavolokas.GdiExtensions;
using Zavolokas.ImageProcessing.PatchMatch;


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
                        var size = new MagickGeometry(Bounds.Width, Bounds.Height);
                        size.IgnoreAspectRatio = true;
                        orig.Resize(size);
                        return orig.ToByteArray();
                    }

                    if (origHeight >= Bounds.Height && origWidth < Bounds.Width)
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

                        //new is widened to the bounds now
                        origWidth = Bounds.Width;
                        //pass the niw extra wide one to be mirrored top and bottom
                        var mirroredHImage = MirrorUpAndDown(mirroredWImage.ToArray(), origWidth, origHeight);
                        return mirroredHImage;
                    }

                }
            }

            return GetImageByteArrayFromFile(i);
        }
        public List<Rectangle> UnpackSpriteSheet(byte[] imageData, int distanceBetweenTiles, int padBoxes)
        {
            var edges = EdgeDetector(imageData, Color.White, Color.White);

            using (MemoryStream ms = new MemoryStream(edges))
            {
                using (Image img = Image.FromStream(ms))
                {
                    ImageUnpacker unpacker = new ImageUnpacker(
                        img,
                        imageData, false, distanceBetweenTiles);
                    unpacker.StartUnpacking();

                    //TODO: can do this in update loop, use event to notify or somethin?
                    while (!unpacker.IsUnpacked())
                    {
                        System.Threading.Thread.Sleep(10);
                    }

                    if (unpacker.IsUnpacked())
                    {
                        var boxes = unpacker.GetBoxes();
                        for (var b = 0; b < boxes.Count; b++)
                            boxes[b] = new Rectangle(boxes[b].X, boxes[b].Y, boxes[b].Width + padBoxes, boxes[b].Height + padBoxes);

                        return boxes.ToList();
                      //  return boxes.Where(b => b.Width > 20 && b.Height > 20).ToList();
                    }
                }

            }
            return null;
        }
        public byte[] GetImageByteArrayFromFile(string filename)
        {
            return new MagickImage(filename).ToByteArray();

        }
        public byte[] GetBlurImageByteArrayFromFile(string filename, int factor)
        {
            using (var img = new MagickImage(filename))
            {
                for (int i = 0; i < factor; i++)
                    img.Blur();
                return img.ToByteArray();
            }
        }
        public byte[] GetBlurImageByteArrayFromData(byte[] data, int factor)
        {
            using (var img = new MagickImage(data))
            {
                for (int i = 0; i < factor; i++)
                    img.Blur();
                img.Write(@"c:\temp\canny_blur.png");
                return img.ToByteArray(MagickFormat.Png);
            }
        }
        public byte[] EdgeDetector(byte[] imageData, Color fillColor, Color alphaColor, bool negateFill = false, bool saveFile = false)
        {
            MagickColor fColor = MagickColors.White;
            MagickColor aColor = MagickColors.White;

            if (fillColor == Color.Black)
                fColor = MagickColors.Black;

            if (alphaColor == Color.Black)
                aColor = MagickColors.Black;

            var image = new MagickImage(imageData);
            image.HasAlpha = true;
            image.CannyEdge(1, 0, new Percentage(5), new Percentage(30));
            image.Negate();
            if (fillColor != null)
            {
                image.ColorFuzz = new Percentage(1);
                image.FloodFill(fColor, 0, 0);
              
            }

            if (alphaColor != null)
            {
                image.InverseTransparentChroma(
                    aColor,
                    aColor);
            }
            if (negateFill)
                image.Negate();
            //image.HoughLine(5,5, Int16.MaxValue);
            image.BitDepth(Channels.Default);
            if (saveFile)
                image.Write("c:\\temp\\canny.png");
            return image.ToByteArray(MagickFormat.Png);
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
                    imageCol.Add(top);
                    imageCol.Add(new MagickImage(orig.ToArray()));
                    bottom.Flip();
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


        public List<CroppedImagePart> GetSpritesFromImage(List<Rectangle> boxes, byte[] image, bool makeTransparent)
        {
            return boxes.Select(r => GetSpriteFromImage(r, image, makeTransparent)).ToList();
        }
        public CroppedImagePart GetSpriteFromImage(Rectangle box, byte[] image, bool makeTransparent)
        {
            Image original;
            using (MemoryStream ms = new MemoryStream(image))
                original = Image.FromStream(ms);

            Bitmap bitmap = new Bitmap(box.Width, box.Height, original.PixelFormat);

            using (Graphics objGraphics = Graphics.FromImage(bitmap))
            {
                objGraphics.DrawImage(original, new Rectangle(0, 0, bitmap.Width, bitmap.Height), box, GraphicsUnit.Pixel);
                objGraphics.Dispose();
            }

            using (var stream = new MemoryStream())
            {
                if (makeTransparent)
                {

                    int m = 15;
                    bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    stream.Seek(0, SeekOrigin.Begin);
                    MagickColor low1;
                    MagickColor low2;
                    MagickColor low3;
                    MagickColor low4;
                    MagickColor high1;
                    MagickColor high2;
                    MagickColor high3;
                    MagickColor high4;
                    using (MagickImage orig = new MagickImage(stream))
                    {
                        using (var highImg = orig.Clone())
                        {
                            highImg.BrightnessContrast(new Percentage(m), new Percentage(0));
                            high1 = highImg.GetPixels().GetPixel(1, 1).ToColor();
                            high2 = highImg.GetPixels().GetPixel(bitmap.Width - 1, bitmap.Height - 1).ToColor();
                            high3 = highImg.GetPixels().GetPixel(bitmap.Width - 1, 1).ToColor();
                            high4 = highImg.GetPixels().GetPixel(1, bitmap.Height - 1).ToColor();
                        }

                        using (var lwoImg = orig.Clone())
                        {
                            lwoImg.BrightnessContrast(new Percentage(m * -1), new Percentage(0));
                            low1 = lwoImg.GetPixels().GetPixel(1, 1).ToColor();
                            low2 = lwoImg.GetPixels().GetPixel(bitmap.Width - 1, bitmap.Height - 1).ToColor();
                            low3 = lwoImg.GetPixels().GetPixel(bitmap.Width - 1, 1).ToColor();
                            low4 = lwoImg.GetPixels().GetPixel(1, bitmap.Height - 1).ToColor();
                        }

                        orig.HasAlpha = true;
                        orig.ColorFuzz = new Percentage(10);
                        orig.TransparentChroma(low1, high1);
                        orig.TransparentChroma(low2, high2);
                        orig.TransparentChroma(low3, high3);
                        orig.TransparentChroma(low4, high4);
                        return new CroppedImagePart
                        {
                            ImageData = orig.ToByteArray(MagickFormat.Png),
                            ImageProperties = box
                        };
                    }


                }
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                stream.Seek(0, SeekOrigin.Begin);

                return new CroppedImagePart
                {
                    ImageData = stream.ToArray(),
                    ImageProperties = box
                };
            }
        }

        private bool PixelInColorRange(byte rgb, byte min, byte max)
        {
            bool replace =
                rgb != min // green is not the smallest value
                && (rgb == max // green is the biggest value
                    || max - rgb < 8) // or at least almost the biggest value
                && (max - min) > 96; // minimum difference between smallest/biggest value (avoid grays)
            return replace;
        }

        public byte[] BlankImage(Rectangle box, Color fillColor)
        {
            using (var blankImage = new System.Drawing.Bitmap(box.Width, box.Height))
            {
                var memStream = new MemoryStream();
                blankImage.Save(memStream, ImageFormat.Jpeg);

                using (Graphics g = Graphics.FromImage(blankImage))
                {
                    SolidBrush shadowBrush = new SolidBrush(fillColor);
                    g.FillRectangles(shadowBrush, new RectangleF[] { box });
                }
                using (MemoryStream sout = new MemoryStream())
                {
                    blankImage.Save(sout, ImageFormat.Png);
                    return sout.ToArray();
                }
            }
        }
        public byte[] ImageMaskFromSprites(List<Rectangle> boxes, byte[] image)
        {
            MagickImage original = new MagickImage(image);

            using (var blankImage = new System.Drawing.Bitmap(original.Width, original.Height))
            {
                var memStream = new MemoryStream();
                blankImage.Save(memStream, ImageFormat.Jpeg);

                foreach (var b in boxes)
                {
                    using (Graphics g = Graphics.FromImage(blankImage))
                    {
                        SolidBrush shadowBrush = new SolidBrush(Color.Red);
                        g.FillRectangles(shadowBrush, new RectangleF[] { b });
                    }
                }
                blankImage.Save(@"C:\temp\redblocks.png", ImageFormat.Png);
                using (MemoryStream sout = new MemoryStream())
                {
                    blankImage.Save(sout, ImageFormat.Png);
                    return sout.ToArray();
                }
            }
        }
        public byte[] ContentAwareFillFromSpriteImageMask(List<Rectangle> boxes, byte[] image)
        {
            var partsToFillImage = ImageMaskFromSprites(boxes, image);

            var inpainter = new Inpainter();
            var origImg = OpenArgbImage(image);
            var partsImg = OpenArgbImage(partsToFillImage);

            var result = inpainter.Inpaint(origImg, partsImg, new InpaintSettings
            {
                ChangedPixelsPercentTreshold = 0.005,
                MaxInpaintIterations = 1,
                PatchDistanceCalculator = ImagePatchDistance.Cie76,
                IgnoreInpaintedPixelsOnFirstIteration = false,
            });



            using (MemoryStream sout = new MemoryStream())
            {
                result.FromArgbToBitmap().Save(sout, ImageFormat.Jpeg);
                return sout.ToArray();
            }
        }

        private ZsImage OpenArgbImage(byte[] orig)
        {
            const double maxSize = 4000;
            using (MemoryStream ms = new MemoryStream(orig))
            {
                var imageBitmap = new Bitmap(ms);
                if (imageBitmap.Width > maxSize || imageBitmap.Height > maxSize)
                {
                    var tmp = imageBitmap;
                    double percent = imageBitmap.Width > imageBitmap.Height
                        ? maxSize / imageBitmap.Width
                        : maxSize / imageBitmap.Height;
                    imageBitmap = imageBitmap.CloneWithScaleTo((int)(imageBitmap.Width * percent),
                        (int)(imageBitmap.Height * percent));
                    tmp.Dispose();
                }

                var image = imageBitmap.ToArgbImage();
                imageBitmap.Dispose();
                return image;
            }
        }
    }
}
