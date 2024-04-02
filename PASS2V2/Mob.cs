using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PASS2V2
{
    public class Mob
    {
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
        protected Texture2D sheildImg;

		// location and rectangle of the mob
		protected Vector2 spawnLoc;
		protected Vector2 curLoc;
		protected Rectangle rec;

		protected Vector2 speed;

		protected bool isShoot = false;
		protected Vector2 shootLocOffset = new Vector2(WIDTH / 2, 0);

        protected bool isSheild = false;
        protected Vector2 shieldOffset = new Vector2(WIDTH * 0.45f, HEIGHT - Assets.shieldImg.Height);

        protected int health;
		protected int damage;
		protected int points;

        public bool IsShoot
        {
            get { return isShoot; }
        }

        public int State
        {
            get { return state; }
            set { state = value; }
        }

        public Rectangle Rectangle
        {
            get { return rec; }
        }

        public Vector2 Location
        {
            get { return curLoc; }
        }

        public int Health
        {
            get { return health; }
            set { health = value; }
        }

        public int Damage
        {
            get { return damage; }
            set { damage = value; }
        }

        public int Points
        {
            get { return points; }
            set { points = value; }
        }

        public Vector2 ShootLocOffset
        {
            get { return shootLocOffset; }
        }

        public bool IsSheild
        {
            get { return isSheild; }
            set { isSheild = value; }
        }

        public Mob(SpriteBatch spriteBatch, Vector2 spawnLoc, Vector2 speed, int points, int health, int damage)
        {
            this.spriteBatch = spriteBatch;

            this.spawnLoc = spawnLoc;
            curLoc = spawnLoc;

            rec = new Rectangle((int)curLoc.X, (int)curLoc.Y, WIDTH, HEIGHT);

            this.speed = speed;

            this.points = points;
            this.health = health;
            this.damage = damage;

            bloodImg = Assets.bloodImgImg;
        }

        /// <summary>
        /// base update method for the mob
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="playerRec"></param>
        public virtual void Update(GameTime gameTime, Rectangle playerRec) {        }

        /// <summary>
        /// base draw method for the mab
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
