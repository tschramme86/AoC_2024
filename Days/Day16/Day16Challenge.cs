using Google.OrTools.LinearSolver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AoC2024.Days.Day16
{
    internal partial class Day16Challenge : AoCChallengeBase
    {
        enum Turn
        {
            CW,
            CCW
        }
        [DebuggerDisplay("MazeStep: {Position} {Direction}")]
        abstract class MazeStep
        {
            public MazeStep? Predecessor { get; set; }
            public (int x, int y) Position { get; set; }
            public Direction Direction { get; set; }

            public abstract int Price { get; }

            private int? _totalScoreCache;
            public int TotalScore
            {
                get
                {
                    if (this._totalScoreCache.HasValue) return this._totalScoreCache.Value;
                    return (this._totalScoreCache = (this.Predecessor?.TotalScore ?? 0) + this.Price).Value;
                }
            }

            public IEnumerable<(int x, int y)> GetPathPositions()
            {
                var current = this;
                while (current != null)
                {
                    yield return current.Position;
                    current = current.Predecessor;
                }
            }
        }
        class MazeStart : MazeStep
        {
            public override int Price => 0;
        }
        [DebuggerDisplay("MazeStep (M): {Position} {Direction}")]
        class MazeStepMove : MazeStep
        {
            public override int Price => 1;
        }
        [DebuggerDisplay("MazeStep (T): {Position} {Direction}")]
        class MazeStepTurn : MazeStep
        {
            public override int Price => 1000;
            public Turn Turn { get; set; }
        }

        public override int Day => 16;
        public override string Name => "Reindeer Maze";

        protected override object ExpectedTestResultPartOne => 7036;
        protected override object ExpectedTestResultPartTwo => 45;

        private readonly Dictionary<Direction, (int x, int y)> _moveDirections = new()
        {
            { Direction.North, (0, -1) },
            { Direction.East, (1, 0) },
            { Direction.South, (0, 1) },
            { Direction.West, (-1, 0) }
        };

        private bool[,] _map = new bool[0, 0];
        private (int x, int y) _startPosition = (0, 0);
        private (int x, int y) _endPosition = (0, 0);

        protected override object SolvePartOneInternal(string[] inputData)
        {
            this.ReadMaze(inputData);
            var bestPathsEnd = this.FindBestPath();
            return bestPathsEnd.FirstOrDefault()?.TotalScore ?? 0;
        }

        protected override object SolvePartTwoInternal(string[] inputData)
        {
            this.ReadMaze(inputData);
            var bestPathsEnd = this.FindBestPath();

            var goodSeats = new HashSet<(int x, int y)>();
            foreach (var bestPath in bestPathsEnd)
            {
                foreach (var pathPosition in bestPath.GetPathPositions())
                {
                    goodSeats.Add(pathPosition);
                }
            }
            return goodSeats.Count;
        }

        private List<MazeStep> FindBestPath()
        {
            var bestPaths = new Dictionary<(int x, int y, Direction d), List<MazeStep>>();

            var startStep = new MazeStart
            {
                Position = this._startPosition,
                Direction = Direction.East,
            };
            bestPaths[(this._startPosition.x, this._startPosition.y, startStep.Direction)] = [startStep];
            var listOfEnds = new List<MazeStep>(128) { startStep };
            var newListOfEnds = new List<MazeStep>(128);
            var maxC = 0;
            do
            {
                // calculate next steps
                newListOfEnds.Clear();
                foreach (var end in listOfEnds)
                {
                    var moveTo = this.AddP(end.Position, this._moveDirections[end.Direction]);
                    if (!this._map[moveTo.x, moveTo.y])
                    {
                        newListOfEnds.Add(new MazeStepMove
                        {
                            Position = moveTo,
                            Direction = end.Direction,
                            Predecessor = end,
                        });
                    }
                    if (end is not MazeStepTurn)
                    {
                        // test CW turn
                        {
                            var newDirection = (Direction)(((int)end.Direction + 1) % 4);
                            var potentialNewPosition = this.AddP(end.Position, this._moveDirections[newDirection]);
                            if (!this._map[potentialNewPosition.x, potentialNewPosition.y])
                            {
                                newListOfEnds.Add(new MazeStepTurn
                                {
                                    Position = end.Position,
                                    Direction = newDirection,
                                    Predecessor = end,
                                    Turn = Turn.CW,
                                });
                            }
                        }
                        // test CCW turn
                        {
                            var newDirection = (Direction)(((int)end.Direction + 3) % 4);
                            var potentialNewPosition = this.AddP(end.Position, this._moveDirections[newDirection]);
                            if (!this._map[potentialNewPosition.x, potentialNewPosition.y])
                            {
                                newListOfEnds.Add(new MazeStepTurn
                                {
                                    Position = end.Position,
                                    Direction = newDirection,
                                    Predecessor = end,
                                    Turn = Turn.CCW,
                                });
                            }
                        }
                    }
                }
                maxC = Math.Max(maxC, newListOfEnds.Count);
                // check whether one next steps leads to a better path to a position
                listOfEnds.Clear();
                foreach (var newEnd in newListOfEnds)
                {
                    var idx = (newEnd.Position.x, newEnd.Position.y, newEnd.Direction);
                    if(!bestPaths.TryGetValue(idx, out var bestPathsForPos))
                        bestPaths[idx] = bestPathsForPos = new List<MazeStep>(8);

                    var oldScore = bestPathsForPos.FirstOrDefault()?.TotalScore ?? int.MaxValue;
                    var newScore = newEnd.TotalScore;

                    if(oldScore < newScore)
                        continue;
                    if(oldScore > newScore)
                        bestPathsForPos.Clear();
                    bestPathsForPos.Add(newEnd);

                    listOfEnds.Add(newEnd);
                }
            } while (listOfEnds.Count > 0);

            // return best paths
            var allBestPaths = new List<MazeStep>(32);

            if (bestPaths.TryGetValue((this._endPosition.x, this._endPosition.y, Direction.East), out var bestPathsEast))
                allBestPaths.AddRange(bestPathsEast);
            if (bestPaths.TryGetValue((this._endPosition.x, this._endPosition.y, Direction.North), out var bestPathsNorth))
                allBestPaths.AddRange(bestPathsNorth);
            if (bestPaths.TryGetValue((this._endPosition.x, this._endPosition.y, Direction.West), out var bestPathsWest))
                allBestPaths.AddRange(bestPathsWest);
            if (bestPaths.TryGetValue((this._endPosition.x, this._endPosition.y, Direction.South), out var bestPathsSouth))
                allBestPaths.AddRange(bestPathsSouth);
            if(allBestPaths.Count == 0)
                return [];

            var minScore = allBestPaths.Min(p => p.TotalScore);
            return allBestPaths.Where(p => p.TotalScore == minScore).ToList();
        }

        private void ReadMaze(string[] inputData)
        {
            this._map = this.MapInput(inputData, (c, pos) =>
            {
                if (c == 'S') this._startPosition = pos;
                if (c == 'E') this._endPosition = pos;
                return c == '#';
            });
        }

        private void PrintMaze(IEnumerable<(int x, int y)> markedLocations, IEnumerable<(int x, int y)>? currentEnds = null)
        {
            var isMarked = new HashSet<(int x, int y)>(markedLocations);
            var isCurrentEnd = new HashSet<(int x, int y)>(currentEnds ?? []);
            for (int y = 0; y < this._map.GetLength(1); y++)
            {
                for (int x = 0; x < this._map.GetLength(0); x++)
                {
                    if (isCurrentEnd.Contains((x, y)))
                    {
                        Console.Write('X');
                    }
                    else if (isMarked.Contains((x, y)))
                    {
                        Console.Write('O');
                    }
                    else if (this._startPosition == (x, y))
                    {
                        Console.Write('S');
                    }
                    else if (this._endPosition == (x, y))
                    {
                        Console.Write('E');
                    }
                    else
                    {
                        Console.Write(this._map[x, y] ? '#' : '.');
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
