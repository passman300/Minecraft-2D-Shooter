using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

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

        private const int NUM_LEVELS = 5;

        // level title text
        private const string SCORE_TEXT = "Score: ";
        private const string LEVEL_TEXT = "Level ";
        private const string KILLS_TEXT = "Kills: ";
        private const string SHOTS_FIRED_TEXT = "Shots Fired: ";
        private const string SHOTS_HIT_TEXT = "Shots Hit: ";
        private const string HIT_PERCENT_TEXT = "Hit %: ";
        private const string CONTINUE_TEXT = "PRESS SPACE TO CONTINUE";

        // instruction text y location and title spacing
        private const int STATS_TEXT_Y = 210;
        private const int LEVEL_TEXT_Y = 270;
        private const int TITLE_SPACING_Y = 4;
        private const int TITLE_SPACING_X = 10;

        // title box width and height and opacity
        private const int STATS_BOX_WIDTH = SCREEN_WIDTH;
        private const int STATS_BOX_HEIGHT = 275;
        private const float STATS_BOX_OPACITY = 0.5f;

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

        // level stats
        private Vector2 levelStatsLoc = new Vector2(TITLE_SPACING_X, LEVEL_TEXT_Y);

        // level stats
        private Level[] level = new Level[NUM_LEVELS];
        private int curLevel = 0;
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

            for (int i = 0; i < NUM_LEVELS; i++)
            {
                level[i] = new Level(spriteBatch, i + 1);
                level[i].Load();
            }

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

            switch (gameState)
            {
                case MENU:
                    break;
                case STATS:
                    break;
                case GAMEPLAY:
                    UpdateGameplay(gameTime);
                    break;
                case LEVEL_STATS:
                    UpdateLevelStats();
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// update the gameplay of the whole game
        /// </summary>
        /// <param name="gameTime"></param> time passed within game
        private void UpdateGameplay(GameTime gameTime)
        {
            level[curLevel].Update(gameTime, player);

            if (level[curLevel].LevelState == Level.LevelStates.PostLevel)
            {
                gameState = LEVEL_STATS;
            }
        }


        private void UpdateLevelStats()
        {
            if (kb.IsKeyDown(Keys.Space) && !prevKb.IsKeyDown(Keys.Space))
            {
                curLevel++;
                gameState = GAMEPLAY;
            }
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
                    DrawLevelStats();
                    break;
            }

            DrawMouseLoc();

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawLevelStats()
        {
            spriteBatch.Draw(Assets.blankPixel, new Rectangle(0, STATS_TEXT_Y, STATS_BOX_WIDTH, STATS_BOX_HEIGHT), Color.Black * STATS_BOX_OPACITY);

            // draw all the stats
            spriteBatch.DrawString(Assets.minecraftEvening, SCORE_TEXT + player.Score, CenterTextX(Assets.minecraftEvening, SCORE_TEXT + " " + player.Score, STATS_TEXT_Y), Color.Yellow);

            for (int i = 0; i < NUM_LEVELS; i++)
            {
                spriteBatch.DrawString(Assets.minecraftRegular, LEVEL_TEXT + (i + 1) + " score: " + level[i].LevelScore, RightTextX((int)levelStatsLoc.Y + (i + 1) * (int)(TITLE_SPACING_Y + Assets.minecraftRegular.MeasureString(SCORE_TEXT).Y), 0f), Color.White);
            }

            spriteBatch.DrawString(Assets.minecraftRegular, LEVEL_TEXT + curLevel, RightTextX((int)levelStatsLoc.Y + (int)(TITLE_SPACING_Y + Assets.minecraftRegular.MeasureString(SCORE_TEXT).Y), 0.5f), Color.White);
            spriteBatch.DrawString(Assets.minecraftRegular, KILLS_TEXT + level[curLevel].LevelKills, RightTextX((int)levelStatsLoc.Y + 2 * (int)(TITLE_SPACING_Y + Assets.minecraftRegular.MeasureString(SCORE_TEXT).Y), 0.5f), Color.White);
            spriteBatch.DrawString(Assets.minecraftRegular, SHOTS_FIRED_TEXT + level[curLevel].LevelShotsFired, RightTextX((int)levelStatsLoc.Y + 3 * (int)(TITLE_SPACING_Y + Assets.minecraftRegular.MeasureString(SCORE_TEXT).Y), 0.5f), Color.White);
            spriteBatch.DrawString(Assets.minecraftRegular, SHOTS_HIT_TEXT + level[curLevel].LevelShotsHit, RightTextX((int)levelStatsLoc.Y + 4 * (int)(TITLE_SPACING_Y + Assets.minecraftRegular.MeasureString(SCORE_TEXT).Y), 0.5f), Color.White);
            if (level[curLevel].LevelShotsFired == 0)
                spriteBatch.DrawString(Assets.minecraftRegular, HIT_PERCENT_TEXT + 0, RightTextX((int)levelStatsLoc.Y + 5 * (int)(TITLE_SPACING_Y + Assets.minecraftRegular.MeasureString(SCORE_TEXT).Y), 0.5f), Color.White);
            else spriteBatch.DrawString(Assets.minecraftRegular, HIT_PERCENT_TEXT + 100 * level[curLevel].LevelShotsHit / level[curLevel].LevelShotsFired, RightTextX((int)levelStatsLoc.Y + 5 * (int)(TITLE_SPACING_Y + Assets.minecraftRegular.MeasureString(SCORE_TEXT).Y), 0.5f), Color.White);
        }

        private void DrawGameplay()
        {
            level[curLevel].Draw(player);
        }

        public static Vector2 CenterTextX(SpriteFont font, string text, int locY, float position = 0.5f)
        {
            return new Vector2(SCREEN_WIDTH * position - (font.MeasureString(text).X / 2), locY);
        }

        public static Vector2 RightTextX(int locY, float position = 0.5f)
        {
            return new Vector2(SCREEN_WIDTH * position + TITLE_SPACING_X, locY);
        }

        private void DrawMouseLoc()
        {
            spriteBatch.DrawString(Assets.debugFont, "Mouse: " + mouse.X + ", " + mouse.Y, new Vector2(mouse.X + 10, mouse.Y + 10), Color.White);
        }
    }
}
