﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using GeoAPI.Coordinates;
using GeoAPI.Utilities;
using NetTopologySuite.Coordinates;
using NPack;
using NPack.Interfaces;
using NUnit.Framework;
#if DOTNET35
using System.Linq;
#endif

namespace ManagedBufferedCoordinate2DTests
{
    [TestFixture]
    public class BufferedCoordinate2DFactoryTests
    {
        [Test]
        public void CreateFactorySucceeds()
        {
            BufferedCoordinate2DFactory factory = new BufferedCoordinate2DFactory();
            Assert.IsNotNull(factory);
        }

        [Test]
        public void CreateBufferedCoordinate2DSucceeds()
        {
            BufferedCoordinate2DFactory factory = new BufferedCoordinate2DFactory();
            BufferedCoordinate2D coord = factory.Create(5, 10);
            Assert.AreEqual(5, coord.X);
            Assert.AreEqual(10, coord.Y);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void Create3DCoordinateFails()
        {
            BufferedCoordinate2DFactory factory = new BufferedCoordinate2DFactory();
            factory.Create3D(5, 10, 15);
        }

        [Test]
        public void CreateCoordinateCopySucceeds()
        {
            BufferedCoordinate2DFactory factory = new BufferedCoordinate2DFactory();
            BufferedCoordinate2D coord = factory.Create(5, 10);
            BufferedCoordinate2D clone = factory.Create(coord);

            Assert.AreEqual(coord.X, clone.X);
            Assert.AreEqual(coord.Y, clone.Y);
        }

        [Test]
        public void CreateAffineTransformSucceeds()
        {
            BufferedCoordinate2DFactory factory = new BufferedCoordinate2DFactory();
            //AffineTransformMatrix<BufferedCoordinate2D> matrix = factory.CreateTransform();

            //Assert.IsNotNull(matrix);
        }

        [Test]
        public void CreateAffineTransformWithAxisOfRotationSucceeds()
        {
            BufferedCoordinate2DFactory factory = new BufferedCoordinate2DFactory();
            //AffineTransformMatrix<BufferedCoordinate2D> matrix = factory.CreateTransform();

            //Assert.IsNotNull(matrix);
        }

        [Test]
        public void CreateHomogenizedCoordinateSucceeds()
        {
            BufferedCoordinate2DFactory factory = new BufferedCoordinate2DFactory();
            BufferedCoordinate2D coord = factory.Create(5, 10);
            BufferedCoordinate2D homogeneous = factory.Homogenize(coord);

            Assert.AreEqual(2, coord.ComponentCount);
            Assert.AreEqual(3, homogeneous.ComponentCount);
        }

        [Test]
        public void CreateHomogenizedCoordinateStreamSucceeds()
        {
            BufferedCoordinate2DFactory factory = new BufferedCoordinate2DFactory();
            Random rnd = new MersenneTwister();
            Int32 count = 10000;

            IEnumerable<BufferedCoordinate2D> coordinates = generateInfiniteCoordinates(factory, rnd);

#if DOTNET35
            IEnumerable<BufferedCoordinate2D> homogeneous = factory.Homogenize(coordinates.Take(count));
#else
            IEnumerable<BufferedCoordinate2D> homogeneous = factory.Homogenize(Enumerable.Take(coordinates, 10000));
#endif

            foreach (BufferedCoordinate2D coord in homogeneous)
            {
                Assert.AreEqual(3, coord.ComponentCount);
                count--;
            }

            Assert.AreEqual(0, count);
        }

        [Test]
        public void CreateDeomogenizedCoordinateSucceeds()
        {
            BufferedCoordinate2DFactory factory = new BufferedCoordinate2DFactory();
            BufferedCoordinate2D coord = factory.Create(5, 10);
            BufferedCoordinate2D homogeneous = factory.Homogenize(coord);
            BufferedCoordinate2D dehomogeneous = factory.Dehomogenize(homogeneous);

            Assert.AreEqual(2, coord.ComponentCount);
            Assert.AreEqual(3, homogeneous.ComponentCount);
            Assert.AreEqual(2, dehomogeneous.ComponentCount);
        }

        [Test]
        public void CreateDehomogenizedCoordinateStreamSucceeds()
        {
            BufferedCoordinate2DFactory factory = new BufferedCoordinate2DFactory();
            Random rnd = new MersenneTwister();
            Int32 count = 10000;

            IEnumerable<BufferedCoordinate2D> homogeneous = generateInfiniteHomogeneousCoordinates(factory, rnd);
            IEnumerable<BufferedCoordinate2D> dehomogeneous = Enumerable.Take(factory.Dehomogenize(homogeneous), count);

            foreach (BufferedCoordinate2D coord in dehomogeneous)
            {
                Assert.AreEqual(2, coord.ComponentCount);
                count--;
            }

            Assert.AreEqual(0, count);
        }

        [Test]
        [Ignore("Need to implement")]
        public void CoorindateAddToBufferSucceeds()
        {
            IVector<DoubleComponent> vector;
        }

        [Test]
        [Ignore("Need to implement")]
        public void BufferedCoordinate2DAddToBufferSucceeds()
        {
            BufferedCoordinate2D vector;
        }

        [Test]
        [Ignore("Need to implement")]
        public void DoublesAddToBufferSucceeds()
        {

        }

        [Test]
        [Ignore("Need to implement")]
        public void ClearBufferSucceeds()
        {

        }

        [Test]
        [Ignore("Need to implement")]
        public void BufferContainsIVectorSucceeds()
        {

        }

        [Test]
        [Ignore("Need to implement")]
        public void BufferContainsBufferedCoordinate2DSucceeds()
        {

        }

        [Test]
        [Ignore("Need to implement")]
        public void CopyToSucceeds()
        {

        }

        [Test]
        [Ignore("Need to implement")]
        public void CountSucceeds()
        {

        }

        [Test]
        [Ignore("Need to implement")]
        public void FactorySucceeds()
        {

        }

        [Test]
        [Ignore("Need to implement")]
        public void IsReadOnlySucceeds()
        {

        }

        [Test]
        [Ignore("Need to implement")]
        public void GettingAndSettingMaximumSizeSucceeds()
        {

        }

        [Test]
        [Ignore("Need to implement")]
        public void RemoveSucceeds()
        {

        }

        [Test]
        [Ignore("Need to implement")]
        public void SizeIncreasedSucceeds()
        {

        }

        [Test]
        [Ignore("Need to implement")]
        public void SizeIncreasingSucceeds()
        {

        }

        [Test]
        [Ignore("Need to implement")]
        public void VectorChangedSucceeds()
        {

        }

        [Test]
        [Ignore("Need to implement")]
        public void VectorLengthSucceeds()
        {

        }

        [Test]
        [Ignore("Need to implement")]
        public void GettingAndSettingIndexerSucceeds()
        {

        }

        [Test]
        [Ignore("Need to implement")]
        public void EnumeratingVertexesSucceeds()
        {

        }

        [Test]
        public void CreatingMultipleCoordinatesWithTheSameValueReturnsExistingCoordinate()
        {
            BufferedCoordinate2DFactory factory = new BufferedCoordinate2DFactory();
            Random random = new MersenneTwister();

            for (Int32 i = 0; i < 100; i++)
            {
                switch (random.Next(0, 4))
                {
                    case 0:
                        factory.Create(0, 0);
                        break;
                    case 1:
                        factory.Create(10, 10);
                        break;
                    case 2:
                        factory.Create(0, 100);
                        break;
                    case 3:
                        factory.Create(100, 100);
                        break;
                    default:
                        break;
                }
            }

            Assert.AreEqual(4, factory.VectorBuffer.Count);
        }

        [Test]
        public void Creating1MDifferentCoordinatesDoesntKillPerformance()
        {
            BufferedCoordinate2DFactory factory = new BufferedCoordinate2DFactory();
            Random random = new MersenneTwister();
            Stopwatch timer = new Stopwatch();

            Int32[] times = new Int32[10];

            for (Int32 i = 0; i < 1000000; i++)
            {
                timer.Start();
                factory.Create(random.NextDouble(), random.NextDouble());
                timer.Stop();

                times[i % 10] = (Int32)timer.ElapsedMilliseconds;

                if (i >= 10)
                {
                    Double avg = Enumerable.Average(times);
                    Assert.Less(avg, 30.0);
                }

                timer.Reset();
            }
        }

        [Test]
        public void Creating1MRepeatedCoordinatesDoesntKillPerformance()
        {
            BufferedCoordinate2DFactory factory = new BufferedCoordinate2DFactory();
            Random random = new MersenneTwister();
            Stopwatch timer = new Stopwatch();

            Int32[] times = new Int32[10];

            for (Int32 i = 0; i < 1000000; i++)
            {
                timer.Start();
                factory.Create(random.Next(500, 1000), random.Next(100000, 100500));
                timer.Stop();

                times[i % 10] = (Int32)timer.ElapsedMilliseconds;

                if (i >= 10)
                {
                    Double avg = Enumerable.Average(times);
                    Assert.Less(avg, 30.0);
                }

                timer.Reset();
            }
        }

        [Test]
        public void BitResolutionSnapsCoordinatesToGrid()
        {
            BufferedCoordinate2DFactory factory = new BufferedCoordinate2DFactory();

            factory.BitResolution = 24;

            factory.Create(10, 10);
            factory.Create(10.000000003, 10.000000003);

            Assert.AreEqual(1, factory.VectorBuffer.Count);
        }

        #region Private helper methods

        private static IEnumerable<BufferedCoordinate2D> generateInfiniteCoordinates(BufferedCoordinate2DFactory factory, Random rnd)
        {
            // Give thanks for Enumerable.Take()!
            while (true)
            {
                yield return factory.Create(rnd.NextDouble(), rnd.NextDouble());
            }
        }

        private static IEnumerable<BufferedCoordinate2D> generateInfiniteHomogeneousCoordinates(BufferedCoordinate2DFactory factory, Random rnd)
        {
            return factory.Homogenize(generateInfiniteCoordinates(factory, rnd));
        }
        #endregion
    }
}
