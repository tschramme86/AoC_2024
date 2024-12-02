using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2024.Days.Day01
{
    internal class Day01Challenge : AoCChallengeBase
    {
        public override int Day => 1;
        public override string Name => "Historian Hysteria";

        protected override object ExpectedTestResultPartOne => 11;
        protected override object ExpectedTestResultPartTwo => 31L;

        protected override object SolvePartOneInternal(string[] inputData)
        {
            var sum = 0;

            var (l1, l2) = ParseInputDataToLists(inputData);
            Debug.Assert(l1.Count == l2.Count);
            l1.Sort();
            l2.Sort();

            for(var i=0; i < l1.Count; i++)
            {
                sum += Math.Abs(l1[i] - l2[i]);
            }

            return sum;
        }

        protected override object SolvePartTwoInternal(string[] inputData)
        {
            var sum = 0L;

            var (l1, l2) = ParseInputDataToLists(inputData);
            var l2Count = l2.ToLookup(x => x);
            Debug.Assert(l1.Count == l2.Count);

            for (var i = 0; i < l1.Count; i++)
            {
                if(l2.Contains(l1[i]))
                {
                    sum += l1[i] * l2Count[l1[i]].LongCount();
                }
            }

            return sum;
        }

        private (List<int> l1, List<int> l2) ParseInputDataToLists(string[] inputData)
        {
            var l1 = new List<int>();
            var l2 = new List<int>();
            foreach (var line in inputData)
            {
                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                l1.Add(int.Parse(parts[0]));
                l2.Add(int.Parse(parts[1]));
            }
            return (l1, l2);
        }
    }
}
