using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace PASS2V2
{
    internal class Enderman : Mob
    {
        // number of locations for the enderman to teleport to, after spawning
        private const int TP_LOC_COUNT = 3;

        // how long the enderman should wait before teleporting
        private const int TP_DUR = 3000; // 3 seconds

        private const int SCARING_DUR = 500; // half a second

        // spawn, teleport, and visited locations
        private static Vector2 spawnLoc = new Vector2(Game1.rng.Next(0, Game1.SCREEN_WIDTH - WIDTH), 0);
        private static Vector2[] tpLoc = new Vector2[TP_LOC_COUNT];
        private bool[] isVisited = new bool[TP_LOC_COUNT];

        // timer for if the enderman should be teleport
        private Timer tpTimer = new Timer(TP_DUR, true);

        private int tpCount = 0;

        // flag and timer for if the ender man is scaring
        private bool isScaring;
        private Timer scaringTimer = new Timer(SCARING_DUR, true);

        /// <summary>
        /// get if the enderman is scaring
        /// </summary>
        public bool IsScaring
        {
            get { return isScaring; }
        }

        /// <summary>
        /// constructor for the enderman
        /// points: 100
        /// health: 5
        /// damage: 0
        /// </summary>
        /// <param name="spriteBatch"></param>
        public Enderman(SpriteBatch spriteBatch) : base(spriteBatch, spawnLoc, Vector2.Zero, 100, 5, 0)
        {
            // check if the locations have not been generated yet, if not generate them
            if (tpLoc[0] == tpLoc[1] && tpLoc[0] == tpLoc[2] && tpLoc[1] == tpLoc[2] && tpLoc[0] == Vector2.Zero) GenerateSpawnLocations();
            
            // set the skin
            skin = Assets.endermanImg;
        }

        /// <summary>
        /// generate the locations for the enderman to teleport to
        /// these locations are the 2nd, 3rd, and 4th locations, since the first location is the spawn location
        /// </summary>
        private static void GenerateSpawnLocations()
        {
            for (int i = 0; i < TP_LOC_COUNT; i++)
            {
                // generate random locations
                Vector2 tempLoc = new Vector2(Game1.rng.Next(0, Game1.SCREEN_WIDTH - WIDTH), Game1.rng.Next(0, Game1.SCREEN_HEIGHT - 2 * HEIGHT));

                // check if the location is valid and not repeated
                while (tempLoc == tpLoc[0] || tempLoc == tpLoc[1] || tempLoc == tpLoc[2])
                {
                    tempLoc = new Vector2(Game1.rng.Next(0, Game1.SCREEN_WIDTH - WIDTH), Game1.rng.Next(0, Game1.SCREEN_HEIGHT - 2 * HEIGHT));
                }

                // set the location
                tpLoc[i] = tempLoc;
            }
        }

        /// <summary>
        /// update the enderman based on states
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="player"></param>
        public override void Update(GameTime gameTime, Player player)
        {
            switch (state)
            {
                case ALIVE:
                    UpdateTeleport(gameTime, player);
                    break;
                case DEAD:
                    deathTimer.Update(gameTime);
                    if (deathTimer.IsFinished()) state = REMOVE;
                    break;
            }
        }

        private void UpdateTeleport(GameTime gameTime, Player player)
        {
            tpTimer.Update(gameTime);
            scaringTimer.Update(gameTime);

            // check if the enderman should be teleported
            if (tpTimer.IsFinished())
            {
                // if all the location have been visited then the enderman will be removed
                if (tpCount > TP_LOC_COUNT)
                {
                    state = REMOVE;
                }
                // otherwise the enderman will be teleported to new location
                else
                {
                    // teleport enderman to random location or in front of player
                    if (tpCount < TP_LOC_COUNT)
                    {
                        // generate a random index
                        int randSpawnIndex = Game1.rng.Next(0, TP_LOC_COUNT);

                        // check if the random index has already been visited, if so generate a new one
                        while (isVisited[randSpawnIndex])
                        {
                            randSpawnIndex = Game1.rng.Next(0, TP_LOC_COUNT);
                        }

                        // teleport enderman to the random location
                        curLoc = tpLoc[randSpawnIndex];
                        rec.Y = (int)curLoc.Y;
                        rec.X = (int)curLoc.X;

                        // increment the teleport count and mark the location as visited
                        tpCount++;
                        isVisited[randSpawnIndex] = true;
                    }
                    else if (tpCount == TP_LOC_COUNT)
                    {
                        // teleport enderman in front of player
                        curLoc.X = player.Location.X;
                        curLoc.Y = player.Location.Y - HEIGHT;
                        rec.Y = (int)curLoc.Y;
                        rec.X = (int)curLoc.X;

                        // increment the teleport count
                        tpCount++;
                    }

                    tpTimer.ResetTimer(true);

                    // apply scaring for half a second
                    isScaring = true;
                    scaringTimer.ResetTimer(true);
                }
            }
            
            // not scaring if the timer is finished and enderman was scaring
            if (scaringTimer.IsFinished() && isScaring) isScaring = false;

        }

        /// <summary>
        /// Draw the enderman given the state
        /// </summary>
        public override void Draw()
        {
            switch (state)
            {
                case ALIVE:
                    spriteBatch.Draw(skin, rec, Color.White);
                    break;
                case DEAD:
                    spriteBatch.Draw(bloodImg, rec, Color.White);
                    break;
            }
        }
    }
}
