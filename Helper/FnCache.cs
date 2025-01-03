﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2024.Helper
{
    public static class FnCache
    {
        public static Func<int, TR> MakeInt<TR>(Func<int, TR> fn)
            where TR : class
        {
            List<TR?> memo = new();
            return x1 =>
            {
                if (x1 < 0)
                {
                    throw new ArgumentException("Argument must be zero or greater");
                }
                lock (memo)
                {
                    if (memo.Count <= x1)
                    {
                        memo.AddRange(Enumerable.Repeat(default(TR?), x1 - memo.Count + 1));
                    }
                }
                return memo[x1] ?? (memo[x1] = fn(x1));
            };
        }

        public static Func<T1, TR> Make<T1, TR>(Func<T1, TR> fn)
            where T1 : notnull
        {
            ConcurrentDictionary<T1, TR> memo = new();
            return x1 =>
            {
                if (memo.TryGetValue(x1, out var result))
                {
                    return result;
                }
                return memo[x1] = fn(x1);
            };
        }
        public static Func<T1, T2, TR> Make<T1, T2, TR>(Func<T1, T2, TR> fn)
        {
            ConcurrentDictionary<(T1, T2), TR> memo = new();
            return (x1, x2) =>
            {
                if (memo.TryGetValue((x1, x2), out var result))
                {
                    return result;
                }
                return memo[(x1, x2)] = fn(x1, x2);
            };
        }
        public static Func<T1, T2, T3, TR> Make<T1, T2, T3, TR>(Func<T1, T2, T3, TR> fn)
        {
            ConcurrentDictionary<(T1, T2, T3), TR> memo = new();
            return (x1, x2, x3) =>
            {
                if (memo.TryGetValue((x1, x2, x3), out var result))
                {
                    return result;
                }
                return memo[(x1, x2, x3)] = fn(x1, x2, x3);
            };
        }
    }
}
