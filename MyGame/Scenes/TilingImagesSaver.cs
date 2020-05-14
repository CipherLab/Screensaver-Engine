using Nez;
using ScreenSaverEngine2.Attributes;
using Unity;

namespace ScreenSaverEngine2.Scenes
{
    [StartupGuiScene("TilingImagesSaver", 9999, "")]
    public class TilingImagesSaver : StartSceneSubSceneHelper
    {
        public TilingImagesSaver()
        {

        }
        public override void Initialize()
        {
            this.Width = 1280;
            this.Height = 720;
            this.HasGui = false;
            this.IsFullScreen = false;
        }

       
    }
}