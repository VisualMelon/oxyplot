// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlotView.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Represents a control that displays a <see cref="PlotModel" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using SkiaSharp;
using System.Drawing;
using System.Windows.Forms;

namespace OxyPlot.SkiaSharp.WindowsForms
{
    /// <summary>
    /// Represents a control that displays a <see cref="PlotModel" />.
    /// Renders with either SkiaSharp or GDI+.
    /// </summary>
    public class PlotView : OxyPlot.WindowsForms.PlotView
    {
        public bool UseSkiaRenderer { get; set; } = true;

        /// <summary>
        /// The <see cref="bitmap"/> on which to render.
        /// </summary>
        private Bitmap bitmap { get; set; } = null;

        protected override void PaintOverride(PaintEventArgs e)
        {
            if (this.UseSkiaRenderer)
            {
                PaintWithSkia(e);
            }
            else
            {
                base.PaintOverride(e);
            }
        }

        private void PaintWithSkia(PaintEventArgs e)
        {
            var dpi = 96; // TODO: Add DPI awareness for NET Core
            var dpiScale = dpi / 96.0f;
            var plotModel = this.ActualModel;
            if (plotModel == null)
            {
                return;
            }

            var size = this.ClientSize;
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
                    surface.Canvas.Clear(SkiaExtensions.ToSKColor(plotModel.Background));
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

            e.Graphics.DrawImage(this.bitmap, rect);
        }
    }
}
