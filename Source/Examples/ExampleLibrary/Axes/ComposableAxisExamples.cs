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

            var xaxis = new HorizontalVerticalAxis<double, DoubleProvider, Linear, double, DoubleAsNaNOptional>(default, default)
            {
                Position = AxisPosition.Bottom,
                Minimum = 0,
                Maximum = 10,
                Title = "X Axis",
                Key = "X",
                StartPosition = 0.8,
                EndPosition = 0.2,
            };

            var xticks = new TickBand<double>(new LinearDoubleTickLocator(), new SpacingOptions<double>(20, 3, double.PositiveInfinity, 0.0));
            xaxis.Bands.Add(xticks);

            plot.Axes.Add(xaxis);

            var yaxis = new HorizontalVerticalAxis<double, DoubleProvider, Linear, double, DoubleAsNaNOptional>(default, default)
            {
                Position = AxisPosition.Left,
                Minimum = -1,
                Maximum = 1,
                Title = "Y Axis",
                Key = "Y",
            };

            var yticks = new TickBand<double>(new LinearDoubleTickLocator(), new SpacingOptions<double>(20, 3, double.PositiveInfinity, 0.0));
            yaxis.Bands.Add(yticks);

            plot.Axes.Add(yaxis);

            return plot;
        }

        [Example("Logarithmic XY")]
        public static PlotModel LogarithmicXY()
        {
            var plot = new PlotModel();

            var xaxis = new HorizontalVerticalAxis<double, DoubleProvider, Linear, double, DoubleAsNaNOptional>(default, default)
            {
                Position = AxisPosition.Bottom,
                Minimum = 1,
                Maximum = 10,
                Title = "X Axis",
                Key = "X",
            };

            var xticks = new TickBand<double>(new LinearDoubleTickLocator(), new SpacingOptions<double>(20, 3, double.PositiveInfinity, 0.0));
            xaxis.Bands.Add(xticks);

            plot.Axes.Add(xaxis);

            var yaxis = new HorizontalVerticalAxis<double, DoubleProvider, Logarithmic, double, DoubleAsNaNOptional>(default, default)
            {
                Position = AxisPosition.Left,
                Minimum = 1,
                Maximum = 100,
                Title = "Y Axis (Log)",
                Key = "Y",
            };

            var yticks = new TickBand<double>(new LogarithmicDoubleTickLocator(), new SpacingOptions<double>(20, 3, double.PositiveInfinity, 0.0));
            yaxis.Bands.Add(yticks);

            plot.Axes.Add(yaxis);

            return plot;
        }

        [Example("Line on Linear XY")]
        public static PlotModel LineOnLinearXY()
        {
            var plot = LinearXY();

            var lines = new OxyPlot.Axes.ComposableAxis.SeriesExamples.LineSeries<DataPoint, double, double, DataPointXYSampleProvider>(default)
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

        [Example("Line on Linear XY, Transposed")]
        public static PlotModel LineOnLinearXYTransposed()
        {
            var plot = LinearXY();

            plot.Axes[0].Position = AxisPosition.Left;
            plot.Axes[1].Position = AxisPosition.Bottom;

            var lines = new OxyPlot.Axes.ComposableAxis.SeriesExamples.LineSeries<DataPoint, double, double, DataPointXYSampleProvider>(default)
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

        [Example("Line on Logarithmic XY")]
        public static PlotModel LineOnLogarithmicXY()
        {
            var plot = LogarithmicXY();

            var lines = new OxyPlot.Axes.ComposableAxis.SeriesExamples.LineSeries<DataPoint, double, double, DataPointXYSampleProvider>(default)
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

        [Example("Many Points")]
        public static PlotModel ManyPoints()
        {
            var plot = new PlotModel()
            {
                Subtitle = "10^8 points",
            };

            var xaxis = new HorizontalVerticalAxis<double, DoubleProvider, Linear, double, DoubleAsNaNOptional>(default, default)
            {
                Position = AxisPosition.Bottom,
                Title = "X",
            };

            var xticks = new TickBand<double>(new LinearDoubleTickLocator(), new SpacingOptions<double>(20, 3, double.PositiveInfinity, 0.0));
            xaxis.Bands.Add(xticks);
            plot.Axes.Add(xaxis);
            
            var yaxis = new HorizontalVerticalAxis<double, DoubleProvider, Linear, double, DoubleAsNaNOptional>(default, default)
            {
                Position = AxisPosition.Left,
                Title = "X",
            };

            var yticks = new TickBand<double>(new LinearDoubleTickLocator(), new SpacingOptions<double>(20, 3, double.PositiveInfinity, 0.0));
            yaxis.Bands.Add(yticks);
            plot.Axes.Add(yaxis);

            var lines = new OxyPlot.Axes.ComposableAxis.SeriesExamples.LineSeries<DataPoint, double, double, DataPointXYSampleProvider>(default);
            for (var x = 1.0; x <= 1000; x += 0.001)
            {
                lines.Samples.Add(new DataPoint(x, Math.Sin(x)));
            }
            plot.Series.Add(lines);

            return plot;
        }

        [Example("Axis Margins, Data Margins, and Padding, Asymmetrical")]
        public static PlotModel MarginsAndPaddingAsymmetrical()
        {
            var plot = new PlotModel
            {
                Title = "Try resizing the plot",
                Subtitle = "ClipMinimum/Maximum are Blue\nActualMinimum/Maximum are Red\nDataMinimum/Maximum are Green"
            };

            var xaxis = new HorizontalVerticalAxis<double, DoubleProvider, Linear, double, DoubleAsNaNOptional>(default, default)
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

            var yaxis = new HorizontalVerticalAxis<double, DoubleProvider, Linear, double, DoubleAsNaNOptional>(default, default)
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

            var rectangle = new OxyPlot.Axes.ComposableAxis.SeriesExamples.LineSeries<DataPoint, double, double, DataPointXYSampleProvider>(default);
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
    }
}
