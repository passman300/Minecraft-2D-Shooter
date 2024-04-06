//Author: Colin Wang
//File Name: Arrow.cs
//Project Name: PASS2 a Minecraft Shooter
//Created Date: March 29, 2024, Remade on April 1, 2024
//Modified Date: April 1, 2024
//Description: Arrow class for the game, manages the arrow movement, direction, damage, color, etc.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PASS2V2
{
    public class Arrow
    {
        /// <summary>
        /// types of arrows direction
        /// </summary>
        public enum ArrowDirection
        {
            Up = 0,
            Down = 1,
        }

        /// <summary>
        /// types of arrows
        /// </summary>
        public enum ArrowState
        {
            Remove = -1,
            Active = 0
        }

        // arrow dimensions
        public const int WIDTH = 19;
        public const int HEIGHT = 64;

        // arrow base speed, without buff and modifier
        public const int BASE_SPEED = 10;

        // local spritebatch
        private SpriteBatch spriteBatch;

        // image of the arrow
        private Texture2D img;

        // location and rectangle of the arrow
        private Vector2 loc;
        private Rectangle rec;

        // speed and damage of the arrow
        private int speed;
        private int damage;

        // direction and state of the arrow
        private ArrowDirection direction;
        private ArrowState state;

        // color of the arrow
        private Color color;

        /// <summary>
        /// get and set the state of the arrow
        /// </summary>
        public ArrowState State
        {
            get { return state; }
            set { state = value; }
        }

        /// <summary>
        /// get the direction of the arrow
        /// </summary>
        public ArrowDirection Direction
        {
            get { return direction; }
        }

        /// <summary>
        /// get the damage of the arrow
        /// </summary>
        public int Damage 
        { 
            get { return damage; }
        }

        /// <summary>
        /// get the location of the arrow
        /// </summary>
        public Vector2 Location
        {
            get { return loc; }
        }

        /// <summary>
        /// get the rectangle of the arrow
        /// </summary>
        public Rectangle Rectangle
        {
            get { return rec; }
        }

        /// <summary>
        /// constructor for the arrow
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="loc"></param> the location of the arrow starting point
        /// <param name="direction"></param> the direction of the arrow
        /// <param name="color"></param> the color of the arrow
        /// <param name="damage"></param> the damage of the arrow
        /// <param name="speed"></param> the speed of the arrow
        public Arrow(SpriteBatch spriteBatch, Vector2 loc, ArrowDirection direction, Color color, int damage, int speed = BASE_SPEED)
        {
            this.spriteBatch = spriteBatch;
            this.direction = direction;

            this.color = color;

            // set the image of the arrow based on the direction
            if (direction == ArrowDirection.Up)
            {
                img = Assets.arrowUpImg;
                this.loc = new Vector2(loc.X - WIDTH / 2, loc.Y - HEIGHT);
            }
            else
            {
                img = Assets.arrowDownImg;
                this.loc = new Vector2(loc.X - WIDTH / 2, loc.Y + HEIGHT);
            }

            // set the rectangle of the arrow
            rec = new Rectangle((int)loc.X, (int)loc.Y, WIDTH, HEIGHT);
            
            // set the speed and damage
            this.damage = damage;
            this.speed = speed;
        }

        /// <summary>
        /// update the arrow
        /// </summary>
        public void Update()
        {
            // update the location of the arrow, and rectangle base on the direction
            if (direction == ArrowDirection.Up) loc.Y -= speed;
            else loc.Y += speed;
            rec.Y = (int)loc.Y;

            // check if the arrow is off the screen
            if (rec.Bottom <= 0 || rec.Top >= Game1.SCREEN_HEIGHT) state = ArrowState.Remove;
        }

        /// <summary>
        /// draw the arrow
        /// </summary>
        public void Draw()
        {
            // note the arrows, hitbox (rec) is off by one pixel to the real location
            // due to rounding half the width of the arrow
            spriteBatch.Draw(img, loc, color);
        }

    }
}
