namespace Minesweeper
{
    public class GameContext
    {
        public int Width{get;set;}
        public int Height{get;set;}
        public int NumberOfBombs {get;set;}
        public int TotalTiles => this.Width * this.Height;
        public bool DebugOutput {get;set;}
        public bool LoadBombs {get;set;}

        public bool UseNewSolver {get;set;}

    }
}