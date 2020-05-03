// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SequenceHelperTests.cs" company="OxyPlot">
//   Copyright (c) 2020 OxyPlot contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OxyPlot.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using NUnit.Framework;
    using OxyPlot.Utilities;

    // ReSharper disable InconsistentNaming
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    [TestFixture]
    public class SequenceHelperTests
    {
        [Test]
        public void Double_IsEmpty()
        {
            var dsh = new DoubleSequenceHelper(false, -2.0, 2.0, false, false);
            Assert.IsTrue(dsh.IsEmpty);

            dsh.ObserveNext(0.0);
            Assert.IsFalse(dsh.IsEmpty);

            dsh.ObserveNext(1.0);
            Assert.IsFalse(dsh.IsEmpty);
        }

        [Test]
        public void Double_Count()
        {
            var dsh = new DoubleSequenceHelper(false, -2.0, 2.0, false, false);
            Assert.AreEqual(0, dsh.Count);

            dsh.ObserveNext(0.0);
            Assert.AreEqual(1, dsh.Count);

            dsh.ObserveNext(1.0);
            Assert.IsFalse(dsh.IsEmpty);
        }

        [Test]
        public void Double_PreserveMinAndMax()
        {
            // preserve minX
            var dsh = new DoubleSequenceHelper(false, -2.0, 2.0, true, false);
            dsh.ObserveNext(0.0);

            Assert.AreEqual(-2.0, dsh.Minimum);
            Assert.AreEqual(0.0, dsh.Maximum);

            // preserve minY
            dsh = new DoubleSequenceHelper(false, -2.0, 2.0, false, true);
            dsh.ObserveNext(0.0);

            Assert.AreEqual(0.0, dsh.Minimum);
            Assert.AreEqual(2.0, dsh.Maximum);

            // preserve both; extremes
            dsh = new DoubleSequenceHelper(false, double.MaxValue, double.MinValue, true, true);
            dsh.ObserveNext(0.0);

            Assert.AreEqual(0.0, dsh.Minimum);
            Assert.AreEqual(0.0, dsh.Maximum);
        }

        [Test]
        public void Double_Min()
        {
            var dsh = new DoubleSequenceHelper(false, -2.0, 2.0, false, false);
            Assert.AreEqual(-2.0, dsh.Minimum);

            dsh.ObserveNext(0.0);
            Assert.AreEqual(0.0, dsh.Minimum);

            dsh.ObserveNext(1.0);
            Assert.AreEqual(0.0, dsh.Minimum);

            dsh.ObserveNext(-1.0);
            Assert.AreEqual(-1.0, dsh.Minimum);

            dsh = new DoubleSequenceHelper(false, -2.0, 2.0, false, false);
            var rnd = new Random(1);
            var doubles = RandomDoubles(rnd, 100);
            foreach (var d in doubles)
                dsh.ObserveNext(d);
            Assert.AreEqual(doubles.Min(), dsh.Minimum);
        }

        [Test]
        public void Double_Max()
        {
            var dsh = new DoubleSequenceHelper(false, -2.0, 2.0, false, false);
            Assert.AreEqual(2.0, dsh.Maximum);

            dsh.ObserveNext(0.0);
            Assert.AreEqual(0.0, dsh.Maximum);

            dsh.ObserveNext(1.0);
            Assert.AreEqual(1.0, dsh.Maximum);

            dsh.ObserveNext(-1.0);
            Assert.AreEqual(1.0, dsh.Maximum);

            dsh = new DoubleSequenceHelper(false, -2.0, 2.0, false, false);
            var rnd = new Random(1);
            var doubles = RandomDoubles(rnd, 100);
            foreach (var d in doubles)
                dsh.ObserveNext(d);
            Assert.AreEqual(doubles.Max(), dsh.Maximum);
        }

        [Test]
        public void Double_IgnoreNaN()
        {
            var dsh = new DoubleSequenceHelper(false, -2.0, 2.0, false, false);

            Assert.AreEqual(true, dsh.CheckValid(1.0));
            Assert.AreEqual(false, dsh.CheckValid(double.NaN));

            // first value
            dsh.ObserveNext(double.NaN);
            Assert.AreEqual(0, dsh.Count);
            Assert.IsTrue(dsh.IsEmpty);

            // non-first value
            dsh.ObserveNext(0.0);
            Assert.AreEqual(1, dsh.Count);
            dsh.ObserveNext(double.NaN);
            Assert.AreEqual(1, dsh.Count);
        }

        [Test]
        public void Double_ThrowOnNaN()
        {
            var dsh = new DoubleSequenceHelper(true, -2.0, 2.0, false, false);

            Assert.AreEqual(true, dsh.CheckValid(1.0));
            Assert.Throws(typeof(ArgumentException), () => dsh.CheckValid(double.NaN));

            // first value
            Assert.Throws(typeof(ArgumentException), () => dsh.ObserveNext(double.NaN));
            Assert.AreEqual(0, dsh.Count);

            // non-first value
            dsh.ObserveNext(0.0);
            Assert.AreEqual(1, dsh.Count);
            Assert.Throws(typeof(ArgumentException), () => dsh.ObserveNext(double.NaN));
            Assert.AreEqual(1, dsh.Count);
        }

        [Test]
        public void Double_Monotonicty()
        {
            Assert.IsTrue(GetMonotonicty(new double [] { }).IsEmpty);
            Assert.IsTrue(GetMonotonicty(new double [] { }).IsNonDecreasing);
            Assert.IsTrue(GetMonotonicty(new double [] { }).IsStrictlyIncreasing);
            Assert.IsTrue(GetMonotonicty(new double [] { 0.0 }).IsConstant);
            Assert.IsTrue(GetMonotonicty(new double[] { 0.0, 0.0 }).IsConstant);
            Assert.IsTrue(GetMonotonicty(new double[] { 0.0, 0.0 }).IsNonDecreasing);
            Assert.IsFalse(GetMonotonicty(new double[] { 0.0, 0.0 }).IsStrictlyIncreasing);
            Assert.IsTrue(GetMonotonicty(new double [] { 0.0, 1.0, 0.0, 1.0 }).IsNotMonotonic);
            Assert.IsTrue(GetMonotonicty(new double[] { 0.0, 0.0, 1.0, 2.0, 3.0, 3.0, 4.0 }).IsNonDecreasing);
            Assert.IsFalse(GetMonotonicty(new double[] { 0.0, 0.0, 1.0, 2.0, 3.0, 3.0, 4.0 }).IsStrictlyIncreasing);
            Assert.IsTrue(GetMonotonicty(new double[] { 0.0, 1.0, 2.0, 3.0, 4.0 }).IsNonDecreasing);
            Assert.IsTrue(GetMonotonicty(new double[] { 0.0, 1.0, 2.0, 3.0, 4.0 }).IsStrictlyIncreasing);
            Assert.IsTrue(GetMonotonicty(new double[] { -0.0, -0.0, -1.0, -2.0, -3.0, -3.0, -4.0 }).IsNonIncreasing);
            Assert.IsFalse(GetMonotonicty(new double[] { -0.0, -0.0, -1.0, -2.0, -3.0, -3.0, -4.0 }).IsStrictlyDecreasing);
            Assert.IsTrue(GetMonotonicty(new double[] { -0.0, -1.0, -2.0, -3.0, -4.0 }).IsNonIncreasing);
            Assert.IsTrue(GetMonotonicty(new double[] { -0.0, -1.0, -2.0, -3.0, -4.0 }).IsStrictlyDecreasing);
        }

        private static List<double> RandomDoubles(Random rnd, int count)
        {
            var res = new List<double>(count);
            
            for (int i = 0; i < count; i++)
            {
                res.Add(rnd.NextDouble());
            }

            return res;
        }

        private static Monotonicity GetMonotonicty(IEnumerable<double> sequence)
        {
            var dsh = new DoubleSequenceHelper(false, double.NaN, double.NaN, false, false);

            foreach (var d in sequence)
            {
                dsh.ObserveNext(d);
            }

            return dsh.GetMonotonicity();
        }
    }
}
