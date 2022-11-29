using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Minesweeper
{
    public interface ITile
    {
        bool IsFlaggedAsBomb { get; }
        bool IsExploded { get; }
        bool IsToggled { get; }
        bool IsKnown {get;}
        decimal GetNumberOfBombNeighbours();
        decimal? GetProbabilityToBeABomb();
        void SetProbabilityToBeABomb(decimal percentile);
        void SetProbabilityOfRandom(int percentile);
        void SetNumberOfBombNeighbours(int number);
        void InitializeBomb();
        void Select();
        void FlagAsBomb(bool setColor = true);
        void ToggleRightClick();
        void MarkAsTotalySafe();
        void ResetProbability();
        IList<int> NeighbourIndexes{get;}
        int MyIndex {get;set;}

        ITile Clone();
    }

    public class Tile : ITile
    {
        // public static int s_width => 20;
        // public static int s_Height => 20;
        public static int s_width => 35;
        public static int s_Height => 35;
        public float XPos => this.Rectangle.XPos;
        public float YPos => this.Rectangle.YPos;
        private IPositionalTexture2D Rectangle;
        private GraphicsDeviceManager graphics;
        private GameContext gameContext;
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
        private bool iAmClonedTile {get;set;}
        public bool IsBomb {get; private set;}
        public bool IsKnown => this.IsExploded || this.IsToggled || this.IsFlaggedAsBomb;
        public IList<int> NeighbourIndexes{get;set;}
        public int MyIndex {get;set;}
        public int NumberOfBombNeighbours {get;set;}
        private decimal? ProbabilityToBeABomb{get;set;}
        public void SetProbabilityToBeABomb(decimal percentile)
        {
            if(this.ProbabilityToBeABomb < percentile || !this.ProbabilityToBeABomb.HasValue && percentile > decimal.Zero)
            {
                this.ProbabilityToBeABomb = percentile;
            }
            else if(percentile < decimal.Zero)
            {
                this.ProbabilityToBeABomb = percentile;
            }

            if(percentile > 99)
            {
                this.FlagAsBomb();
            }
            if(percentile < 1)
            {
                this.MarkAsTotalySafe();
            }

            this.IAmFromRandom = false;
        }
        public void SetProbabilityOfRandom(int percentile)
        {
            this.SetProbabilityToBeABomb(percentile);

            this.IAmFromRandom = true;
        }
        public bool IAmFromRandom {get;private set;}
        private bool totalySafe;
        public void MarkAsTotalySafe()
        {
            this.totalySafe = true;
        }
        public decimal? GetProbabilityToBeABomb()
        {
            if(this.totalySafe)
            {
                return decimal.Zero;
            }
            return this.ProbabilityToBeABomb;
        }
        public decimal GetNumberOfBombNeighbours()
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
        }

        protected Tile(Tile tile)
        {
            this.IsFlaggedAsBomb = tile.IsFlaggedAsBomb;
            this.IsToggled = tile.IsToggled;
            this.IsExploded = tile.IsExploded;
            this.IsBomb = tile.IsBomb;
            this.MyIndex = tile.MyIndex;
            this.NeighbourIndexes = tile.NeighbourIndexes;
            this.gameContext = tile.gameContext;

            //texture
            this.FlaggedColors = tile.FlaggedColors;
            this.NormalColors = tile.NormalColors;

            this.Rectangle = tile.Rectangle;
            
            this.NumberOfBombNeighbours = tile.NumberOfBombNeighbours;
            // this.graphics = tile.graphics;//not needed
        }
        public void Initialize(int startY, int startX, int yOffset, GameContext gameContext)
        {
            var rectange = new Texture2D(this.graphics.GraphicsDevice, s_width, s_Height);
            rectange.SetData(this.NormalColors);

            this.Rectangle = new PositionalTexture2D(rectange, this.graphics)
            {
                XPos = startX*s_Height, 
                YPos = startY*s_width + yOffset,
            };

            this.gameContext = gameContext;
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont spriteFont)
        {   
            var topVector = new Vector2(this.Rectangle.XPos, this.Rectangle.YPos);
            spriteBatch.Draw(this.Rectangle.GetTexture(), topVector, Color.White);
            

            if(this.IsToggled)
            {
                spriteBatch.DrawString(spriteFont, " " + this.NumberOfBombNeighbours, topVector, Color.Black);
            }
            
            //Probability to be bomb
            //spriteBatch.DrawString(spriteFont, " " + this.ProbabilityToBeABomb, new Vector2(this.Rectangle.XPos, this.Rectangle.YPos+20), Color.Black);

        }

        public void Select()
        {
            if(this.iAmClonedTile)
            {
                throw new Exception("Can not select cloned tiles");
            }
            if(this.IsFlaggedAsBomb)
            {
                return;
            }
            else if(this.IsBomb)
            {
                if(this.gameContext.DebugOutput)
                {
                    Console.WriteLine("You died");
                }
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
        public void ToggleRightClick()
        {
            if(!this.IsToggled && !this.IsExploded)
            {
                this.IsFlaggedAsBomb = !this.IsFlaggedAsBomb;
                this.SetColor( this.IsFlaggedAsBomb ? this.FlaggedColors: this.NormalColors);
            }
        }
        public void FlagAsBomb(bool setColor = true)
        {
            if(this.gameContext.DebugOutput)
            {
                Console.WriteLine("flagged as bomb. index " + this.MyIndex);
            }
            this.IsFlaggedAsBomb = true;
            if(setColor)
            {
                this.SetColor(this.FlaggedColors);
            }
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
        public void ResetProbability()
        {
            this.ProbabilityToBeABomb = null;
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
        public ITile Clone()
        {
            var clone = new Tile(this);
            clone.iAmClonedTile = true;

            return clone;
        }
    }
}
