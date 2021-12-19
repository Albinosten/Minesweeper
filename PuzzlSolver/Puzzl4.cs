using System.IO;
using System.Linq;
using System.Collections.Generic;
using System;

namespace AdventOfCode
{
    public class Puzzl4 : IPuzzle
    {
        public static string path => "PuzzlInput/4.txt";
        public int FirstResult => 34506;
        public long SecondResult => 7686;
        public int Solve()
        {
            var lines = File
                .ReadAllLines(path)
                .ToList();

            var inputs = lines[0].Split(",").Select(x => int.Parse(x)).ToList();

            var boardGames = new List<BoardGame>();
            var boardGame = new BoardGame();

            for(int i = 1; i<lines.Count;i++)
            {
                if(i%6 == 1)
                {
                    boardGame = new BoardGame();
                    boardGames.Add(boardGame);
                    continue;

                }

                var numbers = lines[i].Split(" ", StringSplitOptions.RemoveEmptyEntries);

                var row = new List<Tile>();
                for(int j=0;j<5;j++)
                {
                    row.Add(new Tile(int.Parse(numbers[j])));
                }
                boardGame.Rows.Add(row);
            }

            var winningNumber = -1;
            BoardGame winningBoard = null;
            for(int i = 0; i<inputs.Count; i++)
            {
                
                boardGames.ForEach(x => x.SetNumber(inputs[i]));

                var board = boardGames.FirstOrDefault(x => x.CheckIfWin());
                if(board != null)
                {
                    board.CheckIfWin();
                    winningNumber = inputs[i];
                    i = inputs.Count;
                    winningBoard = board;
                    continue;
                }
            }

            var sumOfUnselected = winningBoard.GetSumOfUnselected();
            return  sumOfUnselected * winningNumber;
        }

        public class BoardGame 
        {
            public void SetNumber(int number)
            {
                foreach(var row in this.Rows)
                {
                    foreach(var tile in row)
                    {
                        if(tile.Number == number)
                        {
                            tile.Selected = true;
                        }
                    }
                }
            }
            public bool CheckIfWin()
            {

                foreach(var row in this.Rows)
                {
                    if(row.All(x => x.Selected))
                    {
                        return true;
                    }
                }

                for(int i = 0; i < 5; i++)
                {
                    if(this.Rows.All(x => x[i].Selected))
                    {
                        return true;
                    }
                }

                return false;
            }
            public int GetSumOfUnselected()
            {
                return this.Rows.Sum(x => x.Where(x => !x.Selected).Sum(x => x.Number));
            }
            public BoardGame()
            {
                this.Rows = new List<IList<Tile>>();
            }
            public IList<IList<Tile>> Rows;
        }
        public class Tile
        {
            public Tile(int number)
            {
                this.Number = number;
            }
            public int Number{get;private set;}
            public bool Selected{get;set;}
        }

        public long SolveNext()
        {
        var lines = File
                .ReadAllLines(path)
                .ToList();

            var inputs = lines[0].Split(",").Select(x => int.Parse(x)).ToList();

            var boardGames = new List<BoardGame>();
            var boardGame = new BoardGame();

            for(int i = 1; i<lines.Count;i++)
            {
                if(i%6 == 1)
                {
                    boardGame = new BoardGame();
                    boardGames.Add(boardGame);
                    continue;

                }

                var numbers = lines[i].Split(" ", StringSplitOptions.RemoveEmptyEntries);

                var row = new List<Tile>();
                for(int j=0;j<5;j++)
                {
                    row.Add(new Tile(int.Parse(numbers[j])));
                }
                boardGame.Rows.Add(row);
            }

            var winningNumber = -1;
            BoardGame winningBoard = null;
            for(int i = 0; i<inputs.Count; i++)
            {
                boardGames.RemoveAll(x => x.CheckIfWin());

                if(boardGames.Count == 1)
                {
                    winningBoard = boardGames.Single();
                    winningBoard.SetNumber(inputs[i]);
                    winningNumber = inputs[i];
                    // i = inputs.Count;
                    //continue;
                }
                else
                {
                    boardGames.ForEach(x => x.SetNumber(inputs[i]));
                }
            }

            var sumOfUnselected = winningBoard.GetSumOfUnselected();
            return  sumOfUnselected * winningNumber;
        }
    }
}