using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Console;
using Nez.ImGuiTools;
using Nez.Tweens;
using Nez.UI;
using ScreenSaverEngine2.Attributes;

namespace ScreenSaverEngine2
{
    /// <summary>
    /// this entire class is one big sweet hack job to make adding samples easier. An exceptional hack is made so that we can render small
    /// pixel art scenes pixel perfect and still display our UI at a reasonable size.
    /// </summary>
    public abstract class FullScreenSubSceneHelper : Scene, IFinalRenderDelegate
    {

        public bool HasGui { get; set; }
        public bool IsFullScreen { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }

        ScreenSpaceRenderer _screenSpaceRenderer;

        public override void Initialize()
        {
            //  Graphics.Instance.BitmapFont = Core.Content.LoadBitmapFont("Content\\Shared\\montserrat-32.fnt");
        }

        protected FullScreenSubSceneHelper(bool addExcludeRenderer = true, bool needsFullRenderSizeForUi = false)
        {


            SetDesignResolution(Width, Height, Scene.SceneResolutionPolicy.ShowAllPixelPerfect);
            Screen.SetSize(Width, Height);

            Screen.IsFullscreen = IsFullScreen;
            Screen.ApplyChanges();

        }

        IEnumerable<Type> GetTypesWithWindowedSceneAttribute()
        {
            var assembly = typeof(FullScreenSubSceneHelper).Assembly;
            var scenes = assembly.GetTypes()
                .Where(t => t.GetCustomAttributes(typeof(StartupGuiSceneAttribute), true).Length > 0);


            foreach (var s in scenes)
                yield return s;
        }


        #region IFinalRenderDelegate

        private Scene _scene;

        public void OnAddedToScene(Scene scene) => _scene = scene;

        public void OnSceneBackBufferSizeChanged(int newWidth, int newHeight) => _screenSpaceRenderer.OnSceneBackBufferSizeChanged(newWidth, newHeight);

        public void HandleFinalRender(RenderTarget2D finalRenderTarget, Color letterboxColor, RenderTarget2D source,
                                      Rectangle finalRenderDestinationRect, SamplerState samplerState)
        {
            Core.GraphicsDevice.SetRenderTarget(null);
            Core.GraphicsDevice.Clear(letterboxColor);
            Graphics.Instance.Batcher.Begin(BlendState.Opaque, samplerState, DepthStencilState.None, RasterizerState.CullNone, null);
            Graphics.Instance.Batcher.Draw(source, finalRenderDestinationRect, Color.White);
            Graphics.Instance.Batcher.End();

            _screenSpaceRenderer.Render(_scene);
        }


        #endregion
    }

}