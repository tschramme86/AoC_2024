using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2024.Days.Day20
{
    internal partial class Day20Challenge : AoCChallengeBase
    {
        public override int Day => 20;
        public override string Name => "Reindeer Maze";

        protected override object ExpectedTestResultPartOne => 44;
        protected override object ExpectedTestResultPartTwo => 285;

        private readonly (int x, int y)[] _moveDirections = [(0, -1), (1, 0), (0, 1), (-1, 0)];

        private bool[,] _map = new bool[0, 0];
        private int _w;
        private int _h;
        private (int x, int y) _startPosition = (0, 0);
        private (int x, int y) _endPosition = (0, 0);

        protected override object SolvePartOneInternal(string[] inputData)
        {
            this.ReadMaze(inputData);
            return CalcUniqueCheatPositions(2, this.IsOnTestData ? 1 : 100);
        }

        protected override object SolvePartTwoInternal(string[] inputData)
        {
            this.ReadMaze(inputData);
            return CalcUniqueCheatPositions(20, this.IsOnTestData ? 50 : 100);
        }

        private int CalcUniqueCheatPositions(int maxCheatTime, int minCheatGain)
        {
            var mapFromStart = this.CalcDistanceMap(this._startPosition);
            var mapToEnd = this.CalcDistanceMap(this._endPosition);
            var refScore = mapFromStart[this._endPosition.x, this._endPosition.y];

            var uniqueCheats = 0;
            for (var y = 0; y < this._h; y++)
            {
                for (var x = 0; x < this._w; x++)
                {
                    // cannot start cheating within a wall
                    if (this._map[x, y]) continue;

                    // check all possible positions that can be reached by cheating from current (x, y)
                    for (var dy = -maxCheatTime; dy <= maxCheatTime; dy++)
                    {
                        for (var dx = -maxCheatTime; dx <= maxCheatTime; dx++)
                        {
                            var cheatDuration = Math.Abs(dx) + Math.Abs(dy);
                            if (cheatDuration > maxCheatTime || cheatDuration < 2) continue;

                            var cx = x + dx;
                            var cy = y + dy;
                            if (cx < 0 || cx >= this._w || cy < 0 || cy >= this._h) continue;
                            if (this._map[cx, cy]) continue;

                            // does this cheat position provide a better score than the reference score?
                            var cheatScore = mapFromStart[x, y] + cheatDuration + mapToEnd[cx, cy];
                            if (cheatScore <= refScore - minCheatGain)
                            {
                                uniqueCheats++;
                            }
                        }
                    }
                }
            }
            return uniqueCheats;
        }

        private int[,] CalcDistanceMap((int x, int y) startPos)
        {
            var distanceMap = new int[this._w, this._h];
            for (int y = 0; y < this._h; y++)
            {
                for (int x = 0; x < this._w; x++)
                {
                    distanceMap[x, y] = int.MaxValue;
                }
            }

            distanceMap[startPos.x, startPos.y] = 0;
            var currentPositions = new List<(int x, int y)>(128) { startPos };
            var newPositions = new List<(int x, int y)>(128);
            var round = 1;
            do
            {
                newPositions.Clear();
                foreach (var pos in currentPositions)
                {
                    foreach (var d in this._moveDirections)
                    {
                        var newPos = this.AddP(pos, d);
                        if (newPos.x < 0 || newPos.x >= this._w || newPos.y < 0 || newPos.y >= this._h) continue;
                        if (this._map[newPos.x, newPos.y]) continue;
                        if (distanceMap[newPos.x, newPos.y] <= round) continue;
                        distanceMap[newPos.x, newPos.y] = round;
                        newPositions.Add(newPos);
                    }
                }
                currentPositions.Clear();
                currentPositions.AddRange(newPositions);
                round++;
            } while (newPositions.Count > 0);
            return distanceMap;
        }

        private void ReadMaze(string[] inputData)
        {
            this._map = this.MapInput(inputData, (c, pos) =>
            {
                if (c == 'S') this._startPosition = pos;
                if (c == 'E') this._endPosition = pos;
                return c == '#';
            });
            this._w = this._map.GetLength(0);
            this._h = this._map.GetLength(1);
        }
    }
}
