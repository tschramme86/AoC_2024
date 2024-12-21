using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AoC2024.Days.AoCChallengeBase;

namespace AoC2024.Helper
{
    internal class Math2D
    {
        public static readonly Dictionary<Direction, (int x, int y)> MoveDirectionsMap = new()
        {
            { Direction.North, (0, -1) },
            { Direction.East, (1, 0) },
            { Direction.South, (0, 1) },
            { Direction.West, (-1, 0) }
        };

        public static readonly (int x, int y)[] MoveDirections =
        [
            (0, -1),
            (1, 0),
            (0, 1),
            (-1, 0)
        ];

        public static (int x, int y) Add((int x, int y) a, (int x, int y) b) => (a.x + b.x, a.y + b.y);
    }
}
