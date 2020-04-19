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
using MonoGame.Utilities.Deflate;
using ScreenSaverHelper;
using SharedKernel;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace MonoGameTest
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public Phase CurrentPhase = Phase.GetImage;
        public static Texture2D EmptyPixel;
        public static SpriteFont SimpleFont;
        public Texture2D CurrentImage;
        public float MaxZoom = 1.1f;
        public float MinZoom = .9f;
        int _extraPaddingAmountPx = 150;
        private int _imageIdx = 0;
        private bool _loadingImage;
        SpriteBatch _spriteBatch;

        #region fade stuff
        int alphaIncr = 3;
        int mAlphaValue = 255;
        double mFadeDelay = 0;
        float fadeDelay = .055f;
        #endregion

        float _duration = 15.0f;
        float _elapsedTime = 0;

        public Game1()
        {
            var graphics = new GraphicsDeviceManager(this);
            Bounds = new Rectangle(0, 0, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
                 GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
            //Bounds = new Rectangle(0, 0, 1920, 1280);
            ImageHelper = new ImageHelper(
                new System.Drawing.Rectangle(0, 0,
                    Bounds.Width + _extraPaddingAmountPx,
                    Bounds.Height + _extraPaddingAmountPx), true);

            Settings = new Settings();
            Settings.Load();

            ImageFiles = Directory
                .GetFiles(Settings.Path, "*.jpg", SearchOption.AllDirectories).ToList();
            if (ImageFiles.Count <= 0)
                return;

            if (Settings.Shuffle)
                ImageFiles = ImageFiles.Randomize().ToList();

            graphics.PreferredBackBufferHeight = Bounds.Height;
            graphics.PreferredBackBufferWidth = Bounds.Width;
            graphics.IsFullScreen = true;
            IsMouseVisible = false;
            Content.RootDirectory = "Content";

            Camera = new Camera2d(Bounds.Width, Bounds.Height);
        }

        public Camera2d Camera { get; }
        private Rectangle Bounds { get; }
        private string CurrentImageFile { get; set; }
        private KeyboardState CurrentKeyboardState { get; set; }
        private List<string> ImageFiles { get; }
        private ImageHelper ImageHelper { get; set; }
        private Vector2 PanTarget { get; set; }
        private KeyboardState PreviousKeyboardState { get; set; }
        Random Rand => new Random(Environment.TickCount);

        private ISettings Settings { get; }
        private Texture2D TempImage { get; set; }

        public bool IsKeyPressed(Keys key)
        {
            return PreviousKeyboardState.IsKeyDown(key) && CurrentKeyboardState.IsKeyUp(key);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            var zoomLerp = MathHelper.Lerp(MinZoom, MaxZoom, .001f);


            _spriteBatch.Begin(SpriteSortMode.BackToFront,
                BlendState.AlphaBlend,
                null,
                null,
                null,
                null,
                Camera.get_transformation(GraphicsDevice));

            string drawString = $"{CurrentImageFile}\r\n{Camera.GetString()}\r\n Target:{PanTarget.X}x{PanTarget.Y}\r\nmAlphaValue:{mAlphaValue}\r\nPhase:{CurrentPhase.ToString()}";

            if (CurrentImage != null && !_loadingImage)
                _spriteBatch.Draw(CurrentImage, Vector2.Zero, Color.White);

            //_spriteBatch.Draw(CurrentImage, new Rectangle(0, 0, Bounds.Width, Bounds.Height),
            //    new Color(255, 255, 255, MathHelper.Clamp(mAlphaValue, 0, 255)));

            _spriteBatch.Draw(EmptyPixel, Vector2.Zero, Bounds, new Color(0, 0, 0,
                MathHelper.Clamp(mAlphaValue, 0, 255)),
                0,
                Vector2.Zero,
                new Vector2(Bounds.Width, Bounds.Height),
                SpriteEffects.None, 0);

            _spriteBatch.End();

            _spriteBatch.Begin();
            _spriteBatch.DrawString(SimpleFont, drawString, new Vector2(0, 3), Color.DarkGray);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void Initialize()
        {
            Keyboard.GetState();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            SimpleFont = Content.Load<SpriteFont>("SimpleFont");

            EmptyPixel = new Texture2D(GraphicsDevice, 1, 1);

            EmptyPixel.SetData<Color>(new Color[] { Color.Black });

            //ShowFancyTileImageFromFile(ImageFiles.First());
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private void FadeIn(GameTime gameTime)
        {
            if (mAlphaValue <= 0)
            {
                return;
            }

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            mFadeDelay -= dt;

            //If the Fade delays has dropped below zero, then it is time to 
            //fade in/fade out the image a little bit more.
            if (mFadeDelay <= 0)
            {
                //Reset the Fade delay
                mFadeDelay = fadeDelay;

                //Increment/Decrement the fade value for the image
                mAlphaValue -= alphaIncr;
            }

        }
        private void FadeOut(GameTime gameTime)
        {
            if (mAlphaValue >= 255)
            {
                return;
            }

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            mFadeDelay -= dt;

            //If the Fade delays has dropped below zero, then it is time to 
            //fade in/fade out the image a little bit more.
            if (mFadeDelay <= 0)
            {
                //Reset the Fade delay
                mFadeDelay = fadeDelay;

                //Increment/Decrement the fade value for the image
                mAlphaValue += alphaIncr;
            }
        }
        protected override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _elapsedTime += dt;
            switch (CurrentPhase)
            {

                case Phase.GetImage:
                    TempImage = GetNextImage();
                    if (!_loadingImage)
                    {
                        PanTarget = GetRandomTargetWithinExtraBounds();
                        Camera.Reset();
                        CurrentImage = TempImage;
                        TempImage = null;
                        CurrentPhase = Phase.FadeIn;
                    }

                    break;
                case Phase.FadeIn:
                    FadeIn(gameTime);
                    if (mAlphaValue <= 0)
                    {
                        CurrentPhase = Phase.ShowImage;
                    }
                    break;

                case Phase.ShowImage:
                    if (_elapsedTime > _duration || Camera.Pos.X > PanTarget.X || Camera.Pos.Y > PanTarget.Y)
                    {
                        CurrentPhase = Phase.FadeOut;
                    }
                    break;
                case Phase.FadeOut:
                    FadeOut(gameTime);
                    if (mAlphaValue >= 255)
                    {
                        _elapsedTime = 0;
                        CurrentPhase = Phase.GetImage;
                    }

                    break;
            }

            if (CurrentPhase != Phase.GetImage &&
                !_loadingImage &&
                TempImage == null)
            {
                float param = _elapsedTime / _duration;
                var pos = Vector2.Lerp(Camera.CenterScreen, PanTarget, (float)Math.Pow(param / 2.0f, .5f));
                //var pos = Vector2.Lerp(Camera.CenterScreen, PanTarget, param);
                Camera.Move(pos);
            }

            PreviousKeyboardState = CurrentKeyboardState;
            CurrentKeyboardState = Keyboard.GetState();


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
            {
                _elapsedTime = 0;
                TempImage = GetNextImage();
            }
            else if (IsKeyPressed(Keys.Left))
            {
                _elapsedTime = 0;
                TempImage = GetPrevImage();
            }
            else if (CurrentKeyboardState.IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        private Texture2D GetNextImage()
        {
            _loadingImage = true;



            byte[] image = ImageHelper.MirrorUpconvertImage(ImageFiles[_imageIdx]);
            CurrentImageFile = ImageFiles[_imageIdx];

            _imageIdx++;
            if (_imageIdx >= ImageFiles.Count)
                _imageIdx = 0;

            _loadingImage = false;
            return Texture2D.FromStream(GraphicsDevice, new MemoryStream(image));
        }

        private Texture2D GetPrevImage()
        {
            _loadingImage = true;



            byte[] image = ImageHelper.MirrorUpconvertImage(ImageFiles[_imageIdx]);
            CurrentImageFile = ImageFiles[_imageIdx];

            _imageIdx--;

            if (_imageIdx <= 0)
                _imageIdx = 0;

            _loadingImage = false;
            return Texture2D.FromStream(GraphicsDevice, new MemoryStream(image));
        }

        private Vector2 GetRandomTargetWithinExtraBounds()
        {
            float xAmt = _extraPaddingAmountPx / 2f;
            float yAmt = _extraPaddingAmountPx / 2f;
            var dir = Rand.Next(0, 8);
            switch (dir)
            {
                case 0:
                    xAmt = 0;
                    break;
                case 1:
                    xAmt *= -1;
                    break;
                case 2:
                    yAmt = 0;
                    break;
                case 3:
                    yAmt *= -1;
                    break;

            }

            return new Vector2(((Bounds.Width + _extraPaddingAmountPx) / 2f) + xAmt, ((Bounds.Height + _extraPaddingAmountPx) / 2f) + yAmt);
        }

        public enum Phase
        {
            GetImage,
            FadeIn,
            FadeOut,
            ShowImage
        }
    }


}
