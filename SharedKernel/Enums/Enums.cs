using System;
using System.Collections.Generic;
using System.Text;

namespace SharedKernel.Enums
{
    public enum ScreenEdgeSide
    {
        Left,
        Right,
        Top,
        Bottom
    }

    public enum Phase
    {
        GetImage,
        GotImage,
        FadeIn,
        FadeOut,
        ShowImage
    }
}