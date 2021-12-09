using System.Linq;
using System;
using System.Collections.Generic;

namespace Minesweeper
{
    public class MinesweeperSolverFactory
    {
        //Todo when dep.inj can handle interface, this might be removed.

        private MinesweeperSolverUsingCluster minesweeperSolverUsingCluster;
        public MinesweeperSolverFactory(MinesweeperSolverUsingCluster minesweeperSolverUsingCluster)
        {
            this.minesweeperSolverUsingCluster = minesweeperSolverUsingCluster;
        }
        public IMinesweeperSolver Create(ITileHandler tileHandler, GameContext gameContext)
        {
            
            return new MinesweeperSolver(tileHandler, this.minesweeperSolverUsingCluster);
        }
        
    }


    public interface IMinesweeperSolver
    {
        void SolveNext(GameContext gameContext);
    }
    public class MinesweeperSolver : IMinesweeperSolver
    {
        private ITileHandler TileHandler;
        private MinesweeperSolverUsingCluster minesweeperSolverUsingCluster;
        public MinesweeperSolver(ITileHandler tileHandler, MinesweeperSolverUsingCluster minesweeperSolverUsingCluster)
        {
            this.TileHandler = tileHandler;   
            this.minesweeperSolverUsingCluster = minesweeperSolverUsingCluster;
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
            }
            else
            {
                //om man inte hittar något totaly safe så kan det vara dags för att 
                //räkna ut baserat på potentiella utfall
/* bra exempel på utfal som behöver räkna ut baseat på grannar
height = 5
width = 8
27
29
31
33
35
*/
/*
height = 2
width = 3
3
5
*/
                if(gameContext.UseNewSolver)
                {
                    this.minesweeperSolverUsingCluster.SolveNext(gameContext);
                }
                else
                {
                    var bombsLeft = (decimal)this.TileHandler.GetNumberOfBombLeft(gameContext);
                    var probabilityOfRandomTile = 100 * (bombsLeft / this.DontDivideByZero(allHiddenTiles.Count()));
                    
                    foreach(var a in allHiddenTiles)//dålig prestanda att sätta på alla och sedan hämta ut.
                    //kanske kan göras när man räknar ut vanliga.
                    {
                        a.SetProbabilityOfRandom((int)probabilityOfRandomTile);
                    }

                    var lowestProbabilityTile = relevantTiles
                        .Where(x => x.GetProbabilityToBeABomb().HasValue)
                        .OrderBy(x => x.GetProbabilityToBeABomb())
                        .First();
                        // .FirstOrDefault();

                    Console.WriteLine("probability on selected: " + lowestProbabilityTile.GetProbabilityToBeABomb());
                    this.TileHandler.SelectTile(lowestProbabilityTile);
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
                    if(!neighbourTile.IsToggled && !neighbourTile.IsFlaggedAsBomb && !neighbourTile.IsExploded)
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

                    if(a<decimal.Zero)
                    {
                        //False set
                    }

                    neighbour.SetProbabilityToBeABomb((int)a);
                    // if(a > 99)
                    // {
                    //     neighbour.FlagAsBomb();
                    // }
                    // if(a < 1)
                    // {
                    //     neighbour.MarkAsTotalySafe();
                    // }
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