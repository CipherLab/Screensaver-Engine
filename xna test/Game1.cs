using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FancyTiling;
using MonoGame.Extended.Sprites;
using ScreenSaverHelper;
using SharedKernel;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace MonoGameTest
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {

        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        string _imageTest1 = @"G:\AD\Amazon Drive\Pictures\backgrounds\0s7Faqh.jpg";
        string _imageTest2 = @"G:\AD\Amazon Drive\Pictures\backgrounds\0X3EHcT.jpg";

        public static SpriteFont SimpleFont;
        public static Texture2D EmptyPixel;

        private KeyboardState _lastKeyState;

        private Rectangle Bounds { get; }
        private ISettings Settings { get; }
        private ImageHelper ImageHelper { get; set; }
        public Texture2D CurrentImage;
        Sprite _imageSprite;
        private List<string> ImageFiles { get; }
        private int _imageIdx = 0;
        int _extraPaddingAmountPx = 100;
        public Camera2d Camera { get; }
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Bounds = new Rectangle(0, 0, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
                 GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
            //Bounds = new Rectangle(0, 0, 1920, 1280);
            ImageHelper = new ImageHelper(new System.Drawing.Rectangle(0, 0, Bounds.Width + _extraPaddingAmountPx, Bounds.Height + _extraPaddingAmountPx), true);

            Settings = new Settings().LoadFromReg();

            ImageFiles = Directory
                .GetFiles(Settings.Path, "*.jpg", SearchOption.AllDirectories).ToList();
            if (ImageFiles.Count <= 0)
                return;

            if (Settings.Shuffle)
                ImageFiles = ImageFiles.Randomize().ToList();

            _graphics.PreferredBackBufferHeight = Bounds.Height;
            _graphics.PreferredBackBufferWidth = Bounds.Width;
            _graphics.IsFullScreen = true;
            IsMouseVisible = true;
            Content.RootDirectory = "Content";

            PanTarget1 = new Vector2((Bounds.Width / 2f) + _extraPaddingAmountPx, (Bounds.Height / 2f) + _extraPaddingAmountPx);
            Camera = new Camera2d(Bounds.Width, Bounds.Height);
        }

        protected override void Initialize()
        {
            _lastKeyState = Keyboard.GetState();
            base.Initialize();
        }


        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            SimpleFont = Content.Load<SpriteFont>("SimpleFont");

            EmptyPixel = new Texture2D(GraphicsDevice, 1, 1);
            EmptyPixel.SetData<Color>(new Color[] { Color.White });

            //ShowFancyTileImageFromFile(ImageFiles.First());
            ShowNextImage();
            this._ellapsedWatch.Start();
        }

        //private void ShowFancyTileImageFromFile(string f)
        //{
        //    _loadingImage = true;
        //    try
        //    {
        //        var image = ImageHelper.MirrorUpconvertImage(f);
        //        CurrentImage = Texture2D.FromStream(GraphicsDevice, new MemoryStream(image));
        //    }
        //    finally
        //    {
        //        _loadingImage = false;
        //    }
        //}


        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private bool _loadingImage = false;
        List<int[]> _stepData = new List<int[]>();
        private KeyboardState PreviousKeyboardState { get; set; }
        private KeyboardState CurrentKeyboardState { get; set; }
        public bool IsKeyPressed(Keys key)
        {
            return PreviousKeyboardState.IsKeyDown(key) && CurrentKeyboardState.IsKeyUp(key);
        }

        private bool startMove = false;
        float duration = 15.0f;
        float elapsedTime = 0;
        Stopwatch _ellapsedWatch = new Stopwatch();
        private Vector2 PanTarget1 { get; }
        protected override void Update(GameTime gameTime)
        {
            PreviousKeyboardState = CurrentKeyboardState;
            CurrentKeyboardState = Keyboard.GetState();


            if (CurrentKeyboardState.IsKeyDown(Keys.T))
                startMove = true;

            if (startMove)
            {
                // Move our sprite based on arrow keys being pressed:
                float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

                elapsedTime += dt;
                if (elapsedTime > duration)
                {
                    elapsedTime = 0;
                    startMove = false;
                    ShowNextImage();
                }

                float param = elapsedTime / duration;
                var pos = Vector2.Lerp(Camera.CenterScreen, PanTarget1, (float)Math.Pow(param / 2.0f, .5f));
                //var pos = Vector2.Lerp(Camera.CenterScreen, PanTarget1, param);
                Camera.Move(pos);
            }
            if (CurrentKeyboardState.IsKeyDown(Keys.D))
                Camera.Move(new Vector2(1f, 0));
            if (CurrentKeyboardState.IsKeyDown(Keys.A))
                Camera.Move(new Vector2(-1f, 0));
            if (CurrentKeyboardState.IsKeyDown(Keys.S))
                Camera.Move(new Vector2(0, 1f));
            if (CurrentKeyboardState.IsKeyDown(Keys.W))
                Camera.Move(new Vector2(0, -1f));

            //if (_ellapsedWatch.Elapsed.TotalSeconds >= Settings.Speed)
            //    ShowNextImage();

            else if (CurrentKeyboardState.IsKeyDown(Keys.OemPlus))
                Camera.SetZoom(.005f);
            else if (CurrentKeyboardState.IsKeyDown(Keys.OemMinus))
                Camera.SetZoom(-.005f);
            else if (IsKeyPressed(Keys.Right))
                ShowNextImage();
            else if (IsKeyPressed(Keys.Left))
                ShowPrevImage();
            else if (CurrentKeyboardState.IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }
        private void ShowPrevImage()
        {

            if (_loadingImage)
                return;

            if (_imageIdx <= 0)
                return;
            var image = ImageHelper.MirrorUpconvertImage(ImageFiles[--_imageIdx]);
            CurrentImage = Texture2D.FromStream(GraphicsDevice, new MemoryStream(image.Result));
            _ellapsedWatch.Restart();

        }
        private async Task ShowNextImage()
        {
            if (_loadingImage)
                return;


            if (_imageIdx >= ImageFiles.Count)
                _imageIdx = 0;

            byte[] image = await ImageHelper.MirrorUpconvertImage(ImageFiles[++_imageIdx]);
            //Camera.Reset();

            CurrentImage = Texture2D.FromStream(GraphicsDevice, new MemoryStream(image));
            startMove = true;
            _ellapsedWatch.Restart();

        }


        private double PercentToNext = 0;
        public float MaxZoom = 1.1f;
        public float MinZoom = .9f;
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            var zoomLerp = MathHelper.Lerp(MinZoom, MaxZoom, .1f);

            // _spriteBatch.Begin();
            //spriteBatch.Begin(
            //    SpriteSortMode.FrontToBack, 
            //BlendState.AlphaBlend, null, null, null, null,
            //         Camera.Transform);
            //     spriteBatch.Draw(CurrentImage,
            //         Vector2.Zero,
            //         Bounds,
            //         Color.White,
            //         0.0f,
            //         new Vector2(0, 0),
            //         0.5f,
            //         SpriteEffects.FlipHorizontally,
            //         0.0f);


            _spriteBatch.Begin(SpriteSortMode.BackToFront,
                BlendState.AlphaBlend,
                null,
                null,
                null,
                null,
                Camera.get_transformation(GraphicsDevice));


            string drawString = $"{ImageFiles[_imageIdx]}\r\n{Camera.GetString()}\r\n Target:{PanTarget1.X}x{PanTarget1.Y}";

            if (CurrentImage != null && !_loadingImage)
                _spriteBatch.Draw(CurrentImage, Vector2.Zero, Color.White);

            _spriteBatch.End();

            _spriteBatch.Begin();
            _spriteBatch.DrawString(SimpleFont, drawString, new Vector2(0, 3), Color.DarkGray);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

    }

}
