using AoC2024.Helper;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AoC2024.Days.Day25
{
    internal partial class Day25Challenge : AoCChallengeBase
    {
        public override int Day => 25;
        public override string Name => "Code Chronicle";

        protected override object ExpectedTestResultPartOne => 3;
        protected override object ExpectedTestResultPartTwo => 0;

        private readonly List<int[]> _keys = [];
        private readonly List<int[]> _locks = [];

        protected override object SolvePartOneInternal(string[] inputData)
        {
            this.ReadKeyLocks(inputData);
            var fits = 0;
            foreach (var key in this._keys)
            {
                foreach (var @lock in this._locks)
                {
                    var sumPins = new int[5];
                    for (int i = 0; i < 5; i++)
                    {
                        sumPins[i] = key[i] + @lock[i];
                    }
                    if (sumPins.All(p => p <= 5))
                    {
                        fits++;
                    }
                }
            }
            return fits;
        }

        protected override object SolvePartTwoInternal(string[] inputData)
        {
            return 0;
        }

        private void ReadKeyLocks(string[] inputData)
        {
            this._keys.Clear();
            this._locks.Clear();
            var isNewObject = true;
            var pins = new int[5];
            foreach (string line in inputData)
            {
                if (isNewObject)
                {
                    pins = new int[5];
                    if (line == "#####")
                        this._locks.Add(pins);
                    else
                    {
                        this._keys.Add(pins);
                        pins[0] = pins[1] = pins[2] = pins[3] = pins[4] = -1;
                    }
                    isNewObject = false;
                }
                else if (string.IsNullOrEmpty(line))
                {
                    isNewObject = true;
                }
                else
                {
                    for (int i = 0; i < 5; i++)
                    {
                        pins[i] += line[i] == '#' ? 1 : 0;
                    }
                }
            }
        }
    }
}
