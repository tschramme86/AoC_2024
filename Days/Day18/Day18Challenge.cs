using Google.OrTools.LinearSolver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AoC2024.Days.Day18
{
    internal partial class Day18Challenge : AoCChallengeBase
    {
        public override int Day => 18;
        public override string Name => "RAM Run";

        protected override object ExpectedTestResultPartOne => 22;
        protected override object ExpectedTestResultPartTwo => "6,1";

        private readonly (int x, int y)[] _moveDirections = [(0, -1), (1, 0), (0, 1), (-1, 0)];

        private readonly List<(int x, int y)> _bytesFalling = [];
        private bool[,] _memory = new bool[0, 0];
        private int _w;
        private int _h;
        private (int x, int y) _startPos;
        private (int x, int y) _endPos;

        protected override object SolvePartOneInternal(string[] inputData)
        {
            this.ParseMemoryMap(inputData);
            this.SimulateFallingBytes(0, this.IsOnTestData ? 12 : 1024);
            return this.FindShortestPath();
        }

        protected override object SolvePartTwoInternal(string[] inputData)
        {
            this.ParseMemoryMap(inputData);
            this.SimulateFallingBytes(0, this.IsOnTestData ? 12 : 1024);
            for(var i=(this.IsOnTestData ? 12 : 1024); i<this._bytesFalling.Count; i++)
            {
                this.SimulateFallingBytes(i, 1);
                if(this.FindShortestPath() < 0)
                {
                    return $"{this._bytesFalling[i].x},{this._bytesFalling[i].y}";
                }
            }
            return "n/a";
        }

        private int FindShortestPath()
        {
            var memDist = new int[this._w, this._h];
            memDist[this._endPos.x, this._endPos.y] = -1;

            var nextSteps = new List<(int x, int y)>(512) { (x: this._startPos.x, y: this._startPos.y) };
            var newNextSteps = new List<(int x, int y)>(512);

            var candidateList = nextSteps;
            var takingList = newNextSteps;

            var round = 0;
            do
            {
                round++;
                takingList.Clear();
                foreach (var c in candidateList)
                {
                    foreach (var d in this._moveDirections)
                    {
                        var newPos = this.AddP(c, d);
                        if (newPos.x < 0 || newPos.x >= this._w || newPos.y < 0 || newPos.y >= this._h)
                            continue;
                        if (this._memory[newPos.x, newPos.y] || memDist[newPos.x, newPos.y] > 0)
                            continue;
                        memDist[newPos.x, newPos.y] = round;
                        takingList.Add(newPos);
                    }
                }
                (candidateList, takingList) = (takingList, candidateList);
            } while (nextSteps.Count > 0 && memDist[this._endPos.x, this._endPos.y] < 0);
            return memDist[this._endPos.x, this._endPos.y];
        }


        private void ParseMemoryMap(string[] inputData)
        {
            this._bytesFalling.Clear();
            foreach(var l in inputData)
            {
                var split = l.Split(',');
                this._bytesFalling.Add((int.Parse(split[0]), int.Parse(split[1])));
            }
            this._w = this.IsOnTestData ? 7 : 71;
            this._h = this.IsOnTestData ? 7 : 71;
            this._startPos = (0, 0);
            this._endPos = (this._w - 1, this._h - 1);

            this.ClearMemory();
        }

        private void ClearMemory()
        {
            this._memory = new bool[this._w, this._h];
        }

        private void SimulateFallingBytes(int startIdx, int length)
        {
            for (var idx = startIdx; idx < startIdx + length; idx++)
            {
                this._memory[this._bytesFalling[idx].x, this._bytesFalling[idx].y] = true;
            }
        }
    }
}
