using System;
using System.Collections.Generic;
using System.Text;

namespace OxyPlot.Axes.ComposableAxis
{
    /// <summary>
    /// Provides methods to help working with axis.
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Computes the minimum and maximum X and Y values for the given samples.
        /// </summary>
        /// <typeparam name="TSample"></typeparam>
        /// <typeparam name="TSampleProvider"></typeparam>
        /// <typeparam name="XData"></typeparam>
        /// <typeparam name="YData"></typeparam>
        /// <typeparam name="XDataProvider"></typeparam>
        /// <typeparam name="YDataProvider"></typeparam>
        /// <param name="sampleProvider"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="samples"></param>
        /// <param name="minX"></param>
        /// <param name="minY"></param>
        /// <param name="maxX"></param>
        /// <param name="maxY"></param>
        /// <returns></returns>
        public static bool TryFindMinMax<TSample, TSampleProvider, XData, YData, XDataProvider, YDataProvider>(TSampleProvider sampleProvider, XDataProvider x, YDataProvider y, IReadOnlyList<TSample> samples, out XData minX, out YData minY, out XData maxX, out YData maxY)
            where TSampleProvider : IXYSampleProvider<TSample, XData, YData>
            where XDataProvider : IDataProvider<XData>
            where YDataProvider : IDataProvider<YData>
        {
            minX = default(XData);
            minY = default(YData);
            maxX = default(XData);
            maxY = default(YData);

            bool first = true;

            foreach (var sample in samples)
            {
                if (sampleProvider.TrySample(sample, out var xySample))
                {
                    if (first)
                    {
                        minX = maxX = xySample.X;
                        minY = maxY = xySample.Y;
                        first = false;
                    }
                    else
                    {
                        minX = x.Min(minX, xySample.X);
                        maxX = x.Max(maxX, xySample.X);
                        minY = y.Min(minY, xySample.Y);
                        maxY = y.Max(maxY, xySample.Y);
                    }
                }
            }

            return !first;
        }

        /// <summary>
        /// Computes the minimum and maximum X and Y values for the given samples.
        /// </summary>
        /// <typeparam name="TSample"></typeparam>
        /// <typeparam name="TSampleProvider"></typeparam>
        /// <typeparam name="XData"></typeparam>
        /// <typeparam name="YData"></typeparam>
        /// <typeparam name="XDataProvider"></typeparam>
        /// <typeparam name="YDataProvider"></typeparam>
        /// <param name="sampleProvider"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="samples"></param>
        /// <param name="minX"></param>
        /// <param name="minY"></param>
        /// <param name="maxX"></param>
        /// <param name="maxY"></param>
        /// <param name="xMonotonicity"></param>
        /// <param name="yMonotonicity"></param>
        /// <returns></returns>
        public static bool TryFindMinMax<TSample, TSampleProvider, XData, YData, XDataProvider, YDataProvider>(TSampleProvider sampleProvider, XDataProvider x, YDataProvider y, IReadOnlyList<TSample> samples, out XData minX, out YData minY, out XData maxX, out YData maxY, out Monotonicity xMonotonicity, out Monotonicity yMonotonicity)
            where TSampleProvider : IXYSampleProvider<TSample, XData, YData>
            where XDataProvider : IDataProvider<XData>
            where YDataProvider : IDataProvider<YData>
        {
            var xh = new SequenceHelper<XData, XDataProvider>(x);
            var yh = new SequenceHelper<YData, YDataProvider>(y);

            foreach (var sample in samples)
            {
                if (sampleProvider.TrySample(sample, out var xySample))
                {
                    xh.Next(xySample.X);
                    yh.Next(xySample.Y);
                }
            }

            minX = xh.Minimum;
            minY = yh.Minimum;
            maxX = xh.Maximum;
            maxY = yh.Maximum;
            xMonotonicity = xh.Monotonicity;
            yMonotonicity = yh.Monotonicity;

            return !xh.IsEmpty;
        }
    }

    /// <summary>
    /// Provides methods to help with rendering on axis.
    /// </summary>
    public static class RenderHelpers
    {
        /// <summary>
        /// Extracts a single contiguous line segment beginning with the element at the position of the enumerator when the method
        /// is called. Invalid samples are ignored.
        /// </summary>
        /// <typeparam name="TSample"></typeparam>
        /// <typeparam name="TSampleProvider"></typeparam>
        /// <typeparam name="XData"></typeparam>
        /// <typeparam name="YData"></typeparam>
        /// <typeparam name="XDataProvider"></typeparam>
        /// <typeparam name="YDataProvider"></typeparam>
        /// <typeparam name="XAxisTransformation"></typeparam>
        /// <typeparam name="YAxisTransformation"></typeparam>
        /// <param name="sampleProvider"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="samples">Points collection</param>
        /// <param name="sampleIdx">Current sample index</param>
        /// <param name="previousContiguousLineSegmentEndPoint">Initially set to null, but I will update I won't give a broken line if this is null</param>
        /// <param name="previousContiguousLineSegmentEndPointWithinClipBounds">Where the previous end segment was within the clip bounds</param>
        /// <param name="broken">place to put broken segment</param>
        /// <param name="continuous">place to put contiguous segment</param>
        /// <returns>
        ///   <c>true</c> if line segments are extracted, <c>false</c> if reached end.
        /// </returns>
        public static bool ExtractNextContinuousLineSegment<TSample, TSampleProvider, XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation>(TSampleProvider sampleProvider, XAxisTransformation x, YAxisTransformation y, IReadOnlyList<TSample> samples, ref int sampleIdx, ref ScreenPoint? previousContiguousLineSegmentEndPoint, ref bool previousContiguousLineSegmentEndPointWithinClipBounds, List<ScreenPoint> broken, List<ScreenPoint> continuous)
            where TSampleProvider : IXYSampleProvider<TSample, XData, YData>
            where XDataProvider : IDataProvider<XData>
            where YDataProvider : IDataProvider<YData>
            where XAxisTransformation : IAxisScreenTransformation<XData, XDataProvider>
            where YAxisTransformation : IAxisScreenTransformation<YData, YDataProvider>
        {
            // Need to:
            //  - reject invalid points, forming broken line segments as necessary
            //  - reject points outside the axis bounds

            TSample currentSample = default(TSample);
            DataSample<XData, YData> currentXYSample = default(DataSample<XData, YData>);
            bool hasValidPoint = false;

            // Skip all undefined points
            while (sampleIdx < samples.Count && !sampleProvider.TrySample(currentSample = samples[sampleIdx], out currentXYSample))
            {
                sampleIdx++;
            }

            if (!hasValidPoint)
            {
                // ran out of samples
                return false;
            }

            var currentSampleWithinClipBounds = currentXYSample.WithinClipBounds<XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation>(x, y);
            var currentPoint = currentXYSample.Transform<XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation>(x, y);

            // Handle broken line segment if exists
            // Requires that there is a previous segment, and someone is within the clip bounds
            if (previousContiguousLineSegmentEndPoint.HasValue && (previousContiguousLineSegmentEndPointWithinClipBounds || currentSampleWithinClipBounds))
            {
                // TODO: we should check for discontenuity also, but can't with current API: should ref a TSample? rather than ScreenPoint?
                broken.Add(previousContiguousLineSegmentEndPoint.Value);
                broken.Add(currentPoint);
            }
            previousContiguousLineSegmentEndPoint = null;

            TSample lastSample = default(TSample);
            DataSample<XData, YData> lastXYSample = default(DataSample<XData, YData>);
            ScreenPoint? lastPoint = default(ScreenPoint?);
            bool lastSampleWithinClipBounds = default(bool);

            bool haveLast = false;
            bool firstSample = true;
            bool addedSamples = false;
            while (sampleIdx < samples.Count)
            {
                var currentSampleIsValid = sampleProvider.TrySample(currentSample = samples[sampleIdx], out currentXYSample);

                if (!currentSampleIsValid)
                {
                    // we are invalid: skip current and break
                    sampleIdx++;
                    break;
                }

                currentSampleWithinClipBounds = currentXYSample.WithinClipBounds<XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation>(x, y);
                currentPoint = currentXYSample.Transform<XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation>(x, y);

                if (haveLast)
                {
                    if (x.IsDiscontinuous(lastXYSample.X, currentXYSample.X))
                    {
                        if (firstSample)
                        {
                            // just reset
                            haveLast = false;
                        }
                        else
                        {
                            // this is a break in continuity: clear last point, and break
                            lastPoint = null;
                            break;
                        }
                    }
                }

                if (!currentSampleWithinClipBounds)
                {
                    if (firstSample)
                    {
                        // fine, we just advance
                    }
                    else if (haveLast && !lastSampleWithinClipBounds)
                    {
                        // two in a row out of bounds: break
                        break;
                    }
                    else
                    {
                        // the previous guy was in-bounds, so we need to be added
                        continuous.Add(currentPoint);
                        addedSamples = true;
                    }
                }
                else
                {
                    if (firstSample)
                    {
                        firstSample = false;
                        if (haveLast)
                        {
                            // add the last point
                            continuous.Add(lastPoint.Value);
                        }
                    }

                    continuous.Add(currentPoint);
                    addedSamples = true;
                }

                // advance
                sampleIdx++;

                lastSample = currentSample;
                lastPoint = currentPoint;
                lastXYSample = currentXYSample;
                lastSampleWithinClipBounds = currentSampleWithinClipBounds;
                haveLast = true;
            }

            if (addedSamples)
            {
                previousContiguousLineSegmentEndPoint = lastPoint;
                previousContiguousLineSegmentEndPointWithinClipBounds = lastSampleWithinClipBounds;
            }

            return addedSamples;
        }

        /// <summary>
        /// Transforms a single <see cref="DataSample{XData, YData}"/> to screen space.
        /// </summary>
        /// <typeparam name="XData"></typeparam>
        /// <typeparam name="YData"></typeparam>
        /// <typeparam name="XDataProvider"></typeparam>
        /// <typeparam name="YDataProvider"></typeparam>
        /// <typeparam name="XAxisTransformation"></typeparam>
        /// <typeparam name="YAxisTransformation"></typeparam>
        /// <param name="sample"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>A screen point</returns>
        public static ScreenPoint Transform<XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation>(this DataSample<XData, YData> sample, XAxisTransformation x, YAxisTransformation y)
            where XDataProvider : IDataProvider<XData>
            where YDataProvider : IDataProvider<YData>
            where XAxisTransformation : IAxisScreenTransformation<XData, XDataProvider>
            where YAxisTransformation : IAxisScreenTransformation<YData, YDataProvider>
        {
            return new ScreenPoint(x.Transform(sample.X).Value, y.Transform(sample.Y).Value);
        }

        /// <summary>
        /// Determines whether a <see cref="DataSample{XData, YData}"/> is within the clip bounds.
        /// </summary>
        /// <typeparam name="XData"></typeparam>
        /// <typeparam name="YData"></typeparam>
        /// <typeparam name="XDataProvider"></typeparam>
        /// <typeparam name="YDataProvider"></typeparam>
        /// <typeparam name="XAxisTransformation"></typeparam>
        /// <typeparam name="YAxisTransformation"></typeparam>
        /// <param name="sample"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>A screen point</returns>
        public static bool WithinClipBounds<XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation>(this DataSample<XData, YData> sample, XAxisTransformation x, YAxisTransformation y)
            where XDataProvider : IDataProvider<XData>
            where YDataProvider : IDataProvider<YData>
            where XAxisTransformation : IAxisScreenTransformation<XData, XDataProvider>
            where YAxisTransformation : IAxisScreenTransformation<YData, YDataProvider>
        {
            return x.WithinClipBounds(sample.X) && y.WithinClipBounds(sample.Y);
        }

        /// <summary>
        /// Transforms a list of <see cref="DataSample{XData, YData}"/> along two axes.
        /// </summary>
        /// <typeparam name="XData"></typeparam>
        /// <typeparam name="YData"></typeparam>
        /// <typeparam name="XDataProvider"></typeparam>
        /// <typeparam name="YDataProvider"></typeparam>
        /// <typeparam name="XAxisTransformation"></typeparam>
        /// <typeparam name="YAxisTransformation"></typeparam>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="dataSamples"></param>
        /// <param name="screenPoints">The output buffer. Invalid values indiciate discontenuities in the axis or data spaces.</param>
        public static void TransformSamples<XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation>(XAxisTransformation x, YAxisTransformation y, IReadOnlyList<DataSample<XData, YData>> dataSamples, IList<ScreenPoint> screenPoints)
            where XDataProvider : IDataProvider<XData>
            where YDataProvider : IDataProvider<YData>
            where XAxisTransformation : IAxisScreenTransformation<XData, XDataProvider>
            where YAxisTransformation : IAxisScreenTransformation<YData, YDataProvider>
        {
            if (dataSamples.Count < 1)
                return;

            foreach (var s in dataSamples)
                screenPoints.Add(s.Transform<XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation>(x, y));
        }

        /// <summary>
        /// Interpolates lines on two axis.
        /// </summary>
        /// <typeparam name="XData"></typeparam>
        /// <typeparam name="YData"></typeparam>
        /// <typeparam name="XDataProvider"></typeparam>
        /// <typeparam name="YDataProvider"></typeparam>
        /// <typeparam name="XAxisTransformation"></typeparam>
        /// <typeparam name="YAxisTransformation"></typeparam>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="dataSamples"></param>
        /// <param name="minSegmentLength"></param>
        /// <param name="screenPoints">The output buffer. Invalid values indiciate discontenuities in the axis or data spaces.</param>
        public static void InterpolateLines<XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation>(XAxisTransformation x, YAxisTransformation y, IReadOnlyList<DataSample<XData, YData>> dataSamples, double minSegmentLength, IList<ScreenPoint> screenPoints)
            where XDataProvider : IDataProvider<XData>
            where YDataProvider : IDataProvider<YData>
            where XAxisTransformation : IAxisScreenTransformation<XData, XDataProvider>
            where YAxisTransformation : IAxisScreenTransformation<YData, YDataProvider>
        {
            if (dataSamples.Count < 2)
                return;

            if (minSegmentLength <= 0)
                throw new ArgumentOutOfRangeException(nameof(minSegmentLength), "Should be positive");

            if (x.Provider.IsDiscrete || y.Provider.IsDiscrete)
                throw new ArgumentException("Both transformations must have a non-discrete data space.");

            var bothLinear = x.IsNonDiscontinuous && y.IsLinear;
            var bothContinuous = x.IsNonDiscontinuous && y.IsNonDiscontinuous;

            // fast paths
            if (bothLinear && bothContinuous)
            {
                foreach (var s in dataSamples)
                    screenPoints.Add(s.Transform<XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation>(x, y));
            }
            else
            {
                // all-powerful path

                var s1 = dataSamples[0];
                var p1 = s1.Transform<XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation>(x, y);
                int si = 0;

                bool omitFirst = false;

                while (si < dataSamples.Count)
                {
                    // advance
                    var s0 = s1;
                    var p0 = p1;

                    si++;

                    s1 = dataSamples[si];
                    p1 = s1.Transform<XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation>(x, y);

                    InterpolateLine<XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation>(x, y, s0, s1, minSegmentLength, screenPoints, omitFirst, true);
                }

            }
        }

        /// <summary>
        /// Interpolates between two screen points on two axis.
        /// </summary>
        /// <typeparam name="XData"></typeparam>
        /// <typeparam name="YData"></typeparam>
        /// <typeparam name="XDataProvider"></typeparam>
        /// <typeparam name="YDataProvider"></typeparam>
        /// <typeparam name="XAxisTransformation"></typeparam>
        /// <typeparam name="YAxisTransformation"></typeparam>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="s0"></param>
        /// <param name="s1"></param>
        /// <param name="minSegmentLength"></param>
        /// <param name="omitFirst">A value indiciating whether to not add the first screen point (corresponding to <paramref name="s0"/>) to the output buffer.</param>
        /// <param name="xprinciple">Whether to divide on the x axis by default.</param>
        /// <param name="screenPoints">The output buffer. Invalid values indiciate discontenuities in the axis or data spaces.</param>
        private static void InterpolateLine<XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation>(XAxisTransformation x, YAxisTransformation y, DataSample<XData, YData> s0, DataSample<XData, YData> s1, double minSegmentLength, IList<ScreenPoint> screenPoints, bool omitFirst, bool xprinciple)
            where XDataProvider : IDataProvider<XData>
            where YDataProvider : IDataProvider<YData>
            where XAxisTransformation : IAxisScreenTransformation<XData, XDataProvider>
            where YAxisTransformation : IAxisScreenTransformation<YData, YDataProvider>
        {
            // NOTE: this is really inefficient at the moment, but that's fine
            // TODO: make non-recursive, make it not re-compute interpolations, make it not transform everything twice

            // assumptions that should be enforced by the caller
            System.Diagnostics.Debug.Assert(!x.Provider.IsDiscrete);
            System.Diagnostics.Debug.Assert(!y.Provider.IsDiscrete);
            System.Diagnostics.Debug.Assert(!x.IsDiscontinuous(s0.X, s1.X));
            System.Diagnostics.Debug.Assert(!y.IsDiscontinuous(s0.Y, s1.Y));

            var minLengthSquared = minSegmentLength * minSegmentLength;

            var p0 = s0.Transform<XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation>(x, y);
            var p1 = s1.Transform<XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation>(x, y);

            if (!omitFirst)
                screenPoints.Add(p0);

            // fast path
            if (p0.DistanceToSquared(p1) < minLengthSquared)
            {
                screenPoints.Add(p1);
            }
            else
            {
                double c;

                // NOTE: this would be more efficient than flip-flopping, but flip-flopping may provide more stable outputs (which is important)
                //xprinciple = Math.Abs(p1.X - p0.X) >= Math.Abs(p1.Y - p0.Y);

                if (xprinciple)
                {
                    var m = x.InverseTransform(new ScreenReal((p0.X + p1.X) / 2));
                    c = x.Provider.Deinterpolate(s0.X, s1.X, m);
                }
                else
                {
                    var m = y.InverseTransform(new ScreenReal((p0.Y + p1.Y) / 2));
                    c = y.Provider.Deinterpolate(s0.Y, s1.Y, m);
                }

                // NOTE: not a real sample
                var sm = new DataSample<XData, YData>(x.Provider.Interpolate(s0.X, s1.X, c), y.Provider.Interpolate(s0.Y, s1.Y, c));
                var pm = sm.Transform<XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation>(x, y);

                if (p0.DistanceToSquared(pm) < minLengthSquared)
                {
                    screenPoints.Add(pm);
                }
                else
                {
                    InterpolateLine<XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation>(x, y, s0, sm, minSegmentLength, screenPoints, true, !xprinciple);
                }

                if (pm.DistanceToSquared(p1) < minLengthSquared)
                {
                    screenPoints.Add(p1);
                }
                else
                {
                    InterpolateLine<XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation>(x, y, sm, s1, minSegmentLength, screenPoints, true, !xprinciple);
                }
            }
        }
    }
}
