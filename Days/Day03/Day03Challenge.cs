using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AoC2024.Days.Day03
{
    internal class Day03Challenge : AoCChallengeBase
    {
        private Regex rxMul = new(@"mul\((?<d1>\d{1,3}),(?<d2>\d{1,3})\)", RegexOptions.Compiled);
        private Regex rxMulDoDont = new(@"(?<mx>mul\((?<d1>\d{1,3}),(?<d2>\d{1,3})\))|(?<dnx>don't\(\))|(?<dx>do\(\))", RegexOptions.Compiled);
        public override int Day => 3;
        public override string Name => "Mull It Over";

        protected override object ExpectedTestResultPartOne => 161L;
        protected override object ExpectedTestResultPartTwo => 48L;
        protected override bool ExtraTestDataPartTwo => true;

        protected override object SolvePartOneInternal(string[] inputData)
        {
            var l = string.Join("", inputData);
            var sum = 0L;
            var m = rxMul.Match(l);
            while(m.Success) {
                var d1 = int.Parse(m.Groups["d1"].Value);
                var d2 = int.Parse(m.Groups["d2"].Value);
                sum += d1 * d2;
                m = m.NextMatch();
            }
            return sum;
        }

        protected override object SolvePartTwoInternal(string[] inputData)
        {
            var l = string.Join("", inputData);
            var sum = 0L;
            var m = rxMulDoDont.Match(l);
            var isEnabled = true;
            while(m.Success) {
                if(m.Groups["dx"].Success) {
                    isEnabled = true;
                } else if(m.Groups["dnx"].Success) {
                    isEnabled = false;
                } else if(isEnabled) {
                        var d1 = int.Parse(m.Groups["d1"].Value);
                        var d2 = int.Parse(m.Groups["d2"].Value);
                        sum += d1 * d2;
                }
                m = m.NextMatch();
            }
            return sum;
        }
    }
}
