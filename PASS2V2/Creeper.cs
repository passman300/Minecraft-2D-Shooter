using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PASS2V2
{
    public class Creeper : Mob
    {
        // explode state
        public const int EXPLODE = 3;

        // explode radius
        public const int EXPLODE_RADIUS = 100;

        // explode image
        private Texture2D explodeImg = Assets.explodeImg;

        // explode damage flag
        private bool explodeDamageApplied = false;

        public bool IsExplodeApplied
        {
            get { return explodeDamageApplied; }
            set { explodeDamageApplied = value; }
        }

        public Creeper(SpriteBatch spriteBatch) : base (spriteBatch, new Vector2(Game1.rng.Next(0, Game1.SCREEN_WIDTH), 0 - HEIGHT), new Vector2(5, 5), 40, 3 , 40)
        {
            skin = Assets.creeperImg;
        }

        public override void Update(GameTime gameTime, Rectangle playerRec)
        {
            switch (state)
            {
                case ALIVE:
                    // update creeper's movement
                    UpdateMovement(playerRec);
                    break;
                case EXPLODE:
                    deathTimer.Update(gameTime);
                    if (deathTimer.IsFinished()) state = REMOVE;
                    break;
                case DEAD:
                    deathTimer.Update(gameTime);
                    if (deathTimer.IsFinished()) state = REMOVE;
                    break;
            }            
        }

        private void UpdateMovement(Rectangle playerRec)
        {
            double deltaX = playerRec.X - curLoc.X;
            double deltaY = playerRec.Y - curLoc.Y;

            double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            if (distance > 0)
            {
                double normalizedDeltaX = deltaX / distance;
                double normalizedDeltaY = deltaY / distance;

                curLoc.X += (float)(normalizedDeltaX * speed.X);
                curLoc.Y += (float)(normalizedDeltaY * speed.Y);
            }

            rec.X = (int)curLoc.X;
            rec.Y = (int)curLoc.Y;

            // check if the player is within the creeper's range
            if (IsWithinRange(playerRec))
            {
                state = EXPLODE;
            }
        }

        public override void Draw()
        {
            switch (state)
            {
                case ALIVE:
                    spriteBatch.Draw(skin, rec, Color.White);
                    break;
                case EXPLODE:
                    spriteBatch.Draw(explodeImg, new Vector2(rec.Center.X - explodeImg.Width / 2, rec.Center.Y - explodeImg.Height / 2), Color.White);
                    break;
                case DEAD:
                    spriteBatch.Draw(bloodImg, rec, Color.White);
                    break;
            }
        }

        private bool IsWithinRange(Rectangle playerRec)
        {
            if (GetDistance(playerRec.Center.ToVector2(), rec.Center.ToVector2()) <= EXPLODE_RADIUS) return true;
            return false;
        }

        private double GetDistance(Vector2 point1, Vector2 point2)
        {
            return Math.Sqrt((point1.X - point2.X) * (point1.X - point2.X) + (point1.Y - point2.Y) * (point1.Y - point2.Y));
        }


    }
}
