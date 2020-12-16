using System;
using System.Collections.Generic;
using System.Text;

namespace OxyPlot.Axes.ComposableAxis
{
    /// <summary>
    /// Band position
    /// </summary>
    public enum BandPosition
    {
        /// <summary>
        /// Inline
        /// </summary>
        Inline,

        /// <summary>
        /// Side
        /// </summary>
        Side,

        /// <summary>
        /// Near
        /// </summary>
        Near,

        /// <summary>
        /// Far
        /// </summary>
        Far,

        /// <summary>
        /// InlineNear
        /// </summary>
        InlineNear,

        /// <summary>
        /// InlineFar
        /// </summary>
        InlineFar
    }

    /// <summary>
    /// Represents a tick or label at a position on an axis
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public struct Tick<TData>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="label"></param>
        public Tick(TData value, string label)
        {
            Value = value;
            Label = label ?? throw new ArgumentNullException(nameof(label));
        }

        /// <summary>
        /// The value representated by this tick.
        /// </summary>
        public TData Value { get; }

        /// <summary>
        /// The label - if any - to attach to this tick.
        /// </summary>
        public string Label { get; }
    }

    /// <summary>
    /// The location of a band.
    /// </summary>
    public struct BandLocation
    {
        /// <summary>
        /// The reference point on the left of the band.
        /// </summary>
        public ScreenPoint Reference { get; }

        /// <summary>
        /// The vector which, along with the <see cref="Reference"/>, describes the ray along which the band exists.
        /// </summary>
        public ScreenVector Parallel { get; }

        /// <summary>
        /// A unit vector normal to the <see cref="Parallel"/>.
        /// </summary>
        public ScreenVector Normal { get; }
    }

    /// <summary>
    /// The excesses of a band.
    /// </summary>
    public struct BandExcesses
    {
        /// <summary>
        /// Initialises the <see cref="BandExcesses"/>.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        public BandExcesses(double left, double top, double right, double bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        /// <summary>
        /// The left excess.
        /// </summary>
        public double Left { get; }

        /// <summary>
        /// The top excess.
        /// </summary>
        public double Top { get; }

        /// <summary>
        /// The right excess.
        /// </summary>
        public double Right { get; }

        /// <summary>
        /// The bottom excess.
        /// </summary>
        public double Bottom { get; }
    }

    /// <summary>
    /// Represents a collection of major ticks
    /// </summary>
    public class DoubleMajorTicks : ITicks<double>
    {
        private List<Tick<double>> _ticks = new List<Tick<double>>();

        private bool _invalidated = true;

        /// <inheritdoc/>
        public IList<Tick<double>> ActualTicks => _ticks;

        /// <summary>
        /// Gets or sets the <see cref="ITickLocator{TData}"/> for this instance.
        /// </summary>
        public ITickLocator<double> TickLocator { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ISpacingOptions{TData}"/> for this instance.
        /// </summary>
        public ISpacingOptions<double> SpacingOptions { get; set; }

        /// <inheritdoc/>
        public void Invalidate()
        {
            _invalidated = true;
        }

        /// <inheritdoc/>
        public void Refresh(IViewInformation<double> viewInformation)
        {
            if (!_invalidated)
                return;

            this._ticks.Clear();
            this.TickLocator.GetTicks(viewInformation.ClipMinimum, viewInformation.ClipMaximum, this.SpacingOptions, this._ticks);
        }
    }

    /// <summary>
    /// A basic Linear tick locator for doubles.
    /// </summary>
    public class LinearDoubleTickLocator : ITickLocator<double>
    {
        /// <summary>
        /// Gets or sets the Tick Offset.
        /// </summary>
        public double Offset { get; set; }

        /// <summary>
        /// Gets or sets the Formatter.
        /// </summary>
        public Func<double, string> Formatter { get; set; }

        /// <summary>
        /// Gets or sets the Format String. Used if <see cref="Formatter"/> is <c>null</c>. Default is <c>"G5"</c>.
        /// </summary>
        public string FormatString { get; set; } = "G5";

        /// <summary>
        /// Formats a tick value.
        /// </summary>
        /// <param name="value">The tick value to format.</param>
        /// <returns></returns>
        public string Format(double value)
        {
            if (Formatter != null)
                return Formatter(value);
            else
                return value.ToString(FormatString);
        }

        /// <inheritdoc/>
        public void GetTicks(double minium, double maximum, ISpacingOptions<double> spacingOptions, IList<Tick<double>> ticks)
        {
            // TODO: proper implementation
            var upperBound = (maximum - minium) / spacingOptions.MaximumTickCount;
            var niceLog = Math.Floor(Math.Log10(upperBound));
            var candidate = Math.Pow(10, niceLog);
            if (candidate * 5 < upperBound)
                candidate *= 5; ;
            if (candidate * 2 < upperBound)
                candidate *= 2;

            var next = Math.Round((minium - Offset) / candidate) * candidate;
            while (next < minium)
                next += candidate;

            do
            {
                ticks.Add(new Tick<double>(next, Format(next)));

                next += candidate;
            }
            while (next < candidate);
        }
    }

    /// <summary>
    /// Contains formatting options for ticks.
    /// </summary>
    public class TickFormatting
    {
        /// <summary>
        /// Initialises an instance of the <see cref="TickFormatting"/> class.
        /// </summary>
        public TickFormatting()
        {
            this.TickLength = 5;
            this.TickStyle = TickStyle.Outside;
            this.TickColor = OxyColors.Automatic;
            this.TickLineStyle = LineStyle.Solid;
        }

        /// <summary>
        /// Gets or sets the length of ticks.
        /// </summary>
        public double TickLength { get; set; }

        /// <summary>
        /// Gets or sets the style of ticks.
        /// </summary>
        public TickStyle TickStyle { get; set; }

        /// <summary>
        /// Gets or sets the color of ticks.
        /// </summary>
        public OxyColor TickColor { get; set; }

        /// <summary>
        /// Gets or sets the line style of ticks.
        /// </summary>
        public LineStyle TickLineStyle { get; set; }
    }

    /// <summary>
    /// Represents a band of axis annotations.
    /// </summary>
    public abstract class BandBase
    {
        /// <summary>
        /// Gets or sets the position of the band.
        /// </summary>
        public BandPosition BandPoisition { get; set; }

        /// <summary>
        /// Gets or sets the tier of the band.
        /// </summary>
        public int BandTier { get; set; }

        /// <summary>
        /// Gets the Band Excessess
        /// </summary>
        public BandExcesses Excesses { get; protected set; }

        /// <summary>
        /// Measures the band, setting the <see cref="Excesses"/> accordingly.
        /// </summary>
        /// <param name="renderContext"></param>
        public abstract void Measure(IRenderContext renderContext);

        /// <summary>
        /// Renders the band.
        /// </summary>
        /// <param name="renderContext"></param>
        /// <param name="location"></param>
        public abstract void Render(IRenderContext renderContext, BandLocation location);
    }

    /// <summary>
    /// Represents a sample.
    /// </summary>
    /// <typeparam name="XData"></typeparam>
    /// <typeparam name="YData"></typeparam>
    public struct DataSample<XData, YData>
    {
        /// <summary>
        /// Creates a sample from values.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public DataSample(XData x, YData y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Gets the X value.
        /// </summary>
        public XData X { get; }

        /// <summary>
        /// Gets the Y value.
        /// </summary>
        public YData Y { get; }
    }

    /// <summary>
    /// Represents a value in Interaction space.
    /// </summary>
    public struct InteractionReal : IComparable<InteractionReal>
    {
        /// <summary>
        /// Initialises a <see cref="InteractionReal"/> with the given value.
        /// </summary>
        /// <param name="value"></param>
        public InteractionReal(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                throw new ArgumentOutOfRangeException(nameof(value), "Value must be finite");

            Value = value;
        }

        /// <summary>
        /// Zero.
        /// </summary>
        public static readonly InteractionReal Zero = new InteractionReal(0);

        /// <summary>
        /// The double value of the <see cref="InteractionReal"/>.
        /// </summary>
        public double Value { get; }

        /// <inheritdoc/>
        public int CompareTo(InteractionReal other)
        {
            return Value.CompareTo(other.Value);
        }

        /// <summary>
        /// Adds two <see cref="InteractionReal"/> values.
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static InteractionReal operator +(InteractionReal l, InteractionReal r)
        {
            return new InteractionReal(l.Value + r.Value);
        }

        /// <summary>
        /// Computes the difference between two <see cref="InteractionReal"/> values.
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static InteractionReal operator -(InteractionReal l, InteractionReal r)
        {
            return new InteractionReal(l.Value - r.Value);
        }

        /// <summary>
        /// Scales a <see cref="InteractionReal"/> by a factor.
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static InteractionReal operator *(InteractionReal l, double r)
        {
            return new InteractionReal(l.Value * r);
        }

        /// <summary>
        /// Scales a <see cref="InteractionReal"/> by a factor.
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static InteractionReal operator /(InteractionReal l, double r)
        {
            return new InteractionReal(l.Value / r);
        }

        /// <summary>
        /// Computes the radio of two <see cref="InteractionReal"/> values.
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static double operator /(InteractionReal l, InteractionReal r)
        {
            return l.Value / r.Value;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is InteractionReal other && other.Value == Value;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <summary>
        /// Compares two <see cref="InteractionReal"/> values for equality.
        /// </summary>
        public static bool operator ==(InteractionReal left, InteractionReal right)
        {
            return left.Value == right.Value;
        }

        /// <summary>
        /// Compares two <see cref="InteractionReal"/> values for inequality.
        /// </summary>
        public static bool operator !=(InteractionReal left, InteractionReal right)
        {
            return !(left == right);
        }
    }

    /// <summary>
    /// Represents a value in Screen space.
    /// </summary>
    public struct ScreenReal : IComparable<ScreenReal>
    {
        /// <summary>
        /// Initialises a <see cref="ScreenReal"/> with the given value.
        /// </summary>
        /// <param name="value"></param>
        public ScreenReal(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                throw new ArgumentOutOfRangeException(nameof(value), "Value must be finite");

            Value = value;
        }

        /// <summary>
        /// Zero.
        /// </summary>
        public static readonly ScreenReal Zero = new ScreenReal(0);

        /// <summary>
        /// The double value of the <see cref="ScreenReal"/>.
        /// </summary>
        public double Value { get; }

        /// <inheritdoc/>
        public int CompareTo(ScreenReal other)
        {
            return Value.CompareTo(other.Value);
        }

        /// <summary>
        /// Adds two <see cref="ScreenReal"/> values.
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static ScreenReal operator +(ScreenReal l, ScreenReal r)
        {
            return new ScreenReal(l.Value + r.Value);
        }

        /// <summary>
        /// Computes the difference between two <see cref="ScreenReal"/> values.
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static ScreenReal operator -(ScreenReal l, ScreenReal r)
        {
            return new ScreenReal(l.Value - r.Value);
        }

        /// <summary>
        /// Scales a <see cref="ScreenReal"/> by a factor.
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static ScreenReal operator *(ScreenReal l, double r)
        {
            return new ScreenReal(l.Value * r);
        }

        /// <summary>
        /// Computes the radio of two <see cref="ScreenReal"/> values.
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static double operator /(ScreenReal l, ScreenReal r)
        {
            return l.Value / r.Value;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is ScreenReal other && other.Value == Value;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <summary>
        /// Compares two <see cref="ScreenReal"/> values for equality.
        /// </summary>
        public static bool operator ==(ScreenReal left, ScreenReal right)
        {
            return left.Value == right.Value;
        }

        /// <summary>
        /// Compares two <see cref="ScreenReal"/> values for inequality.
        /// </summary>
        public static bool operator !=(ScreenReal left, ScreenReal right)
        {
            return !(left == right);
        }
    }

    /// <summary>
    /// Represents a range that may be empty.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct Range<T>
    {
        private bool _isNonEmpty;
        private T _min;
        private T _max;

        /// <summary>
        /// Gets the empty range.
        /// </summary>
        public static readonly Range<T> Empty = default;

        /// <summary>
        /// Initialises a range.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public Range(T min, T max) : this()
        {
            _isNonEmpty = true;
            _min = min;
            _max = max;
        }

        /// <summary>
        /// Initialises a zero-width range.
        /// </summary>
        /// <param name="value"></param>
        public Range(T value)
            : this(value, value)
        {
        }

        /// <summary>
        /// Gets a value indiciating whether the range is empty.
        /// </summary>
        public bool IsEmpty => !_isNonEmpty;
        
        /// <summary>
        /// Tries to get the min and max values.
        /// </summary>
        /// <param name="max"></param>
        /// <param name="min"></param>
        /// <returns><c>true</c> if the range is not empty.</returns>
        public bool TryGetMinMax(out T min, out T max)
        {
            max = _max;
            min = _min;
            return _isNonEmpty;
        }
    }
}
