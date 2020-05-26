using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ScreenSaverEngine2.Attributes;
using ScreenSaverEngine2.Scenes.SceneHelpers;
using SharedKernel.Interfaces;
using Unity;

namespace ScreenSaverEngine2.Scenes
{
    /*
    public class RigidBodySceneProperties
    {
        public RigidBodySceneProperties(
            bool hasGlitchPostProcessor,
            bool hasVignettePostProcessor,
            bool hasRigidBorders,
            bool edgeDetectRigidFloatingObjectsFromBackground,
            bool renderRigidBodiesAfterPostProcessors,
            int maxFloatingRigidBodies,
            bool isFullScreen,
            bool hasGui)
        {
            HasGlitchPostProcessor = hasGlitchPostProcessor;
            HasVignettePostProcessor = hasVignettePostProcessor;
            HasRigidBorders = hasRigidBorders;
            EdgeDetectRigidFloatingObjectsFromBackground = edgeDetectRigidFloatingObjectsFromBackground;
            RenderRigidBodiesAfterPostProcessors = renderRigidBodiesAfterPostProcessors;
            MaxFloatingRigidBodies = maxFloatingRigidBodies;
            IsFullScreen = isFullScreen;
            HasGui = hasGui;
        }

        public bool HasGlitchPostProcessor{get;}
      public bool HasVignettePostProcessor{get;}
      public bool HasRigidBorders{get;}
      public bool EdgeDetectRigidFloatingObjectsFromBackground{get;}
      public bool RenderRigidBodiesAfterPostProcessors{get;}
      public int MaxFloatingRigidBodies{get;}
      public bool IsFullScreen{get;}
      public bool HasGui { get; }
    }
    */

    [StartupGuiScene("Rigid Bg Particles", 80, "")]
    //can't use reflection to setup an abstract class as a scene
    public class RigidBodyWithParticlesScene : BaseScreenSaver
    {
        public override void Initialize()
        {
            InitProps(
                false,
                false,
                false,
                true,
                true,
                false,
                true,
                500,
                Vector2.Zero,
                false,
                true);
            base.Initialize();
        }

        private void InitProps(
            bool hasGlitchPostProcessor,
            bool hasVignettePostProcessor,
            bool hasRigidBorders,
            bool edgeDetectRigidFloatingObjectsFromBackground,
            bool showImageOnDetectedRigidFloatingObjects,
            bool renderRigidBodiesAfterPostProcessors,
            bool hasParticleSystem,
            int maxFloatingRigidBodies,
            Vector2 edgeDetectedRigidFloatingObjectsVelocity,
            bool isFullScreen,
            bool hasGui)
        {
            InitProps(
                Game1.StartupBackgroundScreenshot,
                Game1.Container.Resolve<ISimpleImageHelper>(),
                false,
                hasGlitchPostProcessor,
                hasVignettePostProcessor,
                hasRigidBorders,
                edgeDetectRigidFloatingObjectsFromBackground,
                showImageOnDetectedRigidFloatingObjects,
                renderRigidBodiesAfterPostProcessors,
                hasParticleSystem,
                maxFloatingRigidBodies,
                edgeDetectedRigidFloatingObjectsVelocity,
                isFullScreen,
                hasGui,
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height,
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width);
        }

        //thought this would help make sure everything that must be set is set
        public override void InitProps(
            byte[] backgroundImage,
            ISimpleImageHelper imageHelper,
            bool isTilingScreenSaver,
            bool hasGlitchPostProcessor,
            bool hasVignettePostProcessor,
            bool hasRigidBorders,
            bool edgeDetectRigidFloatingObjectsFromBackground,
            bool showImageOnDetectedRigidFloatingObjects,
            bool renderRigidBodiesAfterPostProcessors,
            bool hasParticleSystem,
            int maxFloatingRigidBodies,
            Vector2 edgeDetectedRigidFloatingObjectsVelocity,
            bool isFullScreen,
            bool hasGui,
            int height,
            int width)
        {
            this.BackgroundImage = backgroundImage;
            this.ImageHelper = imageHelper;
            this.HasGlitchPostProcessor = hasGlitchPostProcessor;
            this.HasRigidBorders = hasRigidBorders;
            this.HasEdgeDetectRigidFloatingObjectsFromBackground = edgeDetectRigidFloatingObjectsFromBackground;
            this.ShowImageOnDetectedRigidFloatingObjects = showImageOnDetectedRigidFloatingObjects;
            this.RenderRigidBodiesAfterPostProcessors = renderRigidBodiesAfterPostProcessors;
            this.HasParticleSystem = hasParticleSystem;
            this.EdgeDetectedRigidFloatingObjectsVelocity = edgeDetectedRigidFloatingObjectsVelocity;
            this.MaxFloatingRigidBodies = maxFloatingRigidBodies;
            this.IsFullScreen = isFullScreen;
            this.HasGui = hasGui;
            this.Width = width;
            this.Height = height;
        }
    }
}