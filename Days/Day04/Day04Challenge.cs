using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AoC2024.Days.Day04
{
    internal class Day04Challenge : AoCChallengeBase
    {
        public override int Day => 4;
        public override string Name => "Ceres monitoring station";

        protected override object ExpectedTestResultPartOne => 18;
        protected override object ExpectedTestResultPartTwo => 9;

        private readonly List<(int dx, int dy)> _directions = [(1,0), (-1,0), (0,1), (0,-1), (-1,-1), (1,-1), (1,1), (-1,1)];
        private readonly List<char> _word = ['X', 'M', 'A', 'S'];

        protected override object SolvePartOneInternal(string[] inputData)
        {
            var grid = this.MapInput(inputData);
            var h = grid.GetLength(1);
            var w = grid.GetLength(0);
            var wordCount = 0;
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    foreach(var d in this._directions) {
                        var v = (x,y);
                        var wordFound = true;
                        for (var i = 0; i < this._word.Count; i++)
                        {
                            if(v.x < 0 || v.y < 0 || v.x >= w || v.y >= h || grid[v.x, v.y] != this._word[i]) {
                                wordFound = false;
                                break;
                            }
                            v = AddP(v, d);
                        }
                        if(wordFound) wordCount++;
                    }
                }
            }
            return wordCount;
        }

        private readonly List<List<(int dx, int dy, char c)>> _wordToSearchP2 = [
            [(0,0,'M'),(1,1,'A'),(2,2,'S'),(2,0,'M'),(0,2,'S')],
            [(0,0,'M'),(1,1,'A'),(2,2,'S'),(2,0,'S'),(0,2,'M')],
            [(0,0,'S'),(1,1,'A'),(2,2,'M'),(2,0,'M'),(0,2,'S')],
            [(0,0,'S'),(1,1,'A'),(2,2,'M'),(2,0,'S'),(0,2,'M')]
        ];
        protected override object SolvePartTwoInternal(string[] inputData)
        {
            var grid = this.MapInput(inputData);
            var h = grid.GetLength(1);
            var w = grid.GetLength(0);
            var wordCount = 0;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    foreach (var wordMatrix in this._wordToSearchP2)
                    {
                        var wordFound = true;
                        foreach (var c in wordMatrix)
                        {
                            var v = this.AddP((x,y), (c.dx, c.dy));
                            if(v.x < 0 || v.y < 0 || v.x >= w || v.y >= h || grid[v.x, v.y] != c.c) {
                                wordFound = false;
                                break;
                            }
                        }
                        if(wordFound) {
                            wordCount++;
                            break;
                        }
                    }
                }
            }
            return wordCount;
        }
    }
}
