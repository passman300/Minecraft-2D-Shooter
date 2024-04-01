using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PASS2V2
{
    public class Level
    {
        public enum LevelState
        {
            PreLevel = 0,
            GamePlay = 1,
            PostLevel = 2
        }

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

        // player movment box
        private const int PLAYER_MOVEMENT_BOX_Y = Game1.SCREEN_HEIGHT - Player.HEIGHT;
        private const float PLAYER_MOVEMENT_BOX_OPACITY = 0.5f;

        // score display
        private const int SCORE_DISPLAY_X = 10;
        private const int SCORE_DISPLAY_Y = PLAYER_MOVEMENT_BOX_Y - 20;

        // default level stats
        private const string DEFAULT_LEVEL_STATS = "20,20,20,20,20,1,1,1";

        private string levelPath;

        private StreamReader inFile;
        private StreamWriter outFile;

        private SpriteBatch spriteBatch;

        private int levelNum;

        private LevelState levelState = LevelState.GamePlay;

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

        public Level(SpriteBatch spriteBatch, int levelNum)
        {
            this.spriteBatch = spriteBatch;

            this.levelNum = levelNum;

            levelPath = $"Levels/{levelNum}.txt";
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
                inFile.Close();

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
                    Console.Write((int)type + ",");
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
                case LevelState.PreLevel:
                    break;
                case LevelState.GamePlay:
                    UpdateGamePlay(gameTime, player);
                    break;
                case LevelState.PostLevel:
                    break;
            }
        }

        private void UpdateGamePlay(GameTime gameTime, Player player)
        {
            // update spawn timer
            mobSpawnTimer.Update(gameTime);

            UpdatePlayer(gameTime, player);

            UpdateMobs(gameTime, player);

            UpdateArrows(player);
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
                arrows.Add(new Arrow(spriteBatch, player.Location + player.ShootLocOffset, Arrow.ArrowDirection.Up, Player.BASE_DAMAGE));
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

                if (mobs[i] is Creeper && !((Creeper)mobs[i]).IsExplodeApplied && mobs[i].State == Creeper.EXPLODE)
                {
                    ((Creeper)mobs[i]).IsExplodeApplied = true;
                     player.Score -= mobs[i].Damage;
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

                //testing
            }
        }

        private void SpawnMob()
        {
            int randNum = Game1.rng.Next(0, 100);

            if (randNum < 50) mobs.Add(new Villager(spriteBatch));
            else if (randNum < 100) mobs.Add(new Creeper(spriteBatch));


            //if (randNum < levelStats[VILLAGER_ODDS_INDEX]) mobs.Add(new Villager(spriteBatch));
            //else if (randNum < levelStats[CREEPER_ODDS_INDEX]) mobs.Add(new Creeper(spriteBatch));

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
                        mobs[j].Health -= arrows[i].Damage;
                        levelShotsHit++;

                        if (mobs[j].Health <= 0)
                        {
                            levelKills++;
                            mobs[j].State = Mob.DEAD;     
                            levelScore += mobs[j].Points;
                            player.Score += mobs[j].Points;
                        }

                        arrows[i].State = Arrow.ArrowState.Remove;
                        break;
                    }
                }

                if (arrows[i].Direction == Arrow.ArrowDirection.Down)
                {
                    // check if arrows hit player
                    if (arrows[i].Rectangle.Intersects(player.Rectangle))
                    {
                        player.Score = Math.Max(player.Score - arrows[i].Damage, 0);
                        arrows[i].State = Arrow.ArrowState.Remove;
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
                case LevelState.PreLevel:
                    break;
                case LevelState.GamePlay:
                    DrawGamePlay(player);
                    break;
                case LevelState.PostLevel:
                    break;
            }
                 
        }

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

            DEBUG_MENU();
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

        private void DEBUG_MENU()
        {
            spriteBatch.DrawString(Assets.debugFont, "MOBS LIST: " + mobs.Count, new Vector2(3, 0), Color.White);
            spriteBatch.DrawString(Assets.debugFont, "MOBS SPAWNED: " + mobsSpawned, new Vector2(3, 10), Color.White);
            spriteBatch.DrawString(Assets.debugFont, "ARROWS LIST: " + arrows.Count, new Vector2(3, 20), Color.White);
        }

    }
}
