using OxyPlot.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OxyPlot.Axes.ComposableAxis.AnnotationExamples
{
    /// <summary>
    /// Represents a typed annotation on a pair of XY Axes.
    /// </summary>
    /// <typeparam name="XData"></typeparam>
    /// <typeparam name="YData"></typeparam>
    public class XYAnnotation<XData, YData> : Annotations.AnnotationBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XYAnnotation{XData, YData}" /> class.
        /// </summary>
        protected XYAnnotation()
        {
            this.ClipByXAxis = true;
            this.ClipByYAxis = true;
        }

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
        /// Gets or sets a value indicating whether to clip the annotation by the X axis range.
        /// </summary>
        /// <value><c>true</c> if clipping by the X axis is enabled; otherwise, <c>false</c>.</value>
        public bool ClipByXAxis { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to clip the annotation by the Y axis range.
        /// </summary>
        /// <value><c>true</c> if clipping by the Y axis is enabled; otherwise, <c>false</c>.</value>
        public bool ClipByYAxis { get; set; }

        /// <summary>
        /// Gets or sets the X/Y Collator for the axes.
        /// </summary>
        protected XYCollator<XData, YData> Collator { get; private set; }

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
        protected virtual IXYHelper<XData, YData> GetHelper()
        {
            var transpose = XAxis.Position == AxisPosition.Left || XAxis.Position == AxisPosition.Right;
            return XYHelperPreparer<XData, YData>.PrepareHorizontalVertial(Collator, transpose);
        }

        /// <summary>
        /// Gets an <see cref="IXYHelper{XData, YData}"/> for the current axis view state.
        /// </summary>
        /// <returns></returns>
        protected virtual IXYRenderHelper<XData, YData> GetRenderHelper()
        {
            var transpose = XAxis.Position == AxisPosition.Left || XAxis.Position == AxisPosition.Right;
            return XYRenderHelperPreparer<XData, YData>.PrepareHorizontalVertical(Collator, transpose);
        }

        /// <inheritdoc/>
        public override void EnsureAxes()
        {
            ResolveAxes();
        }

        /// <inheritdoc/>
        public override void Render(IRenderContext rc)
        {
            ResolveAxes();
        }


        /// <inheritdoc/>
        public override OxyRect GetClippingRect()
        {
            var xrect = new OxyRect(XAxis.ScreenMin, XAxis.ScreenMax);
            var yrect = new OxyRect(YAxis.ScreenMin, YAxis.ScreenMax);
            return xrect.Intersect(yrect);

            // TODO: our cousin in TransposableAnnotation doesn't some other stuff... do we need to do that stuff?
        }
    }

    /// <summary>
    /// Provides an abstract base class for annotations that contains text.
    /// </summary>
    public abstract class TextualAnnotation<XData, YData> : XYAnnotation<XData, YData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextualAnnotation{XData, YData}"/> class.
        /// </summary>
        protected TextualAnnotation()
        {
            this.TextHorizontalAlignment = HorizontalAlignment.Center;
            this.TextVerticalAlignment = VerticalAlignment.Middle;
            this.TextPosition = null;
            this.TextRotation = 0;
        }

        /// <summary>
        /// Gets or sets the annotation text.
        /// </summary>
        /// <value>The text.</value>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the position of the text.
        /// </summary>
        /// <remarks>If the value is <c>null</c>, the default position of the text will be used.</remarks>
        public DataSample<XData, YData>? TextPosition { get; set; }

        /// <summary>
        /// Gets or sets the horizontal alignment of the text.
        /// </summary>
        public HorizontalAlignment TextHorizontalAlignment { get; set; }

        /// <summary>
        /// Gets or sets the vertical alignment of the text.
        /// </summary>
        public VerticalAlignment TextVerticalAlignment { get; set; }

        /// <summary>
        /// Gets or sets the rotation of the text.
        /// </summary>
        /// <value>The text rotation in degrees.</value>
        public double TextRotation { get; set; }

        /// <summary>
        /// Gets the actual position of the text.
        /// </summary>
        /// <param name="defaultPosition">A function that returns the default position. This is used if <see cref="TextPosition" /> is undefined.</param>
        /// <returns>The actual position of the text, in screen space.</returns>
        protected ScreenPoint GetActualTextPosition(Func<ScreenPoint> defaultPosition)
        {
            var xyRenderHelper = this.GetRenderHelper();
            return this.TextPosition.HasValue ? xyRenderHelper.TransformSample(this.TextPosition.Value) : defaultPosition();
        }

        /// <summary>
        /// Gets the actual text alignment.
        /// </summary>
        /// <param name="ha">The horizontal alignment.</param>
        /// <param name="va">The vertical alignment.</param>
        protected void GetActualTextAlignment(out HorizontalAlignment ha, out VerticalAlignment va)
        {
            ha = this.TextHorizontalAlignment;
            va = this.TextVerticalAlignment;
        }
    }

    /// <summary>
    /// A typed path annotation on XY axes.
    /// </summary>
    /// <typeparam name="XData"></typeparam>
    /// <typeparam name="YData"></typeparam>
    /// <typeparam name="XDataOptional"></typeparam>
    /// <typeparam name="YDataOptional"></typeparam>
    /// <typeparam name="XDataOptionalProvider"></typeparam>
    /// <typeparam name="YDataOptionalProvider"></typeparam>
    public abstract class PathAnnotation<XData, YData, XDataOptional, YDataOptional, XDataOptionalProvider, YDataOptionalProvider> : TextualAnnotation<XData, YData>
        where XDataOptionalProvider : IOptionalProvider<XData, XDataOptional>
        where YDataOptionalProvider : IOptionalProvider<YData, YDataOptional>
    {
        /// <summary>
        /// Gets or sets the minimum segment length for this annotation.
        /// </summary>
        public double MinimumSegmentLength { get; set; } = 4;

        /// <summary>
        /// The points of the line, transformed to screen coordinates.
        /// </summary>
        private IList<ScreenPoint> screenPoints;

        /// <summary>
        /// Initializes a new instance of the <see cref="PathAnnotation{XData, YData, XDataOptional, YDataOptional, XOptionalProvider, YOptionalProvider}" /> class.
        /// </summary>
        protected PathAnnotation(XDataOptionalProvider xOptionalProvider, YDataOptionalProvider yOptionalProvider)
        {
            this.XOptionalProvider = xOptionalProvider;
            this.YOptionalProvider = yOptionalProvider;

            this.MinimumX = xOptionalProvider.None;
            this.MaximumX = xOptionalProvider.None;
            this.MinimumY = yOptionalProvider.None;
            this.MaximumY = yOptionalProvider.None;
            this.Color = OxyColors.Blue;
            this.StrokeThickness = 1;
            this.LineStyle = LineStyle.Dash;
            this.LineJoin = LineJoin.Miter;

            this.TextLinePosition = 1;
            this.TextOrientation = AnnotationTextOrientation.AlongLine;
            this.TextMargin = 12;
            this.TextHorizontalAlignment = HorizontalAlignment.Right;
            this.TextVerticalAlignment = VerticalAlignment.Top;
        }

        /// <summary>
        /// Gets or sets the color of the line.
        /// </summary>
        public OxyColor Color { get; set; }

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
        /// Gets or sets the maximum X coordinate for the line.
        /// </summary>
        public XDataOptional MaximumX { get; set; }

        /// <summary>
        /// Gets or sets the maximum Y coordinate for the line.
        /// </summary>
        public YDataOptional MaximumY { get; set; }

        /// <summary>
        /// Gets or sets the minimum X coordinate for the line.
        /// </summary>
        public XDataOptional MinimumX { get; set; }

        /// <summary>
        /// Gets or sets the minimum Y coordinate for the line.
        /// </summary>
        public YDataOptional MinimumY { get; set; }

        /// <summary>
        /// Gets or sets the stroke thickness.
        /// </summary>
        /// <value>The stroke thickness.</value>
        public double StrokeThickness { get; set; }

        /// <summary>
        /// Gets or sets the text margin (along the line).
        /// </summary>
        /// <value>The text margin.</value>
        public double TextMargin { get; set; }

        /// <summary>
        /// Gets or sets the text padding (in the direction of the text).
        /// </summary>
        /// <value>The text padding.</value>
        public double TextPadding { get; set; }

        /// <summary>
        /// Gets or sets the text orientation.
        /// </summary>
        /// <value>The text orientation.</value>
        public AnnotationTextOrientation TextOrientation { get; set; }

        /// <summary>
        /// Gets or sets the text position relative to the line.
        /// </summary>
        /// <value>The text position in the interval [0,1].</value>
        /// <remarks>Positions smaller than 0.25 are left aligned at the start of the line
        /// Positions larger than 0.75 are right aligned at the end of the line
        /// Other positions are center aligned at the specified position</remarks>
        public double TextLinePosition { get; set; }

        /// <summary>
        /// Gets or sets the actual minimum value on the x axis.
        /// </summary>
        /// <value>The actual minimum value on the x axis.</value>
        protected XData ActualMinimumX { get; set; }

        /// <summary>
        /// Gets or sets the actual minimum value on the y axis.
        /// </summary>
        /// <value>The actual minimum value on the y axis.</value>
        protected YData ActualMinimumY { get; set; }

        /// <summary>
        /// Gets or sets the actual maximum value on the x axis.
        /// </summary>
        /// <value>The actual maximum value on the x axis.</value>
        protected XData ActualMaximumX { get; set; }

        /// <summary>
        /// Gets or sets the actual maximum value on the y axis.
        /// </summary>
        /// <value>The actual maximum value on the y axis.</value>
        protected YData ActualMaximumY { get; set; }

        /// <summary>
        /// Gets the optional provider for the X space.
        /// </summary>
        public XDataOptionalProvider XOptionalProvider { get; }

        /// <summary>
        /// Gets the optional provider for the Y space.
        /// </summary>
        public YDataOptionalProvider YOptionalProvider { get; }

        /// <inheritdoc/>
        public override void Render(IRenderContext rc)
        {
            base.Render(rc);

            this.CalculateActualMinimumsMaximums();

            this.screenPoints = this.GetScreenPoints();

            var clippedPoints = new List<ScreenPoint>();
            var dashArray = this.LineStyle.GetDashArray();

            if (this.StrokeThickness > 0 && this.LineStyle != LineStyle.None)
            {
                rc.DrawReducedLine(
                   this.screenPoints,
                   MinimumSegmentLength * MinimumSegmentLength,
                   this.GetSelectableColor(this.Color),
                   this.StrokeThickness,
                   this.EdgeRenderingMode,
                   dashArray,
                   this.LineJoin,
                   null,
                   clippedPoints.AddRange);
            }

            var margin = this.TextMargin;

            this.GetActualTextAlignment(out var ha, out var va);

            if (ha == HorizontalAlignment.Center)
            {
                margin = 0;
            }
            else
            {
                margin *= this.TextLinePosition < 0.5 ? 1 : -1;
            }

            if (GetPointAtRelativeDistance(clippedPoints, this.TextLinePosition, margin, out var position, out var angle))
            {
                if (angle < -90)
                {
                    angle += 180;
                }

                if (angle > 90)
                {
                    angle -= 180;
                }

                switch (this.TextOrientation)
                {
                    case AnnotationTextOrientation.Horizontal:
                        angle = 0;
                        break;
                    case AnnotationTextOrientation.Vertical:
                        angle = -90;
                        break;
                }

                // Apply 'padding' to the position
                var angleInRadians = angle / 180 * Math.PI;
                var f = 1;

                if (ha == HorizontalAlignment.Right)
                {
                    f = -1;
                }

                if (ha == HorizontalAlignment.Center)
                {
                    f = 0;
                }

                position += new ScreenVector(f * this.TextPadding * Math.Cos(angleInRadians), f * this.TextPadding * Math.Sin(angleInRadians));

                if (!string.IsNullOrEmpty(this.Text))
                {
                    var textPosition = this.GetActualTextPosition(() => position);

                    if (this.TextPosition.HasValue)
                    {
                        angle = this.TextRotation;
                    }

                    rc.DrawText(
                        textPosition,
                        this.Text,
                        this.ActualTextColor,
                        this.ActualFont,
                        this.ActualFontSize,
                        this.ActualFontWeight,
                        angle,
                        ha,
                        va);
                }
            }
        }

        /// <summary>
        /// When overridden in a derived class, tests if the plot element is hit by the specified point.
        /// </summary>
        /// <param name="args">The hit test arguments.</param>
        /// <returns>
        /// The result of the hit test.
        /// </returns>
        protected override HitTestResult HitTestOverride(HitTestArguments args)
        {
            if (this.screenPoints == null)
            {
                return null;
            }

            var nearestPoint = ScreenPointHelper.FindNearestPointOnPolyline(args.Point, this.screenPoints);
            double dist = (args.Point - nearestPoint).Length;
            if (dist < args.Tolerance)
            {
                return new HitTestResult(this, nearestPoint);
            }

            return null;
        }

        /// <summary>
        /// Gets the screen points.
        /// </summary>
        /// <returns>The list of points to display on screen for this path.</returns>
        protected abstract IList<ScreenPoint> GetScreenPoints();

        /// <summary>
        /// Calculates the actual minimums and maximums.
        /// </summary>
        protected virtual void CalculateActualMinimumsMaximums()
        {
            var renderHelper = GetRenderHelper();

            // not performance critical: use an untyped provider
            var x = renderHelper.XProvider;
            var y = renderHelper.YProvider;

            this.ActualMinimumX = XOptionalProvider.TryGetValue(this.MinimumX, out var minX)
                ? DataHelpers.Max(x, minX, this.XAxis.ClipMinimum)
                : this.XAxis.ClipMinimum;
            this.ActualMaximumX = XOptionalProvider.TryGetValue(this.MaximumX, out var maxX)
                ? DataHelpers.Max(x, maxX, this.XAxis.ClipMinimum)
                : this.XAxis.ClipMaximum;
            this.ActualMinimumY = YOptionalProvider.TryGetValue(this.MinimumY, out var minY)
                ? DataHelpers.Max(y, minY, this.YAxis.ClipMinimum)
                : this.YAxis.ClipMinimum;
            this.ActualMaximumY = YOptionalProvider.TryGetValue(this.MaximumY, out var maxY)
                ? DataHelpers.Max(y, maxY, this.YAxis.ClipMinimum)
                : this.YAxis.ClipMaximum;

            
            var topLeft = renderHelper.InverseTransform(this.PlotModel.PlotArea.TopLeft);
            var bottomRight = renderHelper.InverseTransform(this.PlotModel.PlotArea.BottomRight);

            if (!this.ClipByXAxis)
            {
                this.ActualMinimumX = DataHelpers.Min(x, topLeft.X, bottomRight.X);
                this.ActualMaximumX = DataHelpers.Max(x, topLeft.X, bottomRight.X);
            }

            if (!this.ClipByYAxis)
            {
                this.ActualMinimumY = DataHelpers.Min(y, topLeft.Y, bottomRight.Y);
                this.ActualMaximumY = DataHelpers.Max(y, topLeft.Y, bottomRight.Y);
            }
        }

        /// <summary>
        /// Gets the point on a curve at the specified relative distance along the curve.
        /// </summary>
        /// <param name="pts">The curve points.</param>
        /// <param name="p">The relative distance along the curve.</param>
        /// <param name="margin">The margins.</param>
        /// <param name="position">The position.</param>
        /// <param name="angle">The angle.</param>
        /// <returns>True if a position was found.</returns>
        private static bool GetPointAtRelativeDistance(
            IList<ScreenPoint> pts, double p, double margin, out ScreenPoint position, out double angle)
        {
            if (pts == null || pts.Count == 0)
            {
                position = new ScreenPoint();
                angle = 0;
                return false;
            }

            double length = 0;
            for (int i = 1; i < pts.Count; i++)
            {
                length += (pts[i] - pts[i - 1]).Length;
            }

            double l = (length * p) + margin;
            double eps = 1e-8;
            length = 0;
            for (int i = 1; i < pts.Count; i++)
            {
                double dl = (pts[i] - pts[i - 1]).Length;
                if (l >= length - eps && l <= length + dl + eps)
                {
                    double f = (l - length) / dl;
                    double x = (pts[i].X * f) + (pts[i - 1].X * (1 - f));
                    double y = (pts[i].Y * f) + (pts[i - 1].Y * (1 - f));
                    position = new ScreenPoint(x, y);
                    double dx = pts[i].X - pts[i - 1].X;
                    double dy = pts[i].Y - pts[i - 1].Y;
                    angle = Math.Atan2(dy, dx) / Math.PI * 180;
                    return true;
                }

                length += dl;
            }

            position = pts[0];
            angle = 0;
            return false;
        }
    }

    /// <summary>
    /// Represents an annotation that shows a straight line.
    /// </summary>
    public class LineAnnotation : PathAnnotation<double, double, double, double, DoubleAsNaNOptional, DoubleAsNaNOptional>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref = "LineAnnotation" /> class.
        /// </summary>
        public LineAnnotation()
            : base(default, default)
        {
            this.Type = LineAnnotationType.LinearEquation;
        }

        /// <summary>
        /// Gets or sets the y-intercept when Type is LinearEquation.
        /// </summary>
        /// <value>The intercept value.</value>
        /// <remarks>Linear equation y-intercept (the b in y=mx+b).
        /// http://en.wikipedia.org/wiki/Linear_equation</remarks>
        public double Intercept { get; set; }

        /// <summary>
        /// Gets or sets the slope when Type is LinearEquation.
        /// </summary>
        /// <value>The slope value.</value>
        /// <remarks>Linear equation slope (the m in y=mx+b)
        /// http://en.wikipedia.org/wiki/Linear_equation</remarks>
        public double Slope { get; set; }

        /// <summary>
        /// Gets or sets the type of line equation.
        /// </summary>
        public LineAnnotationType Type { get; set; }

        /// <summary>
        /// Gets or sets the X position for vertical lines (only for Type==Vertical).
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Gets or sets the Y position for horizontal lines (only for Type==Horizontal)
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Gets the screen points.
        /// </summary>
        /// <returns>The list of points to display on screen for this path.</returns>
        protected override IList<ScreenPoint> GetScreenPoints()
        {
            // y=f(x)
            Func<double, double> fx = null;

            // x=f(y)
            Func<double, double> fy = null;

            switch (this.Type)
            {
                case LineAnnotationType.Horizontal:
                    fx = x => this.Y;
                    break;
                case LineAnnotationType.Vertical:
                    fy = y => this.X;
                    break;
                default:
                    fx = x => (this.Slope * x) + this.Intercept;
                    break;
            }

            var renderHelper = GetRenderHelper();

            var psuedoSamples = new List<DataSample<double, double>>();

            if (fx != null)
            {
                psuedoSamples.Add(new DataSample<double, double>(this.ActualMinimumX, fx(this.ActualMinimumX)));
                psuedoSamples.Add(new DataSample<double, double>(this.ActualMaximumX, fx(this.ActualMaximumX)));
            }
            else
            {
                psuedoSamples.Add(new DataSample<double, double>(fy(this.ActualMinimumY), this.ActualMinimumY));
                psuedoSamples.Add(new DataSample<double, double>(fy(this.ActualMaximumY), this.ActualMaximumY));
            }

            var points = new List<ScreenPoint>();
            var subsegmentLength = MinimumSegmentLength / 2;
            renderHelper.InterpolateLines(psuedoSamples.AsReadOnlyList(), subsegmentLength, points);
            return points;
        }
    }

    /// <summary>
    /// Represents an annotation that shows a function rendered as a path.
    /// </summary>
    public class FunctionAnnotation<XData, YData> : PathAnnotation<XData, YData, Option<XData>, Option<YData>, Optional<XData>, Optional<YData>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionAnnotation" /> class.
        /// </summary>
        public FunctionAnnotation(Func<XData, YData> equation)
            : base(default, default)
        {
            this.Equation = equation;

            this.Resolution = 400;
        }

        /// <summary>
        /// Gets or sets the y=f(x) equation when Type is Equation.
        /// </summary>
        public Func<XData, YData> Equation { get; set; }

        /// <summary>
        /// Gets or sets the resolution.
        /// </summary>
        /// <value>The resolution.</value>
        public int Resolution { get; set; }

        /// <summary>
        /// Gets the screen points.
        /// </summary>
        /// <returns>The list of screen points defined by this function annotation.</returns>
        protected override IList<ScreenPoint> GetScreenPoints()
        {
            var x0 = this.ActualMinimumX;
            var x1 = this.ActualMaximumX;

            var renderHelper = this.GetRenderHelper();
            var xProvider = renderHelper.XProvider;

            var psuedoSamples = new List<DataSample<XData, YData>>();

            for (int i = 0; i <= Resolution; i++)
            {
                var c = (double)i / Resolution;
                var x = xProvider.Interpolate(x0, x1, c);
                var y = this.Equation(x);

                psuedoSamples.Add(new DataSample<XData, YData>(x, y));
            }

            var points = new List<ScreenPoint>();
            renderHelper.InterpolateLines(psuedoSamples.AsReadOnlyList(), 2, points);
            return points;
        }
    }
}
