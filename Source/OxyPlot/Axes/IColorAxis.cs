// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IColorAxis.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Specifies functionality for color axes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using OxyPlot.Axes.ComposableAxis;

namespace OxyPlot.Axes
{
    /// <summary>
    /// Specifies functionality for color axes.
    /// </summary>
    public interface IColorAxis : IPlotElement, IColorAxis<double>
    {
    }
}
