using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MonoGameTest.Scenes;
using Nez.ImGuiTools;
using Nez;

namespace MonoGameTest
{
    public class Game1 : Core
    {
        public Game1(int width = 1280, int height = 720, bool isFullScreen = false, bool enableEntitySystems = true,
            string windowTitle = "Nez", string contentDirectory = "Content") : base(width,height, isFullScreen, enableEntitySystems, windowTitle, contentDirectory)
        {

        }

        protected override void Initialize()
        {
            base.Initialize();
            
            //Scene = new PlatformerScene();
            //Scene = new BasicScene();
            Scene = new RigidBodyScene();
            //Scene = new FancyTilingScene();
            Window.AllowUserResizing = true;


        }
    }

    /*
    public class Game1 : Core
    {
  
        GraphicsDeviceManager graphics;
        SpriteBatch _spriteBatch;


        public static SpriteFont SimpleFont;

        private KeyboardState LastKeyState;

        private TilingScreenSaverComponent tilingScreenSaverComponent;
        public Game1()
        {
            var Bounds = new Rectangle(0, 0, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);

            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = Bounds.Height;
            graphics.PreferredBackBufferWidth = Bounds.Width;
            //graphics.IsFullScreen = true;

            IsMouseVisible = true;
            Content.RootDirectory = "Content";

           // var imGuiManager = new ImGuiManager();
           // Core.RegisterGlobalManager(imGuiManager);
        }

        protected override void Initialize()
        {
            LastKeyState = Keyboard.GetState();
            base.Initialize();
        }


        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            SimpleFont = Content.Load<SpriteFont>("SimpleFont");

            //ShowFancyTileImageFromFile(ImageFiles.First());
            tilingScreenSaverComponent = new TilingScreenSaverComponent(this, SimpleFont);
        }



        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        List<int[]> StepData = new List<int[]>();
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            KeyboardState kState = Keyboard.GetState();
            MouseState mState = Mouse.GetState();
            Vector2 mPos = new Vector2(mState.X, mState.Y);


            if (kState.IsKeyDown(Keys.F1) && LastKeyState.IsKeyUp(Keys.F1))
            {
            }


            //tilingScreenSaverComponent.Update(gameTime);
            tilingScreenSaverComponent.Update(gameTime, kState, LastKeyState);
            base.Update(gameTime);

            LastKeyState = kState;

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();

            string drawString = $"{StepData.Count}";

            _spriteBatch.DrawString(SimpleFont, drawString, new Vector2(0, 3), Color.White);
            _spriteBatch.End();
            base.Draw(gameTime);

            tilingScreenSaverComponent.Draw(gameTime, _spriteBatch);
        }

    }
    */
}