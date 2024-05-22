using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace TilemapPipeline
{
    [ContentProcessor(DisplayName = "HeroTilemap Processor")]
    public class HeroTilemapProcessor : ContentProcessor<TiledMapContent, HeroTilemapContent>
    {

        private struct HeroTileInfo
        {
            public ExternalReference<Texture2DContent> Texture;

            public Rectangle SourceRect;

            public bool Solid;

            public bool Liquid;
        }

        public override HeroTilemapContent Process(TiledMapContent input, ContentProcessorContext context)
        {
            // Create our output object
            HeroTilemapContent heroMap = new();

            // Convert the tilesets into TileInfo structs
            List<HeroTileInfo> tiles = ProcessTilesets(input.TileWidth, input.TileHeight, input.Tilesets, context);
                        
            // Process the layers using the processed tiles
            heroMap.Layers = ProcessLayers(input.TileLayers, tiles, input.TileWidth, input.TileHeight, context).ToArray();

            // Create our hero object 
            heroMap.Hero = new();

            context.Logger.LogMessage(input.ObjectGroups.Count + " object groups found");
            foreach(var objG in input.ObjectGroups)
            {
                context.Logger.LogMessage(objG.Name);
            }

            // Get the sprites objectgroup from the Tiled data
            var objGroup = input.ObjectGroups.Find(group => (group.Name == "sprites"));
            context.Logger.LogMessage("Sprites is " + objGroup.Name);

            // Get the hero object from the Tiled data 
            var heroObj = objGroup.Objects.Find(obj => obj.Name == "hero");
            context.Logger.LogMessage("Hero is " + heroObj.Name);

            context.Logger.LogMessage("Hero location at " + heroObj.X + "," + heroObj.Y);

            // Get the hero's image path from the hero object's properties
            string path = heroObj.Properties["image"];
            context.Logger.LogMessage("Image is " + path);

            // Build the hero texture
            Texture2DContent texture = context.BuildAndLoadAsset<TextureContent, Texture2DContent>(new ExternalReference<TextureContent>(path), "TextureProcessor");

            // Save the Hero in the HeroTilemapContent object 
            heroMap.Hero = new()
            {
                Position = new Vector2(heroObj.X, heroObj.Y),
                Texture = texture
            };

            // get the enemies objects from the Tiled data 
            var enemiesObjs = objGroup.Objects.FindAll(obj => obj.Name == "enemy");
            
            context.Logger.LogMessage("FOUND " + enemiesObjs.Count + " ENEMIES");

            // create a list to hold enemies
            List<EnemyContent> enemies = new List<EnemyContent>();

            // process each enemy
            foreach (var enemyObj in enemiesObjs)
            {
                // Get the enemy's position
                context.Logger.LogMessage("Object location at " + enemyObj.X + "," + enemyObj.Y);

                // Get the enemy's health from the enemy object's properties
                int health = 10;
                //int.TryParse(enemyObj.Properties["health"], out health);
                context.Logger.LogMessage(enemyObj.Properties["health"]);
                
                // create the enemy
                EnemyContent enemy = new()
                {
                    Position = new Vector2(enemyObj.X, enemyObj.Y),
                    Health = health
                };

                // add the enemy to the list of enemies
                enemies.Add(enemy);
            }

            // store the enemies in the heromap
            heroMap.Enemies = enemies;

            // Return the processed map
            return heroMap;
        }


        /// <summary>
        /// This helper method processes the tilesets in a Tiled map,
        /// returning an array of TileInfo objects populated with thier
        /// data.
        /// </summary>
        /// <param name="tileWidth"></param>
        /// <param name="tileHeight"></param>
        /// <param name="tilesets"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private List<HeroTileInfo> ProcessTilesets(int tileWidth, int tileHeight, List<TilesetContent> tilesets, ContentProcessorContext context)
        {
            // Create a list 
            List<HeroTileInfo> processedTiles = new();

            foreach (TilesetContent tileset in tilesets)
            {
                // Load the texture as an external reference, so we can embed it
                // into our tiles
                ExternalReference<Texture2DContent> texture = context.BuildAsset<TextureContent, Texture2DContent>(new ExternalReference<TextureContent>(tileset.ImageFilename), "TextureProcessor");

                // Also load the texture directly so we can access its dimensions
                Texture2DContent image = context.BuildAndLoadAsset<TextureContent, Texture2DContent>(new ExternalReference<TextureContent>(tileset.ImageFilename), "TextureProcessor");

                // Now we can determine the width and height of the texture
                // The first mipmap will be the original texture.
                int textureWidth = image.Mipmaps[0].Width;
                int textureHeight = image.Mipmaps[0].Height;

                // With the width and height, we can determine the number of tiles in the texture
                // Note that Tiled allows for tilesets with both margins and spacing between tiles
                int tilesetColumns = (textureWidth - 2 * tileset.Margin) / (tileWidth + tileset.Spacing);
                int tilesetRows = (textureHeight - 2 * tileset.Margin) / (tileHeight + tileset.Spacing);

                // Tiles begin with ID of 1 (0 is used for no tile)
                int currentTileIndex = 1;

                // We need to create the bounds for each tile in the tileset image
                for (int y = 0; y < tilesetRows; y++)
                {
                    for (int x = 0; x < tilesetColumns; x++)
                    {
                        // Get the properties for the current tile (if any)
                        Dictionary<string, string> tileProperties = new();
                        tileset.TileProperties.TryGetValue(currentTileIndex, out tileProperties);

                        // Special tile properties need to be initilized to default values, in 
                        // case they aren't defined
                        bool solid = false;
                        bool liquid = false;

                        if(tileProperties != null)
                        {
                            bool.TryParse(tileProperties.GetValueOrDefault("soild", "false"), out solid);
                            bool.TryParse(tileProperties.GetValueOrDefault("liquid", "false"), out liquid);
                        }

                        // The Tiles array provides the source rectangle for a tile
                        // within the tileset texture, and the texture
                        processedTiles.Add(new HeroTileInfo()
                        {
                            Texture = texture,
                            SourceRect = new Rectangle(
                                x * (tileWidth + tileset.Spacing) + tileset.Margin,
                                y * (tileHeight + tileset.Spacing) + tileset.Margin,
                                tileWidth,
                                tileHeight
                            ),
                            Solid = solid,
                            Liquid = liquid
                        });
                    }
                }
            }

            // We can then return the processed tiles
            return processedTiles;
        }

        private List<HeroTilemapLayerContent> ProcessLayers(List<TiledLayerContent> layers, List<HeroTileInfo> tileInfoList, int tileWidth, int tileHeight, ContentProcessorContext context)
        {
                        
            List<HeroTilemapLayerContent> processedLayers = new();

            foreach (var layer in layers)
            {
                context.Logger.LogMessage("Processing Layer " + layer.Name);

                // Create a list to hold the tiles we will be generating
                List<HeroBaseTileContent> tiles = new();

                // For each tile in our layer, we need to create a corresponding
                // object to store in our new layers
                for (int y = 0; y < layer.Height; y++)
                {
                    for (int x = 0; x < layer.Width; x++)
                    {
                        // Calculate the 1d index for our 2d position
                        int index = y * layer.Width + x;

                        // Find the specific tile index for this tile
                        // in the map. Remember, a 0 stands for no tile, 
                        // so we shift by 1.
                        int tileIndex = layer.TileIndices[index] - 1;

                        // If the tileIndex is -1, then there is no tile
                        // at this spot in the map. We need to have *something*
                        // there, so we'll fill it with a basic tile
                        // and move on to the next iteration
                        if (tileIndex == -1)
                        {
                            tiles.Add(new HeroBaseTileContent());
                            continue;
                        }

                        // Otherwise, we need to create a textured tile for this
                        // tile location.
                        var tileInfo = tileInfoList[tileIndex];
                        tiles.Add(new HeroTileContent()
                        {
                            Texture = tileInfo.Texture,
                            SourceRect = tileInfo.SourceRect,
                            WorldRect = new Rectangle()
                            {
                                X = x * tileWidth,
                                Y = y * tileHeight,
                                Width = tileWidth,
                                Height = tileHeight
                            },
                            SpriteEffects = layer.SpriteEffects[tileIndex],
                            Solid = tileInfo.Solid,
                            Liquid = tileInfo.Liquid
                        });
                    }
                }

                // Add the processed layer to the collection
                processedLayers.Add(new HeroTilemapLayerContent()
                {
                    Tiles = tiles.ToArray(),
                    Opacity = layer.Opacity
                });
            }

            foreach(var layer in processedLayers)
            {
                context.Logger.LogMessage("layer w/ " + layer.Tiles.Count());
            }

            return processedLayers;
        }
    }
}
