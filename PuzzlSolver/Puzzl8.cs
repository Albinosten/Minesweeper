using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

namespace AdventOfCode
{
    public class Puzzl8 : IPuzzle
    {
        public static string path => "PuzzlInput/8.txt";
        public int FirstResult => 0;
        public long SecondResult => 0;
        public int Solve()
        {
            var positions = File
                .ReadAllLines(path)
                .ToList()[0]
                .Split(",")
                .ToList();
            return 0;
        }
        public long SolveNext()
        {
            return 0;
        }
    }

}