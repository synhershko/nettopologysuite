using System.Collections.Generic;
using GeoAPI.Geometries;

namespace GisSharpBlog.NetTopologySuite.Index.Quadtree
{
    //public abstract class NodeBase : NodeBase<object>
    //{}

    /// <summary>
    /// The base class for nodes in a <c>Quadtree</c>.
    /// </summary>
    public abstract class NodeBase<T> 
    {        
        /// <summary> 
        /// Returns the index of the subquad that wholly contains the given envelope.
        /// If none does, returns -1.
        /// </summary>
        /// <param name="env"></param>
        /// <param name="centre"></param>
        public static int GetSubnodeIndex(IEnvelope env, ICoordinate centre)
        {
            int subnodeIndex = -1;
            if (env.MinX >= centre.X)
            {
                if (env.MinY >= centre.Y) 
                    subnodeIndex = 3;
                if (env.MaxY <= centre.Y) 
                    subnodeIndex = 1;
            }
            if (env.MaxX <= centre.X)
            {
                if (env.MinY >= centre.Y) 
                    subnodeIndex = 2;
                if (env.MaxY <= centre.Y) 
                    subnodeIndex = 0;
            }
            return subnodeIndex;
        }

        /// <summary>
        /// 
        /// </summary>
        protected IList<T> items = new List<T>();

        /// <summary>
        /// subquads are numbered as follows:
        /// 2 | 3
        /// --+--
        /// 0 | 1
        /// </summary>
        protected Node<T>[] Subnode = new Node<T>[4];

        /// <summary>
        /// 
        /// </summary>
        public NodeBase() { }

        /// <summary>
        /// 
        /// </summary>
        public IList<T> Items
        {
            get
            {
                return items;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool HasItems
        {
            get
            {
                // return !items.IsEmpty; 
                if (items.Count == 0)
                    return false;
                return true;                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            items.Add(item);            
        }

        /// <summary> 
        /// Removes a single item from this subtree.
        /// </summary>
        /// <param name="itemEnv">The envelope containing the item.</param>
        /// <param name="item">The item to remove.</param>
        /// <returns><c>true</c> if the item was found and removed.</returns>
        public bool Remove(IEnvelope itemEnv, T item)
        {
            // use envelope to restrict nodes scanned
            if (!IsSearchMatch(itemEnv))
                return false;

            bool found = false;
            for (int i = 0; i < 4; i++)
            {
                if (Subnode[i] != null)
                {
                    found = Subnode[i].Remove(itemEnv, item);
                    if (found)
                    {
                        // trim subtree if empty
                        if (Subnode[i].IsPrunable)
                            Subnode[i] = null;
                        break;
                    }
                }
            }

            // if item was found lower down, don't need to search for it here
            if (found) 
                return found;

            // otherwise, try and remove the item from the list of items in this node
            if(items.Contains(item))
            {                
                items.Remove(item);
                found = true;
            }
            return found;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsPrunable
        {
            get
            {
                return !(HasChildren || HasItems);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool HasChildren
        {
            get
            {
                for (int i = 0; i < 4; i++)
                {
                    if (Subnode[i] != null)
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                bool isEmpty = true;
                if(items.Count != 0)
                    isEmpty = false;
                for (int i = 0; i < 4; i++)
                    if (Subnode[i] != null)
                        if (!Subnode[i].IsEmpty)
                            isEmpty = false;
                return isEmpty;
            }
        }
               
        /// <summary>
        /// Insert items in <c>this</c> into the parameter!
        /// </summary>
        /// <param name="resultItems">IList for adding items.</param>
        /// <returns>Parameter IList with <c>this</c> items.</returns>
        public IList<T> AddAllItems(ref IList<T> resultItems)
        {
            // this node may have items as well as subnodes (since items may not
            // be wholely contained in any single subnode
            // resultItems.addAll(this.items);
            foreach (T o in items)
                resultItems.Add(o);
            for (int i = 0; i < 4; i++)            
                if (Subnode[i] != null)
                    Subnode[i].AddAllItems(ref resultItems);                
            return resultItems;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchEnv"></param>
        /// <returns></returns>
        protected abstract bool IsSearchMatch(IEnvelope searchEnv);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchEnv"></param>
        /// <param name="resultItems"></param>
        public void AddAllItemsFromOverlapping(IEnvelope searchEnv, ref IList<T> resultItems)
        {
            if (!IsSearchMatch(searchEnv))
                return;

            // this node may have items as well as subnodes (since items may not
            // be wholely contained in any single subnode
            foreach (T o in this.items)
                resultItems.Add(o);

            for (int i = 0; i < 4; i++)            
                if (Subnode[i] != null)
                    Subnode[i].AddAllItemsFromOverlapping(searchEnv, ref resultItems);                            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchEnv"></param>
        /// <param name="visitor"></param>
        public void Visit(IEnvelope searchEnv, IItemVisitor<T> visitor)
        {
            if (!IsSearchMatch(searchEnv))
                return;

            // this node may have items as well as subnodes (since items may not
            // be wholely contained in any single subnode
            VisitItems(searchEnv, visitor);

            for (int i = 0; i < 4; i++)
                if (Subnode[i] != null)                
                    Subnode[i].Visit(searchEnv, visitor);                            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchEnv"></param>
        /// <param name="visitor"></param>
        private void VisitItems(IEnvelope searchEnv, IItemVisitor<T> visitor)
        {
            // would be nice to filter items based on search envelope, but can't until they contain an envelope
            for (IEnumerator<T> i = items.GetEnumerator(); i.MoveNext(); )            
                visitor.VisitItem(i.Current);            
        }
       
        /// <summary>
        /// 
        /// </summary>
        public int Depth
        {
            get
            {
                int maxSubDepth = 0;
                for (int i = 0; i < 4; i++)
                {
                    if (Subnode[i] != null)
                    {
                        int sqd = Subnode[i].Depth;
                        if (sqd > maxSubDepth)
                            maxSubDepth = sqd;
                    }
                }
                return maxSubDepth + 1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count    
        {
            get
            {
                int subSize = 0;
                for (int i = 0; i < 4; i++)
                    if (Subnode[i] != null)                    
                        subSize += Subnode[i].Count;                                    
                return subSize + items.Count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int NodeCount
        {
            get
            {
                int subSize = 0;
                for (int i = 0; i < 4; i++)
                    if (Subnode[i] != null)
                        subSize += Subnode[i].Count;
                return subSize + 1;
            }
        }
    }
}