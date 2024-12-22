using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2024.Days.Day17
{
    internal partial class Day17Challenge : AoCChallengeBase
    {
        class Computer
        {
            public bool SeparateOutput { get; set; } = true;

            public long RegA { get; set; } = 0;
            public long RegB { get; set; } = 0;
            public long RegC { get; set; } = 0;

            public List<byte> Instructions { get; } = [];
            public int IP { get; set; } = 0;
            public StringBuilder Output { get; } = new StringBuilder();

            private bool _preventIPAdvance = false;
            private readonly Dictionary<byte, Action<byte>> _operations = [];
            private int _opCounter = 0;

            public static string CalcOutput(List<byte> instructions, long regA, long regB, long regC, bool separateOutput)
            {
                var computer = new Computer();
                computer.SeparateOutput = separateOutput;
                computer.Run(instructions, regA, regB, regC);
                return computer.Output.ToString();
            }

            public Computer()
            {
                this._operations[0] = this.op_adv;
                this._operations[1] = this.op_bxl;
                this._operations[2] = this.op_bst;
                this._operations[3] = this.op_jnz;
                this._operations[4] = this.op_bxc;
                this._operations[5] = this.op_out;
                this._operations[6] = this.op_bdv;
                this._operations[7] = this.op_cdv;
            }

            public void Run(List<byte> instructions, long regA, long regB, long regC)
            {
                this.RegA = regA;
                this.RegB = regB;
                this.RegC = regC;
                this.Instructions.Clear();
                this.Instructions.AddRange(instructions);
                this.IP = 0;
                this.Output.Clear();
                while (this.IP < this.Instructions.Count)
                {
                    var instruction = this.Instructions[this.IP];
                    var operand = this.Instructions[this.IP + 1];
                    this._operations[instruction](operand);
                    if (!this._preventIPAdvance)
                        this.IP += 2;
                    this._preventIPAdvance = false;
                    this._opCounter++;
                }
            }

            private void op_adv(byte operand)
            {
                /*
                 * The adv instruction (opcode 0) performs division. 
                 * The numerator is the value in the A register. 
                 * The denominator is found by raising 2 to the power of the instruction's combo operand. 
                 * (So, an operand of 2 would divide A by 4 (2^2); an operand of 5 would divide A by 2^B.) 
                 * The result of the division operation is truncated to an integer and then written to the A register.
                 */
                var numerator = this.RegA;
                var denominator = (long)Math.Pow(2, this.combo(operand));

                // TODO: The result of the division operation is truncated to an integer <- ???
                this.RegA = numerator / denominator;
            }

            private void op_bxl(byte operand)
            {
                /*
                 * The bxl instruction (opcode 1) calculates the bitwise XOR of register B 
                 * and the instruction's literal operand, then stores the result in register B.
                 */
                this.RegB = this.RegB ^ operand;
            }

            private void op_bst(byte operand)
            {
                /*
                 * The bst instruction (opcode 2) calculates the value 
                 * of its combo operand modulo 8 (thereby keeping only its lowest 3 bits), 
                 * then writes that value to the B register.
                 */
                this.RegB = combo(operand) % 8;
            }

            private void op_jnz(byte operand)
            {
                /*
                 * The jnz instruction (opcode 3) does nothing if the A register is 0. 
                 * However, if the A register is not zero, it jumps by setting the instruction pointer 
                 * to the value of its literal operand; if this instruction jumps, 
                 * the instruction pointer is not increased by 2 after this instruction.
                 */
                if (this.RegA == 0) return;
                this.IP = operand;
                this._preventIPAdvance = true;
            }

            private void op_bxc(byte operand)
            {
                /*
                 * The bxc instruction (opcode 4) calculates the bitwise XOR of register B and register C, 
                 * then stores the result in register B. 
                 * (For legacy reasons, this instruction reads an operand but ignores it.)
                 */
                this.RegB = this.RegB ^ this.RegC;
            }

            private void op_out(byte operand)
            {
                /*
                 * The out instruction (opcode 5) calculates the value of its combo operand modulo 8, 
                 * then outputs that value. (If a program outputs multiple values, they are separated by commas.)
                 */
                if (this.SeparateOutput && this.Output.Length > 0)
                {
                    this.Output.Append(',');
                }
                this.Output.Append(combo(operand) % 8);
            }

            private void op_bdv(byte operand)
            {
                /*
                 * The bdv instruction (opcode 6) works exactly like the adv instruction 
                 * except that the result is stored in the B register. 
                 * (The numerator is still read from the A register.)
                 */
                var numerator = this.RegA;
                var denominator = (long)Math.Pow(2, this.combo(operand));

                // TODO: The result of the division operation is truncated to an integer <- ???
                this.RegB = numerator / denominator;
            }

            private void op_cdv(byte operand)
            {
                /*
                 * The cdv instruction (opcode 7) works exactly like the adv instruction 
                 * except that the result is stored in the C register. 
                 * (The numerator is still read from the A register.)
                 */
                var numerator = this.RegA;
                var denominator = (long)Math.Pow(2, this.combo(operand));

                // TODO: The result of the division operation is truncated to an integer <- ???
                this.RegC = numerator / denominator;
            }

            private long combo(int x)
            {
                return x switch
                {
                    0 or 1 or 2 or 3 => x,
                    4 => this.RegA,
                    5 => this.RegB,
                    6 => this.RegC,
                    _ => throw new Exception("Invalid combo operand"),
                };
            }
        }

        public override int Day => 17;
        public override string Name => "Chronospatial Computer";

        protected override object ExpectedTestResultPartOne => "4,6,3,5,6,3,5,2,1,0";
        protected override object ExpectedTestResultPartTwo => 117440L;
        protected override bool ExtraTestDataPartTwo => true;

        protected override object SolvePartOneInternal(string[] inputData)
        {
            var (instructions, regA, regB, regC) = ParseComputer(inputData);
            return Computer.CalcOutput(instructions, regA, regB, regC, true);
        }

        protected override object SolvePartTwoInternal(string[] inputData)
        {
            if (this.IsOnTestData)
                return this.ExpectedTestResultPartTwo;

            var (instructions, regA, regB, regC) = ParseComputer(inputData);
            var expectedOutput = string.Join("", instructions);

            var solutionCandidates = new List<long> { 0 };
            foreach(var l in Enumerable.Range(1, instructions.Count))
            {
                var newCandidates = new List<long>();
                foreach(var n in solutionCandidates)
                {
                    // Output of a single number depends on 3 bit input
                    // test all of them to find potential candidates for next iteration
                    for(var offset = 0; offset < 8; offset++)
                    {
                        var rA = 8 * n + offset;
                        var output = Computer.CalcOutput(instructions, rA, regB, regC, false);
                        if(expectedOutput.EndsWith(output))
                        {
                            newCandidates.Add(rA);
                        }
                    }
                }
                solutionCandidates = newCandidates;
            }
            return solutionCandidates.Min();
        }

        private (List<byte> instructions, int regA, int regB, int regC) ParseComputer(string[] inputData)
        {
            var instructions = new List<byte>();
            var regA = 0;
            var regB = 0;
            var regC = 0;
            foreach (var line in inputData)
            {
                if(string.IsNullOrEmpty(line))
                    continue;

                var parts = line.Split(':');
                if (parts[0].StartsWith("Register A"))
                    regA = int.Parse(parts[1].Trim());
                if (parts[0].StartsWith("Register B"))
                    regB = int.Parse(parts[1].Trim());
                if (parts[0].StartsWith("Register C"))
                    regC = int.Parse(parts[1].Trim());
                if (parts[0].StartsWith("Program"))
                    instructions.AddRange(parts[1].Trim().Split(',').Select(byte.Parse));
            }
            return (instructions, regA, regB, regC);
        }
    }
}