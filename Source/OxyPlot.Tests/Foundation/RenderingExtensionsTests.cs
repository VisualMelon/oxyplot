// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RenderingExtensionsTests.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Provides unit tests for the <see cref="RenderingExtensions" /> class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OxyPlot.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using NSubstitute;

    using NUnit.Framework;

    /// <summary>
    /// Provides unit tests for the <see cref="RenderingExtensions" /> class.
    /// </summary>
    public class RenderingExtensionsTests
    {
        /// <summary>
        /// Tests the <see cref="RenderingExtensions.DrawClippedLine" /> extension method.
        /// </summary>
        public class DrawClippedLine
        {
            /// <summary>
            /// Given an empty array.
            /// </summary>
            [Test]
            public void EmptyArray()
            {
                var points = new ScreenPoint[0];
                var clippingRectangle = new OxyRect(0.3, -0.5, 0.5, 1);
                var rc = Substitute.For<IRenderContext>();
                var received = new List<ScreenPoint>();
                rc.DrawClippedLine(clippingRectangle, points, 1, OxyColors.Black, 1, EdgeRenderingMode.Adaptive, null, LineJoin.Miter, null, received.AddRange);
                Assert.AreEqual(0, received.Count);
            }
        }
    }
}
