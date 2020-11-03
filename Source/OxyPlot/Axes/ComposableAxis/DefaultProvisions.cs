using OxyPlot.Axes.ComposableAxes;
using System;
using System.Collections.Generic;
using System.Text;

namespace OxyPlot.Axes.ComposableAxis
{
    /// <summary>
    /// A linear (no-op) data projection.
    /// </summary>
    public struct Linear : IDataProjection<double>
    {
        /// <inheritdoc/>
        public bool IsSemiContinuous => true;

        /// <inheritdoc/>
        public double InverseProject(double x)
        {
            return x;
        }

        /// <inheritdoc/>
        public bool IsBetween(double value, double min, double max)
        {
            return value >= min && value <= max;
        }

        /// <inheritdoc/>
        public void LocateBreaks(double data, IList<double> breaks)
        {
        }

        /// <inheritdoc/>
        public double Project(double data)
        {
            return data;
        }
    }

    /// <summary>
    /// A logarithmic data projection.
    /// </summary>
    public struct Logarithmic : IDataProjection<double>
    {
        /// <inheritdoc/>
        public bool IsSemiContinuous => true;

        /// <inheritdoc/>
        public double InverseProject(double x)
        {
            return Math.Exp(x);
        }

        /// <inheritdoc/>
        public bool IsBetween(double value, double min, double max)
        {
            return value >= min && value <= max;
        }

        /// <inheritdoc/>
        public void LocateBreaks(double data, IList<double> breaks)
        {
        }

        /// <inheritdoc/>
        public double Project(double data)
        {
            return Math.Log(data);
        }
    }
}
