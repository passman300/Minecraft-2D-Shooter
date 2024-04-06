//Author: Colin Wang
//File Name: Skeleton.cs
//Project Name: PASS2 a Minecraft Shooter
//Created Date: March 29, 2024, Remade on April 1, 2024
//Modified Date: April 3, 2024
//Description: Skeleton class for the game, manages the skeleton mob, spiral movement, shooting, etc.

using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace PASS2V2
{
    public class Skeleton: Mob
    {
        /// <summary>
        /// spiral states the skeleton can be in
        /// </summary>
        public enum SpiralStates
        {
            Pre_Spiral = 0,
            Spiral = 1,
            Post_Spiral = 2
        }

        // constants of the spiral locations
        private const int STARTING_RADIUS = Game1.SCREEN_HEIGHT / 2 - (2 * HEIGHT);
        private const int STARTING_X = Game1.SCREEN_WIDTH / 2 + (STARTING_RADIUS - WIDTH / 2);

        // number of rotations, and rate of the spiral
        private const double ROTATIONS = 8 * Math.PI;
        private const float SPIRAL_RATE = 0.05f; // radians per update
        private const double RADIUS_RATE = (STARTING_RADIUS * SPIRAL_RATE) / (ROTATIONS);

        // time for the skeleton to wait to shoot
        private const int SHOOTING_DUR = 750; // ms

        // timer for when the skeleton should shoot
        private Timer shootingTimer = new Timer(SHOOTING_DUR, true);

        private SpiralStates spiralState = SpiralStates.Pre_Spiral;
        
        // current angle and radius of the spiral
        private double curRadius = STARTING_RADIUS;
        private double angle = 0;

        /// <summary>
        /// constructor for the skeleton
        /// points: 25
        /// health: 4
        /// damage: 20 (arrow damage)
        /// </summary>
        /// <param name="spriteBatch"></param>
        public Skeleton (SpriteBatch spriteBatch) : base (spriteBatch, new Vector2(STARTING_X, 0 - Mob.HEIGHT), new Vector2(3.5f, 3.5f), 25, 4, 20)
        {
            // set the mob skin
            skin = Assets.skeletonImg;
            
            // reset the timer, and isShoot flag
            isShoot = false;
            shootingTimer.ResetTimer(true);
        }


        /// <summary>
        /// base update method for the skeleton mob
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="player"></param> not used, however is a parameter in the base class
        public override void Update(GameTime gameTime, Player player)
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

        /// <summary>
        /// update the skeleton movement
        /// </summary>
        private void UpdateMovement()
        {
            // update the movement based on the spiral state
            switch (spiralState)
            {
                case SpiralStates.Pre_Spiral:
                    // update the location and rectangle
                    curLoc.Y += speed.Y;
                    rec.Y = (int)curLoc.Y;

                    // check if skeleton reach half way
                    if (curLoc.Y + HEIGHT / 2 >= Game1.SCREEN_HEIGHT / 2) spiralState = SpiralStates.Spiral;
                    break;

                case SpiralStates.Spiral:
                    UpdateSpiral(new Vector2(Game1.SCREEN_WIDTH / 2, Game1.SCREEN_HEIGHT / 2));
                    break;

                case SpiralStates.Post_Spiral:
                    // update the location
                    curLoc.X += speed.X;
                    rec.X = (int)curLoc.X;

                    // check if off the screen
                    if (rec.Left >= Game1.SCREEN_WIDTH) state = REMOVE;
                    break;
            }
        }

        /// <summary>
        /// update the location of the skeleton in the spiral
        /// </summary>
        /// <param name="screenCenter"></param> the center of the screen
        private void UpdateSpiral(Vector2 screenCenter)
        {
            // use trigonometry to calculate the location of the spiral as radius decreases
            curLoc.X = (float)(Math.Cos(angle) * curRadius + screenCenter.X) - WIDTH / 2;
            curLoc.Y = (float)(Math.Sin(angle) * curRadius + screenCenter.Y) - HEIGHT / 2;

            // update the rectangle
            rec.X = (int)curLoc.X;
            rec.Y = (int)curLoc.Y;

            // update the angle and radius of the spiral
            angle += SPIRAL_RATE;   
            curRadius -= RADIUS_RATE;

            // if the spiral radius is less than 0, the spiraling is done
            if (curRadius <= 0) spiralState = SpiralStates.Post_Spiral;
        }
    }
}
