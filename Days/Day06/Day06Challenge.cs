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
            return this.SimulateWalk(null);
        }

        protected override object SolvePartTwoInternal(string[] inputData)
        {
            this.ParseMap(inputData);
            var creatingLoopPositions = 0;
            Parallel.For(0, this._height, y=> {
                for(var x=0; x<this._width; x++) {
                    if(this._map[x,y] || (this._start.x == x && this._start.y == y)) continue;
                    if(this.SimulateWalk((x,y)) < 0)
                        Interlocked.Increment(ref creatingLoopPositions);
                }
            });
            return creatingLoopPositions;
        }

        private int SimulateWalk((int x, int y)? additionalObstacle) {
            var visited = new HashSet<(int x, int y)>();
            var visitedWDir = new HashSet<(int x, int y, int d)>();
            var pos = this._start;
            var d = 0;
            var obs = additionalObstacle ?? (-100, -100);
            while(true) {
                visited.Add(pos);
                if(visitedWDir.Contains((x: pos.x, y: pos.y, d: d))) return -1;
                visitedWDir.Add((x: pos.x, y: pos.y, d: d));
                var next = this.AddP(pos, this._directions[d]);
                if(next.x < 0 || next.y < 0 || next.x >= this._width || next.y >= this._height) break;
                if(this._map[next.x, next.y] || (obs.x == next.x && obs.y == next.y))
                    d = (d + 1) % 4;
                else
                    pos = next;
            }

            return visited.Count;
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
