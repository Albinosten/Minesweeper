using System.IO;
using System.Linq;
using System.Collections.Generic;
using System;

namespace AdventOfCode
{
    public class Puzzl3 : IPuzzle
    {
        public static string path => "PuzzlInput/3.txt";

        public int FirstResult => 2250414;
        public long SecondResult => 6085575;
        public int Solve()
        {
            var lines = File
                .ReadAllLines(path)
                .ToList();

            var width = lines[0].Length;

            var sumOfBits = new List<int>(new int[width]);
            for(var i = 0; i<lines.Count; i++)
            {
                for(var j = 0; j< width; j++)
                {
                    var a = lines[i][j].ToString(); 
                    sumOfBits[j] += int.Parse(a);
                }
            }
            
            int gammaRate = 0;
            // long abbeboy = 0;
            // var justForFun = string.Empty;
            // var justForFunInverted = string.Empty;
            for (int i = 0; i < width; i++) 
            {
                // justForFun +=  sumOfBits[i] > lines.Count / 2 ? 1 : 0;
                // justForFunInverted +=  sumOfBits[i] < lines.Count / 2 ? 1 : 0;

                if(sumOfBits[i] > lines.Count / 2)
                {
                    gammaRate += (1 << width-1-i);
                }
                // if(sumOfBits[i] < lines.Count / 2)
                // {
                //     abbeboy += (1 << width-1-i);
                // }
            }

            int epsilon =  ((1 << width) - 1) - gammaRate;

            return epsilon * gammaRate;
        }

        public long SolveNext()
        {
            IList<string> initialLines = File
                .ReadAllLines(path)
                .ToList();

            var width = initialLines[0].Length;


            var oxygen = string.Empty;
            var co2 = string.Empty;

            IList<string> oxygenLines = initialLines;
            IList<string> co2Lines = initialLines;
            for(var i = 0; i < width; i++)
            {
                oxygenLines = this.Oxygen(oxygenLines, i);
                if (oxygenLines.Count == 1)
                {
                    oxygen = oxygenLines.Single();   
                }
                co2Lines = this.CO2(co2Lines, i);
                if (co2Lines.Count == 1)
                {
                    co2 = co2Lines.Single();
                }
            }
            
            return this.BinaryToInt(oxygen) * this.BinaryToInt(co2);
        }

        private int BinaryToInt(string line)
        {
            var result = Convert.ToInt32(line, 2);
            return result;
        }
        private IList<string> Oxygen(IList<string> lines, int i)
        {
            int sumOfBit = 0;
            for(var j = 0; j < lines.Count; j++)
            {
                var a = lines[j][i].ToString(); 
                sumOfBit += int.Parse(a);
            }
            
            var selector = ((sumOfBit >= (decimal)lines.Count / 2) ? 1 : 0);
            var newLines =  lines
                .Where(x => this.ParseString(x, i) == selector)
                .ToList();

            return newLines;
        }
         private IList<string> CO2(IList<string> lines, int i)
        {
            int sumOfBit = 0;
            for(var j = 0; j < lines.Count; j++)
            {
                var a = lines[j][i].ToString(); 
                sumOfBit += int.Parse(a);
            }
            
            var selector = ((sumOfBit < (decimal)lines.Count / 2) ? 1 : 0);
            var newLines =  lines
                .Where(x => this.ParseString(x, i) == selector)
                .ToList();

            return newLines;
        }

        private int ParseString(string x, int i)
        {
            var result = int.Parse(x[i].ToString());
            return result;
        }
    }

}