using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

namespace AdventOfCode
{
    public class Puzzl9 : IPuzzle
    {
        public static string path => "PuzzlInput/9.txt";
        public int FirstResult => 0;
        public long SecondResult => 0;
        public int Solve()
        {
            var lines = File
                .ReadAllLines(path)
                .ToList()
                .ToList();

            var totalOfIdentifyableNumbers = 0;
            var knownValues = new []{2,4,3,7};

            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                var outputString  = line.Split(" | ")[1];
                var outputValues = outputString.Split(" ").ToList();

                for(int j = 0; j < outputValues.Count; j++)
                {
                    if(knownValues.Contains( outputValues[j].Length ))
                    {
                        totalOfIdentifyableNumbers++;
                    }
                }
            }
            return totalOfIdentifyableNumbers ;
        }
        public long SolveNext()
        {
            var lines = File
                .ReadAllLines(path)
                .ToList()
                .ToList();

            long sumOfAllRows = 0;
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                
                var inputValues = line.Split(" | ")[0].Split(" ").ToList();
                var outputValues = line.Split(" | ")[1].Split(" ").ToList();

                //hitta krypterade värdet för alla nummer;
                string c_One = inputValues.Single(x => x.Length == 2);
                inputValues.Remove(c_One);
                string c_Four = inputValues.Single(x => x.Length == 4);
                inputValues.Remove(c_Four);
                string c_Seven = inputValues.Single(x => x.Length == 3);
                inputValues.Remove(c_Seven);
                string c_Eight = inputValues.Single(x => x.Length == 7);
                inputValues.Remove(c_Eight);
                string c_Three = inputValues.Where(x => x.Length == 5).Single(x => ContainsSubstring(x, c_Seven));
                inputValues.Remove(c_Three);
                string c_Nine = inputValues.Where(x => x.Length == 6).Single(x => ContainsSubstring(x, c_Three));
                inputValues.Remove(c_Nine);
                string c_Zero = inputValues.Where(x => x.Length == 6).Single(x => ContainsSubstring(x, c_Seven));
                inputValues.Remove(c_Zero);
                string c_Six = inputValues.Single(x => x.Length == 6);
                inputValues.Remove(c_Six);
                string c_Five = inputValues.Single(x => ContainsSubstring(c_Six, x));
                inputValues.Remove(c_Five);
                string c_Two = inputValues.Single();

                var AllValues = new[]
                {
                    c_Zero,
                    c_One,
                    c_Two,
                    c_Three,
                    c_Four,
                    c_Five,
                    c_Six,
                    c_Seven,
                    c_Eight,
                    c_Nine,
                }.ToList();

                string resultString = string.Empty;
                for(int j = 0; j < outputValues.Count; j++)
                {
                    var outputValue = outputValues[j];
                    resultString += AllValues.IndexOf(AllValues.Single(x => CompareString(x, outputValue)));
                    
                }
                sumOfAllRows += int.Parse(resultString);
            }
            return sumOfAllRows;
        }
        private static bool ContainsSubstring(string first, string second)
        {
            if(second.ToCharArray().All(x => first.Contains(x)))
            {
                return true;
            }
            return false;
        }
        private static bool CompareString(string first, string second)
        {
            if(first.Length == second.Length && second.ToCharArray().All(x => first.Contains(x)))
            {
                return true;
            }
            return false;
        }
    }
}