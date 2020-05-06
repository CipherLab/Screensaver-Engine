using System;
using System.Drawing;

namespace ScreenSaverHelper
{
    public interface ISimpleImageHelper : IDisposable
    {
        byte[] BlankImage(Rectangle box, Color fillColor);
        byte[] FlattenImages(params byte[][] images);
    }
}