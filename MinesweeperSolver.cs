using System.Linq;
using System;
using System.Collections.Generic;

namespace Minesweeper
{
    public class MinesweeperSolverFactory
    {
        public MinesweeperSolver Create(ITileHandler tileHandler)
        {
            return new MinesweeperSolver(tileHandler);
        }
    }

    public class MinesweeperSolver
    {
        ITileHandler TileHandler;
        public MinesweeperSolver(ITileHandler tileHandler)
        {
            this.TileHandler = tileHandler;   
        }
        public void SolveNext(GameContext gameContext)
        {
            var allTiles = this.TileHandler
            .GetTilesInterface()
            .ToList()
            ;

            foreach(var tile in allTiles.Where(x => x.IsToggled))
            {
                float numberOfBombNeighbours = tile.GetNumberOfBombNeighbours();

                var hiddenNeighbours = new List<ITile>();
                foreach(var tileIndex in tile.NeighbourIndexes)
                {
                    var neighbourTile = allTiles[tileIndex];
                    if(!neighbourTile.IsToggled && !neighbourTile.IsFlaggedAsBomb)
                    {
                        hiddenNeighbours.Add(neighbourTile);
                    }
                }

                foreach(var neighbour in hiddenNeighbours)
                {
                    var a = 100*(numberOfBombNeighbours / hiddenNeighbours.Count);
                    neighbour.SetProbabilityToBeABomb((int)a);
                }
            }

            
            //Flag bomb
            allTiles
                .Where(x => x.GetProbabilityToBeABomb() == 100)
                .Select(x => 
                {
                     x.FlagAsBomb();
                     return true;
                });


            var relevantTiles = allTiles 
                .Where(x => !x.IsToggled && !x.IsFlaggedAsBomb)
                .Where(x => x.GetProbabilityToBeABomb() != null)
                ;

            var nextTile = relevantTiles.FirstOrDefault(x => x.GetNumberOfBombNeighbours() == 0);
            if(nextTile != null)
            {
                nextTile.Select();
            }
            else
            {
                float bombsLeft = (float)this.TileHandler.GetNumberOfBombLeft(gameContext);
                float probabilityOfRandomTile = 100 * (bombsLeft / allTiles
                    .Where(x => x.GetProbabilityToBeABomb() == null)
                    .Count());
                
                var lowestProbability = relevantTiles
                    .Select(x => x.GetNumberOfBombNeighbours())
                    .OrderBy(x => x)
                    .First();
            }
        }
        private int GetRandomNumber(int max)
        {
            return new Random().Next(max);
        }
    }
}