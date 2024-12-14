using Google.OrTools.LinearSolver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AoC2024.Days.Day14
{
    internal partial class Day14Challenge : AoCChallengeBase
    {
        [DebuggerDisplay("p: ({Position.x}, {Position.y}), d: ({Direction.x}, {Direction.y})")]
        class Robot
        {
            public (int x, int y) Position { get; set; }
            public (int x, int y) Direction { get; set; }
        }
        
        public override int Day => 14;
        public override string Name => "Restroom Redoubt";

        protected override object ExpectedTestResultPartOne => 12L;
        protected override object ExpectedTestResultPartTwo => 0;

        private readonly List<Robot> _robots = [];
        private int _w;
        private int _h;

        protected override object SolvePartOneInternal(string[] inputData)
        {
            this.ReadRobots(inputData);

            for(var i=0; i<100;i++)
            {
                foreach(var r in this._robots)
                {
                    r.Position = this.Wrap(this.AddP(r.Position, r.Direction));
                }
            }

            var midx = (this._w - 1) / 2;
            var midy = (this._h - 1) / 2;
            var q1Robots = (long)this._robots.Count(r => r.Position.x < midx && r.Position.y < midy);
            var q2Robots = this._robots.Count(r => r.Position.x > midx && r.Position.y < midy);
            var q3Robots = this._robots.Count(r => r.Position.x < midx && r.Position.y > midy);
            var q4Robots = this._robots.Count(r => r.Position.x > midx && r.Position.y > midy);

            return q1Robots * q2Robots * q3Robots * q4Robots;
        }

        protected override object SolvePartTwoInternal(string[] inputData)
        {
            if (this.IsOnTestData) return 0;

            this.ReadRobots(inputData);

            var allVx = new List<double>();
            var allVy = new List<double>();
            for (var i = 0; i < 1000000; i++)
            {
                foreach (var r in this._robots)
                {
                    r.Position = this.Wrap(this.AddP(r.Position, r.Direction));
                }
                
                var vx = this._robots.Select(r => (double)r.Position.x).Variance();
                var vy = this._robots.Select(r => (double)r.Position.y).Variance();

                allVx.Add(vx);
                allVy.Add(vy);
                var avgVx = allVx.Average();
                var avgVy = allVy.Average();
                Console.Write($"\rRound {i + 1}, vx={vx:0.000}, vy={vy:0.000} (Avg vx={avgVx:0.000}, vy={avgVy:0.000})");

                // tree is there when variance is much lower than usual
                if (vx < 0.5 * avgVx && vy < 0.5 * avgVy)
                {
                    Console.WriteLine();
                    Console.WriteLine($"After second {i + 1}:");
                    var grid = new bool[this._w, this._h];
                    foreach (var r in this._robots)
                    {
                        grid[r.Position.x, r.Position.y] = true;
                    }
                    for (var y = 0; y < this._h; y++)
                    {
                        for (var x = 0; x < this._w; x++)
                        {
                            Console.Write(grid[x, y] ? "#" : ".");
                        }
                        Console.WriteLine();
                    }

                    return i + 1;
                }
            }

            return 0;
        }

        private (int x, int y) Wrap((int x, int y) p)
        {
            return ((p.x + this._w) % this._w, (p.y + this._h) % this._h);
        }

        private void ReadRobots(string[] inputData)
        {
            this._robots.Clear();
            foreach (var line in inputData)
            {
                var match = RxRobotDefinition().Match(line);
                if (!match.Success)
                {
                    throw new Exception($"Invalid robot definition: {line}");
                }
                var robot = new Robot
                {
                    Position = (int.Parse(match.Groups["px"].Value), int.Parse(match.Groups["py"].Value)),
                    Direction = (int.Parse(match.Groups["vx"].Value), int.Parse(match.Groups["vy"].Value))
                };
                this._robots.Add(robot);
            }

            if(this.IsOnTestData)
            {
                this._w = 11;
                this._h = 7;
            }
            else
            {
                this._w = 101;
                this._h = 103;
            }
        }

        [GeneratedRegex(@"p=(?<px>\d+),(?<py>\d+) v=(?<vx>-?\d+),(?<vy>-?\d+)")]
        private static partial Regex RxRobotDefinition();
    }
}
