using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace Minesweeper
{
    public interface ITileHandler
    {
        int GetNumberOfBombLeft(GameContext context);
        IEnumerable<ITile> GetTilesInterface();
        void SelectTile(ITile tile);
    }
    public class TileHandler : ITileHandler
    {
        public TileCreator TileCreator{get;private set;}

        private IList<Tile> tiles;
        public TileHandler(TileCreator tileCreator)
        {
            this.TileCreator = tileCreator;
            this.tiles = new List<Tile>();
            this.checkedIndexes = new List<int>();
        }
        public void CreateTiles(GameContext context, GraphicsDeviceManager graphics, int menuHeight)
        {
            this.tiles = this.TileCreator.CreateTiles(context, graphics, menuHeight);
        }
        public void SelectTile(int xPos, int yPos,GameContext context)
        {
            var tile = this.GetSelectedTile(xPos,yPos,context);
            this.SelectTile(tile);   
            this.checkedIndexes.Clear();
        }
        public int GetNumberOfBombLeft(GameContext context)
        {
            //kan köras utan gamecontext om man kollar på tile.isbomb but whaatever.
            return Math.Max(context.NumberOfBombs - this.tiles.Count(x => x.IsFlaggedAsBomb || x.IsExploded), 0);
        }
        public int NumberOfDeaths()
        {
            return this.tiles.Count(x => x.IsExploded);
        }
        public void SelectTile(ITile tile)
        {
            this.SelectTile((Tile)tile);
        }
        public void SelectTile(Tile tile)
        {
            tile.Select();

            if(tile.NumberOfBombNeighbours == 0 && !tile.IsBomb)
            {
                foreach(var neighbourIndex in tile.NeighbourIndexes)
                {
                    var neighbour = this.tiles[neighbourIndex];
                    if(!neighbour.IsFlaggedAsBomb
                        && !this.checkedIndexes.Contains(neighbourIndex))
                    {
                        neighbour.Select();
                        this.checkedIndexes.Add(neighbourIndex);

                        if(neighbour.NumberOfBombNeighbours==0)
                        {
                            this.SelectTile(neighbour);
                        }
                    }
                }
            }
        }

        private List<int> checkedIndexes;
        public void FlagAsBomb(int xPos, int yPos,GameContext context)
        {
            var tile = this.GetSelectedTile(xPos,yPos,context);
            tile.ToggleRightClick();
        }
        public void ToggleAll()
        {
            for(int i = 0; i<this.tiles.Count; i++)
            {
                var tile = this.tiles[i];
                tile.Select();
            }
        }
        public void UseHint()
        {
            var tile = this.tiles.FirstOrDefault(x => !x.IsBomb && !x.IsToggled);
            tile?.Select();
        }
        public IList<Tile> GetTiles()
        {
            return this.tiles;
        }

        public IEnumerable<ITile> GetTilesInterface()
        {
            return this.tiles;
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            for(int i = 0; i<this.tiles.Count; i++)
            {
                var tile = this.tiles[i];
                tile.Draw(gameTime, spriteBatch, spriteFont);
            }
        }
        private Tile GetSelectedTile(int xPos, int yPos,GameContext context)
        {
            var x = xPos/Tile.s_width;
            var y = (yPos-MenuBar.s_Height)/Tile.s_Height;
            
            var index = y * context.Width + x;

            Console.WriteLine("X: "+ x + "          Y: " + y + "         index: " + index);

            if(index>= 0 && index < context.TotalTiles)
            {
                return this.tiles[index];
            }
            return null;
        }
    }
}