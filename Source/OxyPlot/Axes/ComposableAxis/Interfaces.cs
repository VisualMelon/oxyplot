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
    }

    /// <summary>
    /// Maps samples to XYZ samples
    /// </summary>
    /// <typeparam name="TSample"></typeparam>
    /// <typeparam name="XData"></typeparam>
    /// <typeparam name="YData"></typeparam>
    /// <typeparam name="ZData"></typeparam>
    public interface IXYZSampleProvider<TSample, XData, YData, ZData>
    {
        /// <summary>
        /// Extracts a <see cref="DataSample{XData, YData, ZData}"/> from a <typeparamref name="TSample"/>.
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        DataSample<XData, YData, ZData> Sample(TSample sample);
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
    /// Provides methods to transform between Data space and Screen space along an axis
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TDataProvider"></typeparam>
    public interface IAxisScreenTransformation<TData, TDataProvider> where TDataProvider : IDataProvider<TData>
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
        /// Determines whether there is a discontenuity between the two values.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns><c>true</c> if there is a discontenuity between the two values.</returns>
        bool IsDiscontinuous(TData a, TData b);
    }

    // TODO: to be exposed by Axis (so it probably shouldn't expose the interaction information)
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
        /// <param name="spacingOptions"></param>
        /// <param name="ticks"></param>
        /// <returns></returns>
        void GetTicks(TData minium, TData maximum, ISpacingOptions<TData> spacingOptions, IList<Tick<TData>> ticks);
    }

    /// <summary>
    /// Provides 
    /// </summary>
    public interface ISpacingOptions<TData>
    {
        /// <summary>
        /// Gets the maximum allowed number of ticks.
        /// </summary>
        public int MaximumTickCount { get; }

        /// <summary>
        /// Gets the minimum allowed number of ticks.
        /// </summary>
        public int MinimumTickCount { get; }

        /// <summary>
        /// Gets the maximum allowed step.
        /// </summary>
        public TData MaximumStep { get; }

        /// <summary>
        /// Gets the minimum allowed step.
        /// </summary>
        public TData MinimumStep { get; }

        /// <summary>
        /// Gets the minimum allowed number of steps.
        /// </summary>
        public int MinimumCount { get; }
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
