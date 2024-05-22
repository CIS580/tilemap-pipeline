using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Graphics;

namespace TilemapPipeline
{
    /// <summary>
    /// A class representing a Hero's information
    /// </summary>
    [ContentSerializerRuntimeType("ExampleGame.Hero, ExampleGame")]
    public class HeroContent
    {
        /// <summary>
        /// The hero's texture
        /// </summary>
        public Texture2DContent Texture;

        /// <summary>
        /// The hero's position in the map
        /// </summary>
        public Vector2 Position;
    }

    /// <summary>
    /// A class representing an Enemy's information
    /// </summary>
    [ContentSerializerRuntimeType("ExampleGame.Enemy, ExampleGame")]
    public class EnemyContent
    {
        /// <summary>
        /// The enemy's starting health
        /// </summary>
        public int Health;

        /// <summary>
        /// The enemy's position in the map
        /// </summary>
        public Vector2 Position;
    }

    /// <summary>
    /// A representation of tile layers for the HeroTilemapContent
    /// </summary>
    [ContentSerializerRuntimeType("ExampleGame.HeroTilemapLayer, ExampleGame")]
    public class HeroTilemapLayerContent
    {
        /// <summary>
        /// The tiles in the layer.
        /// </summary>
        public HeroBaseTileContent[] Tiles { get; set; }

        /// <summary>
        /// The opacity of the layer
        /// </summary>
        public float Opacity { get; set; } = 1.0f;

        /// <summary>
        /// The visibility of the layer
        /// </summary>
        public bool Visible { get; set; } = true;
    }


    /// <summary>
    /// The base tile representation for an HeroTilemapContent.  
    /// Note that it contains no information - the base tile is
    /// a placeholder for a "no tile here".
    /// </summary>
    [ContentSerializerRuntimeType("ExampleGame.HeroBaseTile, ExampleGame")]
    public class HeroBaseTileContent
    {

    }


    /// <summary>
    /// A representation for a textured tile in the HeroTilemapContent 
    /// </summary>
    [ContentSerializerRuntimeType("ExampleGame.HeroTile, ExampleGame")]
    public class HeroTileContent : HeroBaseTileContent
    {
        /// <summary>
        /// The portion of the Texture used for this tile
        /// </summary>
        public Rectangle SourceRect { get; set; }

        /// <summary>
        /// The bounds of the tile in the game world
        /// </summary>
        public Rectangle WorldRect { get; set; }

        /// <summary>
        /// The texture used by this tile
        /// </summary>
        public ExternalReference<Texture2DContent> Texture { get; init; }

        /// <summary>
        /// Indicates if this tile should be flipped in some way
        /// </summary>
        public SpriteEffects SpriteEffects { get; init; }

        /// <summary>
        /// If this tile is a solid 
        /// </summary>
        public bool Solid { get; init; } = false;

        /// <summary>
        /// If this tile represents a liquid
        /// </summary>
        public bool Liquid { get; init; } = false;
    }

    /// <summary>
    /// A class representing a Tilemap with a hero in it
    /// </summary>
    [ContentSerializerRuntimeType("ExampleGame.HeroTilemap, ExampleGame")]
    public class HeroTilemapContent
    {
        /// <summary>
        /// The tile layers, which are composed of tiles
        /// </summary>
        public HeroTilemapLayerContent[] Layers { get; set; }

        /// <summary>
        /// The hero's starting location (and appearance) in the map
        /// </summary>
        public HeroContent Hero = new();

        /// <summary>
        /// A list of enemy starting locations (and health) in the map
        /// </summary>
        public List<EnemyContent> Enemies = new();
    }
}
