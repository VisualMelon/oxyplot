using System;
using System.Drawing;
using System.Windows.Forms;

namespace OxyPlot.WindowsForms
{
    /// <summary>
    /// Represents something that can render a <see cref="PlotModel" /> to a <see cref="Graphics"/> instance.
    /// </summary>
    public interface IPlotModelPainter : IDisposable
    {
        /// <summary>
        /// Paints the PlotView
        /// </summary>
        void Paint(PlotModel plotModel, PaintEventArgs paintEventArgs, Size size, int dpi);
    }
}
