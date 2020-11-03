using System;
using System.Collections.Generic;
using System.Text;

namespace OxyPlot.Axes.ComposableAxes
{
    /// <summary>
    /// Band position
    /// </summary>
    public enum BandPoisition
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
        Far
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
        /// Gets or sets the tier of the band.
        /// </summary>
        public int BandTier { get; set; }

        /// <summary>
        /// Band Margins
        /// </summary>
        public OxyThickness Margins { get; protected set; } 

        /// <summary>
        /// Measures the band
        /// </summary>
        /// <param name="renderContext"></param>
        public abstract void Measure(IRenderContext renderContext);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="renderContext"></param>
        /// <param name="tierOffset"></param>
        public abstract void Render(IRenderContext renderContext, double tierOffset);
    }
}
