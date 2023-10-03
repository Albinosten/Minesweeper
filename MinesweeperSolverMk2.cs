using System.Linq;
using System;
using System.Collections.Generic;
using Minesweeper;

namespace MinesweeperSolver
{
    public class MinesweeperSolverMk2 : IMinesweeperSolver
    {
        private ITileHandler TileHandler;
        private MinesweeperSolverUsingClusterMk2 minesweeperSolverUsingClusterMk2;
        public MinesweeperSolverMk2(TileHandler tileHandler
        , MinesweeperSolverUsingClusterMk2 minesweeperSolverUsingClusterMk2
        )
        {
            this.TileHandler = tileHandler;
            this.minesweeperSolverUsingClusterMk2 = minesweeperSolverUsingClusterMk2;
        }
        public void SolveNext(GameContext gameContext)
        {
            var allHiddenTiles = this.TileHandler
            .GetTilesInterface()
            .Where(x => !x.IsToggled && !x.IsFlaggedAsBomb && !x.IsExploded)
            .ToList()
            ;

            if(!allHiddenTiles.Any())
            {
                return;
            }

            //reset calculated probability
            this.ResetCalculatedProbability();
            
            var changedAnyTiles = this.MarkTotalyBombAndTotalySafeTiles();

            if(!changedAnyTiles)
            {
                if (this.CalculateClusterbasedOnNumberOfBombsLeft(gameContext))
                {
                    //fucking gissa bara kanske?
                    // this.UnmarkAllFlagged();
                }
            }
        }

        private void UnmarkAllFlagged()
        {
            var flaggedBombs = this.TileHandler.GetTilesInterface().Where(x => x.IsFlaggedAsBomb);
            foreach(var tile in flaggedBombs)
            {
                tile.ToggleRightClick();
            }
        }

        private bool CalculateClusterbasedOnNumberOfBombsLeft(GameContext gameContext)
        {
            bool anyClusterWasSolved = false;
            var numberOfBombsLeft = this.TileHandler.GetNumberOfBombLeft(gameContext);

            var clusters = new List<HashSet<ITile>>();
            var allHiddenTiles = this.TileHandler
                .GetTilesInterface()
                .Where(x => !x.IsToggled && !x.IsFlaggedAsBomb && !x.IsExploded)
                .Where(x => this.TileHandler.GetAllNeighbours(x).Any(t => t.IsKnown))
                .ToList();

            foreach(var tile in allHiddenTiles)
            {
                var cluster = clusters.FirstOrDefault(x => x.Contains(tile));
                if(cluster != null)
                {
                    this.InsertNewValues(tile, cluster);
                }
                else
                {
                    clusters.Add(this.InsertNewValues(tile, new HashSet<ITile>()));
                }
            }

            var maxBombsPerCluster = (numberOfBombsLeft - clusters.Count()) + 1;
            clusters = this.SplitClusters(clusters);

            foreach(var cluster in clusters)
            {
                if(this.minesweeperSolverUsingClusterMk2.SolveCluster(cluster, maxBombsPerCluster))
                {
                    anyClusterWasSolved = true;
                }
            }
            return anyClusterWasSolved;
        }
        private List<HashSet<ITile>> SplitClusters(List<HashSet<ITile>> clusters)
        {
            List<HashSet<ITile>> newClusters = new  List<HashSet<ITile>>();
            foreach(var cluster in clusters)
            {
                if(cluster.Count > Permuterer.MaxSize)
                {
                    decimal value = (decimal)cluster.Count / (decimal)Permuterer.MaxSize;
                    var splittedClusters = cluster.Divide((int)Math.Ceiling(value));
                    foreach(var splittedCluster in splittedClusters)
                    {
                        newClusters.Add(splittedCluster);
                    }
                }
                else
                {
                    newClusters.Add(cluster);
                }
            }
            return newClusters;
        }
        private HashSet<ITile> InsertNewValues(ITile tile, HashSet<ITile> cluster)
        {
            cluster.Add(tile);
            foreach(var neighbour in this.TileHandler.GetAllNeighbours(tile).Where(x => !x.IsKnown && this.TileHandler.GetAllNeighbours(x).Any(n => n.IsKnown)))
            {
                if(!cluster.Contains(neighbour))
                {
                    InsertNewValues(neighbour, cluster);
                }
            }
            return cluster;
        }
        private bool MarkTotalyBombAndTotalySafeTiles()
        {
            bool changedAnyTiles = false;
            var allTiles = this.TileHandler.GetTilesInterface();
            foreach(var tile in allTiles.Where(x => x.IsToggled && !x.IsFlaggedAsBomb))
            {
                var originNumber =  tile.GetNumberOfBombNeighbours();
                var numberOfBombNeighbours = tile.GetNumberOfBombNeighbours();
                var hiddenNeighbours = new List<ITile>();

                foreach(var neighbour in this.TileHandler.GetAllNeighbours(tile))
                {
                    if(neighbour.IsFlaggedAsBomb || neighbour.IsExploded)
                    {
                        numberOfBombNeighbours--;
                    }

                    if(!neighbour.IsKnown)
                    {
                        hiddenNeighbours.Add(neighbour);
                    }
                }

                var probability = this.SafeDivide(numberOfBombNeighbours, hiddenNeighbours.Count);

                if(probability.HasValue)
                {
                    foreach(var neighbour in hiddenNeighbours)
                    {
                        if(probability == decimal.One)
                        {
                            neighbour.FlagAsBomb();
                            changedAnyTiles = true;
                        }
                        if(probability == decimal.Zero)
                        {
                            neighbour.Select();
                            changedAnyTiles = true;

                            //kan hanmna här om min kluster lösning har hittat en 
                            //potentiell lösning men som visar sig vara fel
                        }
                    }
                }
            }
            return changedAnyTiles;
        }

        private decimal? SafeDivide(decimal firstValue, decimal secondValue)
        {
            if(secondValue == decimal.Zero)
            {
                return null;
            }
            return firstValue/secondValue;
        }        
        
        private void ResetCalculatedProbability()
        {
            foreach(var tile in this.TileHandler.GetTilesInterface())
            {
                tile.ResetProbability();
            }
        }
    }
}