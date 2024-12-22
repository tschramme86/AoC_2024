using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2024.Days.Day09
{
    internal class Day09Challenge : AoCChallengeBase
    {
        abstract class DiskBlock
        {
            public virtual bool IsFree { get; }
            public virtual long Checksum { get; }
            public int Size { get; set; } = 1;
            public int Index { get; set; }
        }
        class FreeBlock : DiskBlock
        {
            public override bool IsFree => true;
            public override long Checksum => 0;
        }
        class DataBlock(int fileId) : DiskBlock
        {
            public int FileId { get; private set; } = fileId;
            public override bool IsFree => false;
            public override long Checksum
            {
                get
                {
                    return Enumerable.Range(this.Index, this.Size).Select(i => (long)i * this.FileId).Sum();
                }
            }
        }

        public override int Day => 9;
        public override string Name => "Disk Fragmenter";

        protected override object ExpectedTestResultPartOne => 1928L;
        protected override object ExpectedTestResultPartTwo => 2858L;

        protected override object SolvePartOneInternal(string[] inputData)
        {
            var blocks = ParseDiskBlocks(inputData[0]);
            var freeSpaceIndex = 0;
            var dataBlockIndex = blocks.Count - 1;
            while (true)
            {
                for(; freeSpaceIndex < blocks.Count; freeSpaceIndex++)
                {
                    if (blocks[freeSpaceIndex].IsFree) break;
                }
                for (; dataBlockIndex >= 0; dataBlockIndex--)
                {
                    if (!blocks[dataBlockIndex].IsFree) break;
                }
                if (freeSpaceIndex >= dataBlockIndex) break;
                (blocks[dataBlockIndex], blocks[freeSpaceIndex]) = (blocks[freeSpaceIndex], blocks[dataBlockIndex]);
            }

            // reindex blocks
            for (var x = 0; x < blocks.Count; x++)
            {
                blocks[x].Index = x;
            }
            return blocks.Sum(b => b.Checksum);
        }

        protected override object SolvePartTwoInternal(string[] inputData)
        {
            var blocks = ParseDiskBlocksFull(inputData[0]);
            var totalSize = blocks.Sum(b => b.Size);

            var fileBlocks = blocks.OfType<DataBlock>().OrderByDescending(b => b.FileId).ToList();
            foreach (var fb in fileBlocks)
            {
                var blockIndex = blocks.IndexOf(fb);
                for (var i = 0; i < blockIndex; i++)
                {
                    if (blocks[i].IsFree && blocks[i].Size >= fb.Size)
                    {
                        // replace file block with free block at the end
                        blocks[blockIndex] = new FreeBlock { Size = fb.Size };

                        // shrink the free block at the beginning or remove completely
                        if (fb.Size < blocks[i].Size)
                            blocks[i].Size -= fb.Size;
                        else
                            blocks.RemoveAt(i);

                        // insert the data block at the new position
                        blocks.Insert(i, fb);

                        // merge free blocks at the end
                        for (var k=blockIndex-1; k<blockIndex+1; k++)
                        {
                            if (k + 1 >= blocks.Count) break;
                            if (blocks[k].IsFree && blocks[k + 1].IsFree)
                            {
                                blocks[k].Size += blocks[k + 1].Size;
                                blocks.RemoveAt(k + 1);
                                k--;
                            }
                        }

                        break;
                    }
                }
#if DEBUG
                var newSize = blocks.Sum(b => b.Size);
                if (newSize != totalSize)
                {
                    throw new InvalidOperationException("Block size mismatch");
                }
                // PrintBlocks(blocks);
#endif
            }

            // reindex blocks
            var idx = 0;
            foreach (var b in blocks)
            {
                b.Index = idx;
                idx += b.Size;
            }
            return blocks.Sum(b => b.Checksum);
        }

        private List<DiskBlock> ParseDiskBlocks(string line)
        {
            var isFreeBlock = false;
            var ret = new List<DiskBlock>();
            var fileId = 0;
            foreach(var l in line.Select(c => (c - '0')))
            {
                if(l > 0)
                {
                    for(var x=0; x < l; x++)
                    {
                        if(isFreeBlock)
                        {
                            ret.Add(new FreeBlock());
                        }
                        else
                        {
                            ret.Add(new DataBlock(fileId));
                        }
                    }
                    if (!isFreeBlock) fileId++;
                }
                isFreeBlock = !isFreeBlock;
            }

            return ret;
        }

        private List<DiskBlock> ParseDiskBlocksFull(string line)
        {
            var isFreeBlock = false;
            var ret = new List<DiskBlock>();
            var fileId = 0;
            foreach (var l in line.Select(c => (c - '0')))
            {
                if (l > 0)
                {
                    if (isFreeBlock)
                    {
                        ret.Add(new FreeBlock { Size = l });
                    }
                    else
                    {
                        ret.Add(new DataBlock(fileId) { Size = l });
                    }
                    if (!isFreeBlock) fileId++;
                }
                isFreeBlock = !isFreeBlock;
            }

            return ret;
        }

        private void PrintBlocks(List<DiskBlock> blocks)
        {
            foreach (var b in blocks)
            {
                if (b.IsFree)
                    Console.Write($"{new string('.', b.Size)}");
                else
                    Console.Write($"{new string((((DataBlock)b).FileId.ToString())[0], b.Size)}");
            }
            Console.WriteLine();
        }
    }
}
