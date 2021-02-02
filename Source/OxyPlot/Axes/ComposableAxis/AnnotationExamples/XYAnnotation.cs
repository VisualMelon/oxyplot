using System;
using System.Collections.Generic;
using System.Text;

namespace OxyPlot.Axes.ComposableAxis.AnnotationExamples
{
    /// <summary>
    /// Represents a typed annotation on a pair of XY Axes.
    /// </summary>
    /// <typeparam name="XData"></typeparam>
    /// <typeparam name="YData"></typeparam>
    public class XYAnnotation<XData, YData> : Annotations.AnnotationBase
    {
        /// <summary>
        /// Gets or set the X Axis key
        /// </summary>
        public string XAxisKey { get; set; }

        /// <summary>
        /// Gets or set the X Axis key
        /// </summary>
        public string YAxisKey { get; set; }

        /// <summary>
        /// Gets or sets the X Axis associated with this series.
        /// </summary>
        public IAxis<XData> XAxis { get; private set; }

        /// <summary>
        /// Gets or sets the Y Axis associated with this series.
        /// </summary>
        public IAxis<YData> YAxis { get; private set; }

        /// <summary>
        /// Gets or sets the X/Y Collator for the axes.
        /// </summary>
        protected XYCollator<XData, YData> Collator { get; private set; }

        /// <summary>
        /// Resolves axes from the axis keys.
        /// </summary>
        protected void ResolveAxes()
        {
            // should throw if we can't get axes of the right type
            XAxis = (IAxis<XData>)this.PlotModel.GetAxisOrDefault(XAxisKey, this.PlotModel.DefaultXAxis);
            YAxis = (IAxis<YData>)this.PlotModel.GetAxisOrDefault(YAxisKey, this.PlotModel.DefaultYAxis);
            Collator = XYCollator<XData, YData>.Prepare(XAxis, YAxis);
        }

        /// <summary>
        /// Gets an <see cref="IXYHelper{XData, YData}"/> for the current axis view state.
        /// </summary>
        /// <returns></returns>
        protected virtual IXYRenderHelper<XData, YData> GetRenderHelper()
        {
            var transpose = XAxis.Position == AxisPosition.Left || XAxis.Position == AxisPosition.Right;
            return XYRenderHelperPreparer<XData, YData>.PrepareHorizontalVertial(Collator, transpose);
        }

        /// <inheritdoc/>
        public override void EnsureAxes()
        {
            ResolveAxes();
        }

        /// <inheritdoc/>
        public override void Render(IRenderContext rc)
        {
        }
    }
}
