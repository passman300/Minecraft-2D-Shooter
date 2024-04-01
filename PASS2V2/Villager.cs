using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PASS2V2
{
    public class Villager : Mob
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spriteBatch"></param>
        public Villager(SpriteBatch spriteBatch) : base (spriteBatch, new Vector2(0 - HEIGHT, Game1.rng.Next(0, Game1.SCREEN_HEIGHT - HEIGHT * 2)), new Vector2(8, 0), 10, 1, 0)
        {
            skin = Assets.villagerImg;
        }

        public override void Update(GameTime gameTime, Rectangle playerRec)
        {
            switch (state)
            {
                case ALIVE:
                    // update movement
                    UpdateMovement();

                    // check if the villager has no more health
                    if (health <= 0) {state = DEAD;}
                    break;
                case DEAD:
                    deathTimer.Update(gameTime);
                    if (deathTimer.IsFinished()) state = REMOVE;
                    break;
            }
        }

        /// <summary>
        /// Update the villager's movement
        /// </summary>
        private void UpdateMovement()
        {
            curLoc.X += speed.X;
            rec.X = (int)curLoc.X;

            // check if the villager is off the screen
            if (rec.Left > Game1.SCREEN_WIDTH) state = REMOVE;
        }

        public override void Draw()
        {
            base.Draw();
        }
    }
}
