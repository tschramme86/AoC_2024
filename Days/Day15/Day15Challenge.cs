using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2024.Days.Day15
{
    internal partial class Day15Challenge : AoCChallengeBase
    {
        enum MapObject
        {
            Wall,
            Box,
            Fish,
            Empty,
            BoxLeft,
            BoxRight
        }
        public override int Day => 15;
        public override string Name => "Warehouse Woes";

        protected override object ExpectedTestResultPartOne => 10092;
        protected override object ExpectedTestResultPartTwo => 9021;

        private readonly Dictionary<Direction, (int x, int y)> _moveDirections = new()
        {
            { Direction.North, (0, -1) },
            { Direction.East, (1, 0) },
            { Direction.South, (0, 1) },
            { Direction.West, (-1, 0) }
        };
        private readonly List<(int x, int y)> _moves = [];
        private MapObject[,] _map = new MapObject[0, 0];
        private int _w;
        private int _h;
        private (int x, int y) _fishPos;

        protected override object SolvePartOneInternal(string[] inputData)
        {
            this.ParsePlayground(inputData, false);
            // this.PrintMap();

            foreach (var move in this._moves)
            {
                var newMap = (MapObject[,])this._map.Clone();
                if(this.DoMove(newMap, this._fishPos, move, MapObject.Empty))
                {
                    this._map = newMap;
                    this._fishPos = this.AddP(move, this._fishPos);
                }
            }

            // this.PrintMap();
            return this.CalcMapSum();
        }

        protected override object SolvePartTwoInternal(string[] inputData)
        {
            this.ParsePlayground(inputData, true);
            // this.PrintMap();

            foreach (var move in this._moves)
            {
                var newMap = (MapObject[,])this._map.Clone();
                if (this.DoMove(newMap, this._fishPos, move, MapObject.Empty))
                {
                    this._map = newMap;
                    this._fishPos = this.AddP(move, this._fishPos);
                }
            }

            // this.PrintMap();
            return this.CalcMapSum();
        }

        private bool DoMove(MapObject[,] map, (int x, int y) pos, (int x, int y) move, MapObject replacement)
        {
            var currentObj = map[pos.x, pos.y];
            var newPos = this.AddP(pos, move);
            if (map[newPos.x, newPos.y] == MapObject.Wall)
                return false;

            MapObject nextMoveObj = map[newPos.x, newPos.y];
            (int x, int y)? neighborPos = null;
            if (move.y != 0)
            {
                if (map[newPos.x, newPos.y] == MapObject.BoxLeft) neighborPos = this.AddP(newPos, this._moveDirections[Direction.East]);
                if (map[newPos.x, newPos.y] == MapObject.BoxRight) neighborPos = this.AddP(newPos, this._moveDirections[Direction.West]);
            }

            if (map[newPos.x, newPos.y] != MapObject.Empty)
            {
                if (!this.DoMove(map, newPos, move, nextMoveObj))
                    return false;
            }

            if (neighborPos.HasValue)
            {
                if (!this.DoMove(map, neighborPos.Value, move, MapObject.Empty))
                    return false;
            }
            map[pos.x, pos.y] = replacement;
            map[newPos.x, newPos.y] = currentObj;

            return true;
        }

        private void ParsePlayground(string[] inputData, bool expand)
        {
            var mapLines = new List<string>();
            var moveCommands = new StringBuilder();
            var onMapLines = true;
            foreach (var line in inputData)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    onMapLines = false;
                    continue;
                }
                if (onMapLines)
                    mapLines.Add(expand ? ExpandLine(line) : line);
                else
                    moveCommands.Append(line);
            }

            this._moves.Clear();
            this._moves.AddRange(moveCommands.ToString().Select(c => c switch
            {
                '^' => this._moveDirections[Direction.North],
                '>' => this._moveDirections[Direction.East],
                'v' => this._moveDirections[Direction.South],
                '<' => this._moveDirections[Direction.West],
                _ => throw new InvalidOperationException($"Invalid move command: {c}")
            }));

            this._map = this.MapInput([.. mapLines], (c, p) =>
            {
                if (c == '@')
                    this._fishPos = p;

                return c switch
                {
                    '#' => MapObject.Wall,
                    'O' => MapObject.Box,
                    '@' => MapObject.Fish,
                    '.' => MapObject.Empty,
                    '[' => MapObject.BoxLeft,
                    ']' => MapObject.BoxRight,
                    _ => throw new InvalidOperationException($"Invalid map object: {c}")
                };
            });
            this._w = this._map.GetLength(0);
            this._h = this._map.GetLength(1);
        }

        private void PrintMap()
        {
            for (var y = 0; y < this._h; y++)
            {
                for (var x = 0; x < this._w; x++)
                {
                    var c = this._map[x, y] switch
                    {
                        MapObject.Wall => '#',
                        MapObject.Box => 'O',
                        MapObject.Fish => '@',
                        MapObject.Empty => '.',
                        MapObject.BoxLeft => '[',
                        MapObject.BoxRight => ']',
                        _ => throw new InvalidOperationException($"Invalid map object: {this._map[x, y]}")
                    };
                    Console.Write(c);
                }
                Console.WriteLine();
            }
        }

        private int CalcMapSum()
        {
            var boxSum = 0;
            for (var y = 0; y < this._h; y++)
            {
                for (var x = 0; x < this._w; x++)
                {
                    if (this._map[x, y] == MapObject.Box || this._map[x, y] == MapObject.BoxLeft)
                        boxSum += (y * 100 + x);
                }
            }
            return boxSum;
        }

        private static string ExpandLine(string line)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < line.Length; i++)
            {
                switch (line[i])
                {
                    case '.':
                        sb.Append("..");
                        break;
                    case '@':
                        sb.Append("@.");
                        break;
                    case 'O':
                        sb.Append("[]");
                        break;
                    case '#':
                        sb.Append("##");
                        break;
                    default:
                        throw new InvalidOperationException($"Invalid character: {line[i]}");
                }
            }
            return sb.ToString();
        }
    }
}
