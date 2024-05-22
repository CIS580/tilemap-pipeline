using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace ExampleGame
{
    public class Enemy
    {
        public Vector2 Position { get; set; }

        public int Health { get; set; }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //   spriteBatch.Draw(Texture, Position, Color.White);
        }
    }

   
}
