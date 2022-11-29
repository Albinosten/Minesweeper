using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode;

namespace Minesweeper
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            var gameContext =  new GameContext()
            {
                Height = 17,
                Width = 30,
                DebugOutput = true,
                // UseNewSolver = true,
            };
            var bombFactor = 0.15f;
            //var bombFactor = 0.17f;
            gameContext.NumberOfBombs = (int)(gameContext.Width * gameContext.Height * bombFactor);
            // gameContext.NumberOfBombs = 3;
        
            var dependencyInjector = new DependencyInjector.DependencyInjector();
            
            var aaa = 1;
            for(int i = 0; i < aaa; i++)
            {
                using (var game = dependencyInjector.Resolve<MinesweeperGame>())
                {
                    game.GameContext = gameContext;
                    //game.DebugUpdateAllTiles = true;
                    //game.SimmulateOnly = true;
                    
                    if(game.SimmulateOnly)
                    {
                        // aaa = 10;
                    }
                    game.Run();
                }
            }

            SolveAllPuzzle(dependencyInjector);
        }

        public static void SolveAllPuzzle(DependencyInjector.DependencyInjector dependencyInjector)
        {
            IPuzzle puzzlSolver = null;

            var objects = new []
            {
                // typeof(Puzzl1),
                // typeof(Puzzl2),
                // typeof(Puzzl3),
                // typeof(Puzzl4),
                // typeof(Puzzl5),
                // typeof(Puzzl6),
                // typeof(Puzzl7),
                // typeof(Puzzl8),
                // typeof(Puzzl9),
                typeof(Puzzl10),
            };

            foreach(var obj in objects)
            {
                puzzlSolver = dependencyInjector.Resolve<IPuzzle>(obj);
            
                Console.WriteLine("First should be " + puzzlSolver.FirstResult + " is : " + puzzlSolver.Solve());
                Console.WriteLine("Second should be " + puzzlSolver.SecondResult + " is : " + puzzlSolver.SolveNext());
            }
        }
       
    }
}
