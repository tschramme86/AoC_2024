using AoC2024.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2024.Days.Day02
{
    internal class Day02Challenge : AoCChallengeBase
    {
        public override int Day => 2;
        public override string Name => "Red-Nosed Reports";

        protected override object ExpectedTestResultPartOne => 2;
        protected override object ExpectedTestResultPartTwo => 4;

        protected override object SolvePartOneInternal(string[] inputData)
        {
            var reports = ParseReports(inputData);
            var validCount = 0;
            foreach (var report in reports)
            {
                if (IsSafeReport(report)) validCount++;
            }
            return validCount;
        }

        protected override object SolvePartTwoInternal(string[] inputData)
        {
            var reports = ParseReports(inputData);
            var validCount = 0;
            foreach (var report in reports)
            {
                if (IsSafeReport(report)) validCount++;
                else
                {
                    for(int levelToRemove = 0; levelToRemove < report.Length; levelToRemove++)
                    {
                        var newReport = new List<int>(report);
                        newReport.RemoveAt(levelToRemove);
                        if (IsSafeReport(newReport.ToArray()))
                        {
                            validCount++;
                            break;
                        }
                    }
                }
            }
            return validCount;
        }

        private List<int[]> ParseReports(string[] inputData)
        {
            var ret = new List<int[]>();
            foreach (var line in inputData)
            {
                ret.Add(line.Split(' ').Select(int.Parse).ToArray());
            }
            return ret;
        }

        private bool IsSafeReport(int[] report)
        {
            var dir = Math.Sign(report[1] - report[0]);
            for (var i = 0; i < report.Length - 1; i++)
            {
                var d = Math.Abs(report[i] - report[i + 1]);
                if (Math.Sign(report[i + 1] - report[i]) != dir || !d.IsBetween(1, 3))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
