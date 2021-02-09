using System;
using System.Collections.Generic;
using System.Text;

namespace OxyPlot.Axes.ComposableAxis
{
    /// <summary>
    /// Represents the monotonicity of a sequence.
    /// </summary>
    public readonly struct Monotonicity
    {
        private const byte Increase = 1;
        private const byte Decrease = 2;
        private const byte Repeat = 4;
        private const byte _Empty = 8;

        private readonly byte Flags;

        private Monotonicity(byte flags)
        {
            Flags = flags;
        }

        /// <summary>
        /// Prepares a <see cref="Monotonicity"/>.
        /// </summary>
        /// <param name="constaintsRepeatedValues"></param>
        /// <param name="constaintsIncreasingSequences"></param>
        /// <param name="containsDecreasingSequences"></param>
        public Monotonicity(bool constaintsRepeatedValues, bool constaintsIncreasingSequences, bool containsDecreasingSequences)
        {
            byte flags = 0;
            if (constaintsRepeatedValues)
                flags |= Repeat;
            if (constaintsIncreasingSequences)
                flags |= Increase;
            if (containsDecreasingSequences)
                flags |= Decrease;
            Flags = flags;
        }

        /// <summary>
        /// The monotonicty of an empty sequence.
        /// </summary>
        public static Monotonicity Empty => new Monotonicity(_Empty);

        /// <summary>
        /// The monotonicty of a sequence of one value repeated many times.
        /// </summary>
        public static Monotonicity Repeating => new Monotonicity(Repeat);

        /// <summary>
        /// The monotonicty of a sequence of strictly increasing values.
        /// </summary>
        public static Monotonicity Increasing => new Monotonicity(Increase);

        /// <summary>
        /// The monotonicty of a sequence of strictly decreasing values.
        /// </summary>
        public static Monotonicity Decreasing => new Monotonicity(Decrease);

        /// <summary>
        /// The monotonicty of a sequence of strictly decreasing values.
        /// </summary>
        public static Monotonicity None => new Monotonicity(Decrease | Increase | Repeat);

        /// <summary>
        /// Gets a value indiciating whether the monotonicity is monotone.
        /// </summary>
        public bool IsEmpty => Flags == _Empty;

        /// <summary>
        /// Gets a value indiciating whether the monotonicity is monotone.
        /// </summary>
        public bool IsMonotone => (Flags & (Increase | Decrease)) != (Increase | Decrease);

        /// <summary>
        /// Gets a value indiciating whether the monotonicity is constant.
        /// </summary>
        public bool IsConstant => (Flags & (Increase | Decrease)) == 0;

        /// <summary>
        /// Gets a value indiciating whether the monotonicity is non-decreasing.
        /// </summary>
        public bool IsNonDecreasing => (Flags & Decrease) == 0;

        /// <summary>
        /// Gets a value indiciating whether the monotonicity is non-increasing.
        /// </summary>
        public bool IsNonIncreasing => (Flags & Increase) == 0;

        /// <summary>
        /// Gets a value indiciating whether the monotonicity is strictly decreasing.
        /// </summary>
        public bool IsStrictlyDecreasing => Flags == Decrease;

        /// <summary>
        /// Gets a value indiciating whether the monotonicity is strictly increasing.
        /// </summary>
        public bool IsStrictlyIncreasing => Flags == Increase;
    }

    /// <summary>
    /// Measures basic metrics concerning a sequence.
    /// </summary>
    public class MonotonicityHelper<TData, TDataProvider>
        where TDataProvider : IDataProvider<TData>
    {
        private const byte Increase = 1;
        private const byte Decrease = 2;
        private const byte Repeat = 4;
        private const byte Empty = 8;

        private byte Flags;

        private readonly TDataProvider Provider;

        private TData LastValue;

        /// <summary>
        /// Initialises an instance of <see cref="MonotonicityHelper{TData, TDataProvider}"/>.
        /// </summary>
        public MonotonicityHelper(TDataProvider provider)
        {
            Flags = Empty;
            Provider = provider;
            LastValue = default(TData);
        }

        /// <summary>
        /// Accepts the next value in the sequence.
        /// </summary>
        /// <param name="value"></param>
        public void Next(TData value)
        {
            if (Flags == Empty)
            {
                Flags = 0;
            }
            else
            {
                var c = Provider.Compare(LastValue, value);
                if (c == 0)
                    Flags |= Repeat;
                else if (c > 0)
                    Flags |= Increase;
                else
                    Flags |= Decrease;
            }

            LastValue = value;
        }

        /// <summary>
        /// Translates the current state of the <see cref="MonotonicityHelper{TData, TDataProvider}"/> into a <see cref="Monotonicity"/>.
        /// </summary>
        public Monotonicity ToMonotonicity => new Monotonicity((Flags | Repeat) > 0, (Flags | Increase) > 0, (Flags | Decrease) > 0);
    }

    /// <summary>
    /// Measures basic metrics concerning a sequence.
    /// </summary>
    public class SequenceHelper<TData, TDataProvider>
        where TDataProvider : IDataProvider<TData>
    {
        private const byte Increase = 1;
        private const byte Decrease = 2;
        private const byte Repeat = 4;
        private const byte Empty = 8;

        private byte Flags;

        /// <summary>
        /// The provider.
        /// </summary>
        private readonly TDataProvider Provider;

        /// <summary>
        /// The minimum value observed by the <see cref="SequenceHelper{TData, TDataProvider}"/>.
        /// </summary>
        public TData Minimum { get; private set; }

        /// <summary>
        /// The maximum value observed by the <see cref="SequenceHelper{TData, TDataProvider}"/>.
        /// </summary>
        public TData Maximum { get; private set; }

        private TData Previous;

        /// <summary>
        /// Gets a value indicating whether no values have been observed.
        /// </summary>
        public bool IsEmpty => Flags == Empty;

        /// <summary>
        /// Initialises an instance of <see cref="SequenceHelper{TData, TDataProvider}"/>.
        /// </summary>
        public SequenceHelper(TDataProvider provider)
        {
            Flags = Empty;
            Provider = provider;
            Minimum = Maximum = default(TData);
        }

        /// <summary>
        /// Accepts the next value in the sequence.
        /// </summary>
        /// <param name="value"></param>
        public void Next(TData value)
        {
            if (Flags == Empty)
            {
                Flags = 0;
                Minimum = Maximum = Previous = value;
            }
            else
            {
                var c = Provider.Compare(value, Previous);

                if (c < 0)
                {
                    Flags |= Decrease;
                    if (Provider.Compare(value, Minimum) < 0)
                    {
                        Minimum = value;
                    }
                }
                else if (c > 0)
                {
                    Flags |= Increase;
                    if (Provider.Compare(value, Maximum) > 0)
                    {
                        Maximum = value;
                    }
                }
                else
                {
                    Flags |= Repeat;
                }

                Previous = value;
            }
        }

        /// <summary>
        /// Translates the current state of the <see cref="SequenceHelper{TData, TDataProvider}"/> into a <see cref="ComposableAxis.Monotonicity"/>.
        /// </summary>
        public Monotonicity Monotonicity
        {
            get
            {
                if (IsEmpty)
                    return Monotonicity.Empty;
                else
                    return new Monotonicity((Flags & Repeat) > 0, (Flags & Increase) > 0, (Flags & Decrease) > 0);
            }
        }
    }
}
