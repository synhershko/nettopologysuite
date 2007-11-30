using System;
using GeoAPI.Coordinates;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;
using NPack.Interfaces;

namespace GisSharpBlog.NetTopologySuite.Operation.Relate
{
    /// <summary>
    /// Implements the <c>Relate()</c> operation on <see cref="Geometry{TCoordinate}"/>s.
    /// </summary>
    public class RelateOp<TCoordinate> : GeometryGraphOperation<TCoordinate>
         where TCoordinate : ICoordinate, IEquatable<TCoordinate>, IComparable<TCoordinate>, 
                             IComputable<TCoordinate>, IConvertible
    {
        public static IntersectionMatrix Relate(IGeometry<TCoordinate> a, IGeometry<TCoordinate> b)
        {
            RelateOp<TCoordinate> relOp = new RelateOp<TCoordinate>(a, b);
            IntersectionMatrix im = relOp.IntersectionMatrix;
            return im;
        }

        private RelateComputer<TCoordinate> relate = null;

        public RelateOp(IGeometry<TCoordinate> g0, IGeometry<TCoordinate> g1)
            : base(g0, g1)
        {
            relate = new RelateComputer<TCoordinate>(arg);
        }

        public IntersectionMatrix IntersectionMatrix
        {
            get { return relate.ComputeIntersectionMatrix(); }
        }
    }
}