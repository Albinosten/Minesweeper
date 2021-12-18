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
                Height = 2,
                Width = 3,
                DebugOutput = true,
                UseNewSolver = true,
            };
            var bombFactor = 0.2f;
            gameContext.NumberOfBombs = (int)(gameContext.Width * gameContext.Height * bombFactor);
        
            // gameContext.NumberOfBombs = 2;
            var dependencyInjector = new DependencyInjector.DependencyInjector();
            


            // var permuterer = dependencyInjector.Resolve<Permuterer>();

            var bools = new List<bool>{true, false, true, false};
            // var results = permuterer.GetPer(bools);
            var results = new List<List<bool>>();

            var resultstrings = new List<string>();
            foreach(var result in results)
            {
                var stringi = "";
                foreach(var booli in result)
                {
                    stringi += booli ? "True" : "False";
                }
                // var output = result.Select(x => x.ToString()).ToList().ToString();
                Console.WriteLine(stringi);
                resultstrings.Add(stringi);
            }
            Console.WriteLine("combos : " +resultstrings.Count);
            Console.WriteLine("unique combos : " +resultstrings.Distinct().Count());
            

            var aaa = 0;
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

            SolvePuzzlz(dependencyInjector);
        }

        public static void SolvePuzzlz(DependencyInjector.DependencyInjector dependencyInjector)
        {
            IPuzzle puzzlSolver = dependencyInjector.Resolve<Puzzl1>();
            /*************************/
            // Console.WriteLine("Puzzl1");
            
            var puzzlResult = 0;
            // puzzlResult = puzzlSolver.Solve();
            // Console.WriteLine("Should be 1162 is : " + puzzlResult); //1162

            // puzzlResult = puzzlSolver.SolveNext();
            // Console.WriteLine("Should be 1190 is : " + puzzlResult); //1190

            // /*************************/
            // Console.WriteLine("Puzzl2");
            // puzzlSolver = dependencyInjector.Resolve<Puzzl2>();
            
            // puzzlResult = puzzlSolver.Solve();
            // Console.WriteLine("Should be 2036120 is : " + puzzlResult); //2036120

            // puzzlResult = puzzlSolver.SolveNext();
            // Console.WriteLine("Should be 2015547716 is : " + puzzlResult); //2015547716

            // /*************************/
            // Console.WriteLine("Puzzl3");
            // puzzlSolver = dependencyInjector.Resolve<Puzzl3>();
            
            // puzzlResult = puzzlSolver.Solve();
            // Console.WriteLine("Should be 2250414 is : " + puzzlResult); //2250414

            // puzzlResult = puzzlSolver.SolveNext();
            // Console.WriteLine("Should be 6085575 is : " + puzzlResult);//6085575

            // /*************************/
            // Console.WriteLine("Puzzl4");
            // puzzlSolver = dependencyInjector.Resolve<Puzzl4>();
            
            // puzzlResult = puzzlSolver.Solve();
            // Console.WriteLine("Should be 34506 is : " + puzzlResult); //34506

            // puzzlResult = puzzlSolver.SolveNext();
            // Console.WriteLine("Should be 7686 is : " + puzzlResult);//7686


            /*************************/
            Console.WriteLine("Puzzl5");
            puzzlSolver = dependencyInjector.Resolve<Puzzl5>();
            
            puzzlResult = puzzlSolver.Solve();
            Console.WriteLine("Should be 6666 is : " + puzzlResult); //6666

            puzzlResult = puzzlSolver.SolveNext();
            Console.WriteLine("Should be 19081 is : " + puzzlResult); //19081
            
        }
    }
}
