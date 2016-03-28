// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HeatMapSeries.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Specifies how the heat map coordinates are defined.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OxyPlot.Series
{
    using System;

    using OxyPlot.Axes;

    /// <summary>
    /// Specifies how the heat map coordinates are defined.
    /// </summary>
    public enum VectorFieldCoordinateDefinition
    {
        /// <summary>
        /// The coordinates defines the center of the cells
        /// </summary>
        Center,

        /// <summary>
        /// The coordinates defines the edge of the cells
        /// </summary>
        Edge
    }
    
    /// <summary>
    /// Represents a heat map.
    /// </summary>
    public class VectorFieldSeries : XYAxisSeries
    {
        /// <summary>
        /// The default tracker format string
        /// </summary>
        public new const string DefaultTrackerFormatString = "{0}\n{1}: {2}\n{3}: {4}\n{5}: {6}";

        /// <summary>
        /// The default color-axis title
        /// </summary>
        private const string DefaultColorAxisTitle = "Value";
        
        /// <summary>
        /// Initializes a new instance of the <see cref="HeatMapSeries" /> class.
        /// </summary>
        public VectorFieldSeries()
        {
            this.TrackerFormatString = DefaultTrackerFormatString;
            this.Interpolate = false;
            this.LabelFormatString = "0.00";
            this.LabelFontSize = 0;

            // arrow properties
            this.HeadLength = 5;
            this.HeadWidth = 5;
            this.Color = OxyColors.Black;
            this.StrokeThickness = 1;
            this.LineStyle = LineStyle.Solid;
            this.LineJoin = LineJoin.Miter;
        }

        /// <summary>
        /// Gets or sets the x-coordinate of the elements at index [0,*] in the data set.
        /// </summary>
        /// <value>
        /// If <see cref="CoordinateDefinition" /> equals <see cref="HeatMapCoordinateDefinition.Center"/>, the value defines the mid point of the element at index [0,*] in the data set.
        /// If <see cref="CoordinateDefinition" /> equals <see cref="HeatMapCoordinateDefinition.Edge"/>, the value defines the coordinate of the left edge of the element at index [0,*] in the data set.
        /// </value>
        public double X0 { get; set; }

        /// <summary>
        /// Gets or sets the x-coordinate of the mid point for the elements at index [m-1,*] in the data set.
        /// </summary>
        /// <value>
        /// If <see cref="CoordinateDefinition" /> equals <see cref="HeatMapCoordinateDefinition.Center"/>, the value defines the mid point of the element at index [m-1,*] in the data set.
        /// If <see cref="CoordinateDefinition" /> equals <see cref="HeatMapCoordinateDefinition.Edge"/>, the value defines the coordinate of the right edge of the element at index [m-1,*] in the data set.
        /// </value>
        public double X1 { get; set; }

        /// <summary>
        /// Gets or sets the y-coordinate of the mid point for the elements at index [*,0] in the data set.
        /// </summary>
        /// <value>
        /// If <see cref="CoordinateDefinition" /> equals <see cref="HeatMapCoordinateDefinition.Center"/>, the value defines the mid point of the element at index [*,0] in the data set.
        /// If <see cref="CoordinateDefinition" /> equals <see cref="HeatMapCoordinateDefinition.Edge"/>, the value defines the coordinate of the bottom edge of the element at index [*,0] in the data set.
        /// </value>
        public double Y0 { get; set; }

        /// <summary>
        /// Gets or sets the y-coordinate of the mid point for the elements at index [*,n-1] in the data set.
        /// </summary>
        /// <value>
        /// If <see cref="CoordinateDefinition" /> equals <see cref="HeatMapCoordinateDefinition.Center"/>, the value defines the mid point of the element at index [*,n-1] in the data set.
        /// If <see cref="CoordinateDefinition" /> equals <see cref="HeatMapCoordinateDefinition.Edge"/>, the value defines the coordinate of the top edge of the element at index [*,n-1] in the data set.
        /// </value>
        public double Y1 { get; set; }

        /// <summary>
        /// Gets or sets the data array.
        /// </summary>
        /// <remarks>Note that the indices of the data array refer to [x,y].
        /// The first dimension is along the x-axis.
        /// The second dimension is along the y-axis.
        /// Remember to call the <see cref="Invalidate" /> method if the contents of the <see cref="Data" /> array is changed.</remarks>
        public ScreenVector[,] Data { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to interpolate when rendering. The default value is <c>false</c>.
        /// </summary>
        /// <remarks>This property is not supported on all platforms.</remarks>
        public bool Interpolate { get; set; }

        /// <summary>
        /// Gets the minimum value of the dataset.
        /// </summary>
        //public double MinValue { get; private set; }

        /// <summary>
        /// Gets the maximum value of the dataset.
        /// </summary>
        //public double MaxValue { get; private set; }

        /// <summary>
        /// Gets or sets the color axis.
        /// </summary>
        /// <value>The color axis.</value>
        public IColorAxis ColorAxis { get; protected set; }

        /// <summary>
        /// Gets or sets the color axis key.
        /// </summary>
        /// <value>The color axis key.</value>
        public string ColorAxisKey { get; set; }

        /// <summary>
        /// Gets or sets the coordinate definition. The default value is <see cref="HeatMapCoordinateDefinition.Center" />.
        /// </summary>
        /// <value>The coordinate definition.</value>
        public VectorFieldCoordinateDefinition CoordinateDefinition { get; set; }
        
        /// <summary>
        /// Gets or sets the format string for the cell labels. The default value is <c>0.00</c>.
        /// </summary>
        /// <value>The format string.</value>
        /// <remarks>The label format string is only used when <see cref="LabelFontSize" /> is greater than 0.</remarks>
        public string LabelFormatString { get; set; }

        /// <summary>
        /// Gets or sets the font size of the labels. The default value is <c>0</c> (labels not visible).
        /// </summary>
        /// <value>The font size relative to the cell height.</value>
        public double LabelFontSize { get; set; }

        /// <summary>
        /// Gets or sets the color of the arrow.
        /// </summary>
        public OxyColor Color { get; set; }

        /// <summary>
        /// Gets or sets the end point of the arrow.
        /// </summary>
        public DataPoint EndPoint { get; set; }

        /// <summary>
        /// Gets or sets the length of the head (relative to the stroke thickness) (the default value is 10).
        /// </summary>
        /// <value>The length of the head.</value>
        public double HeadLength { get; set; }

        /// <summary>
        /// Gets or sets the width of the head (relative to the stroke thickness) (the default value is 3).
        /// </summary>
        /// <value>The width of the head.</value>
        public double HeadWidth { get; set; }

        /// <summary>
        /// Gets or sets the line join type.
        /// </summary>
        /// <value>The line join type.</value>
        public LineJoin LineJoin { get; set; }

        /// <summary>
        /// Gets or sets the line style.
        /// </summary>
        /// <value>The line style.</value>
        public LineStyle LineStyle { get; set; }

        /// <summary>
        /// Gets or sets the start point of the arrow.
        /// </summary>
        /// <remarks>This property is overridden by the ArrowDirection property, if set.</remarks>
        public DataPoint StartPoint { get; set; }

        /// <summary>
        /// Gets or sets the stroke thickness (the default value is 2).
        /// </summary>
        /// <value>The stroke thickness.</value>
        public double StrokeThickness { get; set; }

        /// <summary>
        /// Gets or sets the 'veeness' of the arrow head (relative to thickness) (the default value is 0).
        /// </summary>
        /// <value>The 'veeness'.</value>
        public double Veeness { get; set; }

        /// <summary>
        /// Renders an arrow in screen space.
        /// </summary>
        /// <param name="rc">The render context.</param>
        public void RenderArrow(IRenderContext rc, ScreenPoint screenStartPoint, ScreenVector arrowDirection)
        {
            var screenEndPoint = screenStartPoint + arrowDirection;

            // copied from ArrowAnnotation, should probably be a render helper method somewhere
            var d = screenEndPoint - screenStartPoint;
            d.Normalize();
            var n = new ScreenVector(d.Y, -d.X);

            var p1 = screenEndPoint - (d * this.HeadLength * this.StrokeThickness);
            var p2 = p1 + (n * this.HeadWidth * this.StrokeThickness);
            var p3 = p1 - (n * this.HeadWidth * this.StrokeThickness);
            var p4 = p1 + (d * this.Veeness * this.StrokeThickness);

            var clippingRectangle = this.GetClippingRect();
            const double MinimumSegmentLength = 4;

            var dashArray = this.LineStyle.GetDashArray();

            rc.DrawClippedLine(
                clippingRectangle,
                new[] { screenStartPoint, /*p4*/screenEndPoint },
                MinimumSegmentLength * MinimumSegmentLength,
                this.GetSelectableColor(this.Color),
                this.StrokeThickness,
                dashArray,
                this.LineJoin,
                false);

            // wings
            rc.DrawClippedLine(
                clippingRectangle,
                new[] { screenEndPoint, p3 },
                MinimumSegmentLength * MinimumSegmentLength,
                this.GetSelectableColor(this.Color),
                this.StrokeThickness,
                dashArray,
                this.LineJoin,
                false);

            rc.DrawClippedLine(
                clippingRectangle,
                new[] { screenEndPoint, p2 },
                MinimumSegmentLength * MinimumSegmentLength,
                this.GetSelectableColor(this.Color),
                this.StrokeThickness,
                dashArray,
                this.LineJoin,
                false);

            // disabled in favour of wings
            /*rc.DrawClippedPolygon(
                clippingRectangle,
                new[] { p3, screenEndPoint, p2, p4 },
                MinimumSegmentLength * MinimumSegmentLength,
                this.GetSelectableColor(this.Color),
                OxyColors.Undefined);*/

            /*if (!string.IsNullOrEmpty(this.Text))
            {
                var ha = this.TextHorizontalAlignment;
                var va = this.TextVerticalAlignment;
                if (!this.TextPosition.IsDefined())
                {
                    // automatic position => use automatic alignment
                    ha = d.X < 0 ? HorizontalAlignment.Left : HorizontalAlignment.Right;
                    va = d.Y < 0 ? VerticalAlignment.Top : VerticalAlignment.Bottom;
                }

                var textPoint = this.GetActualTextPosition(() => this.screenStartPoint);
                rc.DrawClippedText(
                    clippingRectangle,
                    textPoint,
                    this.Text,
                    this.ActualTextColor,
                    this.ActualFont,
                    this.ActualFontSize,
                    this.ActualFontWeight,
                    this.TextRotation,
                    ha,
                    va);
            }*/
        }

        /// <summary>
        /// Invalidates the image that renders the heat map. The image will be regenerated the next time the <see cref="HeatMapSeries" /> is rendered.
        /// </summary>
        /// <remarks>Call <see cref="PlotModel.InvalidatePlot" /> to refresh the view.</remarks>
        public void Invalidate()
        {
        }

        /// <summary>
        /// Transforms data space coordinates to orientated screen space coordinates.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <returns>The transformed point.</returns>
        public new ScreenPoint Transform(double x, double y)
        {
            return this.Orientate(base.Transform(x, y));
        }

        /// <summary>
        /// Transforms data space coordinates to orientated screen space coordinates.
        /// </summary>
        /// <param name="point">The point to transform.</param>
        /// <returns>The transformed point.</returns>
        public new ScreenPoint Transform(DataPoint point)
        {
            return this.Orientate(base.Transform(point));
        }

        /// <summary>
        /// Transforms orientated screen space coordinates to data space coordinates.
        /// </summary>
        /// <param name="point">The point to inverse transform.</param>
        /// <returns>The inverse transformed point.</returns>
        public new DataPoint InverseTransform(ScreenPoint point)
        {
            return base.InverseTransform(this.Orientate(point));
        }

        /// <summary>
        /// Renders the series on the specified render context.
        /// </summary>
        /// <param name="rc">The rendering context.</param>
        public override void Render(IRenderContext rc)
        {
            if (this.Data == null)
            {
                return;
            }

            if (this.ColorAxis == null)
            {
                //throw new InvalidOperationException("Color axis not specified.");
            }

            double left = this.X0;
            double right = this.X1;
            double bottom = this.Y0;
            double top = this.Y1;

            int m = this.Data.GetLength(0);
            int n = this.Data.GetLength(1);
            double dx = (this.X1 - this.X0) / (m - 1);
            double dy = (this.Y1 - this.Y0) / (n - 1);

            if (this.CoordinateDefinition == VectorFieldCoordinateDefinition.Edge)
            {
                if (this.XAxis.IsLogarithmic())
                {
                    double gx = Math.Log(this.X1 / this.X0) / (m - 1);
                    left *= Math.Exp(gx / 2);
                    right *= Math.Exp(gx / -2);
                }
                else
                {
                    left -= dx / -2;
                    right += dx / -2;
                }

                if (this.YAxis.IsLogarithmic())
                {
                    double gy = Math.Log(this.Y1 / this.Y0) / (n - 1);
                    bottom *= Math.Exp(gy / 2);
                    top *= Math.Exp(gy / -2);
                }
                else
                {
                    bottom -= dy / -2;
                    top += dy / -2;
                }
            }

            var s00 = this.Transform(left, bottom);
            var s11 = this.Transform(right, top);
            var rect = new OxyRect(s00, s11);

            var clip = this.GetClippingRect();
            
            s00 = this.Orientate(s00); // disorientate
            s11 = this.Orientate(s11); // disorientate

            double sdx = (s11.X - s00.X) / (m - 1);
            double sdy = (s11.Y - s00.Y) / (n - 1);

            // draw lots of arrows
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    //var rectcolor = this.ColorAxis.GetColor(this.Data[i, j]);

                    var pointa = this.Orientate(new ScreenPoint(s00.X + (i * sdx), s00.Y + (j * sdy))); // re-orientate
                    var pointb = new ScreenPoint(pointa.X + Data[i, j].X, pointa.Y + Data[i, j].Y);

                    RenderArrow(rc, pointa, Data[i, j]);
                    //rc.DrawLine(pointa.X, pointa.Y, pointb.X, pointb.Y, arrowPen);

                    //rc.DrawClippedRectangle(clip, rectrect, rectcolor, OxyColors.Undefined, 0);
                }
            }
        }

        /// <summary>
        /// Gets the point on the series that is nearest the specified point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="interpolate">Interpolate the series if this flag is set to <c>true</c>.</param>
        /// <returns>A TrackerHitResult for the current hit.</returns>
        public override TrackerHitResult GetNearestPoint(ScreenPoint point, bool interpolate)
        {
            if (!this.Interpolate)
            {
                // It makes no sense to interpolate the tracker when the plot is not interpolated.
                interpolate = false;
            }

            var p = this.InverseTransform(point);

            if (!this.IsPointInRange(p))
            {
                return null;
            }

            double i;
            double j;

            if (this.XAxis.IsLogarithmic())
            {
                double gx = Math.Log(this.X1 / this.X0) / (this.Data.GetLength(0) - 1);
                i = Math.Log(p.X / this.X0) / gx;
            }
            else
            {
                double dx = (this.X1 - this.X0) / (this.Data.GetLength(0) - 1);
                i = (p.X - this.X0) / dx;
            }

            if (this.YAxis.IsLogarithmic())
            {
                double gy = Math.Log(this.Y1 / this.Y0) / (this.Data.GetLength(1) - 1);
                j = Math.Log(p.Y / this.Y0) / gy;
            }
            else
            {
                double dy = (this.Y1 - this.Y0) / (this.Data.GetLength(1) - 1);
                j = (p.Y - this.Y0) / dy;
            }

            if (!interpolate)
            {
                i = Math.Round(i);
                j = Math.Round(j);

                double px;
                double py;

                if (this.XAxis.IsLogarithmic())
                {
                    double gx = Math.Log(this.X1 / this.X0) / (this.Data.GetLength(0) - 1);
                    px = this.X0 * Math.Exp(i * gx);
                }
                else
                {
                    double dx = (this.X1 - this.X0) / (this.Data.GetLength(0) - 1);
                    px = (i * dx) + this.X0;
                }

                if (this.YAxis.IsLogarithmic())
                {
                    double gy = Math.Log(this.Y1 / this.Y0) / (this.Data.GetLength(1) - 1);
                    py = this.Y0 * Math.Exp(j * gy);
                }
                else
                {
                    double dy = (this.Y1 - this.Y0) / (this.Data.GetLength(1) - 1);
                    py = (j * dy) + this.Y0;
                }

                p = new DataPoint(px, py);
                point = this.Transform(p);
            }

            var value = this.Data[(int)i, (int)j]; // TODO: FIXME
            var colorAxis = this.ColorAxis as Axis;
            var colorAxisTitle = (colorAxis != null ? colorAxis.Title : null) ?? DefaultColorAxisTitle;

            return new TrackerHitResult
            {
                Series = this,
                DataPoint = p,
                Position = point,
                Item = null,
                Index = -1,
                Text = StringHelper.Format(
                this.ActualCulture,
                this.TrackerFormatString,
                null,
                this.Title,
                this.XAxis.Title ?? DefaultXAxisTitle,
                this.XAxis.GetValue(p.X),
                this.YAxis.Title ?? DefaultYAxisTitle,
                this.YAxis.GetValue(p.Y),
                colorAxisTitle,
                value)
            };
        }

        /// <summary>
        /// Ensures that the axes of the series is defined.
        /// </summary>
        protected internal override void EnsureAxes()
        {
            base.EnsureAxes();

            this.ColorAxis =
                this.PlotModel.GetAxisOrDefault(this.ColorAxisKey, (Axis)this.PlotModel.DefaultColorAxis) as IColorAxis;
        }

        /// <summary>
        /// Updates the maximum and minimum values of the series for the x and y dimensions only.
        /// </summary>
        protected internal void UpdateMaxMinXY()
        {
            int m = this.Data.GetLength(0);
            int n = this.Data.GetLength(1);

            this.MinX = Math.Min(this.X0, this.X1);
            this.MaxX = Math.Max(this.X0, this.X1);

            this.MinY = Math.Min(this.Y0, this.Y1);
            this.MaxY = Math.Max(this.Y0, this.Y1);

            if (this.CoordinateDefinition == VectorFieldCoordinateDefinition.Center)
            {
                if (this.XAxis.IsLogarithmic())
                {
                    double gx = Math.Log(this.MaxX / this.MinX) / (m - 1);
                    this.MinX *= Math.Exp(gx / -2);
                    this.MaxX *= Math.Exp(gx / 2);
                }
                else
                {
                    double dx = (this.MaxX - this.MinX) / (m - 1);
                    this.MinX -= dx / 2;
                    this.MaxX += dx / 2;
                }

                if (this.YAxis.IsLogarithmic())
                {
                    double gy = Math.Log(this.MaxY / this.MinY) / (n - 1);
                    this.MinY *= Math.Exp(gy / -2);
                    this.MaxY *= Math.Exp(gy / 2);
                }
                else
                {
                    double dy = (this.MaxY - this.MinY) / (n - 1);
                    this.MinY -= dy / 2;
                    this.MaxY += dy / 2;
                }
            }
        }

        /// <summary>
        /// Updates the maximum and minimum values of the series.
        /// </summary>
        protected internal override void UpdateMaxMin()
        {
            base.UpdateMaxMin();

            this.UpdateMaxMinXY();

            // TODO: implement?
            //this.MinValue = this.Data.Min2D(true);
            //this.MaxValue = this.Data.Max2D();
        }

        /// <summary>
        /// Updates the axes to include the max and min of this series.
        /// </summary>
        protected internal override void UpdateAxisMaxMin()
        {
            base.UpdateAxisMaxMin();
            //var colorAxis = this.ColorAxis as Axis;
            //if (colorAxis != null)
            //{
            //    colorAxis.Include(this.MinValue);
            //    colorAxis.Include(this.MaxValue);
            //}
        }

        /// <summary>
        /// Gets the clipping rectangle, transposed if the X axis is vertically orientated.
        /// </summary>
        /// <returns>The clipping rectangle.</returns>
        protected new OxyRect GetClippingRect()
        {
            double minX = Math.Min(this.XAxis.ScreenMin.X, this.XAxis.ScreenMax.X);
            double minY = Math.Min(this.YAxis.ScreenMin.Y, this.YAxis.ScreenMax.Y);
            double maxX = Math.Max(this.XAxis.ScreenMin.X, this.XAxis.ScreenMax.X);
            double maxY = Math.Max(this.YAxis.ScreenMin.Y, this.YAxis.ScreenMax.Y);

            if (this.XAxis.IsVertical())
            {
                return new OxyRect(minY, minX, maxY - minY, maxX - minX);
            }

            return new OxyRect(minX, minY, maxX - minX, maxY - minY);
        }

        /// <summary>
        /// Transposes the ScreenPoint if the X axis is vertically orientated
        /// </summary>
        /// <param name="point">The <see cref="ScreenPoint" /> to orientate.</param>
        /// <returns>The oriented point.</returns>
        private ScreenPoint Orientate(ScreenPoint point)
        {
            if (this.XAxis.IsVertical())
            {
                point = new ScreenPoint(point.Y, point.X);
            }
            
            return point;
        }

        /// <summary>
        /// Tests if a <see cref="DataPoint" /> is inside the heat map
        /// </summary>
        /// <param name="p">The <see cref="DataPoint" /> to test.</param>
        /// <returns><c>True</c> if the point is inside the heat map.</returns>
        private bool IsPointInRange(DataPoint p)
        {
            this.UpdateMaxMinXY();

            return p.X >= this.MinX && p.X <= this.MaxX && p.Y >= this.MinY && p.Y <= this.MaxY;
        }
    }
}