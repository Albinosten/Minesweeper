using Minesweeper;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace MinesweeperSolver
{

    public class MinesweeperSolverUsingClusterMk2
    {
        private TileHandler tileHandler;
        public MinesweeperSolverUsingClusterMk2(TileHandler tileHandler)
        {
            this.tileHandler = tileHandler;
        }
        public bool SolveCluster(HashSet<ITile> cluster, int maxNumberOfBombs)
        {
            if(cluster.Count>Permuterer.MaxSize)
            {
                return false;
            }
            maxNumberOfBombs = System.Math.Min(maxNumberOfBombs, cluster.Count);
            var possibleCombinations = new Permuterer().GetPerFromNumber(cluster.Count);
            
           


            // return LoopThrougParallelPermutations(possibleCombinations, maxNumberOfBombs, cluster, this.tileHandler);
            return this.LoopThroughPermutations(possibleCombinations, maxNumberOfBombs, cluster);
        }

        private static bool LoopThrougParallelPermutations(IEnumerable<Permutation> possibleCombinations, int maxNumberOfBombs, HashSet<ITile> cluster, TileHandler tileHandler)
        {
            var workingNumber = 0;
            var partitioner = Partitioner.Create(possibleCombinations, EnumerablePartitionerOptions.None);
            
            var result = Parallel.ForEach (partitioner, (item) => 
            //foreach (var item in possibleCombinations)
            {
                Console.WriteLine("trying with cluster no : " + item.Number);

                var tileHandlerInternal = tileHandler.Clone();
                
                var tiles = tileHandlerInternal
                    .GetTilesInterface()
                    .Where(x => cluster.Select(s => s.MyIndex).Contains(x.MyIndex))
                    .ToList();

                //set possible permutation
                for(int i = 0; i < cluster.Count; i++)
                {
                    var tile  = tiles[i];
                    if(item.Values[i])
                    {
                        tile.ToggleRightClick();
                    }
                }
                if(ValidatePerfectCombination(tiles, tileHandlerInternal))
                {
                    Console.WriteLine("solved by clustering");
                    //workingNumber = item.Number;
                    //break;
                    //return;
                    //continue?
                }
                else
                {
                    //reset flg if not perfect combo.
                    for(int i = 0; i < cluster.Count; i++)  
                    {
                        var tile  = tiles[i];
                        if(item.Values[i])
                        {
                            tile.ToggleRightClick();
                        }
                    }
                }
            //}
            });

            if(workingNumber != decimal.Zero)
            {
                var correctPermutation = possibleCombinations.First(x => x.Number == workingNumber);
                for(int i = 0; i < cluster.Count; i++)
                {
                    var tile  = cluster.ToList()[i];
                    if(correctPermutation.Values[i])
                    {
                        tile.ToggleRightClick();
                    }
                }
            }

            return workingNumber != decimal.Zero;
        }
        private bool LoopThroughPermutations(IEnumerable<Permutation> possibleCombinations, int maxNumberOfBombs, HashSet<ITile> cluster)
        {
             foreach(var combination in possibleCombinations.Where(x => x.Values.Sum(x => x == true ? decimal.One : decimal.Zero) <= maxNumberOfBombs))
            {
                //cloned tiles so do whatever works;
                var tiles = cluster
                    .ToList();

                //set possible permutation
                for(int i = 0; i < cluster.Count; i++)
                {
                    var tile  = tiles[i];
                    if(combination.Values[i])
                    {
                        tile.ToggleRightClick();
                    }
                }
                if(ValidatePerfectCombination(tiles, this.tileHandler))
                {
                    Console.WriteLine("solved by clustering");
                    return true;
                }
                else
                {
                    //reset flg if not perfect combo.
                    for(int i = 0; i < cluster.Count; i++)  
                    {
                        var tile  = tiles[i];
                        if(combination.Values[i])
                        {
                            tile.ToggleRightClick();
                        }
                    }
                }
            }
            Console.WriteLine("Could not solve by cluster");
            return false;
        }
        private static bool ValidatePerfectCombination(IList<ITile> clusterTiles, TileHandler tileHandlerInternal)
        {
            var tilesNeighbours = clusterTiles
                .SelectMany(x => tileHandlerInternal.GetAllNeighbours(x))
                .ToHashSet()
                .Where(x => x.IsToggled)
                .ToList();
            foreach(var tile in tilesNeighbours)
            {
                if(tile.GetNumberOfBombNeighbours() != tileHandlerInternal
                    .GetAllNeighbours(tile)
                    .Where(x => x.IsFlaggedAsBomb)
                    .Count()
                    )
                {
                    return false;
                }
            }

            return true && tilesNeighbours.Any();
        }
    }
}