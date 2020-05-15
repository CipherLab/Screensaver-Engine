using System.Collections.Generic;
using System.Drawing;
using SharedKernel.Interfaces;

namespace ScreenSaverEngine2.SceneHelpers
{
    public delegate bool DelegateDeclarationBool();

    public delegate byte[] DelegateDeclarationImageData();

    public delegate List<Rectangle> DelegateDeclarationListRectangle(byte[] imageData, int distanceBetweenTiles, int padBoxes);

    public delegate IEnumerable<ICroppedImagePart> DelegateDeclarationListCroppedImageParts(byte[] imageData, List<Rectangle> boxData, bool alpha);
}