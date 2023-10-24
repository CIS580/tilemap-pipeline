using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExampleGame
{
    public class ExampleMap
    {
        public int Width { get; init; }

        public int Height { get; init; }

        public int TileWidth { get; init; }

        public int TileHeight { get; init; }

        public Color BackgroundColor { get; init; }

        public void Draw(GameTime gameTime)
        {

        }
     
    }

    public class Layer 
    {
        Tile[] Tiles { get; init; }
    
    }

    public class Tile
    {
        Rectangle Source { get; init; }

        Rectangle Bounds { get; init; }

        Texture2D Texture { get; init; }


        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Bounds, Source, Color.White);
        }
    
    }
}
