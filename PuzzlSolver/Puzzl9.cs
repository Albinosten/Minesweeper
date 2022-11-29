using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

namespace AdventOfCode
{
    public class Puzzl9 : IPuzzle
    {
        public static string path => "PuzzlInput/test.txt";
        public int FirstResult => 558;
        public long SecondResult => 0;
        public int Solve()
        {
            var lines = File
                .ReadAllLines(path)
                .ToList()
                .ToList();
            
            var result = 0;
            for (int y = 0; y < lines.Count; y++)
            {
                var row = lines[y];
                for (int x = 0; x < row.Length; x++)
                {
                    var value = this.GetNumber(row[x]);

                    var mValue = int.MaxValue;
                    if(y-1 >= 0)
                    {
                        mValue = this.GetNumber(lines[y-1][x]);
                    }
                    if(y+1 < lines.Count)
                    {
                        mValue = Math.Min(mValue, this.GetNumber(lines[y+1][x]));
                    }
                    if(x-1 >= 0)
                    {
                        mValue = Math.Min(mValue, this.GetNumber(lines[y][x-1]));
                    }
                    if(x+1 < row.Length)
                    {
                        mValue = Math.Min(mValue, this.GetNumber(lines[y][x+1]));
                    }

                    if(mValue > value)
                    {
                        result +=value+1;
                    }
                }
            }

            return result;
        }


        private class Cordinate
        {
            public int Y {get;set;}
            public int X {get;set;}
            public bool Included{get;set;}
            public string Aaa => Y + "," + X;
        }
        public long SolveNext()
        {
            var lines = File
                .ReadAllLines(path)
                .ToList()
                .ToList();
            
            var startCordinates = new List<Cordinate>();
            var result = new List<int>();

            for (int y = 0; y < lines.Count; y++)
            {
                var row = lines[y];
                for (int x = 0; x < row.Length; x++)
                {
                    var value = this.GetNumber(row[x]);
                    var mValue = int.MaxValue;

                    var neighbours = this.GetNeighbours(new Cordinate{Y = y, X = x});
                    foreach(var neighbour in neighbours)
                    {
                        mValue = Math.Min(mValue, this.GetNumber(lines[neighbour.Y][neighbour.X]));
                    }
                    if(mValue > value)
                    {
                        startCordinates.Add(new Cordinate
                        {
                            Y=y,
                            X=x
                        });
                    }
                }
            }
            var results = new List<int>();
            foreach(var cordinate in startCordinates)
            {
                results.Add(this.GetCluster(null, cordinate, lines).Count());
            }
            results = results.OrderBy(x=>x).ToList();

            return results[0] *  results[1] * results[2];
        }

        private List<Cordinate> GetNeighbours(Cordinate cordinate)
        {
            var neighbours = new List<Cordinate>();
            if(cordinate.Y-1 >= 0)
            {
                neighbours.Add(new Cordinate{Y = cordinate.Y-1, X = cordinate.X});
            }
            if(cordinate.Y+1 < 5)
            {
                neighbours.Add(new Cordinate{Y = cordinate.Y+1, X = cordinate.X});

            }
            if(cordinate.X-1 >= 0)
            {
                neighbours.Add(new Cordinate{Y = cordinate.Y, X = cordinate.X-1});

            }
            if(cordinate.X+1 < 10)
            {
                neighbours.Add(new Cordinate{Y = cordinate.Y, X = cordinate.X+1});
            }

            return neighbours;
        }

        private List<string> GetCluster(List<string> visited, Cordinate next, IList<string> input)
        {
            var listOfVisited = visited ?? new List<string>();
            
            var neighbours = this.GetNeighbours(next).Where(x => !listOfVisited.Contains(x.Aaa)).ToList();
            
            foreach(var neighbour in neighbours)
            {
                var number = this.GetNumber(input[neighbour.Y][neighbour.X]);
                if(number == 9)
                {
                    neighbour.Included = false;
                    listOfVisited.Add(neighbour.Aaa);

                    continue;
                }
                neighbour.Included = true;

                if(!listOfVisited.Contains(neighbour.Aaa))
                {                
                    listOfVisited.Add(neighbour.Aaa);

                    var apa = this.GetCluster(listOfVisited, neighbour, input);
                    listOfVisited.AddRange(apa);
                }
            }
            return listOfVisited;
        }

        private int GetNumber(char a)
        {
             return (int)Char.GetNumericValue(a);
        }
    }
}