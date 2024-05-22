using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExampleGame
{
    public class HeroTilemap
    {
        /// <summary>
        /// The tilemap's layers
        /// </summary>
        public HeroTilemapLayer[] Layers { get; init; }

        /// <summary>
        /// The hero
        /// </summary>
        public Hero Hero { get; init; }

        /// <summary>
        /// The enemies
        /// </summary>
        public List<Enemy> Enemies { get; init; }

        /// <summary>
        /// Updates the tilemap 
        /// </summary>
        /// <param name="gameTime">The game time</param>
        public virtual void Update(GameTime gameTime)
        {
            foreach (var layer in Layers)
            {
                layer.Update(gameTime);
            }
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Draw the map
            foreach (var layer in Layers)
            {
                if (layer.Visible) layer.Draw(gameTime, spriteBatch);
            }

            // Draw the hero
            Hero.Draw(gameTime, spriteBatch);
        }
    }

    /// <summary>
    /// This class represents a layer in an HeroTilemap
    /// </summary>
    public class HeroTilemapLayer
    {
        /// <summary>
        /// The tiles of the layer
        /// </summary>
        public HeroBaseTile[] Tiles { get; init; }

        /// <summary>
        /// Applies a translucency to the layer. 
        /// A value of 1 is fully opaque, 0 fully transparent
        /// </summary>
        public float Opacity { get; set; }

        /// <summary>
        /// Determines if the layer should be drawn
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// Updates all tiles in the layer
        /// </summary>
        /// <param name="gameTime">The current game time</param>
        public void Update(GameTime gameTime)
        {
            foreach (var tile in Tiles)
            {
                tile.Update(gameTime);
            }
        }

        /// <summary>
        /// Draws all tiles in the layer
        /// </summary>
        /// <param name="gameTime">The current game time</param>
        /// <param name="spriteBatch">The SpriteBatch to draw with</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!Visible) return;

            foreach (var tile in Tiles)
            {
                tile.Draw(gameTime, spriteBatch);
            }
        }
    }

    /// <summary>
    /// A base class for all tiles - can also be used to 
    /// represent a "no tile" placeholder in a 2D array
    /// of tiles when we are using tile indices to indicate
    /// position.
    /// </summary>
    public class HeroBaseTile
    {
        /// <summary>
        /// Updates the tile.  Essentially a no-op that can be overridden 
        /// in derived classes that need to update.
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Update(GameTime gameTime) { }

        /// <summary>
        /// Draws the tile. Essentially a no-op that can be overriden in
        /// derived classes.
        /// </summary>
        /// <param name="gameTime">The current game time</param>
        /// <param name="spriteBatch">The SpriteBatch to draw the map with</param>
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch) { }
    }

    /// <summary>
    /// A tile that consists of a texture in a tileset image, 
    /// and a location in the world to draw it, as well as 
    /// SpriteEffects indicating if it should be flipped
    /// </summary>
    public class HeroTile : HeroBaseTile
    {
        /// <summary>
        /// The bounds of the tile in the Texture
        /// </summary>
        public Rectangle SourceRect { get; init; }

        /// <summary>
        /// The bounds of the tile where it is placed in the world
        /// </summary>
        public Rectangle WorldRect { get; init; }

        /// <summary>
        /// The texture congtaining the tile's raster data
        /// </summary>
        public Texture2D Texture { get; init; }

        /// <summary>
        /// SpriteEffects to be applied to the tile (flipped horizontally, vertically, or both)
        /// </summary>
        public SpriteEffects SpriteEffects { get; init; }

        public bool Solid { get; init; }

        public bool Liquid { get; init; }

        /// <summary>
        /// Draws the tile using the Texture and SourceRect
        /// </summary>
        /// <param name="gameTime">The current game time</param>
        /// <param name="spriteBatch">The SpriteBatch to draw with</param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Color color = Color.White;

            if (Solid) color = Color.Red;

            spriteBatch.Draw(Texture, WorldRect, SourceRect, color, 0f, Vector2.Zero, SpriteEffects, 1f);
        }
    }
}
