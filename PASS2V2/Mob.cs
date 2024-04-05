using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace PASS2V2
{
    public class Mob
    {
        public const int MOB_COUNT = 5;

        // mob dimensions
        public const int WIDTH = 64;
		public const int HEIGHT = 64;

        // mob stats
        public const int REMOVE = -1;
        public const int ALIVE = 0;
        public const int DEAD = 1;

        // death timer duration
        protected const int DEATH_DUR = 250; // ms

		protected SpriteBatch spriteBatch;

        // state of the mob
        protected int state = ALIVE;

        // death timer for how long the bloodImg remains
        protected Timer deathTimer = new Timer(DEATH_DUR, true);

		// textures of the mob
		protected Texture2D skin;
		protected Texture2D bloodImg;
        protected Texture2D shieldImg;

		// location and rectangle of the mob
		protected Vector2 spawnLoc;
		protected Vector2 curLoc;
		protected Rectangle rec;

        // speed of the mob
		protected Vector2 speed;

        // flag, and offset for shooting (used for the skeleton)
		protected bool isShoot = false;
		protected Vector2 shootLocOffset = new Vector2(WIDTH / 2, 0);

        // flag, and offset for shield (used for the pillager)
        protected bool isShield = false;
        protected Vector2 shieldOffset = new Vector2(WIDTH * 0.45f, HEIGHT - Assets.shieldImg.Height);

        // mob stats
        protected int health;
		protected int damage;
		protected int points;

        /// <summary>
        /// get the mob's isShoot flag
        /// </summary>
        public bool IsShoot
        {
            get { return isShoot; }
        }

        /// <summary>
        /// get or set the mob's state
        /// </summary>
        public int State
        {
            get { return state; }
            set { state = value; }
        }

        /// <summary>
        /// get the mob's rectangle
        /// </summary>
        public Rectangle Rectangle
        {
            get { return rec; }
        }

        /// <summary>
        /// get the mob's location
        /// </summary>
        public Vector2 Location
        {
            get { return curLoc; }
        }

        /// <summary>
        /// get or set the mob's health
        /// </summary>
        public int Health
        {
            get { return health; }
            set { health = value; }
        }

        /// <summary>
        /// get the mob's damage
        /// </summary>
        public int Damage
        {
            get { return damage; }
        }

        /// <summary>
        /// get the mob's points
        /// </summary>
        public int Points
        {
            get { return points; }
        }

        /// <summary>
        /// get or set the mob's shootLocOffset
        /// </summary>
        public Vector2 ShootLocOffset
        {
            get { return shootLocOffset; }
        }

        /// <summary>
        /// get or set the mob's isShield flag
        /// </summary>
        public bool IsShield
        {
            get { return isShield; }
            set { isShield = value; }
        }

        /// <summary>
        /// constructor for the mob
        /// </summary>
        /// <param name="spriteBatch"></param> pass in the spritebatch 
        /// <param name="spawnLoc"></param> the spawn location of the mob
        /// <param name="speed"></param> the speed of the mob, in the x and y direction
        /// <param name="points"></param> the points of the mob
        /// <param name="health"></param> the health of the mob
        /// <param name="damage"></param> the damage of the mob
        public Mob(SpriteBatch spriteBatch, Vector2 spawnLoc, Vector2 speed, int points, int health, int damage)
        {
            // save the spritebatch
            this.spriteBatch = spriteBatch;

            // set the spawn location
            this.spawnLoc = spawnLoc;
            curLoc = spawnLoc;

            // set the rectangle
            rec = new Rectangle((int)curLoc.X, (int)curLoc.Y, WIDTH, HEIGHT);

            // set the speed
            this.speed = speed;

            // set the stats
            this.points = points;
            this.health = health;
            this.damage = damage;

            // set the texture for blood
            bloodImg = Assets.bloodImgImg;
        }

        /// <summary>
        /// base update method for the mob
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="playerRec"></param>
        public virtual void Update(GameTime gameTime, Player playerRec) {        } // nothing to update as all children have overridden the update method

        /// <summary>
        /// base draw method for the mob
        /// </summary>
        public virtual void Draw()
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
