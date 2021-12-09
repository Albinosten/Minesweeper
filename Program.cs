using System;
using System.Collections.Generic;
using System.Linq;

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
            var dependencyInjector = new DependencyInjector();

            var permuterer = dependencyInjector.Resolve<Permuterer>();

            var bools = new List<bool>{true, false, true, false};
            var results = permuterer.GetPer(bools);

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
        }
    }
}
