using System;
using System.Collections.Generic;
using System.Text;
using OxyPlot.Axes.ComposableAxis;
using OxyPlot.Series;

namespace OxyPlot.Axes.ComposableAxis.SeriesExamples
{
    /// <summary>
    /// Represents a series of on a pair of X and Y axes.
    /// </summary>
    /// <typeparam name="TSample"></typeparam>
    /// <typeparam name="XData"></typeparam>
    /// <typeparam name="YData"></typeparam>
    /// <typeparam name="TSampleFilter"></typeparam>
    public abstract class XYSeries<TSample, XData, YData, TSampleFilter> : Series.Series
        where TSampleFilter : IFilter<TSample>
    {
        /// <summary>
        /// Initializes an instance of the <see cref="XYSeries{TSample, XData, YData, TSampleFilter}"/> class.
        /// </summary>
        protected XYSeries(TSampleFilter sampleFilter)
        {
            this.SampleFilter = sampleFilter;
            Samples = new List<TSample>();
        }

        /// <summary>
        /// The samples to be used by this series.
        /// </summary>
        public List<TSample> Samples { get; set; }

        /// <summary>
        /// Gets or sets the sample filter.
        /// </summary>
        public TSampleFilter SampleFilter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the tracker can interpolate points.
        /// </summary>
        public bool CanTrackerInterpolatePoints { get; set; }

        /// <summary>
        /// Gets or set the X Axis key
        /// </summary>
        public string XAxisKey { get; set; }

        /// <summary>
        /// Gets or set the X Axis key
        /// </summary>
        public string YAxisKey { get; set; }

        /// <summary>
        /// Gets or sets the X Axis associated with this series.
        /// </summary>
        public IAxis<XData> XAxis { get; private set; }

        /// <summary>
        /// Gets or sets the Y Axis associated with this series.
        /// </summary>
        public IAxis<YData> YAxis { get; private set; }

        /// <summary>
        /// Gets or sets the X/Y Collator for the axes.
        /// </summary>
        protected XYCollator<XData, YData> Collator { get; private set; }

        /// <summary>
        /// Gets or sets the X/Y Render Helpers for the axes.
        /// </summary>
        protected IXYHelper<XData, YData> XYHelper { get; private set; }

        /// <summary>
        /// Gets or sets the minimum X value of any sample.
        /// </summary>
        public XData MinX { get; protected set; }

        /// <summary>
        /// Gets or sets the maximum X value of any sample.
        /// </summary>
        public XData MaxX { get; protected set; }

        /// <summary>
        /// Gets or sets the minimum Y value of any sample.
        /// </summary>
        public YData MinY { get; protected set; }

        /// <summary>
        /// Gets or sets the maximum Y value of any sample.
        /// </summary>
        public YData MaxY { get; protected set; }

        /// <summary>
        /// The monotonicity of the X values.
        /// </summary>
        public Monotonicity XMonotonicity { get; protected set; }

        /// <summary>
        /// The monotonicity of the Y values.
        /// </summary>
        public Monotonicity YMonotonicity { get; protected set; }

        /// <summary>
        /// Gets or sets a value indiciating whether the min/max X/Y and monotonicity properties are meaningful.
        /// </summary>
        public bool HasMeaningfulDataRange { get; protected set; }

        /// <summary>
        /// Resolves axes from the axis keys.
        /// </summary>
        protected virtual void ResolveAxes()
        {
            // should throw if we can't get axes of the right type
            XAxis = (IAxis<XData>)this.PlotModel.GetAxisOrDefault(XAxisKey, this.PlotModel.DefaultXAxis);
            YAxis = (IAxis<YData>)this.PlotModel.GetAxisOrDefault(YAxisKey, this.PlotModel.DefaultYAxis);
            Collator = XYCollator<XData, YData>.Prepare(XAxis, YAxis);
        }

        /// <summary>
        /// Gets an <see cref="IXYHelper{XData, YData}"/> for the current axis, which does not depend on the view state.
        /// </summary>
        /// <returns></returns>
        protected virtual IXYHelper<XData, YData> GetHelper()
        {
            var transpose = XAxis.Position == AxisPosition.Left || XAxis.Position == AxisPosition.Right;
            return XYHelperPreparer<XData, YData>.PrepareHorizontalVertial(Collator, transpose);
        }

        /// <summary>
        /// Gets an <see cref="IAxisScreenValueHelper{XData}"/> for the current X axis.
        /// </summary>
        /// <returns></returns>
        protected virtual IAxisScreenValueHelper<XData> GetXHelper()
        {
            return AxisValueHelperPreparer<XData>.Prepare(XAxis);
        }

        /// <summary>
        /// Gets an <see cref="IAxisScreenValueHelper{YData}"/> for the current X axis.
        /// </summary>
        /// <returns></returns>
        protected virtual IAxisScreenValueHelper<YData> GetYHelper()
        {
            return AxisValueHelperPreparer<YData>.Prepare(YAxis);
        }

        /// <summary>
        /// Gets an <see cref="IXYHelper{XData, YData}"/> for the current axis view state.
        /// </summary>
        /// <returns></returns>
        protected virtual IXYRenderHelper<XData, YData> GetRenderHelper()
        {
            var transpose = XAxis.Position == AxisPosition.Left || XAxis.Position == AxisPosition.Right;
            return XYRenderHelperPreparer<XData, YData>.PrepareHorizontalVertial(Collator, transpose);
        }

        /// <inheritdoc/>
        protected internal override bool AreAxesRequired()
        {
            return true;
        }

        /// <inheritdoc/>
        protected internal override void EnsureAxes()
        {
            ResolveAxes();
        }

        /// <inheritdoc/>
        protected internal override bool IsUsing(Axis axis)
        {
            return this.XAxis == axis || this.YAxis == axis;
        }

        /// <inheritdoc/>
        protected internal override void UpdateAxisMaxMin()
        {
            this.XAxis.Include(this.MinX);
            this.XAxis.Include(this.MaxX);
            this.YAxis.Include(this.MinY);
            this.YAxis.Include(this.MaxY);
        }

        /// <inheritdoc/>
        public override OxyRect GetClippingRect()
        {
            var xrect = new OxyRect(XAxis.ScreenMin, XAxis.ScreenMax);
            var yrect = new OxyRect(YAxis.ScreenMin, YAxis.ScreenMax);
            return xrect.Intersect(yrect);
        }
    }

    /// <summary>
    /// Represents a series of XY Samples.
    /// </summary>
    /// <typeparam name="TSample"></typeparam>
    /// <typeparam name="XData"></typeparam>
    /// <typeparam name="YData"></typeparam>
    /// <typeparam name="TSampleProvider"></typeparam>
    /// <typeparam name="TSampleFilter"></typeparam>
    public abstract class XYSampleSeries<TSample, XData, YData, TSampleProvider, TSampleFilter> : XYSeries<TSample, XData, YData, TSampleFilter> // TODO: we want these transposable, but we can't implement ITransposablePlotElement at the moment
        where TSampleProvider : IXYSampleProvider<TSample, XData, YData>
        where TSampleFilter : IFilter<TSample>
    {
        /// <summary>
        /// Initializes an instance of the <see cref="XYSampleSeries{TSample, XData, YData, TSampleProvider, TSampleFilter}"/> class.
        /// </summary>
        protected XYSampleSeries(TSampleProvider sampleProvider, TSampleFilter sampleFilter)
             : base(sampleFilter)
        {
            this.SampleProvider = sampleProvider;
            CanTrackerInterpolatePoints = false;
        }

        /// <summary>
        /// Gets or sets the sample provider.
        /// </summary>
        public TSampleProvider SampleProvider { get; }

        /// <summary>
        /// Finds a window of sample that are within the clip bounds if the data are monotonic.
        /// </summary>
        /// <param name="startIndex">The first index, inclusive.</param>
        /// <param name="endIndex">The last index, inclusive.</param>
        protected void FindWindow(out int startIndex, out int endIndex)
        {
            var xyRenderHelper = GetRenderHelper();

            var xtransformation = xyRenderHelper.XTransformation;
            var ytransformation = xyRenderHelper.YTransformation;

            if (XMonotonicity.IsMonotone || YMonotonicity.IsMonotone)
            {
                var minSample = new DataSample<XData, YData>(xtransformation.ClipMinimum, ytransformation.ClipMinimum);
                var maxSample = new DataSample<XData, YData>(xtransformation.ClipMaximum, ytransformation.ClipMaximum);
                xyRenderHelper.FindWindow(SampleProvider, SampleFilter, Samples.AsReadOnlyList(), minSample, maxSample, XMonotonicity, YMonotonicity, out startIndex, out endIndex);
            }
            else
            {
                startIndex = 0;
                endIndex = Samples.Count - 1;
            }
        }

        /// <inheritdoc/>
        protected internal override void UpdateData()
        {
            // We don't have an ItemSource, so we have nothing to do here
            // If we do add an ItemSource (e.g. for convience, that maps to an XYSample`2) then we update the actual samples collection here
        }

        /// <summary>
        /// Updates the minimum and maximum values
        /// </summary>
        protected virtual void UpdateMinMax()
        {
            if (Samples.Count == 0)
                return; // bail before we crash

            ResolveAxes();
            var xyHelper = GetHelper();

            HasMeaningfulDataRange = xyHelper.FindMinMax(SampleProvider, SampleFilter, Samples.AsReadOnlyList(), out var minX, out var minY, out var maxX, out var maxY, out var xm, out var ym);
            MinX = minX;
            MinY = minY;
            MaxX = maxX;
            MaxY = maxY;
            XMonotonicity = xm;
            YMonotonicity = ym;
        }

        /// <inheritdoc/>
        protected internal override void UpdateMaxMin()
        {
            this.UpdateMinMax();
        }

        /// <summary>
        /// Gets the point on the series that is nearest the specified point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="interpolate">Interpolate the series if this flag is set to <c>true</c>.</param>
        /// <returns>A TrackerHitResult for the current hit.</returns>
        public override TrackerHitResult GetNearestPoint(ScreenPoint point, bool interpolate)
        {
            var xyRenderHelper = GetRenderHelper();

            this.FindWindow(out var startIdx, out var endIdx);

            if (xyRenderHelper.TryFindNearest(SampleProvider, Samples.AsReadOnlyList(), point, startIdx, endIdx, CanTrackerInterpolatePoints & interpolate, out var nearest, out var distance))
            {
                var sample = Samples[nearest];
                var xySample = SampleProvider.Sample(sample);
                var position = xyRenderHelper.TransformSample(xySample);

                var result = new TrackerHitResult()
                {
                    Item = sample,
                    Index = nearest,
                    PlotModel = this.PlotModel,
                    LineExtents = this.GetClippingRect(),
                    Position = position,
                    Series = this,
                    Text = Samples[nearest].ToString(), // TODO: proper tracker string
                    // NOTE: the typed axes do not have a concept of value formatting: should they?
                    // I was planning to leave formatting to the bands, but I guess that will never be good enough for compat...
                    // Yeah... need to keep all the old properties; bands can default to them, etc.
                };

                return result;
            }
            else
            {
                // no points found
                return null;
            }
        }
    }

    /// <summary>
    /// A line series.
    /// </summary>
    /// <typeparam name="TSample"></typeparam>
    /// <typeparam name="XData"></typeparam>
    /// <typeparam name="YData"></typeparam>
    /// <typeparam name="TSampleProvider"></typeparam>
    /// <typeparam name="TSampleFilter"></typeparam>
    public class LineSeries<TSample, XData, YData, TSampleProvider, TSampleFilter> : XYSampleSeries<TSample, XData, YData, TSampleProvider, TSampleFilter>
        where TSampleProvider : IXYSampleProvider<TSample, XData, YData>
        where TSampleFilter : IFilter<TSample>
    {
        /// <summary>
        /// The output buffer for continuous line segments.
        /// </summary>
        private List<ScreenPoint> continuousBuffer;

        /// <summary>
        /// The output buffer for broken line segments.
        /// </summary>
        private List<ScreenPoint> brokenBuffer;

        /// <summary>
        /// The buffer for decimated points.
        /// </summary>
        private List<ScreenPoint> decimatorBuffer;

        /// <summary>
        /// Not sure what this is for yet.
        /// </summary>
        private List<ScreenPoint> outputBuffer;

        /// <summary>
        /// The default color.
        /// </summary>
        private OxyColor defaultColor;

        /// <summary>
        /// The default marker fill color.
        /// </summary>
        private OxyColor defaultMarkerFill;

        /// <summary>
        /// The default line style.
        /// </summary>
        private LineStyle defaultLineStyle;

        /// <summary>
        /// The smoothed points.
        /// </summary>
        private List<DataPoint> smoothedPoints;

        /// <summary>
        /// Initializes a new instance of the <see cref="LineSeries{TSample, XData, YData, TSampleProvider, TSampleFilter}" /> class.
        /// </summary>
        public LineSeries(TSampleProvider sampleProvider, TSampleFilter sampleFilter)
            : base(sampleProvider, sampleFilter)
        {
            this.defaultColor = OxyColors.Undefined;
            this.defaultMarkerFill = OxyColors.Undefined;
            this.defaultLineStyle = LineStyle.Automatic;

            this.StrokeThickness = 2;
            this.LineJoin = LineJoin.Bevel;
            this.LineStyle = LineStyle.Automatic;

            this.Color = OxyColors.Automatic;
            this.BrokenLineColor = OxyColors.Undefined;

            this.MarkerFill = OxyColors.Automatic;
            this.MarkerStroke = OxyColors.Automatic;
            this.MarkerResolution = 0;
            this.MarkerSize = 3;
            this.MarkerStrokeThickness = 1;
            this.MarkerType = MarkerType.None;

            this.MinimumSegmentLength = 2;

            this.CanTrackerInterpolatePoints = true;
            this.LabelMargin = 6;
            this.smoothedPoints = new List<DataPoint>();
            SampleFilter = sampleFilter;
        }

        /// <summary>
        /// Gets or sets the color of the curve.
        /// </summary>
        /// <value>The color.</value>
        public OxyColor Color { get; set; }

        /// <summary>
        /// Gets or sets the color of the broken line segments. The default is <see cref="OxyColors.Undefined"/>. Set it to <see cref="OxyColors.Automatic"/> if it should follow the <see cref="Color" />.
        /// </summary>
        /// <remarks>Add <c>DataPoint.Undefined</c> in the Points collection to create breaks in the line.</remarks>
        public OxyColor BrokenLineColor { get; set; }

        /// <summary>
        /// Gets or sets the broken line style. The default is <see cref="OxyPlot.LineStyle.Solid" />.
        /// </summary>
        public LineStyle BrokenLineStyle { get; set; }

        /// <summary>
        /// Gets or sets the broken line thickness. The default is <c>0</c> (no line).
        /// </summary>
        public double BrokenLineThickness { get; set; }

        /// <summary>
        /// Gets or sets the dash array for the rendered line (overrides <see cref="LineStyle" />). The default is <c>null</c>.
        /// </summary>
        /// <value>The dash array.</value>
        /// <remarks>If this is not <c>null</c> it overrides the <see cref="LineStyle" /> property.</remarks>
        public double[] Dashes { get; set; }

        /// <summary>
        /// Gets or sets the decimator.
        /// </summary>
        /// <value>
        /// The decimator action.
        /// </value>
        /// <remarks>The decimator can be used to improve the performance of the rendering. See the example.</remarks>
        public Action<List<ScreenPoint>, List<ScreenPoint>> Decimator { get; set; }

        /// <summary>
        /// Gets or sets the label format string. The default is <c>null</c> (no labels).
        /// </summary>
        /// <value>The label format string.</value>
        public string LabelFormatString { get; set; }

        /// <summary>
        /// Gets or sets the label margins. The default is <c>6</c>.
        /// </summary>
        public double LabelMargin { get; set; }

        /// <summary>
        /// Gets or sets the line join. The default is <see cref="OxyPlot.LineJoin.Bevel" />.
        /// </summary>
        /// <value>The line join.</value>
        public LineJoin LineJoin { get; set; }

        /// <summary>
        /// Gets or sets the line style. The default is <see cref="OxyPlot.LineStyle.Automatic" />.
        /// </summary>
        /// <value>The line style.</value>
        public LineStyle LineStyle { get; set; }

        /// <summary>
        /// Gets or sets a value specifying the position of a legend rendered on the line. The default is <c>LineLegendPosition.None</c>.
        /// </summary>
        /// <value>A value specifying the position of the legend.</value>
        public LineLegendPosition LineLegendPosition { get; set; }

        /// <summary>
        /// Gets or sets the marker fill color. The default is <see cref="OxyColors.Automatic" />.
        /// </summary>
        /// <value>The marker fill.</value>
        public OxyColor MarkerFill { get; set; }

        /// <summary>
        /// Gets or sets the a custom polygon outline for the markers. Set <see cref="MarkerType" /> to <see cref="OxyPlot.MarkerType.Custom" /> to use this property. The default is <c>null</c>.
        /// </summary>
        /// <value>A polyline.</value>
        public ScreenPoint[] MarkerOutline { get; set; }

        /// <summary>
        /// Gets or sets the marker resolution. The default is <c>0</c>.
        /// </summary>
        /// <value>The marker resolution.</value>
        public int MarkerResolution { get; set; }

        /// <summary>
        /// Gets or sets the size of the marker. The default is <c>3</c>.
        /// </summary>
        /// <value>The size of the marker.</value>
        public double MarkerSize { get; set; }

        /// <summary>
        /// Gets or sets the marker stroke. The default is <c>OxyColors.Automatic</c>.
        /// </summary>
        /// <value>The marker stroke.</value>
        public OxyColor MarkerStroke { get; set; }

        /// <summary>
        /// Gets or sets the marker stroke thickness. The default is <c>2</c>.
        /// </summary>
        /// <value>The marker stroke thickness.</value>
        public double MarkerStrokeThickness { get; set; }

        /// <summary>
        /// Gets or sets the type of the marker. The default is <c>MarkerType.None</c>.
        /// </summary>
        /// <value>The type of the marker.</value>
        /// <remarks>If MarkerType.Custom is used, the MarkerOutline property must be specified.</remarks>
        public MarkerType MarkerType { get; set; }

        /// <summary>
        /// Gets or sets the minimum length of the segment.
        /// Increasing this number will increase performance,
        /// but make the curve less accurate. The default is <c>2</c>.
        /// </summary>
        /// <value>The minimum length of the segment.</value>
        public double MinimumSegmentLength { get; set; }

        /// <summary>
        /// Gets or sets a type of interpolation algorithm used for smoothing this <see cref="LineSeries{TSample, XData, YData, TSampleProvider, TSampleFilter}" />.
        /// </summary>
        /// <value>Type of interpolation algorithm.</value>
        public IInterpolationAlgorithm InterpolationAlgorithm { get; set; }

        /// <summary>
        /// Gets or sets the thickness of the curve.
        /// </summary>
        /// <value>The stroke thickness.</value>
        public double StrokeThickness { get; set; }

        /// <summary>
        /// Gets the actual color.
        /// </summary>
        /// <value>The actual color.</value>
        public OxyColor ActualColor
        {
            get
            {
                return this.Color.GetActualColor(this.defaultColor);
            }
        }

        /// <summary>
        /// Gets the actual marker fill color.
        /// </summary>
        /// <value>The actual color.</value>
        public OxyColor ActualMarkerFill
        {
            get
            {
                return this.MarkerFill.GetActualColor(this.defaultMarkerFill);
            }
        }

        /// <summary>
        /// Gets the actual line style.
        /// </summary>
        /// <value>The actual line style.</value>
        protected LineStyle ActualLineStyle
        {
            get
            {
                return this.LineStyle != LineStyle.Automatic ? this.LineStyle : this.defaultLineStyle;
            }
        }

        /// <summary>
        /// Gets the actual dash array for the line.
        /// </summary>
        protected double[] ActualDashArray
        {
            get
            {
                return this.Dashes ?? this.ActualLineStyle.GetDashArray();
            }
        }

        private static void ClearOrCreate<T>(ref List<T> list, int? initialSize = null)
        {
            if (list == null)
            {
                if (initialSize.HasValue)
                {
                    list = new List<T>(initialSize.Value);
                }
                else
                {
                    list = new List<T>();
                }
            }
            else
            {
                list.Clear();
            }
        }

        /// <inheritdoc/>
        public override void Render(IRenderContext rc)
        {
            if (Samples.Count == 0)
                return; // bail before we crash

            ResolveAxes();
            var xyRenderHelper = GetRenderHelper();

            var areBrokenLinesRendered = this.BrokenLineThickness > 0 && this.BrokenLineStyle != LineStyle.None;
            var dashArray = areBrokenLinesRendered ? this.BrokenLineStyle.GetDashArray() : null;
            var broken = areBrokenLinesRendered ? new List<ScreenPoint>(2) : null;

            brokenBuffer = brokenBuffer ?? new List<ScreenPoint>();
            continuousBuffer = continuousBuffer ?? new List<ScreenPoint>();

            ScreenPoint? lp = null;
            XYClipInfo lci = default;

            base.FindWindow(out var startIdx, out var endIdx);

            int sampleIdx = startIdx;

            var clipRect = this.GetClippingRect();

            rc.PopClip();

            // inflate the clip rect so that in includes the stroke thickness
            var adjustedClipRect = clipRect.Inflate(new OxyThickness(this.StrokeThickness));

            while (xyRenderHelper.ExtractNextContinuousLineSegment<TSample, TSampleProvider, TSampleFilter, RectangleClipFilter>(SampleProvider, SampleFilter, new RectangleClipFilter(adjustedClipRect), Samples.AsReadOnlyList(), ref sampleIdx, endIdx, ref lp, ref lci, brokenBuffer, continuousBuffer))
            {
                if (areBrokenLinesRendered)
                {
                    if (broken.Count > 0)
                    {
                        var actualBrokenLineColor = this.BrokenLineColor.IsAutomatic()
                            ? this.ActualColor
                            : this.BrokenLineColor;

                        rc.DrawLineSegments(
                            broken,
                            actualBrokenLineColor,
                            this.BrokenLineThickness,
                            this.EdgeRenderingMode,
                            dashArray,
                            this.LineJoin);
                        broken.Clear();
                    }
                }

                if (this.Decimator != null)
                {
                    if (this.decimatorBuffer == null)
                    {
                        this.decimatorBuffer = new List<ScreenPoint>(this.continuousBuffer.Count);
                    }
                    else
                    {
                        this.decimatorBuffer.Clear();
                    }

                    this.Decimator(this.continuousBuffer, this.decimatorBuffer);
                    this.RenderLineAndMarkers(rc, xyRenderHelper, this.decimatorBuffer);
                }
                else
                {
                    this.RenderLineAndMarkers(rc, xyRenderHelper, this.continuousBuffer);
                }

                this.continuousBuffer.Clear();
            }

            rc.PushClip(clipRect);

        }

        /// <summary>
        /// Renders the transformed points as a line (smoothed if <see cref="InterpolationAlgorithm"/> isn’t <c>null</c>) and markers (if <see cref="MarkerType"/> is not <c>None</c>).
        /// </summary>
        /// <param name="rc">The render context.</param>
        /// <param name="renderHelper"></param>
        /// <param name="pointsToRender">The points to render.</param>
        protected virtual void RenderLineAndMarkers(IRenderContext rc, IXYRenderHelper<XData, YData> renderHelper, IList<ScreenPoint> pointsToRender)
        {
            var screenPoints = pointsToRender;
            if (this.InterpolationAlgorithm != null)
            {
                // spline smoothing (should only be used on small datasets)
                var resampledPoints = ScreenPointHelper.ResamplePoints(pointsToRender, this.MinimumSegmentLength);
                screenPoints = this.InterpolationAlgorithm.CreateSpline(resampledPoints, false, 0.25);
            }

            // clip the line segments with the clipping rectangle
            if (this.StrokeThickness > 0 && this.ActualLineStyle != LineStyle.None)
            {
                this.RenderLine(rc, screenPoints);
            }

            if (this.MarkerType != MarkerType.None)
            {
                var markerBinOffset = this.MarkerResolution > 0 ? renderHelper.TransformSample(new DataSample<XData, YData>(this.MinX, this.MaxY)) : default(ScreenPoint);

                rc.DrawMarkers(
                    pointsToRender,
                    this.MarkerType,
                    this.MarkerOutline,
                    new[] { this.MarkerSize },
                    this.ActualMarkerFill,
                    this.MarkerStroke,
                    this.MarkerStrokeThickness,
                    this.EdgeRenderingMode,
                    this.MarkerResolution,
                    markerBinOffset);
            }
        }

        /// <summary>
        /// Renders a continuous line.
        /// </summary>
        /// <param name="rc">The render context.</param>
        /// <param name="pointsToRender">The points to render.</param>
        protected virtual void RenderLine(IRenderContext rc, IList<ScreenPoint> pointsToRender)
        {
            var dashArray = this.ActualDashArray;

            if (this.outputBuffer == null)
            {
                this.outputBuffer = new List<ScreenPoint>(pointsToRender.Count);
            }

            rc.DrawReducedLine(
                pointsToRender,
                this.MinimumSegmentLength * this.MinimumSegmentLength,
                this.GetSelectableColor(this.ActualColor),
                this.StrokeThickness,
                this.EdgeRenderingMode,
                dashArray,
                this.LineJoin,
                this.outputBuffer);
        }

        /// <inheritdoc/>
        public override void RenderLegend(IRenderContext rc, OxyRect legendBox)
        {
            var points = new[] { new ScreenPoint(legendBox.Left, legendBox.Center.Y), new ScreenPoint(legendBox.Right, legendBox.Center.Y) };
            rc.DrawLine(points, this.ActualColor, StrokeThickness, EdgeRenderingMode, ActualDashArray, LineJoin);
        }

        /// <inheritdoc/>
        protected internal override void SetDefaultValues()
        {
            if (this.Color.IsAutomatic())
            {
                this.defaultColor = this.PlotModel.GetDefaultColor();
            }

            if (this.LineStyle == LineStyle.Automatic)
            {
                this.defaultLineStyle = this.PlotModel.GetDefaultLineStyle();
            }
        }
    }
}
