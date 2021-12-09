using System.Linq;
using System;
using System.Collections.Generic;

namespace Minesweeper
{
    public class MinesweeperSolverUsingCluster : IMinesweeperSolver
    {
        ITileHandler TileHandler;
        TileCreator TileCreator;
        Permuterer Permuterer;
        public MinesweeperSolverUsingCluster(TileHandler tileHandler, TileCreator tileCreator, Permuterer permuterer)
        {
            this.TileHandler = tileHandler;   
            this.TileCreator = tileCreator;
            this.Permuterer = permuterer;
        }
        public void SolveNext(GameContext gameContext)
        {
           
        }
        public void GetGrouping()
        {
            var grouping = new List<ITile>();

             var allTiles = this.TileHandler.GetTilesInterface();
            foreach(var tile in allTiles.Where(x => !x.IsKnown))
            {
                var neighboursWithNumber = this.FindAllNeighboursWithNumber(tile).ToList();
                if(!neighboursWithNumber.Any())
                {
                    continue;
                }


                var tilesToUse = this.GetGroupingLeft(tile)
                    .Concat(this.GetGroupingRight(tile))
                    //kanske lägga till upp och ner här
                    .Distinct()
                    .ToList();

                var clusters = new List<TileCluster>();
                // var cluster = new TileCluster(this.TileCreator.CloneTiles(tilesToUse).ToList());
                if(tilesToUse.Any())
                {
                    //kanske lägga till så att varje Yile vet hjur många bomber som är kvar bredvis sig.
                    //lös all skit här xD

                    //testa alla olika permuteringar och se vilken som har samanlagt bästa score.
                    //eftersom en känd tile kan ha bomber som inte är i detta kluster så kanske
                    //det inte går att få ett perfect score. därför välja bästa

                    //antal bomber att försöka placera ut i all permisioner
                    for(int numberOfBombs = 1; numberOfBombs < tilesToUse.Count; numberOfBombs++)
                    {
                        var permutList = this.Permuterer.GetUniquePer(tilesToUse.Count, numberOfBombs);
                        foreach(var permut in permutList)
                        {
                            var cluster = new TileCluster(this.TileCreator.CloneTiles(tilesToUse).ToList());
                            for(var index = 0; index < tilesToUse.Count; index++)
                            {
                                if(permut[index])
                                {
                                    cluster.Tiles[index].FlagAsBomb();
                                }
                            }
                            //räkna ut probabiliteten för detta cluster
                            clusters.Add(cluster);
                        }

//sedan försök att räkna ut probability för alla clonade tiles, med hjälp utav dom riktiga tilesen.
//clonetile.neighbour bla bla bla räkna probability
//tror att probabilityn blir negativ om bomben är felplacerad. 
                    }
                    var bestCluster = clusters.OrderBy(x => x.GetProbability()).First();
                    //var indexOfBombs = bestCluster.
                }
            }
        }
        private IList<ITile> GetGroupingLeft(ITile tile, List<ITile> result = null)
        {
            result = result ?? new List<ITile>();
            if(tile.NeighbourIndexes.Any(x => x - decimal.One == tile.MyIndex))
            {
                var allTiles = this.TileHandler
                    .GetTilesInterface()
                    .ToList();
                var leftNeighbour = allTiles[tile.MyIndex - (int)decimal.One];

                if(!leftNeighbour.IsKnown)
                {
                    result.Add(leftNeighbour);
                    
                    result.AddRange(this.GetGroupingLeft(leftNeighbour, result));
                }
            }

            return result;
        }
         private IList<ITile> GetGroupingRight(ITile tile, List<ITile> result = null)
        {
            result = result ?? new List<ITile>();
            if(tile.NeighbourIndexes.Any(x => x + decimal.One == tile.MyIndex))
            {
                var allTiles = this.TileHandler
                    .GetTilesInterface()
                    .ToList();
                var rightNeighbour = allTiles[tile.MyIndex + (int)decimal.One];
                if(!rightNeighbour.IsKnown)
                {
                    result.Add(rightNeighbour);
                
                    result.AddRange(this.GetGroupingLeft(rightNeighbour, result));
                }
            }

            return result;
        }

        private IEnumerable<ITile> FindAllNeighboursWithNumber(ITile tile)
        {
            return this.FindNeighbours(tile)
                .Where(x => x.IsKnown)
                .Where(x => x.GetNumberOfBombNeighbours() != decimal.Zero)
                .ToList()
                ;
        }
        private IEnumerable<ITile> FindNeighbours(ITile tile)
        {
            var allTiles = this.TileHandler
                .GetTilesInterface()
                .ToList();

            var allNeighbours = new List<ITile>();
            foreach(var index in tile.NeighbourIndexes)
            {
                var neighbour = allTiles[index];
                allNeighbours.Add(neighbour);
            }

            return allNeighbours;
        }
        private class TileCluster
        {
            public IList<ITile> Tiles {get;private set;}
            public bool Any => this.Tiles.Count > decimal.Zero;
            public int Count => this.Tiles.Count;
            public TileCluster(IList<ITile> tiles)
            {
                this.Tiles = tiles;
            }

            public decimal GetProbability()
            {
                return decimal.Zero;
            }
        }
    }
}