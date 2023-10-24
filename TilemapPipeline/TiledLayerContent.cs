using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TilemapPipeline
{
    public class TiledLayerContent
    {
        public string Name { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public float Opacity { get; set; }

        public int[] Tiles { get; set; }

        public byte[] FlipAndRotate { get; set; }
    }
}
