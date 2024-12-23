using AoC2024.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2024.Days.Day23
{
    internal partial class Day23Challenge : AoCChallengeBase
    {
        public override int Day => 23;
        public override string Name => "LAN Party";

        protected override object ExpectedTestResultPartOne => 7;
        protected override object ExpectedTestResultPartTwo => "co,de,ka,ta";

        private string[] _computerNodes = [];
        private bool[,] _connections = new bool[0, 0];

        protected override object SolvePartOneInternal(string[] inputData)
        {
            this.ParseComputerConnections(inputData);

            var interconnected = this.Find3InterconnectedComputer();
            var probablyChief = this._computerNodes.Select(n => n[0] == 't').ToArray();
            var groupsWithChief = interconnected.Where(i => probablyChief[i.c1] || probablyChief[i.c2] || probablyChief[i.c3]).ToList();

            return groupsWithChief.Count;
        }

        protected override object SolvePartTwoInternal(string[] inputData)
        {
            this.ParseComputerConnections(inputData);
            var allConnectedGroups = this.FindConnectedSubgraphs();

            var maxGroupSize = allConnectedGroups.Max(g => g.Count);
            var maxGroup = allConnectedGroups.First(g => g.Count == maxGroupSize);
            var allComputersInGroup = maxGroup.Select(i => this._computerNodes[i]).ToList();
            allComputersInGroup.Sort();
            var password = string.Join(",", allComputersInGroup);

            return password;
        }

        private List<List<int>> FindConnectedSubgraphs()
        {
            var result = new List<List<int>>();
            for (var node = 0; node < this._computerNodes.Length; node++)
            {
                this.AddConnectedNodes(node, [node], result);
            }
            return result;
        }

        private void AddConnectedNodes(int node, List<int> connected, List<List<int>> target)
        {
            var canExtend = false;
            for (var i = node + 1; i < this._computerNodes.Length; i++)
            {
                var isConnectdToAll = connected.All(c => this._connections[c, i]);
                if (isConnectdToAll)
                {
                    this.AddConnectedNodes(i, [.. connected.Append(i)], target);
                    canExtend = true;
                }
            }
            if (!canExtend)
            {
                target.Add(connected);
            }
        }

        private List<(int c1, int c2, int c3)> Find3InterconnectedComputer()
        {
            var result = new List<(int c1, int c2, int c3)>();

            for (var i = 0; i < this._computerNodes.Length; i++)
            {
                for (var j = i + 1; j < this._computerNodes.Length; j++)
                {
                    if (this._connections[i, j])
                    {
                        for (var k = j + 1; k < this._computerNodes.Length; k++)
                        {
                            if (this._connections[j, k] && this._connections[i, k])
                            {
                                result.Add((i, j, k));
                            }
                        }
                    }
                }
            }
            return result;
        }

        private void ParseComputerConnections(string[] inputData)
        {
            var allNodes = new HashSet<string>();
            var allConnections = new List<(string, string)>();
            foreach (var conn in inputData)
            {
                var n = conn.Split('-');
                allNodes.Add(n[0]);
                allNodes.Add(n[1]);
                allConnections.Add((n[0], n[1]));
            }

            this._computerNodes = [.. allNodes];
            this._connections = new bool[this._computerNodes.Length, this._computerNodes.Length];
            foreach (var conn in allConnections)
            {
                var i = Array.IndexOf(this._computerNodes, conn.Item1);
                var j = Array.IndexOf(this._computerNodes, conn.Item2);
                this._connections[i, j] = true;
                this._connections[j, i] = true;
            }
        }
    }
}
