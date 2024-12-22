using AoC2024.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2024.Days.Day05
{
    internal class Day05Challenge : AoCChallengeBase
    {
        public override int Day => 5;
        public override string Name => "Print Queue";

        protected override object ExpectedTestResultPartOne => 143;
        protected override object ExpectedTestResultPartTwo => 123;

        protected override object SolvePartOneInternal(string[] inputData)
        {
            var (orderRules, printJobs) = ParseInput(inputData);
            var (correctJobs, incorrectJobs) = CategorizePrintJobs(orderRules, printJobs);
            return correctJobs.Sum(job => job[(job.Count - 1) / 2]);
        }

        protected override object SolvePartTwoInternal(string[] inputData)
        {
            var (orderRules, printJobs) = ParseInput(inputData);
            var (correctJobs, incorrectJobs) = CategorizePrintJobs(orderRules, printJobs);
            var hashedRules = new HashSet<(int first, int second)>(orderRules);
            var midPageSum = 0;
            foreach (var job in incorrectJobs)
            {
                job.Sort(new Comparison<int>((a, b) =>
                {
                    if(hashedRules.Contains((b, a))) return -1;
                    return 1;
                }));

                midPageSum += job[(job.Count - 1) / 2];
            }
            return midPageSum;
        }

        private (List<List<int>> correctJobs, List<List<int>> incorrectJobs) CategorizePrintJobs(List<(int first, int second)> orderRules, List<List<int>> printJobs)
        {
            var correctJobs = new List<List<int>>();
            var incorrectJobs = new List<List<int>>();

            var rulesDict = orderRules.ToLookup(x => x.first);
            foreach (var job in printJobs)
            {
                var isValid = true;
                for (var i = 1; i < job.Count; i++)
                {
                    if (rulesDict.Contains(job[i]))
                    {
                        foreach (var rule in rulesDict[job[i]])
                        {
                            if (job.IndexOf(rule.second).IsBetween(0, i - 1))
                            {
                                isValid = false;
                                break;
                            }
                        }
                    }
                    if (!isValid) break;
                }
                if (isValid)
                    correctJobs.Add(job);
                else
                    incorrectJobs.Add(job);
            }
            return (correctJobs, incorrectJobs);
        }

        private (List<(int first, int second)> orderRules, List<List<int>> printJobs) ParseInput(string[] inputData)
        {
            var orderRules = new List<(int first, int second)>();
            var printJobs = new List<List<int>>();
            var parsingOrderRules = true;
            foreach (var line in inputData)
            {
                if (string.IsNullOrEmpty(line))
                {
                    parsingOrderRules = false;
                    continue;
                }
                if (parsingOrderRules)
                {
                    var parts = line.Split('|');
                    orderRules.Add((int.Parse(parts[0]), int.Parse(parts[1])));
                }
                else
                {
                    printJobs.Add([..line.Split(',').Select(int.Parse)]);
                }
            }
            return (orderRules, printJobs);
        }
    }
}
