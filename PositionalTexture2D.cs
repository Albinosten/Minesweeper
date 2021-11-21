using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Minesweeper
{
    public interface IPositionalTexture2D
    {
        float XPos {get;set;}
        float Width {get;}
        float YPos {get;set;}
        float Height{get;}
        Texture2D GetTexture();
    }

    public class PositionalTexture2D : IPositionalTexture2D
    {
        protected Texture2D Texture{get;set;}
        protected GraphicsDeviceManager graphics;
        public PositionalTexture2D(Texture2D texture, GraphicsDeviceManager graphics)
        {
            this.Texture = texture;
            this.graphics = graphics;
            this.YPos = 1;
            this.scale = 1;
        }
        public Texture2D GetTexture()
        {
            return this.Texture;
        }
        public float XPos {get;set;}
        public float Width => this.Texture.Width * this.scale;
        public float YPos {get;set;}
        public float Height => this.Texture.Height * this.scale;
        public float scale {get;set;}
        
        

    }
}