using System;
using System.Collections.Generic;
using System.Text;

namespace OxyPlot.Axes.ComposableAxes
{
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
    /// Provides methods to project from Data space to Interaction space
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public interface IDataProjection<TData>
    {
        /// <summary>
        /// Gets a value indicating whether the projection is continous.
        /// </summary>
        bool IsSemiContinuous { get; }

        /// <summary>
        /// Projects a value in Data space to Interaction space.
        /// </summary>
        /// <param name="data">The value in Data space.</param>
        /// <returns>A value in Interaction space.</returns>
        double Project(TData data);

        /// <summary>
        /// Projects a value in Interaction space to Data space.
        /// </summary>
        /// <param name="x">The value in Interaction space.</param>
        /// <returns>A value in Data space.</returns>
        TData InverseProject(double x);

        /// <summary>
        /// Projects a value in Data space to Interaction space.
        /// </summary>
        /// <param name="data">The value in Data space.</param>
        /// <param name="breaks">The breaks.</param>
        /// <returns>A value in Interaction space.</returns>
        void LocateBreaks(TData data, IList<double> breaks);

        /// <summary>
        /// Determines whether a value is between the minimum and maximum values.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <param name="min">The inclusive low bound.</param>
        /// <param name="max">The inclusive upper bound.</param>
        /// <returns></returns>
        bool IsBetween(TData value, TData min, TData max);
    }

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
        /// 
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
    /// Providers 
    /// </summary>
    public interface ISpacingOptions<TData>
    {
        /// <summary>
        /// Gets the minimum allowed step.
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
}
