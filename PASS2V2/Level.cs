using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;

namespace PASS2V2
{
    public class Level
    {
        public enum LevelStates
        {
            PreLevel = 0,
            GamePlay = 1,
            PostLevel = 2
        }

        // level title text
        private const string INTRO_TEXT = "INTRODUCTION";
        private const string MOVE_TEXT = "MOVE: A,D or Arrows";
        private const string SHOOT_TEXT = "SHOOT: Space";
        private const string GOAL_TEXT = "GOAL: Kill mobs before they escape";
        private const string TIPS_TEXT = "TIPS: Watch for movement patterns and unique abilities";
        private const string BEGIN_TEXT = "PRESS SPACE TO BEGIN";

        // instruction text y location and title spacing
        private const int INSTRUCTION_TEXT_Y = 210;
        private const int TITLE_SPACING_Y = 4;

        // title box width and height and opacity
        private const int TITLE_BOX_WIDTH = Game1.SCREEN_WIDTH;
        private const int TITLE_BOX_HEIGHT = 275;
        private const float TITLE_BOX_OPACITY = 0.5f;

        // level loading stats indexes
        private const int NUM_LEVEL_LOADING_STATS = 8;
        private const int VILLAGER_ODDS_INDEX = 0;
        private const int CREEPER_ODDS_INDEX = 1;
        private const int SKELETON_ODDS_INDEX = 2;
        private const int PILLAGER_ODDS_INDEX = 3;
        private const int ENDERMAN_ODDS_INDEX = 4;
        private const int MAX_MOBS_INDEX = 5;
        private const int MAX_SCREEN_MOBS_INDEX = 6;
        private const int MOBS_SPAWN_DUR__INDEX = 7;

        // player movement box
        private const int PLAYER_MOVEMENT_BOX_Y = Game1.SCREEN_HEIGHT - Player.HEIGHT;
        private const float PLAYER_MOVEMENT_BOX_OPACITY = 0.5f;

        // score display
        private const int SCORE_DISPLAY_X = 10;
        private const int SCORE_DISPLAY_Y = PLAYER_MOVEMENT_BOX_Y - 30;

        // buff icon display
        private const int BUFF_ICON_Y = 390;
        private const int BUFF_ICON_X = 600;
        private const int BUFF_ICON_SPACER_Y = 5;

        // default level stats
        private const string DEFAULT_LEVEL_STATS = "20,20,20,20,20,10,2,1";

        private string levelPath;

        private StreamReader inFile;
        private StreamWriter outFile;

        private SpriteBatch spriteBatch;

        private int levelNum;

        private LevelStates levelState = LevelStates.PreLevel;

        // pre level titles locations
        private Vector2 introTitleLoc;
        private Vector2 moveTitleLoc;
        private Vector2 shootTitleLoc;
        private Vector2 goalTitleLoc;
        private Vector2 tipsTitleLoc;
        private Vector2 beginTitleLoc;

        private float[] levelStats = new float[NUM_LEVEL_LOADING_STATS];

        private Rectangle playerMovementBoxRec = new Rectangle(0, PLAYER_MOVEMENT_BOX_Y, Game1.SCREEN_WIDTH, Player.HEIGHT);

        private List<Tile> tiles = new List<Tile>();

        private List<Mob> mobs = new List<Mob>();
        private int mobsSpawned = 0;
        private Timer mobSpawnTimer;

        private List<Arrow> arrows = new List<Arrow>();

        private int levelScore = 0;
        private int levelKills = 0;
        private int levelShotsFired = 0;
        private int levelShotsHit = 0;

        // buff icons varibles
        private Texture2D[] buffIconImgs = new Texture2D[Player.NUM_UPGRADES];
        private Vector2[] buffIconLocs = new Vector2[Player.NUM_UPGRADES];

        public LevelStates LevelState
        {
            get { return levelState; }
            set { levelState = value; }
        }

        public int LevelScore
        {
            get { return levelScore; }
            set { levelScore = value; }
        }

        public int LevelKills
        {
            get { return levelKills; }
            set { levelKills = value; }
        }

        public int LevelShotsFired
        {
            get { return levelShotsFired; }
            set { levelShotsFired = value; }
        }

        public int LevelShotsHit
        {
            get { return levelShotsHit; }
            set { levelShotsHit = value; }
        }

        public float[] LevelStats
        {
            get { return levelStats; }
        }

        public Level(SpriteBatch spriteBatch, int levelNum)
        {
            // save the spritebatch
            this.spriteBatch = spriteBatch;

            // save the level number
            this.levelNum = levelNum;

            // set the title locations 
            introTitleLoc = Game1.CenterTextX(Assets.minecraftEvening, INTRO_TEXT, INSTRUCTION_TEXT_Y);
            moveTitleLoc = Game1.CenterTextX(Assets.minecraftRegular, MOVE_TEXT, (int)(introTitleLoc.Y + Assets.minecraftEvening.MeasureString(INTRO_TEXT).Y + TITLE_SPACING_Y), 0.3333f);
            shootTitleLoc = Game1.CenterTextX(Assets.minecraftRegular, SHOOT_TEXT, (int)(introTitleLoc.Y + Assets.minecraftEvening.MeasureString(INTRO_TEXT).Y + TITLE_SPACING_Y), 0.6666f);
            goalTitleLoc = Game1.CenterTextX(Assets.minecraftRegular, GOAL_TEXT, (int)(shootTitleLoc.Y + Assets.minecraftRegular.MeasureString(SHOOT_TEXT).Y + TITLE_SPACING_Y));
            tipsTitleLoc = Game1.CenterTextX(Assets.minecraftRegular, TIPS_TEXT, (int)(goalTitleLoc.Y + Assets.minecraftRegular.MeasureString(GOAL_TEXT).Y + TITLE_SPACING_Y));
            beginTitleLoc = Game1.CenterTextX(Assets.minecraftBold, BEGIN_TEXT, (int)(tipsTitleLoc.Y + Assets.minecraftRegular.MeasureString(TIPS_TEXT).Y + TITLE_SPACING_Y));

            // set the level path
            levelPath = $"Levels/{levelNum}.txt";

            // set the buff icons
            buffIconImgs[Player.DOUBLE_SPEED_INDEX] = Assets.speedIconImg;
            buffIconImgs[Player.TRIPLE_DAMAGE_INDEX] = Assets.damageIconImg;
            buffIconImgs[Player.DOUBLE_FIRE_RATE_INDEX] = Assets.fireRateIconImg;
            buffIconImgs[Player.DOUBLE_POINTS_INDEX] = Assets.pointMutiIconImg;

            // set buff icon locations
            buffIconLocs[Player.DOUBLE_SPEED_INDEX] = new Vector2(BUFF_ICON_X, BUFF_ICON_Y);
            buffIconLocs[Player.TRIPLE_DAMAGE_INDEX] = new Vector2(BUFF_ICON_X, BUFF_ICON_Y + buffIconImgs[Player.TRIPLE_DAMAGE_INDEX].Height + BUFF_ICON_SPACER_Y);
            buffIconLocs[Player.DOUBLE_FIRE_RATE_INDEX] = new Vector2(BUFF_ICON_X, BUFF_ICON_Y + Player.DOUBLE_FIRE_RATE_INDEX * (buffIconImgs[Player.DOUBLE_FIRE_RATE_INDEX].Height + BUFF_ICON_SPACER_Y));
            buffIconLocs[Player.DOUBLE_POINTS_INDEX] = new Vector2(BUFF_ICON_X, BUFF_ICON_Y + Player.DOUBLE_POINTS_INDEX * (buffIconImgs[Player.DOUBLE_FIRE_RATE_INDEX].Height + BUFF_ICON_SPACER_Y));
        }

        /// <summary>
        /// Loads the level by populating the tileList with Tiles, based on the levelTilePath.
        /// </summary>
        public void Load()
        {
            try
            {
                // open the level tile file
                inFile = File.OpenText(levelPath);

                // location of the current tile loading
                Vector2 curTileLoc = Vector2.Zero;

                // read the rest of the lines
                for (int i = 0; i < Game1.SCREEN_HEIGHT / Tile.HEIGHT; i++)
                {
                    // read the next line
                    string line = inFile.ReadLine();

                    foreach (char ele in line)
                    {
                        // check if element in line isn't a comma and is a digit within the tile types
                        if (ele != ',')
                        {
                            if (ele - '0' >= (int)TileTypes.Dirt && ele - '0' <= (int)TileTypes.Coblestone)
                            {
                                // create the tile and add it to the tile list
                                switch (int.Parse(ele.ToString()))
                                {
                                    case (int)TileTypes.Dirt:
                                        tiles.Add(new Tile(spriteBatch, TileTypes.Dirt, curTileLoc));
                                        break;

                                    case (int)TileTypes.Grass1:
                                        tiles.Add(new Tile(spriteBatch, TileTypes.Grass1, curTileLoc));
                                        break;

                                    case (int)TileTypes.Grass2:
                                        tiles.Add(new Tile(spriteBatch, TileTypes.Grass2, curTileLoc));
                                        break;

                                    case (int)TileTypes.Coblestone:
                                        tiles.Add(new Tile(spriteBatch, TileTypes.Coblestone, curTileLoc));
                                        break;
                                }
                            }
                            else
                            {
                                // add a dirt tile if the element is not a digit or not in the tile types
                                tiles.Add(new Tile(spriteBatch, TileTypes.Dirt, curTileLoc));
                            }
                            curTileLoc.X += Tile.WIDTH;
                        }
                    }

                    // update the next tile Y location
                    curTileLoc.X = 0;
                    curTileLoc.Y += Tile.HEIGHT;
                }

                // read last line as level stats
                string[] statsLine = inFile.ReadLine().Split(',');
                for (int i = 0; i < NUM_LEVEL_LOADING_STATS; i++)
                {
                    levelStats[i] = float.Parse(statsLine[i]);
                }

                // load the spawn timer
                mobSpawnTimer = new Timer(levelStats[MOBS_SPAWN_DUR__INDEX] * 1000, true);

                inFile.Close();
            }
            // catch the file not found exception
            catch (FileNotFoundException fnfe)
            {
                // display the error message
                Console.WriteLine(fnfe.Message);

                // create a blank tile file
                CreateBlankFile();

                // reinitialize the tile list
                Load();
            }
            catch (NullReferenceException)
            {
                inFile.Close();

                // create a blank tile file
                CreateBlankFile();

                // reinitialize the tile list
                Load();
            }
        }

        /// <summary>
        /// Creates a blank tile file, full of a tile type. (dirt by default)
        /// </summary>
        /// <param name="type"></param> The type of tile to write to the file
        private void CreateBlankFile(TileTypes type = TileTypes.Dirt)
        {
            // create the blank tile file
            outFile = File.CreateText(levelPath);

            // write the lines of the file
            for (int i = 0; i < Game1.SCREEN_HEIGHT / Tile.HEIGHT; i++)
            {
                // for each row write the type to the file with a comma in between
                for (int j = 0; j < Game1.SCREEN_WIDTH / Tile.WIDTH - 1; j++)
                {
                    outFile.Write((int)type + ",");
                }

                outFile.WriteLine((int)type);
            }

            // write the default level stats
            outFile.WriteLine(DEFAULT_LEVEL_STATS);

            outFile.Close();
        }


        public void Update(GameTime gameTime, Player player)
        {
            switch (levelState)
            {
                case LevelStates.PreLevel:
                    UpdatePreLevel();
                    break;
                case LevelStates.GamePlay:
                    UpdateGamePlay(gameTime, player);
                    break;
            }
        }

        private void UpdatePreLevel()
        {
            if (Game1.kb.IsKeyDown(Keys.Space) && !Game1.prevKb.IsKeyDown(Keys.Space))
            {
                levelState = LevelStates.GamePlay;
                // spawn a new mob
                SpawnMob();
                mobSpawnTimer.ResetTimer(true);
            }
        }

        private void UpdateGamePlay(GameTime gameTime, Player player)
        {
            // update spawn timer
            mobSpawnTimer.Update(gameTime);

            UpdatePlayer(gameTime, player);

            UpdateMobs(gameTime, player);

            UpdateArrows(player);

            // check if game over when there is no active mobs and no more to spawn
            if (mobs.Count == 0 && mobsSpawned == levelStats[MAX_MOBS_INDEX]) levelState = LevelStates.PostLevel;
        }


        /// <summary>
        /// update the player
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="player"></param>
        /// <param name=""></param>
        private void UpdatePlayer(GameTime gameTime, Player player)
        {
            // update player
            player.Update(gameTime);

            // check if player shoots a arrow
            if (player.IsShoot)
            {
                arrows.Add(new Arrow(spriteBatch, player.Location + player.ShootLocOffset, Arrow.ArrowDirection.Up, player.Damage));
                levelShotsFired++;
            }
        }

        /// <summary>
        /// update the mobs
        /// </summary>
        /// <param name="gameTime"></param>
        private void UpdateMobs(GameTime gameTime, Player player)
        {
            for (int i = 0; i < mobs.Count; i++)
            {
                mobs[i].Update(gameTime, player.Rectangle);

                // check if the mob is exploding creeper and the damage isn't applied yet
                if (mobs[i] is Creeper && !((Creeper)mobs[i]).IsExplodeApplied && mobs[i].State == Creeper.EXPLODE)
                {
                    ((Creeper)mobs[i]).IsExplodeApplied = true;
                    levelScore = Math.Max(levelScore - mobs[i].Damage, 0);
                    player.Score = Math.Max(levelScore - mobs[i].Damage, 0);
                }

                // check if the mob is shooting
                else if (mobs[i].IsShoot)
                {
                    // add a new arrow from the center of the mob going down
                    arrows.Add(new Arrow(spriteBatch, mobs[i].Location + mobs[i].ShootLocOffset, Arrow.ArrowDirection.Down, mobs[i].Damage));
                }

                if (mobs[i].State == Mob.REMOVE)
                {
                    mobs.RemoveAt(i);
                    i--;
                }
            }

            // check if new mobs can spawn
            if (mobs.Count < levelStats[MAX_SCREEN_MOBS_INDEX] && mobsSpawned < levelStats[MAX_MOBS_INDEX] && mobSpawnTimer.IsFinished())
            {
                // spawn a new mob
                SpawnMob();
                mobSpawnTimer.ResetTimer(true);
            }
        }

        private void SpawnMob()
        {
            int randNum = Game1.rng.Next(0, 100);

            if (randNum < levelStats[VILLAGER_ODDS_INDEX]) mobs.Add(new Villager(spriteBatch));
            else if (randNum < levelStats[CREEPER_ODDS_INDEX] + levelStats[VILLAGER_ODDS_INDEX]) mobs.Add(new Creeper(spriteBatch));
            else if (randNum < levelStats[SKELETON_ODDS_INDEX] + levelStats[CREEPER_ODDS_INDEX] + levelStats[VILLAGER_ODDS_INDEX]) mobs.Add(new Skeleton(spriteBatch));
            else if (randNum < levelStats[PILLAGER_ODDS_INDEX] + levelStats[SKELETON_ODDS_INDEX] + levelStats[CREEPER_ODDS_INDEX] + levelStats[VILLAGER_ODDS_INDEX]) mobs.Add(new Pillager(spriteBatch));
            else if (randNum < levelStats[ENDERMAN_ODDS_INDEX] + levelStats[PILLAGER_ODDS_INDEX] + levelStats[SKELETON_ODDS_INDEX] + levelStats[CREEPER_ODDS_INDEX] + levelStats[VILLAGER_ODDS_INDEX]) mobs.Add(new Enderman(spriteBatch));

            mobsSpawned++;
        }

        /// <summary>
        /// update the arrows and collisions
        /// </summary>
        private void UpdateArrows(Player player)
        {
            for (int i = 0; i < arrows.Count; i++)
            {
                // update arrows
                arrows[i].Update();

                // check if arrows hit mobs
                for (int j = 0; j < mobs.Count && arrows[i].Direction == Arrow.ArrowDirection.Up; j++)
                {
                    if (arrows[i].Rectangle.Intersects(mobs[j].Rectangle) && mobs[j].State == Mob.ALIVE)
                    {
                        // check if the mob has a shield
                        if (mobs[j].IsSheild)
                        {
                            mobs[j].IsSheild = false;

                        }
                        else
                        {
                            mobs[j].Health -= arrows[i].Damage;
                            levelShotsHit++;

                            if (mobs[j].Health <= 0)
                            {
                                levelKills++;
                                mobs[j].State = Mob.DEAD;
                                levelScore += mobs[j].Points;
                                player.Score += mobs[j].Points;
                            }
                        }

                        arrows[i].State = Arrow.ArrowState.Remove;
                        break;
                    }
                }

                // check if arrows hit player
                if (arrows[i].Direction == Arrow.ArrowDirection.Down)
                {
                    // check if arrows hit player
                    if (arrows[i].Rectangle.Intersects(player.Rectangle))
                    {
                        levelScore = Math.Max(levelScore - arrows[i].Damage, 0);
                        player.Score = Math.Max(player.Score - arrows[i].Damage, 0);
                        arrows[i].State = Arrow.ArrowState.Remove;

                        // reset all player buffs
                        player.ResetBuffs();
                    }
                }

                // remove arrows if they are off the screen
                if (arrows[i].State == Arrow.ArrowState.Remove)
                {
                    arrows.RemoveAt(i);
                    i--;
                }
            }
        }

        /// <summary>
        /// draw all level objects
        /// </summary>
        public void Draw(Player player)
        {
            DrawTiles();

            switch (levelState)
            {
                case LevelStates.PreLevel:
                    DrawPreLevel(player);
                    break;
                case LevelStates.GamePlay:
                    DrawGamePlay(player);
                    break;
                case LevelStates.PostLevel:
                    break;
            }

        }

        private void DrawPreLevel(Player player)
        {
            // draw title box
            spriteBatch.Draw(Assets.blankPixel, new Rectangle(0, Game1.SCREEN_HEIGHT / 2 - TITLE_BOX_HEIGHT / 2, TITLE_BOX_WIDTH, TITLE_BOX_HEIGHT), Color.Black * TITLE_BOX_OPACITY);

            // draw introduction and tips text
            spriteBatch.DrawString(Assets.minecraftEvening, INTRO_TEXT, introTitleLoc, Color.CornflowerBlue);
            spriteBatch.DrawString(Assets.minecraftRegular, MOVE_TEXT, moveTitleLoc, Color.White);
            spriteBatch.DrawString(Assets.minecraftRegular, SHOOT_TEXT, shootTitleLoc, Color.White);
            spriteBatch.DrawString(Assets.minecraftRegular, GOAL_TEXT, goalTitleLoc, Color.White);
            spriteBatch.DrawString(Assets.minecraftRegular, TIPS_TEXT, tipsTitleLoc, Color.White);
            spriteBatch.DrawString(Assets.minecraftBold, BEGIN_TEXT, beginTitleLoc, Color.Yellow);

            // draw player and movement box
            spriteBatch.Draw(Assets.blankPixel, playerMovementBoxRec, Color.Black * PLAYER_MOVEMENT_BOX_OPACITY);
            player.Draw();

        }

        /// <summary>
        /// draw all the gameplay objects
        /// </summary>
        /// <param name="player"></param>
        private void DrawGamePlay(Player player)
        {
            // draw arrows
            DrawArrows();

            // draw mobs
            DrawMobs();

            // draw player movement box
            spriteBatch.Draw(Assets.blankPixel, playerMovementBoxRec, Color.Black * PLAYER_MOVEMENT_BOX_OPACITY);

            // draw player
            player.Draw();

            // draw score
            DrawScore(player.Score);

            // draw the player buffs
            for (int i = 0; i < buffIconImgs.Length; i++)
            {
                // check if the buff is activated
                if (player.UsedBuffs[i]) spriteBatch.Draw(buffIconImgs[i], buffIconLocs[i], Color.White);
                else spriteBatch.Draw(buffIconImgs[i], buffIconLocs[i], Color.White * 0.5f);
            }

            DEBUG_MENU(player);
        }

        private void DrawArrows()
        {
            foreach (Arrow arrow in arrows) { arrow.Draw(); }
        }

        private void DrawMobs()
        {
            for (int i = 0; i < mobs.Count; i++)
            {
                mobs[i].Draw();
            }
        }

        /// <summary>
        /// Draws all tiles
        /// </summary>
        private void DrawTiles()
        {
            foreach (Tile tile in tiles) { tile.Draw(); }
        }

        private void DrawScore(int playerScore)
        {
            spriteBatch.DrawString(Assets.minecraftRegular, "Score: " + playerScore, new Vector2(SCORE_DISPLAY_X, SCORE_DISPLAY_Y), Color.Yellow);
        }



        private void DEBUG_MENU(Player player)
        {
            spriteBatch.DrawString(Assets.debugFont, "MOBS LIST: " + mobs.Count, new Vector2(3, 0), Color.White);
            spriteBatch.DrawString(Assets.debugFont, "MOBS SPAWNED: " + mobsSpawned, new Vector2(3, 10), Color.White);
            spriteBatch.DrawString(Assets.debugFont, "ARROWS LIST: " + arrows.Count, new Vector2(3, 20), Color.White);
        
            spriteBatch.DrawString(Assets.debugFont, "Player Damage" + player.Damage, new Vector2(3, 30), Color.White);
        }


    }
}
