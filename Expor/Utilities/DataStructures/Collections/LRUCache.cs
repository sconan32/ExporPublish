using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.DataStructures.Collections
{
   
﻿// Copyright (C) 2009 Robert Rossney <rrossney@gmail.com>
    //
    // This program is free software: you can redistribute it and/or modify
    // it under the terms of the GNU General Public License as published by
    // the Free Software Foundation, either version 3 of the License, or
    // (at your option) any later version.
    // 
    // This program is distributed in the hope that it will be useful,
    // but WITHOUT ANY WARRANTY; without even the implied warranty of
    // MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    // GNU General Public License for more details.
    // 
    // You should have received a copy of the GNU General Public License
    // along with this program.  If not, see <http://www.gnu.org/licenses/>.


    public class LRUCache<K, T> : IDictionary<K, T>
    {

        private const int _DefaultCapacity = 1000;

        /// <summary>
        /// The default Capacity that the LRUCache uses if none is provided in the constructor.
        /// </summary>
        public static int DefaultCapacity { get { return _DefaultCapacity; } }

        // The list of items in the cache.  New items are added to the end of the list;
        // existing items are moved to the end when added; the items thus appear in
        // the list in the order they were added/used, with the least recently used
        // item being the first.  This is internal because the LRUCacheEnumerator
        // needs to access it.
        internal readonly LinkedList<KeyValuePair<K, T>> List = new LinkedList<KeyValuePair<K, T>>();

        // The index into the list, used by Add, Remove, and Contains.
        private readonly Dictionary<K, LinkedListNode<KeyValuePair<K, T>>> Index =
            new Dictionary<K, LinkedListNode<KeyValuePair<K, T>>>();

        // Add, Clear, CopyTo, and Remove lock on this object to keep them threadsafe.
        private readonly object Lock = new object();

        #region LRUCache Members

        /// <summary>
        /// Initializes a new instance of the LRUCache class that is empty and has the default
        /// capacity.
        /// </summary>
        public LRUCache() : this(_DefaultCapacity) { }

        /// <summary>
        /// Initializes a new instance of the LRUCache class that is empty and has the specified
        /// initial capacity.
        /// </summary>
        /// <param name="capacity"></param>
        public LRUCache(int capacity)
        {
            if (capacity < 0)
            {
                throw new InvalidOperationException("LRUCache capacity must be positive.");
            }
            Capacity = capacity;
        }

        /// <summary>
        /// Occurs when the LRUCache is about to discard its oldest item
        /// because its capacity has been reached and a new item is being added.  
        /// </summary>
        /// <remarks>The item has not been discarded yet, and thus is still contained in 
        /// the Oldest property.</remarks>
        public event EventHandler DiscardingOldestItem;

        /// <summary>
        /// The maximum number of items that the LRUCache can contain without discarding
        /// the oldest item when a new one is added.
        /// </summary>
        public int Capacity { get; private set; }

        /// <summary>
        /// The oldest (i.e. least recently used) item in the LRUCache.
        /// </summary>
        public KeyValuePair<K,T> Oldest
        {
            get
            {
                return List.First.Value;
            }
        }

        #endregion

        #region ICollection<T> Members

        /// <summary>
        /// Add an item to the LRUCache, making it the newest item (i.e. the last
        /// item in the list).  If the item is already in the LRUCache, it is moved to the end
        /// of the list and becomes the newest item in the LRUCache.
        /// </summary>
        /// <param name="item">The item that is being used.</param>
        /// <remarks>If the LRUCache has a nonzero capacity, and it is at its capacity, this 
        /// method will discard the oldest item, raising the DiscardingOldestItem event before 
        /// it does so.</remarks>
        public void Add(KeyValuePair<K, T> item)
        {
            lock (Lock)
            {
                if (Index.ContainsKey(item.Key))
                {
                    List.Remove(Index[item.Key]);
                    Index[item.Key] = List.AddLast(item);
                    return;
                }

                if (Count >= Capacity && Capacity != 0)
                {
                    EventHandler h = DiscardingOldestItem;
                    if (h != null)
                    {
                        h(this, new EventArgs());
                    }
                    Remove(Oldest);
                }
                Index.Add(item.Key, List.AddLast(item));

            }

        }

        /// <summary>
        /// Determines whether the LRUCache contains a specific value.
        /// </summary>
        /// <param name="item">The item to locate in the LRUCache.</param>
        /// <returns>true if the item is in the LRUCache, otherwise false.</returns>
        public bool Contains(KeyValuePair<K, T> item)
        {
            return Index.ContainsKey(item.Key);
        }

        /// <summary>
        /// Copies the elements of the LRUCache to an array, starting at a particular 
        /// array index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of
        /// items copied from the LRUCache.</param>
        /// <param name="arrayIndex">The index in array at which copying begins.</param>
        public void CopyTo(KeyValuePair<K,T>[] array, int arrayIndex)
        {
            lock (Lock)
            {
                foreach (var  item in this)
                {
                    array[arrayIndex++] = item;
                }
            }
        }

        /// <summary>
        /// Clear the contents of the LRUCache.
        /// </summary>
        public void Clear()
        {
            lock (Lock)
            {
                List.Clear();
                Index.Clear();
            }
        }

        /// <summary>
        /// Gets the number of items contained in the LRUCache.
        /// </summary>
        public int Count
        {
            get
            {
                return List.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the LRUCache is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Remove the specified item from the LRUCache.
        /// </summary>
        /// <param name="item">The item to remove from the LRUCache.</param>
        /// <returns>true if the item was successfully removed from the LRUCache,
        /// otherwise false.  This method also returns false if the item was not
        /// found in the LRUCache.</returns>
        public bool Remove(KeyValuePair<K, T> item)
        {
            lock (Lock)
            {
                if (Index.ContainsKey(item.Key))
                {
                    List.Remove(Index[item.Key]);
                    Index.Remove(item.Key);
                    return true;
                }
                return false;
            }
        }

        #endregion

        #region IEnumerable<T> Members

        /// <summary>
        /// Returns an enumerator that iterates through the items in the LRUCache.
        /// </summary>
        /// <returns>An IEnumerator object that may be used to iterate through the 
        /// LRUCache./></returns>
        IEnumerator<KeyValuePair<K, T>> IEnumerable<KeyValuePair<K, T>>.GetEnumerator()
        {
            LinkedListNode<KeyValuePair<K, T>> node = List.First;
            while (node != null)
            {
                yield return node.Value;
                node = node.Next;
            }
        }


        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through the items in the LRUCache.
        /// </summary>
        /// <returns>An LRUCacheEnumerator object that may be used it iterate through the 
        /// LRUCache./></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new LRUCacheEnumerator<K, T>(this);
        }

        #endregion
        #region IDictoionary
        public void Add(K key, T value)
        {
            this.Add(new KeyValuePair<K, T>(key, value));
        }

        public bool ContainsKey(K key)
        {
            return Index.ContainsKey(key);
        }

        public ICollection<K> Keys
        {
            get { return Index.Keys; }
        }

        public bool Remove(K key)
        {
            if (ContainsKey(key))
            {
                return this.Remove(Index[key].Value);
            }
            return false;
        }

        public bool TryGetValue(K key, out T value)
        {
            if (Index.ContainsKey(key))
            {
                value = Index[key].Value.Value;
                return true;
            }
            else
            {
                value = default(T);
                return false;
            }
        }

        public ICollection<T> Values
        {
            get
            {
                List<T> val = new List<T>();
                foreach (var id in Index)
                { val.Add(id.Value.Value.Value); }
                return val;
            }
        }

        public T this[K key]
        {
            get
            {
                return Index[key].Value.Value;
            }
            set
            {
               this.Add(new KeyValuePair<K,T>(key,value));
            }
        }


     



        #endregion
    }

}
