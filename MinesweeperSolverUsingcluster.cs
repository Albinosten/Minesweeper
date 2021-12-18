using System.Linq;
using System;
using System.Collections.Generic;
using Minesweeper;

namespace MinesweeperSolver
{
    public class MinesweeperSolverUsingCluster : IMinesweeperSolver
    {
        ITileHandler TileHandler;
        TileCreator TileCreator;
        Permuterer Permuterer;
        BombProbabilityCalculator BombProbabilityCalculator;
        public MinesweeperSolverUsingCluster(TileHandler tileHandler
            , TileCreator tileCreator
            , Permuterer permuterer
            , BombProbabilityCalculator bombProbabilityCalculator
            )
        {
            this.TileHandler = tileHandler;   
            this.TileCreator = tileCreator;
            this.Permuterer = permuterer;
            this.BombProbabilityCalculator = bombProbabilityCalculator;
        }
        public void SolveNext(GameContext gameContext)
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

                var clusters = new List<TileClusterPermutation>();
                // var cluster = new TileCluster(this.TileCreator.CloneTiles(tilesToUse).ToList());
                if(tilesToUse.Any())
                {
                    tilesToUse.Add(tile);//addera sigsjälv kan vara bra hehe

                    //kanske lägga till så att varje Yile vet hjur många bomber som är kvar bredvis sig.
                    //lös all skit här xD

                    //testa alla olika permuteringar och se vilken som har samanlagt bästa score.
                    //eftersom en känd tile kan ha bomber som inte är i detta kluster så kanske
                    //det inte går att få ett perfect score. därför välja bästa

                    //antal bomber att försöka placera ut i all permisioner
                    for(int numberOfBombs = tilesToUse.Count; numberOfBombs > 0; numberOfBombs--)
                    {
                        var permutList = this.Permuterer.GetUniquePer(tilesToUse.Count, numberOfBombs);
                        foreach(var permut in permutList)
                        {
                            var clusterPermutation = new TileClusterPermutation(this.TileCreator.CloneTiles(tilesToUse).ToList());
                            clusterPermutation.NumberOfBombsForPermutation = numberOfBombs;
                            for(var index = 0; index < tilesToUse.Count; index++)
                            {
                                if(permut[index])
                                {
                                    clusterPermutation.Tiles[index].FlagAsBomb(false);
                                }
                            }
                            //räkna ut probabiliteten för detta cluster
                            //kan detta göras via att göra vanliga uträkningen?


                            var tilesWithClusterPermutationPosibility = this.GetAllTilesWithClusterPermutationPosibility(clusterPermutation);
                            this.BombProbabilityCalculator.CalculateProbabilityAndMarkAsBomb(tilesWithClusterPermutationPosibility);
                            clusters.Add(clusterPermutation);
                        }

//sedan försök att räkna ut probability för alla clonade tiles, med hjälp utav dom riktiga tilesen.
//clonetile.neighbour bla bla bla räkna probability
//tror att probabilityn blir negativ om bomben är felplacerad. 
                    }
                    var clustersInOrder = clusters.OrderByDescending(x => x.ProbabilityToBeSafe).ToList();
                    var bestCluster = clustersInOrder.First();

                    //Todo: blir alltid så att en cluster med 1 bomb blir vald
                    //eftersom jag börjar simulera med 1 bomb först, och dom går alltid att anta är rätt
                    //eftersom vi inte kan bevisa mossatsen. men det blir ju fel! 
                    //på något sätt måste vi veta antalet bomber i clustret som ska sättas in..
                    //kanske går om man börjar med anta max och sedan räkna ner?

                    Console.WriteLine("number of clusters : " + clustersInOrder.Count());
                    Console.WriteLine("number of Good Clusters : " + clustersInOrder.Count(x => x.ProbabilityToBeSafe == decimal.One));
                    Console.WriteLine("number of Bad Clusters : " + clustersInOrder.Count(x => x.ProbabilityToBeSafe == -decimal.One));
                    Console.WriteLine("number of neutral Clusters : " + clustersInOrder.Count(x => x.ProbabilityToBeSafe == decimal.Zero));

                    Console.WriteLine("BestCluster is cluster with NumberOfBombsForPermutation :" + bestCluster.NumberOfBombsForPermutation);


                    var alltiles = this.TileHandler
                        .GetTilesInterface()
                        .ToList();

                    var indexOfTotalySafe = bestCluster.IndexOfTotalySafe();
                    foreach(var index in indexOfTotalySafe) //kanske bara en eftersom man löser en i taget?
                    {
                        Console.WriteLine("indexOfTotalySafe:" + index);
                        this.TileHandler.SelectTile(alltiles[index]);
                    }

                    var indexOfBombs = bestCluster.IndexOfBombs();
                    foreach(var index in indexOfBombs)
                    {
                        Console.WriteLine("indexOfBomb:" + index);

                        alltiles[index].FlagAsBomb();
                    }
                }
            }
        }

        private  IList<ITile> GetAllTilesWithClusterPermutationPosibility(TileClusterPermutation tileCluster)
        {
            var result = new List<ITile>();

            foreach(var tile in this.TileHandler.GetTilesInterface())
            {
                if(tileCluster.Tiles.Any(x => x.MyIndex == tile.MyIndex))
                {
                    //finns en fake'ad tile ifrån cluster permutation
                    result.Add(tileCluster.Tiles.Single(x => x.MyIndex == tile.MyIndex));
                }
                else
                {
                    result.Add(tile);
                }
            }
            return result;
        }

        private IList<ITile> GetGroupingLeft(ITile tile, List<ITile> result = null)
        {
            result = result ?? new List<ITile>();
            int index = tile.MyIndex - (int)decimal.One;
            if(tile.NeighbourIndexes.Any(x => x  == index))
            {
                var allTiles = this.TileHandler
                    .GetTilesInterface()
                    .ToList();
                var leftNeighbour = allTiles[index];

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

            int index = tile.MyIndex + (int)decimal.One;

            if(tile.NeighbourIndexes.Any(x => x == index))
            {
                var allTiles = this.TileHandler
                    .GetTilesInterface()
                    .ToList();
                var rightNeighbour = allTiles[index];
                if(!rightNeighbour.IsKnown)
                {
                    result.Add(rightNeighbour);
                
                    result.AddRange(this.GetGroupingRight(rightNeighbour, result));
                }
            }

            return result;
        }

        private IEnumerable<ITile> FindAllNeighboursWithNumber(ITile tile)
        {
            return this.FindNeighbours(tile)
                .Where(x => x.IsToggled)
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
        private class TileClusterPermutation
        {
            public IList<ITile> Tiles { get; private set;}
            public bool Any => this.Tiles.Count > decimal.Zero;
            public int NumberOfBombsForPermutation {get;set;}

            public IList<int> IndexOfTotalySafe()
            {
                var totalySafeTiles = this.Tiles
                    .Where(x => x.GetProbabilityToBeABomb().HasValue)
                    .Where(x => x.GetProbabilityToBeABomb().Value == decimal.Zero);
                
                return totalySafeTiles
                    .Select(x => x.MyIndex)
                    .ToList();
            }

            public IList<int> IndexOfBombs()
            {
                var totalySafeTiles = this.Tiles
                    .Where(x => x.GetProbabilityToBeABomb().HasValue)
                    .Where(x => x.GetProbabilityToBeABomb().Value > 99);
                
                return totalySafeTiles
                    .Select(x => x.MyIndex)
                    .ToList();
            }

            public int Count => this.Tiles.Count;
            public TileClusterPermutation(IList<ITile> tiles)
            {
                this.Tiles = tiles;
            }

            public decimal ProbabilityToBeSafe => this.GetProbabilityToBeSafe();

            public decimal GetProbabilityToBeSafe()
            {
                var tiles = this.Tiles
                    .Where(x => x.GetProbabilityToBeABomb().HasValue) 
//Alla borde ha värde
//stämmer inte. inte alltid det finns ett värde. ex om alla tiles i klustret blir markerad som bomb.,

                    ;
                if(tiles.Any(x => x.GetProbabilityToBeABomb().Value < decimal.Zero))
                {
                    return -decimal.One;
                    //då har vi hittat en permutering som inte funkar.
                }
                else if(this.Tiles.All(x => x.IsFlaggedAsBomb || x.GetProbabilityToBeABomb() == decimal.Zero))
                {   

//kanske funkar. betyder att alla i klustert blivit flaggad eller makerad totaly safe
//this.Tiles.All(x => x.IsFlaggedAsBomb || x.GetProbabilityToBeABomb() == decimal.Zero)


//hitta en permutering som funkar.
//hur vet jag det? 
// - Kanske för att alla har antingen blivit markerat som bomb, eller prob = 0.

                    return decimal.One;
                }
                return decimal.Zero;
            }
        }
    }
}