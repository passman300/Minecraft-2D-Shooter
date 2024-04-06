//Author: Colin Wang
//File Name: Villager.cs
//Project Name: PASS2 a Minecraft Shooter
//Created Date: March 28, 2024, Remade on April 1, 2024
//Modified Date: April 1, 2024
//Description: Villager class for the game, manages the villager mob movement

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace PASS2V2
{
    public class Villager : Mob
    {

        /// <summary>
        /// constructor for the villager
        /// points: 10
        /// health: 1
        /// damage: 0
        /// </summary>
        /// <param name="spriteBatch"></param>
        public Villager(SpriteBatch spriteBatch) : base (spriteBatch, new Vector2(0 - HEIGHT, Game1.rng.Next(0, Game1.SCREEN_HEIGHT - HEIGHT * 2)), new Vector2(8, 0), 10, 1, 0)
        {
            skin = Assets.villagerImg;
        }


        /// <summary>
        /// Update the villager mob
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="player"></param> not used, however is a parameter in the base class
        public override void Update(GameTime gameTime, Player player)
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
    }
}
