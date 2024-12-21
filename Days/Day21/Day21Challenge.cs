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
            public Dictionary<(char, char), List<List<Direction>>> PossibleWays { get; } = [];

            protected abstract int KeypadWidth { get; }
            protected abstract int KeypadHeight { get; }

            public void BuildMap()
            {
                this.PossibleWays.Clear();
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
                        this.PossibleWays[(startKey, targetKey)] = possibleWays;
                    }
                }
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

            public void BacktrackCode(string code)
            {
            }
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

            public void BacktrackCode(string code)
            {
            }
        }

        public override int Day => 21;
        public override string Name => "Keypad Conundrum";

        protected override object ExpectedTestResultPartOne => 126384;
        protected override object ExpectedTestResultPartTwo => 285;

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

        private int Solve(string[] inputData, int involvedDirectionalKeypadRobots)
        {
            var result = 0;
            foreach (var code in inputData)
            {
                var moves = CalculateCodeMoves(code, involvedDirectionalKeypadRobots);
                var num = int.Parse(code[..^1]);
                Console.WriteLine($"{code}: {moves} * {num}");
                result += (moves * num);
            }
            return result;
        }

        private int CalculateCodeMoves(string code, int involvedDirectionalKeypadRobots)
        {
            var doorKeypad = new BacktrackDoorKeypad();
            doorKeypad.BuildMap();

            var directionalKeypad = new BacktrackDirectionalKeypad();
            directionalKeypad.BuildMap();

            var minCodeLength = int.MaxValue;
            var minCode = string.Empty;

            foreach(var s0 in GetPossibleSequences(code, doorKeypad))
            {
                var thisSequenceList = new List<string>(4096) { s0 };
                var nextSequenceList = new List<string>(4096);
                for (var i = 0; i < involvedDirectionalKeypadRobots; i++)
                {
                    Console.WriteLine($" - iteration {i}, sequence list size = {thisSequenceList.Count}");
                    nextSequenceList.Clear();
                    foreach (var s in thisSequenceList)
                    {
                        nextSequenceList.AddRange(GetPossibleSequences(s, directionalKeypad));
                    }
                    (thisSequenceList, nextSequenceList) = (nextSequenceList, thisSequenceList);
                }

                foreach(var finalSequence in thisSequenceList)
                {
                    if (finalSequence.Length < minCodeLength)
                    {
                        minCodeLength = finalSequence.Length;
                        minCode = finalSequence;
                    }
                }
            }

            return minCodeLength;
        }

        private Dictionary<string, List<string>> _cache = [];
        private List<string> GetPossibleSequences(string code, BacktrackKeypad keypad)
        {
            if (this._cache.ContainsKey(code))
                return this._cache[code];

            var result = new List<string>();
            var startKey = 'A';
            var sequences = new List<List<Direction>>[code.Length];
            for(var i=0; i<code.Length; i++)
            {
                sequences[i] = keypad.PossibleWays[(startKey, code[i])];
                startKey = code[i];
            }
            _generateSequences(0, [], result);

            this._cache[code] = result;
            return result;

            void _generateSequences(int index, List<List<Direction>> soFar, List<string> target)
            {
                if (index == code.Length)
                {
                    var sb = new StringBuilder();
                    foreach (var seq in soFar)
                    {
                        foreach (var d in seq)
                        {
                            sb.Append(_dToC[d]);
                        }
                        sb.Append('A');
                    }
                    target.Add(sb.ToString());
                }
                else
                {
                    foreach(var nextSequence in sequences[index])
                    {
                        var newSequences = new List<List<Direction>>(soFar);
                        newSequences.Add(nextSequence);
                        _generateSequences(index + 1, newSequences, target);
                    }
                }
            }
        }
    }
}
