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
                Width = 10,
                NumberOfBombs = 30,
            };
        
            var dependencyInjector = new DependencyInjector();
            using (var game = dependencyInjector.Resolve<MinesweeperGame>())
            {
                game.GameContext = gameContext;
                //game.DebugUpdateAllTiles = true;
                game.Run();
            }
        }
    }
}
