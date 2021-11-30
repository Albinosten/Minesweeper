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
            var allHiddenTiles = this.TileHandler
            .GetTilesInterface()
            .Where(x => !x.IsToggled && !x.IsFlaggedAsBomb && !x.IsExploded)
            ;

            //reset calculated probability
            this.ResetCalculatedProbability();

            //calculate probability
            this.CalculateProbabilityAndMarkAsBomb();

            var relevantTiles = allHiddenTiles 
                .Where(x => x.GetProbabilityToBeABomb().HasValue)
                ;


            var totalySafeTile = relevantTiles
                .FirstOrDefault(x => x.GetProbabilityToBeABomb() < decimal.One);
            if(totalySafeTile != null)
            {
                Console.WriteLine("totaly safe tile: " + totalySafeTile.GetProbabilityToBeABomb());

                totalySafeTile.Select();
            }
            else
            {
                var bombsLeft = (decimal)this.TileHandler.GetNumberOfBombLeft(gameContext);
                var probabilityOfRandomTile = 100 * (bombsLeft /this.DontDivideByZero( allHiddenTiles.Count()));
                
                var lowestProbabilityTile = relevantTiles
                    .Where(x => x.GetProbabilityToBeABomb().HasValue)
                    .OrderBy(x => x.GetProbabilityToBeABomb())
                    .FirstOrDefault();

                if(probabilityOfRandomTile < (lowestProbabilityTile?.GetProbabilityToBeABomb() ?? 100))
                {
                    Console.WriteLine("Jumping to a random tile with probability: " + probabilityOfRandomTile);

                    var tile = this.GetRandomTile(this.TileHandler
                        .GetTilesInterface()
                        .Where(x => !x.GetProbabilityToBeABomb().HasValue)
                        .Where(x => !x.IsFlaggedAsBomb)
                        .Where(x => !x.IsToggled)
                        .ToList());

                    tile.Select();

                }
                else
                {
                    //lowest probabilityTile
                    Console.WriteLine("lowest probability tile with probability: " + probabilityOfRandomTile);

                    lowestProbabilityTile.Select();
                }
            }
        }
        private decimal DontDivideByZero(decimal value)
        {
            if(value == decimal.Zero)
            {
                return decimal.One;
            }
            return value;
        }        
        private void CalculateProbabilityAndMarkAsBomb()
        {
            var allTiles = this.TileHandler
                .GetTilesInterface()
                .ToList();

            foreach(var tile in allTiles.Where(x => x.IsToggled && !x.IsFlaggedAsBomb))
            {
                var numberOfBombNeighbours = tile.GetNumberOfBombNeighbours();

                var hiddenNeighbours = new List<ITile>();
                foreach(var tileIndex in tile.NeighbourIndexes)
                {
                    var neighbourTile = allTiles[tileIndex];
                    if(!neighbourTile.IsToggled && !neighbourTile.IsFlaggedAsBomb)
                    {
                        hiddenNeighbours.Add(neighbourTile);
                    }
                    if(neighbourTile.IsFlaggedAsBomb || neighbourTile.IsExploded)
                    {
                        numberOfBombNeighbours--;
                    }
                }

                foreach(var neighbour in hiddenNeighbours)
                {
                    var a = 100*(numberOfBombNeighbours / hiddenNeighbours.Count);
                    neighbour.SetProbabilityToBeABomb((int)a);
                    if(a > 99)
                    {
                        neighbour.FlagAsBomb();
                    }
                    if(a < 1)
                    {
                        neighbour.MarkAsTotalySafe();
                    }
                }
            }
        }
        private void ResetCalculatedProbability()
        {
            foreach(var tile in this.TileHandler.GetTilesInterface())
            {
                tile.ResetProbability();
            }
        }
        private ITile GetRandomTile(IList<ITile> tiles)
        {
            var index = this.GetRandomNumber(tiles.Count);
            return tiles[index];
        }
        private int GetRandomNumber(int max)
        {
            return new Random().Next(max);
        }
    }
}