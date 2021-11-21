using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Minesweeper
{
    public class GameContext
    {
        public int Width{get;set;}
        public int Height{get;set;}
        public int NumberOfBombs {get;set;}
        public int TotalTiles => this.Width * this.Height;
    }
}