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
        /// Transforms a value in Screen space to Interaction space.
        /// </summary>
        /// <param name="s">A value in Screen space.</param>
        /// <returns>The resulting value in Interaction space.</returns>
        public InteractionReal InverseTransform(ScreenReal s)
        {
            return new InteractionReal((s.Value - ScreenOffset.Value) / ScreenScale);
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
        public AxisScreenTransformation(TDataTransformation dataTransformation, ViewInfo viewInfo)
        {
            DataTransformation = dataTransformation;
            ViewInfo = viewInfo;
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
    /// Provides methods to help render XY values.
    /// </summary>
    /// <typeparam name="XData"></typeparam>
    /// <typeparam name="YData"></typeparam>
    public class XYRenderHelper<XData, YData>
    {
        // TODO: provide an XY consumer: this can just aggregate two, and pass them on (so that all the 'real' code doesn't get stuffed in here)

        private XYRenderHelper(ITypedXYRenderHelper typed)
        {
            Typed = typed ?? throw new ArgumentNullException(nameof(typed));
        }

        private ITypedXYRenderHelper Typed { get; }

        /// <summary>
        /// Interpolates lines.
        /// </summary>
        /// <param name="dataSamples"></param>
        /// <param name="minSegmentLength"></param>
        /// <param name="screenPoints">The output buffer.</param>
        public void InterpolateLines(IReadOnlyList<DataSample<XData, YData>> dataSamples, double minSegmentLength, IList<ScreenPoint> screenPoints)
        {
            Typed.InterpolateLines(dataSamples, minSegmentLength, screenPoints);
        }

        private interface ITypedXYRenderHelper
        {
            void InterpolateLines(IReadOnlyList<DataSample<XData, YData>> dataSamples, double minSegmentLength, IList<ScreenPoint> screenPoints);
        }

        /// <summary>
        /// Attempts to prepare an <see cref="XYRenderHelper{XData, YData}"/> for two axis. Throws if either axis is of the wrong type.
        /// This method should probably not be here.
        /// </summary>
        /// <param name="xaxis"></param>
        /// <param name="yaxis"></param>
        /// <returns></returns>
        public XYRenderHelper<XData, YData> TryPrepare(IAxis xaxis, IAxis yaxis)
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
        /// Prepares an <see cref="XYRenderHelper{XData, YData}"/> for two axis.
        /// </summary>
        /// <param name="xaxis"></param>
        /// <param name="yaxis"></param>
        /// <returns></returns>
        public XYRenderHelper<XData, YData> Prepare(IAxis<XData> xaxis, IAxis<YData> yaxis)
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

            private XYRenderHelper<XData, YData> _result = null;
            public XYRenderHelper<XData, YData> Result
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

            public XYRenderHelper<XData, YData> Result { get; private set; }

            private XAxisTransformation XTransformation { get; }

            public void Consume<YDataProvider, YAxisScreenTransformation>(YAxisScreenTransformation transformation)
                where YDataProvider : IDataProvider<YData>
                where YAxisScreenTransformation : IAxisScreenTransformation<YData, YDataProvider>
            {
                var typed = new TypedXYRenderHelper<XDataProvider, YDataProvider, XAxisTransformation, YAxisScreenTransformation>(XTransformation, transformation);
                Result = new XYRenderHelper<XData, YData>(typed);
            }
        }

        private class TypedXYRenderHelper<XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation> : ITypedXYRenderHelper
            where XDataProvider : IDataProvider<XData>
            where YDataProvider : IDataProvider<YData>
            where XAxisTransformation : IAxisScreenTransformation<XData, XDataProvider>
            where YAxisTransformation : IAxisScreenTransformation<YData, YDataProvider>
        {
            public TypedXYRenderHelper(XAxisTransformation xTransformation, YAxisTransformation yTransformation)
            {
                XTransformation = xTransformation;
                YTransformation = yTransformation;
            }

            public XAxisTransformation XTransformation { get; }
            public YAxisTransformation YTransformation { get; }

            public void InterpolateLines(IReadOnlyList<DataSample<XData, YData>> dataSamples, double minSegmentLength, IList<ScreenPoint> screenPoints)
            {
                RenderHelpers.InterpolateLines<XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation>(XTransformation, YTransformation, dataSamples, minSegmentLength, screenPoints);
            }
        }
    }
}
