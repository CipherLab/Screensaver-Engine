using System;
using System.Collections.Generic;
using System.Drawing;
using ScreenSaverHelper.Util;
using SharedKernel.Interfaces;

namespace ScreenSaverHelper
{
    public interface IHeroSpriteImageHelper : IDisposable
    {
        IEnumerable<ICroppedImagePart> GetSpritesFromImage(List<Rectangle> boxes, bool makeTransparent);
        ICroppedImagePart GetSpriteFromImage(Rectangle box, bool flipH, bool alpha);

    }
}