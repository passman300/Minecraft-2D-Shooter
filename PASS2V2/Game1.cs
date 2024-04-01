using Animation2D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;
using System.Security.Cryptography;

namespace PASS2V2
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        // Game states
        private const int MENU = 0;
        private const int STATS = 1;
        private const int GAMEPLAY = 2;
        private const int LEVEL_STATS = 3;

        // screen dimensions
        public const int SCREEN_WIDTH = 640;
        public const int SCREEN_HEIGHT = 640;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public static Random rng = new Random();

        // define input variables
        public static KeyboardState kb;
        public static KeyboardState prevKb;
        public static MouseState mouse;
        public static MouseState prevMouse;

        // game stats
        private int gameState = GAMEPLAY;

        // player
        private Player player;

        private Level level;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            // set screen dimensions to the defined width and height
            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;

            // apply the graphics change
            graphics.ApplyChanges();

            // set mouse to visible
            IsMouseVisible = true;


            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            Assets.Content = Content;
            Assets.Initialize();

            level = new Level(spriteBatch, 1);
            level.Load();

            player = new Player(spriteBatch);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            prevKb = kb;
            kb = Keyboard.GetState();

            prevMouse = mouse;
            mouse = Mouse.GetState();

            switch(gameState)
            {
                case MENU:
                    break;
                case STATS:
                    break;
                case GAMEPLAY:
                    UpdateGameplay(gameTime);
                    break;
                case LEVEL_STATS:
                    break;
            }

            base.Update(gameTime);
        }

        private void UpdateGameplay(GameTime gameTime)
        {
            level.Update(gameTime, player);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            // TODO: Add your drawing code here
            switch (gameState)
            {
                case MENU:
                    break;
                case STATS:
                    break;
                case GAMEPLAY:
                    DrawGameplay();
                    break;
                case LEVEL_STATS:
                    break;
            }

            spriteBatch.End();


            base.Draw(gameTime);
        }

        private void DrawGameplay()
        {
            level.Draw(player);
        }
    }
}
