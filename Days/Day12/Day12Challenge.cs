using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2024.Days.Day12
{
    internal class Day12Challenge : AoCChallengeBase
    {
        class GardenTile
        {
            public char Plant { get; set; } = '.';

            public GardenTile? NeighborNorth { get; set; }
            public GardenTile? NeighborEast { get; set; }
            public GardenTile? NeighborSouth { get; set; }
            public GardenTile? NeighborWest { get; set; }

            public int Fences => new[] { this.NeighborNorth, this.NeighborEast, this.NeighborSouth, this.NeighborWest }.Count(n => n == null);

            public int Corners
            {
                get
                {
                    var corners = 0;
                    if (this.NeighborNorth == null && this.NeighborEast == null) corners++;
                    if (this.NeighborEast == null && this.NeighborSouth == null) corners++;
                    if (this.NeighborSouth == null && this.NeighborWest == null) corners++;
                    if (this.NeighborWest == null && this.NeighborNorth == null) corners++;

                    if (this.NeighborEast == null && this.NeighborNorth != null && this.NeighborNorth.NeighborEast != null) corners++;
                    if (this.NeighborSouth == null && this.NeighborEast != null && this.NeighborEast.NeighborSouth != null) corners++;
                    if (this.NeighborWest == null && this.NeighborSouth != null && this.NeighborSouth.NeighborWest != null) corners++;
                    if (this.NeighborNorth == null && this.NeighborWest != null && this.NeighborWest.NeighborNorth != null) corners++;

                    return corners;
                }
            }
        }
        
        public override int Day => 12;
        public override string Name => "Garden Groups";

        protected override object ExpectedTestResultPartOne => 1930;
        protected override object ExpectedTestResultPartTwo => 1206;

        private readonly List<GardenTile> _fullGarden = [];

        protected override object SolvePartOneInternal(string[] inputData)
        {
            this.ReadGarden(inputData);
            var gardenGroups = this.CreateGardenGroups();

            return gardenGroups.Sum(g => g.Count * g.Sum(t => t.Fences));
        }

        protected override object SolvePartTwoInternal(string[] inputData)
        {
            this.ReadGarden(inputData);
            var gardenGroups = this.CreateGardenGroups();

            return gardenGroups.Sum(g => g.Count * g.Sum(t => t.Corners));
        }

        private List<List<GardenTile>> CreateGardenGroups()
        {
            var allTiles = new HashSet<GardenTile>(this._fullGarden);
            var gardenGroups = new List<List<GardenTile>>();
            while (allTiles.Count > 0)
            {
                var gardenGroup = new HashSet<GardenTile>();
                var nextVisit = new Stack<GardenTile>();
                nextVisit.Push(allTiles.First());
                while (nextVisit.Count > 0)
                {
                    var currentTile = nextVisit.Pop();
                    if (gardenGroup.Add(currentTile))
                    {
                        allTiles.Remove(currentTile);
                        if (currentTile.NeighborNorth != null) nextVisit.Push(currentTile.NeighborNorth);
                        if (currentTile.NeighborEast != null) nextVisit.Push(currentTile.NeighborEast);
                        if (currentTile.NeighborSouth != null) nextVisit.Push(currentTile.NeighborSouth);
                        if (currentTile.NeighborWest != null) nextVisit.Push(currentTile.NeighborWest);
                    }
                }
                gardenGroups.Add([.. gardenGroup]);
            }
            return gardenGroups;
        }

        private void ReadGarden(string[] inputData)
        {
            this._fullGarden.Clear();
            var gardenMap = this.MapInput(inputData, (c, l) => new GardenTile { Plant = c });
            var width = gardenMap.GetLength(0);
            var height = gardenMap.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var tile = gardenMap[x, y];
                    tile.NeighborNorth = y > 0 && gardenMap[x, y - 1].Plant == tile.Plant ? gardenMap[x, y - 1] : null;
                    tile.NeighborEast = x < width - 1 && gardenMap[x + 1, y].Plant == tile.Plant ? gardenMap[x + 1, y] : null;
                    tile.NeighborSouth = y < height - 1 && gardenMap[x, y + 1].Plant == tile.Plant ? gardenMap[x, y + 1] : null;
                    tile.NeighborWest = x > 0 && gardenMap[x - 1, y].Plant == tile.Plant ? gardenMap[x - 1, y] : null;
                    this._fullGarden.Add(tile);
                }
            }
        }
    }
}
