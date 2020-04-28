using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.UI;

namespace MyGame
{
    /// <summary>
    /// this entire class is one big sweet hack job to make adding samples easier. An exceptional hack is made so that we can render small
    /// pixel art scenes pixel perfect and still display our UI at a reasonable size.
    /// </summary>
    public class SubSceneHelper : Scene
    {
        public const int ScreenSpaceRenderLayer = 999;
        public UICanvas Canvas;

        Table _table;
        DefaultRenderer _render;

        public SubSceneHelper()
        {
          

            Canvas = CreateEntity("ui").AddComponent(new UICanvas());
            Canvas.IsFullScreen = true;
            Canvas.RenderLayer = 999;
            this.Canvas.Stage.GetRoot().SetPosition(.5f, .5f);

            Canvas.AddComponent(new TextComponent(
                    Graphics.Instance.BitmapFont,
                    "WHAT the FUCK", new Vector2(150, 120), Color.White))
                .SetRenderLayer(0);


        }



    }

}