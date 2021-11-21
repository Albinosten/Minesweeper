using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace Minesweeper
{
    public interface ITile
    {
        bool IsFlaggedAsBomb { get; }
        bool IsExploded { get; }
        bool IsToggled { get; }
        int GetNumberOfBombNeighbours();
        int? GetProbabilityToBeABomb();
        void SetProbabilityToBeABomb(int percentile);
        void SetNumberOfBombNeighbours(int number);
        void InitializeBomb();
        void Select();
        void FlagAsBomb();
        IList<int> NeighbourIndexes{get;}
        int MyIndex {get;set;}

    }

    public class Tile : ITile
    {
        public static int s_width => 35;
        public static int s_Height => 35;
        public float XPos => this.Rectangle.XPos;
        public float YPos => this.Rectangle.YPos;
        private IPositionalTexture2D Rectangle;
        private GraphicsDeviceManager graphics;
        public  Color[] NormalColors { get; set; }
        public  Color[] ToggledColors{ get; set; }
        public  Color[] FlaggedColors{ get; set; }
        public  Color[] BombColors{ get; set; }
        public  Color[] GreenColors{ get; set; }
        public  Color[] YellowColors{ get; set; }
        public  Color[] RedColors{ get; set; }
        public  Color[] DarkRedColors{ get; set; }
        public bool IsExploded { get; private set; }
        public bool IsToggled {get; private set;}
        public bool IsBomb {get; private set;}

        public IList<int> NeighbourIndexes{get;set;}
        public int MyIndex {get;set;}
        public int NumberOfBombNeighbours {get;set;}
        private int ProbabilityToBeABomb{get;set;}
        public void SetProbabilityToBeABomb(int percentile)
        {
            if(this.ProbabilityToBeABomb < percentile)
            {
                this.ProbabilityToBeABomb = percentile;
            }
        }
        public int? GetProbabilityToBeABomb()
        {
            if(this.ProbabilityToBeABomb < 0)
            {
                return null;
            }
            return this.ProbabilityToBeABomb;
        }
        public int GetNumberOfBombNeighbours()
        {
            if(this.IsToggled)
            {
                return this.NumberOfBombNeighbours;
            }
            throw new Exception("Can only get numberOfBombNeighbours for toggled tiles");
        }
        public Tile(GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
            this.ProbabilityToBeABomb = -1;
        }
        public void Initialize(int startY, int startX, int yOffset)
        {
            var rectange = new Texture2D(this.graphics.GraphicsDevice, s_width, s_Height);
            rectange.SetData(this.NormalColors);

            this.Rectangle = new PositionalTexture2D(rectange, this.graphics)
            {
                XPos = startX*s_Height, 
                YPos = startY*s_width + yOffset,
            };
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont spriteFont)
        {   
            var topVector = new Vector2(this.Rectangle.XPos, this.Rectangle.YPos);
            spriteBatch.Draw(this.Rectangle.GetTexture(), topVector, Color.White);
            

            if(this.IsToggled)
            {
                spriteBatch.DrawString(spriteFont, " " + this.NumberOfBombNeighbours + " ", topVector, Color.Black);
            }
        }

        public void Select()
        {
            if(this.IsFlaggedAsBomb)
            {
                return;
            }
            else if(this.IsBomb)
            {
                Console.WriteLine("You died");
                this.SetColor(this.BombColors);
                this.IsExploded = true;
            }
            else 
            {
                this.SetColor(this.GetColorAfterSelected());
                this.IsToggled = true;
            }
        }

        public bool IsFlaggedAsBomb{get;private set;}
        public void TobbleRightClick()
        {
            if(!this.IsToggled && !this.IsExploded)
            {
                this.IsFlaggedAsBomb = !this.IsFlaggedAsBomb;
                this.SetColor( this.IsFlaggedAsBomb ? this.FlaggedColors: this.NormalColors);
            }
        }
        public void FlagAsBomb()
        {
            this.IsFlaggedAsBomb = true;
            this.SetColor(this.FlaggedColors);
        }

        public void InitializeBomb()
        {
            this.IsBomb = true;
        }
        private void SetColor(Color[] color)
        {
            this.Rectangle.GetTexture().SetData(color);
        }
        public void SetNumberOfBombNeighbours(int number)
        {
            this.NumberOfBombNeighbours = number;
        }
        public void SetColor(ColorOption color)
        {
            switch (color)
            {
                case ColorOption.Normal:
                    this.SetColor(this.NormalColors);
                    break;
                case ColorOption.Toggled:
                    this.SetColor(this.ToggledColors);
                    break;
                case ColorOption.FlaggedAsBomb:
                    this.SetColor(this.FlaggedColors);
                    break;
                case ColorOption.Bomb:
                    this.SetColor(this.BombColors);
                    break;
                case ColorOption.Green:
                    this.SetColor(this.GreenColors);
                    break;
                case ColorOption.Yellow:
                    this.SetColor(this.YellowColors);
                    break;
                case ColorOption.Red:
                    this.SetColor(this.RedColors);
                    break;
                case ColorOption.Darkred:
                    this.SetColor(this.DarkRedColors);
                    break;
            }
            
        }
        private ColorOption GetColorAfterSelected()
        {
            var option = ColorOption.Green;
            if(this.NumberOfBombNeighbours > 0)
            {
                option = ColorOption.Yellow;
            }
            if(this.NumberOfBombNeighbours > 1)
            {
                option = ColorOption.Red;
            }
            if(this.NumberOfBombNeighbours > 2)
            {
                option = ColorOption.Darkred;
            }

            return option;
        }
    }
}
