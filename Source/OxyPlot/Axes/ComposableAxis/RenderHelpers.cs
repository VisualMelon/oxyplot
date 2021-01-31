using System;
using System.Collections.Generic;
using System.Text;

namespace OxyPlot.Axes.ComposableAxis
{
    /// <summary>
    /// Provides methods to help working with axis.
    /// </summary>
    public static class XYHelpers
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

        /// <summary>
        /// Finds a window in some monotonic data.
        /// </summary>
        /// <typeparam name="TSample"></typeparam>
        /// <typeparam name="TSampleProvider"></typeparam>
        /// <typeparam name="XData"></typeparam>
        /// <typeparam name="YData"></typeparam>
        /// <typeparam name="XDataProvider"></typeparam>
        /// <typeparam name="YDataProvider"></typeparam>
        /// <param name="sampleProvider"></param>
        /// <param name="xDataProvider"></param>
        /// <param name="yDataProvider"></param>
        /// <param name="samples"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="xMonotonicity"></param>
        /// <param name="yMonotonicity"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public static bool FindWindow<TSample, TSampleProvider, XData, YData, XDataProvider, YDataProvider>(TSampleProvider sampleProvider, XDataProvider xDataProvider, YDataProvider yDataProvider, IReadOnlyList<TSample> samples, DataSample<XData, YData> start, DataSample<XData, YData> end, Monotonicity xMonotonicity, Monotonicity yMonotonicity, out int startIndex, out int endIndex)
            where TSampleProvider : IXYSampleProvider<TSample, XData, YData>
            where XDataProvider : IDataProvider<XData>
            where YDataProvider : IDataProvider<YData>
        {
            int xsign = xMonotonicity.IsNonDecreasing && !xMonotonicity.IsConstant ? 1 : xMonotonicity.IsNonIncreasing ? -1 : 0;
            int ysign = yMonotonicity.IsNonDecreasing && !xMonotonicity.IsConstant ? 1 : yMonotonicity.IsNonIncreasing ? -1 : 0;

            if (xsign == 0 && ysign == 0)
            {
                startIndex = 0;
                endIndex = samples.Count - 1;
                throw new ArgumentException("Either the X or Y values must be monotonic");
            }

            bool startOk = FindWindowStart<TSample, TSampleProvider, XData, YData, XDataProvider, YDataProvider>(sampleProvider, xDataProvider, yDataProvider, samples, start, xMonotonicity, yMonotonicity, out startIndex);
            bool endOk = FindWindowEnd<TSample, TSampleProvider, XData, YData, XDataProvider, YDataProvider>(sampleProvider, xDataProvider, yDataProvider, samples, end, xMonotonicity, yMonotonicity, out endIndex);

            return startOk && endOk;
        }

        /// <summary>
        /// Finds the start of a window in some monotonic data.
        /// </summary>
        /// <typeparam name="TSample"></typeparam>
        /// <typeparam name="TSampleProvider"></typeparam>
        /// <typeparam name="XData"></typeparam>
        /// <typeparam name="YData"></typeparam>
        /// <typeparam name="XDataProvider"></typeparam>
        /// <typeparam name="YDataProvider"></typeparam>
        /// <param name="sampleProvider"></param>
        /// <param name="xDataProvider"></param>
        /// <param name="yDataProvider"></param>
        /// <param name="samples"></param>
        /// <param name="start"></param>
        /// <param name="xMonotonicity"></param>
        /// <param name="yMonotonicity"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static bool FindWindowStart<TSample, TSampleProvider, XData, YData, XDataProvider, YDataProvider>(TSampleProvider sampleProvider, XDataProvider xDataProvider, YDataProvider yDataProvider, IReadOnlyList<TSample> samples, DataSample<XData, YData> start, Monotonicity xMonotonicity, Monotonicity yMonotonicity, out int startIndex)
            where TSampleProvider : IXYSampleProvider<TSample, XData, YData>
            where XDataProvider : IDataProvider<XData>
            where YDataProvider : IDataProvider<YData>
        {
            int xsign = xMonotonicity.IsNonDecreasing && !xMonotonicity.IsConstant ? 1 : xMonotonicity.IsNonIncreasing ? -1 : 0;
            int ysign = yMonotonicity.IsNonDecreasing && !xMonotonicity.IsConstant ? 1 : yMonotonicity.IsNonIncreasing ? -1 : 0;

            if (xsign == 0 && ysign == 0)
            {
                startIndex = 0;
                throw new ArgumentException("Either the X or Y values must be monotonic");
            }

            int l = 0;
            int h = samples.Count - 1;

            while (l < h)
            {
            cont:
                int m = (h + l) / 2;

                var i = m;
                DataSample<XData, YData> candidate; // whatever is at index i

                // now, the hideousness of dealing with NaNs
                while (!sampleProvider.TrySample(samples[i], out candidate))
                {
                    // it was invalid, we'd better scan along a bit
                    i++;

                    if (i >= h)
                    {
                        // oh, we ran out this side... best collapse it 
                        h = m - 1;
                        goto cont;
                    }
                }

                int cx = xsign != 0 ? xDataProvider.Compare(candidate.X, start.X) * xsign : 0;
                int cy = ysign != 0 ? yDataProvider.Compare(candidate.Y, start.Y) * ysign : 0;

                if (cx >= 0 || cy >= 0)
                {
                    h = i - 1;
                }
                else
                {
                    l = m + 1;
                }
            }

            startIndex = l;
            return true;
        }

        /// <summary>
        /// Finds the end of a window in some monotonic data.
        /// </summary>
        /// <typeparam name="TSample"></typeparam>
        /// <typeparam name="TSampleProvider"></typeparam>
        /// <typeparam name="XData"></typeparam>
        /// <typeparam name="YData"></typeparam>
        /// <typeparam name="XDataProvider"></typeparam>
        /// <typeparam name="YDataProvider"></typeparam>
        /// <param name="sampleProvider"></param>
        /// <param name="xDataProvider"></param>
        /// <param name="yDataProvider"></param>
        /// <param name="samples"></param>
        /// <param name="end"></param>
        /// <param name="xMonotonicity"></param>
        /// <param name="yMonotonicity"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public static bool FindWindowEnd<TSample, TSampleProvider, XData, YData, XDataProvider, YDataProvider>(TSampleProvider sampleProvider, XDataProvider xDataProvider, YDataProvider yDataProvider, IReadOnlyList<TSample> samples, DataSample<XData, YData> end, Monotonicity xMonotonicity, Monotonicity yMonotonicity, out int endIndex)
            where TSampleProvider : IXYSampleProvider<TSample, XData, YData>
            where XDataProvider : IDataProvider<XData>
            where YDataProvider : IDataProvider<YData>
        {
            int xsign = xMonotonicity.IsNonDecreasing && !xMonotonicity.IsConstant ? 1 : xMonotonicity.IsNonIncreasing ? -1 : 0;
            int ysign = yMonotonicity.IsNonDecreasing && !xMonotonicity.IsConstant ? 1 : yMonotonicity.IsNonIncreasing ? -1 : 0;

            if (xsign == 0 && ysign == 0)
            {
                endIndex = samples.Count - 1;
                throw new ArgumentException("Either the X or Y values must be monotonic and non-constant");
            }

            int l = 0;
            int h = samples.Count - 1;

            while (l < h)
            {
            cont:
                int m = (h + l + 1) / 2;

                var i = m;
                DataSample<XData, YData> candidate; // whatever is at index i

                // now, the hideousness of dealing with NaNs
                while (!sampleProvider.TrySample(samples[i], out candidate))
                {
                    // it was invalid, we'd better scan along a bit
                    i++;

                    if (i >= h)
                    {
                        // oh, we ran out this side... best collapse it 
                        h = m - 1;
                        goto cont;
                    }
                }

                int cx = xsign != 0 ? xDataProvider.Compare(candidate.X, end.X) * xsign : 0;
                int cy = ysign != 0 ? yDataProvider.Compare(candidate.Y, end.Y) * ysign : 0;

                if (cx <= 0 || cy <= 0)
                {
                    l = m + 1;
                }
                else
                {
                    h = i - 1;
                }
            }

            endIndex = h;
            return true;
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
        /// <typeparam name="XYTransformation"></typeparam>
        /// <param name="sampleProvider"></param>
        /// <param name="transformation"></param>
        /// <param name="samples">Points collection</param>
        /// <param name="sampleIdx">Current sample index</param>
        /// <param name="endIdx">End end index</param>
        /// <param name="previousContiguousLineSegmentEndPoint">Initially set to null, but I will update I won't give a broken line if this is null</param>
        /// <param name="previousContiguousLineSegmentEndPointWithinClipBounds">Where the previous end segment was within the clip bounds</param>
        /// <param name="broken">place to put broken segment</param>
        /// <param name="continuous">place to put contiguous segment</param>
        /// <returns>
        ///   <c>true</c> if line segments are extracted, <c>false</c> if reached end.
        /// </returns>
        public static bool ExtractNextContinuousLineSegment<TSample, TSampleProvider, XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation, XYTransformation>(TSampleProvider sampleProvider, XYTransformation transformation, IReadOnlyList<TSample> samples, ref int sampleIdx, int endIdx, ref ScreenPoint? previousContiguousLineSegmentEndPoint, ref bool previousContiguousLineSegmentEndPointWithinClipBounds, List<ScreenPoint> broken, List<ScreenPoint> continuous)
            where TSampleProvider : IXYSampleProvider<TSample, XData, YData>
            where XDataProvider : IDataProvider<XData>
            where YDataProvider : IDataProvider<YData>
            where XAxisTransformation : IAxisScreenTransformation<XData, XDataProvider>
            where YAxisTransformation : IAxisScreenTransformation<YData, YDataProvider>
            where XYTransformation : IXYAxisTransformation<XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation>
        {
            // Need to:
            //  - reject invalid points, forming broken line segments as necessary
            //  - reject points outside the axis bounds

            TSample currentSample = default(TSample);
            DataSample<XData, YData> currentXYSample = default(DataSample<XData, YData>);

            // Skip all undefined points
            while (sampleIdx <= endIdx && !sampleProvider.TrySample(currentSample = samples[sampleIdx], out currentXYSample))
            {
                sampleIdx++;
            }

            if (sampleIdx >= endIdx)
            {
                // ran out of samples
                return false;
            }

            var currentSampleWithinClipBounds = transformation.WithinClipBounds(currentXYSample);
            var currentPoint = transformation.Transform(currentXYSample);

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
            while (sampleIdx <= endIdx)
            {
                var currentSampleIsValid = sampleProvider.TrySample(currentSample = samples[sampleIdx], out currentXYSample);

                if (!currentSampleIsValid)
                {
                    // we are invalid: skip current and break
                    sampleIdx++;
                    break;
                }

                currentSampleWithinClipBounds = transformation.WithinClipBounds(currentXYSample);
                currentPoint = transformation.Transform(currentXYSample);

                if (haveLast)
                {
                    if (transformation.XTransformation.IsDiscontinuous(lastXYSample.X, currentXYSample.X))
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
        /// Transforms a list of <see cref="DataSample{XData, YData}"/> along two axes.
        /// </summary>
        /// <typeparam name="XData"></typeparam>
        /// <typeparam name="YData"></typeparam>
        /// <typeparam name="XDataProvider"></typeparam>
        /// <typeparam name="YDataProvider"></typeparam>
        /// <typeparam name="XAxisTransformation"></typeparam>
        /// <typeparam name="YAxisTransformation"></typeparam>
        /// <typeparam name="XYTransformation"></typeparam>
        /// <param name="transformation"></param>
        /// <param name="dataSamples"></param>
        /// <param name="screenPoints">The output buffer. Invalid values indiciate discontenuities in the axis or data spaces.</param>
        public static void TransformSamples<XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation, XYTransformation>(XYTransformation transformation, IReadOnlyList<DataSample<XData, YData>> dataSamples, IList<ScreenPoint> screenPoints)
            where XDataProvider : IDataProvider<XData>
            where YDataProvider : IDataProvider<YData>
            where XAxisTransformation : IAxisScreenTransformation<XData, XDataProvider>
            where YAxisTransformation : IAxisScreenTransformation<YData, YDataProvider>
            where XYTransformation : IXYAxisTransformation<XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation>
        {
            if (dataSamples.Count < 1)
                return;

            foreach (var s in dataSamples)
                screenPoints.Add(transformation.Transform(s));
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
        /// <typeparam name="XYTransformation"></typeparam>
        /// <param name="transformation"></param>
        /// <param name="dataSamples"></param>
        /// <param name="minSegmentLength"></param>
        /// <param name="screenPoints">The output buffer. Invalid values indiciate discontenuities in the axis or data spaces.</param>
        public static void InterpolateLines<XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation, XYTransformation>(XYTransformation transformation, IReadOnlyList<DataSample<XData, YData>> dataSamples, double minSegmentLength, IList<ScreenPoint> screenPoints)
            where XDataProvider : IDataProvider<XData>
            where YDataProvider : IDataProvider<YData>
            where XAxisTransformation : IAxisScreenTransformation<XData, XDataProvider>
            where YAxisTransformation : IAxisScreenTransformation<YData, YDataProvider>
            where XYTransformation : IXYAxisTransformation<XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation>
        {
            if (dataSamples.Count < 2)
                return;

            if (minSegmentLength <= 0)
                throw new ArgumentOutOfRangeException(nameof(minSegmentLength), "Should be positive");

            if (transformation.XTransformation.Provider.IsDiscrete || transformation.YTransformation.Provider.IsDiscrete)
                throw new ArgumentException("Both transformations must have a non-discrete data space.");

            var bothLinear = transformation.XTransformation.IsNonDiscontinuous && transformation.YTransformation.IsLinear;
            var bothContinuous = transformation.XTransformation.IsNonDiscontinuous && transformation.YTransformation.IsNonDiscontinuous;

            // fast paths
            if (bothLinear && bothContinuous)
            {
                foreach (var s in dataSamples)
                    screenPoints.Add(transformation.Transform(s));
            }
            else
            {
                // all-powerful path

                var s1 = dataSamples[0];
                var p1 = transformation.Transform(s1);
                int si = 0;

                bool omitFirst = false;

                while (si < dataSamples.Count)
                {
                    // advance
                    var s0 = s1;
                    var p0 = p1;

                    si++;

                    s1 = dataSamples[si];
                    p1 = transformation.Transform(s1);

                    InterpolateLine<XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation, XYTransformation>(transformation, s0, s1, minSegmentLength, screenPoints, omitFirst, true);
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
        /// <typeparam name="XYTransformation"></typeparam>
        /// <param name="transformation"></param>
        /// <param name="s0"></param>
        /// <param name="s1"></param>
        /// <param name="minSegmentLength"></param>
        /// <param name="omitFirst">A value indiciating whether to not add the first screen point (corresponding to <paramref name="s0"/>) to the output buffer.</param>
        /// <param name="xprinciple">Whether to divide on the x axis by default.</param>
        /// <param name="screenPoints">The output buffer. Invalid values indiciate discontenuities in the axis or data spaces.</param>
        private static void InterpolateLine<XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation, XYTransformation>(XYTransformation transformation, DataSample<XData, YData> s0, DataSample<XData, YData> s1, double minSegmentLength, IList<ScreenPoint> screenPoints, bool omitFirst, bool xprinciple)
            where XDataProvider : IDataProvider<XData>
            where YDataProvider : IDataProvider<YData>
            where XAxisTransformation : IAxisScreenTransformation<XData, XDataProvider>
            where YAxisTransformation : IAxisScreenTransformation<YData, YDataProvider>
            where XYTransformation : IXYAxisTransformation<XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation>
        {
            // NOTE: this is really inefficient at the moment, but that's fine
            // TODO: make non-recursive, make it not re-compute interpolations, make it not transform everything twice

            // assumptions that should be enforced by the caller
            System.Diagnostics.Debug.Assert(!transformation.XTransformation.Provider.IsDiscrete);
            System.Diagnostics.Debug.Assert(!transformation.YTransformation.Provider.IsDiscrete);
            System.Diagnostics.Debug.Assert(!transformation.XTransformation.IsDiscontinuous(s0.X, s1.X));
            System.Diagnostics.Debug.Assert(!transformation.YTransformation.IsDiscontinuous(s0.Y, s1.Y));

            var minLengthSquared = minSegmentLength * minSegmentLength;

            var p0 = transformation.Transform(s0);
            var p1 = transformation.Transform(s1);

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
                    var m = transformation.XTransformation.InverseTransform(new ScreenReal((p0.X + p1.X) / 2));
                    c = transformation.XTransformation.Provider.Deinterpolate(s0.X, s1.X, m);
                }
                else
                {
                    var m = transformation.YTransformation.InverseTransform(new ScreenReal((p0.Y + p1.Y) / 2));
                    c = transformation.YTransformation.Provider.Deinterpolate(s0.Y, s1.Y, m);
                }

                // NOTE: not a real sample
                var sm = new DataSample<XData, YData>(transformation.XTransformation.Provider.Interpolate(s0.X, s1.X, c), transformation.YTransformation.Provider.Interpolate(s0.Y, s1.Y, c));
                var pm = transformation.Transform(sm);

                if (p0.DistanceToSquared(pm) < minLengthSquared)
                {
                    screenPoints.Add(pm);
                }
                else
                {
                    InterpolateLine<XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation, XYTransformation>(transformation, s0, sm, minSegmentLength, screenPoints, true, !xprinciple);
                }

                if (pm.DistanceToSquared(p1) < minLengthSquared)
                {
                    screenPoints.Add(p1);
                }
                else
                {
                    InterpolateLine<XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation, XYTransformation>(transformation, sm, s1, minSegmentLength, screenPoints, true, !xprinciple);
                }
            }
        }
    }
}
