using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode
{
    public class Puzzl1 : IPuzzle
    {
        public static string path => "PuzzlInput/1.txt";

        public int Solve()
        {
            var result = 0;

            if(File.Exists(path))
            {
                int lastResult = int.MaxValue;
                foreach(var line in File.ReadAllLines(path))
                {
                    var number = int.Parse(line);
                    if(lastResult < number)
                    {
                        result++;
                    }
                    lastResult = number;
                }
            }

            return result;
        }
        public int SolveNext()
        {
            var result = 0;

            if(File.Exists(path))
            {
                int lastResult = int.MaxValue;

                var lines = File.ReadAllLines(path).Select(x =>int.Parse(x)).ToList();
                for(var i = 0; i<lines.Count - 2 ;i++)
                {
                    var sum = lines[i] + lines[i+1] + lines[i+2];
                    if(lastResult < sum)
                    {
                        result++;
                    }
                    lastResult = sum;
                }
            }

            return result;
        }
    }

}