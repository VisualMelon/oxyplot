using System;
using System.Collections.Generic;
using System.Text;

namespace OxyPlot.Axes.ComposableAxis
{
    /// <summary>
    /// Helper that just returns an untyped version of whatever would be consumed.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public class NonIAxisScreenTransformationConsumer<TData> : IAxisScreenTransformationConsumer<TData>
    {
        /// <summary>
        /// The <see cref="IAxisScreenTransformation{TData}"/> fed to this consumer.
        /// </summary>
        public IAxisScreenTransformation<TData> Transformation { get; private set; }

        /// <inheritdoc/>
        public void Consume<TDataProvider, TAxisScreenTransformation>(TAxisScreenTransformation transformation)
            where TDataProvider : IDataProvider<TData>
            where TAxisScreenTransformation : IAxisScreenTransformation<TData, TDataProvider>
        {
            this.Transformation = transformation;
        }
    }

    /// <summary>
    /// Providers method to interact with <see cref="System.Double"/>.
    /// </summary>
    public readonly struct DoubleProvider : IDataProvider<double>
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
    public readonly struct Linear : IDataTransformation<double, DoubleProvider>
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
        public IDataProvider<double> DataProvider => Provider;

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

        /// <summary>
        /// Scales the given data value into interaction units.
        /// </summary>
        /// <param name="delta"></param>
        /// <returns></returns>
        public InteractionReal Scale(double delta)
        {
            return new InteractionReal(delta);
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
    public readonly struct Logarithmic : IDataTransformation<double, DoubleProvider>
    {
        /// <summary>
        /// Initialises an instance of the <see cref="Logarithmic"/> struct.
        /// </summary>
        /// <param name="base"></param>
        public Logarithmic(double @base)
        {
            _Base = @base;
        }

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
        public IDataProvider<double> DataProvider => Provider;

        private readonly double _Base;

        /// <summary>
        /// Gets the base of the logarithm
        /// </summary>
        public double Base => _Base;

        /// <inheritdoc/>
        public double InverseTransform(InteractionReal x)
        {
            return Math.Pow(_Base, x.Value);
        }

        /// <inheritdoc/>
        public InteractionReal Transform(double data)
        {
            return new InteractionReal(Math.Log(data, _Base));
        }

        /// <inheritdoc/>
        public bool IsDiscontinuous(double a, double b)
        {
            // not sure it makes sense to not throw in this case
            return a <= 0 || b <= 0;
        }
    }

    /// <summary>
    /// A logarithmic data projection over <see cref="System.Double"/>.
    /// </summary>
    public readonly struct LogarithmicNatural : IDataTransformation<double, DoubleProvider>
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
        public IDataProvider<double> DataProvider => Provider;

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
    /// A logarithmic data projection over <see cref="System.Double"/>.
    /// </summary>
    public readonly struct Logarithmic10 : IDataTransformation<double, DoubleProvider>
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
        public IDataProvider<double> DataProvider => Provider;

        /// <inheritdoc/>
        public double InverseTransform(InteractionReal x)
        {
            return Math.Pow(10, x.Value);
        }

        /// <inheritdoc/>
        public InteractionReal Transform(double data)
        {
            return new InteractionReal(Math.Log10(data));
        }

        /// <inheritdoc/>
        public bool IsDiscontinuous(double a, double b)
        {
            // not sure it makes sense to not throw in this case
            return a <= 0 || b <= 0;
        }
    }

    /// <summary>
    /// Providers method to interact with <see cref="System.DateTime"/>.
    /// </summary>
    public readonly struct DateTimeProvider : IDataProvider<DateTime>
    {
        /// <inheritdoc/>
        public bool IsDiscrete => false;

        /// <inheritdoc/>
        public int Compare(DateTime x, DateTime y)
        {
            return x.CompareTo(y);
        }

        /// <inheritdoc/>
        public double Deinterpolate(DateTime v0, DateTime v1, DateTime v)
        {
            return (v - v0).TotalSeconds / (v1 - v0).TotalSeconds;
        }

        /// <inheritdoc/>
        public bool Equals(DateTime x, DateTime y)
        {
            return x.Equals(y);
        }

        /// <inheritdoc/>
        public int GetHashCode(DateTime obj)
        {
            return obj.GetHashCode();
        }

        /// <inheritdoc/>
        public bool Includes(DateTime min, DateTime max, DateTime v)
        {
            return min <= v && v <= max;
        }

        /// <inheritdoc/>
        public DateTime Interpolate(DateTime v0, DateTime v1, double c)
        {
            return v0 + TimeSpan.FromSeconds((v1 - v0).TotalSeconds * c); // TODO: this is not good enough
        }
    }

    /// <summary>
    /// A linear data transformation over <see cref="System.DateTime"/>.
    /// </summary>
    public readonly struct LinearDateTime : IDataTransformation<DateTime, DateTimeProvider>
    {
        /// <inheritdoc/>
        public bool IsNonDiscontinuous => true;

        /// <inheritdoc/>
        public bool IsLinear => true;

        /// <inheritdoc/>
        public bool IsDiscrete => false;

        /// <inheritdoc/>
        public DateTimeProvider Provider => default;

        /// <inheritdoc/>
        public IDataProvider<DateTime> DataProvider => Provider;

        /// <inheritdoc/>
        public DateTime InverseTransform(InteractionReal x)
        {
            return DateTimeAxis.ToDateTime(x.Value);
        }

        /// <inheritdoc/>
        public InteractionReal Transform(DateTime data)
        {
            return new InteractionReal(DateTimeAxis.ToDouble(data));
        }

        /// <summary>
        /// Scales the given delta into interaction units.
        /// </summary>
        /// <param name="delta"></param>
        /// <returns></returns>
        public InteractionReal Scale(TimeSpan delta)
        {
            return new InteractionReal(TimeSpanAxis.ToDouble(delta));
        }

        /// <inheritdoc/>
        public bool IsDiscontinuous(DateTime a, DateTime b)
        {
            return false;
        }
    }

    /// <summary>
    /// This needs a better name
    /// </summary>
    public readonly struct ViewInfo
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
        /// The Screen space offset, that is, the Screen space value to which the Interaction space zero maps.
        /// </summary>
        public readonly ScreenReal ScreenOffset;

        /// <summary>
        /// The Screen space offset, that is, the scaling between Screen space and Interaction space.
        /// </summary>
        public readonly double ScreenScale;

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
            return new InteractionReal(s.Value / ScreenScale);
        }
    }

    /// <summary>
    /// Axis color transformation.
    /// Maps interaction space to 
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TDataProvider"></typeparam>
    /// <typeparam name="TDataTransformation"></typeparam>
    /// <typeparam name="TDataFilter"></typeparam>
    public readonly struct AxisColorTransformation<TData, TDataProvider, TDataTransformation, TDataFilter> : IAxisColorTransformation<TData, TDataProvider>
        where TDataProvider : IDataProvider<TData>
        where TDataTransformation : IDataTransformation<TData, TDataProvider>
        where TDataFilter : IFilter<TData>
    {
        private readonly OxyPalette Palette;
        private readonly TDataTransformation _Transformation;

        private readonly OxyColor LowColor;
        private readonly OxyColor HighColor;
        private readonly InteractionReal InteractionMin;
        private readonly InteractionReal InteractionMax;

        /// <summary>
        /// Initialises an <see cref="AxisColorTransformation{TData, TDataProvider, TDataTransformation, TDataFilter}"/>.
        /// </summary>
        /// <param name="palette"></param>
        /// <param name="transformation"></param>
        /// <param name="filter"></param>
        /// <param name="lowColor"></param>
        /// <param name="highColor"></param>
        /// <param name="interactionMin"></param>
        /// <param name="interactionMax"></param>
        public AxisColorTransformation(OxyPalette palette, TDataTransformation transformation, TDataFilter filter, OxyColor lowColor, OxyColor highColor, InteractionReal interactionMin, InteractionReal interactionMax)
        {
            Palette = palette ?? throw new ArgumentNullException(nameof(palette));
            _Transformation = transformation;
            _Filter = filter;
            LowColor = lowColor;
            HighColor = highColor;
            InteractionMin = interactionMin;
            InteractionMax = interactionMax;
        }

        /// <inheritdoc/>
        public TDataProvider Provider => _Transformation.Provider;

        /// <summary>
        /// The filter.
        /// </summary>
        private readonly TDataFilter _Filter;

        /// <inheritdoc/>
        public bool Filter(TData data)
        {
            return _Filter.Filter(data);
        }

        /// <inheritdoc/>
        public OxyColor Transform(TData data)
        {
            var idx = GetIndex(data);

            if (idx < 0)
                return LowColor.IsUndefined() ? Palette.Colors[0] : LowColor;
            if (idx >= Palette.Colors.Count)
                return HighColor.IsUndefined() ? Palette.Colors[Palette.Colors.Count - 1] : HighColor;

            return Palette.Colors[idx];
        }

        private int GetIndex(TData data)
        {
            var i = _Transformation.Transform(data);
            var c = (i - InteractionMin).Value / (InteractionMax - InteractionMin).Value;

            return (int)(c * this.Palette.Colors.Count);
        }

        private ColorRangeTick<TData> GetRange(int index)
        {
            var c0 = (double)index / this.Palette.Colors.Count;
            var i0 = (InteractionMax - InteractionMin) * c0 + InteractionMin;
            var c1 = (double)(index + 1) / this.Palette.Colors.Count;
            var i1 = (InteractionMax - InteractionMin) * c1 + InteractionMin;

            var min = _Transformation.InverseTransform(i0);
            var max = _Transformation.InverseTransform(i1);

            return new ColorRangeTick<TData>(min, max, Palette.Colors[index]);
        }

        /// <inheritdoc/>
        public void GetColorRanges(TData minimum, TData maximum, IList<ColorRangeTick<TData>> ticks)
        {
            // this is hideous... it does all the work in iteraction space, and projects back for the result...
            // exactly what I didn't want to do for ticks

            var imin = this._Transformation.Transform(minimum);
            var imax = this._Transformation.Transform(minimum);

            if (imin > this.InteractionMax)
            {
                ticks.Add(new ColorRangeTick<TData>(
                    this._Transformation.InverseTransform(imin),
                    this._Transformation.InverseTransform(imax),
                    this.HighColor));
                return;
            }

            if (imax < this.InteractionMin)
            {
                ticks.Add(new ColorRangeTick<TData>(
                    this._Transformation.InverseTransform(imin),
                    this._Transformation.InverseTransform(imax),
                    this.LowColor));
                return;
            }

            var s = Math.Max(0, this.GetIndex(minimum));
            var e = Math.Min(Palette.Colors.Count - 1, this.GetIndex(maximum));

            for (int index = s; index <= e; index++)
            {
                var r = GetRange(index);

                if (index == s)
                {
                    r = new ColorRangeTick<TData>(
                        minimum,
                        r.Maximum,
                        r.Color);
                }

                if (index == e)
                {
                    r = new ColorRangeTick<TData>(
                        r.Minimum,
                        maximum,
                        r.Color);
                }

                ticks.Add(r);
            }
        }
    }

    /// <summary>
    /// Wraps a <typeparamref name="TDataTransformation"/> and ViewInfo to provide an <see cref="IAxisScreenTransformation{TData, TDataProvider}"/>.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TDataProvider"></typeparam>
    /// <typeparam name="TDataTransformation"></typeparam>
    /// <typeparam name="TDataFilter"></typeparam>
    public readonly struct AxisScreenTransformation<TData, TDataProvider, TDataTransformation, TDataFilter> : IAxisScreenTransformation<TData, TDataProvider>
        where TDataProvider : IDataProvider<TData>
        where TDataTransformation : IDataTransformation<TData, TDataProvider>
        where TDataFilter : IFilter<TData>
    {
        /// <summary>
        /// Initialises the <see cref="AxisScreenTransformation{TData, TDataProvider, TDataTransformation, TDataFilter}"/>.
        /// </summary>
        /// <param name="dataTransformation"></param>
        /// <param name="filter"></param>
        /// <param name="viewInfo"></param>
        /// <param name="clipMinimum">The minimum bound of the clipping region.</param>
        /// <param name="clipMaximum">The maximum bound of the clipping region.</param>
        public AxisScreenTransformation(TDataTransformation dataTransformation, TDataFilter filter, ViewInfo viewInfo, TData clipMinimum, TData clipMaximum)
        {
            _DataTransformation = dataTransformation;
            _Filter = filter;
            _ViewInfo = viewInfo;
            ClipMinimum = clipMinimum;
            ClipMaximum = clipMaximum;
        }

        /// <summary>
        /// The <typeparamref name="TDataProvider"/>.
        /// </summary>
        private readonly TDataTransformation _DataTransformation;

        /// <inheritdoc/>
        public IDataTransformation<TData, TDataProvider> DataTransformation => _DataTransformation;

        /// <summary>
        /// The <typeparamref name="TDataFilter"/>.
        /// </summary>
        private readonly TDataFilter _Filter;

        /// <summary>
        /// The ViewInfo.
        /// </summary>
        private readonly ViewInfo _ViewInfo;

        /// <inheritdoc/>
        public ViewInfo ViewInfo => _ViewInfo;

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
        public TDataProvider Provider => _DataTransformation.Provider;

        /// <inheritdoc/>
        public bool IsNonDiscontinuous => _DataTransformation.IsNonDiscontinuous;

        /// <inheritdoc/>
        public bool IsLinear => _DataTransformation.IsLinear;

        /// <inheritdoc/>
        public TData InverseTransform(ScreenReal s)
        {
            return _DataTransformation.InverseTransform(_ViewInfo.InverseTransform(s));
        }

        /// <inheritdoc/>
        public bool IsDiscontinuous(TData a, TData b)
        {
            return _DataTransformation.IsDiscontinuous(a, b);
        }

        /// <inheritdoc/>
        public ScreenReal Transform(TData data)
        {
            // inlined for perf
            return new ScreenReal(_ViewInfo.ScreenOffset.Value + _DataTransformation.Transform(data).Value * _ViewInfo.ScreenScale);
            // original: return ViewInfo.Transform(DataTransformation.Transform(data));
        }

        /// <inheritdoc/>
        public bool Filter(TData data)
        {
            return _Filter.Filter(data);
        }
    }

    /// <summary>
    /// Provides <see cref="System.Double"/> as option when <see cref="double.NaN"/>.
    /// </summary>
    public readonly struct DoubleAsNaNOptional : IOptionalProvider<double, double>
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
    /// Represents an optional type.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public readonly struct Option<TData>
    {
        private readonly bool _HasValue;
        private readonly TData Value;

        private Option(TData value)
        {
            Value = value;
            _HasValue = true;
        }

        /// <summary>
        /// Gets a value indicating whether the option has a value.
        /// </summary>
        public bool HasValue => _HasValue;

        /// <summary>
        /// Tries to get the value associated with this option.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(out TData value)
        {
            value = this.Value;
            return this.HasValue;
        }

        /// <summary>
        /// Gets a None option, which has no associated value.
        /// </summary>
        public static Option<TData> None => default;

        /// <summary>
        /// Gets an option with the given value associated.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Option<TData> Some(TData value)
        {
            return new Option<TData>(value);
        }
    }

    /// <summary>
    /// Provides <see cref="System.Double"/> as option when <see cref="double.NaN"/>.
    /// </summary>
    public readonly struct Optional<TData> : IOptionalProvider<TData, Option<TData>>
    {
        /// <inheritdoc/>
        public bool HasValue(Option<TData> optional)
        {
            return optional.HasValue;
        }

        /// <inheritdoc/>
        public bool TryGetValue(Option<TData> optional, out TData value)
        {
            return optional.TryGetValue(out value);
        }

        /// <inheritdoc/>
        public Option<TData> None => Option<TData>.None;

        /// <inheritdoc/>
        public Option<TData> Some(TData value)
        {
            return Option<TData>.Some(value);
        }
    }

    /// <summary>
    /// Provides basic methods process value.
    /// </summary>
    /// <typeparam name="VData"></typeparam>
    public interface IValueHelper<VData>
    {
        /// <summary>
        /// Finds the minimum and maximum values in the samples.
        /// </summary>
        /// <typeparam name="TSample"></typeparam>
        /// <typeparam name="TValueProvider"></typeparam>
        /// <typeparam name="TSampleFilter"></typeparam>
        /// <param name="sampleProvider"></param>
        /// <param name="sampleFilter"></param>
        /// <param name="samples"></param>
        /// <param name="minV"></param>
        /// <param name="maxV"></param>
        bool FindMinMax<TSample, TValueProvider, TSampleFilter>(TValueProvider sampleProvider, TSampleFilter sampleFilter, IReadOnlyList<TSample> samples, out VData minV, out VData maxV)
            where TValueProvider : IValueSampler<TSample, VData>
            where TSampleFilter : IFilter<TSample>;

        /// <summary>
        /// Finds the minimum and maximum X and Y values in the samples.
        /// </summary>
        /// <typeparam name="TSample"></typeparam>
        /// <typeparam name="TValueProvider"></typeparam>
        /// <typeparam name="TSampleFilter"></typeparam>
        /// <param name="sampleProvider"></param>
        /// <param name="sampleFilter"></param>
        /// <param name="samples"></param>
        /// <param name="minV"></param>
        /// <param name="maxV"></param>
        /// <param name="vMonotonicity"></param>
        bool FindMinMax<TSample, TValueProvider, TSampleFilter>(TValueProvider sampleProvider, TSampleFilter sampleFilter, IReadOnlyList<TSample> samples, out VData minV, out VData maxV, out Monotonicity vMonotonicity)
            where TValueProvider : IValueSampler<TSample, VData>
            where TSampleFilter : IFilter<TSample>;

        /// <summary>
        /// Finds the start and end indexs of a window in some data.
        /// Throws if the data is not monotonic.
        /// </summary>
        /// <typeparam name="TSample"></typeparam>
        /// <typeparam name="TValueProvider"></typeparam>
        /// <typeparam name="TSampleFilter"></typeparam>
        /// <param name="sampleProvider"></param>
        /// <param name="sampleFilter"></param>
        /// <param name="samples"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="monotonicity"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        bool FindWindow<TSample, TValueProvider, TSampleFilter>(TValueProvider sampleProvider, TSampleFilter sampleFilter, IReadOnlyList<TSample> samples, VData start, VData end, Monotonicity monotonicity, out int startIndex, out int endIndex)
            where TValueProvider : IValueSampler<TSample, VData>
            where TSampleFilter : IFilter<TSample>;

        /// <summary>
        /// Gets the value data provider.
        /// </summary>
        IDataProvider<VData> VProvider { get; }
    }

    /// <summary>
    /// Provides basic methods process value.
    /// </summary>
    /// <typeparam name="VData"></typeparam>
    public interface IAxisScreenValueHelper<VData> : IValueHelper<VData>
    {
        /// <summary>
        /// The the underlying transformation.
        /// </summary>
        IAxisScreenTransformation<VData> Transformation { get; }
    }

    /// <summary>
    /// Provides basic methods to help with colors.
    /// </summary>
    /// <typeparam name="VData"></typeparam>
    public interface IColorHelper<VData> : IValueHelper<VData>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        OxyColor Transform(VData value);

        /// <summary>
        /// Gets the underlying color transform.
        /// </summary>
        IAxisColorTransformation<VData> ColorTransformation { get; }
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
        /// <typeparam name="TSampleFilter"></typeparam>
        /// <param name="sampleProvider"></param>
        /// <param name="sampleFilter"></param>
        /// <param name="samples"></param>
        /// <param name="minX"></param>
        /// <param name="minY"></param>
        /// <param name="maxX"></param>
        /// <param name="maxY"></param>
        bool FindMinMax<TSample, TSampleProvider, TSampleFilter>(TSampleProvider sampleProvider, TSampleFilter sampleFilter, IReadOnlyList<TSample> samples, out XData minX, out YData minY, out XData maxX, out YData maxY)
            where TSampleProvider : IXYSampleProvider<TSample, XData, YData>
            where TSampleFilter : IFilter<TSample>;

        /// <summary>
        /// Finds the minimum and maximum X and Y values in the samples.
        /// </summary>
        /// <typeparam name="TSample"></typeparam>
        /// <typeparam name="TSampleProvider"></typeparam>
        /// <typeparam name="TSampleFilter"></typeparam>
        /// <param name="sampleProvider"></param>
        /// <param name="sampleFilter"></param>
        /// <param name="samples"></param>
        /// <param name="minX"></param>
        /// <param name="minY"></param>
        /// <param name="maxX"></param>
        /// <param name="maxY"></param>
        /// <param name="xMonotonicity"></param>
        /// <param name="yMonotonicity"></param>
        bool FindMinMax<TSample, TSampleProvider, TSampleFilter>(TSampleProvider sampleProvider, TSampleFilter sampleFilter, IReadOnlyList<TSample> samples, out XData minX, out YData minY, out XData maxX, out YData maxY, out Monotonicity xMonotonicity, out Monotonicity yMonotonicity)
            where TSampleProvider : IXYSampleProvider<TSample, XData, YData>
            where TSampleFilter : IFilter<TSample>;

        /// <summary>
        /// Finds the start and end indexs of a window in some data.
        /// Throws if the data is not monotonic.
        /// </summary>
        /// <typeparam name="TSample"></typeparam>
        /// <typeparam name="TSampleProvider"></typeparam>
        /// <typeparam name="TSampleFilter"></typeparam>
        /// <param name="sampleProvider"></param>
        /// <param name="sampleFilter"></param>
        /// <param name="samples"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="xMonotonicity"></param>
        /// <param name="yMonotonicity"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        bool FindWindow<TSample, TSampleProvider, TSampleFilter>(TSampleProvider sampleProvider, TSampleFilter sampleFilter, IReadOnlyList<TSample> samples, DataSample<XData, YData> start, DataSample<XData, YData> end, Monotonicity xMonotonicity, Monotonicity yMonotonicity, out int startIndex, out int endIndex)
            where TSampleProvider : IXYSampleProvider<TSample, XData, YData>
            where TSampleFilter : IFilter<TSample>;

        /// <summary>
        /// Gets the X provider.
        /// </summary>
        IDataProvider<XData> XProvider { get; }

        /// <summary>
        /// Gets the Y provider.
        /// </summary>
        IDataProvider<YData> YProvider { get; }
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
        /// <typeparam name="TSampleFilter"></typeparam>
        /// <typeparam name="ClipFilter"></typeparam>
        /// <param name="sampleProvider"></param>
        /// <param name="sampleFilter"></param>
        /// <param name="clipFilter"></param>
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
        public bool ExtractNextContinuousLineSegment<TSample, TSampleProvider, TSampleFilter, ClipFilter>(TSampleProvider sampleProvider, TSampleFilter sampleFilter, ClipFilter clipFilter, IReadOnlyList<TSample> samples, ref int sampleIdx, int endIdx, ref ScreenPoint? previousContiguousLineSegmentEndPoint, ref bool previousContiguousLineSegmentEndPointWithinClipBounds, List<ScreenPoint> broken, List<ScreenPoint> continuous)
            where TSampleProvider : IXYSampleProvider<TSample, XData, YData>
            where TSampleFilter : IFilter<TSample>
            where ClipFilter : IFilter<ScreenPoint>;

        /// <summary>
        /// Tries to find the sample that is closest to the given <see cref="ScreenPoint"/> in screen space.
        /// </summary>
        /// <typeparam name="TSample"></typeparam>
        /// <typeparam name="TSampleProvider"></typeparam>
        /// <param name="sampleProvider"></param>
        /// <param name="samples"></param>
        /// <param name="screenPoint"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="interpolate"></param>
        /// <param name="nearest"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        bool TryFindNearest<TSample, TSampleProvider>(TSampleProvider sampleProvider, IReadOnlyList<TSample> samples, ScreenPoint screenPoint, int startIndex, int endIndex, bool interpolate, out int nearest, out double distance)
            where TSampleProvider : IXYSampleProvider<TSample, XData, YData>;

        /// <summary>
        /// Transforms many <see cref="DataSample{XData, YData}"/>.
        /// </summary>
        /// <param name="xySamples"></param>
        /// <param name="screenPoints"></param>
        void TransformSamples(IReadOnlyList<DataSample<XData, YData>> xySamples, IList<ScreenPoint> screenPoints);

        /// <summary>
        /// Transforms a single <see cref="DataSample{XData, YData}"/> to a <see cref="ScreenPoint"/>.
        /// </summary>
        /// <param name="xySample"></param>
        /// <returns></returns>
        ScreenPoint TransformSample(DataSample<XData, YData> xySample);

        /// <summary>
        /// Transforms a single <see cref="ScreenPoint"/> back to a <see cref="DataSample{XData, YData}"/>.
        /// </summary>
        /// <param name="screenPoint"></param>
        /// <returns></returns>
        DataSample<XData, YData> InverseTransform(ScreenPoint screenPoint);

        /// <summary>
        /// Gets the underlying XTransformation.
        /// </summary>
        IAxisScreenTransformation<XData> XTransformation { get; }

        /// <summary>
        /// Gets the underlying YTransformation.
        /// </summary>
        IAxisScreenTransformation<YData> YTransformation { get; }
    }

    /// <summary>
    /// Prepares instances of <see cref="IValueHelper{VData}"/>.
    /// </summary>
    /// <typeparam name="VData"></typeparam>
    public class ValueHelperPreparer<VData>
    {
        private class Generator : IAxisScreenTransformationConsumer<VData>
        {
            public Generator()
            {
            }

            public void Consume<VDataProvider, TAxisScreenTransformation>(TAxisScreenTransformation transformation)
                where VDataProvider : IDataProvider<VData>
                where TAxisScreenTransformation : IAxisScreenTransformation<VData, VDataProvider>
            {
                Result = new ValueHelper<VData, VDataProvider>(transformation.Provider);
            }

            public IValueHelper<VData> Result { get; private set; }
        }

        /// <summary>
        /// Prepares a <see cref="IAxisScreenValueHelper{VData}"/> for an axis.
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static IValueHelper<VData> Prepare(IAxis<VData> axis)
        {
            var generator = new Generator();
            axis.ConsumeTransformation(generator);
            return generator.Result;
        }
    }

    /// <summary>
    /// Prepares instances of <see cref="IValueHelper{VData}"/>.
    /// </summary>
    /// <typeparam name="VData"></typeparam>
    public class AxisValueHelperPreparer<VData>
    {
        private class Generator : IAxisScreenTransformationConsumer<VData>
        {
            public Generator()
            {
            }

            public void Consume<VDataProvider, TAxisScreenTransformation>(TAxisScreenTransformation transformation)
                where VDataProvider : IDataProvider<VData>
                where TAxisScreenTransformation : IAxisScreenTransformation<VData, VDataProvider>
            {
                Result = new AxisValueHelper<VData, VDataProvider, TAxisScreenTransformation>(transformation);
            }

            public IAxisScreenValueHelper<VData> Result { get; private set; }
        }

        /// <summary>
        /// Prepares a <see cref="IAxisScreenValueHelper{VData}"/> for an axis.
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static IAxisScreenValueHelper<VData> Prepare(IAxis<VData> axis)
        {
            var generator = new Generator();
            axis.ConsumeTransformation(generator);
            return generator.Result;
        }
    }

    /// <summary>
    /// Prepares instances of <see cref="IValueHelper{VData}"/>.
    /// </summary>
    /// <typeparam name="VData"></typeparam>
    public class ColorHelperPreparer<VData>
    {
        private class Generator : IAxisColorTransformationConsumer<VData>
        {
            public Generator()
            {
            }

            public void Consume<VDataProvider, TAxisColorTransformation>(TAxisColorTransformation transformation)
                where VDataProvider : IDataProvider<VData>
                where TAxisColorTransformation : IAxisColorTransformation<VData, VDataProvider>
            {
                Result = new ColorHelper<VData, VDataProvider, TAxisColorTransformation>(transformation);
            }

            public IColorHelper<VData> Result { get; private set; }
        }

        /// <summary>
        /// Prepares a <see cref="IColorHelper{VData}"/> for a horizontal or vertical axis.
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static IColorHelper<VData> Prepare(IColorAxis<VData> axis)
        {
            var generator = new Generator();
            axis.ConsumeTransformation(generator);
            return generator.Result;
        }
    }

    /// <summary>
    /// Value helper
    /// </summary>
    /// <typeparam name="VData"></typeparam>
    /// <typeparam name="VDataProvider"></typeparam>
    public class ValueHelper<VData, VDataProvider> : IValueHelper<VData>
        where VDataProvider : IDataProvider<VData>
    {
        private readonly VDataProvider _VDataProvider;

        /// <summary>
        /// Initialises an isntance of the <see cref="ValueHelper{VData, VDataProvider}"/> class.
        /// </summary>
        /// <param name="vDataProvider"></param>
        public ValueHelper(VDataProvider vDataProvider)
        {
            _VDataProvider = vDataProvider;
        }

        /// <inheritdoc/>
        public IDataProvider<VData> VProvider => _VDataProvider;

        /// <inheritdoc/>
        public bool FindMinMax<TSample, TValueProvider, TSampleFilter>(TValueProvider valueProvider, TSampleFilter sampleFilter, IReadOnlyList<TSample> samples, out VData minV, out VData maxV)
            where TValueProvider : IValueSampler<TSample, VData>
            where TSampleFilter : IFilter<TSample>
        {
            return ValueHelpers.FindMinMax(valueProvider, sampleFilter, _VDataProvider, samples, out minV, out maxV);
        }

        /// <inheritdoc/>
        public bool FindMinMax<TSample, TValueProvider, TSampleFilter>(TValueProvider valueProvider, TSampleFilter sampleFilter, IReadOnlyList<TSample> samples, out VData minV, out VData maxV, out Monotonicity vMonotonicity)
            where TValueProvider : IValueSampler<TSample, VData>
            where TSampleFilter : IFilter<TSample>
        {
            return ValueHelpers.FindMinMax(valueProvider, sampleFilter, _VDataProvider, samples, out minV, out maxV, out vMonotonicity);
        }

        /// <inheritdoc/>
        public bool FindWindow<TSample, TValueProvider, TSampleFilter>(TValueProvider valueProvider, TSampleFilter sampleFilter, IReadOnlyList<TSample> samples, VData start, VData end, Monotonicity monotonicity, out int startIndex, out int endIndex)
            where TValueProvider : IValueSampler<TSample, VData>
            where TSampleFilter : IFilter<TSample>
        {
            return ValueHelpers.FindWindow(valueProvider, sampleFilter, _VDataProvider, samples, start, end, monotonicity, out startIndex, out endIndex);
        }
    }

    /// <summary>
    /// Value helper
    /// </summary>
    /// <typeparam name="VData"></typeparam>
    /// <typeparam name="VDataProvider"></typeparam>
    /// <typeparam name="VDataTransformation"></typeparam>
    public class AxisValueHelper<VData, VDataProvider, VDataTransformation> : ValueHelper<VData, VDataProvider>, IAxisScreenValueHelper<VData>
        where VDataProvider : IDataProvider<VData>
        where VDataTransformation : IAxisScreenTransformation<VData, VDataProvider>
    {
        /// <summary>
        /// Initialises an isntance of the <see cref="AxisValueHelper{VData, VDataProvider, VDataTransformation}"/> class.
        /// </summary>
        /// <param name="transformation"></param>
        public AxisValueHelper(VDataTransformation transformation)
            : base(transformation.Provider)
        {
            _Transformation = transformation;
        }

        /// <summary>
        /// Gets the underlying transformation.
        /// </summary>
        public VDataTransformation _Transformation { get; }

        /// <inheritdoc/>
        public IAxisScreenTransformation<VData> Transformation => _Transformation;
    }

    /// <summary>
    /// Provides basic methods to render in X/Y space
    /// </summary>
    /// <typeparam name="VData"></typeparam>
    /// <typeparam name="VDataProvider"></typeparam>
    /// <typeparam name="VAxisColorTransformation"></typeparam>
    public class ColorHelper<VData, VDataProvider, VAxisColorTransformation> : ValueHelper<VData, VDataProvider>, IColorHelper<VData>
        where VDataProvider : IDataProvider<VData>
        where VAxisColorTransformation : IAxisColorTransformation<VData, VDataProvider>
    {
        /// <summary>
        /// Initialises an instance of the <see cref="ColorHelper{VData, VDataProvider, VAxisColorTransformation}"/> class.
        /// </summary>
        /// <param name="transformation"></param>
        public ColorHelper(VAxisColorTransformation transformation)
            : base(transformation.Provider)
        {
            _Transformation = transformation;
        }

        /// <summary>
        /// Gets the underlying color transform.
        /// </summary>
        public VAxisColorTransformation _Transformation { get; }

        /// <inheritdoc/>
        public IAxisColorTransformation<VData> ColorTransformation => _Transformation;

        /// <inheritdoc/>
        public OxyColor Transform(VData value)
        {
            return _Transformation.Transform(value);
        }
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
            public Generator(bool transpose)
            {
                Transpose = transpose;
            }

            public bool Transpose { get; }

            public void Consume<XDataProvider, YDataProvider, XAxisScreenTransformation, YAxisScreenTransformation>(XAxisScreenTransformation x, YAxisScreenTransformation y)
                where XDataProvider : IDataProvider<XData>
                where YDataProvider : IDataProvider<YData>
                where XAxisScreenTransformation : IAxisScreenTransformation<XData, XDataProvider>
                where YAxisScreenTransformation : IAxisScreenTransformation<YData, YDataProvider>
            {
                if (Transpose)
                {
                    var xyTransformation = new TransposedHorizontalVertialXYTransformation<XData, YData, XDataProvider, YDataProvider, XAxisScreenTransformation, YAxisScreenTransformation>(x, y);
                    Result = new XYRenderHelper<XData, YData, XDataProvider, YDataProvider, XAxisScreenTransformation, YAxisScreenTransformation, TransposedHorizontalVertialXYTransformation<XData, YData, XDataProvider, YDataProvider, XAxisScreenTransformation, YAxisScreenTransformation>>(xyTransformation);
                }
                else
                {
                    var xyTransformation = new HorizontalVertialXYTransformation<XData, YData, XDataProvider, YDataProvider, XAxisScreenTransformation, YAxisScreenTransformation>(x, y);
                    Result = new XYRenderHelper<XData, YData, XDataProvider, YDataProvider, XAxisScreenTransformation, YAxisScreenTransformation, HorizontalVertialXYTransformation<XData, YData, XDataProvider, YDataProvider, XAxisScreenTransformation, YAxisScreenTransformation>>(xyTransformation);
                }
            }

            public IXYHelper<XData, YData> Result { get; private set; }
        }

        /// <summary>
        /// Prepares an <see cref="IXYHelper{XData, YData}"/> from the given collator.
        /// </summary>
        /// <param name="collator"></param>
        /// <param name="transpose"></param>
        /// <returns></returns>
        public static IXYHelper<XData, YData> PrepareHorizontalVertial(XYCollator<XData, YData> collator, bool transpose)
        {
            var generator = new Generator(transpose);
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
            public Generator(bool transpose)
            {
                Transpose = transpose;
            }

            public bool Transpose { get; }

            public void Consume<XDataProvider, YDataProvider, XAxisScreenTransformation, YAxisScreenTransformation>(XAxisScreenTransformation x, YAxisScreenTransformation y)
                where XDataProvider : IDataProvider<XData>
                where YDataProvider : IDataProvider<YData>
                where XAxisScreenTransformation : IAxisScreenTransformation<XData, XDataProvider>
                where YAxisScreenTransformation : IAxisScreenTransformation<YData, YDataProvider>
            {
                if (Transpose)
                {
                    var xyTransformation = new TransposedHorizontalVertialXYTransformation<XData, YData, XDataProvider, YDataProvider, XAxisScreenTransformation, YAxisScreenTransformation>(x, y);
                    Result = new XYRenderHelper<XData, YData, XDataProvider, YDataProvider, XAxisScreenTransformation, YAxisScreenTransformation, TransposedHorizontalVertialXYTransformation<XData, YData, XDataProvider, YDataProvider, XAxisScreenTransformation, YAxisScreenTransformation>>(xyTransformation);
                }
                else
                {
                    var xyTransformation = new HorizontalVertialXYTransformation<XData, YData, XDataProvider, YDataProvider, XAxisScreenTransformation, YAxisScreenTransformation>(x, y);
                    Result = new XYRenderHelper<XData, YData, XDataProvider, YDataProvider, XAxisScreenTransformation, YAxisScreenTransformation, HorizontalVertialXYTransformation<XData, YData, XDataProvider, YDataProvider, XAxisScreenTransformation, YAxisScreenTransformation>>(xyTransformation);
                }
            }

            public IXYRenderHelper<XData, YData> Result { get; private set; }
        }

        /// <summary>
        /// Prepares an <see cref="IXYHelper{XData, YData}"/> from the given collator.
        /// </summary>
        /// <param name="collator"></param>
        /// <param name="transpose"></param>
        /// <returns></returns>
        public static IXYRenderHelper<XData, YData> PrepareHorizontalVertial(XYCollator<XData, YData> collator, bool transpose)
        {
            var generator = new Generator(transpose);
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
            _XProvider = xProvider;
            _YProvider = yProvider;
        }

        private readonly XDataProvider _XProvider;
        private readonly YDataProvider _YProvider;

        /// <inheritdoc/>
        IDataProvider<XData> IXYHelper<XData, YData>.XProvider => _XProvider;

        /// <inheritdoc/>
        IDataProvider<YData> IXYHelper<XData, YData>.YProvider => _YProvider;

        /// <inheritdoc/>
        public bool FindMinMax<TSample, TSampleProvider, TSampleFilter>(TSampleProvider sampleProvider, TSampleFilter sampleFilter, IReadOnlyList<TSample> samples, out XData minX, out YData minY, out XData maxX, out YData maxY)
            where TSampleProvider : IXYSampleProvider<TSample, XData, YData>
            where TSampleFilter : IFilter<TSample>
        {
            return XYHelpers.TryFindMinMax(sampleProvider, sampleFilter, _XProvider, _YProvider, samples, out minX, out minY, out maxX, out maxY);
        }

        /// <inheritdoc/>
        public bool FindMinMax<TSample, TSampleProvider, TSampleFilter>(TSampleProvider sampleProvider, TSampleFilter sampleFilter, IReadOnlyList<TSample> samples, out XData minX, out YData minY, out XData maxX, out YData maxY, out Monotonicity xMonotonicity, out Monotonicity yMonotonicity)
            where TSampleProvider : IXYSampleProvider<TSample, XData, YData>
            where TSampleFilter : IFilter<TSample>
        {
            return XYHelpers.TryFindMinMax(sampleProvider, sampleFilter, _XProvider, _YProvider, samples, out minX, out minY, out maxX, out maxY, out xMonotonicity, out yMonotonicity);
        }

        /// <inheritdoc/>
        public bool FindWindow<TSample, TSampleProvider, TSampleFilter>(TSampleProvider sampleProvider, TSampleFilter sampleFilter, IReadOnlyList<TSample> samples, DataSample<XData, YData> start, DataSample<XData, YData> end, Monotonicity xMonotonicity, Monotonicity yMonotonicity, out int startIndex, out int endIndex)
            where TSampleProvider : IXYSampleProvider<TSample, XData, YData>
            where TSampleFilter : IFilter<TSample>
        {
            return XYHelpers.FindWindow(sampleProvider, sampleFilter, _XProvider, _YProvider, samples, start, end, xMonotonicity, yMonotonicity, out startIndex, out endIndex);
        }
    }

    /// <summary>
    /// Provides an implemention of <see cref="IXYAxisTransformation{XData, YData}"/> for a horizontal X axis, and a vertical Y axis.
    /// </summary>
    /// <typeparam name="XData"></typeparam>
    /// <typeparam name="YData"></typeparam>
    /// <typeparam name="XDataProvider"></typeparam>
    /// <typeparam name="YDataProvider"></typeparam>
    /// <typeparam name="XAxisTransformation"></typeparam>
    /// <typeparam name="YAxisTransformation"></typeparam>
    public readonly struct HorizontalVertialXYTransformation<XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation> : IXYAxisTransformation<XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation>
        where XDataProvider : IDataProvider<XData>
        where YDataProvider : IDataProvider<YData>
        where XAxisTransformation : IAxisScreenTransformation<XData, XDataProvider>
        where YAxisTransformation : IAxisScreenTransformation<YData, YDataProvider>
    {
        /// <summary>
        /// Initialises and instance of the <see cref="HorizontalVertialXYTransformation{XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation}"/> struct.
        /// </summary>
        /// <param name="xTransformation"></param>
        /// <param name="yTransformation"></param>
        public HorizontalVertialXYTransformation(XAxisTransformation xTransformation, YAxisTransformation yTransformation)
        {
            _XTransformation = xTransformation;
            _YTransformation = yTransformation;
        }

        /// <summary>
        /// The x transformation.
        /// </summary>
        private readonly XAxisTransformation _XTransformation;

        /// <summary>
        /// Gets the x transformation.
        /// </summary>
        public XAxisTransformation XTransformation => _XTransformation;

        /// <summary>
        /// The x transformation.
        /// </summary>
        private readonly YAxisTransformation _YTransformation;

        /// <summary>
        /// Gets the x transformation.
        /// </summary>
        public YAxisTransformation YTransformation => _YTransformation;

        /// <inheritdoc/>
        public DataSample<XData, YData> InverseArrangeTransform(ScreenPoint screenPoint)
        {
            InverseArrange(screenPoint, out var x, out var y);
            return new DataSample<XData, YData>(_XTransformation.InverseTransform(x), _YTransformation.InverseTransform(y));
        }

        /// <inheritdoc/>
        public ScreenPoint ArrangeTransform(DataSample<XData, YData> sample)
        {
            return Arrange(_XTransformation.Transform(sample.X), _YTransformation.Transform(sample.Y));
        }

        /// <inheritdoc/>
        public void InverseArrange(ScreenPoint point, out ScreenReal x, out ScreenReal y)
        {
            x = new ScreenReal(point.X);
            y = new ScreenReal(point.Y);
        }

        /// <inheritdoc/>
        public ScreenPoint Arrange(ScreenReal x, ScreenReal y)
        {
            return new ScreenPoint(x.Value, y.Value);
        }
    }

    /// <summary>
    /// Provides an implemention of <see cref="IXYAxisTransformation{XData, YData}"/> for a vertical X axis, and a horizontal Y axis.
    /// </summary>
    /// <typeparam name="XData"></typeparam>
    /// <typeparam name="YData"></typeparam>
    /// <typeparam name="XDataProvider"></typeparam>
    /// <typeparam name="YDataProvider"></typeparam>
    /// <typeparam name="XAxisTransformation"></typeparam>
    /// <typeparam name="YAxisTransformation"></typeparam>
    public readonly struct TransposedHorizontalVertialXYTransformation<XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation> : IXYAxisTransformation<XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation>
        where XDataProvider : IDataProvider<XData>
        where YDataProvider : IDataProvider<YData>
        where XAxisTransformation : IAxisScreenTransformation<XData, XDataProvider>
        where YAxisTransformation : IAxisScreenTransformation<YData, YDataProvider>
    {
        /// <summary>
        /// Initialises and instance of the <see cref="HorizontalVertialXYTransformation{XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation}"/> struct.
        /// </summary>
        /// <param name="xTransformation"></param>
        /// <param name="yTransformation"></param>
        public TransposedHorizontalVertialXYTransformation(XAxisTransformation xTransformation, YAxisTransformation yTransformation)
        {
            _XTransformation = xTransformation;
            _YTransformation = yTransformation;
        }

        /// <summary>
        /// The x transformation.
        /// </summary>
        private readonly XAxisTransformation _XTransformation;

        /// <summary>
        /// Gets the x transformation.
        /// </summary>
        public XAxisTransformation XTransformation => _XTransformation;

        /// <summary>
        /// The x transformation.
        /// </summary>
        private readonly YAxisTransformation _YTransformation;

        /// <summary>
        /// Gets the x transformation.
        /// </summary>
        public YAxisTransformation YTransformation => _YTransformation;

        /// <inheritdoc/>
        public DataSample<XData, YData> InverseArrangeTransform(ScreenPoint screenPoint)
        {
            InverseArrange(screenPoint, out var x, out var y);
            return new DataSample<XData, YData>(_XTransformation.InverseTransform(x), _YTransformation.InverseTransform(y));
        }

        /// <inheritdoc/>
        public ScreenPoint ArrangeTransform(DataSample<XData, YData> sample)
        {
            return Arrange(_XTransformation.Transform(sample.X), _YTransformation.Transform(sample.Y));
        }

        /// <inheritdoc/>
        public void InverseArrange(ScreenPoint point, out ScreenReal x, out ScreenReal y)
        {
            x = new ScreenReal(point.Y);
            y = new ScreenReal(point.X);
        }

        /// <inheritdoc/>
        public ScreenPoint Arrange(ScreenReal x, ScreenReal y)
        {
            return new ScreenPoint(y.Value, x.Value);
        }
    }

    /// <summary>
    /// Represents a value filter.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public interface IFilter<TData>
    {
        /// <summary>
        /// Returns <c>true</c> if the value should be kept, otherwise <c>false</c>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Filter(TData value);
    }

    /// <summary>
    /// A filter that accepts everything.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public readonly struct AcceptAllFilter<TData> : IFilter<TData>
    {
        /// <inheritdoc/>
        public bool Filter(TData value)
        {
            return true;
        }
    }

    /// <summary>
    /// A filter that rejects everything.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public readonly struct RejectAllFilter<TData> : IFilter<TData>
    {
        /// <inheritdoc/>
        public bool Filter(TData value)
        {
            return false;
        }
    }

    /// <summary>
    /// A filter that rejects elemnts outside of a given range.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TProvider"></typeparam>
    public readonly struct MinMaxFilter<TData, TProvider> : IFilter<TData>
        where TProvider : IDataProvider<TData>
    {
        /// <summary>
        /// Initialises an instance of the <see cref="MinMaxFilter{TData, TProvider}"/> struct.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public MinMaxFilter(TProvider provider, TData min, TData max)
        {
            Provider = provider;
            Min = min;
            Max = max;
        }

        private readonly TProvider Provider;
        private readonly TData Min;
        private readonly TData Max;

        /// <inheritdoc/>
        public bool Filter(TData value)
        {
            return Provider.Includes(Min, Max, value);
        }
    }

    /// <summary>
    /// Filters <see cref="ScreenPoint"/> to a given rectangle.
    /// </summary>
    public readonly struct RectangleFilter : IFilter<ScreenPoint>
    {
        private readonly OxyRect Rect;

        /// <summary>
        /// Initialises a new <see cref="RectangleFilter"/> with the given <see cref="OxyRect"/>.
        /// </summary>
        /// <param name="rect"></param>
        public RectangleFilter(OxyRect rect)
        {
            Rect = rect;
        }

        /// <inheritdoc/>
        public bool Filter(ScreenPoint value)
        {
            return Rect.Contains(value);
        }
    }

    /// <summary>
    /// A filter that defers judgement to a delegate.
    /// </summary>
    public readonly struct DelegateFilter<TData> : IFilter<TData>
    {
        /// <summary>
        /// Initialises an instance of the <see cref="DelegateFilter{TData}"/> struct.
        /// </summary>
        public DelegateFilter(Predicate<TData> predicate)
        {
            Predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        }

        private readonly Predicate<TData> Predicate { get; }

        /// <inheritdoc/>
        public bool Filter(TData value)
        {
            return Predicate(value);
        }
    }

    /// <summary>
    /// A filter that defers judgement to a delegate.
    /// </summary>
    public readonly struct DelegateValueProvider<TSample, VData> : IValueSampler<TSample, VData>
    {
        /// <summary>
        /// Initialises an instance of the <see cref="DelegateValueProvider{TSample, VData}"/> struct.
        /// </summary>
        public DelegateValueProvider(Func<TSample, VData> mapping)
        {
            Mapping = mapping ?? throw new ArgumentNullException(nameof(mapping));
        }

        private readonly Func<TSample, VData> Mapping { get; }

        /// <inheritdoc/>
        public bool IsInvalid(TSample sample)
        {
            return false;
        }

        /// <inheritdoc/>
        public VData Sample(TSample sample)
        {
            return Mapping(sample);
        }

        /// <inheritdoc/>
        public bool TrySample(TSample sample, out VData result)
        {
            result = Mapping(sample);
            return true;
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
    /// <typeparam name="XYAxisTransformation"></typeparam>
    public class XYRenderHelper<XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation, XYAxisTransformation> : XYHelper<XData, YData, XDataProvider, YDataProvider>, IXYRenderHelper<XData, YData>
        where XDataProvider : IDataProvider<XData>
        where YDataProvider : IDataProvider<YData>
        where XAxisTransformation : IAxisScreenTransformation<XData, XDataProvider>
        where YAxisTransformation : IAxisScreenTransformation<YData, YDataProvider>
        where XYAxisTransformation : IXYAxisTransformation<XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation>
    {
        /// <summary>
        /// Initialises an XYRenderHelper.
        /// </summary>
        /// <param name="xyTransformation"></param>
        public XYRenderHelper(XYAxisTransformation xyTransformation)
            : base(xyTransformation.XTransformation.Provider, xyTransformation.YTransformation.Provider)
        {
            _XYTransformation = xyTransformation;
        }

        private readonly XYAxisTransformation _XYTransformation;

        /// <summary>
        /// The XY transformation.
        /// </summary>
        public XYAxisTransformation XYTransformation => _XYTransformation;

        /// <inheritdoc/>
        IAxisScreenTransformation<XData> IXYRenderHelper<XData, YData>.XTransformation => _XYTransformation.XTransformation;

        /// <inheritdoc/>
        IAxisScreenTransformation<YData> IXYRenderHelper<XData, YData>.YTransformation => _XYTransformation.YTransformation;

        /// <inheritdoc/>
        public bool ExtractNextContinuousLineSegment<TSample, TSampleProvider, TSampleFilter, ClipFilter>(TSampleProvider sampleProvider, TSampleFilter sampleFilter, ClipFilter clipFilter, IReadOnlyList<TSample> samples, ref int sampleIdx, int endIdx, ref ScreenPoint? previousContiguousLineSegmentEndPoint, ref bool previousContiguousLineSegmentEndPointWithinClipBounds, List<ScreenPoint> broken, List<ScreenPoint> continuous)
            where TSampleProvider : IXYSampleProvider<TSample, XData, YData>
            where TSampleFilter : IFilter<TSample>
            where ClipFilter : IFilter<ScreenPoint>
        {
            return RenderHelpers.ExtractNextContinuousLineSegment<TSample, TSampleProvider, TSampleFilter, XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation, XYAxisTransformation, ClipFilter>(sampleProvider, sampleFilter, _XYTransformation, clipFilter, samples, ref sampleIdx, endIdx, ref previousContiguousLineSegmentEndPoint, ref previousContiguousLineSegmentEndPointWithinClipBounds, broken, continuous);
        }

        /// <inheritdoc/>
        public bool TryFindNearest<TSample, TSampleProvider>(TSampleProvider sampleProvider, IReadOnlyList<TSample> samples, ScreenPoint screenPoint, int startIndex, int endIndex, bool interpolate, out int nearest, out double distance)
            where TSampleProvider : IXYSampleProvider<TSample, XData, YData>
        {
            return RenderHelpers.TryFindNearest<TSample, TSampleProvider, XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation, XYAxisTransformation>(sampleProvider, _XYTransformation, samples, screenPoint, startIndex, endIndex, interpolate, out nearest, out distance);
        }

        /// <inheritdoc/>
        public void InterpolateLines(IReadOnlyList<DataSample<XData, YData>> dataSamples, double minSegmentLength, IList<ScreenPoint> screenPoints)
        {
            RenderHelpers.InterpolateLines<XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation, XYAxisTransformation>(_XYTransformation, dataSamples, minSegmentLength, screenPoints);
        }

        /// <inheritdoc/>
        public void TransformSamples(IReadOnlyList<DataSample<XData, YData>> dataSamples, IList<ScreenPoint> screenPoints)
        {
            RenderHelpers.TransformSamples<XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation, XYAxisTransformation>(_XYTransformation, dataSamples, screenPoints);
        }

        /// <inheritdoc/>
        public ScreenPoint TransformSample(DataSample<XData, YData> sample)
        {
            return XYTransformation.ArrangeTransform(sample);
        }

        /// <inheritdoc/>
        public DataSample<XData, YData> InverseTransform(ScreenPoint screenPoint)
        {
            return XYTransformation.InverseArrangeTransform(screenPoint);
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
                        XAxis.ConsumeTransformation(this);
                    return _result;
                }
            }

            public void Consume<XDataProvider, XAxisScreenTransformation>(XAxisScreenTransformation transformation)
                where XDataProvider : IDataProvider<XData>
                where XAxisScreenTransformation : IAxisScreenTransformation<XData, XDataProvider>
            {
                var yconsumer = new YConsumer<XDataProvider, XAxisScreenTransformation>(transformation);
                YAxis.ConsumeTransformation(yconsumer);
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
                        XAxis.ConsumeTransformation(this);
                    return _result;
                }
            }

            public void Consume<XDataProvider, XAxisScreenTransformation>(XAxisScreenTransformation transformation)
                where XDataProvider : IDataProvider<XData>
                where XAxisScreenTransformation : IAxisScreenTransformation<XData, XDataProvider>
            {
                var yconsumer = new YConsumer<XDataProvider, XAxisScreenTransformation>(transformation, ZAxis);
                YAxis.ConsumeTransformation(yconsumer);
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
                ZAxis.ConsumeTransformation(zconsumer);
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
    public readonly struct IdentityXYSampleProvider<XData, YData> : IXYSampleProvider<DataSample<XData, YData>, XData, YData>
    {
        /// <inheritdoc/>
        public bool IsInvalid(DataSample<XData, YData> sample)
        {
            return false;
        }

        /// <inheritdoc/>
        public DataSample<XData, YData> Sample(DataSample<XData, YData> sample)
        {
            return sample;
        }

        /// <inheritdoc/>
        public bool TrySample(DataSample<XData, YData> sample, out DataSample<XData, YData> result)
        {
            result = sample;
            return true;
        }
    }

    /// <summary>
    /// Provides a mapping from <see cref="DataPoint"/> to a <see cref="DataSample{XData, YData}"/> of doubles.
    /// </summary>
    public readonly struct DataPointXYSampleProvider : IXYSampleProvider<DataPoint, double, double>
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

    /// <summary>
    /// Spacing options class.
    /// </summary>
    public class SpacingOptions
    {
        /// <summary>
        /// Initialises an instance of the <see cref="SpacingOptions"/> class.
        /// </summary>
        public SpacingOptions()
        {
            MaximumTickCount = 100;
            MinimumTickCount = 2;
            MinimumIntervalSize = 10;
            MaximumIntervalSize = 60;
        }

        /// <summary>
        /// Gets the maximum allowed number of ticks.
        /// </summary>
        public int MaximumTickCount { get; set; }

        /// <summary>
        /// Gets the minimum allowed number of ticks.
        /// </summary>
        public int MinimumTickCount { get; set; }

        /// <summary>
        /// Gets the minimum allowed step.
        /// </summary>
        public double MinimumIntervalSize { get; set; }

        /// <summary>
        /// Gets the minimum allowed step.
        /// </summary>
        public double MaximumIntervalSize { get; set; }
    }

    /// <summary>
    /// Provides basic methods to render ticks
    /// </summary>
    public interface ITickRenderHelper<TData>
    {
        /// <summary>
        /// Renders a whole load of <see cref="TickStyle"/>.
        /// </summary>
        /// <param name="renderContext"></param>
        /// <param name="bandLocation"></param>
        /// <param name="ticks"></param>
        /// <param name="tickStyle"></param>
        /// <param name="tickLength"></param>
        /// <param name="strokeThickness"></param>
        /// <param name="color"></param>
        /// <param name="labelFont"></param>
        /// <param name="labelFontSize"></param>
        /// <param name="labelFontWeight"></param>
        /// <param name="labelColor"></param>
        /// <param name="labelAngle"></param>
        /// <param name="AxisTickToLabelDistance"></param>
        void RenderTicks(IRenderContext renderContext, BandLocation bandLocation, IReadOnlyList<Tick<TData>> ticks, TickStyle tickStyle, double tickLength, double strokeThickness, OxyColor color, string labelFont, double labelFontSize, double labelFontWeight, OxyColor labelColor, double labelAngle, double AxisTickToLabelDistance);

        /// <summary>
        /// Measures a whole load of <see cref="TickStyle"/>.
        /// </summary>
        /// <param name="renderContext"></param>
        /// <param name="bandLocation"></param>
        /// <param name="ticks"></param>
        /// <param name="tickStyle"></param>
        /// <param name="tickLength"></param>
        /// <param name="strokeThickness"></param>
        /// <param name="color"></param>
        /// <param name="labelFont"></param>
        /// <param name="labelFontSize"></param>
        /// <param name="labelFontWeight"></param>
        /// <param name="labelColor"></param>
        /// <param name="labelAngle"></param>
        /// <param name="AxisTickToLabelDistance"></param>
        BandExcesses MeasureTicks(IRenderContext renderContext, BandLocation bandLocation, IReadOnlyList<Tick<TData>> ticks, TickStyle tickStyle, double tickLength, double strokeThickness, OxyColor color, string labelFont, double labelFontSize, double labelFontWeight, OxyColor labelColor, double labelAngle, double AxisTickToLabelDistance);

        /// <summary>
        /// Renders a whole load of <see cref="ColorRangeTick{TData}"/>.
        /// </summary>
        /// <param name="renderContext"></param>
        /// <param name="bandLocation"></param>
        /// <param name="ticks"></param>
        /// <param name="tickStyle"></param>
        /// <param name="barWidth"></param>
        /// <param name="lowColor"></param>
        /// <param name="highColor"></param>
        /// <param name="highLowExcess"></param>
        void RenderColorRangeTicks(IRenderContext renderContext, BandLocation bandLocation, IReadOnlyList<ColorRangeTick<TData>> ticks, TickStyle tickStyle, double barWidth, OxyColor lowColor, OxyColor highColor, double highLowExcess);

        /// <summary>
        /// Measures a whole load of <see cref="ColorRangeTick{TData}"/>.
        /// </summary>
        /// <param name="renderContext"></param>
        /// <param name="bandLocation"></param>
        /// <param name="ticks"></param>
        /// <param name="tickStyle"></param>
        /// <param name="barWidth"></param>
        /// <param name="lowColor"></param>
        /// <param name="highColor"></param>
        /// <param name="highLowExcess"></param>
        BandExcesses MeasureColorRangeTicks(IRenderContext renderContext, BandLocation bandLocation, IReadOnlyList<ColorRangeTick<TData>> ticks, TickStyle tickStyle, double barWidth, OxyColor lowColor, OxyColor highColor, double highLowExcess);
    }

    /// <summary>
    /// Provides methods to help render ticks on horizontal and vertical axes.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TDataProvider"></typeparam>
    /// <typeparam name="TAxisScreenTransformation"></typeparam>
    public class HorizontalVerticalAxisRenderHelper<TData, TDataProvider, TAxisScreenTransformation> : ITickRenderHelper<TData>
        where TDataProvider : IDataProvider<TData>
        where TAxisScreenTransformation : IAxisScreenTransformation<TData, TDataProvider>
    {
        /// <summary>
        /// Initialises an instance of the <see cref="HorizontalVerticalAxisRenderHelper{TData, TDataProvider, TAxisScreenTransformation}"/> class.
        /// </summary>
        /// <param name="axisScreenTransformation"></param>
        /// <param name="axisPosition"></param>
        /// <param name="theOtherCoordinate"></param>
        public HorizontalVerticalAxisRenderHelper(TAxisScreenTransformation axisScreenTransformation, AxisPosition axisPosition, ScreenReal theOtherCoordinate)
        {
            AxisScreenTransformation = axisScreenTransformation;
            AxisPosition = axisPosition;
            TheOtherCoordinate = theOtherCoordinate;
        }

        /// <summary>
        /// Whether the axis is vertical
        /// </summary>
        public bool IsVertical => AxisPosition == AxisPosition.Left || AxisPosition == AxisPosition.Right;

        /// <summary>
        /// The axis transformation.
        /// </summary>
        public TAxisScreenTransformation AxisScreenTransformation { get; }

        /// <summary>
        /// The axis position.
        /// </summary>
        public AxisPosition AxisPosition { get; }

        /// <summary>
        /// The other coordinate in the pair.
        /// </summary>
        /// <remarks>
        /// If <see cref="IsVertical"/> is <c>true</c>, then this is the horizontal coordinate.
        /// If <see cref="IsVertical"/> is <c>false</c>, then this is the vertical coordinate.
        /// </remarks>
        public ScreenReal TheOtherCoordinate { get; }

        /// <summary>
        /// Transforms the given value onto the axis line.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="bandLocation"></param>
        /// <returns></returns>
        public ScreenPoint Transform(TData value, BandLocation bandLocation)
        {
            var s = AxisScreenTransformation.Transform(value);

            // TODO: we can get away with this for HorizontalVertical axis; to be more general, we need a second reference which tells us how to interpret s
            // for example, on a magnitude axis, s will NOT be a screen-space axis aligned variable: it will be a screen-space distance along the parallel
            if (IsVertical)
            {
                return new ScreenPoint(bandLocation.Reference.X, s.Value);
            }
            else
            {
                return new ScreenPoint(s.Value, bandLocation.Reference.Y);
            }
        }

        /// <inheritdoc/>
        public void RenderTicks(IRenderContext renderContext, BandLocation bandLocation, IReadOnlyList<Tick<TData>> ticks, TickStyle tickStyle, double tickLength, double strokeThickness, OxyColor color, string labelFont, double labelFontSize, double labelFontWeight, OxyColor labelColor, double labelAngle, double AxisTickToLabelDistance)
        {
            var vnormal = bandLocation.Normal;

            var textHAlign = AxisPosition switch
            {
                AxisPosition.Left => HorizontalAlignment.Right,
                AxisPosition.Top => HorizontalAlignment.Center,
                AxisPosition.Right => HorizontalAlignment.Left,
                AxisPosition.Bottom => HorizontalAlignment.Center,
                _ => throw new NotImplementedException(),
            };

            var textVAlign = AxisPosition switch
            {
                AxisPosition.Left => VerticalAlignment.Middle,
                AxisPosition.Top => VerticalAlignment.Bottom,
                AxisPosition.Right => VerticalAlignment.Middle,
                AxisPosition.Bottom => VerticalAlignment.Top,
                _ => throw new NotImplementedException(),
            };

            var v0 = tickStyle == TickStyle.Crossing || tickStyle == TickStyle.Outside ? vnormal * tickLength : new ScreenVector(0, 0);
            var v1 = tickStyle == TickStyle.Crossing || tickStyle == TickStyle.Inside ? vnormal * -tickLength : new ScreenVector(0, 0);
            var vt = vnormal * (AxisTickToLabelDistance + tickLength);

            var pen = new OxyPen(color, strokeThickness);

            foreach (var tick in ticks)
            {
                if (this.AxisScreenTransformation.IsDiscontinuous(tick.Value, tick.Value))
                    continue;

                var s = this.Transform(tick.Value, bandLocation);

                var s0 = s + v0;
                var s1 = s + v1;

                renderContext.DrawLine(s0.X, s0.Y, s1.X, s1.Y, pen, EdgeRenderingMode.Automatic);

                if (!string.IsNullOrWhiteSpace(tick.Label))
                {
                    var st = s + vt;

                    renderContext.DrawText(st, tick.Label, labelColor, labelFont, labelFontSize, labelFontWeight, labelAngle, textHAlign, textVAlign);
                }
            }
        }

        /// <inheritdoc/>
        public BandExcesses MeasureTicks(IRenderContext renderContext, BandLocation zeroReferenceBandLocation, IReadOnlyList<Tick<TData>> ticks, TickStyle tickStyle, double tickLength, double strokeThickness, OxyColor color, string labelFont, double labelFontSize, double labelFontWeight, OxyColor labelColor, double labelAngle, double AxisTickToLabelDistance)
        {
            // TODO: should consider e.g. band angle

            var top = tickStyle == TickStyle.Crossing || tickStyle == TickStyle.Outside ? tickLength : 0;
            var bottom = tickStyle == TickStyle.Crossing || tickStyle == TickStyle.Inside ? tickLength : 0;
            var left = 0.0;
            var right = 0.0;

            var vnormal = AxisPosition switch
            {
                AxisPosition.Left => new ScreenVector(-1, 0),
                AxisPosition.Top => new ScreenVector(0, -1),
                AxisPosition.Right => new ScreenVector(1, 0),
                AxisPosition.Bottom => new ScreenVector(0, 1),
                _ => throw new NotImplementedException(),
            };

            var textHAlign = AxisPosition switch
            {
                AxisPosition.Left => HorizontalAlignment.Right,
                AxisPosition.Top => HorizontalAlignment.Center,
                AxisPosition.Right => HorizontalAlignment.Left,
                AxisPosition.Bottom => HorizontalAlignment.Center,
                _ => throw new NotImplementedException(),
            };

            var textVAlign = AxisPosition switch
            {
                AxisPosition.Left => VerticalAlignment.Middle,
                AxisPosition.Top => VerticalAlignment.Bottom,
                AxisPosition.Right => VerticalAlignment.Middle,
                AxisPosition.Bottom => VerticalAlignment.Top,
                _ => throw new NotImplementedException(),
            };

            var vt = vnormal * (AxisTickToLabelDistance + tickLength);

            foreach (var tick in ticks)
            {
                if (!string.IsNullOrWhiteSpace(tick.Label))
                {
                    var s = this.Transform(tick.Value, zeroReferenceBandLocation);
                    var st = s + vt;

                    var labelSize = renderContext.MeasureText(tick.Label, labelFont, labelFontSize, labelFontWeight, labelAngle);

                    if (IsVertical)
                    {
                        top = Math.Max(top, labelSize.Width + AxisTickToLabelDistance + tickLength);
                        left = Math.Max(left, labelSize.Height / 2);
                        right = Math.Max(right, labelSize.Height / 2);
                    }
                    else
                    {
                        top = Math.Max(top, labelSize.Height + AxisTickToLabelDistance + tickLength);
                        left = Math.Max(left, labelSize.Width / 2);
                        right = Math.Max(right, labelSize.Width / 2);
                    }
                }
            }

            // TODO: this probably doesn't work for reversed axes

            return new BandExcesses(left, top, right, bottom);
        }

        /// <inheritdoc/>
        public void RenderColorRangeTicks(IRenderContext renderContext, BandLocation bandLocation, IReadOnlyList<ColorRangeTick<TData>> ticks, TickStyle tickStyle, double barWidth, OxyColor lowColor, OxyColor highColor, double highLowExcess)
        {
            var vUnitParallel = bandLocation.Parallel;
            vUnitParallel.Normalize();
            var vNomal = bandLocation.Normal;

            var v0 = tickStyle == TickStyle.Crossing || tickStyle == TickStyle.Outside ? vNomal * barWidth : new ScreenVector(0, 0);
            var v1 = tickStyle == TickStyle.Crossing || tickStyle == TickStyle.Inside ? vNomal * -barWidth : new ScreenVector(0, 0);

            ScreenPoint smin, smax;

            var points = new ScreenPoint[4];

            // main spread
            foreach (var tick in ticks)
            {
                if (this.AxisScreenTransformation.IsDiscontinuous(tick.Minimum, tick.Maximum))
                    continue; // TODO: should probably do something more extreme...

                // TODO: should clamp these (add a TransformClamped method, that clamps to the band width)
                smin = this.Transform(tick.Minimum, bandLocation);
                smax = this.Transform(tick.Maximum, bandLocation);

                points[0] = smin + v0;
                points[1] = smin + v1;
                points[2] = smax + v1;
                points[3] = smax + v0;

                renderContext.DrawPolygon(points, tick.Color, OxyColors.Transparent, 0, EdgeRenderingMode.Automatic);
            }

            bool swapHighLow = AxisScreenTransformation.Transform(AxisScreenTransformation.ClipMinimum).Value > AxisScreenTransformation.Transform(AxisScreenTransformation.ClipMaximum).Value;

            // low
            smin = bandLocation.Reference - vUnitParallel * highLowExcess;
            smax = bandLocation.Reference;

            points[0] = smin + v0;
            points[1] = smin + v1;
            points[2] = smax + v1;
            points[3] = smax + v0;

            renderContext.DrawPolygon(points, swapHighLow ? highColor : lowColor, OxyColors.Transparent, 0, EdgeRenderingMode.Automatic);

            // high
            smin = bandLocation.Reference + bandLocation.Parallel;
            smax = bandLocation.Reference + bandLocation.Parallel + vUnitParallel * highLowExcess;

            points[0] = smin + v0;
            points[1] = smin + v1;
            points[2] = smax + v1;
            points[3] = smax + v0;

            renderContext.DrawPolygon(points, swapHighLow ? lowColor : highColor, OxyColors.Transparent, 0, EdgeRenderingMode.Automatic);
        }

        /// <inheritdoc/>
        public BandExcesses MeasureColorRangeTicks(IRenderContext renderContext, BandLocation bandLocation, IReadOnlyList<ColorRangeTick<TData>> ticks, TickStyle tickStyle, double barWidth, OxyColor lowColor, OxyColor highColor, double highLowExcess)
        {
            return new BandExcesses(highLowExcess, barWidth, highLowExcess, 0.0);
        }
    }

    /// <summary>
    /// Prepares instances of <see cref="ITickRenderHelper{TData}"/>.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public static class TickRenderHelperPreparer<TData>
    {
        private class Generator : IAxisScreenTransformationConsumer<TData>
        {
            public Generator(AxisPosition axisPosition, ScreenReal theOtherCoordinate)
            {
                AxisPosition = axisPosition;
                TheOtherCoordinate = theOtherCoordinate;
            }

            public void Consume<TDataProvider, TAxisScreenTransformation>(TAxisScreenTransformation transformation)
                where TDataProvider : IDataProvider<TData>
                where TAxisScreenTransformation : IAxisScreenTransformation<TData, TDataProvider>
            {
                Result = new HorizontalVerticalAxisRenderHelper<TData, TDataProvider, TAxisScreenTransformation>(transformation, AxisPosition, TheOtherCoordinate);
            }

            public ITickRenderHelper<TData> Result { get; private set; }
            public AxisPosition AxisPosition { get; }
            public ScreenReal TheOtherCoordinate { get; }
        }

        /// <summary>
        /// Prepares a <see cref="ITickRenderHelper{TData}"/> for a horizontal or vertical axis.
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static ITickRenderHelper<TData> PrepareHorizontalVertial(IAxis<TData> axis)
        {
            var s = axis.Position switch
            {
                AxisPosition.Left => axis.ScreenMin.X,
                AxisPosition.Top => axis.ScreenMin.Y,
                AxisPosition.Right => axis.ScreenMax.X,
                AxisPosition.Bottom => axis.ScreenMax.Y,
                _ => throw new NotImplementedException(),
            };

            var generator = new Generator(axis.Position, new ScreenReal(s));
            axis.ConsumeTransformation(generator);
            return generator.Result;
        }
    }
}
