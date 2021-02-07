using System;
using System.Collections.Generic;
using System.Text;

namespace OxyPlot.Axes.ComposableAxis.SeriesExamples
{
    /// <summary>
    /// Represents a series of XY Samples.
    /// </summary>
    /// <typeparam name="TSample"></typeparam>
    /// <typeparam name="XData"></typeparam>
    /// <typeparam name="YData"></typeparam>
    /// <typeparam name="VData"></typeparam>
    /// <typeparam name="TSampleProvider"></typeparam>
    /// <typeparam name="TValueProvider"></typeparam>
    /// <typeparam name="TSampleFilter"></typeparam>
    public abstract class XYVSampleSeries<TSample, XData, YData, VData, TSampleProvider, TValueProvider, TSampleFilter> : XYSampleSeries<TSample, XData, YData, TSampleProvider, TSampleFilter>
        where TSampleProvider : IXYSampleProvider<TSample, XData, YData>
        where TSampleFilter : IFilter<TSample>
        where TValueProvider : IValueProvider<TSample, VData>
    {
        /// <summary>
        /// Initializes an instance of the <see cref="AxisBase"/> class.
        /// </summary>
        protected XYVSampleSeries(TSampleProvider sampleProvider, TSampleFilter sampleFilter, TValueProvider valueProvider)
            : base(sampleProvider, sampleFilter)
        {
            this.ValueProvider = valueProvider;

            Samples = new List<TSample>();
            CanTrackerInterpolatePoints = false;
        }

        /// <summary>
        /// The Value provider.
        /// </summary>
        public TValueProvider ValueProvider { get; }

        /// <summary>
        /// Gets or set the Value Axis key
        /// </summary>
        public string VAxisKey { get; set; }

        /// <summary>
        /// Gets or sets the Value Axis associated with this series.
        /// </summary>
        public IColorAxis<VData> VAxis { get; private set; }

        /// <summary>
        /// Gets or sets the minimum value of any sample.
        /// </summary>
        public VData MinV { get; protected set; }

        /// <summary>
        /// Gets or sets the maximum value of any sample.
        /// </summary>
        public VData MaxV { get; protected set; }

        /// <summary>
        /// The monotonicity of the value.
        /// </summary>
        public Monotonicity VMonotonicity { get; protected set; }

        /// <summary>
        /// Gets or sets a value indiciating whether the min/max V and monotonicity properties are meaningful.
        /// </summary>
        public bool HasMeaningfulValueRange { get; protected set; }

        /// <inheritdoc/>
        protected internal override void UpdateAxisMaxMin()
        {
            base.UpdateAxisMaxMin();

            if (VAxis != null)
            {
                this.VAxis.Include(this.MinV);
                this.VAxis.Include(this.MaxV);
            }
        }

        /// <summary>
        /// Updates the minX, maxX, minY, and maxY values.
        /// </summary>
        protected override void UpdateMinAndMax()
        {
            base.UpdateMinAndMax();

            if (Samples.Count == 0)
                return; // bail before we crash

            if (VAxis == null)
                return;

            var colorHelper = GetColorHelper();

            HasMeaningfulValueRange = colorHelper.FindMinMax(ValueProvider, SampleFilter, Samples.AsReadOnlyList(), out var minV, out var maxV, out var vm);
            MinV = minV;
            MaxV = maxV;
            VMonotonicity = vm;

            // TODO: use DataRange<XData> instead of MinX/MinY: it already has a concept of empty, so we can ditch HasMeaningfulDataRange
        }

        /// <summary>
        /// Resolves axes from the axis keys.
        /// </summary>
        protected override void ResolveAxes()
        {
            base.ResolveAxes();

            // should throw if we can't get axes of the right type
            VAxis = (IColorAxis<VData>)this.PlotModel.GetAxisOrDefault(VAxisKey, null);
        }

        /// <summary>
        /// Gets an <see cref="IXYHelper{XData, YData}"/> for the current axis, which does not depend on the view state.
        /// </summary>
        /// <returns></returns>
        protected virtual IColorHelper<VData> GetColorHelper()
        {
            return VAxis == null ? null : ColorHelperPreparer<VData>.Prepare(VAxis);
        }

        /// <inheritdoc/>
        protected internal override bool IsUsing(Axis axis)
        {
            return this.VAxis == axis || base.IsUsing(axis);
        }
    }

    /// <summary>
    /// A scatter series.
    /// </summary>
    /// <typeparam name="TSample"></typeparam>
    /// <typeparam name="XData"></typeparam>
    /// <typeparam name="YData"></typeparam>
    /// <typeparam name="VData"></typeparam>
    /// <typeparam name="TSampleProvider"></typeparam>
    /// <typeparam name="TValueProvider"></typeparam>
    /// <typeparam name="TSizeProvider"></typeparam>
    /// <typeparam name="TSampleFilter"></typeparam>
    public class ScatterSeries<TSample, XData, YData, VData, TSampleProvider, TValueProvider, TSizeProvider, TSampleFilter> : XYVSampleSeries<TSample, XData, YData, VData, TSampleProvider, TValueProvider, TSampleFilter>
        where TSampleProvider : IXYSampleProvider<TSample, XData, YData>
        where TSampleFilter : IFilter<TSample>
        where TValueProvider : IValueProvider<TSample, VData>
        where TSizeProvider : IValueProvider<TSample, double>
    {
        /// <summary>
        /// The default marker fill color.
        /// </summary>
        private OxyColor defaultMarkerFillColor;

        /// <summary>
        /// Initializes a new instance of the <see cref="LineSeries{TSample, XData, YData, TSampleProvider, TSampleFilter}" /> class.
        /// </summary>
        public ScatterSeries(TSampleProvider sampleProvider, TValueProvider valueProvider, TSizeProvider sizeProvider, TSampleFilter sampleFilter)
            : base(sampleProvider, sampleFilter, valueProvider)
        {
            this.SizeProvider = sizeProvider;

            this.defaultMarkerFillColor = OxyColors.Undefined;

            this.MarkerFill = OxyColors.Automatic;
            this.MarkerSize = 5;
            this.MarkerType = MarkerType.Square;
            this.MarkerStroke = OxyColors.Automatic;
            this.MarkerStrokeThickness = 1;

            this.CanTrackerInterpolatePoints = false;
            this.LabelMargin = 6;
        }

        /// <summary>
        /// The size provider.
        /// </summary>
        public TSizeProvider SizeProvider { get; }

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
        /// Gets the actual marker fill color.
        /// </summary>
        /// <value>The actual color.</value>
        public OxyColor ActualMarkerFillColor
        {
            get
            {
                return this.MarkerFill.GetActualColor(this.defaultMarkerFillColor);
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

            // check if any item of the series is selected
            bool isSelected = this.IsSelected();

            ResolveAxes();
            var xyRenderHelper = GetRenderHelper();
            var colorHelper = YAxis == null ? null : GetColorHelper();

            base.FindWindow(out var startIdx, out var endIdx);

            // let's do this one slowly... to check that it is easy, and see how slow it is
            var x = xyRenderHelper.XTransformation;
            var y = xyRenderHelper.YTransformation;

            for (int i = startIdx; i <= endIdx; i++)
            {
                var currentSample = Samples[i];

                if (!SampleFilter.Filter(currentSample)
                    || !SampleProvider.TrySample(currentSample, out var currentXYSample)
                    || !x.Filter(currentXYSample.X)
                    || !y.Filter(currentXYSample.Y)
                    || !x.WithinClipBounds(currentXYSample.X)
                    || !y.WithinClipBounds(currentXYSample.Y))
                {
                    // skip
                    continue;
                }

                var currentPoint = xyRenderHelper.TransformSample(currentXYSample);
                var currentValue = ValueProvider.Sample(currentSample);
                var currentSize = SizeProvider.Sample(currentSample);
                var currentColor = colorHelper?.Transform(currentValue) ?? OxyColors.Automatic;
                var fill = currentColor.IsAutomatic() ? this.ActualMarkerFillColor : currentColor;
                var stroke = currentColor.IsAutomatic() ? this.MarkerStroke : currentColor;

                rc.DrawMarker(currentPoint, this.MarkerType, this.MarkerOutline, currentSize, fill, stroke, this.MarkerStrokeThickness, EdgeRenderingMode.Automatic);
            }
        }

        /// <inheritdoc/>
        public override void RenderLegend(IRenderContext rc, OxyRect legendBox)
        {
            double xmid = (legendBox.Left + legendBox.Right) / 2;
            double ymid = (legendBox.Top + legendBox.Bottom) / 2;

            var midpt = new ScreenPoint(xmid, ymid);

            rc.DrawMarker(
                midpt,
                this.MarkerType,
                this.MarkerOutline,
                this.MarkerSize,
                this.IsSelected() ? this.PlotModel.SelectionColor : this.ActualMarkerFillColor,
                this.IsSelected() ? this.PlotModel.SelectionColor : this.MarkerStroke,
                this.MarkerStrokeThickness,
                this.EdgeRenderingMode);
        }

        /// <inheritdoc/>
        protected internal override void SetDefaultValues()
        {
            if (this.MarkerFill.IsAutomatic())
            {
                this.defaultMarkerFillColor = this.PlotModel.GetDefaultColor();
            }
        }

        /// <inheritdoc/>
        public override OxyRect GetClippingRect()
        {
            var xrect = new OxyRect(XAxis.ScreenMin, XAxis.ScreenMax);
            var yrect = new OxyRect(YAxis.ScreenMin, YAxis.ScreenMax);
            return xrect.Intersect(yrect);
        }
    }
}
