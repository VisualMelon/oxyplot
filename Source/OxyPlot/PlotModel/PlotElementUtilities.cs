// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlotModelUtilities.cs" company="OxyPlot">
//   Copyright (c) 2020 OxyPlot contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OxyPlot
{
    /// <summary>
    /// Provides utility functions for plot elements.
    /// </summary>
    public static class PlotElementUtilities
    {
        /// <summary>
        /// Gets the clipping rectangle defined by the Axis the <see cref="IXyAxisPlotElement"/> uses.
        /// </summary>
        /// <param name="element">The <see cref="IXyAxisPlotElement" />.</param>
        /// <returns>The clipping rectangle.</returns>
        public static OxyRect GetClippingRect(IXyAxisPlotElement element)
        {
            var xrect = new OxyRect(element.XAxis.ScreenMin, element.XAxis.ScreenMax);
            var yrect = new OxyRect(element.YAxis.ScreenMin, element.YAxis.ScreenMax);
            return xrect.Intersect(yrect);
        }
    }
}
