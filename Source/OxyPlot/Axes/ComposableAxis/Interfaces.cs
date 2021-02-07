using System;
using System.Collections.Generic;
using System.Text;

namespace OxyPlot.Axes.ComposableAxis
{
    /// <summary>
    /// Maps samples to XY samples
    /// </summary>
    /// <typeparam name="TSample"></typeparam>
    /// <typeparam name="XData"></typeparam>
    /// <typeparam name="YData"></typeparam>
    public interface IXYSampleProvider<TSample, XData, YData>
    {
        /// <summary>
        /// Extracts a <see cref="DataSample{XData, YData}"/> from a <typeparamref name="TSample"/>.
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        DataSample<XData, YData> Sample(TSample sample);

        /// <summary>
        /// Trys to extract a sample from a sample.
        /// </summary>
        /// <param name="sample"></param>
        /// <param name="result"></param>
        /// <returns><c>true</c> if the sample was valid.</returns>
        bool TrySample(TSample sample, out DataSample<XData, YData> result);

        /// <summary>
        /// Determines whether the given sample is an invalid sample.
        /// </summary>
        /// <param name="sample"></param>
        /// <returns><c>true</c> if the sample is valid, otherwise <c>false</c>.</returns>
        bool IsInvalid(TSample sample);
    }

    /// <summary>
    /// Maps samples to values.
    /// </summary>
    /// <typeparam name="TSample"></typeparam>
    /// <typeparam name="VData"></typeparam>
    public interface IValueProvider<TSample, VData>
    {
        /// <summary>
        /// Extracts a <typeparamref name="VData"/> from a <typeparamref name="TSample"/>.
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        VData Sample(TSample sample);
    }

    /// <summary>
    /// A <see cref="IValueProvider{TSample, VData}"/> that just returns a constant.
    /// </summary>
    /// <typeparam name="TSample"></typeparam>
    /// <typeparam name="VData"></typeparam>
    public readonly struct ConstantProvider<TSample, VData> : IValueProvider<TSample, VData>
    {
        /// <summary>
        /// The constant.
        /// </summary>
        public readonly VData Constant;

        /// <summary>
        /// Initialises a new instance of the <see cref="ConstantProvider{TSample, VData}"/> struct.
        /// </summary>
        /// <param name="constant"></param>
        public ConstantProvider(VData constant)
        {
            Constant = constant;
        }

        /// <inheritdoc/>
        public VData Sample(TSample sample)
        {
            return Constant;
        }
    }

    /// <summary>
    /// Represents something with shared view state.
    /// </summary>
    public interface ISharedViewState<TData>
    {
        /// <summary>
        /// Invalidates the state in this object and its dependencies.
        /// </summary>
        void Invalidate();

        /// <summary>
        /// Refreshes the state in this object and its dependencies if they are invalidated.
        /// </summary>
        void Refresh(IViewInformation<TData> data);
    }

    /// <summary>
    /// Provides methods to interact with data
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public interface IDataProvider<TData> : IComparer<TData>, IEqualityComparer<TData>
    {
        /// <summary>
        /// Gets a value indicating whether the values in the data space are discrete.
        /// </summary>
        bool IsDiscrete { get; }

        /// <summary>
        /// Computes v0 * (1 - c) + v1 * (c)
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        TData Interpolate(TData v0, TData v1, double c);

        /// <summary>
        /// Computes c, such that v0 * (1 - c) + v1 * (c) = v
        /// Equivalently, computes c = (v - v0) / (v1 - v0)
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        double Deinterpolate(TData v0, TData v1, TData v);

        /// <summary>
        /// Determines whether <paramref name="v"/> lies within the bounds defined by <paramref name="min"/> and <paramref name="max"/>.
        /// </summary>
        /// <param name="min">The minimum bound.</param>
        /// <param name="max">The maximum bound</param>
        /// <param name="v"></param>
        /// <returns><c>true</c> if min &lt;= v and v &lt;= max, otherwise <c>false</c>.</returns>
        /// <remarks>This is only included for performance reasons: it must be equivalent to using the implementation of <see cref="IComparer{TData}"/> for comparison.</remarks>
        bool Includes(TData min, TData max, TData v);
    }

    /// <summary>
    /// Provides methods to transform between Data space and Interaction space
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TDataProvider"></typeparam>
    public interface IDataTransformation<TData, TDataProvider> where TDataProvider : IDataProvider<TData>
    {
        /// <summary>
        /// Gets the <see cref="IDataPointProvider"/> for <typeparamref name="TData"/>.
        /// </summary>
        public TDataProvider Provider { get; }

        /// <summary>
        /// Gets a value indicating whether the transformation is non-discontinuous.
        /// </summary>
        bool IsNonDiscontinuous { get; }

        /// <summary>
        /// Gets a value indicating whether the transformation is linear.
        /// </summary>
        bool IsLinear { get; }

        /// <summary>
        /// Transforms a value in Data space to Interaction space.
        /// </summary>
        /// <param name="data">The value in Data space.</param>
        /// <returns>A value in Interaction space.</returns>
        InteractionReal Transform(TData data);

        /// <summary>
        /// Transforms a value in Interaction space to Data space.
        /// </summary>
        /// <param name="x">The value in Interaction space.</param>
        /// <returns>A value in Data space.</returns>
        TData InverseTransform(InteractionReal x);

        /// <summary>
        /// Determines whether there is a discontenuity between the two values.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns><c>true</c> if there is a discontenuity between the two values.</returns>
        bool IsDiscontinuous(TData a, TData b);
    }

    /// <summary>
    /// Provides methods to transform data samples into screen points.
    /// </summary>
    /// <typeparam name="XData"></typeparam>
    /// <typeparam name="YData"></typeparam>
    public interface IXYAxisTransformation<XData, YData>
    {
        // NOTE NOTE: by using this abstraction, we make it easier to implement e.g. triangle axes later on, because this does the job of interpreting the screen space values

        /// <summary>
        /// Transforms the given sample into screen space.
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        ScreenPoint Transform(DataSample<XData, YData> sample);

        /// <summary>
        /// Transforms the given screen point into data space.
        /// </summary>
        /// <param name="screenPoint"></param>
        /// <returns></returns>
        DataSample<XData, YData> InverseTransform(ScreenPoint screenPoint);

        /// <summary>
        /// Determines whether the given sample is within the clip bounds.
        /// </summary>
        bool WithinClipBounds(DataSample<XData, YData> sample);
    }

    /// <summary>
    /// Provides methods to transform data samples into screen points, and more.
    /// </summary>
    /// <typeparam name="XData"></typeparam>
    /// <typeparam name="YData"></typeparam>
    /// <typeparam name="XDataProvider"></typeparam>
    /// <typeparam name="YDataProvider"></typeparam>
    /// <typeparam name="XAxisTransformation"></typeparam>
    /// <typeparam name="YAxisTransformation"></typeparam>
    public interface IXYAxisTransformation<XData, YData, XDataProvider, YDataProvider, XAxisTransformation, YAxisTransformation> : IXYAxisTransformation<XData, YData>
        where XDataProvider : IDataProvider<XData>
        where YDataProvider : IDataProvider<YData>
        where XAxisTransformation : IAxisScreenTransformation<XData, XDataProvider>
        where YAxisTransformation : IAxisScreenTransformation<YData, YDataProvider>
    {
        /// <summary>
        /// The x transformation.
        /// </summary>
        XAxisTransformation XTransformation { get; }

        /// <summary>
        /// The y transformation.
        /// </summary>
        YAxisTransformation YTransformation { get; }

        /// <summary>
        /// Arranges the screen coordinate into a screen point.
        /// </summary>
        ScreenPoint Arrange(ScreenReal x, ScreenReal y);

        /// <summary>
        /// Extracts screen coordaintes from a screen point.
        /// </summary>
        void InverseArrange(ScreenPoint point, out ScreenReal x, out ScreenReal y);
    }

    /// <summary>
    /// Provides methods to transform between values and colors.
    /// </summary>
    /// <typeparam name="TData">The type of Data space</typeparam>
    public interface IAxisColorTransformation<TData>
    {
        /// <summary>
        /// Transforms a value to a color.
        /// </summary>
        /// <param name="data">The value.</param>
        /// <returns>A <see cref="OxyColor"/>.</returns>
        OxyColor Transform(TData data);

        /// <summary>
        /// Determines whether the given value should be presented on the axis.
        /// </summary>
        /// <param name="data">The value to test</param>
        /// <returns><c>true</c> if the value should be presented, otherwise <c>false</c>.</returns>
        bool Filter(TData data);
    }

    /// <summary>
    /// Provides methods to transform between Data space and Screen space along an axis
    /// </summary>
    /// <typeparam name="TData">The type of Data space</typeparam>
    public interface IAxisScreenTransformation<TData>
    {
        /// <summary>
        /// Gets a value indicating whether the transformation is non-discontinuous.
        /// </summary>
        bool IsNonDiscontinuous { get; }

        /// <summary>
        /// Gets a value indicating whether the transformation is linear.
        /// </summary>
        bool IsLinear { get; }

        /// <summary>
        /// Transforms a value in Data space to Screen space.
        /// </summary>
        /// <param name="data">The value in Data space.</param>
        /// <returns>A value in Screen space.</returns>
        ScreenReal Transform(TData data);

        /// <summary>
        /// Transforms a value in Screen space to Data space.
        /// </summary>
        /// <param name="s">The value in Screen space.</param>
        /// <returns>A value in Data space.</returns>
        TData InverseTransform(ScreenReal s);

        /// <summary>
        /// Determines whether the given value should be presented on the axis.
        /// </summary>
        /// <param name="data">The value to test</param>
        /// <returns><c>true</c> if the value should be presented, otherwise <c>false</c>.</returns>
        bool Filter(TData data);

        /// <summary>
        /// Determines whether there is a discontenuity between the two values.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns><c>true</c> if there is a discontenuity between the two values.</returns>
        bool IsDiscontinuous(TData a, TData b);

        /// <summary>
        /// Gets the minimum clip value.
        /// </summary>
        public TData ClipMinimum { get; }

        /// <summary>
        /// Gets the maximum clip value.
        /// </summary>
        public TData ClipMaximum { get; }

        /// <summary>
        /// Determines whether the value <paramref name="v"/> is within the clip bounds.
        /// </summary>
        /// <param name="v"></param>
        /// <returns><c>true</c> if the value is within the axis clip bounds.</returns>
        public bool WithinClipBounds(TData v);
    }

    /// <summary>
    /// Provides methods to transform between values and colors.
    /// </summary>
    /// <typeparam name="TData">The type of Data space</typeparam>
    /// <typeparam name="TDataProvider">The type of Data space</typeparam>
    public interface IAxisColorTransformation<TData, TDataProvider> : IAxisColorTransformation<TData>
        where TDataProvider : IDataProvider<TData>
    {
        /// <summary>
        /// Gets the <see cref="IDataPointProvider"/> for <typeparamref name="TData"/>.
        /// </summary>
        public TDataProvider Provider { get; }
    }

    /// <summary>
    /// Provides methods to transform between Data space and Screen space along an axis
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TDataProvider"></typeparam>
    public interface IAxisScreenTransformation<TData, TDataProvider> : IAxisScreenTransformation<TData>
        where TDataProvider : IDataProvider<TData>
    {
        /// <summary>
        /// Gets the <see cref="IDataPointProvider"/> for <typeparamref name="TData"/>.
        /// </summary>
        public TDataProvider Provider { get; }
    }

    // TODO: to be exposed by Axis
    /// <summary>
    /// Represents a view over some data.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public interface IViewInformation<TData>
    {
        /// <summary>
        /// The actual maximum value.
        /// </summary>
        public TData ActualMaximum { get; set; }

        /// <summary>
        /// The actual minimum value.
        /// </summary>
        public TData ActualMinimum { get; set; }

        /// <summary>
        /// The clip maximum value.
        /// </summary>
        public TData ClipMaximum { get; set; }

        /// <summary>
        /// The clip minimum value.
        /// </summary>
        public TData ClipMinimum { get; set; }

        // TOOD: we do need to expose these (somewhere) so that interaction code can use them: do we need seperate actual/clip? (nobody cares about Actual except for the code that computes Clip, right?)
        /// <summary>
        /// The interaction maximum value.
        /// </summary>
        public double InteractionMaximum { get; set; }

        /// <summary>
        /// The interaction minimum value.
        /// </summary>
        public double InteractionMinimum { get; set; }
    }

    /// <summary>
    /// A collection of ticks.
    /// </summary>
    public interface ITicks<TData> : ISharedViewState<TData>
    {
        /// <summary>
        /// Gets the list of ticks.
        /// </summary>
        IList<Tick<TData>> ActualTicks { get; }
    }

    /// <summary>
    /// Provides methods to locate ticks and gridlines on an axis
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public interface ITickLocator<TData>
    {
        /// <summary>
        /// Generates ticks between the given minimum and maximum data values.
        /// </summary>
        /// <param name="minium"></param>
        /// <param name="maximum"></param>
        /// <param name="availableWidth"></param>
        /// <param name="spacingOptions"></param>
        /// <param name="majorTicks"></param>
        /// <param name="minorTicks"></param>
        /// <returns></returns>
        void GetTicks(TData minium, TData maximum, double availableWidth, SpacingOptions spacingOptions, IList<Tick<TData>> majorTicks, IList<Tick<TData>> minorTicks);
    }

    /// <summary>
    /// Provides methods to locate ticks and gridlines on an axis
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public interface IRangeTickLocator<TData>
    {
        /// <summary>
        /// Generates range ticks between the given minimum and maximum data values.
        /// </summary>
        /// <param name="minium"></param>
        /// <param name="maximum"></param>
        /// <param name="availableWidth"></param>
        /// <param name="spacingOptions"></param>
        /// <param name="ticks"></param>
        /// <returns></returns>
        void GetTicks(TData minium, TData maximum, double availableWidth, SpacingOptions spacingOptions, IList<RangeTick<TData>> ticks);
    }

    /// <summary>
    /// Provides for an optional over a given value.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TOptional"></typeparam>
    public interface IOptionalProvider<TValue, TOptional>
    {
        /// <summary>
        /// Gets a value indiciating whether the given value is set.
        /// </summary>
        /// <param name="optional"></param>
        /// <returns><c>true</c> if the optional is not none.</returns>
        bool HasValue(TOptional optional);

        /// <summary>
        /// Gets a value indiciating whether the given value is set.
        /// Sets <paramref name="value"/> if it is.
        /// </summary>
        /// <param name="optional"></param>
        /// <param name="value"></param>
        /// <returns><c>true</c> if the optional is not none.</returns>
        bool TryGetValue(TOptional optional, out TValue value);

        /// <summary>
        /// Gets a value that represents a 'none' option.
        /// </summary>
        TOptional None { get; }

        /// <summary>
        /// Gets an optional that wraps a value.
        /// </summary>
        TOptional Some(TValue value);
    }
}
