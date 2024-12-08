using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AoC2024.Days.Day08
{
    internal class Day08Challenge : AoCChallengeBase
    {
        public override int Day => 8;
        public override string Name => "Resonant Collinearity";

        protected override object ExpectedTestResultPartOne => 14;
        protected override object ExpectedTestResultPartTwo => 34;

        private Dictionary<char, List<(int x, int y)>> _antennaDict = [];
        private int _w = 0;
        private int _h = 0;

        protected override object SolvePartOneInternal(string[] inputData)
        {
            this.CreateAntennaDict(inputData);

            var antinodes = new HashSet<(int x, int y)>();
            foreach (var c in this._antennaDict.Keys)
            {
                if (this._antennaDict[c].Count < 2) continue;
                for (var i = 0; i < this._antennaDict[c].Count; i++)
                {
                    for (var j = 0; j < this._antennaDict[c].Count; j++)
                    {
                        if (i == j) continue;

                        var dx = this._antennaDict[c][j].x - this._antennaDict[c][i].x;
                        var dy = this._antennaDict[c][j].y - this._antennaDict[c][i].y;

                        var a1 = (x: this._antennaDict[c][i].x - dx, y: this._antennaDict[c][i].y - dy);
                        var a2 = (x: this._antennaDict[c][j].x + dx, y: this._antennaDict[c][j].y + dy);

                        if(a1.x >= 0 && a1.x < this._w && a1.y >= 0 && a1.y < this._h)
                            antinodes.Add(a1);
                        if (a2.x >= 0 && a2.x < this._w && a2.y >= 0 && a2.y < this._h)
                            antinodes.Add(a2);
                    }
                }
            }

            return antinodes.Count;
        }

        protected override object SolvePartTwoInternal(string[] inputData)
        {
            this.CreateAntennaDict(inputData);

            var antinodes = new HashSet<(int x, int y)>();
            foreach (var c in this._antennaDict.Keys)
            {
                if (this._antennaDict[c].Count < 2) continue;
                for (var i = 0; i < this._antennaDict[c].Count; i++)
                {
                    for (var j = 0; j < this._antennaDict[c].Count; j++)
                    {
                        if (i == j) continue;

                        var dx = this._antennaDict[c][j].x - this._antennaDict[c][i].x;
                        var dy = this._antennaDict[c][j].y - this._antennaDict[c][i].y;

                        for (var n = 0; ; n++)
                        {
                            var a1 = (x: this._antennaDict[c][i].x - dx * n, y: this._antennaDict[c][i].y - dy * n);
                            var a2 = (x: this._antennaDict[c][j].x + dx * n, y: this._antennaDict[c][j].y + dy * n);

                            var added = false;
                            if (a1.x >= 0 && a1.x < this._w && a1.y >= 0 && a1.y < this._h)
                            {
                                antinodes.Add(a1);
                                added = true;
                            }
                            if (a2.x >= 0 && a2.x < this._w && a2.y >= 0 && a2.y < this._h)
                            {
                                antinodes.Add(a2);
                                added = true;
                            }
                            if (!added) break;
                        }
                    }
                }
            }

            return antinodes.Count;
        }

        private void CreateAntennaDict(string[] inputData)
        {
            var dict = new Dictionary<char, List<(int x, int y)>>();
            var map = this.MapInput(inputData, (char c, (int x, int y) p) =>
            {
                if (!dict.ContainsKey(c))
                {
                    dict[c] = new List<(int x, int y)>();
                }
                dict[c].Add(p);
                return c;
            });
            dict.Remove('.');

            this._antennaDict = dict;
            this._w = map.GetLength(0);
            this._h = map.GetLength(1);
        }
    }
}
