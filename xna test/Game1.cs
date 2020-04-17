using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        string ImageTest1 = @"G:\AD\Amazon Drive\Pictures\backgrounds\0s7Faqh.jpg";
        string ImageTest2 = @"G:\AD\Amazon Drive\Pictures\backgrounds\0X3EHcT.jpg";

        public static SpriteFont SimpleFont;
        public static Texture2D EmptyPixel;

        private KeyboardState LastKeyState;

        private Rectangle Bounds { get; }
        private ISettings Settings { get; }
        private ImageHelper ImageHelper { get; set; }
        public Texture2D CurrentImage;
        Sprite ImageSprite;
        private List<string> ImageFiles { get; }
        private int imageIdx = 0;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Bounds = new Rectangle(0, 0, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
                 GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
            //Bounds = new Rectangle(0, 0, 1920, 1280);
            ImageHelper = new ImageHelper(new System.Drawing.Rectangle(0, 0, Bounds.Width, Bounds.Height), true);

            Settings = new Settings().LoadFromReg();

            ImageFiles = Directory
                .GetFiles(Settings.Path, "*.jpg", SearchOption.AllDirectories).ToList();
            if (ImageFiles.Count <= 0)
                return;

            if (Settings.Shuffle)
                ImageFiles = ImageFiles.Randomize().ToList();

            graphics.PreferredBackBufferHeight = Bounds.Height;
            graphics.PreferredBackBufferWidth = Bounds.Width;
            graphics.IsFullScreen = true;
            IsMouseVisible = true;
            Content.RootDirectory = "Content";

        }

        protected override void Initialize()
        {
            LastKeyState = Keyboard.GetState();
            base.Initialize();
        }


        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            SimpleFont = Content.Load<SpriteFont>("SimpleFont");

            EmptyPixel = new Texture2D(GraphicsDevice, 1, 1);
            EmptyPixel.SetData<Color>(new Color[] { Color.White });

            ShowFancyTileImageFromFile(ImageFiles.First());

            this.EllapsedWatch.Start();
        }

        private void ShowFancyTileImageFromFile(string f)
        {
            LoadingImage = true;
            try
            {
                var image = ImageHelper.MirrorUpconvertImage(f);
                CurrentImage = Texture2D.FromStream(GraphicsDevice, new MemoryStream(image));
            }
            finally
            {
                LoadingImage = false;
            }
        }


        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private bool LoadingImage = false;
        List<int[]> StepData = new List<int[]>();
        private KeyboardState previousKeyboardState { get; set; }
        private KeyboardState currentKeyboardState { get; set; }
        public bool isKeyPressed(Keys key)
        {
            return previousKeyboardState.IsKeyDown(key) && currentKeyboardState.IsKeyUp(key);
        }

        Stopwatch EllapsedWatch = new Stopwatch();
        protected override void Update(GameTime gameTime)
        {
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
            // Move our sprite based on arrow keys being pressed:
            if (EllapsedWatch.Elapsed.TotalSeconds >= Settings.Speed)
                ShowNextImage();
            else if (isKeyPressed(Keys.Right))
                ShowNextImage();
            else if (isKeyPressed(Keys.Left))
                ShowPrevImage();
            else if (isKeyPressed(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }
        private void ShowPrevImage()
        {

            if (LoadingImage)
                return;

            if (imageIdx <= 0)
                return;
            var image = ImageHelper.MirrorUpconvertImage(ImageFiles[--imageIdx]);
            CurrentImage = Texture2D.FromStream(GraphicsDevice, new MemoryStream(image));
            EllapsedWatch.Restart();

        }
        private void ShowNextImage()
        {
            if (LoadingImage)
                return;

            if (imageIdx >= ImageFiles.Count)
                imageIdx = 0;

            var image = ImageHelper.MirrorUpconvertImage(ImageFiles[++imageIdx]);
            CurrentImage = Texture2D.FromStream(GraphicsDevice, new MemoryStream(image));
            EllapsedWatch.Restart();

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            string drawString = $"{StepData.Count}";

            if (CurrentImage != null && !LoadingImage)
                spriteBatch.Draw(CurrentImage, Vector2.Zero, Color.White);

            spriteBatch.DrawString(SimpleFont, drawString, new Vector2(0, 3), Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }

    }

}
