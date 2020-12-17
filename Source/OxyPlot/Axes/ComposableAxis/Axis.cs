using System;
using System.Collections.Generic;
using System.Text;

namespace OxyPlot.Axes.ComposableAxis
{
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
    // -> yes, because this is the context in which all serise will render (and who knows what else)

    /// <summary>
    /// Base class for axes.
    /// </summary>
    public abstract class AxisBase : PlotElement, IAxis
    {
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
        /// Gets the view info
        /// </summary>
        public abstract ViewInfo ViewInfo { get; }

        /// <summary>
        /// Gets or sets the screen coordinate of the maximum end of the axis.
        /// </summary>
        public ScreenPoint ScreenMax { get; protected set; }

        /// <summary>
        /// Gets or sets the screen coordinate of the minimum end of the axis.
        /// </summary>
        public ScreenPoint ScreenMin { get; protected set; }

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
        public bool IsHorizontal => this.Position == AxisPosition.Top || this.Position == AxisPosition.Bottom;

        /// <summary>
        /// Determines whether the axis is vertical.
        /// </summary>
        /// <returns><c>true</c> if the axis is horizontal; otherwise, <c>false</c> .</returns>
        public bool IsVertical => this.Position == AxisPosition.Left || this.Position == AxisPosition.Right;

        /// <inheritdoc/>
        public abstract void Reset();

        /// <inheritdoc/>
        public abstract void ZoomAtCenter(double factor);

        /// <inheritdoc/>
        public abstract void ZoomAt(ScreenPoint staticPoint, double factor);

        /// <inheritdoc/>
        public abstract void Pan(ScreenPoint previousPoint, ScreenPoint newPoint);

        /// <inheritdoc/>
        public bool IsAxisVisible { get; set; }
    }

    /// <summary>
    /// All the stuff needed for compat that is too weakly typed.
    /// </summary>
    public interface IOldAxisStuffIWouldPreferDidNotExist
    {
    }

    /// <summary>
    /// An axis.
    /// </summary>
    public interface IAxis : IOldAxisStuffIWouldPreferDidNotExist
    {
        /// <summary>
        /// Gets the Key.
        /// </summary>
        string Key { get; set; }

        /// <summary>
        /// Gets the Position.
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
        /// Gets the View information.
        /// </summary>
        ViewInfo ViewInfo { get; }

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
        /// Zooms by the given factor, maintaining the position of given screen-space point.
        /// </summary>
        /// <param name="staticPoint"></param>
        /// <param name="factor"></param>
        void ZoomAt(ScreenPoint staticPoint, double factor);

        /// <summary>
        /// Pans the axis.
        /// </summary>
        /// <param name="previousPoint"></param>
        /// <param name="newPoint"></param>
        public abstract void Pan(ScreenPoint previousPoint, ScreenPoint newPoint);

        /// <summary>
        /// Gets a value indicating whether the axis is visible.
        /// </summary>
        bool IsAxisVisible { get; }
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

        /// <summary>
        /// The data margin an the minimum.
        /// </summary>
        ScreenReal MinimumDataMargin { get; set; }

        /// <summary>
        /// The data margin at the maximum.
        /// </summary>
        ScreenReal MaximumDataMargin { get; set; }

        /// <summary>
        /// The margin an the minimum.
        /// </summary>
        ScreenReal MinimumMargin { get; set; }

        /// <summary>
        /// The margin at the maximum.
        /// </summary>
        ScreenReal MaximumMargin { get; set; }

        /// <summary>
        /// The padding an the minimum.
        /// </summary>
        double MinimumPadding { get; set; }

        /// <summary>
        /// The padding at the maximum.
        /// </summary>
        double MaximumPadding { get; set; }
    }

    /// <summary>
    /// An axis.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TDataProvider"></typeparam>
    /// <typeparam name="TDataTransformation"></typeparam>
    /// <typeparam name="TDataOptional"></typeparam>
    /// <typeparam name="TDataOptionalProvider"></typeparam>
    public class HorizontalVerticalAxis<TData, TDataProvider, TDataTransformation, TDataOptional, TDataOptionalProvider> : AxisBase, IAxis<TData>
        where TDataProvider : IDataProvider<TData>
        where TDataTransformation : IDataTransformation<TData, TDataProvider>
        where TDataOptionalProvider : IOptionalProvider<TData, TDataOptional>
    {
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
        public ScreenReal MinimumDataMargin { get; set; }

        /// <inheritdoc/>
        public ScreenReal MaximumDataMargin { get; set; }

        /// <inheritdoc/>
        public ScreenReal MinimumMargin { get; set; }

        /// <inheritdoc/>
        public ScreenReal MaximumMargin { get; set; }

        /// <inheritdoc/>
        public double MinimumPadding { get; set; }

        /// <inheritdoc/>
        public double MaximumPadding { get; set; }
        
        /// <summary>
        /// Gets a value indicating whether the axis is reversed.
        /// </summary>
        public bool IsReversed => StartPosition > EndPosition;

        private ViewInfo _viewInfo;

        // TODO: absolute maximums

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
        /// The clip minimum, in Interaction space. Maps to <see cref="ClipMinimum"/>.
        /// </summary>
        private InteractionReal ClipInteractionMinimum { get; set; }

        /// <summary>
        /// The clip maximum, in Interaction space. Maps to <see cref="ClipMaximum"/>.
        /// </summary>
        private InteractionReal ClipInteractionMaximum { get; set; }

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
        public override void ZoomAt(ScreenPoint staticPoint, double factor)
        {
            var ip = ViewInfo.InverseTransform(new ScreenReal(IsHorizontal ? staticPoint.X : staticPoint.Y));

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
            consumer.Consume<TDataProvider, AxisScreenTransformation<TData, TDataProvider, TDataTransformation>>(new AxisScreenTransformation<TData, TDataProvider, TDataTransformation>(DataTransformation, ViewInfo));
        }

        // NOTE: use plotBounds instead of PlotModel to make Sub-Plot support easier
        /// <summary>
        /// Measures and lays the axis out on the given plot bounds.
        /// </summary>
        /// <param name="plotBounds"></param>
        public void MeasureAndLayout(OxyRect plotBounds)
        {
            ScreenReal screenMinimum;
            ScreenReal screenMaximum;

            if (IsHorizontal)
            {
                screenMinimum = new ScreenReal(IsReversed ? plotBounds.Left : plotBounds.Right) + MinimumMargin;
                screenMaximum = new ScreenReal(IsReversed ? plotBounds.Right : plotBounds.Left) - MinimumMargin;
            }
            else if (IsVertical)
            {
                screenMinimum = new ScreenReal(IsReversed ? plotBounds.Bottom : plotBounds.Top) + MinimumMargin;
                screenMaximum = new ScreenReal(IsReversed ? plotBounds.Top : plotBounds.Bottom) - MinimumMargin;
            }
            else
            {
                // we should not render
                return;
            }

            ScreenRange = new Range<ScreenReal>(screenMinimum, screenMaximum);
        }

        // NOTE: use plotBounds instead of PlotModel to make Sub-Plot support easier
        /// <summary>
        /// Renders the axis.
        /// </summary>
        /// <param name="plotBounds"></param>
        public void Render(OxyRect plotBounds)
        {
            // assumes we have been measured
            if (!(IsHorizontal || IsVertical))
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
    }
}
