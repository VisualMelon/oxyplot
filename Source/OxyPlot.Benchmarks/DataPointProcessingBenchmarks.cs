using BenchmarkDotNet.Attributes;
using OxyPlot.Axes.ComposableAxis;
using System;
using System.Collections.Generic;
using System.Text;

namespace OxyPlot.Benchmarks
{
    [MemoryDiagnoser]
    public class DataPointProcessingBenchmarks
    {
        [Params(1000, 1000000)]
        public int PointCount;

        private List<DataPoint> Points { get; set; }
        private List<ScreenPoint> Broken { get; set; }
        private List<ScreenPoint> Continuous { get; set; }

        [GlobalSetup]
        public void Initialize()
        {
            Points = new List<DataPoint>(PointCount);
            Broken = new List<ScreenPoint>(PointCount);
            Continuous = new List<ScreenPoint>(PointCount);

            var t = 0.0;
            for (int i = 0; i < PointCount; i++)
            {
                Points.Add(new DataPoint(t, Math.Sin(t)));
                t += 0.01;
            }
        }

        private static AxisScreenTransformation<double, DoubleProvider, Linear> PrepareUnitTransform()
        {
            return new AxisScreenTransformation<double, DoubleProvider, Linear>(default, new ViewInfo(new ScreenReal(0), 1), double.MinValue, double.MaxValue);
        }

        private static IXYRenderHelper<double, double> PrepareUnitTransformHelper()
        {
            var xtransformation = PrepareUnitTransform();
            var ytransformation = PrepareUnitTransform();
            var xytransformation = new HorizontalVertialXYTransformation<double, double, DoubleProvider, DoubleProvider, AxisScreenTransformation<double, DoubleProvider, Linear>, AxisScreenTransformation<double, DoubleProvider, Linear>>(xtransformation, ytransformation);

            var xyRenderHelper = new XYRenderHelper<double, double, DoubleProvider, DoubleProvider, AxisScreenTransformation<double, DoubleProvider, Linear>, AxisScreenTransformation<double, DoubleProvider, Linear>, HorizontalVertialXYTransformation<double, double, DoubleProvider, DoubleProvider, AxisScreenTransformation<double, DoubleProvider, Linear>, AxisScreenTransformation<double, DoubleProvider, Linear>>>(xytransformation);
            return xyRenderHelper;
        }

        [Benchmark]
        public void XYRenderHelpers_TypicalLinearDoubleConfiguration_NoBreaks()
        {
            // v-call here is realitic
            var xyRenderHelper = PrepareUnitTransformHelper();

            int startIndex = 0;
            int endIndex = Points.Count - 1;
            ScreenPoint? psp = null;
            bool pb = false;

            Broken.Clear();
            Continuous.Clear();
            xyRenderHelper.ExtractNextContinuousLineSegment<DataPoint, DataPointXYSampleProvider>(default, Points.AsReadOnlyList(), ref startIndex, endIndex, ref psp, ref pb, Broken, Continuous);

            if (Continuous.Count != Points.Count)
                throw new Exception("xyRenderHelper.ExtractNextContinuousLineSegment doesn't work.");
        }

        [Benchmark]
        public void XYRenderHelpers_MonotonicityCheck_MontoneX()
        {
            // v-call here is realitic
            var xyRenderHelper = PrepareUnitTransformHelper();

            var worked = xyRenderHelper.FindMinMax<DataPoint, DataPointXYSampleProvider>(default, Points.AsReadOnlyList(), out var minX, out var minY, out var maxX, out var maxY, out var mX, out var mY);

            if (!worked || !mX.IsMonotone)
                throw new Exception("xyRenderHelper.FindMinMax doesn't work.");
        }
    }
}
