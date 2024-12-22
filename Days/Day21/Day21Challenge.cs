using AoC2024.Helper;
using Google.OrTools.LinearSolver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AoC2024.Days.Day21
{
    internal partial class Day21Challenge : AoCChallengeBase
    {
        class BfsItem
        {
            public (int x, int y) Position { get; set; }
            public BfsItem? Predecessor { get; set; }
            public int Steps { get; set; }
            public Direction? MoveToHere { get; set; }
        }
        abstract class BacktrackKeypad
        {
            protected abstract char[] Keys { get; }
            protected abstract Dictionary<char, (int x, int y)> KeyPositions { get; }
            public Dictionary<(char, char), List<Direction>> WayFromTo { get; } = [];
            public Dictionary<(char, char), string> SequenceFromTo { get; } = [];

            protected abstract int KeypadWidth { get; }
            protected abstract int KeypadHeight { get; }

            public void BuildMap()
            {
                this.WayFromTo.Clear();
                var mapPositionToKey = this.KeyPositions.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

                foreach (var startKey in this.Keys)
                {
                    var distMap = new List<BfsItem>[this.KeypadWidth, this.KeypadHeight];
                    for (var x = 0; x < this.KeypadWidth; x++)
                    {
                        for (var y = 0; y < this.KeypadHeight; y++)
                        {
                            distMap[x, y] = [];
                        }
                    }

                    var startItem = new BfsItem { Position = this.KeyPositions[startKey], Steps = 0 };
                    distMap[startItem.Position.x, startItem.Position.y].Add(startItem);

                    var currentSteps = new List<BfsItem> { startItem };
                    var nextSteps = new List<BfsItem>();
                    var round = 0;
                    do
                    {
                        round++;
                        nextSteps.Clear();
                        foreach (var s in currentSteps)
                        {
                            foreach (var dir in Math2D.MoveDirectionsMap)
                            {
                                var nextPos = Math2D.Add(s.Position, dir.Value);
                                if (mapPositionToKey.ContainsKey(nextPos))
                                {
                                    if (distMap[nextPos.x, nextPos.y].Any(i => i.Steps < round))
                                        continue;

                                    var nextItem = new BfsItem { Position = nextPos, Steps = round, Predecessor = s, MoveToHere = dir.Key };
                                    distMap[nextPos.x, nextPos.y].Add(nextItem);
                                    nextSteps.Add(nextItem);
                                }
                            }
                        }
                        currentSteps.Clear();
                        currentSteps.AddRange(nextSteps);
                    } while (currentSteps.Count > 0);

                    // copy distance map to list
                    foreach (var targetKey in this.Keys)
                    {
                        var possibleWays = new List<List<Direction>>();
                        foreach (var end in distMap[this.KeyPositions[targetKey].x, this.KeyPositions[targetKey].y])
                        {
                            var way = new List<Direction>();
                            var current = end;
                            while (current.Predecessor != null)
                            {
                                if (current.MoveToHere.HasValue)
                                    way.Add(current.MoveToHere.Value);
                                current = current.Predecessor;
                            }
                            way.Reverse();
                            possibleWays.Add(way);
                        }

                        var isGoingLeft = this.KeyPositions[startKey].x > this.KeyPositions[targetKey].x;
                        var wayComplexity = possibleWays.ToLookup(_dChanges);
                        var minWayComplexity = wayComplexity.Min(g => g.Key);
                            
                            
                        var bestSequence = this.ReducePossibleSequences(wayComplexity[minWayComplexity].ToList(), isGoingLeft);
                        this.WayFromTo[(startKey, targetKey)] = bestSequence;

                        var sb = new StringBuilder();
                        foreach (var d in bestSequence)
                        {
                            sb.Append(d switch
                            {
                                Direction.North => '^',
                                Direction.East => '>',
                                Direction.South => 'v',
                                Direction.West => '<',
                                _ => throw new ArgumentOutOfRangeException()
                            });
                        }
                        sb.Append('A');
                        this.SequenceFromTo[(startKey, targetKey)] = sb.ToString();
                    }
                }

                int _dChanges(List<Direction> path)
                {
                    var result = 0;
                    for (var i = 1; i < path.Count; i++)
                    {
                        if (path[i] != path[i - 1])
                            result++;
                    }
                    return result;
                }
            }

            private List<Direction> ReducePossibleSequences(List<List<Direction>> sequences, bool preferLeRiFirst)
            {
                if(sequences.Count == 1)
                    return sequences[0];

                return sequences.OrderBy(s =>
                {
                    if(preferLeRiFirst)
                        return s[0] == Direction.West || s[0] == Direction.East ? 0 : 1;
                    return s[0] == Direction.North || s[0] == Direction.South ? 0 : 1;
                }).First();
            }
        }
        class BacktrackDoorKeypad : BacktrackKeypad
        {
            protected override char[] Keys => ['A', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'];
            protected override Dictionary<char, (int x, int y)> KeyPositions => new()
            {
                ['A'] = (2, 3),
                ['0'] = (1, 3),
                ['1'] = (0, 2),
                ['2'] = (1, 2),
                ['3'] = (2, 2),
                ['4'] = (0, 1),
                ['5'] = (1, 1),
                ['6'] = (2, 1),
                ['7'] = (0, 0),
                ['8'] = (1, 0),
                ['9'] = (2, 0)
            };
            protected override int KeypadWidth => 3;
            protected override int KeypadHeight => 4;
        }
        class BacktrackDirectionalKeypad : BacktrackKeypad
        {
            protected override char[] Keys => ['A', '^', 'v', '<', '>'];
            protected override Dictionary<char, (int x, int y)> KeyPositions => new()
            {
                ['A'] = (2, 0),
                ['^'] = (1, 0),
                ['<'] = (0, 1),
                ['v'] = (1, 1),
                ['>'] = (2, 1)
            };
            protected override int KeypadWidth => 3;
            protected override int KeypadHeight => 2;
        }

        public override int Day => 21;
        public override string Name => "Keypad Conundrum";

        protected override object ExpectedTestResultPartOne => 126384L;
        protected override object ExpectedTestResultPartTwo => 154115708116294L;

        private readonly Dictionary<Direction, char> _dToC = new()
        {
            { Direction.North, '^' },
            { Direction.East, '>' },
            { Direction.South, 'v' },
            { Direction.West, '<' }
        };

        protected override object SolvePartOneInternal(string[] inputData)
        {
            return this.Solve(inputData, 2);
        }

        protected override object SolvePartTwoInternal(string[] inputData)
        {
            return this.Solve(inputData, 25);
        }

        private long Solve(string[] inputData, int involvedDirectionalKeypadRobots)
        {
            var result = 0L;
            foreach (var code in inputData)
            {
                var moves = CalculateCodeMoves(code, involvedDirectionalKeypadRobots);
                var num = int.Parse(code[..^1]);
                Console.WriteLine($"{code}: {moves} * {num}");
                result += (moves * num);
            }
            return result;
        }

        private long CalculateCodeMoves(string code, int involvedDirectionalKeypadRobots)
        {
            var doorKeypad = new BacktrackDoorKeypad();
            doorKeypad.BuildMap();

            var directionalKeypad = new BacktrackDirectionalKeypad();
            directionalKeypad.BuildMap();

            this._sequenceLengthCache.Clear();
            var sequence = this.BacktrackCode(code, doorKeypad);
            return this.CalculateSequenceLength(sequence, involvedDirectionalKeypadRobots, directionalKeypad);
        }

        private readonly Dictionary<(char s, char e, int level), long> _sequenceLengthCache = [];
        private long CalculateSequenceLength(string code, int it, BacktrackKeypad keypad)
        {
            if(it == 0)
                return code.Length;

            var totalLength = 0L;
            var prevKey = 'A';
            foreach (var nextKey in code)
            {
                if (!this._sequenceLengthCache.TryGetValue((prevKey, nextKey, it), out var length))
                {
                    var expandedSequence = keypad.SequenceFromTo[(prevKey, nextKey)];
                    length = this.CalculateSequenceLength(expandedSequence, it - 1, keypad);
                    this._sequenceLengthCache[(prevKey, nextKey, it)] = length;
                }
                totalLength += length;
                prevKey = nextKey;
            }
            return totalLength;
        }

        private string BacktrackCode(string code, BacktrackKeypad keypad, char startKey = 'A')
        {
            var sb = new StringBuilder();
            for (var i = 0; i < code.Length; i++)
            {
                foreach (var d in keypad.WayFromTo[(startKey, code[i])])
                {
                    sb.Append(_dToC[d]);
                }
                sb.Append('A');
                startKey = code[i];
            }
            return sb.ToString();
        }
    }
}
