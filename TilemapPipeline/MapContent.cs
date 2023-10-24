using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace TilemapPipeline
{
    public class MapContent
    {
        public int Width;

        public int Height;

        public int TileWidth;

        public int TileHeight;

        public Color BackgroundColor;

        public List<LayerContent> TileLayers = new();

        public List<TilesetContent> Tilesets = new();


    }
}
