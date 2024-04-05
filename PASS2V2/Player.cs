using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace PASS2V2
{
    public class Player
    {
        // player upgrades
        public const int NUM_UPGRADES = 4;
        public const int DOUBLE_SPEED_INDEX = 0;
        public const int TRIPLE_DAMAGE_INDEX = 1;
        public const int DOUBLE_FIRE_RATE_INDEX = 2;
        public const int DOUBLE_POINTS_INDEX = 3;

        // player dimensions
        public const int WIDTH = 64;
        public const int HEIGHT = 64;

        // player y location
        public const int Y_LOC = Game1.SCREEN_HEIGHT - HEIGHT;

        public const int VILLAGER_KILL_INDEX = 0;
        public const int CREEPER_KILL_INDEX = 1;
        public const int SKELETON_KILL_INDEX = 2;
        public const int PILLAGER_KILL_INDEX = 3;
        public const int ENDERMAN_KILL_INDEX = 4;

        // player base speed
        public const int BASE_SPEED = 3;
        
        // player shooting speed
        public const float BASE_SHOOTING_DUR = 1000f / 3; // 3 shots per second

        public const int BASE_DAMAGE = 1;

        // local spritebatch
        private SpriteBatch spriteBatch;

        // player skin
        private Texture2D skin;

        // player location and rectangle
        private Vector2 loc;
        private Rectangle rec;

        // speed of player
        private int speed; // player only moves in the x direction

        // player shooting variables
        private Timer shootTimer = new Timer(BASE_SHOOTING_DUR, true);
        private bool isShoot = false;
        private Vector2 shootLocOffset = new Vector2(WIDTH / 2, 0);

        // player score
        private int highScore = 0;
        private int score = 0;

        private int gamesPlayed = 0;
        private int shotsFired = 0;
        private int shotsHit = 0;
        private float topHitPercent = 0;

        // calculated stats
        private int totalKills = 0;
        private float allTimeHitPercent = 0;
        private float avgShotsPerGame = 0;
        private float avgKillsPerGame = 0;

        private int[] mobsKilled = new int[Mob.MOB_COUNT];

        // player damage
        private int damage = BASE_DAMAGE;

        // list of upgrades
        private bool[] buffs = new bool[NUM_UPGRADES];

        // flag for is the player is feared
        private bool isFear;

        /// <summary>
        /// get and set if the player is shooting
        /// </summary>
        public bool IsShoot
        {
            get { return isShoot; }
            set { isShoot = value; }
        }

        /// <summary>
        /// get and set the player location
        /// </summary>
        public Vector2 Location
        {
            get { return loc; }
            set { loc = value; }
        }

        /// <summary>
        /// get and set the player rectangle
        /// </summary>
        public Rectangle Rectangle
        {
            get { return rec; }
            set { rec = value; }
        }

        /// <summary>
        /// get the shoot location offset
        /// </summary>
        public Vector2 ShootLocOffset
        {
            get { return shootLocOffset; }
        }

        /// <summary>
        /// get the player high score
        /// </summary>
        public int HighScore
        {
            get { return highScore; }
            set { highScore = value; }
        }

        public int GamesPlayed
        {
            get { return gamesPlayed; }
            set { gamesPlayed = value; }
        }

        public int ShotsFired
        {
            get { return shotsFired; }
            set { shotsFired = value; }
        }

        public int ShotsHit
        {
            get { return shotsHit; }
            set { shotsHit = value; }
        }

        public float TopHitPercent
        {
            get { return topHitPercent; }
            set { topHitPercent = value; }
        }

        public int[] MobsKilled
        {
            get { return mobsKilled; }
            set { mobsKilled = value; }
        }

        public int TotalKills
        {
            get { return totalKills; }
        }

        public float AllTimeHitPercent
        {
            get { return allTimeHitPercent; }
        }

        public float AvgShotsPerGame
        {
            get { return avgShotsPerGame; }
        }

        public float AvgKillsPerGame
        {
            get { return avgKillsPerGame; }
        }


        /// <summary>
        /// get and set the player score
        /// </summary>
        public int Score
        {
            get { return score; }
            set 
            { 
                int delta = value - score;
                // check if player has double points
                if (buffs[DOUBLE_POINTS_INDEX] && delta > 0) delta *= 2; // double point buff is handled when reading the score
                score = Math.Max(0, score + delta);
            }
        }

        /// <summary>
        /// get damage of the player
        /// </summary>
        public int Damage
        {
            get 
            {
                if (buffs[TRIPLE_DAMAGE_INDEX]) return damage * 3; // triple damage buff is handled when reading the damage
                return damage;
            }
        }

        /// <summary>
        /// get a array of player buffs
        /// </summary>
        public bool[] UsedBuffs
        {
            get { return buffs; }
        }

        public bool IsFear
        {
            get { return isFear; }
            set { isFear = value; }
        }

        public Player(SpriteBatch spriteBatch)
        {
            this.spriteBatch = spriteBatch;

            skin = Assets.alexImg;

            loc = new Vector2(Game1.SCREEN_WIDTH / 2 - WIDTH / 2, Y_LOC);
            rec = new Rectangle((int)loc.X, (int)loc.Y, WIDTH, HEIGHT);

            speed = BASE_SPEED;
        }


        /// <summary>
        /// update the player movement and shooting
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            // update player position
            UpdateLocation();

            // update player shooting
            UpdateShoot(gameTime);
        }

        /// <summary>
        /// update the players shooting
        /// </summary>
        /// <param name="gameTime"></param> used to update the shoot timer
        private void UpdateShoot(GameTime gameTime)
        {
            // update shooting timer
            shootTimer.Update(gameTime);

            if (shootTimer.IsActive()) isShoot = false;
            else if (shootTimer.IsFinished() && Game1.kb.IsKeyDown(Keys.Space) && !Game1.prevKb.IsKeyDown(Keys.Space))
            {
                // set the isShoot flag to true
                isShoot = true;

                // check if any shooting buffs
                if (buffs[DOUBLE_FIRE_RATE_INDEX])
                {
                    shootTimer.SetTargetTime(BASE_SHOOTING_DUR / 2);
                }
                else shootTimer.SetTargetTime(BASE_SHOOTING_DUR);

                // reset the timer
                shootTimer.ResetTimer(true);
            }
        }

        /// <summary>
        /// update the players location
        /// </summary>
        private void UpdateLocation()
        {
            // check if speed buff
            if (buffs[DOUBLE_SPEED_INDEX]) speed = 2 * BASE_SPEED;
            else speed = BASE_SPEED;

            // update player position base on key presses
            if (Game1.prevKb.IsKeyDown(Keys.Left) || Game1.prevKb.IsKeyDown(Keys.A)) loc.X -= speed;
            else if (Game1.prevKb.IsKeyDown(Keys.Right) || Game1.prevKb.IsKeyDown(Keys.D)) loc.X += speed;

            // clamp player position and set as rectangle
            loc.X = MathHelper.Clamp(loc.X, 0, Game1.SCREEN_WIDTH - WIDTH);
            rec.X = (int)loc.X;
        }

        /// <summary>
        /// draw the player
        /// </summary>
        public void Draw()
        {
            if (isFear) 
                spriteBatch.Draw(skin, rec, Color.PowderBlue);
            else spriteBatch.Draw(skin, rec, Color.White);
        }

        /// <summary>
        /// reset the player buffs
        /// </summary>
        public void ResetBuffs()
        {
            for (int i = 0; i < NUM_UPGRADES; i++) buffs[i] = false;

            damage = BASE_DAMAGE;
            shootTimer.SetTargetTime(BASE_SHOOTING_DUR);
            speed = BASE_SPEED;
        }

        /// <summary>
        /// add a buff
        /// </summary>
        /// <param name="buffIndex"></param>
        public void AddBuff(int buffIndex)
        {
            buffs[buffIndex] = true;
        }

        public void CalculateExtraStats()
        {
            // total kills
            foreach(int kills in mobsKilled) totalKills += kills;

            // all time hit percentage
            allTimeHitPercent = (float)shotsHit / shotsFired;

            // average shots per game
            avgShotsPerGame = (gamesPlayed == 0) ? 0 : (float)shotsFired / gamesPlayed;

            // average hit percentage
            avgShotsPerGame = (gamesPlayed == 0) ? 0 : (float)shotsHit / gamesPlayed;
        }
    }
}
