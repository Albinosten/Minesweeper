using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
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
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont spriteFont, int numberOfBombs,int numberOfBombsLeft, int numberOfDeaths)
        {
            this.TotalGameTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            var bar = new Vector2(this.texture.XPos, this.texture.YPos);//0,0
            spriteBatch.Draw(this.texture.GetTexture(), bar, Color.White);
            

            spriteBatch.DrawString(spriteFont, " " + (int)this.TotalGameTime + " ", bar, Color.Black);

            var secondColumn = new Vector2(this.texture.Width/4, this.texture.YPos);
            spriteBatch.DrawString(spriteFont, " " + numberOfBombsLeft + " ", secondColumn, Color.Black);    

            var thirdColumn = new Vector2(this.texture.Width/4*2, this.texture.YPos);
            spriteBatch.DrawString(spriteFont, " " +  + numberOfDeaths + " ", thirdColumn, Color.Black);    
                        
            var forthColumn = new Vector2(this.texture.Width/4*3, this.texture.YPos);
            decimal deathscore = this.GetDeathScore(numberOfDeaths,numberOfBombs);
            //Console.WriteLine("deathscore " +deathscore);
            spriteBatch.DrawString(spriteFont, "death score: " +  deathscore  + "% ", forthColumn, Color.Black);    
        }
        public int GetDeathScore(decimal numberOfDeaths, decimal numberOfBombs)
        {
            return (int)(100*(numberOfDeaths/numberOfBombs));
        }
    }
}