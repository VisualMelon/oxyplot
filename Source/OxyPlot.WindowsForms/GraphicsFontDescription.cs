
#if OXYPLOT_COREDRAWING
namespace OxyPlot.Core.Drawing
#else
namespace OxyPlot.WindowsForms
#endif
{
    using System.Drawing;

    /// <summary>
    /// Describes a GDI+ Font.
    /// </summary>
    public class GraphicsFontDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GraphicsFontDescription" /> class.
        /// </summary>
        /// <param name="fontFamily">The font family.</param>
        /// <param name="fontSize">The font family.</param>
        /// <param name="fontStyle">The font family.</param>
        public GraphicsFontDescription(string fontFamily, double fontSize, FontStyle fontStyle)
        {
            this.FontFamily = fontFamily;
            this.FontSize = fontSize;
            this.FontStyle = fontStyle;

            this.cachedHashCode = ComputeHashCode();
        }

        /// <summary>
        /// Gets the font family.
        /// </summary>
        /// <value>The font family.</value>
        public string FontFamily { get; }

        /// <summary>
        /// Gets the font size.
        /// </summary>
        /// <value>The font size.</value>
        public double FontSize { get; }

        /// <summary>
        /// Gets the font style.
        /// </summary>
        /// <value>The font style.</value>
        public FontStyle FontStyle { get; }

        /// <summary>
        /// The HashCode of the <see cref="GraphicsPenDescription" /> instance, as computed in the constructor.
        /// </summary>
        private readonly int cachedHashCode;

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c> .</returns>
        public override bool Equals(object obj)
        {
            var description = obj as GraphicsFontDescription;

            return description != null &&
                   this.FontFamily == description.FontFamily &&
                   this.FontStyle == description.FontStyle &&
                   this.FontStyle == description.FontStyle;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return cachedHashCode;
        }

        /// <summary>
        /// Computes the HashCode for the instance.
        /// </summary>
        /// <returns>The HashCode for the instance.</returns>
        private int ComputeHashCode()
        {
            var hashCode = 754997215;

            unchecked
            {
                hashCode = hashCode * -1521134295 + this.FontFamily.GetHashCode();
                hashCode = hashCode * -1521134295 + this.FontSize.GetHashCode();
                hashCode = hashCode * -1521134295 + this.FontStyle.GetHashCode();
            }

            return hashCode;
        }

    }
}
