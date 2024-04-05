using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace PASS2V2
{
    public class Pillager : Mob
    {
        // pillagers sinusoidal movement constants
        private const float ANGLE_RATE = 0.05f; // radian per update
        private const int AMPLITUDE = 100;

        // pillager angle of movement
        private double angle = 0;

        /// <summary>
        /// constructor for the pillager
        /// points: 25
        /// health: 2
        /// damage: 0
        /// </summary>
        /// <param name="spriteBatch"></param>
        public Pillager(SpriteBatch spriteBatch) : base(spriteBatch, new Vector2(Game1.SCREEN_WIDTH + WIDTH, Game1.rng.Next(AMPLITUDE, Game1.SCREEN_HEIGHT - AMPLITUDE - (2*HEIGHT))), new Vector2(3.5f, 3.5f), 25, 2, 0)
        {
            skin = Assets.pillagerImg;
            shieldImg = Assets.shieldImg;

            isShield = true;
        }

        /// <summary>
        /// base update method for the pillager
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="player"></param> not used, however is a parameter in the base class
        public override void Update(GameTime gameTime, Player player)
        {
            switch (state)
            {
                case ALIVE:
                    UpdateMovement();
                    break;
                case DEAD:
                    deathTimer.Update(gameTime);
                    if (deathTimer.IsFinished()) state = REMOVE;
                    break;
            }
        }

        /// <summary>
        /// update the pillager movement
        /// </summary>
        private void UpdateMovement()
        {
            // update location of the pillager
            curLoc.Y = (float)(Math.Sin(angle) * AMPLITUDE + spawnLoc.Y);
            curLoc.X -= speed.X; // move to the left

            rec.Y = (int)curLoc.Y;
            rec.X = (int)curLoc.X;

            // update the angle
            angle += ANGLE_RATE;

            // check if the pillager is off the screen
            if (rec.Right <= 0) state = REMOVE;
        }

        /// <summary>
        /// draw the pillager based on its state
        /// </summary>
        public override void Draw()
        {
            switch (state)
            {
                case ALIVE:
                    spriteBatch.Draw(skin, rec, Color.White);

                    // draw shield if the pillager has it
                    if (isShield) spriteBatch.Draw(shieldImg, curLoc + shieldOffset, Color.White);
                    break;
                case DEAD:
                    spriteBatch.Draw(bloodImg, rec, Color.White);
                    break;
            }
        }
    }
}
