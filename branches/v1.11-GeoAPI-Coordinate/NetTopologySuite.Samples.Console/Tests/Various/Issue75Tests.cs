using GeoAPI.Geometries;

namespace NetTopologySuite.Tests.Various
{
    using Geometries;
    using NUnit.Framework;

    [TestFixture]
    class Issue75Tests
    {

        [Test]
        public void EqualsThrowsInvalidCastExceptionBugFix()
        {
            Point point = new Point(1.0, 1.0);
            Coordinate coordinate = new Coordinate(-1.0, -1.0);
            bool condition = point.Equals(coordinate);
            Assert.IsFalse(condition);
        }
    }
}
