namespace OxyPlot.Axes
{
    using OxyPlot.Axes.ComposableAxis;

    /// <summary>
    /// Represents an angular axis that covers the whole plot area.
    /// </summary>
    public class AngleAxisFullPlotArea : AngleAxis
    {
        /// <summary>
        /// Renders the axis on the specified render context.
        /// </summary>
        /// <param name="rc">The render context.</param>
        /// <param name="pass">The pass.</param>
        public override void Render(IRenderContext rc, AxisRenderPass pass)
        {
            var r = new AngleAxisFullPlotAreaRenderer(rc, this.PlotModel);
            r.Render(this, pass);
        }
    }
}
