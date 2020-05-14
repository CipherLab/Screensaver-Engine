using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;
using ScreenSaverEngine2.Attributes;
using ScreenSaverEngine2.Shared;
using SharedKernel.Enums;
using SharedKernel.Interfaces;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using Nez.Tweens;
using Unity;
using Debug = Nez.Debug;
using Graphics = Nez.Graphics;
using Random = Nez.Random;

namespace ScreenSaverEngine2.Scenes
{
    [StartupGuiScene("RigidBodyScene", 9999, "")]
    public class RigidBodyFromImageSaver : StartSceneSubSceneHelper, IDisposable, IRigidBodyFromImageSaver
    {
        public int TwitchOffsetMin = 0;
        public int TwitchOffsetMax = 0;
        private readonly int BackgroundRenderLayer = 15;
        private readonly int RigidBodiesRenderLayer = 5;
        private bool IsFunctionDone = true;
        public RigidBodyFromImageSaver()
        {
            ImageHelper = Game1.Container.Resolve<ISimpleImageHelper>();
        }

        public byte[] MaskImageData { get; set; }
        public byte[] BackgroundImage { get; set; }
        public bool HasRigidBorders { get; set; }
        public ISimpleImageHelper ImageHelper { get; }
        private ConcurrentQueue<RunLoadingFunction> LoadingFunctions = new ConcurrentQueue<RunLoadingFunction>();
        private PixelGlitchPostProcessor PixelGlitchPostProcessor { get; set; }
        private bool StartUpdate { get; set; }

        public void Dispose()
        {
            ImageHelper?.Dispose();
        }

        public override void Initialize()
        {
            InitInterfaceProprties();
            base.Initialize();
        }

        public void InitInterfaceProprties()
        {
            this.SimpleFont = Content.Load<SpriteFont>("Shared\\SimpleFont");
            this.Width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            this.Height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            this.HasGui = true;
            this.BackgroundImage = Game1.StartupBackgroundScreenshot;
            this.IsFullScreen = false;
            this.HasRigidBorders = true;
        }

        public override void OnStart()
        {
            var bg = AddRenderer(new RenderLayerRenderer(0, BackgroundRenderLayer));
            var rb = AddRenderer(new RenderLayerRenderer(1, RigidBodiesRenderLayer));
            bg.WantsToRenderAfterPostProcessors = false;
            rb.WantsToRenderAfterPostProcessors = true;

            this.AddRenderer(bg);
            this.AddRenderer(rb);

            EnqueuLoadingFunction(SetBackground, 0, false);

            EnqueuLoadingFunction(GetEdgeDetectedObjectsFromBackground, 0, true);

            if (HasRigidBorders)
                EnqueuLoadingFunction(AddRigidBorders, 0, false);

            EnqueuLoadingFunction(AddRigidBorders, 0, false);

            PixelGlitchPostProcessor = AddPostProcessor(new PixelGlitchPostProcessor(1));
            PixelGlitchPostProcessor.HorizontalOffset = 0;

            Core.StartCoroutine(TwitchGlitchBackground(1, 5));

            Core.StartCoroutine(RunAllFunctions(LoadingFunctions));

            base.OnStart();
            StartUpdate = true;
        }

        public IEnumerator RunAllFunctions(ConcurrentQueue<RunLoadingFunction> rf)
        {
            yield return null;

            // load up the new Scene
            while (LoadingFunctions.Any())
            {
                if (IsFunctionDone)
                {
                    IsFunctionDone = false;
                    if (rf.TryDequeue(out RunLoadingFunction result))
                    {
                        yield return Core.StartCoroutine(result.RunUntilDurationAndDone());
                        result.JobComplete -= Rlf_JobComplete;
                    }
                }
                yield return null;
            }
        }
        public IEnumerator TwitchGlitchBackground(int twitchDelayMin, int twitchDelayMax)
        {
            yield return null;

            var elapsed = 0f;
            int twitchDelay = Random.Range(twitchDelayMin, twitchDelayMax);
            while (elapsed < twitchDelay)
            {
                elapsed += Time.DeltaTime;
                if (elapsed > twitchDelay)
                {
                    PixelGlitchPostProcessor.HorizontalOffset = Random.Range(TwitchOffsetMin, TwitchOffsetMax);
                    twitchDelay = Random.Range(twitchDelayMin, twitchDelayMax);
                    elapsed = 0;
                }
                yield return null;
            }

        }
        public override void Update()
        {
            if (!StartUpdate)
                return;

            if (TwitchOffsetMin + TwitchOffsetMax <= 0 && !LoadingFunctions.Any())
            {
                TwitchOffsetMin = 1;
                TwitchOffsetMax = 4;
            }

            base.Update();
        }

        private bool AddRigidBorders()
        {
            CreateEntity("leftBorder").AddComponent(new RigidBorders(4, ScreenEdgeSide.Left, ImageHelper));
            CreateEntity("topBorder").AddComponent(new RigidBorders(4, ScreenEdgeSide.Top, ImageHelper));
            CreateEntity("rightBorder").AddComponent(new RigidBorders(4, ScreenEdgeSide.Right, ImageHelper));
            CreateEntity("bottomBorder").AddComponent(new RigidBorders(4, ScreenEdgeSide.Bottom, ImageHelper));
            Debug.DrawText("AddRigidBorders", 1000f);
            return true;
        }

        private byte[] GetEdgeDetectedObjectsFromBackground()
        {
            return ImageHelper.EdgeDetectedFilledBlack(this.BackgroundImage, ImageFormat.Png);
        }
        private void EnqueuLoadingFunction(DelegateDeclarationBool processToRun, int minDuration, bool useBackgroundThread)
        {
            RunLoadingFunction rlf1 = new RunLoadingFunction(processToRun, minDuration, useBackgroundThread);
            rlf1.JobComplete += Rlf_JobComplete;
            LoadingFunctions.Enqueue(rlf1);
        }
        private void EnqueuLoadingFunction(DelegateDeclarationImageData processToRun, int minDuration, bool useBackgroundThread)
        {
            RunLoadingFunction rlf1 = new RunLoadingFunction(processToRun, minDuration, useBackgroundThread);
            rlf1.JobComplete += Rlf_JobComplete;
            LoadingFunctions.Enqueue(rlf1);
        }

        private void Rlf_JobComplete(object sender, JobCompleteEventArgs e)
        {
            Debug.DrawText("Job Complete", 1000f);
            IsFunctionDone = true;

            if (e.EdgeDetectedImageData != null && e.EdgeDetectedImageData.Any())
                MaskImageData = e.EdgeDetectedImageData;
        }
        private bool SetBackground()
        {
            var originalImageTex = Texture2D.FromStream(Graphics.Instance.Batcher.GraphicsDevice, new MemoryStream(BackgroundImage));


            var spriteR = new SpriteRenderer(originalImageTex);

            spriteR.SetRenderLayer(BackgroundRenderLayer);

            var bgEnt = CreateEntity("bg").SetPosition(Screen.Center);

            var bgComponent = bgEnt.AddComponent(spriteR);
            bgComponent.SetRenderLayer(BackgroundRenderLayer);

            Debug.DrawText("AddRigidBorders", 1000f);
            return true;
        }
    }
}