using System;
using System.Collections.Generic;
using System.Text;
using OxyPlot.Axes.ComposableAxis;
using OxyPlot.Series;

namespace OxyPlot.Axes.ComposableAxis.SeriesExamples
{
    /// <summary>
    /// Represents a series that works on samples?
    /// </summary>
    public abstract class SampleSeries : PlotElement
    {
        /// <summary>
        /// Renders the series on the specified render context.
        /// </summary>
        /// <param name="rc">The rendering context.</param>
        public abstract void Render(IRenderContext rc);

        /// <summary>
        /// Renders the legend symbol on the specified render context.
        /// </summary>
        /// <param name="rc">The rendering context.</param>
        /// <param name="legendBox">The legend rectangle.</param>
        public abstract void RenderLegend(IRenderContext rc, OxyRect legendBox);
    }

    /// <summary>
    /// Represents a series of XY Samples.
    /// </summary>
    /// <typeparam name="TSample"></typeparam>
    /// <typeparam name="XData"></typeparam>
    /// <typeparam name="YData"></typeparam>
    /// <typeparam name="TSampleProvider"></typeparam>
    public abstract class XYSampleSeries<TSample, XData, YData, TSampleProvider> : SampleSeries
        where TSampleProvider : IXYSampleProvider<TSample, XData, YData>
    {
        /// <summary>
        /// The samples to be used by this series.
        /// </summary>
        public List<TSample> Samples { get; set; }

        /// <summary>
        /// Gets or sets the sample provider.
        /// </summary>
        public TSampleProvider SampleProvider { get; set; }

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
        /// Updates the minX, maxX, minY, and maxY values.
        /// </summary>
        public void UpdateMinAndMax()
        {
            ResolveAxes();
            var xyHelper = GetHelper();

            HasMeaningfulDataRange = xyHelper.FindMinMax(SampleProvider, Samples.AsReadOnlyList(), out var minX, out var minY, out var maxX, out var maxY, out var xm, out var ym);
            MinX = minX;
            MinY = minY;
            MaxX = maxX;
            MaxY = maxY;
            XMonotonicity = xm;
            YMonotonicity = ym;

            // TODO: use DataRange<XData> instead of MinX/MinY: it already has a concept of empty, so we can ditch HasMeaningfulDataRange
        }

        /// <summary>
        /// Resolves axes from the axis keys.
        /// </summary>
        protected void ResolveAxes()
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
        protected IXYHelper<XData, YData> GetHelper()
        {
            return XYHelperPreparer<XData, YData>.Prepare(Collator);
        }

        /// <summary>
        /// Gets an <see cref="IXYHelper{XData, YData}"/> for the current axis view state.
        /// </summary>
        /// <returns></returns>
        protected IXYRenderHelper<XData, YData> GetRenderHelper()
        {
            return XYRenderHelperPreparer<XData, YData>.Prepare(Collator);
        }
    }

    /// <summary>
    /// A line series.
    /// </summary>
    /// <typeparam name="TSample"></typeparam>
    /// <typeparam name="XData"></typeparam>
    /// <typeparam name="YData"></typeparam>
    /// <typeparam name="TSampleProvider"></typeparam>
    public class LineSeries<TSample, XData, YData, TSampleProvider> : XYSampleSeries<TSample, XData, YData, TSampleProvider>
        where TSampleProvider : IXYSampleProvider<TSample, XData, YData>
    {
        /// <summary>
        /// The divisor value used to calculate tolerance for line smoothing.
        /// </summary>
        private const double ToleranceDivisor = 200;

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
        /// Initializes a new instance of the <see cref="LineSeries{TSample, XData, YData, TSampleProvider}" /> class.
        /// </summary>
        public LineSeries()
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
        /// Gets or sets a type of interpolation algorithm used for smoothing this <see cref="LineSeries{TSample, XData, YData, TSampleProvider}" />.
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
            ResolveAxes();
            var xyRenderHelper = GetRenderHelper();

            var areBrokenLinesRendered = this.BrokenLineThickness > 0 && this.BrokenLineStyle != LineStyle.None;
            var dashArray = areBrokenLinesRendered ? this.BrokenLineStyle.GetDashArray() : null;
            var broken = areBrokenLinesRendered ? new List<ScreenPoint>(2) : null;

            brokenBuffer = brokenBuffer ?? new List<ScreenPoint>();
            continuousBuffer = continuousBuffer ?? new List<ScreenPoint>();

            int startIdx = 0;
            int endIdx = Samples.Count - 1;

            ScreenPoint? lp = null;
            bool lpb = default(bool);

            if (XMonotonicity.IsMonotone || YMonotonicity.IsMonotone)
            {
                xyRenderHelper.FindWindow(SampleProvider, Samples.AsReadOnlyList(), new DataSample<XData, YData>(MinX, MinY), new DataSample<XData, YData>(MaxX, MaxY), XMonotonicity, YMonotonicity, out startIdx, out endIdx);
            }

            int sampleIdx = startIdx;

            while (xyRenderHelper.ExtractNextContinuousLineSegment<TSample, TSampleProvider>(SampleProvider, Samples.AsReadOnlyList(), ref sampleIdx, endIdx, ref lp, ref lpb, brokenBuffer, continuousBuffer))
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
            rc.DrawLine(points, Color, StrokeThickness, EdgeRenderingMode, ActualDashArray, LineJoin);
        }
    }
}
