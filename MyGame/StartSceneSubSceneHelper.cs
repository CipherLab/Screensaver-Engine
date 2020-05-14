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
using Unity;

namespace ScreenSaverEngine2
{
    /// <summary>
    /// this entire class is one big sweet hack job to make adding samples easier. An exceptional hack is made so that we can render small
    /// pixel art scenes pixel perfect and still display our UI at a reasonable size.
    /// </summary>
    public abstract class StartSceneSubSceneHelper : Scene, IFinalRenderDelegate
    {
        public bool IsFullScreen { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }


        public const int ScreenSpaceRenderLayer = 999;
        public UICanvas Canvas;
        public UICanvas ForeGroundStage;


        Table _table;
        List<Button> _sceneButtons = new List<Button>();
        ScreenSpaceRenderer _screenSpaceRenderer;
        static bool _needsFullRenderSizeForUi;

        public SpriteFont SimpleFont { get; set; }
        public override void Initialize()
        {
            Init(false, false);
            //  Graphics.Instance.BitmapFont = Core.Content.LoadBitmapFont("Content\\Shared\\montserrat-32.fnt");
        }

     

        private void Init(bool addExcludeRenderer, bool needsFullRenderSizeForUi)
        {
            SetDesignResolution(Width, Height, Scene.SceneResolutionPolicy.ShowAll);
            Screen.SetSize(Width, Height);

            Screen.IsFullscreen = IsFullScreen;
            Screen.ApplyChanges();


            if (HasGui)
            {
                _needsFullRenderSizeForUi = needsFullRenderSizeForUi;

                // setup one renderer in screen space for the UI and then (optionally) another renderer to render everything else
                if (needsFullRenderSizeForUi)
                {
                    // dont actually add the renderer since we will manually call it later
                    _screenSpaceRenderer = new ScreenSpaceRenderer(100, ScreenSpaceRenderLayer);
                    _screenSpaceRenderer.ShouldDebugRender = false;
                    FinalRenderDelegate = this;
                }
                else
                {
                    AddRenderer(new ScreenSpaceRenderer(100, ScreenSpaceRenderLayer));
                }

                if (addExcludeRenderer)
                    AddRenderer(new RenderLayerExcludeRenderer(0, ScreenSpaceRenderLayer));


                // create our canvas and put it on the screen space render layer
                Canvas = CreateEntity("ui").AddComponent(new UICanvas());
                Canvas.IsFullScreen = true;
                Canvas.RenderLayer = ScreenSpaceRenderLayer;
                SetupSceneSelector();
                GetAttributesAndCreateElements();
            }
        }


        void SetupSceneSelector()
        {
            _table = Canvas.Stage.AddElement(new Table());
            _table.SetFillParent(true).Right().Top();

            var topButtonStyle = new TextButtonStyle(new PrimitiveDrawable(Color.Black, 10f),
                new PrimitiveDrawable(Color.Yellow), new PrimitiveDrawable(Color.DarkSlateBlue))
            {
                DownFontColor = Color.Black
            };
            _table.Add(new TextButton("Toggle Scene List", topButtonStyle)).SetFillX().SetMinHeight(30)
                .GetElement<Button>().OnClicked += OnToggleSceneListClicked;

            _table.Row().SetPadTop(10);
            var checkbox = _table.Add(new CheckBox("Debug Render", new CheckBoxStyle
            {
                CheckboxOn = new PrimitiveDrawable(30, Color.Green),
                CheckboxOff = new PrimitiveDrawable(30, new Color(0x00, 0x3c, 0xe7, 0xff))
            })).GetElement<CheckBox>();
            checkbox.OnChanged += enabled => Core.DebugRenderEnabled = enabled;
            checkbox.IsChecked = Core.DebugRenderEnabled;
            _table.Row().SetPadTop(30);

            //var buttonStyle = new TextButtonStyle(
            //             new PrimitiveDrawable(new Color(78, 91, 98), 10f),
            //	new PrimitiveDrawable(new Color(244, 23, 135)), 
            //             new PrimitiveDrawable(new Color(168, 207, 115)))
            //{
            //	DownFontColor = Color.Black
            //};


            // find every Scene with the SampleSceneAttribute and create a button for each one
        }
        IEnumerable<Type> GetTypesWithWindowedSceneAttribute()
        {
            var assembly = typeof(StartSceneSubSceneHelper).Assembly;
            var scenes = assembly.GetTypes()
                .Where(t => t.GetCustomAttributes(typeof(StartupGuiSceneAttribute), true).Length > 0)
                .OrderBy<Type, int>(t =>
                    ((StartupGuiSceneAttribute)t.GetCustomAttributes(typeof(StartupGuiSceneAttribute), true)[0]).Order);

            foreach (var s in scenes)
                yield return s;
        }



        public bool HasGui { get; set; }

        private void GetAttributesAndCreateElements()
        {
            var buttonStyle = new TextButtonStyle(
                new PrimitiveDrawable(Color.Black),
                new PrimitiveDrawable(Color.Black),
                new PrimitiveDrawable(Color.Black),
                Graphics.Instance.BitmapFont
            );
            foreach (Type type in GetTypesWithWindowedSceneAttribute())
            {
                foreach (var attr in type.GetCustomAttributes(true))
                {
                    if (attr.GetType() == typeof(StartupGuiSceneAttribute))
                    {
                        var sampleAttr = attr as StartupGuiSceneAttribute;
                        if (sampleAttr == null)
                            throw new NullReferenceException("Attrib Width Cannot Be Null");

                        var button = _table.Add(
                                new TextButton(sampleAttr.ButtonName, buttonStyle)).SetFillX()
                            .SetMinHeight(30).GetElement<TextButton>();
                        _sceneButtons.Add(button);
                        button.OnClicked += butt =>
                        {
                            // stop all tweens in case any demo scene started some up
                            TweenManager.StopAllTweens();
                            Core.GetGlobalManager<ImGuiManager>()?.SetEnabled(false);
                            Core.StartSceneTransition(new FadeTransition(() =>
                                Activator.CreateInstance(type) as Scene));
                        };

                        _table.Row().SetPadTop(10);

                        // optionally add instruction text for the current scene
                        if (sampleAttr?.InstructionText != null && type == GetType())
                            AddInstructionText(sampleAttr.InstructionText);
                    }
                }
            }
        }

  
        void AddInstructionText(string text)
        {
            var instructionsEntity = CreateEntity("instructions");
            instructionsEntity
                .AddComponent(new TextComponent(
                    //this.SensationBitmapFont,
                    new NezSpriteFont(this.SimpleFont),
                    //Graphics.Instance.BitmapFont, 
                    text, new Vector2(10, 10), Color.White))
                .SetRenderLayer(ScreenSpaceRenderLayer);
        }

        void OnToggleSceneListClicked(Button butt)
        {
            foreach (var button in _sceneButtons)
                button.SetIsVisible(!button.IsVisible());
        }

        [Nez.Console.Command("toggle-imgui", "Toggles the Dear ImGui renderer")]
        static void ToggleImGui()
        {
            if (_needsFullRenderSizeForUi)
            {
                DebugConsole.Instance.Log("Error: due to the way the sample scenes are assembled, only full screen sample scenes can use ImGui");
                return;
            }

            // install the service if it isnt already there
            var service = Core.GetGlobalManager<ImGuiManager>();
            if (service == null)
            {
                service = new ImGuiManager();
                Core.RegisterGlobalManager(service);
            }
            else
            {
                service.SetEnabled(!service.Enabled);
            }
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