﻿using System;
using System.Collections.Generic;
using System.Linq;
using VirsTimer.Core.Models;

namespace VirsTimer.Core.Utils
{
    /// <summary>
    /// Utility class for statistics.
    /// </summary>
    public static class StatisticsUtils
    {
        /// <summary>
        /// Default DNF time.
        /// </summary>
        public static readonly TimeSpan DnfTime = TimeSpan.MaxValue;

        /// <summary>
        /// Calculates best time from <paramref name="solves"/>/
        /// </summary>
        public static TimeSpan? BestTime(this ICollection<Solve> solves)
        {
            return solves.Min(x => x.TimeWithFlag);
        }

        /// <summary>
        /// Calculates Mo3 from <paramref name="source"/>/
        /// </summary>
        public static TimeSpan? Mo3(this ICollection<Solve> source)
        {
            var solves = source.TakeLast(3).ToList();
            return solves.Count < 3
                ? null
                : (solves.Any(x => x.Flag == Constants.SolveFlag.DNF)
                ? DnfTime
                : solves.Average());
        }

        /// <summary>
        /// Calculates Ao5 from <paramref name="source"/>/
        /// </summary>
        public static TimeSpan? Ao5(this ICollection<Solve> source)
        {
            return Ao(source, 5);
        }

        /// <summary>
        /// Calculates Ao12 from <paramref name="source"/>/
        /// </summary>
        public static TimeSpan? Ao12(this ICollection<Solve> source)
        {
            return Ao(source, 12);
        }

        /// <summary>
        /// Calculates Ao12 from <paramref name="source"/>/
        /// </summary>
        public static TimeSpan? Ao100(this ICollection<Solve> source)
        {
            return Ao(source, 100);
        }

        /// <summary>
        /// Calculates Ao from <paramref name="source"/>/
        /// </summary>
        private static TimeSpan? Ao(this ICollection<Solve> source, int amount)
        {
            if (source.Count < amount)
                return null;

            var solves = source.TakeLast(amount).ToList();
            if (solves.Count(x => x.Flag == Constants.SolveFlag.DNF) > 1)
                return DnfTime;
            var times = solves.OrderBy(x => x.TimeWithFlag).Skip(1).Take(amount - 2).ToList();
            return times.Average();
        }

        /// <summary>
        /// Calculates average from <paramref name="source"/>/
        /// </summary>
        private static TimeSpan Average(this ICollection<Solve> source)
        {
            var sum = 0L;
            foreach (var solve in source)
                sum += solve.TimeWithFlag.Ticks;

            var average = sum / (decimal)source.Count;

            var rounded = Math.Round(average / 100000m, 0, MidpointRounding.AwayFromZero) * 100000;
            return TimeSpan.FromTicks((long)rounded);
        }
    }
}
