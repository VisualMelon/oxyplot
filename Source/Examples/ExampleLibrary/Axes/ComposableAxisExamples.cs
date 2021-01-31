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

            plot.Axes.Add(xaxis);

            var yaxis = new HorizontalVerticalAxis<double, DoubleProvider, Logarithmic, double, DoubleAsNaNOptional>(default, default)
            {
                Position = AxisPosition.Left,
                Minimum = 1,
                Maximum = 100,
                Title = "Y Axis (Log)",
                Key = "Y",
            };

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

            var lines = new OxyPlot.Axes.ComposableAxis.SeriesExamples.LineSeries<DataPoint, double, double, DataPointXYSampleProvider>(default);

            for (var x = 1.0; x <= 10; x += 0.01)
            {
                lines.Samples.Add(new DataPoint(x, x * x));
            }

            plot.Series.Add(lines);

            return plot;
        }
    }
}
