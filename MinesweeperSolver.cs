using System.Linq;
using System;
using System.Collections.Generic;
using Minesweeper;

namespace MinesweeperSolver
{
    // public class MinesweeperSolverFactory
    // {
    //     //Todo when dep.inj can handle interface, this might be removed.

    //     private MinesweeperSolverUsingCluster minesweeperSolverUsingCluster;
    //     public MinesweeperSolverFactory(MinesweeperSolverUsingCluster minesweeperSolverUsingCluster)
    //     {
    //         this.minesweeperSolverUsingCluster = minesweeperSolverUsingCluster;
    //     }
    //     public IMinesweeperSolver Create(TileHandler tileHandler, GameContext gameContext)
    //     {
            
    //         return new MinesweeperSolver(tileHandler, this.minesweeperSolverUsingCluster);
    //     }
        
    // }


    public interface IMinesweeperSolver
    {
        void SolveNext(GameContext gameContext);
    }
    public class MinesweeperSolver : IMinesweeperSolver
    {
        private ITileHandler TileHandler;
        private MinesweeperSolverUsingCluster minesweeperSolverUsingCluster;
        private BombProbabilityCalculator bombProbabilityCalculator;
        public MinesweeperSolver(TileHandler tileHandler
            , MinesweeperSolverUsingCluster minesweeperSolverUsingCluster
            , BombProbabilityCalculator bombProbabilityCalculator
            )
        {
            this.TileHandler = tileHandler;   
            this.minesweeperSolverUsingCluster = minesweeperSolverUsingCluster;
            this.bombProbabilityCalculator = bombProbabilityCalculator;
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
            this.bombProbabilityCalculator.CalculateProbabilityAndMarkAsBomb();

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