using AoC2024.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2024.Days.Day19
{
    internal partial class Day19Challenge : AoCChallengeBase
    {
        public override int Day => 19;
        public override string Name => "Linen Layout";

        protected override object ExpectedTestResultPartOne => 6L;
        protected override object ExpectedTestResultPartTwo => 16L;

        private readonly List<string> _availableTowels = [];
        private readonly List<string> _desiredPatterns = [];

        protected override object SolvePartOneInternal(string[] inputData)
        {
            this.ParseTowels(inputData);
            var result = 0L;
            foreach(var p in this._desiredPatterns)
            {
                result += (this.CalcPossibleTowelArrangements(p) > 0 ? 1 : 0);
            }
            return result;
        }

        protected override object SolvePartTwoInternal(string[] inputData)
        {
            this.ParseTowels(inputData);
            var result = 0L;
            foreach (var p in this._desiredPatterns)
            {
                result += this.CalcPossibleTowelArrangements(p);
            }
            return result;
        }

        private long CalcPossibleTowelArrangements(string desiredPattern)
        {
            Func<int, long> fnArrangementFinder = _ => throw new NotImplementedException();
            fnArrangementFinder = FnCache.Make((int index) =>
            {
                if (index >= desiredPattern.Length)
                {
                    return 1L;
                }
                long possibleArrangements = 0;
                foreach (var towel in this._availableTowels)
                {
                    if (desiredPattern.AsSpan()[index..].StartsWith(towel))
                    {
                        var tails = fnArrangementFinder(index + towel.Length);
                        possibleArrangements += tails;
                    }
                }
                return possibleArrangements;
            });
            return fnArrangementFinder(0);
        }

        private void ParseTowels(string[] inputData)
        {
            this._availableTowels.Clear();
            this._desiredPatterns.Clear();

            this._availableTowels.AddRange(inputData[0].Split(',').Select(x => x.Trim()));
            foreach(var p in inputData.Skip(2))
            {
                this._desiredPatterns.Add(p);
            }
        }
    }
}
