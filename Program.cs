using System;

namespace Minesweeper
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            var gameContext =  new GameContext()
            {
                Height = 20,
                Width = 30,
                DebugOutput = false,
            };
            var bombFactor = 0.25f;

            gameContext.NumberOfBombs = (int)(gameContext.Width * gameContext.Height * bombFactor);
        
            var dependencyInjector = new DependencyInjector();

            for(int i = 0; i < 10; i++)
            {
                using (var game = dependencyInjector.Resolve<MinesweeperGame>())
                {
                    game.GameContext = gameContext;
                    //game.DebugUpdateAllTiles = true;
                    game.SimmulateOnly = true;
                    game.Run();
                }
            }
        }
    }
}
