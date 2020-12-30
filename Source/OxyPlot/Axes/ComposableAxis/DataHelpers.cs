﻿using System;
using System.Collections.Generic;
using System.Text;

namespace OxyPlot.Axes.ComposableAxis
{
    /// <summary>
    /// Data Helpers
    /// </summary>
    public static class DataHelpers
    {
        /// <summary>
        /// Finds the maximum of two data values.
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="TProvider"></typeparam>
        /// <param name="provider"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static TData Max<TData, TProvider>(this TProvider provider, TData a, TData b)
            where TProvider : IComparer<TData>
        {
            return provider.Compare(a, b) >= 0 ? a : b;
        }

        /// <summary>
        /// Finds the maximum of two data values.
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="TProvider"></typeparam>
        /// <param name="provider"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static TData Min<TData, TProvider>(this TProvider provider, TData a, TData b)
            where TProvider : IComparer<TData>
        {
            // make sure we return the opposite of Max when they compare equal
            return provider.Compare(a, b) >= 0 ? b : a;
        }

        /// <summary>
        /// Returns a default value as a TOptional if the given optional is not set, otherwise returns the given optional
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TOptional"></typeparam>
        /// <typeparam name="TOptionalProvider"></typeparam>
        /// <param name="provider"></param>
        /// <param name="optional"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static TValue Unpack<TValue, TOptional, TOptionalProvider>(this TOptionalProvider provider, TOptional optional, TValue defaultValue)
            where TOptionalProvider : IOptionalProvider<TValue, TOptional>
        {
            return provider.TryGetValue(optional, out var found) ? found : defaultValue;
        }

        /// <summary>
        /// Returns a default value as a TOptional if the given optional is not set, otherwise returns the given optional
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TOptional"></typeparam>
        /// <typeparam name="TOptionalProvider"></typeparam>
        /// <param name="provider"></param>
        /// <param name="optional"></param>
        /// <param name="defaultOptional"></param>
        /// <returns></returns>
        public static TOptional Coerce<TValue, TOptional, TOptionalProvider>(TOptionalProvider provider, TOptional optional, TOptional defaultOptional)
            where TOptionalProvider : IOptionalProvider<TValue, TOptional>
        {
            return provider.HasValue(optional) ? optional : defaultOptional;
        }

        /// <summary>
        /// Includes the given value in the given range given the given comparer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TComparer"></typeparam>
        /// <param name="range"></param>
        /// <param name="comparer"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Range<T> Include<T, TComparer>(this Range<T> range, TComparer comparer, T value)
            where TComparer : IComparer<T>
        {
            if (range.TryGetMinMax(out var min, out var max))
            {
                return new Range<T>(comparer.Min(min, value), comparer.Max(max, value));
            }
            else
            {
                return new Range<T>(value);
            }
        }
    }
}