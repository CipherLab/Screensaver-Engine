using System;

namespace ScreenSaverEngine2.Scenes
{
    public class JobCompleteEventArgs : EventArgs
    {
        public JobCompleteEventArgs(bool b, byte[] edgeDetectedImageData)
        {
            Complete = b;
            EdgeDetectedImageData = edgeDetectedImageData;
        }

        public bool Complete { get; set; }
        public byte[] EdgeDetectedImageData { get; set; }
    }
}