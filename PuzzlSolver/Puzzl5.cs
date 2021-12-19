using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

namespace AdventOfCode
{
    public class Puzzl5 : IPuzzle
    {
        public static string path => "PuzzlInput/5.txt";
        public int FirstResult => 6666;
        public long SecondResult => 19081;
        public int Solve()
        {
            var lines = File
                .ReadAllLines(path)
                .ToList();
            var board = new List<IList<int>>();

            
            var inputs = new List<Input>();
            for (int i = 0; i < lines.Count; i++)
            {
                //Read input
                var line = lines[i].Split(",");
                var input = new Input()
                {
                    x1 = int.Parse(line[0]),
                    y1 = int.Parse(line[1].Split(" ")[0]),

                    x2 = int.Parse(line[1].Split(" ")[2]),
                    y2 = int.Parse(line[2]),
                };

                //swap min value to first pos.
                this.MakeSureFirstIsLower(input);
                inputs.Add(input);
            }

            //make boards with 0's
            var xMax = inputs.Max(x => x.x2) +1;
            var yMax = inputs.Max(x => x.y2) +1;
            for (int y = 0; y < yMax; y++)
            {
                var row = new List<int>();
                for (int x = 0; x < xMax; x++)
                {
                    row.Add(0);
                }
                board.Add(row);
            }
            //apply inputs
            for(int i = 0; i<inputs.Count; i++)
            {
                var input = inputs[i];
                if(input.y1 == input.y2)
                {
                    for (int x = input.x1; x < input.x2 +1; x++)
                    {
                        board[input.y1][x]++;
                    }
                }
                else if(input.x1 == input.x2)
                {
                    for (int y = input.y1; y < input.y2 +1; y++)
                    {
                        board[y][input.x1]++;
                    }
                }
            }


            //Get result
            var result = 0;
            for (int y = 0; y < yMax; y++)
            {

                for (int x = 0; x < xMax; x++)
                {
                    if(board[y][x] > 1)
                    {
                        result++;
                    }
                }

            }
            return result;
        }

        private void MakeSureFirstIsLower(Input input)
        {
            if(input.x2 < input.x1)
            {
                var temp = input.x2;
                input.x2 = input.x1;
                input.x1 = temp;
            }
            if(input.y2 < input.y1)
            {
                var temp = input.y2;
                input.y2 = input.y1;
                input.y1 = temp;
            }
        }
        public long SolveNext()
        {
            var lines = File
                .ReadAllLines(path)
                .ToList();

            var board = new List<IList<int>>();
            
            var inputs = new List<Input>();
            for (int i = 0; i < lines.Count; i++)
            {
                //Read input
                var line = lines[i].Split(",");
                var input = new Input()
                {
                    x1 = int.Parse(line[0]),
                    y1 = int.Parse(line[1].Split(" ")[0]),

                    x2 = int.Parse(line[1].Split(" ")[2]),
                    y2 = int.Parse(line[2]),
                };
                inputs.Add(input);
            }

            //make boards with 0's
            var xMax = Math.Max(inputs.Max(x => x.x1),inputs.Max(x => x.x2))+1;
            var yMax = Math.Max(inputs.Max(x => x.y1),inputs.Max(x => x.y2))+1;
            // var xMax = 10;
            // var yMax = 10;
            for (int y = 0; y < yMax; y++)
            {
                var row = new List<int>();
                for (int x = 0; x < xMax; x++)
                {
                    row.Add(0);
                }
                board.Add(row);
            }
            //apply inputs
            for(int i = 0; i<inputs.Count; i++)
            {
                var input = inputs[i];
                if(input.y1 == input.y2)
                {
                    this.MakeSureFirstIsLower(input);
                    for (int x = input.x1; x < input.x2 +1; x++)
                    {
                        board[input.y1][x]++;
                    }
                }
                else if(input.x1 == input.x2)
                {
                    this.MakeSureFirstIsLower(input);
                    for (int y = input.y1; y < input.y2 +1; y++)
                    {
                        board[y][input.x1]++;
                    }
                }
                else
                {
                    //diagonal
                    if(input.x1 > input.x2 && input.y1 < input.y2)//ner vänster
                    {
                        var apa = 0;
                        for (int y = input.y1; y < input.y2 + 1; y++)
                        {
                            board[y][input.x1+apa]++;
                            apa--;
                        }
                    }
                    if(input.x1 < input.x2 && input.y1 > input.y2)//upp höger. fixad
                    {
                        var apa = 0;
                        for (int y = input.y1; y >= input.y2; y--)
                        {
                            board[y][input.x1+apa]++;
                            apa++;
                        }
                    }
                    if(input.x1 > input.x2 && input.y1 > input.y2)//upp vänster. fixad
                    {
                        var apa = 0;
                        var latestY = 1000;
                        for (int y = input.y1; y >= input.y2; y--)
                        {
                            board[y][input.x1+apa]++;
                            apa--;
                            latestY=y;
                        }
                    }
                    if(input.x1 < input.x2 && input.y1 < input.y2)//ner höger
                    {
                        var apa = 0;
                        for (int y = input.y1; y < input.y2 + 1; y++)
                        {
                            board[y][input.x1+apa]++;
                            apa++;
                        }
                    }
                }
            }


            //Get result
            var result = 0;
            for (int y = 0; y < yMax; y++)
            {
                for (int x = 0; x < xMax; x++)
                {
                    var output = board[y][x] == 0 ? "." : board[y][x].ToString();
                    Console.Write(output.ToString());
                    if(board[y][x] > 1)
                    {
                        result++;
                    }
                }
                Console.WriteLine();
            }
            return result;
        }
        private class Input
        {
            public int x1{get;set;}
            public int x2{get;set;}
            public int y1{get;set;}
            public int y2{get;set;}
        }
    }
}