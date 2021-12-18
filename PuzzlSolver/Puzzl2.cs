using System.IO;
using System.Linq;

namespace AdventOfCode
{
    public class Puzzl2 : IPuzzle
    {
        public static string path => "PuzzlInput/2.txt";

        public int Solve()
        {
            var forward = 0;
            var debth = 0;

            foreach(var line in File.ReadAllLines(path))
            {
                var input = line.Split(' ');
                var direction = input[0];
                var value = int.Parse(input[1]);

                if("forward" == direction)
                {
                    forward += value;
                }
                if("down" == direction)
                {
                    debth += value;
                }
                if("up" == direction)
                {
                    debth -= value;
                }
            }

            return forward * debth;
        }
        public int SolveNext()
        {
            var forward = 0;
            var debth = 0;
            var aim = 0;
            foreach(var line in File.ReadAllLines(path))
            {
                var input = line.Split(' ');
                var direction = input[0];
                var value = int.Parse(input[1]);

                if("forward" == direction)
                {
                    forward += value;
                    debth += value * aim;
                }
                if("down" == direction)
                {
                    aim += value;
                }
                if("up" == direction)
                {
                    aim -= value;
                }
            }

            return forward * debth;
        }
    }

}