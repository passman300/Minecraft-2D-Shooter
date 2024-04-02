using Microsoft.Xna.Framework;
using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PASS2V2
{
    public class Pillager : Mob
    {
        private const float ANGLE_RATE = 0.05f; // radian per update
        private const int AMPLITUDE = 100;

        private double angle = 0;

        public Pillager(SpriteBatch spriteBatch) : base(spriteBatch, new Vector2(Game1.SCREEN_WIDTH + WIDTH, Game1.rng.Next(AMPLITUDE, Game1.SCREEN_HEIGHT - AMPLITUDE - (2*HEIGHT))), new Vector2(3.5f, 3.5f), 25, 2, 0)
        {
            skin = Assets.pillagerImg;
            sheildImg = Assets.shieldImg;

            isSheild = true;
        }

        public override void Update(GameTime gameTime, Rectangle playerRec)
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

        private void UpdateMovement()
        {
            // update location of the pillager
            curLoc.Y = (float)(Math.Sin(angle) * AMPLITUDE + spawnLoc.Y);
            curLoc.X -= speed.X;

            rec.Y = (int)curLoc.Y;
            rec.X = (int)curLoc.X;

            // update the angle
            angle += ANGLE_RATE;

            // check if the pillager is off the screen
            if (rec.Right <= 0) state = REMOVE;
        }

        public override void Draw()
        {
            switch (state)
            {
                case ALIVE:
                    spriteBatch.Draw(skin, rec, Color.White);

                    if (isSheild)
                    {
                        spriteBatch.Draw(sheildImg, curLoc + shieldOffset, Color.White);
                    }
                    break;
                case DEAD:
                    spriteBatch.Draw(bloodImg, rec, Color.White);
                    break;
            }
        }
    }
}
