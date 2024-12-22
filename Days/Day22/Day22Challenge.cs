using AoC2024.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2024.Days.Day22
{
    internal partial class Day22Challenge : AoCChallengeBase
    {
        public override int Day => 22;
        public override string Name => "Monkey Market";

        protected override object ExpectedTestResultPartOne => 37327623L;
        protected override object ExpectedTestResultPartTwo => 23L;
        protected override bool ExtraTestDataPartTwo => true;

        protected override object SolvePartOneInternal(string[] inputData)
        {
            var numbers = this.ParseSecretNumbers(inputData, 2000);
            var result = 0L;
            for (var i = 0; i < inputData.Length; i++)
            {
                result += numbers[i, 2000];
            }
            return result;
        }

        protected override object SolvePartTwoInternal(string[] inputData)
        {
            var numbers = this.ParseSecretNumbers(inputData, 2000);
            var n = numbers.GetLength(0);
            var i = numbers.GetLength(1);

            // derive prices
            var prices = new int[n, i];
            for (var k = 0; k < n; k++)
            {
                for (var j = 0; j < i; j++)
                {
                    prices[k, j] = (int)(numbers[k, j] % 10);
                }
            }

            // derive price changes
            var priceChanges = new int[n, i];
            var changeSequences = new Dictionary<(int s1, int s2, int s3, int s4), int[]>();
            for (var k = 0; k < n; k++)
            {
                for (var j = 1; j < i; j++)
                {
                    priceChanges[k, j] = prices[k, j] - prices[k, j - 1];
                    if(j > 3)
                    {
                        var seq = (priceChanges[k, j - 3], priceChanges[k, j - 2], priceChanges[k, j - 1], priceChanges[k, j]);
                        if (!changeSequences.ContainsKey(seq))
                            changeSequences[seq] = new int[n];
                        if(changeSequences[seq][k] == 0)
                        {
                            changeSequences[seq][k] = prices[k, j];
                        }
                    }
                }
            }

            // derive max profit
            var maxProfit = 0L;
            foreach (var seq in changeSequences)
            {
                var profit = seq.Value.Sum();
                if (profit > maxProfit)
                    maxProfit = profit;
            }

            return maxProfit;
        }

        private long[,] ParseSecretNumbers(string[] inputData, int iterations)
        {
            var startNumbers = inputData.Select(long.Parse).ToList();
            var numbers = new long[inputData.Length, iterations + 1];
            for (var i = 0; i <= iterations; i++)
            {
                for(var k = 0; k < startNumbers.Count; k++)
                {
                    if (i == 0)
                        numbers[k, i] = startNumbers[k];
                    else
                        numbers[k, i] = this.CalcNextSecretNumber(numbers[k, i - 1]);
                }
            }
            return numbers;
        }

        private long CalcNextSecretNumber(long n)
        {
            n = (n ^ (n * 64)) % 16_777_216;
            n = (n ^ (n / 32)) % 16_777_216;
            n = (n ^ (n * 2048)) % 16_777_216;
            return n;
        }
    }
}
