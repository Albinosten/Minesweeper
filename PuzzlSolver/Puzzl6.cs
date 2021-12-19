using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

namespace AdventOfCode
{
    public class Puzzl6 : IPuzzle
    {
        public static string path => "PuzzlInput/6.txt";
        public int FirstResult => 375482;
        public long SecondResult => 1689540415957;
        public int Solve()
        {
            var fishes = File
                .ReadAllLines(path)
                .ToList()
                .Single()
                .Split(",")
                .Select(x => int.Parse(x))
                .ToList();

            for(int i = 0; i < 80 ; i++)
            {
                var numberOfNewFish = 0;
                for (int f = 0; f < fishes.Count; f++)
                {
                    // var fish = fishes[f];
                    if(fishes[f] == 0)
                    {
                        fishes[f] = 6;
                        numberOfNewFish++;
                        continue;
                    }
                        fishes[f]--;
                }

                for(int n = 0; n<numberOfNewFish; n++)
                {
                    fishes.Add(8);
                }
            }

            return fishes.Count;
        }
        public long SolveNext()
        {
            var fishContainers = File
                .ReadAllLines(path)
                .ToList()
                .Single()
                .Split(",")
                .Select(x => int.Parse(x))
                .GroupBy(x => x)
                .Select(x => new FishContainer(x.Count(), x.First()))
                .ToList();

            for(int i = 0; i < 256 ; i++)
            {
                long numberOfNewFish = 0;
                for (int f = 0; f < fishContainers.Count; f++)
                {
                    if(fishContainers[f].DaysToBirth == 0)
                    {
                        fishContainers[f].DaysToBirth = 6;
                        numberOfNewFish += fishContainers[f].NumberOfFishes;
                        continue;
                    }
                        fishContainers[f].DaysToBirth --;
                }
                fishContainers.Add(new FishContainer(numberOfNewFish, 8));
            }

            return fishContainers.Sum(x => x.NumberOfFishes);
        }
        private class FishContainer
        {
            public FishContainer(long numberOfFishes, int DaysToBirth)
            {
                this.NumberOfFishes = numberOfFishes;
                this.DaysToBirth = DaysToBirth;
            }
            public long NumberOfFishes{get;set;}
            public int DaysToBirth{get;set;}
        }
    }

}