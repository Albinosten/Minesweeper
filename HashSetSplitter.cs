using System.Linq;
using System;
using System.Collections.Generic;
using Minesweeper;
namespace MinesweeperSolver
{
    public static  class HashSetSplitter
    {
        public static IList<HashSet<T>> 
    Divide<T>(this HashSet<T> hashset, int divisions)
{
    if (hashset == null)
        throw new ArgumentNullException("hashset");

    if (divisions <= 0)
        throw new ArgumentOutOfRangeException("divisions");

    HashSet<T>[] sets = new HashSet<T>[divisions];

    for (int i = 0; i < sets.Length; i++)
        sets[i] = new HashSet<T>();

    int capacity = hashset.Count / divisions;
    int remainder = hashset.Count % divisions;
    int itemCount = 0;
    int setIndex = 0;

    foreach (T item in hashset)
    {
        sets[setIndex].Add(item);
        itemCount++;

        if (itemCount >= capacity)
        {
            if (setIndex < remainder && itemCount == capacity)
                continue;

            setIndex++;
            itemCount = 0;
        }
    }
    return Array.AsReadOnly(sets);
}
    }
}