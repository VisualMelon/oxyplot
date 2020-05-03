﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoubleSequenceHelper.cs" company="OxyPlot">
//   Copyright (c) 2020 OxyPlot contributors
// </copyright>
// <summary>
//   Provides a base class for extracting basic information about sequences of double values.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OxyPlot.Utilities
{
    using System;

    /// <summary>
    /// Extracts basic information about sequences of <see cref="double"/> values.
    /// </summary>
    public sealed class DoubleSequenceHelper : SequenceHelperBase<double>
    {
        /// <summary>
        /// Initalises an sequence helper with the given default miniums and maximums for empty sequences.
        /// These values will be overwritten when any entry if observed.
        /// </summary>
        /// <param name="throwOnNaN">Whether the <see cref="DoubleSequenceHelper"/> should thrown if a NaN value is observed.</param>
        /// <param name="defaultMinimum">The default minimum of an empty sequence.</param>
        /// <param name="defaultMaximum">The default maximum of an empty sequence.</param>
        /// <param name="preserveDefaultMinimum">Whether the default minimum value should be preserved.</param>
        /// <param name="preserveDefaultMaximum">Whether the default maximum value should be preserved.</param>
        public DoubleSequenceHelper(bool throwOnNaN, double defaultMinimum, double defaultMaximum, bool preserveDefaultMinimum, bool preserveDefaultMaximum)
            : base(defaultMinimum, defaultMaximum, preserveDefaultMinimum, preserveDefaultMaximum)
        {
            ThrowOnNaN = throwOnNaN;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="DoubleSequenceHelper"/> should throw on NaN values.
        /// If this value is true, observing a NaN value will results in a <see cref="ArgumentException"/>.
        /// If this value is false, NaN values will be ignored.
        /// </summary>
        public bool ThrowOnNaN { get; }

        /// <inheritdoc/>
        public override bool CheckValid(double value)
        {
            if (double.IsNaN(value))
            {
                if (ThrowOnNaN)
                {
                    throw new ArgumentException("Value must not be NaN", nameof(value));
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
    }
}
