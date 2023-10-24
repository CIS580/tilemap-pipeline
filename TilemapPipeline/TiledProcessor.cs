using Microsoft.Xna.Framework.Content.Pipeline;


namespace TilemapPipeline
{
    [ContentProcessor(DisplayName = "Tiled Map - ExampleMap")]
    internal class TiledProcessor : ContentProcessor<MapContent, ExampleMapContent>
    {
        public override ExampleMapContent Process(MapContent input, ContentProcessorContext context)
        {
            ExampleMapContent output = new();

            // Copy map dimensions
            output.Width = input.Width;
            output.Height = input.Height;
            output.TileWidth = input.TileWidth;
            output.TileHeight = input.TileHeight;
            

            
            return output;
        }
    }
}