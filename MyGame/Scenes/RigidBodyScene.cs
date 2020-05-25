using System;
using Microsoft.Xna.Framework.Graphics;
using ScreenSaverEngine2.Attributes;
using ScreenSaverEngine2.Scenes.SceneHelpers;
using SharedKernel.Interfaces;
using Unity;

namespace ScreenSaverEngine2.Scenes
{
    [StartupGuiScene("RigidBodyScene", 9999, "")]
    //can't use reflection to setup an abstract class as a scene
    public class RigidBodyScene : BaseScreenSaver
    {
        public override void Initialize()
        {
            InitProps(
                true,
                true,
                true,
                true,
                true,
                300,
                false,
                true);
            base.Initialize();
        }

        private void InitProps(
            bool hasGlitchPostProcessor,
            bool hasVignettePostProcessor,
            bool hasRigidBorders,
            bool edgeDetectRigidFloatingObjectsFromBackground,
            bool renderRigidBodiesAfterPostProcessors,
            int maxFloatingRigidBodies,
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
                renderRigidBodiesAfterPostProcessors,
                maxFloatingRigidBodies,
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
            bool renderRigidBodiesAfterPostProcessors,
            int maxFloatingRigidBodies,
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
            this.RenderRigidBodiesAfterPostProcessors = renderRigidBodiesAfterPostProcessors;
            this.MaxFloatingRigidBodies = maxFloatingRigidBodies;
            this.IsFullScreen = isFullScreen;
            this.HasGui = hasGui;
            this.Width = width;
            this.Height = height;
        }
    }
}