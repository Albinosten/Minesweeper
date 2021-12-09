using System;
using System.Collections.Generic;

namespace Minesweeper
{
    public class Permuterer
    {
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


        public IList<List<bool>>  GetUniquePer(int countSet, int countSubSet)
        {
            //Fixa mig unique
            return this.GetPer(countSet, countSubSet);
        }

        public IList<List<bool>>  GetPer(int countSet, int countSubSet)
        {
            var boolList = new List<bool>(countSet);
            for(int i = 0; i< countSet; i++)
            {
                boolList[i] = i < countSubSet;
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

                // foreach(var bbb in list)
                // {
                //     Console.Write(bbb.ToString() + " ");

                // }
                // Console.WriteLine();
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