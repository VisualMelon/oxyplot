﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XYAxisSeries.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Provides an abstract base class for series that are related to an X-axis and a Y-axis.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OxyPlot.Series
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using OxyPlot.Axes;

    /// <summary>
    /// Provides an abstract base class for series that are related to an X-axis and a Y-axis.
    /// </summary>
    public abstract class XYAxisSeries : ItemsSeries, ITransposablePlotElement
    {
        /// <summary>
        /// The default tracker format string
        /// </summary>
        public const string DefaultTrackerFormatString = "{0}\n{1}: {2}\n{3}: {4}";

        /// <summary>
        /// The default x-axis title
        /// </summary>
        protected const string DefaultXAxisTitle = "X";

        /// <summary>
        /// The default y-axis title
        /// </summary>
        protected const string DefaultYAxisTitle = "Y";

        /// <summary>
        /// Initializes a new instance of the <see cref="XYAxisSeries"/> class.
        /// </summary>
        protected XYAxisSeries()
        {
            this.TrackerFormatString = DefaultTrackerFormatString;
        }

        /// <summary>
        /// Gets or sets the maximum x-coordinate of the dataset.
        /// </summary>
        /// <value>The maximum x-coordinate.</value>
        public double MaxX { get; protected set; }

        /// <summary>
        /// Gets or sets the maximum y-coordinate of the dataset.
        /// </summary>
        /// <value>The maximum y-coordinate.</value>
        public double MaxY { get; protected set; }

        /// <summary>
        /// Gets or sets the minimum x-coordinate of the dataset.
        /// </summary>
        /// <value>The minimum x-coordinate.</value>
        public double MinX { get; protected set; }

        /// <summary>
        /// Gets or sets the minimum y-coordinate of the dataset.
        /// </summary>
        /// <value>The minimum y-coordinate.</value>
        public double MinY { get; protected set; }

        /// <summary>
        /// Gets the x-axis.
        /// </summary>
        /// <value>The x-axis.</value>
        public Axis XAxis { get; private set; }

        /// <summary>
        /// Gets or sets the x-axis key. The default is <c>null</c>.
        /// </summary>
        /// <value>The x-axis key.</value>
        public string XAxisKey { get; set; }

        /// <summary>
        /// Gets the y-axis.
        /// </summary>
        /// <value>The y-axis.</value>
        public Axis YAxis { get; private set; }

        /// <summary>
        /// Gets or sets the y-axis key. The default is <c>null</c>.
        /// </summary>
        /// <value>The y-axis key.</value>
        public string YAxisKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the X coordinate of all data point increases monotonically.
        /// </summary>
        protected bool IsXMonotonic { get; set; }

        /// <summary>
        /// Gets or sets the last visible window start position in the data points collection.
        /// </summary>
        protected int WindowStartIndex { get; set; }

        /// <summary>
        /// Gets the rectangle the series uses on the screen (screen coordinates).
        /// </summary>
        /// <returns>The rectangle.</returns>
        public OxyRect GetScreenRectangle()
        {
            return this.GetClippingRect();
        }

        /// <summary>
        /// Renders the legend symbol on the specified rendering context.
        /// </summary>
        /// <param name="rc">The rendering context.</param>
        /// <param name="legendBox">The legend rectangle.</param>
        public override void RenderLegend(IRenderContext rc, OxyRect legendBox)
        {
        }

        /// <summary>
        /// Check if this data series requires X/Y axes. (e.g. Pie series do not require axes)
        /// </summary>
        /// <returns>The are axes required.</returns>
        protected internal override bool AreAxesRequired()
        {
            return true;
        }

        /// <summary>
        /// Ensures that the axes of the series is defined.
        /// </summary>
        protected internal override void EnsureAxes()
        {
            this.XAxis = this.XAxisKey != null ?
                         this.PlotModel.GetAxis(this.XAxisKey) :
                         this.PlotModel.DefaultXAxis;

            this.YAxis = this.YAxisKey != null ?
                         this.PlotModel.GetAxis(this.YAxisKey) :
                         this.PlotModel.DefaultYAxis;
        }

        /// <summary>
        /// Check if the data series is using the specified axis.
        /// </summary>
        /// <param name="axis">An axis.</param>
        /// <returns>True if the axis is in use.</returns>
        protected internal override bool IsUsing(Axis axis)
        {
            return false;
        }

        /// <summary>
        /// Sets default values from the plot model.
        /// </summary>
        protected internal override void SetDefaultValues()
        {
        }

        /// <summary>
        /// Updates the axes to include the max and min of this series.
        /// </summary>
        protected internal override void UpdateAxisMaxMin()
        {
            this.XAxis.Include(this.MinX);
            this.XAxis.Include(this.MaxX);
            this.YAxis.Include(this.MinY);
            this.YAxis.Include(this.MaxY);
        }

        /// <summary>
        /// Updates the data.
        /// </summary>
        protected internal override void UpdateData()
        {
            this.WindowStartIndex = 0;
        }

        /// <summary>
        /// Updates the maximum and minimum values of the series.
        /// </summary>
        protected internal override void UpdateMaxMin()
        {
            this.MinX = this.MinY = this.MaxX = this.MaxY = double.NaN;
        }

        /// <summary>
        /// Gets the point on the curve that is nearest the specified point.
        /// </summary>
        /// <param name="points">The point list.</param>
        /// <param name="point">The point.</param>
        /// <returns>A tracker hit result if a point was found.</returns>
        /// <remarks>The Text property of the result will not be set, since the formatting depends on the various series.</remarks>
        protected TrackerHitResult GetNearestInterpolatedPointInternal(List<DataPoint> points, ScreenPoint point)
        {
            return this.GetNearestInterpolatedPointInternal(points, 0, point);
        }

        /// <summary>
        /// Gets the point on the curve that is nearest the specified point.
        /// </summary>
        /// <param name="points">The point list.</param>
        /// <param name="startIdx">The index to start from.</param>
        /// <param name="point">The point.</param>
        /// <returns>A tracker hit result if a point was found.</returns>
        /// <remarks>The Text property of the result will not be set, since the formatting depends on the various series.</remarks>
        protected TrackerHitResult GetNearestInterpolatedPointInternal(List<DataPoint> points, int startIdx, ScreenPoint point)
        {
            if (this.XAxis == null || this.YAxis == null || points == null)
            {
                return null;
            }

            var spn = default(ScreenPoint);
            var dpn = default(DataPoint);
            double index = -1;

            double minimumDistance = double.MaxValue;

            for (int i = startIdx; i + 1 < points.Count; i++)
            {
                var p1 = points[i];
                var p2 = points[i + 1];
                if (!this.IsValidPoint(p1) || !this.IsValidPoint(p2))
                {
                    continue;
                }

                var sp1 = this.Transform(p1);
                var sp2 = this.Transform(p2);

                // Find the nearest point on the line segment.
                var spl = ScreenPointHelper.FindPointOnLine(point, sp1, sp2);

                if (ScreenPoint.IsUndefined(spl))
                {
                    // P1 && P2 coincident
                    continue;
                }

                double l2 = (point - spl).LengthSquared;

                if (l2 < minimumDistance)
                {
                    double segmentLength = (sp2 - sp1).Length;
                    double u = segmentLength > 0 ? (spl - sp1).Length / segmentLength : 0;
                    dpn = this.InverseTransform(spl);
                    spn = spl;
                    minimumDistance = l2;
                    index = i + u;
                }
            }

            if (minimumDistance < double.MaxValue)
            {
                var item = this.GetItem((int)Math.Round(index));
                return new TrackerHitResult
                {
                    Series = this,
                    DataPoint = dpn,
                    Position = spn,
                    Item = item,
                    Index = index
                };
            }

            return null;
        }

        /// <summary>
        /// Gets the nearest point.
        /// </summary>
        /// <param name="points">The points (data coordinates).</param>
        /// <param name="point">The point (screen coordinates).</param>
        /// <returns>A <see cref="TrackerHitResult" /> if a point was found, <c>null</c> otherwise.</returns>
        /// <remarks>The Text property of the result will not be set, since the formatting depends on the various series.</remarks>
        protected TrackerHitResult GetNearestPointInternal(IEnumerable<DataPoint> points, ScreenPoint point)
        {
            return this.GetNearestPointInternal(points, 0, point);
        }

        /// <summary>
        /// Gets the nearest point.
        /// </summary>
        /// <param name="points">The points (data coordinates).</param>
        /// <param name="startIdx">The index to start from.</param>
        /// <param name="point">The point (screen coordinates).</param>
        /// <returns>A <see cref="TrackerHitResult" /> if a point was found, <c>null</c> otherwise.</returns>
        /// <remarks>The Text property of the result will not be set, since the formatting depends on the various series.</remarks>
        protected TrackerHitResult GetNearestPointInternal(IEnumerable<DataPoint> points, int startIdx, ScreenPoint point)
        {
            var spn = default(ScreenPoint);
            var dpn = default(DataPoint);
            double index = -1;

            double minimumDistance = double.MaxValue;
            int i = 0;
            foreach (var p in points.Skip(startIdx))
            {
                if (!this.IsValidPoint(p))
                {
                    i++;
                    continue;
                }

                var sp = this.Transform(p.x, p.y);
                double d2 = (sp - point).LengthSquared;

                if (d2 < minimumDistance)
                {
                    dpn = p;
                    spn = sp;
                    minimumDistance = d2;
                    index = i;
                }

                i++;
            }

            if (minimumDistance < double.MaxValue)
            {
                var item = this.GetItem((int)Math.Round(index));
                return new TrackerHitResult
                {
                    Series = this,
                    DataPoint = dpn,
                    Position = spn,
                    Item = item,
                    Index = index
                };
            }

            return null;
        }

        /// <summary>
        /// Determines whether the specified point is valid.
        /// </summary>
        /// <param name="pt">The point.</param>
        /// <returns><c>true</c> if the point is valid; otherwise, <c>false</c> .</returns>
        protected virtual bool IsValidPoint(DataPoint pt)
        {
            return
                this.XAxis != null && this.XAxis.IsValidValue(pt.X) &&
                this.YAxis != null && this.YAxis.IsValidValue(pt.Y);
        }

        /// <summary>
        /// Determines whether the specified point is valid.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <returns><c>true</c> if the point is valid; otherwise, <c>false</c> . </returns>
        protected bool IsValidPoint(double x, double y)
        {
            return
                this.XAxis != null && this.XAxis.IsValidValue(x) &&
                this.YAxis != null && this.YAxis.IsValidValue(y);
        }

        /// <summary>
        /// Updates the Max/Min limits from the specified <see cref="DataPoint" /> list.
        /// </summary>
        /// <param name="points">The list of points.</param>
        protected void InternalUpdateMaxMin(List<DataPoint> points)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }

            this.IsXMonotonic = true;

            if (points.Count == 0)
            {
                return;
            }

            // preserve existing min/max if non-NaN
            var xdsh = new Utilities.DoubleSequenceHelper(false, this.MinX, this.MaxX, !double.IsNaN(this.MinX), !double.IsNaN(this.MaxX));
            var ydsh = new Utilities.DoubleSequenceHelper(false, this.MinY, this.MaxY, !double.IsNaN(this.MinY), !double.IsNaN(this.MaxY));

            foreach (var pt in points)
            {
                double x = pt.X;
                double y = pt.Y;

                // Check if the point is valid
                if (!this.IsValidPoint(pt))
                {
                    continue;
                }

                xdsh.ObserveNext(x);
                ydsh.ObserveNext(y);
            }

            if (!xdsh.IsEmpty)
            {
                this.IsXMonotonic = xdsh.GetMonotonicity().IsNonDecreasing;

                this.MinX = Math.Max(xdsh.Minimum, this.XAxis.FilterMinValue);
                this.MinY = Math.Max(ydsh.Minimum, this.YAxis.FilterMinValue);
                this.MaxX = Math.Min(xdsh.Maximum, this.XAxis.FilterMaxValue);
                this.MaxY = Math.Min(ydsh.Maximum, this.YAxis.FilterMaxValue);
            }
        }

        /// <summary>
        /// Updates the Max/Min limits from the specified list.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the list.</typeparam>
        /// <param name="items">The items.</param>
        /// <param name="xf">A function that provides the x value for each item.</param>
        /// <param name="yf">A function that provides the y value for each item.</param>
        /// <exception cref="System.ArgumentNullException">The items argument cannot be null.</exception>
        protected void InternalUpdateMaxMin<T>(List<T> items, Func<T, double> xf, Func<T, double> yf)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            this.IsXMonotonic = true;

            if (items.Count == 0)
            {
                return;
            }

            // preserve existing min/max if non-NaN
            var xdsh = new Utilities.DoubleSequenceHelper(false, this.MinX, this.MaxX, !double.IsNaN(this.MinX), !double.IsNaN(this.MaxX));
            var ydsh = new Utilities.DoubleSequenceHelper(false, this.MinY, this.MaxY, !double.IsNaN(this.MinY), !double.IsNaN(this.MaxY));

            foreach (var item in items)
            {
                double x = xf(item);
                double y = yf(item);

                // Check if the point is valid
                if (!this.IsValidPoint(x, y))
                {
                    continue;
                }

                xdsh.ObserveNext(x);
                xdsh.ObserveNext(y);
            }

            if (!xdsh.IsEmpty)
            {
                this.IsXMonotonic = xdsh.GetMonotonicity().IsNonDecreasing;

                this.MinX = Math.Max(xdsh.Minimum, this.XAxis.FilterMinValue);
                this.MinY = Math.Max(ydsh.Minimum, this.YAxis.FilterMinValue);
                this.MaxX = Math.Min(xdsh.Maximum, this.XAxis.FilterMaxValue);
                this.MaxY = Math.Min(ydsh.Maximum, this.YAxis.FilterMaxValue);
            }
        }

        /// <summary>
        /// Updates the Max/Min limits from the specified collection.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        /// <param name="items">The items.</param>
        /// <param name="xmin">A function that provides the x minimum for each item.</param>
        /// <param name="xmax">A function that provides the x maximum for each item.</param>
        /// <param name="ymin">A function that provides the y minimum for each item.</param>
        /// <param name="ymax">A function that provides the y maximum for each item.</param>
        /// <exception cref="System.ArgumentNullException">The items argument cannot be null.</exception>
        protected void InternalUpdateMaxMin<T>(List<T> items, Func<T, double> xmin, Func<T, double> xmax, Func<T, double> ymin, Func<T, double> ymax)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            this.IsXMonotonic = true;

            if (items.Count == 0)
            {
                return;
            }

            // preserve existing min/max if non-NaN
            var x0dsh = new Utilities.DoubleSequenceHelper(false, this.MinX, this.MaxX, !double.IsNaN(this.MinX), !double.IsNaN(this.MaxX));
            var x1dsh = new Utilities.DoubleSequenceHelper(false, this.MinX, this.MaxX, !double.IsNaN(this.MinX), !double.IsNaN(this.MaxX));
            var y0dsh = new Utilities.DoubleSequenceHelper(false, this.MinY, this.MaxY, !double.IsNaN(this.MinY), !double.IsNaN(this.MaxY));
            var y1dsh = new Utilities.DoubleSequenceHelper(false, this.MinY, this.MaxY, !double.IsNaN(this.MinY), !double.IsNaN(this.MaxY));

            foreach (var item in items)
            {
                double x0 = xmin(item);
                double x1 = xmax(item);
                double y0 = ymin(item);
                double y1 = ymax(item);

                if (!this.IsValidPoint(x0, y0) || !this.IsValidPoint(x1, y1))
                {
                    continue;
                }

                x0dsh.ObserveNext(x0);
                x1dsh.ObserveNext(x1);
                y0dsh.ObserveNext(y0);
                y1dsh.ObserveNext(y1);
            }

            if (!x0dsh.IsEmpty)
            {
                this.IsXMonotonic = x0dsh.GetMonotonicity().IsNonDecreasing && x1dsh.GetMonotonicity().IsNonDecreasing;

                this.MinX = Math.Max(XAxis.FilterMinValue, Math.Min(x0dsh.Minimum, x1dsh.Minimum));
                this.MinY = Math.Max(YAxis.FilterMinValue, Math.Min(y0dsh.Minimum, y1dsh.Minimum));
                this.MaxX = Math.Min(XAxis.FilterMaxValue, Math.Max(x0dsh.Maximum, x1dsh.Maximum));
                this.MaxY = Math.Min(YAxis.FilterMaxValue, Math.Max(y0dsh.Maximum, y1dsh.Maximum));
            }
        }

        /// <summary>
        /// Verifies that both axes are defined.
        /// </summary>
        protected void VerifyAxes()
        {
            if (this.XAxis == null)
            {
                throw new InvalidOperationException("XAxis not defined.");
            }

            if (this.YAxis == null)
            {
                throw new InvalidOperationException("YAxis not defined.");
            }
        }

        /// <summary>
        /// Updates visible window start index.
        /// </summary>
        /// <typeparam name="T">The type of the list items.</typeparam>
        /// <param name="items">Data points.</param>
        /// <param name="xgetter">Function that gets data point X coordinate.</param>
        /// <param name="targetX">X coordinate of visible window start.</param>
        /// <param name="lastIndex">Last window index.</param>
        /// <returns>The new window start index.</returns>
        protected int UpdateWindowStartIndex<T>(IList<T> items, Func<T, double> xgetter, double targetX, int lastIndex)
        {
            lastIndex = this.FindWindowStartIndex(items, xgetter, targetX);
            if (lastIndex > 0)
            {
                lastIndex--;
            }

            return lastIndex;
        }

        /// <summary>
        /// Finds the index of max(x) &lt;= target x in a list of data points
        /// </summary>
        /// <typeparam name="T">The type of the list items.</typeparam>
        /// <param name="items">vector of data points</param>
        /// <param name="xgetter">Function that gets data point X coordinate.</param>
        /// <param name="targetX">target x.</param>
        /// <returns>
        /// Index of x with max(x) &lt;= target x or 0 if not found.
        /// </returns>
        protected int FindWindowStartIndex<T>(IList<T> items, Func<T, double> xgetter, double targetX)
        {
            int start = 0;
            int end = items.Count - 1;

            while (start < end)
            {
                // mid-point closest to start
                var mid = start + (end - start) / 2;
                var guess = mid;

                var guessX = xgetter(items[guess]);

                // scan backward through NaNs
                while (double.IsNaN(guessX))
                {
                    if (guess <= start)
                    {
                        // ran off the end
                        start = mid + 1;
                        continue;
                    }

                    guess--;
                    guessX = xgetter(items[guess]);
                }

                if (guessX >= targetX)
                {
                    end = guess;
                }
                else if (guess == start)
                {
                    break;
                }
                else
                {
                    start = guess;
                }
            }

            return start;
        }

        /// <summary>
        /// Finds the index of max(x) &lt;= target x in a list of data points
        /// </summary>
        /// <typeparam name="T">The type of the list items.</typeparam>
        /// <param name="items">vector of data points</param>
        /// <param name="xgetter">Function that gets data point X coordinate.</param>
        /// <param name="targetX">target x.</param>
        /// <param name="initialGuess">initial guess index.</param>
        /// <returns>
        /// index of x with max(x) &lt;= target x or -1 if cannot find
        /// </returns>
        protected int FindWindowStartIndexOld<T>(IList<T> items, Func<T, double> xgetter, double targetX, int initialGuess)
        {
            int lastguess = 0;
            int start = 0;
            int end = items.Count - 1;
            int curGuess = initialGuess;

            while (start <= end)
            {
                if (curGuess < start)
                {
                    return lastguess;
                }
                else if (curGuess > end)
                {
                    return end;
                }

                double guessX = xgetter(items[curGuess]);
                if (guessX.Equals(targetX))
                {
                    return curGuess;
                }
                else if (guessX > targetX)
                {
                    end = curGuess - 1;
                    if (end < start)
                    {
                        return lastguess;
                    }
                    else if (end == start)
                    {
                        return end;
                    }
                }
                else
                {
                    start = curGuess + 1;
                    lastguess = curGuess;
                }

                if (start >= end)
                {
                    return lastguess;
                }

                double endX = xgetter(items[end]);
                double startX = xgetter(items[start]);

                var m = (end - start + 1) / (endX - startX);
                curGuess = start + (int)((targetX - startX) * m);
            }

            return lastguess;
        }
    }
}
