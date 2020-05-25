using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace SharedKernel.Interfaces
{
    public interface ISimpleImageHelper : IDisposable
    {
        byte[] BlankImage(Rectangle box, Color fillColor);

        List<Rectangle> GetSpriteBoundingBoxesInImage(byte[] maskSolid, int i, int i1);

        List<ICroppedImagePart> GetSpritesFromImage(byte[] screenCapturedImage, List<Rectangle> foundObjectsBoxes, bool alpha);

        void SaveByteImageToFile(byte[] backgroundImage, string filePath);

        byte[] EdgeDetectedFilledBlack(byte[] inputImage, ImageFormat outformat);

        byte[] MirrorUpconvertImage(Rectangle rectangle, string imageFile);
    }
}