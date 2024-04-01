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
    public class Arrow
    {
        public enum ArrowDirection
        {
            Up = 0,
            Down = 1,
        }

        public enum ArrowState
        {
            Remove = -1,
            Active = 0
        }

        public const int WIDTH = 19;
        public const int HEIGHT = 64;

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

        // direction of the arrow
        private ArrowDirection direction;

        private ArrowState state;

        public ArrowState State
        {
            get { return state; }
            set { state = value; }
        }

        public ArrowDirection Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        public int Damage 
        { 
            get { return damage; }
            set { damage = value; }
        }

        /// <summary>
        /// The location of the arrow
        /// </summary>
        public Vector2 Location
        {
            get { return loc; }
            set { loc = value; }
        }

        public Rectangle Rectangle
        {
            get { return rec; }
            set { rec = value; }
        }


        public Arrow(SpriteBatch spriteBatch, Vector2 loc, ArrowDirection direction, int damage, int speed = BASE_SPEED)
        {
            this.spriteBatch = spriteBatch;
            this.direction = direction;

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

            rec = new Rectangle((int)loc.X, (int)loc.Y, WIDTH, HEIGHT);
            
            this.damage = damage;
            this.speed = speed;
        }

        public void Update()
        {
            if (direction == ArrowDirection.Up) loc.Y -= speed;
            else loc.Y += speed;

            rec.Y = (int)loc.Y;

            // check if the arrow is off the screen
            if (rec.Bottom <= 0 || rec.Top >= Game1.SCREEN_HEIGHT) state = ArrowState.Remove;
        }

        public void Draw()
        {
            spriteBatch.Draw(img, rec, Color.White);
        }

    }
}
