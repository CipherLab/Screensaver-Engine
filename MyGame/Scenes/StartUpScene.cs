using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;
using ScreenSaverEngine2.Attributes;
using ScreenSaverEngine2.Shared;
using SharedKernel.Interfaces;
using Unity;
using Color = System.Drawing.Color;

namespace ScreenSaverEngine2.Scenes
{
    [StartupGuiScene("Startup Scene", 9999, "Entry point, list screensavers")]
    public class StartUpScene : StartSceneSubSceneHelper
    {
        public override void Initialize()
        {
            //  Graphics.Instance.BitmapFont = Core.Content.LoadBitmapFont("Content\\Shared\\montserrat-32.fnt");
            this.SimpleFont = Content.Load<SpriteFont>("Shared\\SimpleFont");
            this.Width = 1280;
            this.Height = 720;
            this.HasGui = true;
            this.IsFullScreen = false;
            base.Initialize();
        }

      
    }
}