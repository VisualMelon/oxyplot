using System;
using System.Collections.Generic;
using System.Text;

namespace OxyPlot.Axes.ComposableAxis
{
    /// <summary>
    /// Provides methods to help with rendering on axis.
    /// </summary>
    public static class RenderHelpers
    {
        /// <summary>
        /// Transforms a Data sample to screen space.
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
            return new ScreenPoint(x.Transform(sample.X), y.Transform(sample.Y));
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

        // NOTE: this can't work as planned: we MUST perform interpolations in the data-space (i.e. need to ask for lerp(x0,x1,c) and lerp(y0,y1,c) for common c to achieve anything
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
                    var m = x.InverseTransform((p0.X + p1.X) / 2);
                    c = x.Provider.Deinterpolate(s0.X, s1.X, m);
                }
                else
                {
                    var m = y.InverseTransform((p0.Y + p1.Y) / 2);
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
