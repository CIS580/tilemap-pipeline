using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace TilemapPipeline
{
    public class LayerContent
    {
        private const uint FlippedHorizontallyFlag = 0x80000000;
        private const uint FlippedVerticallyFlag = 0x40000000;
        private const uint FlippedDiagonallyFlag = 0x20000000;

        internal const byte HorizontalFlipDrawFlag = 1;
        internal const byte VerticalFlipDrawFlag = 2;
        internal const byte DiagonallyFlipDrawFlag = 4;

        public string Name;

        public int Width;
        public int Height;
        public float Opacity;

        public int[] TileIndices;
        public SpriteEffects[] SpriteEffects;
    }
}
