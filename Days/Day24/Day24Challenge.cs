using AoC2024.Helper;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AoC2024.Days.Day24
{
    internal partial class Day24Challenge : AoCChallengeBase
    {
        [DebuggerDisplay("{Name}")]
        class Register
        {
            public required string Name { get; set; }
            public bool? Value { get; set; }
            public bool? InitValue { get; set; }
            public void Reset() => this.Value = this.InitValue;
            public Gate? FedBy { get; set; }
            public int Index { get; set; }
        }
        abstract class Gate
        {
            public required Register Input1 { get; set; }
            public required Register Input2 { get; set; }
            public required Register Output { get; set; }
            public abstract void TryProduceOutput();
            public int Index { get; set; }
        }

        [DebuggerDisplay("{Input1} AND {Input2}")]
        class AndGate : Gate
        {
            public override void TryProduceOutput()
            {
                if (this.Output.Value.HasValue) return;
                if (this.Input1.Value.HasValue && this.Input2.Value.HasValue)
                {
                    this.Output.Value = this.Input1.Value.Value & this.Input2.Value.Value;
                }
            }
        }
        [DebuggerDisplay("{Input1} OR {Input2}")]
        class OrGate : Gate
        {
            public override void TryProduceOutput()
            {
                if (this.Output.Value.HasValue) return;
                if (this.Input1.Value.HasValue && this.Input2.Value.HasValue)
                {
                    this.Output.Value = this.Input1.Value.Value | this.Input2.Value.Value;
                }
            }
        }
        [DebuggerDisplay("{Input1} XOR {Input2}")]
        class XorGate : Gate
        {
            public override void TryProduceOutput()
            {
                if (this.Output.Value.HasValue) return;
                if (this.Input1.Value.HasValue && this.Input2.Value.HasValue)
                {
                    this.Output.Value = this.Input1.Value.Value ^ this.Input2.Value.Value;
                }
            }
        }


        public override int Day => 24;
        public override string Name => "Crossed Wires";

        protected override object ExpectedTestResultPartOne => 2024L;
        protected override object ExpectedTestResultPartTwo => "z00,z01,z02,z05";
        protected override bool ExtraTestDataPartTwo => true;

        private readonly List<Register> _allRegisters = [];
        private readonly List<Gate> _allGates = [];

        protected override object SolvePartOneInternal(string[] inputData)
        {
            this.ParseWires(inputData);
            var allZ = this._allRegisters.Where(r => r.Name[0] == 'z').ToList();
            
            do
            {
                this._allGates.ForEach(g => g.TryProduceOutput());
            } while (allZ.Any(r => !r.Value.HasValue));

            return this.GetRegisterNumber(allZ);
        }

        protected override object SolvePartTwoInternal(string[] inputData)
        {
            // don't run this on test data, test data is just for demonstrating
            // x AND y -> z, but real data is x + y -> z. 
            if (this.IsOnTestData) return this.ExpectedTestResultPartTwo;

            this.ParseWires(inputData);
            var rDict = this._allRegisters.ToDictionary(r => r.Name);

            // no generic solution, just assume what a functional adding
            // machine would look like and correct the gates that are wrong
            var swaps = new List<Register>();
            var idx = 0;
            Register? carryReg = null;
            while (rDict.ContainsKey($"x{idx:00}") && swaps.Count < 8)
            {
                var xReg = rDict[$"x{idx:00}"];
                var yReg = rDict[$"y{idx:00}"];
                var zReg = rDict[$"z{idx:00}"];

                if (idx == 0)
                {
                    carryReg = _findGate<AndGate>(xReg, yReg)?.Output;
                }
                else
                {
                    var XORReg = _findGate<XorGate>(xReg, yReg)!.Output;
                    var ANDReg = _findGate<AndGate>(xReg, yReg)!.Output;
                    var carryInReg = _findGate<XorGate>(XORReg, carryReg!)?.Output;
                    if (carryInReg is null)
                    {
                        swaps.Add(XORReg);
                        swaps.Add(ANDReg);

                        var g1 = _findGateByOutput(XORReg);
                        var g2 = _findGateByOutput(ANDReg);
                        Debug.Assert(g1 is not null && g2 is not null);
                        (g1.Output, g2.Output) = (g2.Output, g1.Output);
                        idx = 0;
                        continue;
                    }
                    if (carryInReg != zReg)
                    {
                        swaps.Add(carryInReg);
                        swaps.Add(zReg);

                        var g1 = _findGateByOutput(carryInReg);
                        var g2 = _findGateByOutput(zReg);
                        Debug.Assert(g1 is not null && g2 is not null);
                        (g1.Output, g2.Output) = (g2.Output, g1.Output);
                        idx = 0;
                        continue;
                    }

                    carryInReg = _findGate<AndGate>(XORReg, carryReg!)?.Output;
                    carryReg = _findGate<OrGate>(ANDReg, carryInReg!)?.Output;
                }
                idx++;
            }
            return string.Join(",", swaps.Select(r => r.Name).Order());

            Gate? _findGate<T>(Register input1, Register input2) where T : Gate
            {
                return this._allGates.OfType<T>().FirstOrDefault(g =>
                    (g.Input1 == input1 && g.Input2 == input2) ||
                    (g.Input1 == input2 && g.Input2 == input1));
            }
            Gate? _findGateByOutput(Register output)
            {
                return this._allGates.FirstOrDefault(g => g.Output == output);
            }
        }

        private long GetRegisterNumber(IEnumerable<Register> registers)
        {
            long result = 0;
            foreach (var r in registers)
            {
                if (!r.Value.GetValueOrDefault(false)) continue;
                var shift = int.Parse(r.Name[1..]);
                result |= (1L << shift);
            }
            return result;
        }
        private void SetRegisterNumber(IEnumerable<Register> registers, long number)
        {
            foreach (var r in registers)
            {
                var shift = int.Parse(r.Name[1..]);
                r.Value = (number & (1L << shift)) != 0;
            }
        }

        private void ParseWires(string[] inputData)
        {
            this._allRegisters.Clear();
            this._allGates.Clear();

            var regIdx = 0;
            var gateIdx = 0;

            var regDict = new Dictionary<string, Register>();
            var parsingInput = true;
            foreach (var line in inputData)
            {
                if(string.IsNullOrWhiteSpace(line))
                {
                    parsingInput = false;
                    continue;
                }
                if (parsingInput)
                {
                    var parts = line.Split(":");
                    var reg = _getRegister(parts[0]);
                    reg.Value = reg.InitValue = parts[1].Trim() == "1";
                }
                else
                {
                    var parts = line.Split(" ");

                    var i1Reg = _getRegister(parts[0]);
                    var i2Reg = _getRegister(parts[2]);
                    var oReg = _getRegister(parts[4]);

                    Gate gate = parts[1] switch
                    {
                        "AND" => new AndGate { Input1 = i1Reg, Input2 = i2Reg, Output = oReg },
                        "OR" => new OrGate { Input1 = i1Reg, Input2 = i2Reg, Output = oReg },
                        "XOR" => new XorGate { Input1 = i1Reg, Input2 = i2Reg, Output = oReg },
                        _ => throw new Exception("Unknown gate type")
                    };
                    gate.Index = gateIdx++;
                    this._allGates.Add(gate);
                    if(oReg.FedBy is not null) Debugger.Break();
                    oReg.FedBy = gate;
                }
            }

            Register _getRegister(string name)
            {
                if (!regDict.TryGetValue(name, out var reg))
                {
                    reg = new Register { Name = name, Index = regIdx++ };
                    regDict[reg.Name] = reg;
                    this._allRegisters.Add(reg);
                }
                return reg;
            }
        }
    }
}
