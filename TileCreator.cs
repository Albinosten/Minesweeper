using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;
using System.Linq;

namespace  Minesweeper
{
    public enum ColorOption
    {
        Normal,Toggled,FlaggedAsBomb,Bomb,Green,Yellow,Red,Darkred,

    }
    public class TileCreator
    {
        private TileNeighbourCounter tileNeighbourCounter;
        private GameLoader gameLoader;
        public TileCreator(TileNeighbourCounter tileNeighbourCounter, GameLoader gameLoader)
        {
            this.tileNeighbourCounter = tileNeighbourCounter;
            this.gameLoader = gameLoader;
        }
        public IList<Tile> CreateTiles(GameContext context, GraphicsDeviceManager graphics, int menuHeight)
        {
            var normalColors = this.GetColorsWithBorders(Tile.s_width, Tile.s_Height, ColorOption.Normal);
            var toggledColors = this.GetColorsWithBorders(Tile.s_width, Tile.s_Height, ColorOption.Toggled);
            var FlaggedAsBombColor = this.GetColorsWithBorders(Tile.s_width, Tile.s_Height, ColorOption.FlaggedAsBomb);
            var bombColor = this.GetColorsWithBorders(Tile.s_width, Tile.s_Height, ColorOption.Bomb);
            var greenColor = this.GetColorsWithBorders(Tile.s_width, Tile.s_Height, ColorOption.Green);
            var yellowColor = this.GetColorsWithBorders(Tile.s_width, Tile.s_Height, ColorOption.Yellow);
            var redColor = this.GetColorsWithBorders(Tile.s_width, Tile.s_Height, ColorOption.Red);
            var darkRedColor =  this.GetColorsWithBorders(Tile.s_width, Tile.s_Height, ColorOption.Darkred);

            var result = new List<Tile>();
            for (int i = 0; i < context.Height; i++)
            {
                for (int j = 0; j < context.Width; j++)
                {
                    var tile = new Tile(graphics)
                    {
                        NormalColors = normalColors,
                        ToggledColors = toggledColors,
                        FlaggedColors = FlaggedAsBombColor,
                        BombColors = bombColor,
                        GreenColors = greenColor,
                        YellowColors = yellowColor,
                        RedColors = redColor,
                        DarkRedColors = darkRedColor,
                        
                    };
                    tile.Initialize(i,j, menuHeight, context);
                    tile.NeighbourIndexes = this.tileNeighbourCounter.GetNeighbourIndexes(j,i,context);

                    result.Add(tile);
                    tile.MyIndex = result.Count-1;
                }
            }

            var indexOfBombs = this.GetIndexOfBombs(context);
            foreach(var i in indexOfBombs.OrderBy(x=>x))
            {
                if(context.DebugOutput)
                {
                    Console.WriteLine(" bomb is: " + i);
                }
                result[i].InitializeBomb();   
            }

            foreach(var tile in result)
            {
                this.tileNeighbourCounter.UpdateTileWithNeighbourData(tile, result);
            }

            return result;
        }


        public IEnumerable<ITile> CloneTiles(IList<ITile> tiles)
        {
            var result = new List<ITile>(tiles.Count);
            foreach(var tile in tiles)
            {
                result.Add(tile.Clone());
            }

            return result;
        }

        private Color[] GetColorsWithBorders(int width, int height, ColorOption colorOption)
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
                    switch (colorOption)
                    {
                        case ColorOption.Normal:
                            data[i] = Color.Chocolate;
                            break;
                        case ColorOption.Toggled:
                            data[i] = Color.Red;
                            break;
                        case ColorOption.FlaggedAsBomb:
                            data[i] = Color.Transparent;
                            break;
                        case ColorOption.Bomb:
                            data[i] = Color.Black;
                            break;
                        case ColorOption.Yellow:
                            data[i] = Color.Yellow;
                            break;
                        case ColorOption.Green:
                            data[i] = Color.Green;
                            break;
                        case ColorOption.Red:
                            data[i] = Color.Red;
                            break;
                        case ColorOption.Darkred:
                            data[i] = Color.DarkRed;
                            break;
                            //Darkred
                    }
                }
            }
            return data;
        }

        private IList<int> GetIndexOfBombs(GameContext gameContext)
        {
            IList<int> result = new List<int>(gameContext.NumberOfBombs);

            if(gameContext.LoadBombs)
            {
                result = this.gameLoader.LoadResult();
            }
            else
            {
                var exceptionList = new List<int>();
                for(int i = 0; i < gameContext.NumberOfBombs; i++)
                {
                    result.Add(this.GetRandomNumber(gameContext.Height*gameContext.Width, exceptionList));
                }
            }
            return result;
        }

        private int GetRandomNumber(int max, IList<int> except)
        {
            var number = new Random().Next(0,max);

            if(except.Contains(number))
            {
                return GetRandomNumber(max, except);
            }
            except.Add(number);
            
            return number;
        }
    }
}