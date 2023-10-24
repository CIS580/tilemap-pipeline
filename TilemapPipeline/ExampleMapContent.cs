using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TilemapPipeline
{
    [ContentSerializerRuntimeType("ExampleGame.ExampleMap, ExampleGame")]

    public class ExampleMapContent
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public int TileWidth { get; set; }

        public int TileHeight { get; set; }

        public Color BackgroundColor { get; init; }
    }
}
