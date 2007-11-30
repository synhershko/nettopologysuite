using System;
using System.Collections.Generic;
using GeoAPI.Coordinates;
using GisSharpBlog.NetTopologySuite.Noding;
using NPack.Interfaces;

namespace GisSharpBlog.NetTopologySuite.GeometriesGraph
{
    /// <summary>
    /// Validates that a collection of SegmentStrings is correctly noded.
    /// Throws an appropriate exception if an noding error is found.
    /// </summary>
    public class EdgeNodingValidator<TCoordinate>
        where TCoordinate : ICoordinate, IEquatable<TCoordinate>, IComparable<TCoordinate>,
                            IComputable<TCoordinate>, IConvertible
    {
        private static IEnumerable<SegmentString<TCoordinate>> toSegmentStrings(IEnumerable<Edge<TCoordinate>> edges)
        {
            foreach (Edge<TCoordinate> e in edges)
            {
                yield return new SegmentString<TCoordinate>(e.Coordinates, e);
            }
        }

        private readonly NodingValidator _nodingValidator;

        public EdgeNodingValidator(IEnumerable<Edge<TCoordinate>> edges)
        {
            _nodingValidator = new NodingValidator(toSegmentStrings(edges));
        }

        public void CheckValid()
        {
            _nodingValidator.CheckValid();
        }
    }
}