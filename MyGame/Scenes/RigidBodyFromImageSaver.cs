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
using System.Collections.Generic;
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
        public byte[] BackgroundImage { get; set; }
        public bool HasGlitchPostProcessor { get; set; }
        public bool HasEdgeDetectedRigidFloatingObjects { get; set; }
        public bool HasRigidBorders { get; set; }
        public bool RenderRigidBodiesAfterPostProcess { get; set; }
        public ISimpleImageHelper ImageHelper { get; }
        public int MaxFloatingRigidBodies { get; set; }

        private PixelGlitchPostProcessor PixelGlitchPostProcessor { get; set; }

        private bool StartUpdate { get; set; }

        public int GlitchOffsetMax = 0;
        public int GlitchOffsetMin = 0;
        private readonly int BackgroundRenderLayer = 15;
        private readonly int RigidBodiesRenderLayer = 5;
        private bool _isFunctionDone = true;

        private ConcurrentQueue<RunLoadingFunction> LoadingFunctions = new ConcurrentQueue<RunLoadingFunction>();

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
            //this.Width = 1920;
            //this.Height = 1080;

            this.HasGui = false;
            this.BackgroundImage = Game1.StartupBackgroundScreenshot;
            this.IsFullScreen = true;
            this.HasRigidBorders = true;
            this.HasGlitchPostProcessor = true;
            this.HasEdgeDetectedRigidFloatingObjects = true;
            this.RenderRigidBodiesAfterPostProcess = true;
            this.MaxFloatingRigidBodies = 300;
        }

        public RigidBodyFromImageSaver()
        {
            ImageHelper = Game1.Container.Resolve<ISimpleImageHelper>();
        }

        public override void OnStart()
        {
            var bg = AddRenderer(new RenderLayerRenderer(0, BackgroundRenderLayer));
            var rb = AddRenderer(new RenderLayerRenderer(1, RigidBodiesRenderLayer));
            bg.WantsToRenderAfterPostProcessors = false;
            rb.WantsToRenderAfterPostProcessors = RenderRigidBodiesAfterPostProcess;

            this.AddRenderer(bg);
            this.AddRenderer(rb);

            EnqueueLoadingFunction(SetBackground, 0, false);

            if (HasRigidBorders)
                EnqueueLoadingFunction(AddRigidBorders, 0, false);

            if (HasEdgeDetectedRigidFloatingObjects)
                EnqueueLoadingFunction(GetEdgeDetectedObjectsFromBackground, 0, true);

            if (HasGlitchPostProcessor)
            {
                PixelGlitchPostProcessor = AddPostProcessor(new PixelGlitchPostProcessor(1));
                PixelGlitchPostProcessor.HorizontalOffset = 0;
                Core.StartCoroutine(GlitchGlitchBackground(1, 5));
            }

            Core.StartCoroutine(RunAllFunctions(LoadingFunctions));

            base.OnStart();
            StartUpdate = true;
        }

        public IEnumerator AddRigidBodyPartsToScene(List<ICroppedImagePart> parts)
        {
            yield return null;

            int total = parts.Count();
            int idx = 0;

            while (idx < total)
            {
                AddRigidBodies(parts[idx++]);
                Console.WriteLine($"AddRigidBodies {idx}");
                yield return null;
            }
        }

        public void Dispose()
        {
            ImageHelper?.Dispose();
        }

        public IEnumerator GlitchGlitchBackground(int glitchDelayMin, int glitchDelayMax)
        {
            yield return null;

            var elapsed = 0f;
            int glitchDelay = Random.Range(glitchDelayMin, glitchDelayMax);
            while (elapsed < glitchDelay)
            {
                elapsed += Time.DeltaTime;
                if (elapsed > glitchDelay)
                {
                    PixelGlitchPostProcessor.HorizontalOffset = Random.Range(GlitchOffsetMin, GlitchOffsetMax);
                    glitchDelay = Random.Range(glitchDelayMin, glitchDelayMax);
                    elapsed = 0;
                }
                yield return null;
            }
        }

        public IEnumerator RunAllFunctions(ConcurrentQueue<RunLoadingFunction> rf)
        {
            yield return null;

            // load up the new Scene
            while (LoadingFunctions.Any())
            {
                if (_isFunctionDone)
                {
                    _isFunctionDone = false;
                    if (rf.TryDequeue(out RunLoadingFunction result))
                    {
                        yield return Core.StartCoroutine(result.RunUntilDurationAndDone());
                    }
                }
                yield return null;
            }
        }

        public override void Update()
        {
            if (!StartUpdate)
                return;

            if (GlitchOffsetMin + GlitchOffsetMax <= 0 && !LoadingFunctions.Any())
            {
                GlitchOffsetMin = 1;
                GlitchOffsetMax = 4;
            }

            base.Update();
        }

        private bool AddRigidBodies(ICroppedImagePart part)
        {
            CreateEntity(Guid.NewGuid().ToString()).AddComponent(new RigidBodies(part, RigidBodiesRenderLayer));
            return true;
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

        private void EnqueueLoadingBoundingBoxes(byte[] imageData)
        {
            SceneHelpers.DelegateDeclarationListRectangle d1 = ImageHelper.GetSpriteBoundingBoxesInImage;
            RunLoadingFunction rlf = new RunLoadingFunction(d1, 0, true, imageData, 4, 1);

            rlf.JobComplete += (sender, e) =>
            {
                Console.WriteLine("Got bounding boxes");
                //got bounding boxes
                _isFunctionDone = true;
                //get sprites
                if (e.BoxData.Any())
                    EnqueueLoadingSpritesFromImageBoxData(e.BoxData);
            };
            LoadingFunctions.Enqueue(rlf);
        }

        private void EnqueueLoadingFunction(SceneHelpers.DelegateDeclarationBool processToRun, int minDuration, bool useBackgroundThread)
        {
            RunLoadingFunction rlf1 = new RunLoadingFunction(processToRun, minDuration, useBackgroundThread);
            rlf1.JobComplete += (sender, args) =>
            {
                Console.WriteLine("Loaded a thing");
                _isFunctionDone = true;
            };
            LoadingFunctions.Enqueue(rlf1);
        }

        private void EnqueueLoadingFunction(SceneHelpers.DelegateDeclarationImageData processToRun, int minDuration, bool useBackgroundThread)
        {
            RunLoadingFunction rlf = new RunLoadingFunction(processToRun, minDuration, useBackgroundThread);
            rlf.JobComplete += (sender, e) =>
            {
                Console.WriteLine("Got mask image data");
                //Got mask image data
                _isFunctionDone = true;
                //get bounding boxes
                if (e.EdgeDetectedImageData != null && e.EdgeDetectedImageData.Any())
                    EnqueueLoadingBoundingBoxes(e.EdgeDetectedImageData);
            };
            LoadingFunctions.Enqueue(rlf);
        }

        private void EnqueueLoadingSpritesFromImageBoxData(List<Rectangle> boxData)
        {
            SceneHelpers.DelegateDeclarationListCroppedImageParts d1 = ImageHelper.GetSpritesFromImage;
            RunLoadingFunction rlf = new RunLoadingFunction(d1, 0, true, this.BackgroundImage,
                boxData.OrderByDescending(r => r.Width * r.Height).Take(MaxFloatingRigidBodies).ToList());

            rlf.JobComplete += (sender, e) =>
            {
                Console.WriteLine("Got sprites from bounding boxes");
                //got sprites
                _isFunctionDone = true;
                //put it on the screen
                if (e.CroppedImageParts.Any())
                    Core.StartCoroutine(AddRigidBodyPartsToScene(e.CroppedImageParts.ToList()));
            };
            LoadingFunctions.Enqueue(rlf);
        }

        private byte[] GetEdgeDetectedObjectsFromBackground()
        {
            return ImageHelper.EdgeDetectedFilledBlack(this.BackgroundImage, ImageFormat.Png);
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