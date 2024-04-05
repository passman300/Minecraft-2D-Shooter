using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private const int MENU_BUTTON_Y = 220;
        private const int MENU_BUTTON_SPACER_Y = 4;
        private const int MENU_BUTTON_WIDTH = 400;
        private const int MENU_BUTTON_HEIGHT = 127;

        // menu button text offset
        private const int MENU_BUTTON_TEXT_OFFSET_X = -5;
        private const int MENU_BUTTON_TEXT_OFFSET_Y = 5;

        // number of levels
        private const int NUM_LEVELS = 5;

        // instruction text y location and title spacing
        private const int STATS_TEXT_Y = 210;
        private const int LEVEL_TEXT_Y = 270;
        private const int TITLE_SPACING_Y = 4;
        private const int TITLE_SPACING_X = 10;

        // title box width and height and opacity
        private const int STATS_BOX_WIDTH = SCREEN_WIDTH;
        private const int STATS_BOX_HEIGHT = 300;
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

        // stats menu file path
        private const string STATS_FILE_PATH = "stats.txt";
        private const string STATS_TEMPLATE = "0\n0\n0,0,0\n0,0,0,0,0";

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public static Random rng = new Random();

        // define input variables
        public static KeyboardState kb;
        public static KeyboardState prevKb;
        public static MouseState mouse;
        public static MouseState prevMouse;

        private StreamReader inFile;
        private StreamWriter outFile;

        // game stats
        private int gameState = MENU;

        // list of buttons in the menu
        private Rectangle[] menuButtonRecs = new Rectangle[MENU_BUTTON_NUM];
        private Button[] menuButtons = new Button[MENU_BUTTON_NUM];
        private string[] menuButtonText = { "PLAY", "STATS", "EXIT" };

        // player
        private Player player;

        // level stats
        private Vector2 levelStatsLoc = new Vector2(TITLE_SPACING_X, LEVEL_TEXT_Y);

        // is high score
        private bool isHighScore = false;

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

            // load and initialize menu buttons
            LoadButtons();

            // load and initialize all levels
            InitializeLevels();

            // load player
            player = new Player(spriteBatch);

            // load stats
            ReadStats();

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
        /// load menu buttons
        /// </summary>
        private void LoadButtons()
        {
            // load menu buttons rectangles and buttons
            menuButtonRecs[PLAY_BUTTON_INDEX] = new Rectangle((int)CenterRectangleX(MENU_BUTTON_WIDTH, MENU_BUTTON_Y).X, MENU_BUTTON_Y, MENU_BUTTON_WIDTH, MENU_BUTTON_HEIGHT);
            menuButtonRecs[STATS_BUTTON_INDEX] = new Rectangle((int)CenterRectangleX(MENU_BUTTON_WIDTH, MENU_BUTTON_Y + MENU_BUTTON_SPACER_Y + MENU_BUTTON_HEIGHT).X, MENU_BUTTON_Y + MENU_BUTTON_SPACER_Y + MENU_BUTTON_HEIGHT, MENU_BUTTON_WIDTH, MENU_BUTTON_HEIGHT);
            menuButtonRecs[EXIT_BUTTON_INDEX] = new Rectangle((int)CenterRectangleX(MENU_BUTTON_WIDTH, (MENU_BUTTON_SPACER_Y + MENU_BUTTON_HEIGHT) * EXIT_BUTTON_INDEX).X, MENU_BUTTON_Y + (MENU_BUTTON_SPACER_Y + MENU_BUTTON_HEIGHT) * EXIT_BUTTON_INDEX, MENU_BUTTON_WIDTH, MENU_BUTTON_HEIGHT);

            // load menu buttons
            menuButtons[PLAY_BUTTON_INDEX] = new Button(Assets.buttonImg, menuButtonRecs[PLAY_BUTTON_INDEX], Color.White);
            menuButtons[STATS_BUTTON_INDEX] = new Button(Assets.buttonImg, menuButtonRecs[STATS_BUTTON_INDEX], Color.White);
            menuButtons[EXIT_BUTTON_INDEX] = new Button(Assets.buttonImg, menuButtonRecs[EXIT_BUTTON_INDEX], Color.White);

            // set the text as if it was not hovered
            PlayButtonExit();
            StatsButtonExit();
            ExitButtonExit();

            // initialize the button hovers and clicks
            menuButtons[PLAY_BUTTON_INDEX].Clicked += PlayButtonClick;
            menuButtons[PLAY_BUTTON_INDEX].HoverEnter += () => PlayButtonHover();
            menuButtons[PLAY_BUTTON_INDEX].HoverExit += () => PlayButtonExit();
            menuButtons[STATS_BUTTON_INDEX].Clicked += StatsButtonClick;
            menuButtons[STATS_BUTTON_INDEX].HoverEnter += () => StatsButtonHover();
            menuButtons[STATS_BUTTON_INDEX].HoverExit += () => StatsButtonExit();
            menuButtons[EXIT_BUTTON_INDEX].Clicked += ExitButtonClick;
            menuButtons[EXIT_BUTTON_INDEX].HoverEnter += () => ExitButtonHover();
            menuButtons[EXIT_BUTTON_INDEX].HoverExit += () => ExitButtonExit();
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

            // update the keyboard
            prevKb = kb;
            kb = Keyboard.GetState();

            // update the mouse
            prevMouse = mouse;
            mouse = Mouse.GetState();

            // update based on the current game state
            switch (gameState)
            {
                case MENU:
                    UpdateMenu();
                    break;
                case STATS:
                    UpdateStats();
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

        /// <summary>
        /// update the main menu, update the buttons
        /// </summary>
        private void UpdateMenu()
        {
            for (int i = 0; i < MENU_BUTTON_NUM; i++)
            {
                menuButtons[i].Update(mouse);
            }
        }

        private void UpdateStats()
        {
            // check if space is pressed, player wants to return to menu
            if (kb.IsKeyDown(Keys.Space) && !prevKb.IsKeyDown(Keys.Space))
                gameState = MENU;
        }

        /// <summary>
        /// update the gameplay of the whole game
        /// </summary>
        /// <param name="gameTime"></param> time passed within game
        private void UpdateGameplay(GameTime gameTime)
        {
            // update the level
            level[curLevel].Update(gameTime, player);

            // check if level is over
            if (level[curLevel].LevelState == Level.LevelStates.PostLevel)
            {
                gameState = LEVEL_STATS;

                // calculate level hit percent
                level[curLevel].LevelHitPercent = (level[curLevel].LevelShotsFired == 0) ? 0 : (float)(level[curLevel].LevelShotsHit / level[curLevel].LevelShotsFired) * 100;
            }
        }


        private void UpdateLevelStats()
        {
            // check if space is pressed, player wants to play next level or return to menu
            if (kb.IsKeyDown(Keys.Space) && !prevKb.IsKeyDown(Keys.Space))
            {
                // check if last level
                if (curLevel == NUM_LEVELS - 1)
                {
                    // check if player has beaten high score
                    if (player.Score > player.HighScore && curLevel == NUM_LEVELS - 1)
                    {
                        player.HighScore = player.Score;
                        isHighScore = true; // show high score
                    }

                    // increment games played
                    player.GamesPlayed++;

                    // end game, return to menu
                    gameState = MENU;

                    float totalHitPercent = 0;

                    // calculate total hit percent
                    for (int i = 0; i < NUM_LEVELS; i++)
                    {
                        totalHitPercent += level[i].LevelHitPercent;
                    }

                    // check if player has new top hit percent
                    if (player.TopHitPercent < totalHitPercent)
                    {
                        player.TopHitPercent = totalHitPercent;
                    }

                    // save stats
                    SaveStats();
                }
                else // go to next level
                {
                    curLevel++;
                    gameState = SHOP;
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
            if (kb.IsKeyDown(Keys.Space) && !prevKb.IsKeyDown(Keys.Space)) gameState = GAMEPLAY;
        }

        /// <summary>
        /// function when play button is hovered
        /// </summary>
        private void PlayButtonHover()
        {
            menuButtons[PLAY_BUTTON_INDEX].SetShadowText(MENU_BUTTON_TEXT_OFFSET_X, MENU_BUTTON_TEXT_OFFSET_Y, Color.Black);
            menuButtons[PLAY_BUTTON_INDEX].SetText(Assets.minecraftBold, menuButtonText[PLAY_BUTTON_INDEX], Color.Yellow);
        }

        /// <summary>
        /// function when play button is exited
        /// </summary>
        private void PlayButtonExit()
        {
            menuButtons[PLAY_BUTTON_INDEX].SetShadowText(0, 0, Color.White);
            menuButtons[PLAY_BUTTON_INDEX].SetText(Assets.minecraftBold, menuButtonText[PLAY_BUTTON_INDEX], Color.Black);
        }

        /// <summary>
        /// function when play button is clicked
        /// </summary>
        private void PlayButtonClick()
        {
            // set the game state to gameplay
            gameState = GAMEPLAY;
            curLevel = 0; // note the current level is at the zero index in the level array, but the player starts at level 1

            // reset player
            player.ResetBuffs();
            player.Score = 0;

            isHighScore = false;

            // reset level
            ResetLevels();
        }

        /// <summary>
        /// function when stats button is clicked
        /// </summary>
        private void StatsButtonClick()
        {
            gameState = STATS;
        }

        /// <summary>
        /// function when stats button is hovered
        /// </summary>
        private void StatsButtonHover()
        {
            menuButtons[STATS_BUTTON_INDEX].SetShadowText(MENU_BUTTON_TEXT_OFFSET_X, MENU_BUTTON_TEXT_OFFSET_Y, Color.Black);
            menuButtons[STATS_BUTTON_INDEX].SetText(Assets.minecraftBold, menuButtonText[STATS_BUTTON_INDEX], Color.Yellow);
        }

        /// <summary>
        /// function when stats button is exited
        /// </summary>
        private void StatsButtonExit()
        {
            menuButtons[STATS_BUTTON_INDEX].SetShadowText(0, 0, Color.White);
            menuButtons[STATS_BUTTON_INDEX].SetText(Assets.minecraftBold, menuButtonText[STATS_BUTTON_INDEX], Color.Black);
        }

        /// <summary>
        /// function when exit button is clicked
        /// </summary>
        private void ExitButtonClick()
        {
            Exit(); // exit the game
        }

        /// <summary>
        /// function when exit button is hovered
        /// </summary>
        private void ExitButtonHover()
        {
            menuButtons[EXIT_BUTTON_INDEX].SetShadowText(MENU_BUTTON_TEXT_OFFSET_X, MENU_BUTTON_TEXT_OFFSET_Y, Color.Black);
            menuButtons[EXIT_BUTTON_INDEX].SetText(Assets.minecraftBold, menuButtonText[EXIT_BUTTON_INDEX], Color.Yellow);
        }

        /// <summary>
        /// function when exit button is exited
        /// </summary>
        private void ExitButtonExit()
        {
            menuButtons[EXIT_BUTTON_INDEX].SetShadowText(0, 0, Color.White);
            menuButtons[EXIT_BUTTON_INDEX].SetText(Assets.minecraftBold, menuButtonText[EXIT_BUTTON_INDEX], Color.Black);
        }

        private void ReadStats()
        {
            try
            {
                // open the level tile file
                inFile = File.OpenText(STATS_FILE_PATH);


                player.HighScore = int.Parse(inFile.ReadLine());
                player.GamesPlayed = int.Parse(inFile.ReadLine());

                float[] shotsLine = inFile.ReadLine().Split(',').Select(x => float.Parse(x)).ToArray(); // cool array parsing
                player.ShotsFired = (int)shotsLine[0];
                player.ShotsHit = (int)shotsLine[1];
                player.TopHitPercent = shotsLine[2];

                // read the 
                int[] killLine = inFile.ReadLine().Split(',').Select(x => int.Parse(x)).ToArray();
                player.MobsKilled[Player.VILLAGER_KILL_INDEX] = killLine[0];
                player.MobsKilled[Player.SKELETON_KILL_INDEX] = killLine[1];
                player.MobsKilled[Player.SKELETON_KILL_INDEX] = killLine[2];
                player.MobsKilled[Player.PILLAGER_KILL_INDEX] = killLine[3];
                player.MobsKilled[Player.ENDERMAN_KILL_INDEX] = killLine[4];

                player.CalculateExtraStats(); // calculate extra stats (total kills, all time hit percentage, average shots per game, average hit percentage)

                inFile.Close();
            }
            catch (IndexOutOfRangeException) // cases where line is read as too many values
            {
                inFile.Close();

                outFile = File.CreateText(STATS_FILE_PATH);

                // set everything to 0 if there is an error with blank template
                outFile.Write(STATS_TEMPLATE);

                outFile.Close();

                ReadStats();
            }
            catch (FileNotFoundException) // cases where file is not found
            {
                inFile.Close();

                outFile = File.CreateText(STATS_FILE_PATH);

                // set everything to 0 if there is an error with blank template
                outFile.Write(STATS_TEMPLATE);

                outFile.Close();

                ReadStats();
            }
            catch (NullReferenceException) // cases where line is read as null
            {
                inFile.Close();

                outFile = File.CreateText(STATS_FILE_PATH);

                // set everything to 0 if there is an error with blank template
                outFile.Write(STATS_TEMPLATE);

                outFile.Close();

                ReadStats();
            }
        }

        /// <summary>
        /// Save all player stats to file
        /// 
        /// FILE FORMAT
        /// 
        /// high score 
        /// games played
        /// shots fired, shots hit, top hit percent
        /// villager kills, creeper kills, skeleton kills, pillager kills, enderman kills
        /// </summary>
        private void SaveStats()
        {
            outFile = File.CreateText(STATS_FILE_PATH);

            outFile.WriteLine(player.HighScore);
            outFile.WriteLine(player.GamesPlayed);

            outFile.WriteLine(player.ShotsFired + "," + player.ShotsHit + "," + player.TopHitPercent);
            outFile.WriteLine(player.MobsKilled[Player.SKELETON_KILL_INDEX] + "," + player.MobsKilled[Player.CREEPER_KILL_INDEX] + "," + player.MobsKilled[Player.SKELETON_KILL_INDEX] + "," + player.MobsKilled[Player.PILLAGER_KILL_INDEX] + "," + player.MobsKilled[Player.ENDERMAN_KILL_INDEX]);
        
            outFile.Close();
        }

        /// <summary>
        /// Initialize all levels
        /// </summary>
        private void InitializeLevels()
        {
            // load and initialize all levels
            for (int i = 0; i < NUM_LEVELS; i++)
            {
                level[i] = new Level(spriteBatch, i + 1); // level index starts at 1
                level[i].Load(); // load the level
            }
        }

        /// <summary>
        /// Reset all levels to their original state
        /// </summary>
        private void ResetLevels()
        {
            for (int i = 0; i < NUM_LEVELS; i++)
            {
                level[i].LevelReset();
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

            // draw based on the game state
            switch (gameState)
            {
                case MENU:
                    DrawMenu();
                    break;
                case STATS:
                    DrawStats();
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

            // DEBUG
            //DrawMouseLoc();

            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// draw the main menu
        /// </summary>
        private void DrawMenu()
        {
            // draw back ground
            spriteBatch.Draw(Assets.menuImg3, Vector2.Zero, Color.White);

            // draw all the buttons
            for (int i = 0; i < MENU_BUTTON_NUM; i++)
            {
                menuButtons[i].Draw(spriteBatch);
            }
        }

        public void DrawStats()
        {
            // draw back ground
            spriteBatch.Draw(Assets.menuImg3, Vector2.Zero, Color.White);

            // draw stats box
            spriteBatch.Draw(Assets.blankPixel, new Rectangle(0, STATS_TEXT_Y, STATS_BOX_WIDTH, STATS_BOX_HEIGHT), Color.Black * STATS_BOX_OPACITY);

            // draw all the stats
            spriteBatch.DrawString(Assets.minecraftEvening, "High Score: " + player.HighScore, CenterTextX(Assets.minecraftEvening, "High Score: " + player.HighScore, STATS_TEXT_Y), Color.Yellow);

            // draw player general stats
            spriteBatch.DrawString(Assets.minecraftRegular, "Games Played: " + player.GamesPlayed, RightTextX((int)levelStatsLoc.Y, 0), Color.White);
            spriteBatch.DrawString(Assets.minecraftRegular, "Shots Fired: " + player.ShotsFired, RightTextX((int)levelStatsLoc.Y + 1 * (int)(TITLE_SPACING_Y + Assets.minecraftRegular.MeasureString(" ").Y), 0), Color.White);
            spriteBatch.DrawString(Assets.minecraftRegular, "Shots Hit: " + player.ShotsHit, RightTextX((int)levelStatsLoc.Y + 2 * (int)(TITLE_SPACING_Y + Assets.minecraftRegular.MeasureString(" ").Y), 0), Color.White);
            spriteBatch.DrawString(Assets.minecraftRegular, "Top Hit %: " + string.Format("{0:0.00}", player.TopHitPercent) ,RightTextX((int)levelStatsLoc.Y + 3 * (int)(TITLE_SPACING_Y + Assets.minecraftRegular.MeasureString(" ").Y), 0), Color.White);
            spriteBatch.DrawString(Assets.minecraftRegular, "All-Time Hit %: " + player.AllTimeHitPercent, RightTextX((int)levelStatsLoc.Y + 4 * (int)(TITLE_SPACING_Y + Assets.minecraftRegular.MeasureString(" ").Y), 0), Color.White);
            spriteBatch.DrawString(Assets.minecraftRegular, "Avg. Shots: " + player.AvgShotsPerGame, RightTextX((int)levelStatsLoc.Y + 5 * (int)(TITLE_SPACING_Y + Assets.minecraftRegular.MeasureString(" ").Y), 0), Color.White);

            // draw player mobs kills
            spriteBatch.DrawString(Assets.minecraftRegular, "Villager Kills: " + player.MobsKilled[Player.VILLAGER_KILL_INDEX], RightTextX((int)levelStatsLoc.Y, 0.5f), Color.White);
            spriteBatch.DrawString(Assets.minecraftRegular, "Creeper Kills: " + player.MobsKilled[Player.CREEPER_KILL_INDEX], RightTextX((int)levelStatsLoc.Y + 1 * (int)(TITLE_SPACING_Y + Assets.minecraftRegular.MeasureString(" ").Y), 0.5f), Color.White);
            spriteBatch.DrawString(Assets.minecraftRegular, "Skeleton Kills: " + player.MobsKilled[Player.SKELETON_KILL_INDEX], RightTextX((int)levelStatsLoc.Y + 2 * (int)(TITLE_SPACING_Y + Assets.minecraftRegular.MeasureString(" ").Y), 0.5f), Color.White);
            spriteBatch.DrawString(Assets.minecraftRegular, "Pillager Kills: " + player.MobsKilled[Player.PILLAGER_KILL_INDEX], RightTextX((int)levelStatsLoc.Y + 3 * (int)(TITLE_SPACING_Y + Assets.minecraftRegular.MeasureString(" ").Y), 0.5f), Color.White);
            spriteBatch.DrawString(Assets.minecraftRegular, "Enderman Kills: " + player.MobsKilled[Player.ENDERMAN_KILL_INDEX], RightTextX((int)levelStatsLoc.Y + 4 * (int)(TITLE_SPACING_Y + Assets.minecraftRegular.MeasureString(" ").Y), 0.5f), Color.White);
            spriteBatch.DrawString(Assets.minecraftRegular, "Avg. Kills: " + player.AvgKillsPerGame, RightTextX((int)levelStatsLoc.Y + 5 * (int)(TITLE_SPACING_Y + Assets.minecraftRegular.MeasureString(" ").Y), 0.5f), Color.White);
            spriteBatch.DrawString(Assets.minecraftEvening, "TOTAL KILLS: " + player.TotalKills, CenterTextX(Assets.minecraftEvening, "TOTAL KILLS: " + player.TotalKills, (int)(levelStatsLoc.Y + 6 * (int)(TITLE_SPACING_Y + Assets.minecraftRegular.MeasureString(" ").Y))), Color.Red);
        }

        /// <summary>
        /// draw the level stats
        /// </summary>
        private void DrawLevelStats()
        {
            // draw the level
            level[curLevel].Draw(player);

            // draw stats box
            spriteBatch.Draw(Assets.blankPixel, new Rectangle(0, STATS_TEXT_Y, STATS_BOX_WIDTH, STATS_BOX_HEIGHT), Color.Black * STATS_BOX_OPACITY);

            // draw all the stats
            spriteBatch.DrawString(Assets.minecraftEvening, "Score: " + player.Score, CenterTextX(Assets.minecraftEvening, "Score: " + player.Score, STATS_TEXT_Y), Color.Yellow);

            for (int i = 0; i < NUM_LEVELS; i++)
            {
                spriteBatch.DrawString(Assets.minecraftRegular, "Level " + (i + 1) + " score: " + level[i].LevelScore, RightTextX((int)levelStatsLoc.Y + (i) * (int)(TITLE_SPACING_Y + Assets.minecraftRegular.MeasureString(" ").Y), 0f), Color.White);
            }

            // draw current level stats, level number, kills, shots fired, shots hit, and accuracy
            spriteBatch.DrawString(Assets.minecraftRegular, "Level: " + (curLevel + 1), RightTextX((int)levelStatsLoc.Y, 0.5f), Color.White);
            spriteBatch.DrawString(Assets.minecraftRegular, "Kills: " + level[curLevel].LevelKills, RightTextX((int)levelStatsLoc.Y + 1 * (int)(TITLE_SPACING_Y + Assets.minecraftRegular.MeasureString(" ").Y), 0.5f), Color.White);
            spriteBatch.DrawString(Assets.minecraftRegular, "Shots Fired: " + level[curLevel].LevelShotsFired, RightTextX((int)levelStatsLoc.Y + 2 * (int)(TITLE_SPACING_Y + Assets.minecraftRegular.MeasureString(" ").Y), 0.5f), Color.White);
            spriteBatch.DrawString(Assets.minecraftRegular, "Shots Hit: " + level[curLevel].LevelShotsHit, RightTextX((int)levelStatsLoc.Y + 3 * (int)(TITLE_SPACING_Y + Assets.minecraftRegular.MeasureString(" ").Y), 0.5f), Color.White);
            
            spriteBatch.DrawString(Assets.minecraftRegular, "Hit %: " + string.Format("{0:0.##}", level[curLevel].LevelHitPercent), RightTextX((int)levelStatsLoc.Y + 4 * (int)(TITLE_SPACING_Y + Assets.minecraftRegular.MeasureString(" ").Y), 0.5f), Color.White);
            
            spriteBatch.DrawString(Assets.minecraftBold, "PRESS SPACE TO CONTINUE", CenterTextX(Assets.minecraftBold, "PRESS SPACE TO CONTINUE", (int)(levelStatsLoc.Y + 5 * (int)(TITLE_SPACING_Y + Assets.minecraftRegular.MeasureString(" ").Y))), Color.Yellow);
        }

        // draw the gameplay
        private void DrawGameplay()
        {
            level[curLevel].Draw(player);
        }

        /// <summary>
        /// draw the shop
        /// </summary>
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
                    spriteBatch.Draw(shopButtonImgs[i], shopButtonRecs[i], Color.White * SHOP_BUTTON_USED_OPACITY);
                else spriteBatch.Draw(shopButtonImgs[i], new Rectangle(shopButtonRecs[i].X, shopButtonRecs[i].Y, shopButtonRecs[i].Width, shopButtonRecs[i].Height), Color.White);
            }

            // draw the current player score on the button
            spriteBatch.DrawString(Assets.minecraftBold, "Score: " + player.Score, CenterTextX(Assets.minecraftBold, "Score: " + player.Score, SCORE_TEXT_Y), Color.Yellow);
        }

        /// <summary>
        /// check if a rectangle was clicked
        /// </summary>
        /// <param name="rectangle"></param> the rectangle
        /// <returns></returns>
        private bool IsClickRectangle(Rectangle rectangle)
        {
            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed && rectangle.Contains(mouse.Position)) return true;
            return false;
        }

        /// <summary>
        /// center the text on the x axis, given the y axis
        /// </summary>
        /// <param name="font"></param> the font of the text
        /// <param name="text"></param> the text
        /// <param name="locY"></param> the y axis
        /// <param name="position"></param> the position of the text relative to the screen
        /// <returns></returns>
        public static Vector2 CenterTextX(SpriteFont font, string text, int locY, float position = 0.5f)
        {
            return new Vector2(SCREEN_WIDTH * position - (font.MeasureString(text).X / 2), locY);
        }

        /// <summary>
        /// align the text to the right
        /// </summary>
        /// <param name="locY"></param> the y axis
        /// <param name="position"></param> the position of the text relative to the alignment
        /// <returns></returns>
        public static Vector2 RightTextX(int locY, float position = 0.5f)
        {
            return new Vector2(SCREEN_WIDTH * position + TITLE_SPACING_X, locY);
        }

        /// <summary>
        /// center the rectangle on the x axis
        /// </summary>
        /// <param name="width"></param> the width of the rectangle
        /// <param name="locY"></param> the y axis
        /// <param name="position"></param> the position of the rectangle relative to the screen
        /// <returns></returns>
        public static Vector2 CenterRectangleX(int width, int locY, float position = 0.5f)
        {
            return new Vector2(SCREEN_WIDTH * position - (width / 2), locY);
        }

        /// <summary>
        /// DEBUG
        /// draw the mouse 
        /// </summary>
        private void DrawMouseLoc()
        {
            spriteBatch.DrawString(Assets.debugFont, mouse.X + ", " + mouse.Y, new Vector2(mouse.X + 10, mouse.Y + 10), Color.White);
        }
    }
}
