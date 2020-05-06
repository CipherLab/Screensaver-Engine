using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MonoGame.Utilities.Deflate;
using Nez;
using ScreenSaverHelper;
using SharedKernel;
using Color = Microsoft.Xna.Framework.Color;
using IDrawable = Nez.UI.IDrawable;
using Random = System.Random;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace MonoGameTest
{
    public class TilingScreenSaverComponent : Component, IUpdatable
    {

        private SpriteFont _simpleFont { get; set; }
        public Vector2 ImagePosition { get; set; }

        public Phase CurrentPhase = Phase.GetImage;
        public static Texture2D EmptyPixel;
        public Texture2D CurrentImage;
        private float MaxZoom = 1.1f;
        private float MinZoom = .9f;
        private int _extraPaddingAmountPx = 150;
        private int _imageIdx = 0;
        private bool _loadingImage;
        internal Vector2 CenterScreen => new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2f, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2f);

        #region fade stuff
        int alphaIncr = 3;
        public int mAlphaValue = 255;
        double mFadeDelay = 0;
        float fadeDelay = .055f;
        #endregion

        float _duration = 15.0f;
        float _elapsedTime = 0;

        private readonly GraphicsDevice _graphicsDevice;

        public void InitFont(SpriteFont simpleFont) 
        {
            _simpleFont = simpleFont;
        }

        public TilingScreenSaverComponent(GraphicsDevice graphicsDevice)
        {
            Enabled = true;
           // this._graphicsDevice = game.GraphicsDevice;
           _graphicsDevice = graphicsDevice;
            Bounds = new Rectangle(0, 0, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
                 GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
            //Bounds = new Rectangle(0, 0, 1920, 1280);
        

            Settings = new Settings();
            Settings.Load();

            ImageFiles = Directory
                .GetFiles(Settings.Path, "*.jpg", SearchOption.AllDirectories).ToList();
            if (ImageFiles.Count <= 0)
                return;

            if (Settings.Shuffle)
                ImageFiles = ImageFiles.Randomize().ToList();

            EmptyPixel = new Texture2D(this._graphicsDevice, 1, 1);

            EmptyPixel.SetData<Color>(new Color[] { Color.Black });

            //Camera = new Camera2d(Bounds.Width, Bounds.Height);
            //Camera = new Camera2d(Bounds.Width, Bounds.Height);
        }

        private Rectangle Bounds { get; }
        private string CurrentImageFile { get; set; }
        private KeyboardState CurrentKeyboardState { get; set; }
        private List<string> ImageFiles { get; }
        private Vector2 PanTarget { get; set; }
        private KeyboardState PreviousKeyboardState { get; set; }
        Random Rand => new Random(Environment.TickCount);

        private ISettings Settings { get; }
        private Texture2D TempImage { get; set; }

        public bool IsKeyPressed(Keys key)
        {
            return PreviousKeyboardState.IsKeyDown(key) && CurrentKeyboardState.IsKeyUp(key);
        }

        public override void Initialize()
        {
            Keyboard.GetState();
            base.Initialize();
        }


        protected void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private void FadeIn(float totalSeconds)
        {
            if (mAlphaValue <= 0)
            {
                return;
            }

            float dt = totalSeconds;
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
        private void FadeOut(float totalSeconds)
        {
            if (mAlphaValue >= 255)
            {
                return;
            }

            float dt = totalSeconds;
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

        private Texture2D GetNextImage()
        {
            _loadingImage = true;

            using (var imageHelper = new ImageHelper(new System.Drawing.Rectangle(0, 0, Bounds.Width + _extraPaddingAmountPx, Bounds.Height + _extraPaddingAmountPx), ImageFiles[_imageIdx]))
            {
                byte[] image = imageHelper.MirrorUpconvertImage();
                CurrentImageFile = ImageFiles[_imageIdx];

                _imageIdx++;
                if (_imageIdx >= ImageFiles.Count)
                    _imageIdx = 0;

                _loadingImage = false;
                return Texture2D.FromStream(_graphicsDevice, new MemoryStream(image));
            }

          
        }

        private Texture2D GetPrevImage()
        {
            _loadingImage = true;



            using (var imageHelper =
                new ImageHelper(
                    new System.Drawing.Rectangle(0, 0, Bounds.Width + _extraPaddingAmountPx,
                        Bounds.Height + _extraPaddingAmountPx), ImageFiles[_imageIdx]))
            {
                byte[] image = imageHelper.MirrorUpconvertImage();
                CurrentImageFile = ImageFiles[_imageIdx];

                _imageIdx--;

                if (_imageIdx <= 0)
                    _imageIdx = 0;

                _loadingImage = false;
                return Texture2D.FromStream(_graphicsDevice, new MemoryStream(image));
            }
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

        public void Update(GameTime gameTime, KeyboardState kState, KeyboardState lastKeyState)
        {
            CurrentKeyboardState = kState;
            PreviousKeyboardState = lastKeyState;

            //if (CurrentKeyboardState.IsKeyDown(Keys.D))
            //    Camera.Move(new Vector2(1f, 0));
            //if (CurrentKeyboardState.IsKeyDown(Keys.A))
            //    Camera.Move(new Vector2(-1f, 0));
            //if (CurrentKeyboardState.IsKeyDown(Keys.S))
            //    Camera.Move(new Vector2(0, 1f));
            //if (CurrentKeyboardState.IsKeyDown(Keys.W))
            //    Camera.Move(new Vector2(0, -1f));

            ////if (_ellapsedWatch.Elapsed.TotalSeconds >= Settings.Speed)
            ////    ShowNextImage();

            //else if (CurrentKeyboardState.IsKeyDown(Keys.OemPlus))
            //    Camera.SetZoom(.005f);
            //else if (CurrentKeyboardState.IsKeyDown(Keys.OemMinus))
            //    Camera.SetZoom(-.005f);
            //else if (IsKeyPressed(Keys.Right))
            //{
            //    _elapsedTime = 0;
            //    TempImage = GetNextImage();
            //}
            //else if (IsKeyPressed(Keys.Left))
            //{
            //    _elapsedTime = 0;
            //    TempImage = GetPrevImage();
            //}
            //else
            if (CurrentKeyboardState.IsKeyDown(Keys.Escape))
            {
               // Game.Exit();
            }

           // Update(gameTime);
        }

      
        public void Update()
        {
            float dt = Time.UnscaledDeltaTime;
            _elapsedTime += dt;
            switch (CurrentPhase)
            {

                case Phase.GetImage:
                    TempImage = GetNextImage();
                    if (!_loadingImage)
                    {
                        PanTarget = GetRandomTargetWithinExtraBounds();
                        //Camera.Reset();
                        CurrentImage = TempImage;
                        TempImage = null;
                        CurrentPhase = Phase.FadeIn;
                    }

                    break;
                case Phase.FadeIn:
                    FadeIn(dt);
                    if (mAlphaValue <= 0)
                    {
                        CurrentPhase = Phase.ShowImage;
                    }
                    break;

                case Phase.ShowImage:
                    if (_elapsedTime > _duration)// || Camera.Pos.X > PanTarget.X || Camera.Pos.Y > PanTarget.Y)
                    {
                        CurrentPhase = Phase.FadeOut;
                    }
                    break;
                case Phase.FadeOut:
                    FadeOut(dt);
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
                ImagePosition = Vector2.Lerp(CenterScreen, PanTarget, (float)Math.Pow(param / 2.0f, .5f));
                //var pos = Vector2.Lerp(Camera.CenterScreen, PanTarget, param);
                //Camera.SetPosition(pos);
            }

        


        }


        public void Draw(Batcher batcher, float x, float y, float width, float height, Color color)
        {
            //GraphicsDevice.Clear(Color.Black);
            var zoomLerp = MathHelper.Lerp(MinZoom, MaxZoom, .001f);

            //batcher.Begin(Camera.Transform);

            string drawString = $"{CurrentImageFile}\r\n Target:{PanTarget.X}x{PanTarget.Y}\r\nmAlphaValue:{mAlphaValue}\r\nPhase:{CurrentPhase.ToString()}";

            if (CurrentImage != null && !_loadingImage)
                batcher.Draw(CurrentImage, Vector2.Zero, Color.White);

            //_spriteBatch.Draw(CurrentImage, new Rectangle(0, 0, Bounds.Width, Bounds.Height),
            //    new Color(255, 255, 255, MathHelper.Clamp(mAlphaValue, 0, 255)));

            batcher.Draw(EmptyPixel, Vector2.Zero, Bounds, new Color(0, 0, 0,
                    MathHelper.Clamp(mAlphaValue, 0, 255)),
                0,
                Vector2.Zero,
                new Vector2(Bounds.Width, Bounds.Height),
                SpriteEffects.None, 0);

            batcher.End();

            //batcher.Begin();
            //batcher.DrawString(_simpleFont, drawString, new Vector2(0, 3), Color.DarkGray);
            //batcher.End();

        }

        public void Draw(GameTime gameTime, SpriteBatch _spriteBatch)
        {
          


        }


    }


}
