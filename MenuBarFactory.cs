using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace Minesweeper
{
    public class MenuBarFactory
    {
        public MenuBar Create(GameContext gameContext, GraphicsDeviceManager graphics)
        {            
            var texture = new Texture2D(graphics.GraphicsDevice, gameContext.Width*Tile.s_width, MenuBar.s_Height);
            texture.SetData(this.GetColorsWithBorders(texture.Width, texture.Height));

            var rectangle = new PositionalTexture2D(texture, graphics)
            {
                XPos = 0, 
                YPos = 0,
            };
            return new MenuBar(rectangle, graphics);
        }
    
        private Color[] GetColorsWithBorders(int width, int height)
        {
            Color[] data = new Color[width*height];
            for(int i=0; i < data.Length; ++i) 
            {
                var mod = i%width;
                if(mod < 2 || mod > width - 3 || i < width*2 || i > data.Length - (width*2))
                {
                    data[i] = Color.Black;
                }
                else
                {
                    data[i] = Color.LightCyan;
                }
            }
            return data;
        }
    }
}