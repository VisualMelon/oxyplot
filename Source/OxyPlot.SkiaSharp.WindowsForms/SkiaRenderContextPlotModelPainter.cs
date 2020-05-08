using OxyPlot.SkiaSharp;
using OxyPlot.WindowsForms;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OxyPlot.SkiaSharp.WindowsForms
{
    /// <summary>
    /// Plots plot models using a <see cref="SkiaRenderContext"/>.
    /// </summary>
    public class SkiaRenderContextPlotModelPainter : IPlotModelPainter
    {
        /// <summary>
        /// The render context.
        /// </summary>
        private readonly SkiaRenderContext renderContext;

        /// <summary>
        /// Initialises a <see cref="SkiaRenderContextPlotModelPainter"/>.
        /// </summary>
        public SkiaRenderContextPlotModelPainter()
        {
            this.renderContext = new SkiaRenderContext();
        }

        /// <summary>
        /// The <see cref="bitmap"/> on which to render.
        /// </summary>
        private Bitmap bitmap { get; set; } = null;

        /// <summary>
        /// Paints the plot model.
        /// </summary>
        /// <param name="plotModel"></param>
        /// <param name="paintEventArgs"></param>
        /// <param name="size"></param>
        public void Paint(PlotModel plotModel, PaintEventArgs paintEventArgs, Size size, int dpi)
        {
            var dpiScale = dpi / 96.0f;

            if (plotModel == null)
            {
                return;
            }

            if (size.Width <= 0 || size.Height <= 0)
            {
                return;
            }

            var rect = new Rectangle(0, 0, size.Width, size.Height);

            var info = new SKImageInfo(size.Width, size.Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);

            // reset the bitmap if the size has changed
            if (this.bitmap == null || info.Width != this.bitmap.Width || info.Height != this.bitmap.Height)
            {
                this.bitmap = new Bitmap(size.Width, size.Height);
            }

            // draw on the bitmap
            var bitmapData = this.bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            try
            {
                using (var surface = SKSurface.Create(info, bitmapData.Scan0, bitmapData.Stride))
                {
                    surface.Canvas.Clear(SkiaSharp.SkiaExtensions.ToSKColor(plotModel.Background));
                    using (var rc = new SkiaRenderContext()
                    {
                        RendersToScreen = true,
                        SkCanvas = surface.Canvas,
                        UseTextShaping = true,
                    })
                    {
                        lock (plotModel.SyncRoot)
                        {
                            ((IPlotModel)plotModel).Render(rc, new OxyRect(0, 0, size.Width, size.Height));
                        }
                    }
                }
            }
            finally
            {
                this.bitmap.UnlockBits(bitmapData);
            }

            paintEventArgs.Graphics.DrawImage(this.bitmap, rect);
        }

        /// <summary>
        /// Disposes me.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.renderContext.Dispose();
            }
        }
    }
}
