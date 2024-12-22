using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2024.Days.Day11
{
    internal class Day11Challenge : AoCChallengeBase
    {
        class Stone(long number)
        {
            public long Number { get; set; } = number;

            public Stone[] Blink()
            {
                if (this.Number == 0)
                    return [new Stone(1)];

                var l = this.Number.ToString().Length;
                if (l % 2 == 0)
                    return [
                        new Stone(long.Parse(this.Number.ToString()[..(l/2)])),
                        new Stone(long.Parse(this.Number.ToString()[(l/2)..])),
                        ];
                return [new Stone(this.Number * 2024)];
            }

            public override string ToString()
            {
                return this.Number.ToString();
            }
        }

        public override int Day => 11;
        public override string Name => "Plutonian Pebbles";

        protected override object ExpectedTestResultPartOne => 55312L;
        protected override object ExpectedTestResultPartTwo => 0;

        protected override object SolvePartOneInternal(string[] inputData)
        {
            return this.CalcResultingStoneCount(inputData[0], 25);
        }

        protected override object SolvePartTwoInternal(string[] inputData)
        {
            if(this.IsOnTestData) return 0;
            return this.CalcResultingStoneCount(inputData[0], 75);
        }

        private long CalcResultingStoneCount(string stoneDefinition, int blinks)
        {
            var stones = stoneDefinition.Split(' ').Select(n => new Stone(long.Parse(n))).ToList();
            var stoneCache = new Dictionary<(long stoneNumber, int remainingRounds), long>();

            long CalcResultingStones(Stone stone, int remainingRounds)
            {
                if (stoneCache.TryGetValue((stone.Number, remainingRounds), out var cachedResult))
                    return cachedResult;

                if (remainingRounds == 0)
                    return 1;

                var newStones = stone.Blink();
                var result = newStones.Sum(s => CalcResultingStones(s, remainingRounds - 1));
                stoneCache[(stone.Number, remainingRounds)] = result;
                return result;
            }

            return stones.Sum(s => CalcResultingStones(s, blinks));
        }
    }
}
