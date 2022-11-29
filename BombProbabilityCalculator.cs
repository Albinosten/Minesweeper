using Minesweeper;
using System.Collections.Generic;
using System.Linq;

namespace MinesweeperSolver
{
    public class BombProbabilityCalculator
    {
        ITileHandler TileHandler;
        public BombProbabilityCalculator(TileHandler tileHandler)
        {
            this.TileHandler = tileHandler;
        }


        public void CalculateProbabilityAndMarkAsBomb()
        {
            var allTiles = this.TileHandler
                .GetTilesInterface()
                .ToList();

            this.CalculateProbabilityAndMarkAsBomb(allTiles);
        }

        public void CalculateProbabilityAndMarkAsBomb(IList<ITile> allTiles)
        {

            foreach(var tile in allTiles.Where(x => x.IsToggled && !x.IsFlaggedAsBomb))
            {
                var numberOfBombNeighbours = tile.GetNumberOfBombNeighbours(); 
                var hiddenNeighbours = new List<ITile>();

                foreach(var tileIndex in tile.NeighbourIndexes)
                {
                    var neighbourTile = allTiles[tileIndex];
                    // if(!neighbourTile.IsToggled && !neighbourTile.IsFlaggedAsBomb && !neighbourTile.IsExploded)
                    // {
                        hiddenNeighbours.Add(neighbourTile);
                    // }
                    if(neighbourTile.IsFlaggedAsBomb || neighbourTile.IsExploded)
                    {
                        numberOfBombNeighbours--;
                    }
                }

                var a = 100*(numberOfBombNeighbours / hiddenNeighbours.Count);
                if(a<decimal.Zero)
                {

                }
                foreach(var neighbour in hiddenNeighbours)
                {
                    neighbour.SetProbabilityToBeABomb(a);
                }
            }
        }
        // public void CalculateProbabilityAndMarkAsBomb(IList<ITile> allTiles)
        // {

        //     foreach(var tile in allTiles.Where(x => x.IsToggled && !x.IsFlaggedAsBomb))
        //     {
        //         var numberOfBombNeighbours = tile.GetNumberOfBombNeighbours();

        //         var hiddenNeighbours = new List<ITile>();
        //         foreach(var tileIndex in tile.NeighbourIndexes)
        //         {
        //             var neighbourTile = allTiles[tileIndex];
        //             if(!neighbourTile.IsToggled && !neighbourTile.IsFlaggedAsBomb && !neighbourTile.IsExploded)
        //             {
        //                 hiddenNeighbours.Add(neighbourTile);
        //             }
        //             if(neighbourTile.IsFlaggedAsBomb || neighbourTile.IsExploded)
        //             {
        //                 numberOfBombNeighbours--;
        //             }
        //         }

        //         foreach(var neighbour in hiddenNeighbours)
        //         {
        //             var a = 100*(numberOfBombNeighbours / hiddenNeighbours.Count);

        //             if(a<decimal.Zero)
        //             {
        //                 //False set
        //             }

        //             neighbour.SetProbabilityToBeABomb((int)a);
        //             // if(a > 99)
        //             // {
        //             //     neighbour.FlagAsBomb();
        //             // }
        //             // if(a < 1)
        //             // {
        //             //     neighbour.MarkAsTotalySafe();
        //             // }
        //         }
        //     }
        // }
    }
}