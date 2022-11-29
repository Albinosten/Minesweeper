using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace MinesweeperSolver
{
    public class Permutation
    {
        public int Number{get;set;}
        public List<bool> Values {get;set;}
    }
    public class Permuterer
    {
        public static int MaxSize => 20;
        static bool s_debugOutput => false; 
        public Permuterer()
        {
            this.results = new List<List<bool>>();
        }
        private static void Swap(int a, int b, List<bool> list)
        {
            if (a == b) return;

            var temp = list[a];
            list[a] = list[b];
            list[b] = temp;
        }
        public IEnumerable<Permutation> GetPerFromNumber(int numberOfTiles)
        {
            var totalAmount = System.Math.Pow(2, numberOfTiles);

            var finalResult = new List<Permutation>();
            for(int i = 0; i < totalAmount; i++)
            {
                BitArray b = new BitArray(new int[] { i });
                bool[] bits = new bool[b.Count];
                b.CopyTo(bits, 0);

                yield return new Permutation
                {
                    Number = i+1, 
                    Values = bits.ToList(),
                };
                // finalResult.Add(new Permutation
                // {
                //     Number = i+1, 
                //     Values = bits.ToList(),
                // });
            }

            // return finalResult;
        }

        public IList<List<bool>>  GetUniquePer(int countSet, int countSubSet)
        {
            //Fixa mig unique
            var results = this.GetPer(countSet, countSubSet);

            
            var distinctResult = new HashSet<int>();

            foreach(var result in results)
            {

                BitArray bitField = new BitArray(result.ToArray()); //BitArray takes a bool[]
                int[] bytes = new int[1];
                bitField.CopyTo(bytes, 0);
                distinctResult.Add( bytes[0]);
            }

            var finalResult = new List<List<bool>>();
            foreach(var result in distinctResult)
            {
                BitArray b = new BitArray(new int[] { result });
                bool[] bits = new bool[b.Count];
                b.CopyTo(bits, 0);

                finalResult.Add(bits.ToList());
            }
            return finalResult;
        }

        public IList<List<bool>>  GetPer(int countSet, int countSubSet)
        {

            if(countSet>MaxSize)
            {
                throw new NotSupportedException();
            }
            var boolList = new List<bool>(countSet);
            for(int i = 0; i< countSet; i++)
            {
                boolList.Add(i < countSubSet);
            }

            return this.GetPer(boolList);
        }
        public IList<List<bool>>  GetPer(List<bool> list)
        {
            int x = list.Count - 1;
            GetPer(list, 0, x);
            return results;
        }

        private int numberOfPermutation = 0;
        private List<List<bool>> results;
        private void GetPer(List<bool> list, int k, int m)
        {
            if (k == m)
            {
                numberOfPermutation++;
                results.Add(new List<bool>(list));

                if(s_debugOutput)
                {
                    foreach(var bbb in list)
                    {
                        Console.Write(bbb);

                    }
                    Console.WriteLine(); 
                }
                // Console.WriteLine(numberOfPermutation+ " : " +  new String(list.ToString()));
            }
            else
                for (int i = k; i <= m; i++)
                {
                    Swap(k, i, list);
                    GetPer(list, k + 1, m);
                    Swap(k, i, list);
                }
        }
    }
}