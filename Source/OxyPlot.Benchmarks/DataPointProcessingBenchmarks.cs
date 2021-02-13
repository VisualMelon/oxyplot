using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using OxyPlot.Axes.ComposableAxis;
using System;
using System.Collections.Generic;
using System.Text;

namespace OxyPlot.Benchmarks
{
    [DisassemblyDiagnoser]
    [MemoryDiagnoser]
    public class DataPointProcessingBenchmarks
    {
        [Params(/*1000,*/ 1000000)]
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

        private static AxisScreenTransformation<double, DoubleProvider, Linear, MinMaxFilter<double, DoubleProvider>> PrepareUnitTransform()
        {
            return new AxisScreenTransformation<double, DoubleProvider, Linear, MinMaxFilter<double, DoubleProvider>>(default, new MinMaxFilter<double, DoubleProvider>(default, double.MinValue, double.MaxValue), new ViewInfo(new ScreenReal(0), 1), double.MinValue, double.MaxValue);
        }

        private static XYRenderHelper<double, double, DoubleProvider, DoubleProvider, AxisScreenTransformation<double, DoubleProvider, Linear, MinMaxFilter<double, DoubleProvider>>, AxisScreenTransformation<double, DoubleProvider, Linear, MinMaxFilter<double, DoubleProvider>>, HorizontalVertialXYTransformation<double, double, DoubleProvider, DoubleProvider, AxisScreenTransformation<double, DoubleProvider, Linear, MinMaxFilter<double, DoubleProvider>>, AxisScreenTransformation<double, DoubleProvider, Linear, MinMaxFilter<double, DoubleProvider>>>> PrepareUnitTransformHelper()
        {
            var xtransformation = PrepareUnitTransform();
            var ytransformation = PrepareUnitTransform();
            var xytransformation = new HorizontalVertialXYTransformation<double, double, DoubleProvider, DoubleProvider, AxisScreenTransformation<double, DoubleProvider, Linear, MinMaxFilter<double, DoubleProvider>>, AxisScreenTransformation<double, DoubleProvider, Linear, MinMaxFilter<double, DoubleProvider>>>(xtransformation, ytransformation);

            var xyRenderHelper = new XYRenderHelper<double, double, DoubleProvider, DoubleProvider, AxisScreenTransformation<double, DoubleProvider, Linear, MinMaxFilter<double, DoubleProvider>>, AxisScreenTransformation<double, DoubleProvider, Linear, MinMaxFilter<double, DoubleProvider>>, HorizontalVertialXYTransformation<double, double, DoubleProvider, DoubleProvider, AxisScreenTransformation<double, DoubleProvider, Linear, MinMaxFilter<double, DoubleProvider>>, AxisScreenTransformation<double, DoubleProvider, Linear, MinMaxFilter<double, DoubleProvider>>>>(xytransformation);
            return xyRenderHelper;
        }

        private static IXYRenderHelper<double, double> PrepareUnitTransformHelper_Interface()
        {
            return PrepareUnitTransformHelper();
        }

        [Benchmark]
        public void XYRenderHelpers_TypicalLinearDoubleConfiguration_NoBreaks()
        {
            // v-call would be realistic, but can make no real difference, and means we don't get dissassembly
            // NOTE: these don't do any clipping
            var xyRenderHelper = PrepareUnitTransformHelper(); // PrepareUnitTransformHelper_Interface();

            int startIndex = 0;
            int endIndex = Points.Count - 1;
            ScreenPoint? psp = null;
            XYClipInfo pci = default;

            Broken.Clear();
            Continuous.Clear();

            var clipRect = new OxyRect(-100, -100, PointCount * 2, 200);

            xyRenderHelper.ExtractNextContinuousLineSegment<DataPoint, DataPointXYSampleProvider, AcceptAllFilter<DataPoint>, RectangleClipFilter>(default, default, new RectangleClipFilter(clipRect), Points.AsReadOnlyList(), ref startIndex, endIndex, ref psp, ref pci, Broken, Continuous);

            if (Continuous.Count != Points.Count)
                throw new Exception("xyRenderHelper.ExtractNextContinuousLineSegment doesn't work.");
        }

        [Benchmark]
        public void XYRenderHelpers_TypicalLinearDoubleConfiguration_NoBreaks_NaiveInlined()
        {
            // atrificial version of XYRenderHelpers_TypicalLinearDoubleConfiguration_NoBreaks which reveals the overheads of IReadOnlyList and the other general chaos (nice for the disassembly of the expensive bits, also)
            var xyRenderHelper = PrepareUnitTransformHelper();
            var transformation = xyRenderHelper.XYTransformation;
            var sampleProvider = new DataPointXYSampleProvider();

            var x = transformation.XTransformation;
            var y = transformation.YTransformation;

            var samples = Points;
            int sampleIdx = 0;
            int endIdx = samples.Count - 1;

            Broken.Clear();
            Continuous.Clear();
            if (samples.Count > 0)
            {
                if (sampleIdx > endIdx)
                    return;

                while (sampleIdx <= endIdx)
                {
                    if (sampleProvider.TrySample(samples[sampleIdx++], out var valid))
                    {
                        if (x.Filter(valid.X)
                            && y.Filter(valid.Y))
                        {
                            Continuous.Add(transformation.Arrange(x.Transform(valid.X),
                                y.Transform(valid.Y)));
                        }
                    }
                }
            }
        }

        [Benchmark]
        public void XYRenderHelpers_TypicalLinearDoubleConfiguration_NoBreaks_Baseline()
        {
            // atrificial version of XYRenderHelpers_TypicalLinearDoubleConfiguration_NoBreaks which reveals the massive overheads of all the abstractions
            var xyRenderHelper = PrepareUnitTransformHelper();

            var xfiltermin = double.MinValue;
            var xfiltermax = double.MaxValue;
            var yfiltermin = double.MinValue;
            var yfiltermax = double.MaxValue;

            var xview = xyRenderHelper.XYTransformation.YTransformation.ViewInfo;
            var yview = xyRenderHelper.XYTransformation.YTransformation.ViewInfo;

            Broken.Clear();
            Continuous.Clear();
            for (int i = 0; i < Points.Count; i++)
            {
                var p = Points[i];
                if (!p.IsDefined())
                    return;

                if (p.X > xfiltermax || p.X < xfiltermin || p.Y > yfiltermax || p.Y < yfiltermin)
                    continue;

                Continuous.Add(new ScreenPoint(p.X * xview.ScreenScale + xview.ScreenOffset.Value, p.Y * yview.ScreenScale + yview.ScreenOffset.Value));
            }
        }

        [Benchmark]
        public void XYRenderHelpers_MonotonicityCheck_MontoneX()
        {
            // v-call here is realistic
            var xyRenderHelper = PrepareUnitTransformHelper();

            var worked = xyRenderHelper.FindMinMax<DataPoint, DataPointXYSampleProvider, AcceptAllFilter<DataPoint>>(default, default, Points.AsReadOnlyList(), out var minX, out var minY, out var maxX, out var maxY, out var mX, out var mY);

            if (!worked || !mX.IsMonotone)
                throw new Exception("xyRenderHelper.FindMinMax doesn't work.");
        }
    }
}
