﻿using System;
using System.Collections.Generic;
using System.Text;

namespace OxyPlot.Axes.ComposableAxis
{
    /// <summary>
    /// For some reason axes render in passes: these need to be well defined
    /// </summary>
    public enum AxisRenderPass : int
    {
        /// <summary>
        /// TODO: Not sure what this is yet
        /// Seems to be for minor items
        /// </summary>
        Pass0 = 0,

        /// <summary>
        /// TODO: Not sure what this is yet either
        /// Seems to be for major items and titles and sich
        /// </summary>
        Pass1 = 1
    }

    /// <summary>
    /// Consumes an axis-screen transformation.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public interface IAxisScreenTransformationConsumer<TData>
    {
        /// <summary>
        /// Consumes an axis-screen transformation.
        /// </summary>
        /// <typeparam name="TDataProvider"></typeparam>
        /// <typeparam name="TAxisScreenTransformation"></typeparam>
        /// <param name="transformation"></param>
        void Consume<TDataProvider, TAxisScreenTransformation>(TAxisScreenTransformation transformation)
            where TDataProvider : IDataProvider<TData>
            where TAxisScreenTransformation : IAxisScreenTransformation<TData, TDataProvider>;
    }

    /// <summary>
    /// Consumes two axis-screen transformations.
    /// </summary>
    /// <typeparam name="XData"></typeparam>
    /// <typeparam name="YData"></typeparam>
    public interface IXYAxisScreenTransformationConsumer<XData, YData>
    {
        /// <summary>
        /// Consumes two axis-screen transformations.
        /// </summary>
        /// <typeparam name="XDataProvider"></typeparam>
        /// <typeparam name="YDataProvider"></typeparam>
        /// <typeparam name="XAxisScreenTransformation"></typeparam>
        /// <typeparam name="YAxisScreenTransformation"></typeparam>
        /// <param name="x"></param>
        /// <param name="y"></param>
        void Consume<XDataProvider, YDataProvider, XAxisScreenTransformation, YAxisScreenTransformation>(XAxisScreenTransformation x, YAxisScreenTransformation y)
            where XDataProvider : IDataProvider<XData>
            where YDataProvider : IDataProvider<YData>
            where XAxisScreenTransformation : IAxisScreenTransformation<XData, XDataProvider>
            where YAxisScreenTransformation : IAxisScreenTransformation<YData, YDataProvider>;
    }

    /// <summary>
    /// Consumes two axis-screen transformations.
    /// </summary>
    /// <typeparam name="XData"></typeparam>
    /// <typeparam name="YData"></typeparam>
    public interface IXYAxisProviderConsumer<XData, YData>
    {
        /// <summary>
        /// Consumes two axis-screen transformations.
        /// </summary>
        /// <typeparam name="XDataProvider"></typeparam>
        /// <typeparam name="YDataProvider"></typeparam>
        /// <param name="x"></param>
        /// <param name="y"></param>
        void Consume<XDataProvider, YDataProvider>(XDataProvider x, YDataProvider y)
            where XDataProvider : IDataProvider<XData>
            where YDataProvider : IDataProvider<YData>;
    }

    /// <summary>
    /// Consumes three axis-screen transformations.
    /// </summary>
    /// <typeparam name="XData"></typeparam>
    /// <typeparam name="YData"></typeparam>
    /// <typeparam name="ZData"></typeparam>
    public interface IXYZAxisScreenTransformationConsumer<XData, YData, ZData>
    {
        /// <summary>
        /// Consumes three axis-screen transformations.
        /// </summary>
        /// <typeparam name="XDataProvider"></typeparam>
        /// <typeparam name="YDataProvider"></typeparam>
        /// <typeparam name="ZDataProvider"></typeparam>
        /// <typeparam name="XAxisScreenTransformation"></typeparam>
        /// <typeparam name="YAxisScreenTransformation"></typeparam>
        /// <typeparam name="ZAxisScreenTransformation"></typeparam>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        void Consume<XDataProvider, YDataProvider, ZDataProvider, XAxisScreenTransformation, YAxisScreenTransformation, ZAxisScreenTransformation>(XAxisScreenTransformation x, YAxisScreenTransformation y, ZAxisScreenTransformation z)
            where XDataProvider : IDataProvider<XData>
            where YDataProvider : IDataProvider<YData>
            where ZDataProvider : IDataProvider<ZData>
            where XAxisScreenTransformation : IAxisScreenTransformation<XData, XDataProvider>
            where YAxisScreenTransformation : IAxisScreenTransformation<YData, YDataProvider>
            where ZAxisScreenTransformation : IAxisScreenTransformation<ZData, ZDataProvider>;
    }

    // TODO: will need seperate interfaces for color axis consumers?
    // -> yes, because this is the context in which all series will render (and who knows what else)

    /// <summary>
    /// TODO: work out where to put this stuff...
    /// It should be in AxisBase, probably, but for now it will have to sit in the generic axis.
    /// </summary>
    public interface IComposableAxisStuffThatIsCurrentlyCausingProblemsWithRespectToCompatabilityWithExistingAxisImplementsAndAsSuchCannotAppearInIAxisAtThisTime
    {
        /// <summary>
        /// Gets the View information.
        /// </summary>
        ViewInfo ViewInfo { get; }

        /// <summary>
        /// Zooms the axis at the specified point thing thing point thing point.
        /// </summary>
        /// <param name="staticPoint">The screen point to zoom at.</param>
        /// <param name="factor">The zoom factor.</param>
        void ZoomAt(ScreenPoint staticPoint, double factor);
    }

    /// <summary>
    /// Base class for axes.
    /// </summary>
    public abstract class AxisBase : PlotElement, IAxis
    {
        /// <summary>
        /// Initializes an instance of the <see cref="AxisBase"/> class.
        /// </summary>
        protected AxisBase()
        {
            // defaults
            // TODO: C&P from Axis.cs, and remove anything that doesn't exist
            this.PositionAtZeroCrossing = false;

            this.MinimumPadding = 0.01;
            this.MaximumPadding = 0.01;
            this.MinimumDataMargin = ScreenReal.Zero;
            this.MaximumDataMargin = ScreenReal.Zero;
            this.MinimumMargin = ScreenReal.Zero;
            this.MaximumMargin = ScreenReal.Zero;
        }

        #region Things I definitely want to purge
        /// <summary>
        /// Determines whether this is an X/T axis.
        /// </summary>
        /// <returns></returns>
        public abstract bool IsXyAxis();

        /// <summary>
        /// Gets or sets a value indicating whether the axis should be positioned at the zero-crossing of the related axis. The default value is <c>false</c>.
        /// </summary>
        public bool PositionAtZeroCrossing { get; set; }

        /// <summary>
        /// The scale between Interaction Space and View Space: only here for compat.
        /// <see cref="ViewInfo"/> is the new guy, at the moment.
        /// </summary>
        public abstract double Scale { get; } // used for cartesian enforcement

        /// <summary>
        /// The offset between Interaction Space and View Space: only here for compat.
        /// <see cref="ViewInfo"/> is the new guy, at the moment.
        /// </summary>
        public abstract double Offset { get; } // the other half of the ViewInfo from Scale
        #endregion

        /// <summary>
        /// The Scale and Offset, with some meaningful behaviour bolted on for good measure.
        /// </summary>
        public abstract ViewInfo ViewInfo { get; }

        /// <summary>
        /// Gets or sets the key of the axis. This can be used to specify an axis if you have defined multiple axes in a plot. The default value is <c>null</c>.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the layer of the axis. The default value is <see cref="AxisLayer.BelowSeries"/>.
        /// </summary>
        public AxisLayer Layer { get; set; }

        /// <summary>
        /// Gets or sets the position of the axis. The default value is <see cref="AxisPosition.Left"/>.
        /// </summary>
        public AxisPosition Position { get; set; }

        /// <summary>
        /// Gets or sets the screen coordinate of the maximum end of the axis.
        /// </summary>
        public ScreenPoint ScreenMax { get; protected set; }

        /// <summary>
        /// Gets or sets the screen coordinate of the minimum end of the axis.
        /// </summary>
        public ScreenPoint ScreenMin { get; protected set; }

        /// <summary>
        /// The clip minimum, in Interaction space. Maps to the ActualMinimum in data space.
        /// </summary>
        public InteractionReal ClipInteractionMinimum { get; set; }

        /// <summary>
        /// The clip maximum, in Interaction space. Maps to the ActualMaximum in data space.
        /// </summary>
        public InteractionReal ClipInteractionMaximum { get; set; }

        /// <summary>
        /// Gets or sets the start position of the axis on the plot area. The default value is <c>0</c>.
        /// </summary>
        /// <remarks>The position is defined by a fraction in the range from <c>0</c> to <c>1</c>, where <c>0</c> is at the bottom/left
        /// and <c>1</c> is at the top/right. </remarks>
        public double StartPosition { get; set; }

        /// <summary>
        /// Gets or sets the end position of the axis on the plot area. The default value is <c>1</c>.
        /// </summary>
        /// <remarks>The position is defined by a fraction in the range from <c>0</c> to <c>1</c>, where <c>0</c> is at the bottom/left
        /// and <c>1</c> is at the top/right. </remarks>
        public double EndPosition { get; set; }

        /// <summary>
        /// Determines whether the axis is horizontal.
        /// </summary>
        /// <returns><c>true</c> if the axis is horizontal; otherwise, <c>false</c> .</returns>
        public bool IsHorizontal() => this.Position == AxisPosition.Top || this.Position == AxisPosition.Bottom;

        /// <summary>
        /// Determines whether the axis is vertical.
        /// </summary>
        /// <returns><c>true</c> if the axis is horizontal; otherwise, <c>false</c> .</returns>
        public bool IsVertical() => this.Position == AxisPosition.Left || this.Position == AxisPosition.Right;

        /// <summary>
        /// Resets the user's modification (zooming/panning) to minimum and maximum of this axis.
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// Zooms the axis with the specified zoom factor at the center of the axis.
        /// </summary>
        /// <param name="factor">The zoom factor.</param>
        public abstract void ZoomAtCenter(double factor);

        /// <summary>
        /// Zoom to the specified scale.
        /// </summary>
        /// <param name="newScale">The new scale.</param>
        public abstract void Zoom(double newScale);

        /// <summary>
        /// Pans the specified axis.
        /// </summary>
        /// <param name="previousPoint">The previous point (screen coordinates).</param>
        /// <param name="newPoint">The current point (screen coordinates).</param>
        public abstract void Pan(ScreenPoint previousPoint, ScreenPoint newPoint);

        /// <summary>
        /// Pans the specified axis.
        /// </summary>
        /// <param name="screenOffsetDelta">How much to move along.</param>
        public abstract void Pan(ScreenReal screenOffsetDelta);

        /// <summary>
        /// Gets or sets a value indicating whether this axis is visible. The default value is <c>true</c>.
        /// </summary>
        public bool IsAxisVisible { get; set; }

        /// <summary>
        /// Gets or sets the desired margins such that the axis text ticks will not be clipped.
        /// The actual margins may be smaller or larger than the desired margins if they are set manually.
        /// </summary>
        public OxyThickness DesiredMargin { get; protected set; }

        /// <summary>
        /// Gets or sets the position tier max shift.
        /// </summary>
        internal double PositionTierMaxShift { get; set; }

        /// <summary>
        /// Gets or sets the position tier min shift.
        /// </summary>
        internal double PositionTierMinShift { get; set; }

        /// <summary>
        /// Gets or sets the size of the position tier.
        /// </summary>
        internal double PositionTierSize { get; set; }

        /// <summary>
        /// Gets or sets the position tier which defines in which tier the axis is displayed. The default value is <c>0</c>.
        /// </summary>
        /// <remarks>The bigger the value the further afar is the axis from the graph.</remarks>
        public int PositionTier { get; set; }

        /// <summary>
        /// Gets or sets the 'padding' fraction of the minimum value. The default value is <c>0.01</c>.
        /// </summary>
        /// <remarks>A value of 0.01 gives 1% more space on the minimum end of the axis. This property is not used if an explicit minimum is set.</remarks>
        public double MinimumPadding { get; set; }

        /// <summary>
        /// Gets or sets the screen-space data margin at the minimum. The default value is <c>0</c>.
        /// </summary>
        /// <value>The number of device independent units to included between the clip and actual minima.</value>
        public ScreenReal MinimumDataMargin { get; set; }

        /// <summary>
        /// Gets or sets the screen-space margin at the minimum. The default value is <c>0</c>.
        /// </summary>
        /// <value>The number of device independent units to be left empty between the axis the <see cref="ComposableAxis.AxisBase.StartPosition"/>.</value>
        public ScreenReal MinimumMargin { get; set; }

        /// <summary>
        /// Gets or sets the 'padding' fraction of the maximum value. The default value is <c>0.01</c>.
        /// </summary>
        /// <remarks>A value of 0.01 gives 1% more space on the maximum end of the axis. This property is not used if an explicit maximum is set.</remarks>
        public double MaximumPadding { get; set; }

        /// <summary>
        /// Gets or sets the screen-space data margin at the maximum. The default value is <c>0</c>.
        /// </summary>
        /// <value>The number of device independent units to included between the clip and actual maxima.</value>
        public ScreenReal MaximumDataMargin { get; set; }

        /// <summary>
        /// Gets or sets the screen-space margin at the maximum. The default value is <c>0</c>.
        /// </summary>
        /// <value>The number of device independent units to be left empty between the axis the <see cref="ComposableAxis.AxisBase.StartPosition"/>.</value>
        public ScreenReal MaximumMargin { get; set; }

        /// <summary>
        /// Updates the scale and offset properties of the transform from the specified boundary rectangle.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        internal abstract void UpdateTransform(OxyRect bounds); // what do I do with this one!? Why is everything internal?!

        /// <summary>
        /// Updates the actual minor and major step intervals.
        /// </summary>
        /// <param name="plotArea">The plot area rectangle.</param>
        internal abstract void UpdateIntervals(OxyRect plotArea);

        /// <summary>
        /// Measures the size of the axis and updates <see cref="AxisBase.DesiredMargin"/> accordingly. This takes into account the axis title as well as tick labels
        /// potentially exceeding the axis range.
        /// </summary>
        /// <param name="rc">The render context.</param>
        public abstract void Measure(IRenderContext rc);

        /// <summary>
        /// Renders the axis on the specified render context.
        /// </summary>
        /// <param name="rc">The render context.</param>
        /// <param name="pass">The pass.</param>
        public abstract void Render(IRenderContext rc, AxisRenderPass pass);

        /// <inheritdoc/>
        public abstract void ResetDataMaxMin();

        /// <summary>
        /// Updates the actual minimum and maximum values.
        /// </summary>
        /// <remarks>If the user has zoomed/panned the axis, the internal ViewMaximum/ViewMinimum
        /// values will be used. If Maximum or Minimum have been set, these values will be used. Otherwise the maximum and minimum values
        /// of the series will be used, including the 'padding'.</remarks>
        public abstract void UpdateActualMaxMin();
    }

    /// <summary>
    /// An axis.
    /// </summary>
    public interface IAxis
    {
        /// <summary>
        /// Gets or sets the key of the axis. This can be used to specify an axis if you have defined multiple axes in a plot. The default value is <c>null</c>.
        /// </summary>
        string Key { get; set; }

        /// <summary>
        /// Gets or sets the position of the axis. The default value is <see cref="AxisPosition.Left"/>.
        /// </summary>
        AxisPosition Position { get; set; }

        // TODO: these really should not be here... I think the only generic code that needs them is margin computation, and that's in the axis itself so that should be fine
        /// <summary>
        /// Gets the Start position.
        /// </summary>
        double StartPosition { get; set; }

        /// <summary>
        /// Gets the End position.
        /// </summary>
        double EndPosition { get; set; }

        /// <summary>
        /// Resets the axis view.
        /// </summary>
        void Reset();

        /// <summary>
        /// Zooms by the given factor.
        /// </summary>
        /// <param name="factor"></param>
        void ZoomAtCenter(double factor);

        /// <summary>
        /// Zoom to the specified scale.
        /// </summary>
        /// <param name="newScale">The new scale.</param>
        void Zoom(double newScale);

        /// <summary>
        /// Pans the axis.
        /// </summary>
        /// <param name="previousPoint"></param>
        /// <param name="newPoint"></param>
        void Pan(ScreenPoint previousPoint, ScreenPoint newPoint);

        /// <summary>
        /// Gets a value indicating whether the axis is visible.
        /// </summary>
        bool IsAxisVisible { get; }

        /// <summary>
        /// Resets the data minimum and maximum values.
        /// </summary>
        void ResetDataMaxMin();
    }

    /// <summary>
    /// An axis over a particular data type.
    /// </summary>
    public interface IAxis<TData> : IAxis
    {
        /// <summary>
        /// Zooms to the given range.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        void ZoomTo(TData min, TData max);

        /// <summary>
        /// Consumes a <see cref="IAxisScreenTransformationConsumer{TData}"/>.
        /// </summary>
        /// <param name="consumer"></param>
        void Consume(IAxisScreenTransformationConsumer<TData> consumer);

        /// <summary>
        /// The minimum value that will be rendered.
        /// </summary>
        TData ClipMinimum { get; set; }

        /// <summary>
        /// The maximum value that will be rendered.
        /// </summary>
        TData ClipMaximum { get; set; }

        /// <summary>
        /// The logical minimum value.
        /// </summary>
        TData ActualMinimum { get; set; }

        /// <summary>
        /// The logical maximum value.
        /// </summary>
        TData ActualMaximum { get; set; }
    }

    /// <summary>
    /// An axis.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TDataProvider"></typeparam>
    /// <typeparam name="TDataTransformation"></typeparam>
    /// <typeparam name="TDataOptional"></typeparam>
    /// <typeparam name="TDataOptionalProvider"></typeparam>
    public class HorizontalVerticalAxis<TData, TDataProvider, TDataTransformation, TDataOptional, TDataOptionalProvider> : AxisBase, IAxis<TData>, IComposableAxisStuffThatIsCurrentlyCausingProblemsWithRespectToCompatabilityWithExistingAxisImplementsAndAsSuchCannotAppearInIAxisAtThisTime
        where TDataProvider : IDataProvider<TData>
        where TDataTransformation : IDataTransformation<TData, TDataProvider>
        where TDataOptionalProvider : IOptionalProvider<TData, TDataOptional>
    {
        /// <inheritdoc/>
        public override bool IsXyAxis() => true;

        /// <summary>
        /// The <typeparamref name="TDataTransformation"/>.
        /// </summary>
        public TDataTransformation DataTransformation { get; }

        /// <summary>
        /// Gets the <typeparamref name="TDataProvider"/> from the <typeparamref name="TDataTransformation"/>.
        /// </summary>
        public TDataProvider DataProvider => DataTransformation.Provider;

        /// <summary>
        /// The <typeparamref name="TDataOptionalProvider"/>.
        /// </summary>
        public TDataOptionalProvider OptionalProvider { get; }

        /// <inheritdoc/>
        public TData ClipMinimum { get; set; }

        /// <inheritdoc/>
        public TData ClipMaximum { get; set; }

        /// <inheritdoc/>
        public TData ActualMinimum { get; set; }

        /// <inheritdoc/>
        public TData ActualMaximum { get; set; }

        /// <inheritdoc/>
        public TDataOptional Minimum { get; set; }

        /// <inheritdoc/>
        public TDataOptional Maximum { get; set; }

        /// <summary>
        /// The range of data samples associated with this axis.
        /// </summary>
        public Range<TData> DataRange { get; private set; }

        /// <summary>
        /// The default range presented to the user when there is no better altenative.
        /// </summary>
        public Range<TData> DefaultViewRange { get; set; }

        /// <summary>
        /// Includes a sample in the <see cref="DataRange"/>.
        /// </summary>
        /// <param name="sample"></param>
        public void Include(TData sample)
        {
            DataRange = DataRange.Include(DataProvider, sample);
        }

        /// <inheritdoc/>
        public override void ResetDataMaxMin()
        {
            DataRange = Range<TData>.Empty;
        }

        /// <summary>
        /// Gets a value indicating whether the axis is reversed.
        /// </summary>
        public bool IsReversed => StartPosition > EndPosition;

        private ViewInfo _viewInfo;

        private OxyRect PlotBounds { get; set; }

        /// <summary>
        /// The absolute minimum value to be shown (i.e. the minimum value that <see cref="ClipMinimum"/> will take).
        /// </summary>
        public TData AbsoluteMinimum { get; set; }

        /// <summary>
        /// The absolute maximum value to be shown (i.e. the maximum value that <see cref="ClipMinimum"/> will take).
        /// </summary>
        public TData AbsoluteMaximum { get; set; }

        /// <summary>
        /// The center, in Interaction space.
        /// </summary>
        private InteractionReal ActualInteractionCenter { get; set; }

        /// <summary>
        /// The radius - or half width - in Interaction space.
        /// </summary>
        private InteractionReal ActualInteractionRadius { get; set; }

        /// <summary>
        /// The center, in Interaction space.
        /// </summary>
        private InteractionReal? ViewInteractionCenter { get; set; } = null;

        /// <summary>
        /// The radius - or half width - in Interaction space.
        /// </summary>
        private InteractionReal? ViewInteractionRadius { get; set; } = null;

        /// <summary>
        /// The actual minimum, in Interaction space. Maps to <see cref="ActualMinimum"/>.
        /// </summary>
        private InteractionReal ActualInteractionMinimum => ActualInteractionCenter - ActualInteractionRadius;

        /// <summary>
        /// The actual maximum, in Interaction space. Maps to <see cref="ActualMaximum"/>.
        /// </summary>
        private InteractionReal ActualInteractionMaximum => ActualInteractionCenter + ActualInteractionRadius;

        /// <summary>
        /// The Screen space range.
        /// </summary>
        public Range<ScreenReal> ScreenRange { get; private set; }

        /// <summary>
        /// Resets the view.
        /// </summary>
        public override void Reset()
        {
            ViewInteractionRadius = null;
            ViewInteractionCenter = null;

            RefreshView();
        }

        /// <summary>
        /// Zooms by the given factor.
        /// </summary>
        /// <param name="factor"></param>
        public override void ZoomAtCenter(double factor)
        {
            ViewInteractionRadius = ActualInteractionRadius * factor;
            ViewInteractionCenter = ActualInteractionCenter;

            RefreshView();
        }

        /// <summary>
        /// Zooms by the given factor, maintaining the given screen-space position.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="factor"></param>
        public void ZoomAt(ScreenReal position, double factor)
        {
            var ip = ViewInfo.InverseTransform(position);

            ViewInteractionRadius = ActualInteractionRadius * factor;
            ViewInteractionCenter = (ip - ActualInteractionCenter) * factor - ip;

            RefreshView();
        }

        /// <summary>
        /// Zooms by the given factor, maintaining the given screen-space point.
        /// </summary>
        /// <param name="staticPoint"></param>
        /// <param name="factor"></param>
        public void ZoomAt(ScreenPoint staticPoint, double factor)
        {
            var ip = ViewInfo.InverseTransform(new ScreenReal(IsHorizontal() ? staticPoint.X : staticPoint.Y));

            ViewInteractionRadius = ActualInteractionRadius * factor;
            ViewInteractionCenter = (ip - ActualInteractionCenter) * factor - ip;

            RefreshView();
        }

        /// <summary>
        /// Zooms to the given range.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public void ZoomTo(TData min, TData max)
        {
            var imax = DataTransformation.Transform(min);
            var imin = DataTransformation.Transform(max);

            ViewInteractionCenter = (imax + imin) / 2.0;
            ViewInteractionRadius = (imax - imin) / 2.0;

            RefreshView();
        }

        /// <summary>
        /// Zoom to the specified scale.
        /// </summary>
        /// <param name="newScale">The new scale.</param>
        public override void Zoom(double newScale)
        {
            throw new Exception("No idea what this does. Scale, what is that? I've never heard of it: is that a thing?");
        }

        /// <summary>
        /// Pans by the given factor.
        /// </summary>
        /// <param name="factor"></param>
        public void Pan(double factor)
        {
            ViewInteractionRadius = ActualInteractionRadius;
            ViewInteractionCenter = ActualInteractionCenter + ActualInteractionRadius * factor * 2;

            RefreshView();
        }

        /// <inheritdoc/>
        public override ViewInfo ViewInfo => _viewInfo;

        /// <inheritdoc/>
        public override double Scale => ViewInfo.ScreenScale;

        /// <inheritdoc/>
        public override double Offset => ViewInfo.ScreenOffset.Value;

        /// <inheritdoc/>
        public override void UpdateActualMaxMin()
        {
            // Not sure what this is meant to do... so let's just call RefreshView
            RefreshView();
        }

        /// <summary>
        /// Update the <see cref="ViewInfo"/> and other properties derived from the Actual Interaction Min/Max and the Screen Min/Max.
        /// </summary>
        private void RefreshView()
        {
            if (!ScreenRange.TryGetMinMax(out var screenMin, out var screenMax))
            {
                return; // we cannot
            }

            var screenWidth = screenMin - screenMax;

            if (ViewInteractionCenter.HasValue)
            {
                ActualInteractionCenter = ViewInteractionCenter.Value;
                ActualInteractionRadius = ViewInteractionRadius.Value;
            }
            else
            {
                var fallbackRange = DataRange.IsEmpty ? DefaultViewRange : DataRange;

                if (!fallbackRange.TryGetMinMax(out var dmin, out var dmax))
                    throw new InvalidOperationException("Axis must exist on some range");

                var minimum = OptionalProvider.Unpack(Minimum, dmax);
                var maximum = OptionalProvider.Unpack(Minimum, dmin);

                var imax = DataTransformation.Transform(minimum);
                var imin = DataTransformation.Transform(maximum);

                ActualInteractionCenter = (imax + imin) / 2.0;
                ActualInteractionRadius = (imax - imin) / 2.0;
            }

            if (screenWidth.Value == 0 || ActualInteractionRadius.Value == 0)
            {
                return; // we cannot
            }

            var reversed = IsReversed ? -1.0 : 1.0;

            var actualScreenWidth = screenWidth - MinimumDataMargin - MaximumDataMargin;

            var scale = actualScreenWidth.Value / (ActualInteractionRadius * 2.0).Value * reversed;
            var offset = new ScreenReal(screenMin.Value - (IsReversed ? ClipInteractionMaximum.Value : ClipInteractionMinimum.Value) * scale);

            _viewInfo = new ViewInfo(offset, scale);

            ClipInteractionMinimum = ActualInteractionMinimum - new InteractionReal(MinimumDataMargin.Value * Math.Abs(scale));
            ClipInteractionMinimum = ActualInteractionMinimum + new InteractionReal(MaximumDataMargin.Value * Math.Abs(scale));

            ActualMinimum = DataTransformation.InverseTransform(ActualInteractionMinimum);
            ActualMaximum = DataTransformation.InverseTransform(ActualInteractionMaximum);
            ClipMinimum = DataTransformation.InverseTransform(ClipInteractionMinimum);
            ClipMaximum = DataTransformation.InverseTransform(ClipInteractionMaximum);
        }

        /// <inheritdoc/>
        public void Consume(IAxisScreenTransformationConsumer<TData> consumer)
        {
            consumer.Consume<TDataProvider, AxisScreenTransformation<TData, TDataProvider, TDataTransformation>>(new AxisScreenTransformation<TData, TDataProvider, TDataTransformation>(DataTransformation, ViewInfo, ClipMinimum, ClipMaximum));
        }

        internal override void UpdateIntervals(OxyRect plotArea)
        {
            // the plan is to abstract this away entirely... so let's do nothing for now
        }

        /// <inheritdoc/>
        internal override void UpdateTransform(OxyRect bounds) // WHY INTERNAL WHY
        {
            PlotBounds = bounds;

            ScreenReal screenMinimum;
            ScreenReal screenMaximum;

            if (IsHorizontal())
            {
                screenMinimum = new ScreenReal(IsReversed ? bounds.Left : bounds.Right) + MinimumMargin;
                screenMaximum = new ScreenReal(IsReversed ? bounds.Right : bounds.Left) - MinimumMargin;

                ScreenMin = new ScreenPoint(screenMinimum.Value, bounds.Top);
                ScreenMax = new ScreenPoint(screenMaximum.Value, bounds.Bottom);
            }
            else if (IsVertical())
            {
                screenMinimum = new ScreenReal(IsReversed ? bounds.Bottom : bounds.Top) + MinimumMargin;
                screenMaximum = new ScreenReal(IsReversed ? bounds.Top : bounds.Bottom) - MinimumMargin;

                ScreenMin = new ScreenPoint(bounds.Left, screenMinimum.Value);
                ScreenMax = new ScreenPoint(bounds.Right, screenMaximum.Value);
            }
            else
            {
                // we should not render... so let's bail now and hope for the best
                return;
            }

            ScreenRange = new Range<ScreenReal>(screenMinimum, screenMaximum);

            RefreshView(); // why is this not just here?
        }

        /// <summary>
        /// Measures the size of the axis and updates <see cref="AxisBase.DesiredMargin"/> accordingly. This takes into account the axis title as well as tick labels
        /// potentially exceeding the axis range.
        /// </summary>
        /// <param name="rc">The render context.</param>
        public override void Measure(IRenderContext rc)
        {
            var plotBounds = PlotBounds;

            if (this.Position == AxisPosition.None)
            {
                this.DesiredMargin = new OxyThickness(0);
                return;
            }

            // TODO: see Axis.Measure for more information on what should be here.
        }

        /// <inheritdoc/>
        public override void Render(IRenderContext rc, AxisRenderPass pass)
        {
            // TODO: we want to abstract out rendering, just like ticks and all that... for now lets draw some big rectangles to show we know what we're doing

            if (pass == AxisRenderPass.Pass0)
            {
                rc.DrawRectangle(PlotBounds, OxyColors.Red, new OxyThickness(1), EdgeRenderingMode.Automatic);
                rc.DrawRectangle(new OxyRect(ScreenMin, ScreenMax), OxyColors.Blue, new OxyThickness(1), EdgeRenderingMode.Automatic);
            }
        }

        // NOTE: use plotBounds instead of PlotModel to make Sub-Plot support easier
        /// <summary>
        /// Renders the axis.
        /// </summary>
        /// <param name="plotBounds"></param>
        public void Render(OxyRect plotBounds)
        {
            // assumes we have been measured on these same plotBounds
            if (!(IsHorizontal() || IsVertical()))
            {
                // we should not render
                return;
            }
        }

        /// <inheritdoc/>
        public override void Pan(ScreenPoint previousPoint, ScreenPoint newPoint)
        {
            // TODO: translate screen-space translation into interaction space translation
        }

        /// <inheritdoc/>
        public override void Pan(ScreenReal screenOffsetDelta)
        {
            ViewInteractionCenter = ActualInteractionCenter + ViewInfo.InverseScale(screenOffsetDelta);

            RefreshView();
        }
    }
}