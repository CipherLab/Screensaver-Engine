using System;
using Microsoft.Xna.Framework.Graphics;
using ScreenSaverEngine2.Attributes;
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
                Game1.StartupBackgroundScreenshot,
                Game1.Container.Resolve<ISimpleImageHelper>(),
                true,
                true,
                true,
                true,
                300,
                false,
                true,
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height,
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width);

            base.Initialize();
        }

        //thought this would help make sure everything that must be set is set
        public override void InitProps(
            byte[] backgroundImage,
            ISimpleImageHelper imageHelper,
            bool hasGlitchPostProcessor,
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