using System;
using System.Collections.Generic;
using System.Diagnostics;
using GeoAPI.Coordinates;
using GeoAPI.Geometries;
using GeoAPI.Indexing;
using GisSharpBlog.NetTopologySuite.Geometries;
using GisSharpBlog.NetTopologySuite.Utilities;
using NPack.Interfaces;

namespace GisSharpBlog.NetTopologySuite.Index.Quadtree
{
    /// <summary>
    /// A Quadtree is a spatial index structure for efficient querying
    /// of 2D rectangles.  
    /// </summary>
    /// <remarks>
    /// <para>
    /// If other kinds of spatial objects
    /// need to be indexed they can be represented by their
    /// envelopes.
    /// </para>
    /// <para>
    /// The quadtree structure is used to provide a primary filter
    /// for range rectangle queries.  The Query() method returns a list of
    /// all objects which may intersect the query rectangle.  Note that
    /// it may return objects which do not in fact intersect.
    /// A secondary filter is required to test for exact intersection.
    /// Of course, this secondary filter may consist of other tests besides
    /// intersection, such as testing other kinds of spatial relationships.
    /// This implementation does not require specifying the extent of the inserted
    /// items beforehand.  It will automatically expand to accomodate any extent
    /// of dataset.
    /// </para>
    /// <para>
    /// This data structure is also known as an <c>MX-CIF quadtree</c>
    /// following the usage of Samet and others.
    /// </para>
    /// </remarks>
    public class Quadtree<TCoordinate, TItem> : ISpatialIndex<IExtents<TCoordinate>, TItem>, IEnumerable<TItem>
        where TItem : IBoundable<IExtents<TCoordinate>>
        where TCoordinate : ICoordinate, IEquatable<TCoordinate>, IComparable<TCoordinate>,
                            IComputable<Double, TCoordinate>, IConvertible
    {
        struct QuadTreeEntry : IBoundable<IExtents<TCoordinate>>
        {
            private readonly TItem _item;
            private readonly IExtents<TCoordinate> _bounds;

            public QuadTreeEntry(TItem item, IExtents<TCoordinate> bounds)
            {
                if (bounds == null)
                {
                    throw new ArgumentNullException("bounds");
                }

                _item = item;
                _bounds = bounds;
            }

            public TItem Item
            {
                get { return _item; }
            }

            #region IBoundable<IExtents<TCoordinate>> Members

            public IExtents<TCoordinate> Bounds
            {
                get { return _bounds; }
            }

            public Boolean Intersects(IExtents<TCoordinate> bounds)
            {
                return _bounds.Intersects(bounds);
            }

            #endregion
        }

        public static Int32 ComputeQuadLevel(IExtents<TCoordinate> extents)
        {
            Double dx = extents.GetSize(Ordinates.X);
            Double dy = extents.GetSize(Ordinates.Y);
            Double dMax = dx > dy ? dx : dy;
            Int32 level = DoubleBits.GetExponent(dMax) + 1;
            return level;
        }

        /// <summary>
        /// Ensure that the extents for the inserted item is non-zero.
        /// Use <paramref name="minExtent"/> to pad the envelope, if necessary.
        /// </summary>
        public static IExtents<TCoordinate> EnsureExtent(IExtents<TCoordinate> itemExtents, Double minExtent)
        {
            // The names "ensureExtent" and "minExtent" are misleading -- sounds like
            // this method ensures that the extents are greater than minExtent.
            // Perhaps we should rename them to "ensurePositiveExtent" and "defaultExtent".
            // [Jon Aquino]
            Double minx = itemExtents.GetMin(Ordinates.X);
            Double maxx = itemExtents.GetMax(Ordinates.X);
            Double miny = itemExtents.GetMin(Ordinates.Y);
            Double maxy = itemExtents.GetMax(Ordinates.Y);

            // has a non-zero extent
            if (minx != maxx && miny != maxy)
            {
                return itemExtents;
            }

            // pad one or both extents
            if (minx == maxx)
            {
                minx = minx - minExtent / 2.0;
                maxx = minx + minExtent / 2.0;
            }

            if (miny == maxy)
            {
                miny = miny - minExtent / 2.0;
                maxy = miny + minExtent / 2.0;
            }

            return new Extents<TCoordinate>(minx, maxx, miny, maxy);
        }

        private readonly Root<TCoordinate, QuadTreeEntry> _root = new Root<TCoordinate, QuadTreeEntry>();

        /// <summary>
        /// minExtent is the minimum envelope extent of all items
        /// inserted into the tree so far. It is used as a heuristic value
        /// to construct non-zero envelopes for features with zero X and/or Y extent.
        /// Start with a non-zero extent, in case the first feature inserted has
        /// a zero extent in both directions.  This value may be non-optimal, but
        /// only one feature will be inserted with this value.
        /// </summary>
        private Double _minExtent = 1.0;

        private Boolean _isDisposed = false;

        #region IDisposable Members

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            Dispose(true);
            GC.SuppressFinalize(this);
            _isDisposed = true;
        }

        #endregion

        public Boolean IsDiposed
        {
            get { return _isDisposed; }
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (_isDisposed)
            {
                return;
            }
        }

        public IExtents<TCoordinate> Bounds
        {
            get { return _root.Bounds; }
        }

        /// <summary> 
        /// Returns the number of levels in the tree.
        /// </summary>
        public Int32 Depth
        {
            get
            {
                Debug.Assert(_root != null);
                return _root.Depth;
            }
        }

        /// <summary> 
        /// Returns the number of items in the tree.
        /// </summary>
        public Int32 Count
        {
            get
            {
                Debug.Assert(_root != null);
                return _root.TotalItems;
            }
        }

        public void Insert(IExtents<TCoordinate> itemExtents, TItem item)
        {
            collectStats(itemExtents);
            IExtents<TCoordinate> insertExtents = EnsureExtent(itemExtents, _minExtent);
            _root.Insert(new QuadTreeEntry(item, itemExtents));
        }

        /// <summary> 
        /// Removes a single item from the tree.
        /// </summary>
        /// <param name="itemExtents">The <see cref="IExtents{TCoordinate}"/> of the item to remove.</param>
        /// <param name="item">The item to remove.</param>
        /// <returns><see langword="true"/> if the item was found.</returns>
        public Boolean Remove(IExtents<TCoordinate> itemExtents, TItem item)
        {
            IExtents<TCoordinate> posEnv = EnsureExtent(itemExtents, _minExtent);
            return _root.Remove(posEnv, new QuadTreeEntry(item, posEnv));
        }

        public IEnumerable<TItem> Query(IExtents<TCoordinate> query)
        {
            /*
            * the items that are matched are the items in quads which
            * overlap the search envelope
            */
            foreach (QuadTreeEntry entry in _root.Query(query))
            {
                yield return entry.Item;
            }
        }

        public IEnumerable<TItem> Query(IExtents<TCoordinate> query, Predicate<TItem> filter)
        {
            /*
            * the items that are matched are the items in quads which
            * overlap the search envelope
            */
            foreach (QuadTreeEntry entry in _root.Query(query))
            {
                if (filter(entry.Item))
                {
                    yield return entry.Item;
                }
            }
        }

        #region IEnumerable<TItem> Members

        public IEnumerator<TItem> GetEnumerator()
        {
            foreach (QuadTreeEntry entry in _root)
            {
                yield return entry.Item;
            }
        }

        #endregion

        private void collectStats(IExtents itemExtents)
        {
            Double delX = itemExtents.GetSize(Ordinates.X);

            if (delX < _minExtent && delX > 0.0)
            {
                _minExtent = delX;
            }

            Double delY = itemExtents.GetSize(Ordinates.Y);

            if (delY < _minExtent && delY > 0.0)
            {
                _minExtent = delY;
            }
        }

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ISpatialIndex<IExtents<TCoordinate>,TItem> Members

        public void Insert(TItem item)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TResult> Query<TResult>(IExtents<TCoordinate> bounds, Func<TItem, TResult> selector)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}