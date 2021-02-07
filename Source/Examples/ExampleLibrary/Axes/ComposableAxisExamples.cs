// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AxisExamples.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Provides examples for general axis properties.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ExampleLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using OxyPlot;
    using OxyPlot.Axes;
    using OxyPlot.Series;
    using OxyPlot.Legends;
    using OxyPlot.Axes.ComposableAxis;

    /// <summary>
    /// Provides examples for general axis properties.
    /// </summary>
    [Examples("Composable Axis examples"), Tags("Composable Axes")]
    public class ComposableAxisExamples
    {
        [Example("Linear XY")]
        public static PlotModel LinearXY()
        {
            var plot = new PlotModel();

            var xaxis = new HorizontalVerticalAxis<double, DoubleProvider, Linear, MinMaxFilter<double, DoubleProvider>, double, DoubleAsNaNOptional>(default, default, new MinMaxFilter<double, DoubleProvider>(default, double.MinValue, double.MaxValue))
            {
                Position = AxisPosition.Bottom,
                Minimum = 0,
                Maximum = 10,
                Title = "X Axis",
                Key = "X",
                StartPosition = 0.8,
                EndPosition = 0.2,
            };

            var xticks = new TickBand<double>(new LinearDoubleTickLocator(), new SpacingOptions());
            xaxis.Bands.Add(xticks);

            plot.Axes.Add(xaxis);

            var yaxis = new HorizontalVerticalAxis<double, DoubleProvider, Linear, MinMaxFilter<double, DoubleProvider>, double, DoubleAsNaNOptional>(default, default, new MinMaxFilter<double, DoubleProvider>(default, double.MinValue, double.MaxValue))
            {
                Position = AxisPosition.Left,
                Minimum = -1,
                Maximum = 1,
                Title = "Y Axis",
                Key = "Y",
            };

            var yticks = new TickBand<double>(new LinearDoubleTickLocator(), new SpacingOptions());
            yaxis.Bands.Add(yticks);

            plot.Axes.Add(yaxis);

            return plot;
        }

        [Example("Logarithmic XY")]
        public static PlotModel LogarithmicXY()
        {
            var plot = new PlotModel();

            var xaxis = new HorizontalVerticalAxis<double, DoubleProvider, Linear, MinMaxFilter<double, DoubleProvider>, double, DoubleAsNaNOptional>(default, default, new MinMaxFilter<double, DoubleProvider>(default, double.MinValue, double.MaxValue))
            {
                Position = AxisPosition.Bottom,
                Minimum = 1,
                Maximum = 10,
                Title = "X Axis",
                Key = "X",
            };

            var xticks = new TickBand<double>(new LinearDoubleTickLocator(), new SpacingOptions());
            xaxis.Bands.Add(xticks);

            plot.Axes.Add(xaxis);

            var yaxis = new HorizontalVerticalAxis<double, DoubleProvider, Logarithmic10, MinMaxFilter<double, DoubleProvider>, double, DoubleAsNaNOptional>(default, default, new MinMaxFilter<double, DoubleProvider>(default, double.MinValue, double.MaxValue))
            {
                Position = AxisPosition.Left,
                Minimum = 1,
                Maximum = 100,
                Title = "Y Axis (Log)",
                Key = "Y",
            };

            var yticks = new TickBand<double>(new LogarithmicDoubleTickLocator(), new SpacingOptions());
            yaxis.Bands.Add(yticks);

            plot.Axes.Add(yaxis);

            return plot;
        }

        [Example("LineSeries on default Axes")]
        public static PlotModel LineOnDefaultAxis()
        {
            var plot = new PlotModel() { Subtitle = "The axes are default axes" };

            var lines = new OxyPlot.Axes.ComposableAxis.SeriesExamples.LineSeries<DataPoint, double, double, DataPointXYSampleProvider, AcceptAllFilter<DataPoint>>(default, default)
            {
                Title = "y = sin(x)",
                RenderInLegend = true,
            };

            for (var x = 0.0; x <= 10; x += 0.01)
            {
                lines.Samples.Add(new DataPoint(x, Math.Sin(x)));
            }

            plot.Series.Add(lines);

            plot.Legends.Add(new Legend());

            return plot;
        }

        [Example("LineSeries on immitation default Axes")]
        public static PlotModel LineOnimmitationDefaultAxis()
        {
            var plot = new PlotModel() { Subtitle = "The axes are immitations of the default axes" };

            var xaxis = new HorizontalVerticalAxis<double, DoubleProvider, Linear, MinMaxFilter<double, DoubleProvider>, double, DoubleAsNaNOptional>(default, default, new MinMaxFilter<double, DoubleProvider>(default, double.MinValue, double.MaxValue))
            {
                Position = AxisPosition.Bottom,
            };

            var xticks = new TickBand<double>(new LinearDoubleTickLocator(), new SpacingOptions());
            xaxis.Bands.Add(xticks);

            plot.Axes.Add(xaxis);

            var yaxis = new HorizontalVerticalAxis<double, DoubleProvider, Linear, MinMaxFilter<double, DoubleProvider>, double, DoubleAsNaNOptional>(default, default, new MinMaxFilter<double, DoubleProvider>(default, double.MinValue, double.MaxValue))
            {
                Position = AxisPosition.Left,
            };

            var yticks = new TickBand<double>(new LinearDoubleTickLocator(), new SpacingOptions());
            yaxis.Bands.Add(yticks);

            plot.Axes.Add(yaxis);

            var lines = new OxyPlot.Axes.ComposableAxis.SeriesExamples.LineSeries<DataPoint, double, double, DataPointXYSampleProvider, AcceptAllFilter<DataPoint>>(default, default)
            {
                Title = "y = sin(x)",
                RenderInLegend = true,
            };

            for (var x = 0.0; x <= 10; x += 0.01)
            {
                lines.Samples.Add(new DataPoint(x, Math.Sin(x)));
            }

            plot.Series.Add(lines);

            plot.Legends.Add(new Legend());

            return plot;
        }

        [Example("LineSeries on Linear XY")]
        public static PlotModel LineOnLinearXY()
        {
            var plot = LinearXY();

            var lines = new OxyPlot.Axes.ComposableAxis.SeriesExamples.LineSeries<DataPoint, double, double, DataPointXYSampleProvider, AcceptAllFilter<DataPoint>>(default, default)
            {
                Title = "y = sin(x)",
                RenderInLegend = true,
            };

            for (var x = 0.0; x <= 10; x += 0.01)
            {
                lines.Samples.Add(new DataPoint(x, Math.Sin(x)));
            }

            plot.Series.Add(lines);

            plot.Legends.Add(new Legend());

            return plot;
        }

        [Example("LineSeries on Linear XY, Transposed")]
        public static PlotModel LineOnLinearXYTransposed()
        {
            var plot = LinearXY();

            plot.Axes[0].Position = AxisPosition.Left;
            plot.Axes[1].Position = AxisPosition.Bottom;

            var lines = new OxyPlot.Axes.ComposableAxis.SeriesExamples.LineSeries<DataPoint, double, double, DataPointXYSampleProvider, AcceptAllFilter<DataPoint>>(default, default)
            {
                XAxisKey = "X",
                YAxisKey = "Y",
                MarkerType = MarkerType.Diamond,
                MarkerFill = OxyColors.Red,
                MarkerSize = 2,
            };

            for (var x = 0.0; x <= 10; x += 0.01)
            {
                lines.Samples.Add(new DataPoint(x, Math.Sin(x)));
            }

            plot.Series.Add(lines);

            return plot;
        }

        [Example("LineSeries on Logarithmic XY")]
        public static PlotModel LineOnLogarithmicXY()
        {
            var plot = LogarithmicXY();

            var lines = new OxyPlot.Axes.ComposableAxis.SeriesExamples.LineSeries<DataPoint, double, double, DataPointXYSampleProvider, AcceptAllFilter<DataPoint>>(default, default)
            {
                Title = "y = x^2",
            };

            for (var x = 1.0; x <= 10; x += 0.01)
            {
                lines.Samples.Add(new DataPoint(x, x * x));
            }

            plot.Series.Add(lines);

            plot.Legends.Add(new Legend());

            return plot;
        }

        [Example("LineAnnotation on Linear XY")]
        public static PlotModel LineAnnotationOnLinearXY()
        {
            var plot = LinearXY();

            var lv = new OxyPlot.Axes.ComposableAxis.AnnotationExamples.LineAnnotation
            {
                Type = OxyPlot.Annotations.LineAnnotationType.Vertical,
                X = 3,
                Text = "x = 3",
            };

            var lh = new OxyPlot.Axes.ComposableAxis.AnnotationExamples.LineAnnotation
            {
                Type = OxyPlot.Annotations.LineAnnotationType.Horizontal,
                Y = 3,
                Text = "y = 3",
            };

            var ll = new OxyPlot.Axes.ComposableAxis.AnnotationExamples.LineAnnotation
            {
                Type = OxyPlot.Annotations.LineAnnotationType.LinearEquation,
                Intercept = 1,
                Slope = -0.1,
                Text = "y = 1 - x/10",
            };

            plot.Annotations.Add(lv);
            plot.Annotations.Add(lh);
            plot.Annotations.Add(ll);

            plot.Legends.Add(new Legend());

            return plot;
        }

        [Example("LineAnnotation on Logarithmic XY")]
        public static PlotModel LineAnnotationOnLogarithmicAxis()
        {
            var plot = LogarithmicXY();

            var ll = new OxyPlot.Axes.ComposableAxis.AnnotationExamples.LineAnnotation
            {
                MinimumX = -0.999, // x = -1 crashes the logarithm
                Type = OxyPlot.Annotations.LineAnnotationType.LinearEquation,
                Intercept = 1,
                Slope = 1,
                Text = "y = 1 + x",
            };

            plot.Annotations.Add(ll);

            plot.Legends.Add(new Legend());

            return plot;
        }

        [Example("FunctionAnnotation on Logarithmic XY")]
        public static PlotModel FunctionAnnotationOnLogarithmicAxis()
        {
            var plot = LogarithmicXY();

            var fs = new OxyPlot.Axes.ComposableAxis.AnnotationExamples.FunctionAnnotation<double, double>(x => x * x)
            {
                Text = "y = x^2",
            };

            plot.Annotations.Add(fs);

            plot.Legends.Add(new Legend());

            return plot;
        }

        [Example("LineSeries, 1M points")]
        public static PlotModel LineSeries1M()
        {
            var plot = new PlotModel { Title = "LineSeries, 1M points" };

            var xaxis = new HorizontalVerticalAxis<double, DoubleProvider, Linear, MinMaxFilter<double, DoubleProvider>, double, DoubleAsNaNOptional>(default, default, new MinMaxFilter<double, DoubleProvider>(default, double.MinValue, double.MaxValue))
            {
                Position = AxisPosition.Bottom,
                Title = "X",
            };

            var xticks = new TickBand<double>(new LinearDoubleTickLocator(), new SpacingOptions());
            xaxis.Bands.Add(xticks);
            plot.Axes.Add(xaxis);

            var yaxis = new HorizontalVerticalAxis<double, DoubleProvider, Linear, MinMaxFilter<double, DoubleProvider>, double, DoubleAsNaNOptional>(default, default, new MinMaxFilter<double, DoubleProvider>(default, double.MinValue, double.MaxValue))
            {
                Position = AxisPosition.Left,
                Title = "Y",
            };

            var yticks = new TickBand<double>(new LinearDoubleTickLocator(), new SpacingOptions());
            yaxis.Bands.Add(yticks);
            plot.Axes.Add(yaxis);

            var lines = new OxyPlot.Axes.ComposableAxis.SeriesExamples.LineSeries<DataPoint, double, double, DataPointXYSampleProvider, AcceptAllFilter<DataPoint>>(default, default);

            AddPoints(lines.Samples, 1000000);

            plot.Series.Add(lines);

            return plot;
        }

        [Example("LineSeries, 1M points, Filtered")]
        public static PlotModel LineSeries1M_ZeroZeromMin()
        {
            var plot = new PlotModel { Title = "LineSeries, 1M points" };

            var xaxis = new HorizontalVerticalAxis<double, DoubleProvider, Linear, MinMaxFilter<double, DoubleProvider>, double, DoubleAsNaNOptional>(default, default, new MinMaxFilter<double, DoubleProvider>(default, 0, double.MaxValue))
            {
                Position = AxisPosition.Bottom,
                Title = "X",
            };

            var xticks = new TickBand<double>(new LinearDoubleTickLocator(), new SpacingOptions());
            xaxis.Bands.Add(xticks);
            plot.Axes.Add(xaxis);

            var yaxis = new HorizontalVerticalAxis<double, DoubleProvider, Linear, MinMaxFilter<double, DoubleProvider>, double, DoubleAsNaNOptional>(default, default, new MinMaxFilter<double, DoubleProvider>(default, 0, double.MaxValue))
            {
                Position = AxisPosition.Left,
                Title = "Y",
            };

            var yticks = new TickBand<double>(new LinearDoubleTickLocator(), new SpacingOptions());
            yaxis.Bands.Add(yticks);
            plot.Axes.Add(yaxis);

            var lines = new OxyPlot.Axes.ComposableAxis.SeriesExamples.LineSeries<DataPoint, double, double, DataPointXYSampleProvider, AcceptAllFilter<DataPoint>>(default, default);

            AddPoints(lines.Samples, 1000000);

            plot.Series.Add(lines);

            return plot;
        }

        private static void AddPoints(ICollection<DataPoint> points, int n)
        {
            for (int i = 0; i < n; i++)
            {
                double x = Math.PI * 10 * i / (n - 1);
                points.Add(new DataPoint(x * Math.Cos(x), x * Math.Sin(x)));
            }
        }

        [Example("Axis Margins, Data Margins, and Padding, Asymmetrical")]
        public static PlotModel MarginsAndPaddingAsymmetrical()
        {
            var plot = new PlotModel
            {
                Title = "Try resizing the plot",
                Subtitle = "ClipMinimum/Maximum are Blue\nActualMinimum/Maximum are Red\nDataMinimum/Maximum are Green"
            };

            var xaxis = new HorizontalVerticalAxis<double, DoubleProvider, Linear, MinMaxFilter<double, DoubleProvider>, double, DoubleAsNaNOptional>(default, default, new MinMaxFilter<double, DoubleProvider>(default, double.MinValue, double.MaxValue))
            {
                Position = AxisPosition.Bottom,
                MinimumPadding = 0.1,
                MaximumPadding = 0.05,
                MinimumDataMargin = new ScreenReal(20),
                MaximumDataMargin = new ScreenReal(10),
                MinimumMargin = new ScreenReal(30),
                MaximumMargin = new ScreenReal(15),
                //MajorGridlineStyle = LineStyle.Solid,  // these are not yet implemented
                //MinorGridlineStyle = LineStyle.Dash,
                //CropGridlines = true,
            };

            plot.Axes.Add(xaxis);

            var yaxis = new HorizontalVerticalAxis<double, DoubleProvider, Linear, MinMaxFilter<double, DoubleProvider>, double, DoubleAsNaNOptional>(default, default, new MinMaxFilter<double, DoubleProvider>(default, double.MinValue, double.MaxValue))
            {
                Position = AxisPosition.Left,
                MinimumPadding = 0.2,
                MaximumPadding = 0.1,
                MinimumDataMargin = new ScreenReal(40),
                MaximumDataMargin = new ScreenReal(20),
                MinimumMargin = new ScreenReal(60),
                MaximumMargin = new ScreenReal(30),
                //MajorGridlineStyle = LineStyle.Solid,
                //MinorGridlineStyle = LineStyle.Dash,
                //CropGridlines = true,
            };

            plot.Axes.Add(yaxis);

            var rectangle = new OxyPlot.Axes.ComposableAxis.SeriesExamples.LineSeries<DataPoint, double, double, DataPointXYSampleProvider, AcceptAllFilter<DataPoint>>(default, default);
            rectangle.Color = OxyColors.Green;
            rectangle.Samples.Add(new DataPoint(-5.0, 0.0));
            rectangle.Samples.Add(new DataPoint(-5.0, 20.0));
            rectangle.Samples.Add(new DataPoint(25.0, 20.0));
            rectangle.Samples.Add(new DataPoint(25.0, 0.0));
            rectangle.Samples.Add(new DataPoint(-5.0, 0.0));
            plot.Series.Add(rectangle);

            AddAxisMarginAnnotations(plot);

            return plot;
        }

        private static void AddAxisMarginAnnotations(PlotModel plot)
        {
            plot.Annotations.Add(new RenderingCapabilities.DelegateAnnotation(rc =>
            {
                foreach (var axis in plot.Axes)
                {
                    if (axis.IsHorizontal())
                    {
                        var h = (IAxis<double>)axis;
                        var t = h.GetTransformation();

                        rc.DrawLine(t.Transform(h.ClipMinimum).Value, 0.0, t.Transform(h.ClipMinimum).Value, plot.Height, new OxyPen(OxyColors.Blue, 1, LineStyle.Dot), EdgeRenderingMode.Automatic);
                        rc.DrawLine(t.Transform(h.ClipMaximum).Value, 0.0, t.Transform(h.ClipMaximum).Value, plot.Height, new OxyPen(OxyColors.Blue, 1, LineStyle.Dot), EdgeRenderingMode.Automatic);

                        rc.DrawLine(t.Transform(h.ActualMinimum).Value, 0.0, t.Transform(h.ActualMinimum).Value, plot.Height, new OxyPen(OxyColors.Red, 1, LineStyle.Dot), EdgeRenderingMode.Automatic);
                        rc.DrawLine(t.Transform(h.ActualMaximum).Value, 0.0, t.Transform(h.ActualMaximum).Value, plot.Height, new OxyPen(OxyColors.Red, 1, LineStyle.Dot), EdgeRenderingMode.Automatic);

                        if (h.TryGetDataRange(out var dataMin, out var dataMax))
                        {
                            rc.DrawLine(t.Transform(dataMin).Value, 0.0, t.Transform(dataMin).Value, plot.Height, new OxyPen(OxyColors.Green, 1, LineStyle.Dot), EdgeRenderingMode.Automatic);
                            rc.DrawLine(t.Transform(dataMax).Value, 0.0, t.Transform(dataMax).Value, plot.Height, new OxyPen(OxyColors.Green, 1, LineStyle.Dot), EdgeRenderingMode.Automatic);
                        }
                    }
                    else
                    {
                        var v = (IAxis<double>)axis;
                        var t = v.GetTransformation();

                        rc.DrawLine(0.0, t.Transform(v.ClipMinimum).Value, plot.Width, t.Transform(v.ClipMinimum).Value, new OxyPen(OxyColors.Blue, 1, LineStyle.Dot), EdgeRenderingMode.Automatic);
                        rc.DrawLine(0.0, t.Transform(v.ClipMaximum).Value, plot.Width, t.Transform(v.ClipMaximum).Value, new OxyPen(OxyColors.Blue, 1, LineStyle.Dot), EdgeRenderingMode.Automatic);

                        rc.DrawLine(0.0, t.Transform(v.ActualMinimum).Value, plot.Width, t.Transform(v.ActualMinimum).Value, new OxyPen(OxyColors.Red, 1, LineStyle.Dot), EdgeRenderingMode.Automatic);
                        rc.DrawLine(0.0, t.Transform(v.ActualMaximum).Value, plot.Width, t.Transform(v.ActualMaximum).Value, new OxyPen(OxyColors.Red, 1, LineStyle.Dot), EdgeRenderingMode.Automatic);

                        if (v.TryGetDataRange(out var dataMin, out var dataMax))
                        {
                            rc.DrawLine(0.0, t.Transform(dataMin).Value, plot.Width, t.Transform(dataMin).Value, new OxyPen(OxyColors.Green, 1, LineStyle.Dot), EdgeRenderingMode.Automatic);
                            rc.DrawLine(0.0, t.Transform(dataMax).Value, plot.Width, t.Transform(dataMax).Value, new OxyPen(OxyColors.Green, 1, LineStyle.Dot), EdgeRenderingMode.Automatic);
                        }
                    }
                }
            }));
        }

        [Example("Line on Weekday Axis")]
        public static PlotModel LineOnDateTime()
        {
            var plot = new PlotModel();

            // NOTE: the WeekDayTransformation just takes the linear space provided by DateTimeProvider and fills it with holes
            // This means that basic operations like interpolation will escape the meaningful space
            // You can rectify this by using a custom WeekdayProvider instead in an alternative WeekDayTransformation implementation
            // The TickLocator probably also wants to be aware of these things
            var weekdays = new WeekdayTransformation(8, 20);

            var xaxis = new HorizontalVerticalAxis<DateTime, DateTimeProvider, WeekdayTransformation, AcceptAllFilter<DateTime>, Option<DateTime>, Optional<DateTime>>(weekdays, default, default)
            {
                Position = AxisPosition.Bottom,
                Minimum = Option<DateTime>.Some(DateTime.Now),
                MinimumPadding = 0,
                MaximumPadding = 0,
                MinimumDataMargin = new ScreenReal(10),
                MaximumDataMargin = new ScreenReal(10),
                Title = "Time",
                Key = "X",
            };

            var xticks = new TickBand<DateTime>(new LinearDateTimeTickLocator(), new SpacingOptions());
            xaxis.Bands.Add(xticks);

            plot.Axes.Add(xaxis);

            var yaxis = new HorizontalVerticalAxis<double, DoubleProvider, Linear, MinMaxFilter<double, DoubleProvider>, double, DoubleAsNaNOptional>(default, default, new MinMaxFilter<double, DoubleProvider>(default, double.MinValue, double.MaxValue))
            {
                Position = AxisPosition.Left,
                Minimum = 0,
                MinimumPadding = 0,
                MaximumPadding = 0,
                MinimumDataMargin = new ScreenReal(10),
                MaximumDataMargin = new ScreenReal(10),
                Title = "Y Axis",
                Key = "Y",
            };

            var yticks = new TickBand<double>(new LinearDoubleTickLocator(), new SpacingOptions());
            yaxis.Bands.Add(yticks);

            plot.Axes.Add(yaxis);

            var lines = new OxyPlot.Axes.ComposableAxis.SeriesExamples.LineSeries<DataSample<DateTime, double>, DateTime, double, IdentityXYSampleProvider<DateTime, double>, AcceptAllFilter<DataSample<DateTime, double>>>(default, default);

            var x = DateTime.Now;
            var y = 10.0;

            var rnd = new Random();

            var xmax = x.AddDays(10);
            while (x <= xmax)
            {
                y += (rnd.NextDouble() - 0.5) * y / 10;

                if (x.DayOfWeek == DayOfWeek.Saturday || x.DayOfWeek == DayOfWeek.Sunday || x.Hour < 8 || x.Hour >= 20)
                {
                    // skip
                }
                else
                {
                    lines.Samples.Add(new DataSample<DateTime, double>(x, y));
                }

                x = x.AddMinutes(1);
            }

            plot.Series.Add(lines);

            return plot;
        }
    }

    /// <summary>
    /// A transformation which is logarithmic for positive values, linear for negative values, and discontinuous between 0 and 1
    /// </summary>
    public struct WeekdayTransformation : IDataTransformation<DateTime, DateTimeProvider>
    {
        /// <inheritdoc/>
        public DateTimeProvider Provider => default;

        /// <inheritdoc/>
        public bool IsNonDiscontinuous => false;

        /// <inheritdoc/>
        public bool IsLinear => throw new NotImplementedException();

        /// <summary>
        /// Initialises a new instance of the <see cref="WeekdayTransformation"/> struct.
        /// </summary>
        /// <param name="openHour"></param>
        /// <param name="closeHour"></param>
        public WeekdayTransformation(double openHour, double closeHour)
        {
            if (openHour < 0 || openHour >= closeHour || closeHour > 24)
                throw new ArgumentException("Open and Close hours do not form a meaningful range");
            
            OpenHour = openHour;
            InternalCloseHour = closeHour - 24;
        }

        /// <summary>
        /// The reference monday.
        /// </summary>
        private static readonly DateTime MondayOfWeek0 = new DateTime(2021, 2, 1, 0, 0, 0);

        /// <summary>
        /// The hour that everything opens.
        /// </summary>
        public double OpenHour { get; }

        private double OpenSubDay => OpenHour / 24;

        /// <summary>
        /// The close hour - 24, so that the default CloseHour is 24
        /// </summary>
        private double InternalCloseHour;

        /// <summary>
        /// The hour that everything closed.
        /// </summary>
        public double CloseHour => InternalCloseHour + 24;
        private double CloseSubDay => CloseHour / 24;

        /// <summary>
        /// How many hours everything is open
        /// </summary>
        private double SubDayWidth => CloseSubDay - OpenSubDay;

        private int GetWeek(DateTime dt)
        {
            var days = (dt - MondayOfWeek0).TotalDays;

            var week = days < 0
                ? -1 - (int)Math.Truncate(-days / 7)
                : (int)Math.Truncate(days / 7);

            return week;
        }

        private int GetDayOfWeek(DateTime dt)
        {
            // this says that sunday is 0... so let's fix that
            var dayOfWeek = ((int)dt.DayOfWeek + 6) % 7;

            if (dayOfWeek > 4)
                dayOfWeek = 4; // crush sat/sun... not sure what else to do with them should they turn out

            return dayOfWeek;
        }

        private double GetSubDay(DateTime dt)
        {
            var subday = dt.TimeOfDay.TotalDays;

            if (subday < OpenSubDay)
                subday = OpenSubDay;
            if (subday > CloseSubDay)
                subday = CloseSubDay;

            subday = (subday - OpenSubDay) / SubDayWidth;

            return subday;
        }

        /// <inheritdoc/>
        public DateTime InverseTransform(InteractionReal x)
        {
            var totalDays = x.Value;
            bool past = totalDays < 0;

            if (past)
                totalDays = -totalDays;

            var weeks = ((int)totalDays) / 5;
            var days = ((int)totalDays) % 5 + (past ? 2 : 0);
            var subday = OpenSubDay + (totalDays - Math.Truncate(totalDays)) * SubDayWidth;

            totalDays = weeks * 7 + days + subday;

            if (past)
                totalDays = -totalDays;

            return MondayOfWeek0 + TimeSpan.FromDays(totalDays);
        }

        /// <inheritdoc/>
        public bool IsDiscontinuous(DateTime a, DateTime b)
        {
            if (OpenHour == 0 && InternalCloseHour == 0)
            {
                // special case
                return GetWeek(a) != GetWeek(b)
                    || a.DayOfWeek == DayOfWeek.Sunday || a.DayOfWeek == DayOfWeek.Saturday
                    || b.DayOfWeek == DayOfWeek.Sunday || b.DayOfWeek == DayOfWeek.Saturday;
            }
            else
            {
                return a.Date != b.Date
                    || a.Hour < OpenHour || a.Hour > CloseHour
                    || b.Hour < OpenHour || b.Hour > CloseHour
                    || a.DayOfWeek == DayOfWeek.Sunday || a.DayOfWeek == DayOfWeek.Saturday
                    || b.DayOfWeek == DayOfWeek.Sunday || b.DayOfWeek == DayOfWeek.Saturday;
            }
        }

        /// <inheritdoc/>
        public InteractionReal Transform(DateTime data)
        {
            var weeks = GetWeek(data);
            var days = GetDayOfWeek(data);
            var subday = GetSubDay(data);

            return new InteractionReal(weeks * 5 + days + subday);
        }
    }

}
