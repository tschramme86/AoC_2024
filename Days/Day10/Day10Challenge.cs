using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AoC2024.Days.Day10
{
    internal class Day10Challenge : AoCChallengeBase
    {

        public override int Day => 10;
        public override string Name => "Hoof It";

        protected override object ExpectedTestResultPartOne => 36;
        protected override object ExpectedTestResultPartTwo => 81;

        private int[,]? _map;
        private int _width;
        private int _height;
        private List<(int x, int y)> _trailheads = [];

        protected override object SolvePartOneInternal(string[] inputData)
        {
            this.ParseHikingMap(inputData);
            var sum = 0;
            foreach (var trailhead in this._trailheads)
            {
                var thScore = this.CalcTrailheadScore([trailhead], 0);
                sum += thScore;
            }
            return sum;
        }

        protected override object SolvePartTwoInternal(string[] inputData)
        {
            this.ParseHikingMap(inputData);
            var sum = 0;
            foreach (var trailhead in this._trailheads)
            {
                var thRating = this.CalcTrailheadRating([[trailhead]], 0);
                sum += thRating;
            }
            return sum;
        }

        private int CalcTrailheadScore(List<(int x, int y)> stepLocations, int level)
        {
            ArgumentNullException.ThrowIfNull(this._map);

            var nextLocations = new HashSet<(int x, int y)>();
            foreach (var step in stepLocations)
            {
                (int x, int y)[] neighbors = [
                    (step.x - 1, step.y),
                    (step.x + 1, step.y),
                    (step.x, step.y - 1),
                    (step.x, step.y + 1)
                ];
                foreach (var neighbor in neighbors)
                {
                    if (neighbor.x < 0 || neighbor.x >= this._width || neighbor.y < 0 || neighbor.y >= this._height) continue;
                    if (this._map[neighbor.x, neighbor.y] == level + 1)
                        nextLocations.Add(neighbor);
                }
            }

            if (level == 8) return nextLocations.Count;
            if (nextLocations.Count > 0)
                return this.CalcTrailheadScore(nextLocations.ToList(), level + 1);

            return 0;
        }

        private int CalcTrailheadRating(List<(int x, int y)[]> paths, int level)
        {
            ArgumentNullException.ThrowIfNull(this._map);
            var newPaths = new List<(int x, int y)[]>();
            foreach (var path in paths)
            {
                var lastStep = path[^1];
                (int x, int y)[] neighbors = [
                    (lastStep.x - 1, lastStep.y),
                    (lastStep.x + 1, lastStep.y),
                    (lastStep.x, lastStep.y - 1),
                    (lastStep.x, lastStep.y + 1)
                ];
                foreach (var neighbor in neighbors)
                {
                    if (neighbor.x < 0 || neighbor.x >= this._width || neighbor.y < 0 || neighbor.y >= this._height) continue;
                    if (this._map[neighbor.x, neighbor.y] == level + 1)
                    {
                        var newPath = new (int x, int y)[path.Length + 1];
                        Array.Copy(path, newPath, path.Length);
                        newPath[path.Length] = neighbor;
                        newPaths.Add(newPath);
                    }
                }
            }
            if(level == 8) return newPaths.Count;
            return this.CalcTrailheadRating(newPaths, level + 1);
        }

        private void ParseHikingMap(string[] inputData)
        {
            this._trailheads.Clear();
            this._map = this.MapInput(inputData, (c, pos) =>
            {
                if(c == '0') this._trailheads.Add(pos);
                return c - '0';
            });
            this._width = this._map.GetLength(0);
            this._height = this._map.GetLength(1);
        }
    }
}
