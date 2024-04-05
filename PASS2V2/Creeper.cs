using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;

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

        /// <summary>
        /// get and set explode damage flag
        /// </summary>
        public bool IsExplodeApplied
        {
            get { return explodeDamageApplied; }
            set { explodeDamageApplied = value; }
        }

        /// <summary>
        /// constructor for creeper
        /// points: 40
        /// health: 3
        /// damage: 40
        /// </summary>
        /// <param name="spriteBatch"></param>
        public Creeper(SpriteBatch spriteBatch) : base (spriteBatch, new Vector2(Game1.rng.Next(0, Game1.SCREEN_WIDTH), 0 - HEIGHT), new Vector2(5, 5), 40, 3 , 40)
        {
            skin = Assets.creeperImg;
        }

        /// <summary>
        /// update the creeper
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="player"></param>
        public override void Update(GameTime gameTime, Player player)
        {
            switch (state)
            {
                case ALIVE:
                    // update creeper's movement
                    UpdateMovement(player);
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

        /// <summary>
        /// update the creeper's movement, as it moves towards the player
        /// </summary>
        /// <param name="player"></param>
        private void UpdateMovement(Player player)
        {
            // calculate the distance between the creeper and the player
            double deltaX = player.Rectangle.X - curLoc.X;
            double deltaY = player.Rectangle.Y - curLoc.Y;
            double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            // if the distance is greater than 0, normalize the movement
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
            if (IsWithinRange(player.Rectangle))
            {
                state = EXPLODE;
            }
        }

        /// <summary>
        /// draw the creeper, depending on its state (alive, explode, dead)
        /// </summary>
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

        /// <summary>
        /// returns true if a rectangle (player) is within the creeper's range
        /// </summary>
        /// <param name="playerRec"></param>
        /// <returns></returns>
        private bool IsWithinRange(Rectangle playerRec)
        {
            if (GetDistance(playerRec.Center.ToVector2(), rec.Center.ToVector2()) <= EXPLODE_RADIUS) return true;
            return false;
        }

        /// <summary>
        /// returns the distance between two points
        /// </summary>
        /// <param name="point1"></param> point 1
        /// <param name="point2"></param> point 2
        /// <returns></returns>
        private double GetDistance(Vector2 point1, Vector2 point2)
        {
            return Math.Sqrt((point1.X - point2.X) * (point1.X - point2.X) + (point1.Y - point2.Y) * (point1.Y - point2.Y));
        }


    }
}
