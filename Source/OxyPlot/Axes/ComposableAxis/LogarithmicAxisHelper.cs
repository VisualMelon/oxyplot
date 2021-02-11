using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OxyPlot.Axes.ComposableAxis
{
    /// <summary>
    /// Helps with locating ticks in log-space.
    /// </summary>
    public class LogarithmicAxisTickHelper
    {
        /// <summary>
        /// Initilaises an instance of the <see cref="LogarithmicAxisTickHelper"/> class.
        /// </summary>
        /// <param name="base"></param>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        public LogarithmicAxisTickHelper(double @base, double minimum, double maximum)
        {
            this.Base = @base;
            this.Minimum = minimum;
            this.Maximum = maximum;
            this.LogMinimum = Math.Log(minimum, @base);
            this.LogMaximum = Math.Log(maximum, @base);
        }

        /// <summary>
        /// Gets or sets the Base of the logarithm.
        /// </summary>
        public double Base { get; }

        /// <summary>
        /// The log minimum clip.
        /// </summary>
        public double LogMinimum { get; }

        /// <summary>
        /// The log maximum clip.
        /// </summary>
        public double LogMaximum { get; }

        /// <summary>
        /// The minimum clip.
        /// </summary>
        public double Minimum { get; }

        /// <summary>
        /// The maximum clip.
        /// </summary>
        public double Maximum { get; }

        /// <summary>
        /// Gets the coordinates used to draw ticks and tick labels (numbers or category names).
        /// </summary>
        /// <param name="availableWidth"></param>
        /// <param name="intervalLength"></param>
        /// <param name="majorLabelValues">The major label values.</param>
        /// <param name="majorTickValues">The major tick values.</param>
        /// <param name="minorTickValues">The minor tick values.</param>
        public void GetTickValues(double availableWidth, double intervalLength ,out IList<double> majorLabelValues, out IList<double> majorTickValues, out IList<double> minorTickValues)
        {
            // For easier readability, the nomenclature of this function and all related functions assumes a base of 10, and therefore uses the
            // term "decade". However, the code supports all other bases as well.
            var logBandwidth = Math.Abs(this.LogMaximum - this.LogMinimum);
            var axisBandwidth = availableWidth;

            var desiredNumberOfTicks = axisBandwidth / intervalLength;
            var ticksPerDecade = desiredNumberOfTicks / logBandwidth;
            var logDesiredStepSize = 1.0 / Math.Floor(ticksPerDecade);

            var intBase = Convert.ToInt32(this.Base);

            if (ticksPerDecade < 0.75)
            {   // Major Ticks every few decades (increase in powers of 2), up to eight minor tick subdivisions
                var decadesPerMajorTick = (int)Math.Pow(2, Math.Ceiling(Math.Log(1 / ticksPerDecade, 2)));
                majorTickValues = this.DecadeTicks(decadesPerMajorTick);
                minorTickValues = this.DecadeTicks(Math.Ceiling(decadesPerMajorTick / 8.0));
            }
            else if (Math.Abs(this.Base - intBase) > 1e-10)
            {   // fractional Base, best guess: naively subdivide decades
                majorTickValues = this.DecadeTicks(logDesiredStepSize);
                minorTickValues = this.DecadeTicks(0.5 * logDesiredStepSize);
            }
            else if (ticksPerDecade < 2)
            {   // Major Ticks at every decade, Minor Ticks at fractions (not for fractional base)
                majorTickValues = this.DecadeTicks();
                minorTickValues = this.SubdividedDecadeTicks();
            }
            else if (ticksPerDecade > this.Base * 1.5)
            {   // Fall back to linearly distributed tick values
                var majorInterval = AxisUtilities.CalculateActualIntervalLinear(availableWidth, intervalLength, Maximum - Minimum);
                var minorInterval = AxisUtilities.CalculateMinorInterval(majorInterval);

                minorTickValues = AxisUtilities.CreateTickValues(this.Minimum, this.Maximum, minorInterval);
                majorTickValues = AxisUtilities.CreateTickValues(this.Minimum, this.Maximum, majorInterval);
                majorLabelValues = majorTickValues;

                minorTickValues = AxisUtilities.FilterRedundantMinorTicks(majorTickValues, minorTickValues);
                return;
            }
            else
            {
                // use subdivided decades as major candidates
                var logMajorCandidates = this.LogSubdividedDecadeTicks(false);

                if (logMajorCandidates.Count < 2)
                {   // this should usually not be the case, but if for some reason we should happen to have too few candidates, fall back to linear ticks
                    var majorInterval = AxisUtilities.CalculateActualIntervalLinear(availableWidth, intervalLength, Maximum - Minimum);
                    var minorInterval = AxisUtilities.CalculateMinorInterval(majorInterval);

                    minorTickValues = AxisUtilities.CreateTickValues(this.Minimum, this.Maximum, minorInterval);
                    majorTickValues = AxisUtilities.CreateTickValues(this.Minimum, this.Maximum, majorInterval);
                    majorLabelValues = majorTickValues;

                    minorTickValues = AxisUtilities.FilterRedundantMinorTicks(majorTickValues, minorTickValues);
                    return;
                }

                // check for large candidate intervals; if there are any, subdivide with minor ticks
                var logMinorCandidates = this.LogCalculateMinorCandidates(logMajorCandidates, logDesiredStepSize);

                // use all minor tick candidates that are in the axis range
                minorTickValues = this.PowList(logMinorCandidates, true);

                // find suitable candidates for every desired major step
                majorTickValues = this.AlignTicksToCandidates(logMajorCandidates, logDesiredStepSize);
            }

            majorLabelValues = majorTickValues;
            minorTickValues = AxisUtilities.FilterRedundantMinorTicks(majorTickValues, minorTickValues);
        }

        /// <summary>
        /// Raises all elements of a List to the power of <c>this.Base</c>.
        /// </summary>
        /// <param name="logInput">The input values.</param>
        /// <param name="clip">If true, discards all values that are not in the axis range.</param>
        /// <returns>A new IList containing the resulting values.</returns>
        internal IList<double> PowList(IList<double> logInput, bool clip = false)
        {
            return
                logInput.Where(item => !clip || !(item < this.LogMinimum))
                    .TakeWhile(item => !clip || !(item > this.LogMaximum))
                    .Select(item => Math.Pow(this.Base, item))
                    .ToList();
        }

        /// <summary>
        /// Applies the logarithm with <c>this.Base</c> to all elements of a List.
        /// </summary>
        /// <param name="input">The input values.</param>
        /// <param name="clip">If true, discards all values that are not in the axis range.</param>
        /// <returns>A new IList containing the resulting values.</returns>
        internal IList<double> LogList(IList<double> input, bool clip = false)
        {
            return
                input.Where(item => !clip || !(item < this.Minimum))
                    .TakeWhile(item => !clip || !(item > this.Maximum))
                    .Select(item => Math.Log(item, this.Base))
                    .ToList();
        }

        /// <summary>
        /// Calculates ticks of the decades in the axis range with a specified step size.
        /// </summary>
        /// <param name="step">The step size.</param>
        /// <returns>A new IList containing the decade ticks.</returns>
        internal IList<double> DecadeTicks(double step = 1)
        {
            return this.PowList(this.LogDecadeTicks(step));
        }

        /// <summary>
        /// Calculates logarithmic ticks of the decades in the axis range with a specified step size.
        /// </summary>
        /// <param name="step">The step size.</param>
        /// <returns>A new IList containing the logarithmic decade ticks.</returns>
        internal IList<double> LogDecadeTicks(double step = 1)
        {
            var ret = new List<double>();
            if (step > 0)
            {
                var last = double.NaN;
                for (var exponent = Math.Ceiling(this.LogMinimum); exponent <= this.LogMaximum; exponent += step)
                {
                    if (exponent <= last)
                    {
                        break;
                    }

                    last = exponent;
                    if (exponent >= this.LogMinimum)
                    {
                        ret.Add(exponent);
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Calculates logarithmic ticks of all decades in the axis range and their subdivisions.
        /// </summary>
        /// <param name="clip">If true (default), the lowest and highest decade are clipped to the axis range.</param>
        /// <returns>A new IList containing the logarithmic decade ticks.</returns>
        internal IList<double> LogSubdividedDecadeTicks(bool clip = true)
        {
            return this.LogList(this.SubdividedDecadeTicks(clip));
        }

        /// <summary>
        /// Calculates ticks of all decades in the axis range and their subdivisions.
        /// </summary>
        /// <param name="clip">If true (default), the lowest and highest decade are clipped to the axis range.</param>
        /// <returns>A new IList containing the decade ticks.</returns>
        internal IList<double> SubdividedDecadeTicks(bool clip = true)
        {
            var ret = new List<double>();
            for (var exponent = (int)Math.Floor(this.LogMinimum); ; exponent++)
            {
                if (exponent > this.LogMaximum)
                {
                    break;
                }

                var currentDecade = Math.Pow(this.Base, exponent);
                for (var mantissa = 1; mantissa < this.Base; mantissa++)
                {
                    var currentValue = currentDecade * mantissa;
                    if (clip && currentValue < this.Minimum)
                    {
                        continue;
                    }

                    if (clip && currentValue > this.Maximum)
                    {
                        break;
                    }

                    ret.Add(currentDecade * mantissa);
                }
            }

            return ret;
        }

        /// <summary>
        /// Chooses from a list of candidates so that the resulting List matches the <paramref name="logDesiredStepSize"/> as far as possible.
        /// </summary>
        /// <param name="logCandidates">The candidates.</param>
        /// <param name="logDesiredStepSize">The desired logarithmic step size.</param>
        /// <returns>A new IList containing the chosen candidates.</returns>
        internal IList<double> AlignTicksToCandidates(IList<double> logCandidates, double logDesiredStepSize)
        {
            return this.PowList(this.LogAlignTicksToCandidates(logCandidates, logDesiredStepSize));
        }

        /// <summary>
        /// Chooses from a list of candidates so that the resulting List matches the <paramref name="logDesiredStepSize"/> as far as possible.
        /// </summary>
        /// <param name="logCandidates">The candidates.</param>
        /// <param name="logDesiredStepSize">The desired logarithmic step size.</param>
        /// <returns>A new IList containing the chosen logarithmic candidates.</returns>
        internal IList<double> LogAlignTicksToCandidates(IList<double> logCandidates, double logDesiredStepSize)
        {
            var ret = new List<double>();

            var candidateOffset = 1;
            var logPreviousMajorTick = double.NaN;

            // loop through all desired steps and find a suitable candidate for each of them
            for (var d = Math.Floor(this.LogMinimum); ; d += logDesiredStepSize)
            {
                if (d < this.LogMinimum - logDesiredStepSize)
                {
                    continue;
                }

                if (d > (this.LogMaximum + logDesiredStepSize))
                {
                    break;
                }

                // find closest candidate 
                while (candidateOffset < logCandidates.Count - 1 && logCandidates[candidateOffset] < d)
                {
                    candidateOffset++;
                }

                var logNewMajorTick =
                    Math.Abs(logCandidates[candidateOffset] - d) < Math.Abs(logCandidates[candidateOffset - 1] - d) ?
                    logCandidates[candidateOffset] :
                    logCandidates[candidateOffset - 1];

                // don't add duplicates
                if ((logNewMajorTick != logPreviousMajorTick) && (logNewMajorTick >= this.LogMinimum) && (logNewMajorTick <= this.LogMaximum))
                {
                    ret.Add(logNewMajorTick);
                }

                logPreviousMajorTick = logNewMajorTick;
            }

            return ret;
        }

        /// <summary>
        /// Calculates minor tick candidates for a given set of major candidates.
        /// </summary>
        /// <param name="logMajorCandidates">The major candidates.</param>
        /// <param name="logDesiredMajorStepSize">The desired major step size.</param>
        /// <returns>A new IList containing the minor candidates.</returns>
        internal IList<double> LogCalculateMinorCandidates(IList<double> logMajorCandidates, double logDesiredMajorStepSize)
        {
            var ret = new List<double>();

            for (var c = 1; c < logMajorCandidates.Count; c++)
            {
                var previous = logMajorCandidates[c - 1];
                var current = logMajorCandidates[c];

                if (current < this.LogMinimum)
                {
                    continue;
                }

                if (previous > this.LogMaximum)
                {
                    break;
                }

                var stepSizeRatio = (current - previous) / logDesiredMajorStepSize;
                if (stepSizeRatio > 2)
                {   // Step size is too large... subdivide with minor ticks
                    this.LogSubdivideInterval(ret, this.Base, previous, current);
                }

                ret.Add(current);
            }

            return ret;
        }

        /// <summary>
        /// Subdivides a logarithmic range into multiple, evenly-spaced (in linear scale!) ticks. The number of ticks and the tick intervals are adapted so 
        /// that the resulting steps are "nice" numbers.
        /// </summary>
        /// <param name="logTicks">The IList the computed steps will be added to.</param>
        /// <param name="steps">The minimum number of steps.</param>
        /// <param name="logFrom">The start of the range.</param>
        /// <param name="logTo">The end of the range.</param>
        internal void LogSubdivideInterval(IList<double> logTicks, double steps, double logFrom, double logTo)
        {
            var actualNumberOfSteps = 1;
            var intBase = Convert.ToInt32(this.Base);

            // first, determine actual number of steps that gives a "nice" step size
            if (steps < 2)
            {
                // No Subdivision
                return;
            }

            if (Math.Abs(this.Base - intBase) > this.Base * 1e-10)
            {   // fractional Base; just make a linear subdivision
                actualNumberOfSteps = Convert.ToInt32(steps);
            }
            else if ((intBase & (intBase - 1)) == 0)
            {   // base is a power of 2; use a power of 2 for the stepsize
                while (actualNumberOfSteps < steps)
                {
                    actualNumberOfSteps *= 2;
                }
            }
            else
            {   // integer base, no power of two

                // for bases != 10, first subdivide by the base
                if (intBase != 10)
                {
                    actualNumberOfSteps = intBase;
                }

                // follow 1-2-5-10 pattern
                while (true)
                {
                    if (actualNumberOfSteps >= steps)
                    {
                        break;
                    }

                    actualNumberOfSteps *= 2;

                    if (actualNumberOfSteps >= steps)
                    {
                        break;
                    }

                    actualNumberOfSteps = Convert.ToInt32(actualNumberOfSteps * 2.5);

                    if (actualNumberOfSteps >= steps)
                    {
                        break;
                    }

                    actualNumberOfSteps *= 2;
                }
            }

            var from = Math.Pow(this.Base, logFrom);
            var to = Math.Pow(this.Base, logTo);

            // subdivide with the actual number of steps
            for (var c = 1; c < actualNumberOfSteps; c++)
            {
                var newTick = (double)c / actualNumberOfSteps;
                newTick = Math.Log(from + ((to - from) * newTick), this.Base);

                logTicks.Add(newTick);
            }
        }
    }
}
