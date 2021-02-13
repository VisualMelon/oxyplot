// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IXyAxisPlotElement.cs" company="OxyPlot">
//   Copyright (c) 2020 OxyPlot contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OxyPlot
{
    using OxyPlot.Axes;
    using OxyPlot.Axes.ComposableAxis;

    /// <summary>
    /// Defines a plot element that uses an X and a Y axis.
    /// </summary>
    public interface IXyAxisPlotElement : IPlotElement
    {
        /// <summary>
        /// Gets the X axis.
        /// </summary>
        IPrettyAxis XAxis { get; }

        /// <summary>
        /// Gets the Y axis.
        /// </summary>
        IPrettyAxis YAxis { get; }
    }
}
