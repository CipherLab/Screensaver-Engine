using Nez;
using ScreenSaverEngine2.Attributes;
using Unity;

namespace ScreenSaverEngine2.Scenes
{
    [StartupGuiScene("SpriteCharWRigidBodyBg", 9999, "Sprite character that comes out to interact with objs")]
    public class SpriteCharWRigidBodyBg : RigidBodyFromImageSaver
    {
        public SpriteCharWRigidBodyBg()
        {

        }
        public override void Initialize()
        {
            //init the rigid body bg
            //base.Initialize();

            this.Width = 1280;
            this.Height = 720;
            this.HasGui = false;
            this.IsFullScreen = false;
        }

    }
}