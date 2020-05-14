using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.BitmapFonts;
using Nez.Sprites;

namespace MonoGameTest.Scenes
{
    [WindowScene("Basic Scene", 9999, "Scene with a single Entity. The minimum to have something to show")]
    public class BasicScene : SubSceneHelper
    {
        public override void Initialize()
        {
            base.Initialize();


            this.SimpleFont = Content.Load<SpriteFont>("Shared\\SimpleFont");
            SetDesignResolution(1280, 720, Scene.SceneResolutionPolicy.ShowAll);
            Screen.SetSize(1280, 720);

            var moonTex = Content.Load<Texture2D>(Nez.Content.Shared.Moon);
            var moonEntity = CreateEntity("moon", new Vector2(Screen.Width / 2, Screen.Height / 2));
            moonEntity.AddComponent(new SpriteRenderer(moonTex));

            Screen.IsFullscreen = false;
            Screen.ApplyChanges();

            //  Graphics.Instance.BitmapFont = Core.Content.LoadBitmapFont("Content\\Shared\\montserrat-32.fnt");

        }
    }
}