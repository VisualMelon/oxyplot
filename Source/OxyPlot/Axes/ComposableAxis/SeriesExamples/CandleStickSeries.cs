using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OxyPlot.Axes.ComposableAxis.SeriesExamples
{    /// <summary>
     /// Represents an item in a <see cref="HighLowSeries" />.
     /// </summary>
    public class HighLowItem<XData, YData> : ICodeGenerating
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HighLowItem" /> class.
        /// </summary>
        public HighLowItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HighLowItem"/> class.
        /// </summary>
        /// <param name="x">
        /// The x value.
        /// </param>
        /// <param name="high">
        /// The high value.
        /// </param>
        /// <param name="low">
        /// The low value.
        /// </param>
        /// <param name="open">
        /// The open value.
        /// </param>
        /// <param name="close">
        /// The close value.
        /// </param>
        public HighLowItem(XData x, YData high, YData low, YData open, YData close)
        {
            this.X = x;
            this.High = high;
            this.Low = low;
            this.Open = open;
            this.Close = close;
        }

        /// <summary>
        /// Gets or sets the close value.
        /// </summary>
        /// <value>The close value.</value>
        public YData Close { get; set; }

        /// <summary>
        /// Gets or sets the high value.
        /// </summary>
        /// <value>The high value.</value>
        public YData High { get; set; }

        /// <summary>
        /// Gets or sets the low value.
        /// </summary>
        /// <value>The low value.</value>
        public YData Low { get; set; }

        /// <summary>
        /// Gets or sets the open value.
        /// </summary>
        /// <value>The open value.</value>
        public YData Open { get; set; }

        /// <summary>
        /// Gets or sets the X value (time).
        /// </summary>
        /// <value>The X value.</value>
        public XData X { get; set; }

        /// <summary>
        /// Returns C# code that generates this instance.
        /// </summary>
        /// <returns>The C# code.</returns>
        public string ToCode()
        {
            return CodeGenerator.FormatConstructor(
                this.GetType(), "{0},{1},{2},{3},{4}", this.X, this.High, this.Low, this.Open, this.Close);
        }
    }

    /// <summary>
    /// Provides access to the <see cref="HighLowItem{XData, YData}.X"/> property.
    /// </summary>
    /// <typeparam name="XData"></typeparam>
    /// <typeparam name="YData"></typeparam>
    public readonly struct HighLowItemXProvider<XData, YData> : IValueSampler<HighLowItem<XData, YData>, XData>
    {
        /// <inheritdoc/>
        public bool IsInvalid(HighLowItem<XData, YData> sample)
        {
            return false;
        }

        /// <inheritdoc/>
        public XData Sample(HighLowItem<XData, YData> sample)
        {
            return sample.X;
        }

        /// <inheritdoc/>
        public bool TrySample(HighLowItem<XData, YData> sample, out XData result)
        {
            result = sample.X;
            return true;
        }
    }

    /// <summary>
    /// Represents a series of <see cref="HighLowItem"/>.
    /// </summary>
    /// <typeparam name="TSample"></typeparam>
    /// <typeparam name="XData"></typeparam>
    /// <typeparam name="YData"></typeparam>
    /// <typeparam name="TSampleProvider"></typeparam>
    /// <typeparam name="TSampleFilter"></typeparam>
    public abstract class HighLowSeries<TSample, XData, YData, TSampleProvider, TSampleFilter> : XYSeries<TSample, XData, YData, TSampleFilter>
        where TSampleProvider : IValueSampler<TSample, HighLowItem<XData, YData>>
        where TSampleFilter : IFilter<TSample>
    {
        /// <summary>
        /// Initialises an instance of the <see cref="HighLowSeries{TSample, XData, YData, TSampleProvider, TSampleFilter}"/> class.
        /// </summary>
        /// <param name="sampleProvider"></param>
        /// <param name="filter"></param>
        protected HighLowSeries(TSampleProvider sampleProvider, TSampleFilter filter)
            : base(filter)
        {
            SampleProvider = sampleProvider;
        }

        /// <summary>
        /// Gets the sample provider.
        /// </summary>
        public TSampleProvider SampleProvider { get; }

        /// <inheritdoc/>
        protected internal override void UpdateMaxMin()
        {
            var xHelper = this.GetXHelper();
            var yHelper = this.GetYHelper();

            var xProvider = new DelegateValueProvider<TSample, XData>(s => SampleProvider.Sample(s).X);
            var lowProvider = new DelegateValueProvider<TSample, YData>(s => SampleProvider.Sample(s).Low);
            var highProvider = new DelegateValueProvider<TSample, YData>(s => SampleProvider.Sample(s).High);

            xHelper.FindMinMax(xProvider, this.SampleFilter, this.Samples.AsReadOnlyList(), out var minX, out var maxX, out var mx);

            this.MinX = minX;
            this.MaxX = maxX;
            this.XMonotonicity = mx;

            yHelper.FindMinMax(lowProvider, this.SampleFilter, this.Samples.AsReadOnlyList(), out var minLow, out var maxLow);
            yHelper.FindMinMax(highProvider, this.SampleFilter, this.Samples.AsReadOnlyList(), out var minHigh, out var maxHigh);

            this.MinY = DataHelpers.Min(yHelper.VProvider, minLow, minHigh);
            this.MaxY = DataHelpers.Max(yHelper.VProvider, maxLow, maxHigh);
            this.YMonotonicity = Monotonicity.None;
        }

        /// <summary>
        /// Finds a window of sample that are within the clip bounds if the data are monotonic.
        /// </summary>
        /// <param name="startIndex">The first index, inclusive.</param>
        /// <param name="endIndex">The last index, inclusive.</param>
        protected void FindWindow(out int startIndex, out int endIndex)
        {
            var xHelper = this.GetXHelper();
            var xtransformation = xHelper.Transformation;
            var xProvider = new DelegateValueProvider<TSample, XData>(s => SampleProvider.Sample(s).X);

            if (XMonotonicity.IsMonotone)
            {
                var minSample = xtransformation.ClipMinimum;
                var maxSample = xtransformation.ClipMaximum;
                xHelper.FindWindow(xProvider, SampleFilter, Samples.AsReadOnlyList(), minSample, maxSample, XMonotonicity, out startIndex, out endIndex);
            }
            else
            {
                startIndex = 0;
                endIndex = Samples.Count - 1;
            }
        }
    }

    /// <summary>
    /// Represents a CandleStickSeries
    /// </summary>
    /// <typeparam name="TSample"></typeparam>
    /// <typeparam name="XData"></typeparam>
    /// <typeparam name="YData"></typeparam>
    /// <typeparam name="TSampleProvider"></typeparam>
    /// <typeparam name="TSampleFilter"></typeparam>
    public class CandleStickSeries<TSample, XData, YData, TSampleProvider, TSampleFilter> : HighLowSeries<TSample, XData, YData, TSampleProvider, TSampleFilter>
        where TSampleProvider : IValueSampler<TSample, HighLowItem<XData, YData>>
        where TSampleFilter : IFilter<TSample>
    {
        /// <summary>
        /// Gets or sets the color of the item.
        /// </summary>
        /// <value>The color.</value>
        public OxyColor Color { get; set; }

        /// <summary>
        /// Gets the actual color of the item.
        /// </summary>
        /// <value>The actual color.</value>
        public OxyColor ActualColor
        {
            get { return this.Color.GetActualColor(this.defaultColor); }
        }

        /// <summary>
        /// The default color.
        /// </summary>
        private OxyColor defaultColor;

        /// <summary>
        /// Gets or sets the line join.
        /// </summary>
        /// <value>The line join.</value>
        public LineJoin LineJoin { get; set; }

        /// <summary>
        /// Gets or sets the line style.
        /// </summary>
        /// <value>The line style.</value>
        public LineStyle LineStyle { get; set; }

        /// <summary>
        /// Gets or sets the thickness of the curve.
        /// </summary>
        /// <value>The stroke thickness.</value>
        public double StrokeThickness { get; set; }

        /// <summary>
        /// Gets or sets the color used when the closing value is greater than opening value.
        /// </summary>
        public OxyColor IncreasingColor { get; set; }

        /// <summary>
        /// Gets or sets the fill color used when the closing value is less than opening value.
        /// </summary>
        public OxyColor DecreasingColor { get; set; }

        /// <summary>
        /// Gets or sets the bar width in interaction units.
        /// You can obtain a meaningful value from a linear <see cref="IDataTransformation{TData, TDataProvider}"/>.
        /// For example, <see cref="LinearDateTime.Scale(TimeSpan)"/> will translate a scale into a suitable <see cref="InteractionReal"/>.
        /// If <see cref="CandleWidth"/> is <c>null</c>, then a default value will be computed from the data.
        /// </summary>
        public InteractionReal? CandleWidth { get; set; }

        /// <summary>
        /// Initialises an instance of the <see cref="CandleStickSeries{TSample, XData, YData, TSampleProvider, TSampleFilter}"/> class.
        /// </summary>
        /// <param name="samplerProvider"></param>
        /// <param name="filter"></param>
        public CandleStickSeries(TSampleProvider samplerProvider, TSampleFilter filter)
            : base(samplerProvider, filter)
        {
            this.LineStyle = LineStyle.Automatic;
            this.StrokeThickness = 1;
            this.LineJoin = LineJoin.Miter;
            this.IncreasingColor = OxyColors.DarkGreen;
            this.DecreasingColor = OxyColors.Red;
            this.CandleWidth = null;
        }

        /// <inheritdoc/>
        public override void Render(IRenderContext rc)
        {
            if (Samples.Count == 0)
            {
                return;
            }

            ResolveAxes();

            var xyRenderHelper = this.GetRenderHelper();
            var xHelper = this.GetXHelper();
            var yHelper = this.GetYHelper();
            var yProvider = yHelper.VProvider;

            this.FindWindow(out int startIndex, out int endIndex);
            startIndex = Math.Max(0, startIndex - 1);
            endIndex = Math.Min(Samples.Count - 1, endIndex + 1);

            var fillUp = this.GetSelectableFillColor(this.IncreasingColor);
            var fillDown = this.GetSelectableFillColor(this.DecreasingColor);
            var lineUp = this.GetSelectableColor(this.IncreasingColor.ChangeIntensity(0.70));
            var lineDown = this.GetSelectableColor(this.DecreasingColor.ChangeIntensity(0.70));

            var dashArray = this.LineStyle.GetDashArray();

            double barWidth;

            if (CandleWidth.HasValue)
            {
                barWidth = XAxis.ViewInfo.Scale(CandleWidth.Value).Value;
            }
            else if (XMonotonicity.IsMonotone)
            {
                double mingap = double.MaxValue;

                double? previous = null;

                for (int i = startIndex; i < endIndex; i++)
                {
                    var s = Samples[i];
                    if (SampleFilter.Filter(s)
                        && SampleProvider.TrySample(s, out var bar))
                    {
                        var current = xHelper.Transformation.Transform(bar.X).Value;

                        if (previous.HasValue)
                        {
                            mingap = Math.Min(mingap, Math.Abs(current - previous.Value));
                        }

                        previous = current;
                    }
                }

                barWidth = mingap * 0.8;
            }
            else
            {
                barWidth = 10; // whatever
            }

            var halfOffset = XAxis.Position == AxisPosition.Bottom || XAxis.Position == AxisPosition.Top
                ? new ScreenVector(barWidth / 2, 0)
                : new ScreenVector(0, barWidth / 2);

            // mindless generousness
            for (int i = startIndex; i <= endIndex; i++)
            {
                var s = Samples[i];
                if (SampleFilter.Filter(s)
                    && SampleProvider.TrySample(s, out var bar))
                {
                    var up = yProvider.Compare(bar.Close, bar.Open) >= 0;

                    var fillColor = up ? fillUp : fillDown;
                    var lineColor = up ? lineUp : lineDown;

                    var max = xyRenderHelper.TransformSample(new DataSample<XData, YData>(bar.X, bar.High));
                    var min = xyRenderHelper.TransformSample(new DataSample<XData, YData>(bar.X, bar.Low));
                    var high = xyRenderHelper.TransformSample(new DataSample<XData, YData>(bar.X, up ? bar.Close : bar.Open));
                    var low = xyRenderHelper.TransformSample(new DataSample<XData, YData>(bar.X, up ? bar.Open : bar.Close));

                    if (this.StrokeThickness > 0 && this.LineStyle != LineStyle.None)
                    {
                        // Upper extent
                        rc.DrawLine(
                            new[] { high, max },
                            lineColor,
                            this.StrokeThickness,
                            this.EdgeRenderingMode,
                            dashArray,
                            this.LineJoin);

                        // Lower extent
                        rc.DrawLine(
                            new[] { min, low },
                            lineColor,
                            this.StrokeThickness,
                            this.EdgeRenderingMode,
                            dashArray,
                            this.LineJoin);
                    }

                    var p1 = high - halfOffset;
                    var p2 = low + halfOffset;
                    var rect = new OxyRect(p1, p2);
                    rc.DrawRectangle(rect, fillColor, lineColor, this.StrokeThickness, this.EdgeRenderingMode);
                }
            }
        }

        /// <inheritdoc/>
        public override void RenderLegend(IRenderContext rc, OxyRect legendBox)
        {
            // C&P'd from classic CandleStickSeries
            double xmid = (legendBox.Left + legendBox.Right) / 2;
            double yopen = legendBox.Top + ((legendBox.Bottom - legendBox.Top) * 0.7);
            double yclose = legendBox.Top + ((legendBox.Bottom - legendBox.Top) * 0.3);
            double[] dashArray = this.LineStyle.GetDashArray();

            var candlewidth = legendBox.Width * 0.75;

            if (this.StrokeThickness > 0 && this.LineStyle != LineStyle.None)
            {
                rc.DrawLine(
                    new[] { new ScreenPoint(xmid, legendBox.Top), new ScreenPoint(xmid, legendBox.Bottom) },
                    this.GetSelectableColor(this.ActualColor),
                    this.StrokeThickness,
                    this.EdgeRenderingMode,
                    dashArray,
                    LineJoin.Miter);
            }

            rc.DrawRectangle(
                new OxyRect(xmid - (candlewidth * 0.5), yclose, candlewidth, yopen - yclose),
                this.GetSelectableFillColor(this.IncreasingColor),
                this.GetSelectableColor(this.ActualColor),
                this.StrokeThickness,
                this.EdgeRenderingMode);
        }

        /// <inheritdoc/>
        protected internal override void SetDefaultValues()
        {
            if (this.Color.IsAutomatic())
            {
                this.LineStyle = this.PlotModel.GetDefaultLineStyle();
                this.defaultColor = this.PlotModel.GetDefaultColor();
            }
        }

        /// <inheritdoc/>
        protected internal override void UpdateData()
        {
            // nix
        }
    }
}
