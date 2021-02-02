﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Annotation.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Provides an abstract base class for annotations.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OxyPlot.Annotations
{
    using OxyPlot.Axes;

    /// <summary>
    /// Provides an abstract base class for annotations.
    /// </summary>
    public abstract class AnnotationBase : PlotElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Annotation" /> class.
        /// </summary>
        protected AnnotationBase()
        {
            this.Layer = AnnotationLayer.AboveSeries;
        }

        /// <summary>
        /// Gets or sets the rendering layer of the annotation. The default value is <see cref="AnnotationLayer.AboveSeries" />.
        /// </summary>
        public AnnotationLayer Layer { get; set; }

        /// <summary>
        /// Ensures that the annotation axes are set.
        /// </summary>
        public abstract void EnsureAxes();

        /// <summary>
        /// Renders the annotation on the specified context.
        /// </summary>
        /// <param name="rc">The render context.</param>
        public abstract void Render(IRenderContext rc);
    }

    /// <summary>
    /// Provides an abstract base class for annotations on untyped XY plots.
    /// </summary>
    public abstract class Annotation : AnnotationBase, IXyAxisPlotElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Annotation" /> class.
        /// </summary>
        protected Annotation()
        {
            this.ClipByXAxis = true;
            this.ClipByYAxis = true;
        }

        /// <summary>
        /// Gets the X axis.
        /// </summary>
        /// <value>The X axis.</value>
        public Axis XAxis { get; private set; }

        /// <summary>
        /// Gets or sets the X axis key.
        /// </summary>
        /// <value>The X axis key.</value>
        public string XAxisKey { get; set; }

        /// <summary>
        /// Gets the Y axis.
        /// </summary>
        /// <value>The Y axis.</value>
        public Axis YAxis { get; private set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether to clip the annotation by the X axis range.
        /// </summary>
        /// <value><c>true</c> if clipping by the X axis is enabled; otherwise, <c>false</c>.</value>
        public bool ClipByXAxis { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to clip the annotation by the Y axis range.
        /// </summary>
        /// <value><c>true</c> if clipping by the Y axis is enabled; otherwise, <c>false</c>.</value>
        public bool ClipByYAxis { get; set; }

        /// <summary>
        /// Gets or sets the Y axis key.
        /// </summary>
        /// <value>The Y axis key.</value>
        public string YAxisKey { get; set; }

        /// <inheritdoc/>
        public override void EnsureAxes()
        {
            this.XAxis = (Axis)( this.XAxisKey != null ? this.PlotModel.GetAxis(this.XAxisKey) : this.PlotModel.DefaultXAxis ); // TODO: ... not sure... we need to re-write every single component so that they are typed... so this code won't survive... for now it can assume untyped axes
            this.YAxis = (Axis)(this.YAxisKey != null ? this.PlotModel.GetAxis(this.YAxisKey) : this.PlotModel.DefaultYAxis ); // ... or (much better): it could assume IAxis<double> (note: that's why I need everything in AxisBase in IAxis... I knew there was a reason for IAxis)
        }

        /// <inheritdoc/>
        public override void Render(IRenderContext rc)
        {
        }

        /// <inheritdoc/>
        public override OxyRect GetClippingRect()
        {
            var rect = this.PlotModel.PlotArea;
            var axisRect = PlotElementUtilities.GetClippingRect(this);

            var minX = 0d;
            var maxX = double.PositiveInfinity;
            var minY = 0d;
            var maxY = double.PositiveInfinity;

            if (this.ClipByXAxis)
            {
                minX = axisRect.TopLeft.X;
                maxX = axisRect.BottomRight.X;
            }

            if (this.ClipByYAxis)
            {
                minY = axisRect.TopLeft.Y;
                maxY = axisRect.BottomRight.Y;
            }

            var minPoint = new ScreenPoint(minX, minY);
            var maxPoint = new ScreenPoint(maxX, maxY);

            var axisClipRect = new OxyRect(minPoint, maxPoint);
            return rect.Clip(axisClipRect);
        }

        /// <inheritdoc/>
        public virtual ScreenPoint Transform(DataPoint p)
        {
            return PlotElementUtilities.Transform(this, p);
        }

        /// <inheritdoc/>
        public virtual DataPoint InverseTransform(ScreenPoint p)
        {
            return PlotElementUtilities.InverseTransform(this, p);
        }
    }
}
