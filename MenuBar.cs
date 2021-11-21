using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Minesweeper
{
    public class MenuBar
    {
        public static int s_Height => 30;
        private PositionalTexture2D texture;
        public MenuBar(PositionalTexture2D texture, GraphicsDeviceManager graphics)
        {
            this.texture = texture;
        }

        private float TotalGameTime;
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont spriteFont, int numberOfBombsLeft)
        {
            this.TotalGameTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            var bar = new Vector2(this.texture.XPos, this.texture.YPos);//0,0
            spriteBatch.Draw(this.texture.GetTexture(), bar, Color.White);
            

            spriteBatch.DrawString(spriteFont, " " + (int)this.TotalGameTime + " ", bar, Color.Black);

            var secondColumn = new Vector2(this.texture.Width/3, this.texture.YPos);
            spriteBatch.DrawString(spriteFont, " " + numberOfBombsLeft + " ", secondColumn, Color.Black);    

            var thirdColumn = new Vector2(this.texture.Width/3*2, this.texture.YPos);
            spriteBatch.DrawString(spriteFont, " " + "apa" + " ", thirdColumn, Color.Black);    
        }
    }
}