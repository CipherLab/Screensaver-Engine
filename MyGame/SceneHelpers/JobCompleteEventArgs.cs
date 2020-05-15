using System;
using System.Collections.Generic;
using System.Drawing;
using SharedKernel.Interfaces;

namespace ScreenSaverEngine2.SceneHelpers
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
}