using Nez;
using ScreenSaverEngine2.Scenes;
using ScreenSaverHelper;
using ScreenSaverHelper.Util;
using SharedKernel.Interfaces;
using Unity;

namespace ScreenSaverEngine2
{
    public class Game1 : Core
    {
        public static IUnityContainer Container { get; private set; }
        public static byte[] StartupBackgroundScreenshot { get; private set; }
        protected override void Initialize()
        {
            StartupBackgroundScreenshot = WpfApp.WindowsFormsHelper.ScreenGrab();

            base.Initialize();
            Window.AllowUserResizing = true;

            Container = new UnityContainer();

            Container.RegisterType<ISimpleImageHelper, ImageHelper>();
            Container.RegisterType<ICroppedImagePart, CroppedImagePart>();


            Scene = new StartUpScene();
        }

    }
}
