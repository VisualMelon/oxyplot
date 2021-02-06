using System;
using System.Collections.Generic;
using System.Linq;
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
        void ZoomAt(double factor, ScreenPoint staticPoint);
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

            this.IsZoomEnabled = true;
            this.IsPanEnabled = true;

            this.MinimumPadding = 0.01;
            this.MaximumPadding = 0.01;
            this.MinimumDataMargin = ScreenReal.Zero;
            this.MaximumDataMargin = ScreenReal.Zero;
            this.MinimumMargin = ScreenReal.Zero;
            this.MaximumMargin = ScreenReal.Zero;

            this.StartPosition = 0;
            this.EndPosition = 1;

            this.IsAxisVisible = true;
            this.Layer = AxisLayer.BelowSeries;

            this.TitlePosition = 0.5;
            this.TitleFormatString = "{0} [{1}]";
            this.TitleClippingLength = 0.9;
            this.TitleColor = OxyColors.Automatic;
            this.TitleFontSize = double.NaN;
            this.TitleFontWeight = FontWeights.Normal;
            this.ClipTitle = true;

            this.AxisTitleDistance = 4;
        }

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
        /// The offset in Interaction Space: only here for compat.
        /// <see cref="ViewInfo"/> is the new guy, at the moment: note that <see cref="ViewInfo.ScreenOffset"/> is in screenspace: the conversion is ScreenOffset = Offset/-Scale
        /// </summary>
        public abstract double Offset { get; } // the other half of the ViewInfo from Scale

        /// <summary>
        /// The Scale and Offset, with some meaningful behaviour bolted on for good measure.
        /// </summary>
        public abstract ViewInfo ViewInfo { get; }

        /// <summary>
        /// Gets or sets the key of the axis. This can be used to specify an axis if you have defined multiple axes in a plot. The default value is <c>null</c>.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the title of the axis. The default value is <c>null</c>.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the length of the title clipping rectangle (fraction of the available length of the axis). The default value is <c>0.9</c>.
        /// </summary>
        public double TitleClippingLength { get; set; }

        /// <summary>
        /// Gets or sets the color of the title. The default value is <see cref="OxyColors.Automatic"/>.
        /// </summary>
        /// <remarks>If the value is <c>null</c>, the <see cref="PlotModel.TextColor" /> will be used.</remarks>
        public OxyColor TitleColor { get; set; }

        /// <summary>
        /// Gets or sets the title font. The default value is <c>null</c>.
        /// </summary>
        public string TitleFont { get; set; }

        /// <summary>
        /// Gets or sets the size of the title font. The default value is <c>double.NaN</c>.
        /// </summary>
        public double TitleFontSize { get; set; }

        /// <summary>
        /// Gets or sets the weight of the title font. The default value is <see cref="FontWeights.Normal"/>.
        /// </summary>
        public double TitleFontWeight { get; set; }

        /// <summary>
        /// Gets or sets the minimum distance from the axis labels to the axis title. The default value is <c>4</c>.
        /// </summary>
        public double AxisTitleDistance { get; set; }

        /// <summary>
        /// Gets or sets the orientation angle (degrees) for the axis labels. The default value is <c>0</c>.
        /// </summary>
        public double Angle { get; set; }

        /// <summary>
        /// Gets or sets the distance from the end of the tick lines to the labels. The default value is <c>4</c>.
        /// </summary>
        public double AxisTickToLabelDistance { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to clip the axis title. The default value is <c>true</c>.
        /// </summary>
        public bool ClipTitle { get; set; }

        /// <summary>
        /// Gets the actual title of the axis.
        /// </summary>
        /// <remarks>If the <see cref="Unit" /> property is set, the <see cref="TitleFormatString" /> property is used to format the actual title.</remarks>
        public string ActualTitle
        {
            get
            {
                if (this.Unit != null)
                {
                    return string.Format(this.TitleFormatString, this.Title, this.Unit);
                }

                return this.Title;
            }
        }

        /// <summary>
        /// Gets the actual color of the title.
        /// </summary>
        protected internal OxyColor ActualTitleColor
        {
            get
            {
                return this.TitleColor.GetActualColor(this.PlotModel.TextColor);
            }
        }

        /// <summary>
        /// Gets the actual title font.
        /// </summary>
        protected internal string ActualTitleFont
        {
            get
            {
                return this.TitleFont ?? this.PlotModel.DefaultFont;
            }
        }

        /// <summary>
        /// Gets the actual size of the title font.
        /// </summary>
        protected internal double ActualTitleFontSize
        {
            get
            {
                return !double.IsNaN(this.TitleFontSize) ? this.TitleFontSize : this.ActualFontSize;
            }
        }

        /// <summary>
        /// Gets the actual title font weight.
        /// </summary>
        protected internal double ActualTitleFontWeight
        {
            get
            {
                return !double.IsNaN(this.TitleFontWeight) ? this.TitleFontWeight : this.ActualFontWeight;
            }
        }

        /// <summary>
        /// Gets or sets the format string used for formatting the title and unit when <see cref="Unit" /> is defined.
        /// The default value is "{0} [{1}]", where {0} refers to the <see cref="Title" /> and {1} refers to the <see cref="Unit" />.
        /// </summary>
        /// <remarks>If <see cref="Unit" /> is <c>null</c>, the actual title is defined by <see cref="Title" /> only.</remarks>
        public string TitleFormatString { get; set; }

        /// <summary>
        /// Gets or sets the unit of the axis. The default value is <c>null</c>.
        /// </summary>
        /// <remarks>The <see cref="TitleFormatString" /> is used to format the title including this unit.</remarks>
        public string Unit { get; set; }

        /// <summary>
        /// Gets or sets the position of the title. The default value is <c>0.5</c>.
        /// </summary>
        /// <remarks>The position is defined by a fraction in the range <c>0</c> to <c>1</c>.</remarks>
        public double TitlePosition { get; set; }

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
        /// Zooms to between the specified points in screen space.
        /// </summary>
        /// <param name="s0">The new minimum.</param>
        /// <param name="s1">The new maximum.</param>
        public abstract void Zoom(ScreenReal s0, ScreenReal s1);

        /// <summary>
        /// Zooms by the given factor, maintaining the given screen-space position.
        /// </summary>
        /// <param name="factor"></param>
        /// <param name="position"></param>
        public abstract void ZoomAt(double factor, ScreenReal position);

        /// <summary>
        /// Zoom to the specified scale.
        /// </summary>
        /// <param name="newScale">The new scale.</param>
        public abstract void Zoom(double newScale);

        /// <summary>
        /// Gets or sets a value indicating whether zooming is enabled. The default value is <c>true</c>.
        /// </summary>
        public virtual bool IsZoomEnabled { get; set; }

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
        /// Gets or sets a value indicating whether panning is enabled. The default value is <c>true</c>.
        /// </summary>
        public virtual bool IsPanEnabled { get; set; }

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
        /// <value>The number of device independent units to be left empty between the axis and the <see cref="ComposableAxis.AxisBase.StartPosition"/>.</value>
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
        /// <value>The number of device independent units to be left empty between the axis and the <see cref="ComposableAxis.AxisBase.StartPosition"/>.</value>
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
        /// Zooms to between the specified points in screen space.
        /// </summary>
        /// <param name="s0">The new minimum.</param>
        /// <param name="s1">The new maximum.</param>
        void Zoom(ScreenReal s0, ScreenReal s1);

        /// <summary>
        /// Zooms by the given factor, maintaining the given screen-space position.
        /// </summary>
        /// <param name="factor"></param>
        /// <param name="position"></param>
        public abstract void ZoomAt(double factor, ScreenReal position);

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

        /// <summary>
        /// Gets or sets the screen coordinate of the maximum end of the axis.
        /// </summary>
        ScreenPoint ScreenMax { get; }

        /// <summary>
        /// Gets or sets the screen coordinate of the minimum end of the axis.
        /// </summary>
        ScreenPoint ScreenMin { get; }
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
        /// Consumes a <see cref="IAxisScreenTransformationConsumer{TData}"/>, which provides strongly typed access to the axis' current transformation.
        /// </summary>
        /// <param name="consumer">The consumer that will receive the transformation.</param>
        void ConsumeTransformation(IAxisScreenTransformationConsumer<TData> consumer);

        /// <summary>
        /// Provides convienient access to the axis' current transformation when going via an <see cref="IAxisScreenTransformationConsumer{TData}"/> is not worth the effort.
        /// </summary>
        /// <returns></returns>
        IAxisScreenTransformation<TData> GetTransformation();

        /// <summary>
        /// The minimum value that will be rendered.
        /// </summary>
        TData ClipMinimum { get; }

        /// <summary>
        /// The maximum value that will be rendered.
        /// </summary>
        TData ClipMaximum { get; }

        /// <summary>
        /// The logical minimum value.
        /// </summary>
        TData ActualMinimum { get; }

        /// <summary>
        /// The logical maximum value.
        /// </summary>
        TData ActualMaximum { get; }

        /// <summary>
        /// Tries to get the data range associated with this axis.
        /// Returns <c>true</c> if there is a meaningful data range, otherwise <c>false</c>.
        /// </summary>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <returns></returns>
        bool TryGetDataRange(out TData minimum, out TData maximum);

        /// <summary>
        /// Includes a sample in the axis data range.
        /// </summary>
        /// <param name="sample"></param>
        void Include(TData sample);
    }

    /// <summary>
    /// An axis.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TDataProvider"></typeparam>
    /// <typeparam name="TDataTransformation"></typeparam>
    /// <typeparam name="TDataFilter"></typeparam>
    /// <typeparam name="TDataOptional"></typeparam>
    /// <typeparam name="TDataOptionalProvider"></typeparam>
    public class HorizontalVerticalAxis<TData, TDataProvider, TDataTransformation, TDataFilter, TDataOptional, TDataOptionalProvider> : AxisBase, IAxis<TData>, IComposableAxisStuffThatIsCurrentlyCausingProblemsWithRespectToCompatabilityWithExistingAxisImplementsAndAsSuchCannotAppearInIAxisAtThisTime
        where TDataProvider : IDataProvider<TData>
        where TDataTransformation : IDataTransformation<TData, TDataProvider>
        where TDataFilter : IFilter<TData>
        where TDataOptionalProvider : IOptionalProvider<TData, TDataOptional>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HorizontalVerticalAxis{TData, TDataProvider, TDataTransformation, TDataFilter, TDataOptional, TDataOptionalProvider}" /> class.
        /// </summary>
        public HorizontalVerticalAxis(TDataTransformation dataTransformation, TDataOptionalProvider optionalProvider, TDataFilter filter)
        {
            DataTransformation = dataTransformation;
            OptionalProvider = optionalProvider;
            Filter = filter;

            Minimum = OptionalProvider.None;
            Maximum = OptionalProvider.None;
        }

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

        /// <summary>
        /// The <typeparamref name="TDataFilter"/>.
        /// </summary>
        public TDataFilter Filter { get; set; }

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

        /// <inheritdoc/>
        public bool TryGetDataRange(out TData minimum, out TData maximum)
        {
            return DataRange.TryGetMinMax(out minimum, out maximum);
        }

        /// <summary>
        /// The default range presented to the user when there is no better altenative.
        /// </summary>
        public Range<TData> DefaultViewRange { get; set; }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override void ZoomAtCenter(double factor)
        {
            ViewInteractionRadius = ActualInteractionRadius * factor;
            ViewInteractionCenter = ActualInteractionCenter;

            RefreshView();
        }

        /// <inheritdoc/>
        public override void Zoom(ScreenReal s0, ScreenReal s1)
        {
            var imax = ViewInfo.InverseTransform(s0);
            var imin = ViewInfo.InverseTransform(s1);

            ViewInteractionCenter = (imax + imin) / 2.0;
            ViewInteractionRadius = new InteractionReal(Math.Abs(imax.Value - imin.Value)) / 2.0;

            RefreshView();
        }

        /// <inheritdoc/>
        public override void ZoomAt(double factor, ScreenReal position)
        {
            var ip = ViewInfo.InverseTransform(position);

            ViewInteractionRadius = ActualInteractionRadius / factor;
            ViewInteractionCenter = (ActualInteractionCenter - ip) / factor + ip;

            RefreshView();
        }

        /// <inheritdoc/>
        public void ZoomAt(double factor, ScreenPoint center)
        {
            var s = new ScreenReal(IsHorizontal() ? center.X : center.Y);

            ZoomAt(factor, s);
        }

        /// <inheritdoc/>
        public void ZoomTo(TData min, TData max)
        {
            var imax = DataTransformation.Transform(min);
            var imin = DataTransformation.Transform(max);

            ViewInteractionCenter = (imax + imin) / 2.0;
            ViewInteractionRadius = new InteractionReal(Math.Abs(imax.Value - imin.Value)) / 2.0;

            RefreshView();
        }

        /// <summary>
        /// Zoom to the specified scale.
        /// </summary>
        /// <param name="newScale">The new scale.</param>
        public override void Zoom(double newScale)
        {
            var factor = newScale / Scale;

            ViewInteractionRadius = ActualInteractionRadius * factor;
            ViewInteractionCenter = ActualInteractionCenter;

            RefreshView();
        }

        /// <summary>
        /// Pans by the given factor.
        /// </summary>
        /// <param name="factor"></param>
        public void Pan(double factor)
        {
            ViewInteractionRadius = ActualInteractionRadius;
            ViewInteractionCenter = ActualInteractionCenter - ActualInteractionRadius * factor * 2;

            RefreshView();
        }

        /// <inheritdoc/>
        public override void Pan(ScreenPoint previousPoint, ScreenPoint newPoint)
        {
            var s0 = new ScreenReal(IsHorizontal() ? previousPoint.X : previousPoint.Y);
            var s1 = new ScreenReal(IsHorizontal() ? newPoint.X : newPoint.Y);

            ViewInteractionCenter = ActualInteractionCenter - ViewInfo.InverseScale(s1 - s0);
            ViewInteractionRadius = ActualInteractionRadius;

            RefreshView();
        }

        /// <inheritdoc/>
        public override void Pan(ScreenReal screenOffsetDelta)
        {
            ViewInteractionCenter = ActualInteractionCenter - ViewInfo.InverseScale(screenOffsetDelta);
            ViewInteractionRadius = ActualInteractionRadius;

            RefreshView();
        }

        /// <inheritdoc/>
        public override ViewInfo ViewInfo => _viewInfo;

        /// <inheritdoc/>
        public override double Scale => ViewInfo.ScreenScale;

        /// <inheritdoc/>
        public override double Offset => ViewInfo.ScreenOffset.Value / -Scale;

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

            var screenWidth = screenMax - screenMin;

            if (ViewInteractionCenter.HasValue)
            {
                ActualInteractionCenter = ViewInteractionCenter.Value;
                ActualInteractionRadius = ViewInteractionRadius.Value;
            }
            else
            {
                var fallbackRange = DataRange.IsEmpty ? DefaultViewRange : DataRange;

                bool hasDataRange = fallbackRange.TryGetMinMax(out var dmin, out var dmax);

                bool hasRange = hasDataRange || (OptionalProvider.HasValue(Minimum) && OptionalProvider.HasValue(Maximum));

                if (!hasRange)
                    throw new InvalidOperationException("Axis must exist on some range");

                var minimum = OptionalProvider.Unpack(Minimum, dmin);
                var maximum = OptionalProvider.Unpack(Maximum, dmax);

                var imin = DataTransformation.Transform(minimum);
                var imax = DataTransformation.Transform(maximum);

                // apply interaction-space padding where Minimum/Maximum are not specified
                var iwidth = imax - imin;
                if (!OptionalProvider.HasValue(Minimum))
                    imin = imin - iwidth * MinimumPadding;
                if (!OptionalProvider.HasValue(Maximum))
                    imax = imax + iwidth * MaximumPadding;

                ActualInteractionCenter = (imax + imin) / 2.0;
                ActualInteractionRadius = (imax - imin) / 2.0;
            }

            if (screenWidth.Value == 0 || ActualInteractionRadius.Value == 0)
            {
                return; // we cannot
            }

            var actualScreenWidth = screenWidth - (MinimumDataMargin + MaximumDataMargin) * Math.Sign(screenWidth.Value);

            var scale = actualScreenWidth.Value / (ActualInteractionRadius * 2.0).Value;

            ClipInteractionMinimum = ActualInteractionMinimum - new InteractionReal(MinimumDataMargin.Value / Math.Abs(scale));
            ClipInteractionMaximum = ActualInteractionMaximum + new InteractionReal(MaximumDataMargin.Value / Math.Abs(scale));

            ActualMinimum = DataTransformation.InverseTransform(ActualInteractionMinimum);
            ActualMaximum = DataTransformation.InverseTransform(ActualInteractionMaximum);
            ClipMinimum = DataTransformation.InverseTransform(ClipInteractionMinimum);
            ClipMaximum = DataTransformation.InverseTransform(ClipInteractionMaximum);

            var offset = new ScreenReal(screenMin.Value - ClipInteractionMinimum.Value * scale);

            _viewInfo = new ViewInfo(offset, scale);
        }

        /// <inheritdoc/>
        public void ConsumeTransformation(IAxisScreenTransformationConsumer<TData> consumer)
        {
            consumer.Consume<TDataProvider, AxisScreenTransformation<TData, TDataProvider, TDataTransformation, TDataFilter>>(GetTransformation());
        }

        private AxisScreenTransformation<TData, TDataProvider, TDataTransformation, TDataFilter> GetTransformation()
        {
            return new AxisScreenTransformation<TData, TDataProvider, TDataTransformation, TDataFilter>(DataTransformation, Filter, ViewInfo, ClipMinimum, ClipMaximum);
        }

        IAxisScreenTransformation<TData> IAxis<TData>.GetTransformation()
        {
            return GetTransformation();
        }

        internal override void UpdateIntervals(OxyRect plotArea)
        {
            // the plan is to abstract this away entirely... so let's do nothing for now
        }

        /// <inheritdoc/>
        internal override void UpdateTransform(OxyRect bounds) // WHY INTERNAL WHY
        {
            PlotBounds = bounds;

            static double lerp(double x0, double x1, double c) => x1 * c + x0 * (1 - c);

            ScreenReal screenMinimum;
            ScreenReal screenMaximum;

            double marginSign = (this.IsHorizontal() ^ this.IsReversed) ? 1.0 : -1.0;

            if (IsHorizontal())
            {
                screenMinimum = new ScreenReal(lerp(bounds.Left, bounds.Right, this.StartPosition) + MinimumMargin.Value * marginSign);
                screenMaximum = new ScreenReal(lerp(bounds.Left, bounds.Right, this.EndPosition) - MaximumMargin.Value * marginSign);

                ScreenMin = new ScreenPoint(screenMinimum.Value, bounds.Top);
                ScreenMax = new ScreenPoint(screenMaximum.Value, bounds.Bottom);
            }
            else if (IsVertical())
            {
                screenMinimum = new ScreenReal(lerp(bounds.Bottom, bounds.Top, this.StartPosition) + MinimumMargin.Value * marginSign);
                screenMaximum = new ScreenReal(lerp(bounds.Bottom, bounds.Top, this.EndPosition) - MaximumMargin.Value * marginSign);

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
            // should probably be a better place for this...

            foreach (var band in Bands)
            {
                band.AssociateAxis(this);
            }

            foreach (var band in Bands)
            {
                band.Update();
            }

            var plotBounds = PlotBounds;

            if (this.Position == AxisPosition.None)
            {
                this.DesiredMargin = new OxyThickness(0);
                return;
            }

            if (!ScreenRange.TryGetMinMax(out var min, out var max))
            {
                throw new InvalidOperationException("Axis must exist on some range");
            }

            // margins are determined by the bands
            inlineAndSideWidth = Math.Abs((max - min).Value);
            inlineExcesses = Measure(rc, BandPosition.Inline, inlineAndSideWidth);
            sideExcesses = Measure(rc, BandPosition.Side, inlineAndSideWidth);

            inlineTotalHeight = inlineExcesses.Values.Sum(e => e.Top + e.Bottom);
            sideTotalHeight = sideExcesses.Values.Sum(e => e.Top + e.Bottom);

            inlineNearExcesses = Measure(rc, BandPosition.InlineNear, inlineTotalHeight);
            inlineFarExcesses = Measure(rc, BandPosition.InlineFar, inlineTotalHeight);

            sideNearExcesses = Measure(rc, BandPosition.SideNear, sideTotalHeight);
            sideFarExcesses = Measure(rc, BandPosition.SideFar, sideTotalHeight);

            var topMargin = inlineExcesses.Values.Sum(e => e.Top + e.Bottom)
                + sideExcesses.Values.Sum(e => e.Top + e.Bottom);
            var leftMargin = Math.Max(inlineExcesses.Count == 0 ? 0 : inlineExcesses.Values.Max(e => e.Left),
                sideExcesses.Count == 0 ? 0 : sideExcesses.Values.Max(e => e.Left));
            var rightMargin = Math.Max(inlineExcesses.Count == 0 ? 0 : inlineExcesses.Values.Max(e => e.Right),
                sideExcesses.Count == 0 ? 0 : sideExcesses.Values.Max(e => e.Right));

            var nearHeight = Math.Max(inlineNearExcesses.Values.Sum(e => e.Top + e.Bottom),
                sideNearExcesses.Values.Sum(e => e.Top + e.Bottom));
            var farHeight = Math.Max(inlineFarExcesses.Values.Sum(e => e.Top + e.Bottom),
                sideFarExcesses.Values.Sum(e => e.Top + e.Bottom));

            // now we know how big everything is, we need to position it, starting with the inline/side
            var topRight = this.Position switch
            {
                AxisPosition.Left => false,
                AxisPosition.Top => true,
                AxisPosition.Right => true,
                AxisPosition.Bottom => false,
                _ => throw new NotImplementedException()
            };

            if (topRight)
            {
                leftMargin += nearHeight;
                rightMargin += farHeight;
            }
            else
            {
                rightMargin += nearHeight;
                leftMargin += farHeight;
            }

            this.DesiredMargin = this.Position switch
            {
                AxisPosition.Left => new OxyThickness(topMargin, rightMargin, 0.0, leftMargin),
                AxisPosition.Top => new OxyThickness(leftMargin, topMargin, rightMargin, 0.0),
                AxisPosition.Right => new OxyThickness(0.0, leftMargin, topMargin, rightMargin),
                AxisPosition.Bottom => new OxyThickness(rightMargin, 0.0, leftMargin, topMargin),
                _ => throw new NotImplementedException(),
            };
        }

        private void LayoutBands()
        {
            // now we know how big everything is, we need to position it, starting with the inline/side
            var topRight = this.Position switch
            {
                AxisPosition.Left => false,
                AxisPosition.Top => true,
                AxisPosition.Right => true,
                AxisPosition.Bottom => false,
                _ => throw new NotImplementedException()
            };

            var trSign = topRight ? 1 : -1;

            // NOTE: this doesn't touch the position tiers yet
            var defaultSideOffset = new ScreenReal(this.Position switch
            {
                AxisPosition.Left => this.ScreenMin.X,
                AxisPosition.Top => this.ScreenMin.Y,
                AxisPosition.Right => this.ScreenMax.X,
                AxisPosition.Bottom => this.ScreenMax.Y,
                _ => throw new NotImplementedException(),
            });


            if (this.PositionAtZeroCrossing)
            {
                // TODO: allow the user to specify a perpendicular axis for this purpose
                var perpendicularAxis = this.IsHorizontal() ? this.PlotModel.DefaultYAxis : this.PlotModel.DefaultXAxis;

                inlineOffset = new ScreenReal(perpendicularAxis.ViewInfo.Transform(InteractionReal.Zero).Value);

                var perpendicularScreenRange = perpendicularAxis.IsHorizontal()
                    ? new Range<ScreenReal>(new ScreenReal(Math.Min(perpendicularAxis.ScreenMin.X, perpendicularAxis.ScreenMax.X)), new ScreenReal(Math.Max(perpendicularAxis.ScreenMin.X, perpendicularAxis.ScreenMax.X)))
                    : new Range<ScreenReal>(new ScreenReal(Math.Min(perpendicularAxis.ScreenMin.Y, perpendicularAxis.ScreenMax.Y)), new ScreenReal(Math.Max(perpendicularAxis.ScreenMin.Y, perpendicularAxis.ScreenMax.Y)));

                perpendicularScreenRange.TryGetMinMax(out var pmin, out var pmax);

                inlineOffset = new ScreenReal(Math.Max(pmin.Value, Math.Min(pmax.Value, inlineOffset.Value))); // clamp to the sides
            }
            else
            {
                inlineOffset = new ScreenReal(this.Position switch
                {
                    AxisPosition.Left => this.ScreenMin.X,
                    AxisPosition.Top => this.ScreenMin.Y,
                    AxisPosition.Right => this.ScreenMax.X,
                    AxisPosition.Bottom => this.ScreenMax.Y,
                    _ => throw new NotImplementedException(),
                });
            }

            var inlineTop = inlineExcesses.Sum(kv => kv.Key > 0 ? kv.Value.Top + kv.Value.Bottom : kv.Key == 0 ? kv.Value.Top : 0);
            var sideBottom = sideExcesses.Sum(kv => kv.Key < 0 ? kv.Value.Top + kv.Value.Bottom : kv.Key == 0 ? kv.Value.Bottom : 0);
            sideOffset = topRight
                ? new ScreenReal(Math.Max(defaultSideOffset.Value, inlineOffset.Value + inlineTop + sideBottom))
                : new ScreenReal(Math.Min(defaultSideOffset.Value, inlineOffset.Value - inlineTop - sideBottom)); // clamp to the side

            ScreenPoint reference(double s)
            {
                return this.Position switch
                {
                    AxisPosition.Left => new ScreenPoint(s, Math.Max(this.ScreenMin.Y, this.ScreenMax.Y)),
                    AxisPosition.Top => new ScreenPoint(Math.Min(this.ScreenMin.X, this.ScreenMax.X), s),
                    AxisPosition.Right => new ScreenPoint(s, Math.Min(this.ScreenMin.Y, this.ScreenMax.Y)),
                    AxisPosition.Bottom => new ScreenPoint(Math.Max(this.ScreenMin.X, this.ScreenMax.X), s),
                    _ => throw new NotImplementedException(),
                };
            }

            inlineLocations = Layout(inlineExcesses, this.Position, reference(inlineOffset.Value), inlineAndSideWidth);
            sideLocations = Layout(sideExcesses, this.Position, reference(sideOffset.Value), inlineAndSideWidth);
        }

        // layout state
        private double inlineAndSideWidth;
        private Dictionary<int, BandExcesses> inlineExcesses;
        private Dictionary<int, BandExcesses> sideExcesses;
        private double inlineTotalHeight;
        private double sideTotalHeight;
        private Dictionary<int, BandExcesses> inlineNearExcesses;
        private Dictionary<int, BandExcesses> inlineFarExcesses;
        private Dictionary<int, BandExcesses> sideNearExcesses;
        private Dictionary<int, BandExcesses> sideFarExcesses;
        private ScreenReal inlineOffset;
        private ScreenReal sideOffset;

        // actually useful layout state
        private Dictionary<int, BandLocation> inlineLocations;
        private Dictionary<int, BandLocation> sideLocations;

        private Dictionary<int, BandExcesses> Measure(IRenderContext rc, BandPosition position, double width)
        {
            var bands = Bands.Where(b => b.BandPosition == position).ToArray();

            var res = new Dictionary<int, BandExcesses>();

            if (bands.Length == 0)
                return res;

            var count = 1
                + bands.Max(b => b.BandTier)
                - bands.Min(b => b.BandTier);

            foreach (var b in bands)
            {
                var index = b.BandTier;
                b.Measure(rc, width);

                if (res.TryGetValue(index, out var found))
                {
                    res[index] = BandExcesses.Max(found, b.Excesses);
                }
                else
                {
                    res.Add(index, b.Excesses.ClampToZero());
                }
            }

            return res;
        }

        private Dictionary<int, BandLocation> Layout(Dictionary<int, BandExcesses> excesses, AxisPosition position, ScreenPoint minReference, double width)
        {
            var unitPerpendicular = this.Position switch
            {
                AxisPosition.Left => new ScreenVector(0, -1),
                AxisPosition.Top => new ScreenVector(1, 0),
                AxisPosition.Right => new ScreenVector(0, 1),
                AxisPosition.Bottom => new ScreenVector(-1, 0),
                _ => throw new NotImplementedException(),
            };

            var unitNormal = this.Position switch
            {
                AxisPosition.Left => new ScreenVector(-1, 0),
                AxisPosition.Top => new ScreenVector(0, -1),
                AxisPosition.Right => new ScreenVector(1, 0),
                AxisPosition.Bottom => new ScreenVector(0, 1),
                _ => throw new NotImplementedException(),
            };

            var tiers = excesses.Keys.OrderBy(x => x).ToArray();

            var locations = new Dictionary<int, BandLocation>();

            foreach (var tier in tiers)
            {
                var e = excesses[tier];
                minReference += unitNormal * e.Bottom;
                locations.Add(tier, new BandLocation(minReference, unitPerpendicular * width, unitNormal));
                minReference += unitNormal * e.Top;
            }

            return locations;
        }

        /// <summary>
        /// A list of bands used by this <see cref="HorizontalVerticalAxis{TData, TDataProvider, TDataTransformation, TDataFilter, TDataOptional, TDataOptionalProvider}"/>.
        /// </summary>
        public List<IBand> Bands { get; } = new List<IBand>() { new TitleBand() };

        /// <inheritdoc/>
        public override void Render(IRenderContext rc, AxisRenderPass pass)
        {
            if (pass == AxisRenderPass.Pass0)
            {
                LayoutBands();
                RenderBands(rc);
            }
        }

        private void RenderBands(IRenderContext rc)
        {
            foreach (var band in Bands)
            {
                var table = band.BandPosition switch
                {
                    BandPosition.Inline => inlineLocations,
                    BandPosition.Side => sideLocations,
                    _ => throw new NotImplementedException(),
                };

                var location = table[band.BandTier];

                band.Render(rc, location);
            }
        }
    }
}
