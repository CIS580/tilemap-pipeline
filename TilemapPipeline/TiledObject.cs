using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TilemapPipeline
{
    public class TiledObject
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public Rectangle Bounds { get; set; }

        public int Gid { get; set; }

        public bool Visible { get; set; }

        public float Rotation { get; set; }

        public float Opacity { get; set; }

        // TODO: Template

        public Dictionary<string, string> Properties { get; set; }
    }
}
