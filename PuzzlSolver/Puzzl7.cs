using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

namespace AdventOfCode
{
    public class Puzzl7 : IPuzzle
    {
        public static string path => "PuzzlInput/7.txt";
        public int FirstResult => 352707;
        public long SecondResult => 95519693;
        public int Solve()
        {
            var positions = File
                .ReadAllLines(path)
                .ToList()[0]
                .Split(",")
                .Select(x => int.Parse(x))
                .ToList();

            var smallestAmountOfFuleConsumption = int.MaxValue;
            for (int i = 0; i < positions.Count + 1; i++)
            {
                var fuleConsumption = 0;
                for (int j = 0; j < positions.Count; j++)
                {
                    fuleConsumption += Math.Abs(i - positions[j]);
                }
                if(smallestAmountOfFuleConsumption > fuleConsumption)
                {
                    smallestAmountOfFuleConsumption = fuleConsumption;
                }
            }

            return smallestAmountOfFuleConsumption;
        }
        public long SolveNext()
        {
            var positions = File
                .ReadAllLines(path)
                .ToList()[0]
                .Split(",")
                .Select(x => int.Parse(x))
                .ToList();

            var smallestAmountOfFuleConsumption = int.MaxValue;
            for (int i = 0; i < positions.Count + 1; i++)
            {
                var fuleConsumption = 0;
                for (int j = 0; j < positions.Count; j++)
                {
                    var absValueMoved = Math.Abs(i - positions[j]);
                    fuleConsumption += (absValueMoved*(absValueMoved+1))/2;
                }
                if(smallestAmountOfFuleConsumption > fuleConsumption)
                {
                    smallestAmountOfFuleConsumption = fuleConsumption;
                }
            }

            return smallestAmountOfFuleConsumption;
        }
    }

}