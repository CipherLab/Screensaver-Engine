using Dapper;
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

namespace MonoGameTest
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;


        public static SpriteFont SimpleFont;
        public static Texture2D EmptyPixel;

        private KeyboardState LastKeyState;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 800;
            graphics.PreferredBackBufferWidth = 800;
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


            //PathFinder = new AStar(TileGrid, AllowDirection.NONDIAGONAL);
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

            LastKeyState = kState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            string drawString = $"{StepData.Count}";

            spriteBatch.DrawString(SimpleFont, drawString, new Vector2(0, 3), Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }
 
    }
  
}
