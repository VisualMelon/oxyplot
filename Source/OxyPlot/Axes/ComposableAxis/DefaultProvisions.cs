using System;
using System.Collections.Generic;
using System.Text;

namespace OxyPlot.Axes.ComposableAxis
{
    /// <summary>
    /// Providers method to interact with <see cref="System.Double"/>.
    /// </summary>
    public struct DoubleProvider : IDataProvider<double>
    {
        /// <inheritdoc/>
        public bool IsDiscrete => false;

        /// <inheritdoc/>
        public int Compare(double x, double y)
        {
            return x.CompareTo(y);
        }

        /// <inheritdoc/>
        public double Deinterpolate(double v0, double v1, double v)
        {
            return (v - v0) / (v1 - v0);
        }

        /// <inheritdoc/>
        public bool Equals(double x, double y)
        {
            return x.Equals(y);
        }

        /// <inheritdoc/>
        public int GetHashCode(double obj)
        {
            return obj.GetHashCode();
        }

        /// <inheritdoc/>
        public bool Includes(double min, double max, double v)
        {
            return min <= v && v <= max;
        }

        /// <inheritdoc/>
        public double Interpolate(double v0, double v1, double c)
        {
            return v0 * (1 - c) + v1 * c;
        }
    }

    /// <summary>
    /// A linear data transformation over <see cref="System.Double"/>.
    /// </summary>
    public struct Linear : IDataTransformation<double, DoubleProvider>
    {
        /// <inheritdoc/>
        public bool IsNonDiscontinuous => true;

        /// <inheritdoc/>
        public bool IsLinear => true;

        /// <inheritdoc/>
        public bool IsDiscrete => false;

        /// <inheritdoc/>
        public DoubleProvider Provider => default;

        /// <inheritdoc/>
        public double InverseTransform(InteractionReal x)
        {
            return x.Value;
        }

        /// <inheritdoc/>
        public InteractionReal Transform(double data)
        {
            return new InteractionReal(data);
        }

        /// <inheritdoc/>
        public bool IsDiscontinuous(double a, double b)
        {
            return false;
        }
    }

    /// <summary>
    /// A logarithmic data projection over <see cref="System.Double"/>.
    /// </summary>
    public struct Logarithmic : IDataTransformation<double, DoubleProvider>
    {
        /// <inheritdoc/>
        public bool IsNonDiscontinuous => false;

        /// <inheritdoc/>
        public bool IsLinear => false;

        /// <inheritdoc/>
        public bool IsDiscrete => false;

        /// <inheritdoc/>
        public bool AreEqual(double l, double r)
        {
            return l == r;
        }

        /// <inheritdoc/>
        public DoubleProvider Provider => default;

        /// <inheritdoc/>
        public double InverseTransform(InteractionReal x)
        {
            return Math.Exp(x.Value);
        }

        /// <inheritdoc/>
        public InteractionReal Transform(double data)
        {
            return new InteractionReal(Math.Log(data));
        }

        /// <inheritdoc/>
        public bool IsDiscontinuous(double a, double b)
        {
            // not sure it makes sense to not throw in this case
            return a <= 0 || b <= 0;
        }
    }

    /// <summary>
    /// This needs a better name
    /// </summary>
    public struct ViewInfo
    {
        /// <summary>
        /// Initialises a <see cref="ViewInfo"/>.
        /// </summary>
        /// <param name="screenOffset"></param>
        /// <param name="screenScale"></param>
        public ViewInfo(ScreenReal screenOffset, double screenScale)
        {
            ScreenOffset = screenOffset;
            ScreenScale = screenScale;
        }

        /// <summary>
        /// Gets the Screen space offset, that is, the Screen space value to which the Interaction space zero maps.
        /// </summary>
        public ScreenReal ScreenOffset { get; }

        /// <summary>
        /// Gets the Screen space offset, that is, the scaling between Screen space and Interaction space.
        /// </summary>
        public double ScreenScale { get; }

        /// <summary>
        /// Transforms a value in Interaction space to Screen space.
        /// </summary>
        /// <param name="i">A value in Interaction space.</param>
        /// <returns>The resulting value in Screen space.</returns>
        public ScreenReal Transform(InteractionReal i)
        {
            return ScreenOffset + new ScreenReal(i.Value * ScreenScale);
        }

        /// <summary>
        /// Scales the given distance in Interaction space to Screen space.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public ScreenReal Scale(InteractionReal i)
        {
            return new ScreenReal(i.Value * ScreenScale);
        }

        /// <summary>
        /// Transforms a value in Screen space to Interaction space.
        /// </summary>
        /// <param name="s">A value in Screen space.</param>
        /// <returns>The resulting value in Interaction space.</returns>
        public InteractionReal InverseTransform(ScreenReal s)
        {
            return new InteractionReal((s.Value - ScreenOffset.Value) / ScreenScale);
        }

        /// <summary>
        /// Scales the given distance in Screen space to Interaction space.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public InteractionReal InverseScale(ScreenReal s)
        {
            return new InteractionReal(s.Value * ScreenScale);
        }
    }

    /// <summary>
    /// Wraps a <typeparamref name="TDataTransformation"/> and <see cref="ViewInfo"/> to provide an <see cref="IAxisScreenTransformation{TData, TDataProvider}"/>.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TDataProvider"></typeparam>
    /// <typeparam name="TDataTransformation"></typeparam>
    public struct AxisScreenTransformation<TData, TDataProvider, TDataTransformation> : IAxisScreenTransformation<TData, TDataProvider>
        where TDataProvider : IDataProvider<TData>
        where TDataTransformation : IDataTransformation<TData, TDataProvider>
    {
        /// <summary>
        /// Initialises the <see cref="AxisScreenTransformation{TData, TDataProvider, TDataTransformation}"/>.
        /// </summary>
        /// <param name="dataTransformation"></param>
        /// <param name="viewInfo"></param>
        /// <param name="clipMinimum">The minimum bound of the clipping region.</param>
        /// <param name="clipMaximum">The maximum bound of the clipping region.</param>
        public AxisScreenTransformation(TDataTransformation dataTransformation, ViewInfo viewInfo, TData clipMinimum, TData clipMaximum)
        {
            DataTransformation = dataTransformation;
            ViewInfo = viewInfo;
            ClipMinimum = clipMinimum;
            ClipMaximum = clipMaximum;
        }

        /// <summary>
        /// Gets the <typeparamref name="TDataProvider"/>.
        /// </summary>
        private TDataTransformation DataTransformation { get; }

        /// <summary>
        /// Gets the <see cref="ViewInfo"/>.
        /// </summary>
        private ViewInfo ViewInfo { get; }

        /// <inheritdoc/>
        public TData ClipMinimum { get; }

        /// <inheritdoc/>
        public TData ClipMaximum { get; }

        /// <inheritdoc/>
        public bool WithinClipBounds(TData v)
        {
            return Provider.Includes(ClipMinimum, ClipMaximum, v);
        }

        /// <inheritdoc/>
        public TDataProvider Provider => DataTransformation.Provider;

        /// <inheritdoc/>
        public bool IsNonDiscontinuous => DataTransformation.IsNonDiscontinuous;

        /// <inheritdoc/>
        public bool IsLinear => DataTransformation.IsLinear;

        /// <inheritdoc/>
        public TData InverseTransform(ScreenReal s)
        {
            return DataTransformation.InverseTransform(ViewInfo.InverseTransform(s));
        }

        /// <inheritdoc/>
        public bool IsDiscontinuous(TData a, TData b)
        {
            return DataTransformation.IsDiscontinuous(a, b);
        }

        /// <inheritdoc/>
        public ScreenReal Transform(TData data)
        {
            return ViewInfo.Transform(DataTransformation.Transform(data));
        }
    }

    /// <summary>
    /// Provides <see cref="System.Double"/> as option when <see cref="double.NaN"/>.
    /// </summary>
    public struct DoubleAsNaNOptional : IOptionalProvider<double, double>
    {
        /// <inheritdoc/>
        public bool HasValue(double optional)
        {
            return !double.IsNaN(optional);
        }

        /// <inheritdoc/>
        public bool TryGetValue(double optional, out double value)
        {
            value = optional;
            return !double.IsNaN(optional);
        }

        /// <inheritdoc/>
        public double None => double.NaN;

        /// <inheritdoc/>
        public double Some(double value)
        {
            if (double.IsNaN(value))
                throw new ArgumentException("Cannot represent NaN as a non-none value.");

            return value;
        }
    }

    /// <summary>
    /// Provides basic methods to help in X/Y space
    /// </summary>
    public interface IXYHelper<XData, YData>
    {
        /// <summary>
        /// Finds the minimum and maximum X and Y values in the samples.
        /// </summary>
        /// <typeparam name="TSample"></typeparam>
        /// <typeparam name="TSampleProvider"></typeparam>
        /// <param name="sampleProvider"></param>
        /// <param name="samples"></param>
        /// <param name="minX"></param>
        /// <param name="minY"></param>
        /// <param name="maxX"></param>
        /// <param name="maxY"></param>
        public bool FindMinMax<TSample, TSampleProvider>(TSampleProvider sampleProvider, IReadOnlyList<TSample> samples, out XData minX, out YData minY, out XData maxX, out YData maxY)
            where TSampleProvider : IXYSampleProvider<TSample, XData, YData>;

        /// <summary>
        /// Finds the minimum and maximum X and Y values in the samples.
        /// </summary>
        /// <typeparam name="TSample"></typeparam>
        /// <typeparam name="TSampleProvider"></typeparam>
        /// <param name="sampleProvider"></param>
        /// <param name="samples"></param>
        /// <param name="minX"></param>
        /// <param name="minY"></param>
        /// <param name="maxX"></param>
        /// <param name="maxY"></param>
        /// <param name="xMonotonicity"></param>
        /// <param name="yMonotonicity"></param>
        public bool FindMinMax<TSample, TSampleProvider>(TSampleProvider sampleProvider, IReadOnlyList<TSample> samples, out XData minX, out YData minY, out XData maxX, out YData maxY, out Monotonicity xMonotonicity, out Monotonicity yMonotonicity)
            where TSampleProvider : IXYSampleProvider<TSample, XData, YData>;

        /// <summary>
        /// Finds the start and end indexs of a window in some data.
        /// Throws if the data is not monotonic.
        /// </summary>
        /// <typeparam name="TSample"></typeparam>
        /// <typeparam name="TSampleProvider"></typeparam>
        /// <param name="sampleProvider"></param>
        /// <param name="samples"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="xMonotonicity"></param>
        /// <param name="yMonotonicity"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public bool FindWindow<TSample, TSampleProvider>(TSampleProvider sampleProvider, IReadOnlyList<TSample> samples, DataSample<XData, YData> start, DataSample<XData, YData> end, Monotonicity xMonotonicity, Monotonicity yMonotonicity, out int startIndex, out int endIndex)
            where TSampleProvider : IXYSampleProvider<TSample, XData, YData>;
    }

    /// <summary>
    /// Provides basic methods to render in X/Y space
    /// </summary>
    public interface IXYRenderHelper<XData, YData> : IXYHelper<XData, YData>
    {
        /// <summary>
        /// Interpolates lines
        /// </summary>
        /// <param name="dataSamples"></param>
        /// <param name="minSegmentLength"></param>
        /// <param name="screenPoints"></param>
        void InterpolateLines(IReadOnlyList<DataSample<XData, YData>> dataSamples, double minSegmentLength, IList<ScreenPoint> screenPoints);

        /// <summary>
        /// Extracts a single contiguous line segment beginning with the element at the position of the enumerator when the method
        /// is called. Invalid samples are ignored.
        /// </summary>
        /// <typeparam name="TSample"></typeparam>
        /// <typeparam name="TSampleProvider"></typeparam>
        /// <param name="sampleProvider"></param>
        /// <param name="samples">Points collection</param>
        /// <param name="sampleIdx">Current sample index</param>
        /// <param name="endIdx">End index</param>
        /// <param name="previousContiguousLineSegmentEndPoint">Initially set to null, but I will update I won't give a broken line if this is null</param>
        /// <param name="previousContiguousLineSegmentEndPointWithinClipBounds">Where the previous end segment was within the clip bounds</param>
        /// <param name="broken">Buffer for the broken segment</param>
        /// <param name="continuous">Buffer for the continuous segment</param>
        /// <returns>
        ///   <c>true</c> if line segments are extracted, <c>false</c> if reached end.
        /// </returns>
        bool ExtractNextContinuousLineSegment<TSample, TSampleProvider>(TSampleProvider sampleProvider, IReadOnlyList<TSample> samples, ref int sampleIdx, int endIdx, ref ScreenPoint? previousContiguousLineSegmentEndPoint, ref bool previousContiguousLineSegmentEndPointWithinClipBounds, List<ScreenPoint> broken, List<ScreenPoint> continuous)
            where TSampleProvider : IXYSampleProvider<TSample, XData, YData>;

        /// <summary>
        /// Transforms many <see cref="DataSample{XData, YData}"/>.
        /// </summary>
        /// <param name="xySamples"></param>
        /// <param name="screenPoints"></param>
        void TransformSamples(IReadOnlyList<DataSample<XData, YData>> xySamples, IList<ScreenPoint> screenPoints);

        /// <summary>
        /// Transforms a single <see cref="DataSample{XData, YData}"/>
        /// </summary>
        /// <param name="xySample"></param>
        /// <returns></returns>
        public ScreenPoint TransformSample(DataSample<XData, YData> xySample);
    }

    /// <summary>
    /// Prepares instances of <see cref="IXYHelper{XData, YData}"/>.
    /// </summary>
    /// <typeparam name="XData"></typeparam>
    /// <typeparam name="YData"></typeparam>
    public class XYHelperPreparer<XData, YData>
    {
        private class Generator : IXYAxisScreenTransformationConsumer<XData, YData>
        {
            public void Consume<XDataProvider, YDataProvider, XAxisScreenTransformation, YAxisScreenTransformation>(XAxisScreenTransformation x, YAxisScreenTransformation y)
                where XDataProvider : IDataProvider<XData>
                where YDataProvider : IDataProvider<YData>
                where XAxisScreenTransformation : IAxisScreenTransformation<XData, XDataProvider>
                where YAxisScreenTransformation : IAxisScreenTransformation<YData, YDataProvider>
            {
                Result = new XYRenderHelper<XData, YData, XDataProvider, YDataProvider, XAxisScreenTransformation, YAxisScreenTransformation>(x, y);
            }

            public IXYHelper<XData, YData> Result { get; private set; }
        }

        /// <summary>
        /// Prepares an <see cref="IXYHelper{XData, YData}"/> from the given collator.
        /// </summary>
        /// <param name="collator"></param>
        /// <returns></returns>
        public static IXYHelper<XData, YData> Prepare(XYCollator<XData, YData> collator)
        {
            var generator = new Generator();
            collator.Consume(generator);
            return generator.Result;
        }
    }

    /// <summary>
    /// Prepares instances of <see cref="IXYRenderHelper{XData, YData}"/>.
    /// </summary>
    /// <typeparam name="XData"></typeparam>
    /// <typeparam name="YData"></typeparam>
    public class XYRenderHelperPreparer<XData, YData>
    {
        private class Generator : IXYAxisScreenTransformationConsumer<XData, YData>
        {
            public void Consume<XDataProvider, YDataProvider, XAxisScreenTransformation, YAxisScreenTransformation>(XAxisScreenTransformation x, YAxisScreenTransformation y)
                where XDataProvider : IDataProvider<XData>
                where YDataProvider : IDataProvider<YData>
                where XAxisScreenTransformation : IAxisScreenTransformation<XData, XDataProvider>
                where YAxisScreenTransformation : IAxisScreenTransformation<YData, YDataProvider>
            {
                Result = new XYRenderHelper<XData, YData, XDataProvider, YDataProvider, XAxisScreenTransformation, YAxisScreenTransformation>(x, y);
            }

            public IXYRenderHelper<XData, YData> Result { get; private set; }
        }

        /// <summary>
        /// Prepares an <see cref="IXYHelper{XData, YData}"/> from the given collator.
        /// </summary>
        /// <param name="collator"></param>
        /// <returns></returns>
        public static IXYRenderHelper<XData, YData> Prepare(XYCollator<XData, YData> collator)
        {
            var generator = new Generator();
            collator.Consume(generator);
            return generator.Result;
        }
    }

    /// <summary>
    /// Provides basic methods to help in X/Y space
    /// </summary>
    /// <typeparam name="XData"></typeparam>
    /// <typeparam name="YData"></typeparam>
    /// <typeparam name="XDataProvider"></typeparam>
    /// <typeparam name="YDataProvider"></typeparam>
    public class XYHelper<XData, YData, XDataProvider, YDataProvider> : IXYHelper<XData, YData>
        where XDataProvider : IDataProvider<XData>
        where YDataProvider : IDataProvider<YData>
    {
        /// <summary>
        /// Initialises an XYHelper.
        /// </summary>
        /// <param name="xProvider"></param>
        /// <param name="yProvider"></param>
        public XYHelper(XDataProvider xProvider, YDataProvider yProvider)
        {
            XProvider = xProvider;
            YProvider = yProvider;
        }

        private XDataProvider XProvider { get; }
        private YDataProvider YProvider { get; }

        /// <inheritdoc/>
        public bool FindMinMax<TSample, TSampleProvider>(TSampleProvider sampleProvider, IReadOnlyList<TSample> samples, out XData minX, out YData minY, out XData maxX, out YData maxY)
            where TSampleProvider : IXYSampleProvider<TSample, XData, YData>
        {
            return XYHelpers.TryFindMinMax(sampleProvider, XProvider, YProvider, samples, out minX, out minY, out maxX, out maxY);
        }

        /// <inheritdoc/>
        public bool FindMinMax<TSample, TSampleProvider>(TSampleProvider sampleProvider, IReadOnlyList<TSample> samples, out XData minX, out YData minY, out XData maxX, out YData maxY, out Monotonicity xMonotonicity, out Monotonicity yMonotonicity)
            where TSampleProvider : IXYSampleProvider<TSample, XData, YData>
        {
            return XYHelpers.TryFindMinMax(sampleProvider, XProvider, YProvider, samples, out minX, out minY, out maxX, out maxY, out xMonotonicity, out yMonotonicity);
        }

        /// <inheritdoc/>
        public bool FindWindow<TSample, TSampleProvider>(TSampleProvider sampleProvider, IReadOnlyList<TSample> samples, DataSample<XData, YData> start, DataSample<XData, YData> end, Monotonicity xMonotonicity, Monotonicity yMonotonicity, out int startIndex, out int endIndex)
            where TSampleProvider : IXYSampleProvider<TSample, XData, YData>
        {
            return XYHelpers.FindWindow(sampleProvider, XProvider, YProvider, samples, start, end, xMonotonicity, yMonotonicity, out startIndex, out endIndex);
        }
    }

    /// <summary>
    /// Provides basic methods to render in X/Y space
    /// </summary>
    /// <typeparam name="XData"></typeparam>
    /// <typeparam name="YData"></typeparam>
    /// <typeparam name="XDataProvider"></typeparam>
    /// <typeparam name="YDataProvider"></typeparam>
    /// <typeparam name="XAxisTransformation"></typeparam>
    /// <typeparam name="YAxisTransformation"></typeparam>
    public class XYRenderHelper<XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation> : XYHelper<XData, YData, XDataProvider, YDataProvider>, IXYRenderHelper<XData, YData>
        where XDataProvider : IDataProvider<XData>
        where YDataProvider : IDataProvider<YData>
        where XAxisTransformation : IAxisScreenTransformation<XData, XDataProvider>
        where YAxisTransformation : IAxisScreenTransformation<YData, YDataProvider>
    {
        /// <summary>
        /// Initialises an XYRenderHelper.
        /// </summary>
        /// <param name="xTransformation"></param>
        /// <param name="yTransformation"></param>
        public XYRenderHelper(XAxisTransformation xTransformation, YAxisTransformation yTransformation)
            : base(xTransformation.Provider, yTransformation.Provider)
        {
            XTransformation = xTransformation;
            YTransformation = yTransformation;
        }

        private XAxisTransformation XTransformation { get; }
        private YAxisTransformation YTransformation { get; }

        /// <inheritdoc/>
        public bool ExtractNextContinuousLineSegment<TSample, TSampleProvider>(TSampleProvider sampleProvider, IReadOnlyList<TSample> samples, ref int sampleIdx, int endIdx, ref ScreenPoint? previousContiguousLineSegmentEndPoint, ref bool previousContiguousLineSegmentEndPointWithinClipBounds, List<ScreenPoint> broken, List<ScreenPoint> continuous) where TSampleProvider : IXYSampleProvider<TSample, XData, YData>
        {
            return RenderHelpers.ExtractNextContinuousLineSegment<TSample, TSampleProvider, XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation>(sampleProvider, XTransformation, YTransformation, samples, ref sampleIdx, endIdx, ref previousContiguousLineSegmentEndPoint, ref previousContiguousLineSegmentEndPointWithinClipBounds, broken, continuous);
        }

        /// <inheritdoc/>
        public void InterpolateLines(IReadOnlyList<DataSample<XData, YData>> dataSamples, double minSegmentLength, IList<ScreenPoint> screenPoints)
        {
            RenderHelpers.InterpolateLines<XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation>(XTransformation, YTransformation, dataSamples, minSegmentLength, screenPoints);
        }

        /// <inheritdoc/>
        public void TransformSamples(IReadOnlyList<DataSample<XData, YData>> dataSamples, IList<ScreenPoint> screenPoints)
        {
            RenderHelpers.TransformSamples<XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation>(XTransformation, YTransformation, dataSamples, screenPoints);
        }

        /// <inheritdoc/>
        public ScreenPoint TransformSample(DataSample<XData, YData> sample)
        {
            return sample.Transform<XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation>(XTransformation, YTransformation);
        }
    }

    /// <summary>
    /// Provides methods to help render XY values.
    /// </summary>
    /// <typeparam name="XData"></typeparam>
    /// <typeparam name="YData"></typeparam>
    public class XYCollator<XData, YData>
    {
        private XYCollator(ITypedCollator typed)
        {
            Typed = typed ?? throw new ArgumentNullException(nameof(typed));
        }

        private ITypedCollator Typed { get; }

        /// <summary>
        /// Attempts to prepare an <see cref="XYCollator{XData, YData}"/> for two axis. Throws if either axis is of the wrong type.
        /// This method should probably not be here.
        /// </summary>
        /// <param name="xaxis"></param>
        /// <param name="yaxis"></param>
        /// <returns></returns>
        public static XYCollator<XData, YData> TryPrepare(IAxis xaxis, IAxis yaxis)
        {
            var tx = xaxis as IAxis<XData>;
            var ty = yaxis as IAxis<YData>;

            if (tx == null)
                throw new InvalidOperationException($"XAxis {xaxis.Key} is not of the expected Data type.");
            if (ty == null)
                throw new InvalidOperationException($"YAxis {yaxis.Key} is not of the expected Data type.");

            return Prepare(tx, ty);
        }

        /// <summary>
        /// Prepares an <see cref="XYCollator{XData, YData}"/> for two axis.
        /// </summary>
        /// <param name="xaxis"></param>
        /// <param name="yaxis"></param>
        /// <returns></returns>
        public static XYCollator<XData, YData> Prepare(IAxis<XData> xaxis, IAxis<YData> yaxis)
        {
            var xconsumer = new XConsumer(xaxis, yaxis);
            return xconsumer.Result;
        }

        private class XConsumer : IAxisScreenTransformationConsumer<XData>
        {
            private IAxis<XData> XAxis;
            private IAxis<YData> YAxis;

            public XConsumer(IAxis<XData> xAxis, IAxis<YData> yAxis)
            {
                XAxis = xAxis ?? throw new ArgumentNullException(nameof(xAxis));
                YAxis = yAxis ?? throw new ArgumentNullException(nameof(yAxis));
            }

            private XYCollator<XData, YData> _result = null;
            public XYCollator<XData, YData> Result
            {
                get
                {
                    if (_result == null)
                        XAxis.Consume(this);
                    return _result;
                }
            }

            public void Consume<XDataProvider, XAxisScreenTransformation>(XAxisScreenTransformation transformation)
                where XDataProvider : IDataProvider<XData>
                where XAxisScreenTransformation : IAxisScreenTransformation<XData, XDataProvider>
            {
                var yconsumer = new YConsumer<XDataProvider, XAxisScreenTransformation>(transformation);
                YAxis.Consume(yconsumer);
                _result = yconsumer.Result;
            }
        }

        private class YConsumer<XDataProvider, XAxisTransformation> : IAxisScreenTransformationConsumer<YData>
            where XDataProvider : IDataProvider<XData>
            where XAxisTransformation : IAxisScreenTransformation<XData, XDataProvider>
        {
            public YConsumer(XAxisTransformation xTransformation)
            {
                XTransformation = xTransformation;
            }

            public XYCollator<XData, YData> Result { get; private set; }

            private XAxisTransformation XTransformation { get; }

            public void Consume<YDataProvider, YAxisScreenTransformation>(YAxisScreenTransformation transformation)
                where YDataProvider : IDataProvider<YData>
                where YAxisScreenTransformation : IAxisScreenTransformation<YData, YDataProvider>
            {
                var typed = new TypedCollator<XDataProvider, YDataProvider, XAxisTransformation, YAxisScreenTransformation>(XTransformation, transformation);
                Result = new XYCollator<XData, YData>(typed);
            }
        }

        private interface ITypedCollator
        {
            void Consume(IXYAxisScreenTransformationConsumer<XData, YData> consumer);
        }

        private class TypedCollator<XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation> : ITypedCollator
            where XDataProvider : IDataProvider<XData>
            where YDataProvider : IDataProvider<YData>
            where XAxisTransformation : IAxisScreenTransformation<XData, XDataProvider>
            where YAxisTransformation : IAxisScreenTransformation<YData, YDataProvider>
        {
            public TypedCollator(XAxisTransformation xTransformation, YAxisTransformation yTransformation)
            {
                XTransformation = xTransformation;
                YTransformation = yTransformation;
            }

            public XAxisTransformation XTransformation { get; }
            public YAxisTransformation YTransformation { get; }

            public void Consume(IXYAxisScreenTransformationConsumer<XData, YData> consumer)
            {
                consumer.Consume<XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation>(XTransformation, YTransformation);
            }
        }

        /// <summary>
        /// Passes onto the given consumer.
        /// </summary>
        /// <param name="consumer"></param>
        public void Consume(IXYAxisScreenTransformationConsumer<XData, YData> consumer)
        {
            Typed.Consume(consumer);
        }
    }

    /// <summary>
    /// Provides methods to help render XY values.
    /// </summary>
    /// <typeparam name="XData"></typeparam>
    /// <typeparam name="YData"></typeparam>
    /// <typeparam name="ZData"></typeparam>
    public class XYZCollator<XData, YData, ZData>
    {
        private XYZCollator(ITypedCollator typed)
        {
            Typed = typed ?? throw new ArgumentNullException(nameof(typed));
        }

        private ITypedCollator Typed { get; }

        /// <summary>
        /// Attempts to prepare an <see cref="XYCollator{XData, YData}"/> for three axis. Throws if either axis is of the wrong type.
        /// This method should probably not be here.
        /// </summary>
        /// <param name="xaxis"></param>
        /// <param name="yaxis"></param>
        /// <param name="zaxis"></param>
        /// <returns></returns>
        public XYZCollator<XData, YData, ZData> TryPrepare(IAxis xaxis, IAxis yaxis, IAxis zaxis)
        {
            var tx = xaxis as IAxis<XData>;
            var ty = yaxis as IAxis<YData>;
            var tz = zaxis as IAxis<ZData>;

            if (tx == null)
                throw new InvalidOperationException($"XAxis {xaxis.Key} is not of the expected Data type.");
            if (ty == null)
                throw new InvalidOperationException($"YAxis {yaxis.Key} is not of the expected Data type.");
            if (tz == null)
                throw new InvalidOperationException($"ZAxis {zaxis.Key} is not of the expected Data type.");

            return Prepare(tx, ty, tz);
        }

        /// <summary>
        /// Prepares an <see cref="XYZCollator{XData, YData, ZData}"/> for three axis.
        /// </summary>
        /// <param name="xaxis"></param>
        /// <param name="yaxis"></param>
        /// <param name="zaxis"></param>
        /// <returns></returns>
        public XYZCollator<XData, YData, ZData> Prepare(IAxis<XData> xaxis, IAxis<YData> yaxis, IAxis<ZData> zaxis)
        {
            var xconsumer = new XConsumer(xaxis, yaxis, zaxis);
            return xconsumer.Result;
        }

        private class XConsumer : IAxisScreenTransformationConsumer<XData>
        {
            private IAxis<XData> XAxis;
            private IAxis<YData> YAxis;
            private IAxis<ZData> ZAxis;

            public XConsumer(IAxis<XData> xAxis, IAxis<YData> yAxis, IAxis<ZData> zAxis)
            {
                XAxis = xAxis ?? throw new ArgumentNullException(nameof(xAxis));
                YAxis = yAxis ?? throw new ArgumentNullException(nameof(yAxis));
                ZAxis = zAxis ?? throw new ArgumentNullException(nameof(zAxis));
            }

            private XYZCollator<XData, YData, ZData> _result = null;
            public XYZCollator<XData, YData, ZData> Result
            {
                get
                {
                    if (_result == null)
                        XAxis.Consume(this);
                    return _result;
                }
            }

            public void Consume<XDataProvider, XAxisScreenTransformation>(XAxisScreenTransformation transformation)
                where XDataProvider : IDataProvider<XData>
                where XAxisScreenTransformation : IAxisScreenTransformation<XData, XDataProvider>
            {
                var yconsumer = new YConsumer<XDataProvider, XAxisScreenTransformation>(transformation, ZAxis);
                YAxis.Consume(yconsumer);
                _result = yconsumer.Result;
            }
        }

        private class YConsumer<XDataProvider, XAxisTransformation> : IAxisScreenTransformationConsumer<YData>
            where XDataProvider : IDataProvider<XData>
            where XAxisTransformation : IAxisScreenTransformation<XData, XDataProvider>
        {
            public YConsumer(XAxisTransformation xTransformation, IAxis<ZData> zAxis)
            {
                XTransformation = xTransformation;
                ZAxis = zAxis;
            }

            public XYZCollator<XData, YData, ZData> Result { get; private set; }

            private XAxisTransformation XTransformation { get; }
            public IAxis<ZData> ZAxis { get; }

            public void Consume<YDataProvider, YAxisScreenTransformation>(YAxisScreenTransformation transformation)
                where YDataProvider : IDataProvider<YData>
                where YAxisScreenTransformation : IAxisScreenTransformation<YData, YDataProvider>
            {
                var zconsumer = new ZConsumer<XDataProvider, YDataProvider, XAxisTransformation, YAxisScreenTransformation>(XTransformation, transformation);
                ZAxis.Consume(zconsumer);
                Result = zconsumer.Result;
            }
        }

        private class ZConsumer<XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation> : IAxisScreenTransformationConsumer<ZData>
            where XDataProvider : IDataProvider<XData>
            where YDataProvider : IDataProvider<YData>
            where XAxisTransformation : IAxisScreenTransformation<XData, XDataProvider>
            where YAxisTransformation : IAxisScreenTransformation<YData, YDataProvider>
        {
            public ZConsumer(XAxisTransformation xTransformation, YAxisTransformation yTransformation)
            {
                XTransformation = xTransformation;
                YTransformation = yTransformation;
            }

            public XYZCollator<XData, YData, ZData> Result { get; private set; }

            private XAxisTransformation XTransformation { get; }
            private YAxisTransformation YTransformation { get; }

            public void Consume<ZDataProvider, ZAxisScreenTransformation>(ZAxisScreenTransformation transformation)
                where ZDataProvider : IDataProvider<ZData>
                where ZAxisScreenTransformation : IAxisScreenTransformation<ZData, ZDataProvider>
            {
                var typed = new TypedCollator<XDataProvider, YDataProvider, ZDataProvider, XAxisTransformation, YAxisTransformation, ZAxisScreenTransformation>(XTransformation, YTransformation, transformation);
                Result = new XYZCollator<XData, YData, ZData>(typed);
            }
        }

        private interface ITypedCollator
        {
            void Consume(IXYZAxisScreenTransformationConsumer<XData, YData, ZData> consumer);
        }

        private class TypedCollator<XDataProvider, YDataProvider, ZDataProvider, XAxisTransformation, YAxisTransformation, ZAxisTransformation> : ITypedCollator
            where XDataProvider : IDataProvider<XData>
            where YDataProvider : IDataProvider<YData>
            where ZDataProvider : IDataProvider<ZData>
            where XAxisTransformation : IAxisScreenTransformation<XData, XDataProvider>
            where YAxisTransformation : IAxisScreenTransformation<YData, YDataProvider>
            where ZAxisTransformation : IAxisScreenTransformation<ZData, ZDataProvider>
        {
            public TypedCollator(XAxisTransformation xTransformation, YAxisTransformation yTransformation, ZAxisTransformation zTransformation)
            {
                XTransformation = xTransformation;
                YTransformation = yTransformation;
                ZTransformation = zTransformation;
            }

            public XAxisTransformation XTransformation { get; }
            public YAxisTransformation YTransformation { get; }
            public ZAxisTransformation ZTransformation { get; }

            public void Consume(IXYZAxisScreenTransformationConsumer<XData, YData, ZData> consumer)
            {
                consumer.Consume<XDataProvider, YDataProvider, ZDataProvider, XAxisTransformation, YAxisTransformation, ZAxisTransformation>(XTransformation, YTransformation, ZTransformation);
            }
        }

        /// <summary>
        /// Passes onto the given consumer.
        /// </summary>
        /// <param name="consumer"></param>
        public void Consume(IXYZAxisScreenTransformationConsumer<XData, YData, ZData> consumer)
        {
            Typed.Consume(consumer);
        }
    }

    /// <summary>
    /// Providers a mapping from <see cref="DataPoint"/> to a <see cref="DataSample{XData, YData}"/> of doubles.
    /// </summary>
    public struct DataPointXYSampleProvider : IXYSampleProvider<DataPoint, double, double>
    {
        /// <inheritdoc/>
        public bool IsInvalid(DataPoint sample)
        {
            return !sample.IsDefined();
        }

        /// <inheritdoc/>
        public DataSample<double, double> Sample(DataPoint sample)
        {
            return new DataSample<double, double>(sample.X, sample.Y);
        }

        /// <inheritdoc/>
        public bool TrySample(DataPoint sample, out DataSample<double, double> result)
        {
            if (sample.IsDefined())
            {
                result = new DataSample<double, double>(sample.X, sample.Y);
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }
    }
}
