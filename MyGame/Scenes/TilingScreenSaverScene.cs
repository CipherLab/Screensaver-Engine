using System;
using System.Drawing;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using ScreenSaverEngine2.Attributes;
using ScreenSaverEngine2.Scenes.SceneHelpers;
using SharedKernel.Interfaces;
using Unity;
using Graphics = Nez.Graphics;

namespace ScreenSaverEngine2.Scenes
{
    [StartupGuiScene("TilingScreenSaverScene", 9999, "")]
    //can't use reflection to setup an abstract class as a scene
    public class TilingScreenSaverScene : BaseScreenSaver
    {
        public override void Initialize()
        {
            InitProps(
                true,
                true,
                false,
                false);
            base.Initialize();
        }

        private void InitProps(
            bool isTilingScreenSaver,
            bool hasVignettePostProcessor,
            bool hasGui,
            bool isFullScreen)
        {
            InitProps(
                null,
                Game1.Container.Resolve<ISimpleImageHelper>(),
                isTilingScreenSaver,
                hasVignettePostProcessor,
                false,
                false,
                false,
                false,
                false,
                false,
                0,
                Vector2.Zero,
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
            bool hasVignettePostProcessor,
            bool hasGlitchPostProcessor,
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
            this.IsTilingScreenSaver = isTilingScreenSaver;
            this.BackgroundImage = backgroundImage;
            this.ImageHelper = imageHelper;
            this.HasVignettePostProcessor = hasVignettePostProcessor;
            this.HasGlitchPostProcessor = hasGlitchPostProcessor;
            this.HasRigidBorders = hasRigidBorders;
            this.HasEdgeDetectRigidFloatingObjectsFromBackground = edgeDetectRigidFloatingObjectsFromBackground;
            this.ShowImageOnDetectedRigidFloatingObjects = showImageOnDetectedRigidFloatingObjects;
            this.RenderRigidBodiesAfterPostProcessors = renderRigidBodiesAfterPostProcessors;
            this.MaxFloatingRigidBodies = maxFloatingRigidBodies;
            this.IsFullScreen = isFullScreen;
            this.HasGui = hasGui;
            this.Width = width;
            this.Height = height;
        }
    }
}