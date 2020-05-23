using Nez;
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
using Microsoft.Xna.Framework.Graphics;
using Nez.Sprites;
using Nez.Tweens;
using Unity;
using Debug = Nez.Debug;
using Random = Nez.Random;

namespace ScreenSaverEngine2.Scenes
{
    public abstract class BaseScreenSaver : StartSceneSubSceneHelper
    {
        protected ISimpleImageHelper ImageHelper { get; set; }
        protected bool HasGlitchPostProcessor { get; set; }
        protected bool HasRigidBorders { get; set; }
        protected bool HasEdgeDetectRigidFloatingObjectsFromBackground { get; set; }
        protected bool RenderRigidBodiesAfterPostProcessors { get; set; }
        protected int MaxFloatingRigidBodies { get; set; }

        //these are in StartSceneSubSceneHelper
        //protected bool IsFullScreen { get; set; }
        //protected bool HasGui { get; set; }
        //protected int Height { get; set; }
        //protected int Width { get; set; }
        protected byte[] BackgroundImage { get; set; }

        //must override
        //public abstract override string ToString();

        public virtual bool SetBackgroundImage()
        {
            int backgroundRenderLayer = 15;
            var originalImageTex = Texture2D.FromStream(Nez.Graphics.Instance.Batcher.GraphicsDevice, new MemoryStream(BackgroundImage));

            var spriteR = new SpriteRenderer(originalImageTex);

            spriteR.SetRenderLayer(backgroundRenderLayer);

            var bgEnt = CreateEntity("bg").SetPosition(Screen.Center);

            var bgComponent = bgEnt.AddComponent(spriteR);
            bgComponent.SetRenderLayer(backgroundRenderLayer);

            return true;
        }

        //Optional to override
        public abstract void InitProps(byte[] backgroundImage,
            ISimpleImageHelper imageHelper,
            bool hasGlitchPostProcessor,
            bool hasRigidBorders,
            bool edgeDetectRigidFloatingObjectsFromBackground,
            bool renderRigidBodiesAfterPostProcessors,
            int maxFloatingRigidBodies,
            bool isFullScreen,
            bool hasGui,
            int height,
            int width);

        //Here's the functionality, override it if you want (or as abstract override to force next class)
        public virtual string ToString3()
        {
            return string.Empty;
        }

        private bool StartUpdate { get; set; }

        private int GlitchOffsetMax = 0;
        private int GlitchOffsetMin = 0;
        private readonly int BackgroundRenderLayer = 15;
        private readonly int RigidBodiesRenderLayer = 5;
        private bool _isFunctionDone = true;
        private PixelGlitchPostProcessor PixelGlitchPostProcessor { get; set; }
        private ConcurrentQueue<RunLoadingFunction> LoadingFunctions = new ConcurrentQueue<RunLoadingFunction>();

        public override void OnStart()
        {
            var bg = AddRenderer(new RenderLayerRenderer(0, BackgroundRenderLayer));
            var rb = AddRenderer(new RenderLayerRenderer(1, RigidBodiesRenderLayer));
            bg.WantsToRenderAfterPostProcessors = false;
            rb.WantsToRenderAfterPostProcessors = RenderRigidBodiesAfterPostProcessors;

            this.AddRenderer(bg);
            this.AddRenderer(rb);

            EnqueueLoadingFunction(SetBackgroundImage, 0, false);

            if (HasRigidBorders)
                EnqueueLoadingFunction(AddRigidBorders, 0, false);

            if (HasEdgeDetectRigidFloatingObjectsFromBackground)
                EnqueueLoadingFunction(GetEdgeDetectedObjectsFromBackground, 0, true);

            if (HasGlitchPostProcessor)
            {
                PixelGlitchPostProcessor = AddPostProcessor(new PixelGlitchPostProcessor(1));
                PixelGlitchPostProcessor.HorizontalOffset = 0;
                Core.StartCoroutine(GlitchBackground(1, 5));
            }

            Core.StartCoroutine(RunAllFunctions(LoadingFunctions));

            base.OnStart();
            StartUpdate = true;
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

        public IEnumerator GlitchBackground(int glitchDelayMin, int glitchDelayMax)
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
    }
}