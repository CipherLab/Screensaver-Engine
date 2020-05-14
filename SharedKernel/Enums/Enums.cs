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
        ShowBackground,
        BlurBackground,
        TwitchAndLoad1,
        TwitchAndLoad2,
        RigidBodyMode,
        None

    }
}
