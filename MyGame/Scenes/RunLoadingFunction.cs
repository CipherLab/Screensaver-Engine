using System;
using System.Collections;
using System.Threading.Tasks;
using Nez;

namespace ScreenSaverEngine2.Scenes
{
    public class RunLoadingFunction
    {
        public RunLoadingFunction(DelegateDeclarationBool functionToRun, int minDuration, bool loadOnBackgroundThread)
        {
            MinDuration = minDuration;
            BoolReturnFunctionToRun = functionToRun;
            LoadOnBackgroundThread = loadOnBackgroundThread;
        }
        public RunLoadingFunction(DelegateDeclarationImageData functionToRun, int minDuration, bool loadOnBackgroundThread)
        {
            MinDuration = minDuration;
            ImageDataReturnFunctionToRun = functionToRun;
            LoadOnBackgroundThread = loadOnBackgroundThread;
        }
        public IEnumerator RunUntilDurationAndDone()
        {
            yield return null;

            // load up the new Scene
            if (BoolReturnFunctionToRun != null)
                yield return Core.StartCoroutine(RunFunction(BoolReturnFunctionToRun));
            if (ImageDataReturnFunctionToRun != null)
                yield return Core.StartCoroutine(RunFunction(ImageDataReturnFunctionToRun));

            var elapsed = 0f;
            while (MinDuration < elapsed && !IsFunctionDone)
            {
                elapsed += Time.DeltaTime;
                yield return null;
            }

            //TransitionComplete();
        }

        DelegateDeclarationBool BoolReturnFunctionToRun { get; }
        DelegateDeclarationImageData ImageDataReturnFunctionToRun { get; }
        public int MinDuration = 3000;
        //private bool ReadyToExecute { get; set; }
        private bool IsFunctionDone { get; set; }
        private bool LoadOnBackgroundThread { get; set; }
        protected IEnumerator RunFunction(DelegateDeclarationBool thingToRun)
        {

            //// if we arent loading a new scene we just set the flag as if we did so that the 2 phase transitions complete
            //if (!ReadyToExecute)
            //{
            //    IsFunctionDone = true;
            //    yield break;
            //}

            if (LoadOnBackgroundThread)
            {
                // load the Scene on a background thread
                Task.Run(() =>
                {
                    var done = thingToRun();

                    // get back to the main thread before setting the new Scene active. This isnt fantastic seeing as how
                    // the scheduler is not thread-safe but it should be empty between Scenes and SynchronizationContext.Current
                    // is null for some reason
                    Core.Schedule(0, false, null, timer =>
                    {
                        //Core.Scene = scene;
                        IsFunctionDone = done;
                    });
                });
            }
            else
            {
                //ore.Scene = sceneLoadAction();

                IsFunctionDone = thingToRun(); ;
            }

            // wait for the scene to load if it was loaded on a background thread
            while (!IsFunctionDone)
                yield return null;

            OnJobComplete(new JobCompleteEventArgs(true, null));

        }
        protected IEnumerator RunFunction(DelegateDeclarationImageData thingToRun)
        {

            //// if we arent loading a new scene we just set the flag as if we did so that the 2 phase transitions complete
            //if (!ReadyToExecute)
            //{
            //    IsFunctionDone = true;
            //    yield break;
            //}
            byte[] imageData = null;
            if (LoadOnBackgroundThread)
            {
                // load the Scene on a background thread
                Task.Run(() =>
                {
                    byte[] done = thingToRun();

                    // get back to the main thread before setting the new Scene active. This isnt fantastic seeing as how
                    // the scheduler is not thread-safe but it should be empty between Scenes and SynchronizationContext.Current
                    // is null for some reason
                    Core.Schedule(0, false, null, timer =>
                    {
                        //Core.Scene = scene;
                        IsFunctionDone = true;
                    });
                });
            }
            else
            {
                //ore.Scene = sceneLoadAction();
                imageData = thingToRun();
                IsFunctionDone = true;
            }

            // wait for the scene to load if it was loaded on a background thread
            while (!IsFunctionDone)
                yield return null;

            OnJobComplete(new JobCompleteEventArgs(true, imageData));

        }
        protected virtual void OnJobComplete(JobCompleteEventArgs e)
        {
            if (JobComplete != null)
            {
                JobComplete(this, e);
            }
        }

        public event EventHandler<JobCompleteEventArgs> JobComplete;
    }
}