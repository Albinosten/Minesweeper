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
            .ToList()
            ;

            if(!allHiddenTiles.Any())
            {
                return;
            }

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
                if(gameContext.DebugOutput)
                {
                    Console.WriteLine("totaly safe tile: " + totalySafeTile.GetProbabilityToBeABomb());
                }
                this.TileHandler.SelectTile(totalySafeTile);
                // totalySafeTile.Select();
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
                    if(gameContext.DebugOutput)
                    {
                        Console.WriteLine("Jumping to a random tile with probability: " + probabilityOfRandomTile);
                    }

                    // var tiles = this.TileHandler
                    //     .GetTilesInterface()
                    //     .Where(x => !x.GetProbabilityToBeABomb().HasValue)
                    //     .Where(x => !x.IsFlaggedAsBomb)
                    //     .Where(x => !x.IsToggled)
                    //     .ToList();
                    var tiles = allHiddenTiles;

                    if(tiles.Any())//DEnna blir tom ibland, även fast det blir en probabilityOfRandomTile. 
                    //anledningen är att denna filtrerar bort för många. alternativt att den räkar ut probabilityn för högt
                    {
                        // tile.Select();
                        var tile = this.GetRandomTile(tiles);
                        this.TileHandler.SelectTile(tile);
                    }
                    else
                    {
                        Console.WriteLine("hittade ingen random tile. probability = " + probabilityOfRandomTile);
                        this.SelectLowestProbabilityTile(gameContext, lowestProbabilityTile);
                    }


                }
                else
                {
                    //lowest probabilityTile
                    this.SelectLowestProbabilityTile(gameContext, lowestProbabilityTile);
                }
            }
        }
        private void SelectLowestProbabilityTile(GameContext gameContext, ITile lowestProbabilityTile)
        {
            if(gameContext.DebugOutput)
            {
                Console.WriteLine("lowest probability tile with probability: " + lowestProbabilityTile.GetProbabilityToBeABomb());
            }

            // lowestProbabilityTile.Select();
            this.TileHandler.SelectTile(lowestProbabilityTile);
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