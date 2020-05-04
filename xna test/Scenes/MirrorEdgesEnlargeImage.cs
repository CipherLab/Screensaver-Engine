using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.BitmapFonts;
using Nez.Sprites;
using Nez.Textures;

namespace MonoGameTest.Scenes
{
    //  [SampleScene("FancyTilingScene Scene", 9999, "FancyTilingScene")]
    public class FancyTilingScene : Scene
    {
        private TilingScreenSaverComponent tilingScreenSaverComponent;

        private Texture2D moonTex { get; set; }
        private Entity moonEntity { get; set; }
        private SpriteRenderer spriteRenderer { get; set; }
        SpriteRenderer addedComponent { get; set; }

        public FancyTilingScene()
        {
            Enabled = true;
        }
        public override void Initialize()
        {
            base.Initialize();


            //var bounds = new Rectangle(0, 0, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
            //    GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
            
            var simpleFont = Content.Load<SpriteFont>("Shared\\SimpleFont");

            SetDesignResolution(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height, Scene.SceneResolutionPolicy.ShowAll);
            Screen.SetSize(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
            //Screen.IsFullscreen = true;
            Screen.ApplyChanges();
            //var moonTex = Content.Load<Texture2D>(Nez.Content.Shared.Moon);
            // var imageEntity = CreateEntity("images", new Vector2(Screen.Width / 2, Screen.Height / 2));
            //moonEntity.AddComponent(new SpriteRenderer(moonTex));

            tilingScreenSaverComponent = new TilingScreenSaverComponent(Graphics.Instance.Batcher.GraphicsDevice);
            tilingScreenSaverComponent.InitFont(simpleFont);
            // imageEntity.AddComponent(tilingScreenSaverComponent);

            moonTex = Content.Load<Texture2D>(Nez.Content.Shared.Moon);
            moonEntity = CreateEntity("moon", new Vector2(Screen.Width / 2, Screen.Height / 2));
            spriteRenderer = new SpriteRenderer(moonTex);
            addedComponent = moonEntity.AddComponent(spriteRenderer);


            
            Screen.IsFullscreen = true;
            Screen.ApplyChanges();


        }

        public bool Enabled { get; }
        public int UpdateOrder { get; }
        public override void Update()
        {
             tilingScreenSaverComponent.Update();
            switch (tilingScreenSaverComponent.CurrentPhase)
            {
                case TilingScreenSaverComponent.Phase.GetImage:
                    moonTex = tilingScreenSaverComponent.CurrentImage;
                    if (moonTex != null)
                        addedComponent.Sprite = new Sprite(moonTex);
                    break;
                case TilingScreenSaverComponent.Phase.FadeIn:
                case TilingScreenSaverComponent.Phase.FadeOut:
                    //addedComponent.Color = new Color(0, 0, 0,
                    //    MathHelper.Clamp( tilingScreenSaverComponent.mAlphaValue, 0, 255));
                    break;
                case TilingScreenSaverComponent.Phase.ShowImage:
                    Vector2 loc = tilingScreenSaverComponent.ImagePosition;
                    //addedComponent.Transform.LocalPosition = loc;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            base.Update();
        }

    }
}