using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AoC2024.Days.Day07
{
    internal class Day07Challenge : AoCChallengeBase
    {
        public override int Day => 7;
        public override string Name => "Bridge Repair";

        protected override object ExpectedTestResultPartOne => 3749L;
        protected override object ExpectedTestResultPartTwo => 11387L;

        protected override object SolvePartOneInternal(string[] inputData)
        {
            var equations = this.ParseInput(inputData);
            var sum = 0L;
            foreach(var e in equations) {
                if(EvaluateLine(e.values[0], e.values, 1, e.testValue))
                    sum += e.testValue;

            }
            return sum;
        }

        protected override object SolvePartTwoInternal(string[] inputData)
        {
            var equations = this.ParseInput(inputData);
            var sum = 0L;
            foreach(var e in equations) {
                if(EvaluateLine3Op(e.values[0], e.values, 1, e.testValue))
                    sum += e.testValue;

            }
            return sum;        
        }

        private bool EvaluateLine(long currentValue, long[] numbers, int idx, long testValue) {
            if(idx == numbers.Length)
                return testValue == currentValue;
            if(currentValue > testValue) return false;

            return 
                EvaluateLine(currentValue + numbers[idx], numbers, idx + 1, testValue) ||
                EvaluateLine(currentValue * numbers[idx], numbers, idx + 1, testValue);
        }

        private bool EvaluateLine3Op(long currentValue, long[] numbers, int idx, long testValue) {
            if(idx == numbers.Length)
                return testValue == currentValue;
            if(currentValue > testValue) return false;

            return 
                EvaluateLine3Op(currentValue + numbers[idx], numbers, idx + 1, testValue) ||
                EvaluateLine3Op(currentValue * numbers[idx], numbers, idx + 1, testValue) ||
                EvaluateLine3Op(long.Parse($"{currentValue}{numbers[idx]}"), numbers, idx + 1, testValue);
        }

        private List<(long testValue, long[] values)> ParseInput(string[] inputData) {
            var ret = new List<(long testValue, long[] values)>();
            foreach(var line in inputData) {
                var numbers = line.Split(' ').Select(x => long.Parse(x.Replace(":", ""))).ToList();
                ret.Add((numbers[0], numbers[1..].ToArray()));
            }
            return ret;
        }
    }
}
