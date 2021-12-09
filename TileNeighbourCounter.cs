using System.Collections.Generic;
using System;
using System.Linq;
namespace Minesweeper
{
    public class TileNeighbourCounter
    {
        public void UpdateTileWithNeighbourData(Tile tile, IList<Tile> tiles)
        {   var count = 0;
            foreach(var index in tile.NeighbourIndexes)
            {
                if(tiles[index].IsBomb)
                {
                    count++;
                }
            }
            tile.SetNumberOfBombNeighbours(count);
        }

        public  IList<int> GetNeighbourIndexes(int xPos, int yPos, GameContext gameContext)
        {
            var result = new List<int>();
            
            var xPoses = this.GetNeighbourIndexes(xPos, gameContext.Width);
            var yPoses = this.GetNeighbourIndexes(yPos, gameContext.Height);

             foreach (var y in yPoses)
            {
                foreach (var x in xPoses)
                {
                    var index = this.GetRealIndex(x,y, gameContext);
                    result.Add(index);
                }
            }

            //remove self from neighbours
            result.Remove(this.GetRealIndex(xPos,yPos, gameContext));

             return result;
        }
       
        private int GetRealIndex(int x, int y, GameContext gameContext)
        {
            return Math.Min(y * gameContext.Width + x, gameContext.TotalTiles-1);
        }
        private IList<int> GetNeighbourIndexes(int pos, int max)
        {
            var result = new List<int>();
            
            if(pos - 1 >= 0)
            {
                result.Add(pos-1);
            }
            
            result.Add(pos);

            if(pos + 1 < max)
            {
                result.Add(pos+1);
            }

            return result;
        }
        
    }
}