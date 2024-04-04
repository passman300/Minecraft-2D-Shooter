using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PASS2V2
{
    public class Skeleton: Mob
    {
        public enum SprialStates
        {
            Pre_Sprial = 0,
            Sprial = 1,
            Post_Sprial = 2
        }

        private const int STARTING_RADIUS = Game1.SCREEN_HEIGHT / 2 - (2 * HEIGHT);
        private const int STARTING_X = Game1.SCREEN_WIDTH / 2 + (STARTING_RADIUS - WIDTH / 2);

        private const double ROTATIONS = 8 * Math.PI;
        private const float SPRIAL_RATE = 0.05f; // radians per update
        private const double RADIUS_RATE = (STARTING_RADIUS * SPRIAL_RATE) / (ROTATIONS);

        private const int SHOOTING_DUR = 750; // ms

        private Timer shootingTimer = new Timer(SHOOTING_DUR, true);

        private SprialStates sprialState = SprialStates.Pre_Sprial;
        
        private double curRadius = STARTING_RADIUS;
        private double angle = 0;


        public Skeleton (SpriteBatch spriteBatch) : base (spriteBatch, new Vector2(STARTING_X, 0 - Mob.HEIGHT), new Vector2(3.5f, 3.5f), 25, 4, 20)
        {
            // set the mob skin
            skin = Assets.skeletonImg;
            
            // reset the timer, and isShoot flag
            isShoot = false;
            shootingTimer.ResetTimer(true);
        }

        public override void Update(GameTime gameTime, Rectangle playerRec)
        {
            switch (state)
            {
                case ALIVE:
                    UpdateShooting(gameTime);
                    UpdateMovement();
                    break;
                case DEAD:
                    deathTimer.Update(gameTime);
                    if (deathTimer.IsFinished()) state = REMOVE;
                    break;
            }
        }


        /// <summary>
        /// updates the timer for when the skeleton should shoot
        /// </summary>
        /// <param name="gameTime"></param>
        private void UpdateShooting(GameTime gameTime)
        {
            shootingTimer.Update(gameTime);

            if (!shootingTimer.IsActive())
            {
                isShoot = true;
                shootingTimer.ResetTimer(true);
            }
            else isShoot = false;
        }

        private void UpdateMovement()
        {
            switch (sprialState)
            {
                case SprialStates.Pre_Sprial:
                    curLoc.Y += speed.Y;
                    rec.Y = (int)curLoc.Y;

                    // check if skeleton reach half way
                    if (curLoc.Y + HEIGHT / 2 >= Game1.SCREEN_HEIGHT / 2)
                    {
                        sprialState = SprialStates.Sprial;
                    }
                    break;
                case SprialStates.Sprial:
                    UpdateSprial(new Vector2(Game1.SCREEN_WIDTH / 2, Game1.SCREEN_HEIGHT / 2));
                    break;
                case SprialStates.Post_Sprial:
                    curLoc.X += speed.X;
                    rec.X = (int)curLoc.X;

                    // check if off the screen
                    if (rec.Left >= Game1.SCREEN_WIDTH)
                    {
                        state = REMOVE;
                    }

                    break;
            }
        }

        private void UpdateSprial(Vector2 screenCenter)
        {
            curLoc.X = (float)(Math.Cos(angle) * curRadius + screenCenter.X) - WIDTH / 2;
            curLoc.Y = (float)(Math.Sin(angle) * curRadius + screenCenter.Y) - HEIGHT / 2;
            Console.WriteLine(curLoc);

            rec.X = (int)curLoc.X;
            rec.Y = (int)curLoc.Y;

            // update the angle and radius of the spiral
            angle += SPRIAL_RATE;   
            curRadius -= RADIUS_RATE;

            // if the spiral radius is less than 0, the spiraling is done
            if (curRadius <= 0) sprialState = SprialStates.Post_Sprial;
        }

        public override void Draw()
        {
            base.Draw();
        }
    }
}
