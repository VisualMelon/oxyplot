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
        [Example("Linear X/Y")]
        public static PlotModel LinearXY()
        {
            var plot = new PlotModel();

            var xaxis = new HorizontalVerticalAxis<double, DoubleProvider, Linear, double, DoubleAsNaNOptional>(default)
            {
                Position = AxisPosition.Bottom,
                Minimum = 0,
                Maximum = 10,
                Title = "X Axis",
            };

            plot.Axes.Add(xaxis);

            var yaxis = new HorizontalVerticalAxis<double, DoubleProvider, Linear, double, DoubleAsNaNOptional>(default)
            {
                Position = AxisPosition.Left,
                Minimum = -1,
                Maximum = 1,
                Title = "Y Axis",
            };

            plot.Axes.Add(yaxis);

            return plot;
        }

        [Example("Line on LinearXY")]
        public static PlotModel LineOnLinearXY()
        {
            var plot = LinearXY();

            var lines = new OxyPlot.Axes.ComposableAxis.SeriesExamples.LineSeries<DataPoint, double, double, DataPointXYSampleProvider>(default);
            
            for (var x = 0.0; x < 10; x += 0.001)
            {
                lines.Samples.Add(new DataPoint(x, Math.Sin(x)));
            }
            
            plot.Series.Add(lines);

            return plot;
        }
    }
}
