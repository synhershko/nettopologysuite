using System.Collections;
using System.Collections.Generic;
using GisSharpBlog.NetTopologySuite.Geometries;
using GisSharpBlog.NetTopologySuite.Noding;
#if SILVERLIGHT
using ArrayList = System.Collections.Generic.List<object>;
#endif

namespace GisSharpBlog.NetTopologySuite.GeometriesGraph
{
    /// <summary>
    /// Validates that a collection of <see cref="Edge"/> is correctly noded.
    /// Throws an appropriate exception if an noding error is found.
    /// </summary>
    public class EdgeNodingValidator
    {

        ///<summary>
        /// Checks whether the supplied <see cref="Edge"/>s are correctly noded. 
        ///</summary>
        /// <param name="edges">an enmeration of Edges.</param>
        /// <exception cref="TopologyException">If the SegmentStrings are not correctly noded</exception>
        public static void CheckValid(IEnumerable edges)
        {
            EdgeNodingValidator validator = new EdgeNodingValidator(edges);
            validator.CheckValid();
        }

        private static IEnumerable<ISegmentString> ToSegmentStrings(IEnumerable edges)
        {
            // convert Edges to SegmentStrings
            IList<ISegmentString> segStrings = new List<ISegmentString>();
            foreach (Edge e in edges)
                segStrings.Add(new BasicSegmentString(e.Coordinates, e));
            return segStrings;
        }

        private readonly FastNodingValidator _nv;

       ///<summary>
       /// Creates a new validator for the given collection of <see cref="Edge"/>s.
       /// </summary> 
       public EdgeNodingValidator(IEnumerable edges)
        {
            _nv = new FastNodingValidator(ToSegmentStrings(edges));
        }

        /// <summary>
        /// Checks whether the supplied edges are correctly noded. 
        /// </summary>
       /// <exception cref="TopologyException">If the SegmentStrings are not correctly noded</exception>
        public void CheckValid()
        {
            _nv.CheckValid();
        }
    }
}