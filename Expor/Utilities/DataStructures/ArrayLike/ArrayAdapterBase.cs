using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.DataStructures.ArrayLike
{
    public abstract class ArrayAdapterBase<T> : IArrayAdapter
    {

        public abstract int Size(IEnumerable<T> array);

        public abstract T Get(IEnumerable<T> array, int off);

        int IArrayAdapter.Size(System.Collections.IEnumerable array)
        {
            return Size((IEnumerable<T>)array);
        }

        object IArrayAdapter.Get(System.Collections.IEnumerable array, int off)
        {
            return Get((IEnumerable<T>)array, off);
        }
    }
}
