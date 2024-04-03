using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.CodeDom;
using System.Runtime.InteropServices;

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
        private const int SHOP = 4;

        // screen dimensions
        public const int SCREEN_WIDTH = 640;
        public const int SCREEN_HEIGHT = 640;

        // number of menu buttons and their indexes
        private const int MENU_BUTTON_NUM = 3;
        private const int PLAY_BUTTON_INDEX = 0;
        private const int STATS_BUTTON_INDEX = 1;
        private const int EXIT_BUTTON_INDEX = 2;

        // menu button spacing, base location, and dimensions
        private const int MENU_BUTTON_Y = 100;
        private const int MENU_BUTTON_SPACER_Y = 4;
        private const int MENU_BUTTON_WIDTH = 400;
        private const int MENU_BUTTON_HEIGHT = 127;

        // number of levels
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

        // index of the shop upgrades buttons (rectangles)
        private const int SHOP_NUM_UPGRADES = 4;

        // y location of the score text in the shop menu
        private const int SCORE_TEXT_Y = 420;

        // shop button spacing, dimensions, and opacity
        private const int SHOP_BUTTON_SPACING_Y = 30;
        private const int SHOP_BUTTON_WIDTH = 300;
        private const int SHOP_BUTTON_HEIGHT = 68;
        private const float SHOP_BUTTON_USED_OPACITY = 0.5f;

        // cost of each upgrade
        private const int SHOP_SPEED_COST = 100;
        private const int SHOP_DAMAGE_COST = 200;
        private const int SHOP_FIRE_COST = 300;
        private const int SHOP_POINTS_COST = 500;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public static Random rng = new Random();

        // define input variables
        public static KeyboardState kb;
        public static KeyboardState prevKb;
        public static MouseState mouse;
        public static MouseState prevMouse;

        // game stats
        private int gameState = MENU;

        // list of buttons in the menu
        private Rectangle[] menuButtonRecs = new Rectangle[MENU_BUTTON_NUM];
        private Button[] menuButtons = new Button[MENU_BUTTON_NUM];

        // player
        private Player player;

        // level stats
        private Vector2 levelStatsLoc = new Vector2(TITLE_SPACING_X, LEVEL_TEXT_Y);

        // level stats
        private Level[] level = new Level[NUM_LEVELS];
        private int curLevel = 0;

        // shop upgrades selection and rectangles
        private Rectangle[] shopButtonRecs = new Rectangle[SHOP_NUM_UPGRADES];
        private Texture2D[] shopButtonImgs = new Texture2D[SHOP_NUM_UPGRADES];
        private int[] shopCost = new int[SHOP_NUM_UPGRADES];

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

            // load all content
            Assets.Content = Content;
            Assets.Initialize();

            // load menu buttons rectangles and buttons
            menuButtonRecs[PLAY_BUTTON_INDEX] = new Rectangle((int)CenterRectangleX(MENU_BUTTON_WIDTH, MENU_BUTTON_Y).X, MENU_BUTTON_Y, MENU_BUTTON_WIDTH, MENU_BUTTON_HEIGHT);
            menuButtonRecs[STATS_BUTTON_INDEX] = new Rectangle((int)CenterRectangleX(MENU_BUTTON_WIDTH, MENU_BUTTON_Y + MENU_BUTTON_SPACER_Y + MENU_BUTTON_HEIGHT).X, MENU_BUTTON_Y + MENU_BUTTON_SPACER_Y + MENU_BUTTON_HEIGHT, MENU_BUTTON_WIDTH, MENU_BUTTON_HEIGHT);
            menuButtonRecs[EXIT_BUTTON_INDEX] = new Rectangle((int)CenterRectangleX(MENU_BUTTON_WIDTH, (MENU_BUTTON_SPACER_Y + MENU_BUTTON_HEIGHT) * EXIT_BUTTON_INDEX).X, MENU_BUTTON_Y + (MENU_BUTTON_SPACER_Y + MENU_BUTTON_HEIGHT) * EXIT_BUTTON_INDEX, MENU_BUTTON_WIDTH, MENU_BUTTON_HEIGHT);

            // load menu buttons
            menuButtons[PLAY_BUTTON_INDEX] = new Button(Assets.buttonImg, menuButtonRecs[PLAY_BUTTON_INDEX], Color.White);
            menuButtons[STATS_BUTTON_INDEX] = new Button(Assets.buttonImg, menuButtonRecs[STATS_BUTTON_INDEX], Color.White);
            menuButtons[EXIT_BUTTON_INDEX] = new Button(Assets.buttonImg, menuButtonRecs[EXIT_BUTTON_INDEX], Color.White);

            // intilize the button hovers and clicks
            menuButtons[PLAY_BUTTON_INDEX].Clicked += PlayButtonClick;
            menuButtons[STATS_BUTTON_INDEX].Clicked += PlayButtonClick;
            menuButtons[STATS_BUTTON_INDEX].Clicked += PlayButtonClick;




            // load and initialize all levels
            InitializeLevels();

            // load player
            player = new Player(spriteBatch);

            // load shop buttons
            shopButtonRecs[Player.DOUBLE_SPEED_INDEX] = new Rectangle((int)CenterRectangleX(SHOP_BUTTON_WIDTH, SCREEN_HEIGHT / 2 - SHOP_BUTTON_SPACING_Y / 2 - SHOP_BUTTON_HEIGHT, 0.25f).X, SCREEN_HEIGHT / 2 - SHOP_BUTTON_SPACING_Y / 2 - SHOP_BUTTON_HEIGHT, SHOP_BUTTON_WIDTH, SHOP_BUTTON_HEIGHT);
            shopButtonRecs[Player.TRIPLE_DAMAGE_INDEX] = new Rectangle((int)CenterRectangleX(SHOP_BUTTON_WIDTH, SCREEN_HEIGHT / 2 - SHOP_BUTTON_SPACING_Y / 2, 0.25f).X, SCREEN_HEIGHT / 2 + SHOP_BUTTON_SPACING_Y / 2, SHOP_BUTTON_WIDTH, SHOP_BUTTON_HEIGHT);
            shopButtonRecs[Player.DOUBLE_FIRE_RATE_INDEX] = new Rectangle((int)CenterRectangleX(SHOP_BUTTON_WIDTH, SCREEN_HEIGHT / 2 - SHOP_BUTTON_SPACING_Y / 2 - SHOP_BUTTON_HEIGHT, 0.75f).X, SCREEN_HEIGHT / 2 - SHOP_BUTTON_SPACING_Y / 2 - SHOP_BUTTON_HEIGHT, SHOP_BUTTON_WIDTH, SHOP_BUTTON_HEIGHT);
            shopButtonRecs[Player.DOUBLE_POINTS_INDEX] = new Rectangle((int)CenterRectangleX(SHOP_BUTTON_WIDTH, SCREEN_HEIGHT / 2 - SHOP_BUTTON_SPACING_Y / 2 - SHOP_BUTTON_HEIGHT, 0.75f).X, SCREEN_HEIGHT / 2 + SHOP_BUTTON_SPACING_Y / 2, SHOP_BUTTON_WIDTH, SHOP_BUTTON_HEIGHT);

            // load shop button textures
            shopButtonImgs[Player.DOUBLE_SPEED_INDEX] = Assets.speedShopImg;
            shopButtonImgs[Player.TRIPLE_DAMAGE_INDEX] = Assets.damageShopImg;
            shopButtonImgs[Player.DOUBLE_FIRE_RATE_INDEX] = Assets.fireRateShopImg;
            shopButtonImgs[Player.DOUBLE_POINTS_INDEX] = Assets.pointMutiShopImg;

            // load shop cost
            shopCost[Player.DOUBLE_SPEED_INDEX] = SHOP_SPEED_COST;
            shopCost[Player.TRIPLE_DAMAGE_INDEX] = SHOP_DAMAGE_COST;
            shopCost[Player.DOUBLE_FIRE_RATE_INDEX] = SHOP_FIRE_COST;
            shopCost[Player.DOUBLE_POINTS_INDEX] = SHOP_POINTS_COST;
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
                    UpdateMenu();
                    break;
                case STATS:
                    break;
                case GAMEPLAY:
                    UpdateGameplay(gameTime);
                    break;
                case LEVEL_STATS:
                    UpdateLevelStats();
                    break;
                case SHOP:
                    UpdateShop();
                    break;
            }

            base.Update(gameTime);
        }

        private void UpdateMenu()
        {

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
                // check if last level
                if (curLevel == NUM_LEVELS - 1)
                {
                    // TODO:check if player has beaten high score


                    // end game
                    gameState = MENU;

                    // TODO:save stats
                }
                else
                {
                    curLevel++;
                    gameState = SHOP;

                    // DEBUG: REMOVE
                    player.Score = 1000;
                }
            }
        }

        private void UpdateShop()
        {
            for (int i = 0; i < shopButtonRecs.Length; i++)
            {
                // check if the button is clicked, if the upgrade isn't used, and if player can afford it
                if (IsClickRectangle(shopButtonRecs[i]) && !player.UsedBuffs[i] && player.Score >= shopCost[i])
                {
                    player.UsedBuffs[i] = true;
                    player.AddBuff(i); // same index of buffs in player

                    // update score
                    player.Score -= shopCost[i];
                }
            }

            // check if the player is done with the shop
            if (kb.IsKeyDown(Keys.Space) && !prevKb.IsKeyDown(Keys.Space))
            {
                gameState = GAMEPLAY;
            }
        }

        private void PlayButtonHover()
        {

        }

        private void PlayButtonClick()
        {
            gameState = GAMEPLAY;
            curLevel = 0; // note the current level is at the zero index in the level array, but the player starts at level 1

            player.ResetBuffs();
            player.Score = 0;

            // re load level
            InitializeLevels();
        }

        private void InitializeLevels()
        {
            // load and initialize all levels
            for (int i = 0; i < NUM_LEVELS; i++)
            {
                level[i] = new Level(spriteBatch, i + 1);
                level[i].Load();
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
                case SHOP:
                    DrawShop();
                    break;
            }

            DrawMouseLoc();

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawLevelStats()
        {
            // draw the level
            level[curLevel].Draw(player);

            // draw stats box
            spriteBatch.Draw(Assets.blankPixel, new Rectangle(0, STATS_TEXT_Y, STATS_BOX_WIDTH, STATS_BOX_HEIGHT), Color.Black * STATS_BOX_OPACITY);

            // draw all the stats
            spriteBatch.DrawString(Assets.minecraftEvening, SCORE_TEXT + player.Score, CenterTextX(Assets.minecraftEvening, SCORE_TEXT + " " + player.Score, STATS_TEXT_Y), Color.Yellow);

            for (int i = 0; i < NUM_LEVELS; i++)
            {
                spriteBatch.DrawString(Assets.minecraftRegular, LEVEL_TEXT + (i + 1) + " score: " + level[i].LevelScore, RightTextX((int)levelStatsLoc.Y + (i) * (int)(TITLE_SPACING_Y + Assets.minecraftRegular.MeasureString(SCORE_TEXT).Y), 0f), Color.White);
            }

            spriteBatch.DrawString(Assets.minecraftRegular, LEVEL_TEXT + (curLevel + 1), RightTextX((int)levelStatsLoc.Y, 0.5f), Color.White);
            spriteBatch.DrawString(Assets.minecraftRegular, KILLS_TEXT + level[curLevel].LevelKills, RightTextX((int)levelStatsLoc.Y + 1 * (int)(TITLE_SPACING_Y + Assets.minecraftRegular.MeasureString(SCORE_TEXT).Y), 0.5f), Color.White);
            spriteBatch.DrawString(Assets.minecraftRegular, SHOTS_FIRED_TEXT + level[curLevel].LevelShotsFired, RightTextX((int)levelStatsLoc.Y + 2 * (int)(TITLE_SPACING_Y + Assets.minecraftRegular.MeasureString(SCORE_TEXT).Y), 0.5f), Color.White);
            spriteBatch.DrawString(Assets.minecraftRegular, SHOTS_HIT_TEXT + level[curLevel].LevelShotsHit, RightTextX((int)levelStatsLoc.Y + 3 * (int)(TITLE_SPACING_Y + Assets.minecraftRegular.MeasureString(SCORE_TEXT).Y), 0.5f), Color.White);
            if (level[curLevel].LevelShotsFired == 0)
                spriteBatch.DrawString(Assets.minecraftRegular, HIT_PERCENT_TEXT + 0, RightTextX((int)levelStatsLoc.Y + 4 * (int)(TITLE_SPACING_Y + Assets.minecraftRegular.MeasureString(SCORE_TEXT).Y), 0.5f), Color.White);
            else spriteBatch.DrawString(Assets.minecraftRegular, HIT_PERCENT_TEXT + 100 * level[curLevel].LevelShotsHit / level[curLevel].LevelShotsFired, RightTextX((int)levelStatsLoc.Y + 4 * (int)(TITLE_SPACING_Y + Assets.minecraftRegular.MeasureString(SCORE_TEXT).Y), 0.5f), Color.White);
            spriteBatch.DrawString(Assets.minecraftBold, CONTINUE_TEXT, CenterTextX(Assets.minecraftBold, CONTINUE_TEXT, (int)(levelStatsLoc.Y + 5 * (int)(TITLE_SPACING_Y + Assets.minecraftRegular.MeasureString(SCORE_TEXT).Y))), Color.Yellow);
            


        }

        private void DrawGameplay()
        {
            level[curLevel].Draw(player);
        }

        private void DrawShop()
        {
            // draw back ground
            spriteBatch.Draw(Assets.menuImg3, Vector2.Zero, Color.White);

            // draw stats box
            spriteBatch.Draw(Assets.blankPixel, new Rectangle(0, STATS_TEXT_Y, STATS_BOX_WIDTH, STATS_BOX_HEIGHT), Color.Black * STATS_BOX_OPACITY);

            // draw all the shop buttons
            for (int i = 0; i < SHOP_NUM_UPGRADES; i++)
            {
                if (player.UsedBuffs[i])
                    spriteBatch.Draw(shopButtonImgs[i], shopButtonRecs[i], Color.White * 0.5f);
                else spriteBatch.Draw(shopButtonImgs[i], new Rectangle(shopButtonRecs[i].X, shopButtonRecs[i].Y, shopButtonRecs[i].Width, shopButtonRecs[i].Height), Color.White);
            }

            // draw the current player score on the button
            spriteBatch.DrawString(Assets.minecraftBold, SCORE_TEXT + player.Score, CenterTextX(Assets.minecraftBold, SCORE_TEXT + player.Score, SCORE_TEXT_Y), Color.Yellow);
        }


        private bool IsClickRectangle(Rectangle rectangle)
        {
            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed && rectangle.Contains(mouse.Position) )
            {
                return true;
            }
            return false;
        }

        public static Vector2 CenterTextX(SpriteFont font, string text, int locY, float position = 0.5f)
        {
            return new Vector2(SCREEN_WIDTH * position - (font.MeasureString(text).X / 2), locY);
        }

        public static Vector2 RightTextX(int locY, float position = 0.5f)
        {
            return new Vector2(SCREEN_WIDTH * position + TITLE_SPACING_X, locY);
        }

        public static Vector2 CenterRectangleX(Rectangle rec, int locY, float position = 0.5f)
        {
            return new Vector2(SCREEN_WIDTH * position - (rec.Width / 2), locY);
        }
        public static Vector2 CenterRectangleX(int width, int locY, float position = 0.5f)
        {
            return new Vector2(SCREEN_WIDTH * position - (width / 2), locY);
        }

        private void DrawMouseLoc()
        {
            spriteBatch.DrawString(Assets.debugFont, "Mouse: " + mouse.X + ", " + mouse.Y, new Vector2(mouse.X + 10, mouse.Y + 10), Color.White);
        }
    }
}
