using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.Xml;
using System.IO;
using MonoGame.Framework.Utilities.Deflate;
using Microsoft.Xna.Framework.Graphics;

namespace TilemapPipeline
{
    [ContentImporter(".tmx", DisplayName = "TiledImporter", DefaultProcessor = "TiledProcessor")]
    public class TiledImporter : ContentImporter<MapContent>
    {
        private ContentImporterContext Context { get; set; }

        public override MapContent Import(string filename, ContentImporterContext context)
        {
            Context = context;
            
            XmlReaderSettings settings = new();
            settings.DtdProcessing = DtdProcessing.Parse;

            using var stream = System.IO.File.OpenText(filename);
            using XmlReader reader = XmlReader.Create(stream, settings);

            var map = new MapContent();

            while (reader.Read())
            {
                var name = reader.Name;

                switch(reader.NodeType)
                {
                    case XmlNodeType.DocumentType:
                        if (name != "map")
                            throw new Exception("Invalid Map Format");
                        break;
                    case XmlNodeType.Element:
                        switch(name)
                        {
                            case "map":
                                {
                                    map.Width = int.Parse(reader.GetAttribute("width"));
                                    map.Height = int.Parse(reader.GetAttribute("height"));
                                    map.TileWidth = int.Parse(reader.GetAttribute("tilewidth"));
                                    map.TileHeight = int.Parse(reader.GetAttribute("tileheight"));
                                }
                                break;
                            case "tileset":
                                {
                                    using var st = reader.ReadSubtree();
                                    st.Read();
                                    var tileset = LoadTileset(st);
                                    map.Tilesets.Add(tileset);  
                                }
                                break;
                            case "layer":
                                {
                                    using var st = reader.ReadSubtree();
                                    st.Read();
                                    var layer = LoadLayer(st);
                                    map.TileLayers.Add(layer);
                                }
                                break;
                            case "objectgroup":
                                {

                                }
                                break;
                            case "properties":
                                {

                                }
                                break;
                            default:
                                context.Logger.LogMessage($"Unhandled XML Element {name}");
                                break;
                        }
                        break;
                    case XmlNodeType.EndElement:
                            break;
                    case XmlNodeType.Whitespace:
                            break;
                    default:
                        context.Logger.LogMessage($"Unhandled XML Element {name}");
                        break;
                }
            }

            context.Logger.LogMessage("Dimensions:");
            context.Logger.LogMessage($"{map.TileWidth}");
            context.Logger.LogMessage($"{map.TileHeight}");


            return map;
        }

        private const uint FlippedHorizontallyFlag = 0x80000000;
        private const uint FlippedVerticallyFlag = 0x40000000;
        private const uint FlippedDiagonallyFlag = 0x20000000;

        private TilesetContent LoadTileset(XmlReader reader)
        {
            TilesetContent tileset = new();

            // Load required attributes
            tileset.Name = reader.GetAttribute("name");
            tileset.FirstTileId = int.Parse(reader.GetAttribute("firstgid"));
            tileset.TileWidth = int.Parse(reader.GetAttribute("tilewidth"));
            tileset.TileHeight = int.Parse(reader.GetAttribute("tileheight"));
            
            // Load optional attributes
            int.TryParse(reader.GetAttribute("margin"), out tileset.Margin);
            int.TryParse(reader.GetAttribute("spacing"), out tileset.Spacing);

            int currentTileId = -1;

            // Process the nested nodes
            while(reader.Read())
            {
                var name = reader.Name;

                switch(reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch(name)
                        {
                            case "image":
                                tileset.ImageFilename = reader.GetAttribute("source");
                                break;
                            case "tile":
                                currentTileId = int.Parse(reader.GetAttribute("id"));
                                break;
                            case "property":
                                // TODO: Finish property
                                break;
                        }
                        break;
                    case XmlNodeType.EndElement:
                        break;
                }
            }

            return tileset;
        }



        public LayerContent LoadLayer(XmlReader reader)
        {
            LayerContent layer = new();

            layer.Name = reader.GetAttribute("name");
            int.TryParse(reader.GetAttribute("width"), out layer.Width);
            int.TryParse(reader.GetAttribute("height"), out layer.Height);
            float.TryParse(reader.GetAttribute("opacity"), out layer.Opacity);

            layer.TileIndices = new int[layer.Width * layer.Height];
            layer.SpriteEffects = new SpriteEffects[layer.Width * layer.Height];

            while(reader.Read())
            {
                var name = reader.Name;

                switch(reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch(name)
                        {
                            case "data":
                                {
                                    if(reader.GetAttribute("encoding") != null)
                                    {
                                        var encoding = reader.GetAttribute("encoding");
                                        var compressor = reader.GetAttribute("compression");
                                        switch (encoding)
                                        {
                                            case "base64":
                                                {
                                                    int dataSize = (layer.Width * layer.Height * 4) + 1024;
                                                    var buffer = new byte[dataSize];
                                                    reader.ReadElementContentAsBase64(buffer, 0, dataSize);

                                                    Stream stream = new MemoryStream(buffer, false);
                                                    if (compressor == "gzip")
                                                        stream = new GZipStream(stream, CompressionMode.Decompress, false);

                                                    using (stream)
                                                    using (var br = new BinaryReader(stream))
                                                    {
                                                        for (int i = 0; i < layer.TileIndices.Length; i++)
                                                        {
                                                            uint tileData = br.ReadUInt32();

                                                            SpriteEffects spriteEffects = 0;
                                                            if ((tileData & FlippedHorizontallyFlag) != 0)
                                                            {
                                                                spriteEffects |= SpriteEffects.FlipHorizontally;
                                                                Context.Logger.LogMessage("Flipped Horizontally");
                                                            }
                                                            if ((tileData & FlippedVerticallyFlag) != 0)
                                                            {
                                                                spriteEffects |= SpriteEffects.FlipVertically;
                                                                Context.Logger.LogMessage("Flipped Vertically");
                                                            }
                                                            if ((tileData & FlippedDiagonallyFlag) != 0)
                                                            {
                                                                spriteEffects |= SpriteEffects.FlipVertically & SpriteEffects.FlipHorizontally;
                                                                Context.Logger.LogMessage("Flipped Horizontally and Vertically");
                                                            }

                                                            layer.SpriteEffects[i] = spriteEffects;

                                                            // Clear flipped bits before storing tile data
                                                            tileData &= ~(FlippedHorizontallyFlag | FlippedVerticallyFlag | FlippedDiagonallyFlag);
                                                            Context.Logger.LogMessage(i + " is " + tileData);

                                                            layer.TileIndices[i] = (int)tileData;
                                                        }
                                                    }
                                                    continue;
                                                }

                                            default:
                                                throw new Exception("Unrecognized encoding.");
                                        }
                                    }
                                    else
                                    {
                                        // TODO: Read tiles directly
                                    }
                                    break;
                                }
                            case "properties":
                                {
                                    // TODO: Read properties
                                    break;
                                }

                        }
                        break;
                    case XmlNodeType.EndElement:
                        break;
                }
            }

            // Return the loaded layer
            return layer;
        }
    }
}