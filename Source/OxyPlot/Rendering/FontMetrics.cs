// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextVerticalAlignment.cs" company="OxyPlot">
//   Copyright (c) 2020 OxyPlot contributors
// </copyright>
// <summary>
//   Contains metrics for a given font.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OxyPlot.Rendering
{
    /// <summary>
    /// Contains metrics for a given font.
    /// </summary>
    public class FontMetrics
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FontMetrics" /> class.
        /// </summary>
        /// <param name="ascender">The ascender.</param>
        /// <param name="descender">The descender.</param>
        /// <param name="leading">The leading.</param>
        public FontMetrics(double ascender, double descender, double leading)
        {
            Ascender = ascender;
            Descender = descender;
            Leading = leading;
        }

        /// <summary>
        /// The distance from the baseline to the top of the font.
        /// </summary>
        public double Ascender { get; set; }

        /// <summary>
        /// The distance from the baseline to the bottom of the font.
        /// </summary>
        public double Descender { get; set; }

        /// <summary>
        /// The distance between the bottom of a line of text and the top of the next line of text.
        /// </summary>
        public double Leading { get; set; }

        /// <summary>
        /// The line height of the font.
        /// </summary>
        public double LineHeight => this.Ascender + this.Descender;
    }
}
