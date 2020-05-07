using System;
using System.Collections.Generic;
using System.Drawing;
using ScreenSaverHelper.Util;

namespace ScreenSaverHelper
{
    public interface ISimpleImageHelper : IDisposable
    {
        byte[] BlankImage(Rectangle box, Color fillColor);
        byte[] FlattenImages(params byte[][] images);

        List<Rectangle> GetSpriteBoundingBoxesInImage(byte[] maskSolid, int i, int i1);
        List<CroppedImagePart> GetSpritesFromImage(byte[] screenCapturedImage, List<Rectangle> foundObjectsBoxes, bool alpha);
    }
}