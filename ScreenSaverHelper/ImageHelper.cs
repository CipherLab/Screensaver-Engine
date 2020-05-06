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
    public class ImageHelper : IDisposable, ISimpleImageHelper
    {
        private Rectangle Bounds { get; }

        private IMagickImage OriginalImage { get; }

        private string ImageFile { get; }
        public ImageHelper(Rectangle bounds, string imageFile)
        {
            Bounds = bounds;
            ImageFile = imageFile;
            OriginalImage = new MagickImage(imageFile);
        }
        public ImageHelper(byte[] imageData)
        {
            OriginalImage = new MagickImage(imageData);
        }
        public ImageHelper()
        {
        }
        public byte[] MirrorUpconvertImage()
        {
            int origWidth = OriginalImage.Width;
            int origHeight = OriginalImage.Height;
            string i = ImageFile;
            if (origHeight >= Bounds.Height && origWidth >= Bounds.Width)
            {

                Debug.WriteLine($"{new FileInfo(i).Name} - Original image is big enough");
                var size = new MagickGeometry(Bounds.Width, Bounds.Height);
                size.IgnoreAspectRatio = true;
                OriginalImage.Resize(size);
                return OriginalImage.ToByteArray();
            }

            if (origHeight >= Bounds.Height && origWidth < Bounds.Width)
            {
                Debug.WriteLine($"{new FileInfo(i).Name} - Not wide enough");
                var mirroredWImage = MirrorLeftAndRight();
                return mirroredWImage;
            }

            if (origWidth >= Bounds.Width && origHeight < Bounds.Height)
            {
                Debug.WriteLine($"{new FileInfo(i).Name} - Not tall enough");
                var mirroredHImage = MirrorUpAndDown();
                return mirroredHImage;
            }

            if (origHeight < Bounds.Height && origWidth < Bounds.Width)
            {
                Debug.WriteLine($"{new FileInfo(i).Name} - Not tall or wide enough");
                var mirroredWImage = MirrorLeftAndRight();

                //new is widened to the bounds now
                origWidth = Bounds.Width;
                //pass the niw extra wide one to be mirrored top and bottom
                var mirroredHImage = MirrorUpAndDown();
                return mirroredHImage;
            }


            return GetImageByteArrayFromFile(i);
        }
        /// <summary>
        /// Creates the edge filled object black with no transparency
        /// </summary>
        /// <param name="distanceBetweenTiles"></param>
        /// <param name="padBoxes"></param>
        /// <returns></returns>
        public List<Rectangle> GetSpriteBoundingBoxesInImage(int distanceBetweenTiles, int padBoxes)
        {
            //var edges = EdgeDetectedFilledBlack(ImageFormat.Jpeg);

            using (MemoryStream ms = new MemoryStream(OriginalImage.ToByteArray()))
            {
                using (Image img = Image.FromStream(ms))
                {
                    ImageUnpacker unpacker = new ImageUnpacker(
                        img,
                        OriginalImage.ToByteArray(), false, distanceBetweenTiles);
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
        public byte[] GetBlurImageByteArrayFromData(int factor)
        {
            using (var img = OriginalImage.Clone())
            {
                for (int i = 0; i < factor; i++)
                    img.Blur();
                img.Write(@"c:\temp\canny_blur.png");
                return img.ToByteArray(MagickFormat.Png);
            }
        }

        public byte[] EdgeDetectedFilledBlack(ImageFormat format)
        {
            using (IMagickImage image = new MagickImage(OriginalImage.Clone()))
            {
                image.ColorFuzz = new Percentage(1);
                image.HasAlpha = true;

                //White lines on black
                var sigma = 8;

                var lowerP = 3;
                var upperP = 20;
                image.CannyEdge(1, sigma, new Percentage(lowerP), new Percentage(upperP));
                //black lines on white
                image.Negate();
                //black lines on transparent
                image.InverseTransparentChroma(MagickColors.Black, MagickColors.Black);

                using (var imageCol = new MagickImageCollection())
                {
                    //beef up the lines
                    IMagickImage clone2;
                    IMagickImage clone3;
                    IMagickImage clone4;
                    using (var clone1 = image.Clone())
                    {
                        clone2 = image.Clone();
                        clone3 = image.Clone();
                        clone4 = image.Clone();

                        var size = new MagickGeometry(0, 1, OriginalImage.Width, Bounds.Height + 1);
                        size.IgnoreAspectRatio = true;
                        clone1.Resize(size);
                        size = new MagickGeometry(1, 0, OriginalImage.Width + 1, Bounds.Height);
                        clone2.Resize(size);
                        size = new MagickGeometry(0, -1, OriginalImage.Width, Bounds.Height - 1);
                        clone3.Resize(size);
                        size = new MagickGeometry(-1, 0, OriginalImage.Width - 1, Bounds.Height);
                        clone4.Resize(size);


                        imageCol.Add(clone1);
                        imageCol.Add(clone2);
                        imageCol.Add(clone3);
                        imageCol.Add(clone4);
                        using (var result = imageCol.Flatten(MagickColors.Transparent))
                        {
                            //white with transparent bg.
                            result.FloodFill(MagickColors.Black, 0, 0);

                            //transparent bg with white fill
                            result.Negate();

                            //white fill to black, transparent bg!
                            result.Negate((Channels.RGB));

                            result.Write($"c:\\temp\\Flatten-S{sigma}-{lowerP}-{upperP}.png");
                            if (format == ImageFormat.Png)
                                return result.ToByteArray(MagickFormat.Png);

                            if (format == ImageFormat.Jpeg)
                            {
                                result.Negate((Channels.Alpha));
                                result.Write($"c:\\temp\\Flatten-S{sigma}-{lowerP}-{upperP}.Png");
                                return result.ToByteArray(MagickFormat.Png);
                            }

                            throw new FormatException($"{format} not accounted for");
                        }
                    }
                }

            }
        }

        private byte[] MirrorUpAndDown()
        {
            using (var top = OriginalImage.Clone())
            {
                using (var bottom = OriginalImage.Clone())
                {
                    var hDif = (Bounds.Height - OriginalImage.Height) / 2;

                    var geom1 = new MagickGeometry(0, 0, OriginalImage.Width, hDif);
                    top.Crop(geom1);
                    var geom2 = new MagickGeometry(0, OriginalImage.Height - hDif, OriginalImage.Width, hDif);
                    bottom.Crop(geom2);

                    using (var imageCol = new MagickImageCollection())
                    {
                        top.Flip();
                        imageCol.Add(top);
                        imageCol.Add(OriginalImage.Clone());
                        bottom.Flip();
                        imageCol.Add(bottom);

                        using (var result = imageCol.AppendVertically())
                        {
                            var size = new MagickGeometry(OriginalImage.Width, Bounds.Height);
                            size.IgnoreAspectRatio = true;
                            result.Resize(size);

                            return result.ToByteArray();
                        }
                    }
                }
            }
        }

        private byte[] MirrorLeftAndRight()
        {
            using (var left = OriginalImage.Clone())
            {
                using (var right = OriginalImage.Clone())
                {
                    var wDif = (Bounds.Width - OriginalImage.Width) / 2;

                    var geom1 = new MagickGeometry(OriginalImage.Width - wDif, 0, wDif, OriginalImage.Height);
                    right.Crop(geom1);
                    var geom2 = new MagickGeometry(0, 0, wDif, OriginalImage.Height);
                    left.Crop(geom2);

                    using (var imageCol = new MagickImageCollection())
                    {
                        left.Flop();
                        right.Flop();
                        imageCol.Add(left);
                        imageCol.Add(OriginalImage.Clone());
                        imageCol.Add(right);

                        using (var result = imageCol.AppendHorizontally())
                        {
                            var size = new MagickGeometry(Bounds.Width, OriginalImage.Height);
                            size.IgnoreAspectRatio = true;
                            result.Resize(size);

                            return result.ToByteArray();
                        }
                    }
                }
            }
        }


        public List<CroppedImagePart> GetSpritesFromImage(List<Rectangle> boxes, bool makeTransparent)
        {
            return boxes.Select(r => GetSpriteFromImage(r, makeTransparent)).ToList();
        }
        public CroppedImagePart GetSpriteFromImage(Rectangle box, bool makeTransparent)
        {
            Image original;
            using (MemoryStream ms = new MemoryStream(OriginalImage.ToByteArray()))
                original = Image.FromStream(ms);

            using (Bitmap bitmap = new Bitmap(box.Width, box.Height, original.PixelFormat))
            {
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
                    bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    stream.Seek(0, SeekOrigin.Begin);

                    return new CroppedImagePart
                    {
                        ImageData = stream.ToArray(),
                        ImageProperties = box
                    };
                }
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

        public byte[] FlattenImages(params byte[][] images)
        {
            using (var imageCol = new MagickImageCollection())
            {
                foreach (var image in images)
                {
                    using (IMagickImage im = new MagickImage(image))
                    {
                        imageCol.Add(im.Clone());
                    }
                }

                using (var result = new MagickImage(imageCol.Flatten()))
                {
                    return result.ToByteArray();
                }
            }
        }

        public byte[] ImageMaskFromSprites(List<Rectangle> boxes)
        {


            using (var blankImage = new System.Drawing.Bitmap(OriginalImage.Width, OriginalImage.Height))
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
        public byte[] ContentAwareFillFromSpriteImageMask(List<Rectangle> boxes)
        {
            var partsToFillImage = ImageMaskFromSprites(boxes);

            var inpainter = new Inpainter();
            var origImg = OpenArgbImage(OriginalImage.ToByteArray());
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

        public void Dispose()
        {
            if (OriginalImage != null)
                OriginalImage.Dispose();
        }
    }
}
