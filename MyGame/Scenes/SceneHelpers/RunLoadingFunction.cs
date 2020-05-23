using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Nez;
using SharedKernel.Interfaces;

namespace ScreenSaverEngine2.Scenes
{
    public class RunLoadingFunction
    {
        private readonly int _distanceBetweenTiles;

        private readonly byte[] _imageData;
        private readonly List<Rectangle> _boundingBoxes;

        private readonly int _padBoxes;

        //IEnumerable<ICroppedImagePart> DetectedObjectImages
        public RunLoadingFunction(SceneHelpers.DelegateDeclarationBool functionToRun, int minDuration, bool loadOnBackgroundThread)
        {
            MinDuration = minDuration;
            BoolReturnFunctionToRun = functionToRun;
            LoadOnBackgroundThread = loadOnBackgroundThread;
        }

        public RunLoadingFunction(SceneHelpers.DelegateDeclarationImageData functionToRun, int minDuration, bool loadOnBackgroundThread)
        {
            MinDuration = minDuration;
            ImageDataReturnFunctionToRun = functionToRun;
            LoadOnBackgroundThread = loadOnBackgroundThread;
        }

        public RunLoadingFunction(SceneHelpers.DelegateDeclarationListRectangle functionToRun, int minDuration, bool loadOnBackgroundThread, byte[] imageData, int distanceBetweenTiles, int padBoxes)
        {
            MinDuration = minDuration;
            _imageData = imageData;
            _distanceBetweenTiles = distanceBetweenTiles;
            _padBoxes = padBoxes;
            ListRectangleReturnFunctionToRun = functionToRun;
            LoadOnBackgroundThread = loadOnBackgroundThread;
        }

        public RunLoadingFunction(SceneHelpers.DelegateDeclarationListCroppedImageParts functionToRun, int minDuration, bool loadOnBackgroundThread, byte[] imageData, List<Rectangle> boundingBoxes)
        {
            MinDuration = minDuration;
            _imageData = imageData;
            _boundingBoxes = boundingBoxes;
            ListCroppedImagePartsReturnFunctionToRun = functionToRun;
            LoadOnBackgroundThread = loadOnBackgroundThread;
        }

        public event EventHandler<JobCompleteEventArgs> JobComplete;

        public SceneHelpers.DelegateDeclarationListCroppedImageParts ListCroppedImagePartsReturnFunctionToRun { get; set; }
        public SceneHelpers.DelegateDeclarationListRectangle ListRectangleReturnFunctionToRun { get; set; }
        public int MinDuration { get; set; }

        private SceneHelpers.DelegateDeclarationBool BoolReturnFunctionToRun { get; }

        private SceneHelpers.DelegateDeclarationImageData ImageDataReturnFunctionToRun { get; }

        //private bool ReadyToExecute { get; set; }
        private bool IsFunctionDone { get; set; }

        private bool LoadOnBackgroundThread { get; set; }

        public IEnumerator RunUntilDurationAndDone()
        {
            yield return null;

            // load up the new Scene
            if (BoolReturnFunctionToRun != null)
                yield return Core.StartCoroutine(RunFunction(BoolReturnFunctionToRun));
            if (ImageDataReturnFunctionToRun != null)
                yield return Core.StartCoroutine(RunFunction(ImageDataReturnFunctionToRun));
            if (ListRectangleReturnFunctionToRun != null)
                yield return Core.StartCoroutine(RunFunction(ListRectangleReturnFunctionToRun));
            if (ListCroppedImagePartsReturnFunctionToRun != null)
                yield return Core.StartCoroutine(RunFunction(ListCroppedImagePartsReturnFunctionToRun));

            var elapsed = 0f;
            while (MinDuration < elapsed && !IsFunctionDone)
            {
                elapsed += Time.DeltaTime;
                yield return null;
            }

            //TransitionComplete();
        }

        protected virtual void OnJobComplete(JobCompleteEventArgs e)
        {
            if (JobComplete != null)
            {
                JobComplete(this, e);
            }
        }

        protected IEnumerator RunFunction(SceneHelpers.DelegateDeclarationBool thingToRun)
        {
            if (LoadOnBackgroundThread)
            {
                // load the Scene on a background thread
                Task.Run(() =>
                {
                    var done = thingToRun();
                    Core.Schedule(0, false, null, timer =>
                    {
                        //Core.Scene = scene;
                        IsFunctionDone = done;
                    });
                });
            }
            else
            {
                IsFunctionDone = thingToRun(); ;
            }

            // wait if it was loaded on a background thread
            while (!IsFunctionDone)
                yield return null;

            OnJobComplete(new JobCompleteEventArgs(true));
        }

        protected IEnumerator RunFunction(SceneHelpers.DelegateDeclarationListRectangle thingToRun)
        {
            List<Rectangle> boxData = new List<Rectangle>();
            if (LoadOnBackgroundThread)
            {
                Task.Run(() =>
                {
                    boxData = thingToRun(_imageData, _distanceBetweenTiles, _padBoxes);
                    Core.Schedule(0, false, null, timer =>
                    {
                        //Core.Scene = scene;
                        IsFunctionDone = true;
                    });
                });
            }
            else
            {
                boxData = thingToRun(_imageData, _distanceBetweenTiles, _padBoxes);
                IsFunctionDone = true;
            }
            while (!IsFunctionDone)
                yield return null;

            OnJobComplete(new JobCompleteEventArgs(true, boxData));
        }

        protected IEnumerator RunFunction(SceneHelpers.DelegateDeclarationListCroppedImageParts thingToRun)
        {
            IEnumerable<ICroppedImagePart> boxData = new List<ICroppedImagePart>();
            if (LoadOnBackgroundThread)
            {
                Task.Run(() =>
                {
                    boxData = thingToRun(_imageData, _boundingBoxes, true);
                    Core.Schedule(0, false, null, timer =>
                    {
                        //Core.Scene = scene;
                        IsFunctionDone = true;
                    });
                });
            }
            else
            {
                boxData = thingToRun(_imageData, _boundingBoxes, true);
                IsFunctionDone = true;
            }
            while (!IsFunctionDone)
                yield return null;

            OnJobComplete(new JobCompleteEventArgs(true, boxData));
        }

        protected IEnumerator RunFunction(SceneHelpers.DelegateDeclarationImageData thingToRun)
        {
            byte[] imageData = null;
            if (LoadOnBackgroundThread)
            {
                Task.Run(() =>
                {
                    imageData = thingToRun();

                    Core.Schedule(0, false, null, timer =>
                    {
                        //Core.Scene = scene;
                        IsFunctionDone = true;
                    });
                });
            }
            else
            {
                imageData = thingToRun();
                IsFunctionDone = true;
            }

            while (!IsFunctionDone)
                yield return null;

            OnJobComplete(new JobCompleteEventArgs(true, imageData));
        }
    }
}