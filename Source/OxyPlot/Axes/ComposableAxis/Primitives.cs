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
    /// Represents a discontenuity in a data-space.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    [Obsolete("Better to let the Transformations check on a case by case basis, I think: may be worth adding a Discontenuity Context to support either method")]
    public struct Discontenuity<TData>
    {
        /// <summary>
        /// Inializes a <see cref="Discontenuity{TData}"/> from a pair of start and end values.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public Discontenuity(TData start, TData end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// Gets the start of the discontenuity.
        /// </summary>
        public TData Start { get; }

        /// <summary>
        /// Gets the start of the discontenuity.
        /// </summary>
        public TData End { get; }
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
}
