// Ignore Spelling: spritebatch

using Animation2D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PASS2V2
{
    // types of tiles
    public enum TileTypes
    {
        Dirt = 0,
        Grass1 = 1,
        Grass2 = 2,
        Coblestone = 3
    }

    public class Tile
    {
        // dimensions of the tile in pixels
        public const int WIDTH = 64;
        public const int HEIGHT = 64;

        // dimensions of the tile in a vector2
        public static readonly Vector2 TILE_SIZE = new Vector2(WIDTH, HEIGHT);

        // local spritebatch
        private SpriteBatch spriteBatch;

        // texture of the tile
        private Texture2D texture;

        // location of the tile
        private Vector2 loc;

        // the rectangle of the tile
        private Rectangle rec;


        // property to define the location of the tile
        public Vector2 Location
        {
            get { return loc; }
            set { loc = value; }
        }

        // properties of the rectangle
        public Rectangle Rectangle
        {
            get { return rec; }
            set { rec = value; }
        }

        /// <summary>
        /// The constructor of the tile
        /// </summary>
        /// <param name="spriteBatch"></param> // pass in the spritebatch
        /// <param name="texture"></param> // texture of the tile
        /// <param name="location"></param>
        public Tile(SpriteBatch spriteBatch, TileTypes type, Vector2 location)
        {
            // set the texture of the tile based on the type
            switch (type)
            {
                case TileTypes.Grass1:
                    texture = Assets.grassImg1;
                    break;
                case TileTypes.Grass2:
                    texture = Assets.grassImg2;
                    break;
                case TileTypes.Dirt:
                    texture = Assets.dirtImg;
                    break;
                case TileTypes.Coblestone:
                    texture = Assets.coblestoneImg;
                    break;
            }

            // save the location of the tile
            loc = location;
            rec = new Rectangle((int)loc.X, (int)loc.Y, WIDTH, HEIGHT);

            // save spritebatch into local spritebatch
            this.spriteBatch = spriteBatch;
        }

        /// <summary>
        /// Draws the tile
        /// </summary>
        public void Draw()
        {
            spriteBatch.Draw(texture, loc, Color.White);
        }

    }
}
