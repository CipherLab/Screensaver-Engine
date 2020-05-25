using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Xna.Framework.Graphics;
using SharedKernel.Enums;
using SharedKernel.Interfaces;

namespace ScreenSaverEngine2.Scenes.SceneHelpers
{
    public class JobCompleteEventArgs : EventArgs
    {
        public JobCompleteEventArgs(bool b)
        {
            Complete = b;
        }

        public JobCompleteEventArgs(bool b, byte[] edgeDetectedImageData)
        {
            Complete = b;
            EdgeDetectedImageData = edgeDetectedImageData;
        }

        public JobCompleteEventArgs(bool b, List<Rectangle> boxData)
        {
            Complete = b;
            BoxData = boxData;
        }

        public JobCompleteEventArgs(bool b, IEnumerable<ICroppedImagePart> croppedImageParts)
        {
            Complete = b;
            CroppedImageParts = croppedImageParts;
        }

        public bool Complete { get; set; }
        public IEnumerable<ICroppedImagePart> CroppedImageParts { get; set; }
        public byte[] EdgeDetectedImageData { get; set; }
        public List<Rectangle> BoxData { get; set; }
    }

    public class TilingSaverPhaseChangeEventArgs : EventArgs
    {
        public TilingSaverPhaseChangeEventArgs(Phase currentPhase, Texture2D newTiledImage)
        {
            CurrentPhase = currentPhase;
            NewTiledImage = newTiledImage;
        }

        public TilingSaverPhaseChangeEventArgs(Phase currentPhase)
        {
            CurrentPhase = currentPhase;
        }

        public Phase CurrentPhase { get; set; }

        public Texture2D NewTiledImage { get; set; }
    }
}