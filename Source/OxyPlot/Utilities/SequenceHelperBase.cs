// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SequenceHelperBase.cs" company="OxyPlot">
//   Copyright (c) 2020 OxyPlot contributors
// </copyright>
// <summary>
//   Provides a base class for extracting basic information about sequences.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OxyPlot.Utilities
{
    using System;

    /// <summary>
    /// Provides methods for processing sequences.
    /// </summary>
    public abstract class SequenceHelperBase<T> where T : IComparable<T>
    {
        /// <summary>
        /// Initalised an sequence helper with the given default miniums and maximums for empty sequences.
        /// These values will be overwritten when any entry if observed.
        /// </summary>
        /// <param name="defaultMinimum">The default minimum of an empty sequence.</param>
        /// <param name="defaultMaximum">The default maximum of an empty sequence.</param>
        /// <param name="preserveDefaultMinimum">Whether the default minimum value should be preserved.</param>
        /// <param name="preserveDefaultMaximum">Whether the default maximum value should be preserved.</param>
        public SequenceHelperBase(T defaultMinimum, T defaultMaximum, bool preserveDefaultMinimum, bool preserveDefaultMaximum)
        {
            Minimum = defaultMinimum;
            Maximum = defaultMaximum;
            PreserveDefaultMinimum = preserveDefaultMinimum;
            PreserveDefaultMaximum = preserveDefaultMaximum;

            Count = 0;
            HasIncreases = false;
            HasDecreases = false;
            HasRepeats = false;
        }

        /// <summary>
        /// Gets a value indicating whether the default minimum value should be preserved.
        /// If true, then the computed minimum is bounded by the default minimum.
        /// If false, the default values are discarded when the first value is observed.
        /// </summary>
        public bool PreserveDefaultMinimum { get; }

        /// <summary>
        /// Gets a value indicating whether the default maximum value should be preserved.
        /// If true, then the computed maximum is bounded by the default maximum.
        /// If false, the default values are discarded when the first value is observed.
        /// </summary>
        public bool PreserveDefaultMaximum { get; }

        /// <summary>
        /// Gets a value indicating whether the sequence so far as contained any increases.
        /// </summary>
        public bool HasIncreases { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the sequence so far as contained any decreases.
        /// </summary>
        public bool HasDecreases { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the sequence so far as contained any repeated values.
        /// </summary>
        public bool HasRepeats { get; private set; }

        /// <summary>
        /// Gets the number of elements observed so far.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Gets a value indicating whether no elements have yet to be observed.
        /// </summary>
        public bool IsEmpty => Count == 0;

        /// <summary>
        /// The previous value observed, if not empty.
        /// </summary>
        private T Previous { get; set; }

        /// <summary>
        /// Gets the largest value observed so far.
        /// </summary>
        public T Maximum { get; private set; }

        /// <summary>
        /// Gets the smallest value observed so far.
        /// </summary>
        public T Minimum { get; private set; }

        /// <summary>
        /// Provides the next value in the sequence to be observed.
        /// </summary>
        /// <param name="value">The next value in the sequence.</param>
        public void ObserveNext(T value)
        {
            if (!CheckValid(value))
            {
                return;
            }

            if (IsEmpty)
            {
                if (!PreserveDefaultMinimum || value.CompareTo(Minimum) < 0)
                {
                    Minimum = value;
                }

                // don't assume Minimum < Maximum initially
                if (!PreserveDefaultMaximum || value.CompareTo(Maximum) > 0)
                {
                    Maximum = value;
                }
            }
            else
            {
                var c = value.CompareTo(Previous);

                if (c > 0)
                {
                    HasIncreases = true;

                    if (value.CompareTo(Maximum) > 0)
                    {
                        Maximum = value;
                    }
                }
                else if (c < 0)
                {
                    HasDecreases = true;

                    if (value.CompareTo(Minimum) < 0)
                    {
                        Minimum = value;
                    }
                }
                else
                {
                    HasRepeats = true;
                }
            }

            Previous = value;
            Count++;
        }

        /// <summary>
        /// Gets a value describing the monotonicity of the observed sequence so far.
        /// </summary>
        /// <returns>The monotonicity of the observed sequence.</returns>
        public Monotonicity GetMonotonicity()
        {
            return new Monotonicity(HasIncreases, HasDecreases, HasRepeats);
        }

        /// <summary>
        /// Determines whether a given value is valid.
        /// May throw to signal an illegal input.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if the value should be included, otherwise false.</returns>
        public abstract bool CheckValid(T value);
    }
}
