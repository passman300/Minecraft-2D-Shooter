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
    internal class Enderman : Mob
    {
        public Enderman(SpriteBatch spriteBatch) : base(spriteBatch, Vector2.Zero, Vector2.Zero, 0, 0, 0)
        {
            skin = Assets.endermanImg;
        }
    }
}
