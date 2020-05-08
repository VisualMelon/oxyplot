using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OxyPlot.WindowsForms
{
    /// <summary>
    /// Plots plot models using a <see cref="GraphicsRenderContext"/>.
    /// </summary>
    public class GraphicsRenderContextPlotModelPainter : IPlotModelPainter
    {
        /// <summary>
        /// The render context.
        /// </summary>
        private readonly GraphicsRenderContext renderContext;

        /// <summary>
        /// Initialises a <see cref="GraphicsRenderContextPlotModelPainter"/>.
        /// </summary>
        public GraphicsRenderContextPlotModelPainter()
        {
            this.renderContext = new GraphicsRenderContext();
        }

        /// <summary>
        /// Paints the plot model.
        /// </summary>
        /// <param name="plotModel"></param>
        /// <param name="paintEventArgs"></param>
        /// <param name="size"></param>
        /// <param name="dpi"></param>
        public void Paint(PlotModel plotModel, PaintEventArgs paintEventArgs, Size size, int dpi)
        {
            this.renderContext.SetGraphicsTarget(paintEventArgs.Graphics);

            if (plotModel != null)
            {
                if (!plotModel.Background.IsUndefined())
                {
                    using (var brush = new SolidBrush(plotModel.Background.ToColor()))
                    {
                        paintEventArgs.Graphics.FillRectangle(brush, paintEventArgs.ClipRectangle);
                    }
                }

                ((IPlotModel)plotModel).Render(this.renderContext, new OxyRect(0, 0, size.Width, size.Height));
            }
        }

        /// <summary>
        /// Disposes me.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes me.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.renderContext.Dispose();
            }
        }
    }
}
