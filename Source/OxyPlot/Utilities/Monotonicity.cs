// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Monotonicity.cs" company="OxyPlot">
//   Copyright (c) 2020 OxyPlot contributors
// </copyright>
// <summary>
//   Describes the monotonicty of a sequence.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OxyPlot.Utilities
{
    /// <summary>
    /// Describes the monotonicty of a sequence.
    /// </summary>
    public readonly struct Monotonicity
    {
        /// <summary>
        /// At least one element in the sequence is greater than the previous element.
        /// </summary>
        public readonly bool HasIncreases;

        /// <summary>
        /// At least one element in the sequence is less than the previous element.
        /// </summary>
        public readonly bool HasDecreases;

        /// <summary>
        /// At least one element in equal to the previous element.
        /// </summary>
        public readonly bool HasRepeats;

        /// <summary>
        /// Initialises an instance of <see cref="Monotonicity"/> from the given sequence information.
        /// </summary>
        /// <param name="hasIncreases">Whether the sequence contained increasing values.</param>
        /// <param name="hasDecreases">Whether the sequence contained decreasing values.</param>
        /// <param name="hasRepeats">Whether the sequence contained repeated values.</param>
        public Monotonicity(bool hasIncreases, bool hasDecreases, bool hasRepeats)
        {
            HasIncreases = hasIncreases;
            HasDecreases = hasDecreases;
            HasRepeats = hasRepeats;
        }

        /// <summary>
        /// Gets a value indicating whether the sequence is empty.
        /// There are no elements in the sequence.
        /// </summary>
        public bool IsEmpty => !HasIncreases && !HasDecreases && !HasRepeats;

        /// <summary>
        /// Gets a value indicating whether the sequence is monotonically increasing.
        /// Each element is no greater than the previous element, or the sequence is empty.
        /// </summary>
        public bool IsNonDecreasing => !HasDecreases;

        /// <summary>
        /// Gets a value indicating whether the sequence is monotonically increasing.
        /// Each element is no greater than the previous element, or the sequence is empty.
        /// </summary>
        public bool IsNonIncreasing => !HasIncreases;

        /// <summary>
        /// Gets a value indicating whether the sequence is monotonically increasing.
        /// Each element is strictly less than the previous element, or the sequence is empty.
        /// </summary>
        public bool IsStrictlyDecreasing => !HasIncreases && !HasRepeats;

        /// <summary>
        /// Gets a value indicating whether the sequence is monotonically increasing.
        /// Each element is strictly greater than the previous element, or the sequence is empty.
        /// </summary>
        public bool IsStrictlyIncreasing => !HasDecreases && !HasRepeats;

        /// <summary>
        /// Gets a value indicating whether the sequence is constant.
        /// All elements in the sequence are equal, or the sequence is empty.
        /// </summary>
        public bool IsConstant => !HasDecreases && !HasIncreases;

        /// <summary>
        /// Gets a value indicating where the sequence lacks any monotonicity.
        /// The sequence contains both increasing and decreasing sub-sequences.
        /// </summary>
        public bool IsNotMonotonic => HasDecreases && HasIncreases;
    }
}
