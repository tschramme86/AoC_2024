using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AoC2024.Days.Day06
{
    internal class Day06Challenge : AoCChallengeBase
    {
        public override int Day => 6;
        public override string Name => "Guard Gallivant";

        protected override object ExpectedTestResultPartOne => 41;
        protected override object ExpectedTestResultPartTwo => 6;

        private readonly (int dx, int dy)[] _directions = [(0,-1),(1,0),(0,1),(-1,0)];
        private (int x, int y) _start = (0,0);
        private bool[,] _map = null!;
        private int _width;
        private int _height;
        protected override object SolvePartOneInternal(string[] inputData)
        {
            this.ParseMap(inputData);
            return this.SimulateWalk();
        }

        protected override object SolvePartTwoInternal(string[] inputData)
        {
            this.ParseMap(inputData);
            var creatingLoopPositions = 0;
            Parallel.For(0, this._height, 
                () => 0,
                (y, loop, localState) =>
                {
                    var lsum = 0;
                    for (var x = 0; x < this._width; x++)
                    {
                        if (this._map[x, y] || (this._start.x == x && this._start.y == y)) continue;
                        if (this.SimulateWalkHasLoop((x, y)))
                            lsum++;
                    }
                    return localState + lsum;
                },
                localState => Interlocked.Add(ref creatingLoopPositions, localState)
            );

            return creatingLoopPositions;
        }

        private int SimulateWalk() {
            var visited = new HashSet<(int x, int y)>();
            var pos = this._start;
            var d = 0;
            while(true) {
                visited.Add(pos);
                var next = this.AddP(pos, this._directions[d]);
                if(next.x < 0 || next.y < 0 || next.x >= this._width || next.y >= this._height) break;
                if(this._map[next.x, next.y])
                    d = (d + 1) % 4;
                else
                    pos = next;
            }

            return visited.Count;
        }

        private bool SimulateWalkHasLoop((int x, int y) additionalObstacle)
        {
            var visitedMap = new bool[this._width, this._height, 4];
            var pos = this._start;
            var d = 0;
            while (true)
            {
                if (visitedMap[pos.x, pos.y, d]) return true;
                visitedMap[pos.x, pos.y, d] = true;
                var next = this.AddP(pos, this._directions[d]);
                if (next.x < 0 || next.y < 0 || next.x >= this._width || next.y >= this._height) break;
                if (this._map[next.x, next.y] || (additionalObstacle.x == next.x && additionalObstacle.y == next.y))
                    d = (d + 1) % 4;
                else
                    pos = next;
            }

            return false;
        }

        private void ParseMap(string[] inputData)
        {
            this._map = this.MapInput(inputData, (c, p) => {
                if(c == '#') return true;
                if(c == '^') this._start = p;
                return false;
            });
            this._width = this._map.GetLength(0);
            this._height = this._map.GetLength(1);
        }
    }
}
