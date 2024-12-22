using Google.OrTools.LinearSolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AoC2024.Days.Day13
{
    internal partial class Day13Challenge : AoCChallengeBase
    {
        class Game
        {
            public (long x, long y) ButtonA { get; set; }
            public (long x, long y) ButtonB { get; set; }
            public (long x, long y) Price { get; set; }
        }

        public override int Day => 13;
        public override string Name => "Claw Contraption";

        protected override object ExpectedTestResultPartOne => 480L;
        protected override object ExpectedTestResultPartTwo => 875318608908L;


        protected override object SolvePartOneInternal(string[] inputData)
        {
            var games = this.ReadGames(inputData);
            var result = 0L;
            foreach (var game in games)
            {
                var gameResult = this.SolveGame(game, false);
                result += gameResult.GetValueOrDefault();
            }
            return result;
        }

        protected override object SolvePartTwoInternal(string[] inputData)
        {
            var games = this.ReadGames(inputData);
            var result = 0L;
            foreach (var game in games)
            {
                var gameResult = this.SolveGame(game, true);
                result += gameResult.GetValueOrDefault();
            }
            return result;
        }

        private long? SolveGame(Game game, bool isLarge)
        {
            var priceX = game.Price.x;
            var priceY = game.Price.y;
            if(isLarge)
            {
                priceX += 10000000000000;
                priceY += 10000000000000;
            }

            var solver = Solver.CreateSolver("SCIP");
            var apress = solver.MakeIntVar(0.0, isLarge ? double.PositiveInfinity : 100.0, "ap");
            var bpress = solver.MakeIntVar(0.0, isLarge ? double.PositiveInfinity : 100.0, "bp");
            solver.Add(apress * game.ButtonA.x + bpress * game.ButtonB.x == priceX);
            solver.Add(apress * game.ButtonA.y + bpress * game.ButtonB.y == priceY);
            solver.Minimize(apress * 3 + bpress);
            var resultStatus = solver.Solve();
            if (resultStatus == Solver.ResultStatus.OPTIMAL)
            {
                return (long)(apress.SolutionValue() * 3 + bpress.SolutionValue());
            }
            return null;
        }

        private List<Game> ReadGames(string[] inputData)
        {
            var games = new List<Game>();
            var game = new Game();
            foreach (var line in inputData)
            {
                if (line.Contains("Prize"))
                {
                    var match = RxPrice().Match(line);
                    game.Price = (int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));

                    games.Add(game);
                    game = new Game();
                }
                else if (line.Contains("Button A"))
                {
                    var match = RxButtonPush().Match(line);
                    game.ButtonA = (int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
                }
                else if (line.Contains("Button B"))
                {
                    var match = RxButtonPush().Match(line);
                    game.ButtonB = (int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
                }
            }
            return games;
        }

        [GeneratedRegex(@"X=(\d+), Y=(\d+)")]
        private static partial Regex RxPrice();
        [GeneratedRegex(@"X\+(\d+), Y\+(\d+)")]
        private static partial Regex RxButtonPush();
    }
}
