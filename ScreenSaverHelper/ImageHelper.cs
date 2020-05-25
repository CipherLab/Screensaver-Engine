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
using SharedKernel.Interfaces;
using Zavolokas.ImageProcessing.Inpainting;
using Zavolokas.Structures;
using Rectangle = System.Drawing.Rectangle;
using Zavolokas.GdiExtensions;
using Zavolokas.ImageProcessing.PatchMatch;

namespace ScreenSaverHelper
{
    public class ImageHelper : IDisposable, ISimpleImageHelper, IHeroSpriteImageHelper
    {
        public List<Rectangle> SpriteLocations { get; }
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

        public ImageHelper(Rectangle bounds, byte[] imageData)
        {
            Bounds = bounds;
            OriginalImage = new MagickImage(imageData, MagickFormat.Jpg);
        }

        public ImageHelper()
        {
        }

        public ImageHelper(string file)
        {
            OriginalImage = new MagickImage(file);
            Bounds = new Rectangle(0, 0, OriginalImage.Width, OriginalImage.Height);
        }

        public byte[] GetFlattenedSprite()
        {
            using (var imageCol = new MagickImageCollection())
            {
                int idx = 0;
                foreach (var s in SpriteLocations)
                {
                }

                return imageCol.Flatten(MagickColors.Transparent).ToByteArray(MagickFormat.Png);
            }
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

            return GetSpriteBoundingBoxesInImage(OriginalImage.ToByteArray(), distanceBetweenTiles, padBoxes);
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

        public byte[] ChangeBrightness(int factor)
        {
            using (var img = OriginalImage.Clone())
            {
                img.BrightnessContrast(new Percentage(factor), new Percentage(0));
                for (int i = 0; i < factor; i++)
                    img.Blur();
                img.Write(@"c:\temp\canny_blur.png");
                return img.ToByteArray(MagickFormat.Png);
            }
        }

        public byte[] EdgeDetectedFilledBlack(byte[] inputImage, ImageFormat format)
        {
            using (IMagickImage image = new MagickImage(inputImage))
            {
                image.ColorFuzz = new Percentage(1);
                image.HasAlpha = true;

                //White lines on black
                var sigma = 6;

                var lowerP = 3;
                var upperP = 20;
                image.CannyEdge(1, sigma, new Percentage(lowerP), new Percentage(upperP));
                //black lines on white
                image.Negate();
                //black lines on transparent
                image.InverseTransparentChroma(MagickColors.Black, MagickColors.Black);
                //image.Write($"c:\\temp\\InverseTransparentChroma-S{sigma}-{lowerP}-{upperP}.png");

                image.FloodFill(MagickColors.Black, 1, 1);
                image.FloodFill(MagickColors.Black, 1, image.Height - 1);
                image.FloodFill(MagickColors.Black, image.Width - 1, 1);
                image.FloodFill(MagickColors.Black, image.Width - 1, image.Height - 1);
                //image.Write($"c:\\temp\\Flatten0-S{sigma}-{lowerP}-{upperP}.png");
                for (int i = 1; i < 4; i++)
                {
                    image.FloodFill(MagickColors.Black, i, image.Height / i - 1);
                    image.FloodFill(MagickColors.Black, image.Width / i - 1, i);
                    image.FloodFill(MagickColors.Black, image.Width / i - 1, image.Height / i - 1);
                }
                //image.Write($"c:\\temp\\Flatten1-S{sigma}-{lowerP}-{upperP}.png");
                if (format == ImageFormat.Png)
                    return image.ToByteArray(MagickFormat.Png);

                if (format == ImageFormat.Jpeg)
                {
                    //image.Write($"c:\\temp\\Flatten2-S{sigma}-{lowerP}-{upperP}.png");
                    image.ColorAlpha(MagickColors.White);
                    //image.Write($"c:\\temp\\Flatten3-S{sigma}-{lowerP}-{upperP}.png");
                    return image.ToByteArray(MagickFormat.Png);
                }
                throw new FormatException($"{format} not accounted for");
            }
        }

        public byte[] MirrorUpconvertImage(Rectangle bounds, string imageFile)
        {
            using (var originalImage = new MagickImage(imageFile))
            {
                int origWidth = originalImage.Width;
                int origHeight = originalImage.Height;
                string i = imageFile;
                if (origHeight >= bounds.Height && origWidth >= bounds.Width)
                {
                    Debug.WriteLine($"{new FileInfo(i).Name} - Original image is big enough");
                    var size = new MagickGeometry(bounds.Width, bounds.Height);
                    size.IgnoreAspectRatio = true;
                    originalImage.Resize(size);
                    return originalImage.ToByteArray();
                }

                if (origHeight >= bounds.Height && origWidth < bounds.Width)
                {
                    Debug.WriteLine($"{new FileInfo(i).Name} - Not wide enough");
                    var mirroredWImage = MirrorLeftAndRight(bounds, originalImage.ToByteArray(MagickFormat.Jpg));
                    return mirroredWImage;
                }

                if (origWidth >= bounds.Width && origHeight < bounds.Height)
                {
                    Debug.WriteLine($"{new FileInfo(i).Name} - Not tall enough");
                    var mirroredHImage = MirrorUpAndDown(bounds, originalImage.ToByteArray(MagickFormat.Jpg));
                    return mirroredHImage;
                }

                if (origHeight < bounds.Height && origWidth < bounds.Width)
                {
                    Debug.WriteLine($"{new FileInfo(i).Name} - Not tall or wide enough");
                    var mirroredWImage = MirrorLeftAndRight(bounds, originalImage.ToByteArray(MagickFormat.Jpg));

                    //pass the niw extra wide one to be mirrored top and bottom
                    var mirroredHWImage = MirrorUpAndDown(bounds, mirroredWImage);
                    return mirroredHWImage;
                }

                return GetImageByteArrayFromFile(i);
            }
        }

        public byte[] MirrorUpconvertImage()
        {
            return MirrorUpconvertImage(this.Bounds, this.ImageFile);
        }

        public byte[] EdgeDetectedFilledBlack(ImageFormat format)
        {
            var fmt = MagickFormat.Png;
            if (format == ImageFormat.Jpeg)
                fmt = MagickFormat.Jpg;
            return EdgeDetectedFilledBlack(this.OriginalImage.ToByteArray(fmt), format);
        }

        private byte[] MirrorUpAndDown(Rectangle bounds, byte[] img)
        {
            using (var original = new MagickImage(img))
            {
                using (var top = original.Clone())
                {
                    using (var bottom = original.Clone())
                    {
                        var hDif = (bounds.Height - original.Height) / 2;

                        var geom1 = new MagickGeometry(0, 0, original.Width, hDif);
                        top.Crop(geom1);
                        var geom2 = new MagickGeometry(0, original.Height - hDif, original.Width, hDif);
                        bottom.Crop(geom2);

                        using (var imageCol = new MagickImageCollection())
                        {
                            top.Flip();
                            imageCol.Add(top);
                            imageCol.Add(original);
                            bottom.Flip();
                            imageCol.Add(bottom);

                            using (var result = imageCol.AppendVertically())
                            {
                                var size = new MagickGeometry(original.Width, bounds.Height);
                                size.IgnoreAspectRatio = true;
                                result.Resize(size);

                                return result.ToByteArray();
                            }
                        }
                    }
                }
            }
        }

        private byte[] MirrorUpAndDown()
        {
            return MirrorUpAndDown(Bounds, OriginalImage.ToByteArray());
        }

        private byte[] MirrorLeftAndRight()
        {
            return MirrorLeftAndRight(Bounds, OriginalImage.ToByteArray());
        }

        private byte[] MirrorLeftAndRight(Rectangle bounds, byte[] img)
        {
            using (var original = new MagickImage(img))
            {
                using (var left = original.Clone())
                {
                    using (var right = original.Clone())
                    {
                        var wDif = (bounds.Width - original.Width) / 2;

                        var geom1 = new MagickGeometry(original.Width - wDif, 0, wDif, original.Height);
                        right.Crop(geom1);
                        var geom2 = new MagickGeometry(0, 0, wDif, original.Height);
                        left.Crop(geom2);

                        using (var imageCol = new MagickImageCollection())
                        {
                            left.Flop();
                            right.Flop();
                            imageCol.Add(left);
                            imageCol.Add(original.Clone());
                            imageCol.Add(right);

                            using (var result = imageCol.AppendHorizontally())
                            {
                                var size = new MagickGeometry(bounds.Width, original.Height);
                                size.IgnoreAspectRatio = true;
                                result.Resize(size);

                                return result.ToByteArray();
                            }
                        }
                    }
                }
            }
        }

        public IEnumerable<ICroppedImagePart> GetSpritesFromImage(List<Rectangle> boxes, bool makeTransparent)
        {
            return boxes.Select(r => GetSpriteFromImage(OriginalImage.ToByteArray(), r, false, makeTransparent)).ToList();
        }

        public ICroppedImagePart GetSpriteFromImage(byte[] image, Rectangle box, bool flipH, bool makeTransparent)
        {
            Image original;
            using (MemoryStream ms = new MemoryStream(image))
                original = Image.FromStream(ms);

            Image bitmap;
            using (MemoryStream ms = new MemoryStream(BlankImage(box, Color.Transparent)))
                bitmap = Image.FromStream(ms);

            using (bitmap)
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

                    using (MagickImage tempImage = new MagickImage(stream))
                    {
                        if (flipH)
                            tempImage.Flop();
                        return new CroppedImagePart
                        {
                            ImageData = tempImage.ToByteArray(MagickFormat.Png),
                            ImageProperties = box
                        };
                    }
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

        public List<Rectangle> GetSpriteBoundingBoxesInImage(byte[] image, int distanceBetweenTiles, int padBoxes)
        {
            using (Image img = Image.FromStream(new MemoryStream(image)))
            {
                ImageUnpacker unpacker = new ImageUnpacker(
                    img,
                    image, false, distanceBetweenTiles);
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

            return null;
        }

        public List<ICroppedImagePart> GetSpritesFromImage(byte[] screenCapturedImage, List<Rectangle> foundObjectsBoxes, bool alpha)
        {
            return foundObjectsBoxes.Select(r => GetSpriteFromImage(screenCapturedImage, r, false, alpha)).ToList();
        }

        public void SaveByteImageToFile(byte[] backgroundImage, string cTempScreengrabJpg)
        {
            using (IMagickImage img = new MagickImage(backgroundImage))
            {
                img.Write(cTempScreengrabJpg);
            }
        }

        public ICroppedImagePart GetSpriteFromImage(Rectangle box, bool flipH, bool alpha)
        {
            return GetSpriteFromImage(OriginalImage.ToByteArray(), box, flipH, alpha);
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