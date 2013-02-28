using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Log;
using Socona.Expor.Persistent;
using Socona.Expor.Utilities.DataStructures;

namespace Socona.Expor.Databases.Ids.Int32DbIds
{

    /**
     * Database ID object.
     * 
     * While this currently is just an Integer, it should be avoided to store the
     * object IDs in regular integers to reduce problems if this API ever changes
     * (for example if someone needs to support {@code long}, it should not require
     * changes in too many places!)
     * 
     * In particular, a developer should not make any assumption of these IDs being
     * consistent across multiple results/databases.
     * 
     * @author Erich Schubert
     * 
     * @apiviz.landmark
     * @apiviz.composedOf DynamicSerializer
     * @apiviz.composedOf StaticSerializer
     */
    public class Int32DbId : IDbId
    {
        /**
         * The actual object ID.
         */
        readonly protected int id;

        /**
         * Constructor from integer id.
         * 
         * @param id integer id.
         */
        public Int32DbId(int id)
            : base()
        {

            this.id = id;
        }


        public IDbId GetDbId()
        {
            return this;
        }

        /**
         * Return the integer value of the object ID.
         * 
         * @return integer id
         */

        public int GetInt32ID()
        {
            return this.id;
        }


        public override String ToString()
        {
            return id.ToString();
        }

        public override int GetHashCode()
        {
            return id;
        }


        public override bool Equals(Object obj)
        {
            if (!(obj is Int32DbId))
            {
                if (obj is IDbIdRef)
                {
                    Logging.GetLogger(this.GetType()).Warning("Programming error: IDbId.equals(DBIDRef) is not well-defined. use sameDBID!");
                }
                return false;
            }
            Int32DbId other = (Int32DbId)obj;
            return this.id == other.id;
        }


        public bool SameDbId(IDbIdRef other)
        {
            return this.id == other.Int32Id;
        }


        public int CompareTo(IDbIdRef o)
        {
            int thisVal = this.id;
            int anotherVal = o.Int32Id;
            return (thisVal < anotherVal ? -1 : (thisVal == anotherVal ? 0 : 1));
        }


        public int CompareDbId(IDbIdRef o)
        {
            int thisVal = this.id;
            int anotherVal = o.Int32Id;
            return (thisVal < anotherVal ? -1 : (thisVal == anotherVal ? 0 : 1));
        }


        public IEnumerator<IDbId> GetEnumerator()
        {
            yield return GetDbId();
        }


        public IDbId Get(int i)
        {
            if (i != 0)
            {
                throw new IndexOutOfRangeException();
            }
            return this;
        }


        //public Iterator<IDbId> iterator() {
        //  return new Itr();
        //}

        public bool Contains(IDbIdRef o)
        {
            return o.Int32Id == id;
        }


        public int Count
        {
            get
            {
                return 1;
            }
        }


        public int BinarySearch(IDbIdRef key)
        {
            return (id == key.Int32Id) ? 0 : -1;
        }

        /**
         * Pseudo iterator for DBIDs interface.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        //protected class Itr implements Iterator<IDbId> {
        //  /**
        //   * Whether we've already returned our object.
        //   */
        //  boolean first = true;

        //  @Override
        //  public boolean hasNext() {
        //    return first == true;
        //  }

        //  @Override
        //  public IDbId next() {
        //    Debug.Assert (first);
        //    first = false;
        //    return IntegerDBID.this;
        //  }

        //  @Override
        //  public void remove() {
        //    throw new UnsupportedOperationException();
        //  }
        //}

        /**
         * Pseudo iterator for DBIDs interface.
         * 
         * @author Erich Schubert
         * 
         * @apiviz.exclude
         */
        //protected class DBIDItr implements DBIDIter {
        //  /**
        //   * Whether we've already returned our object.
        //   */
        //  boolean first = true;

        //  @Override
        //  public void advance() {
        //    first = false;
        //  }

        //  @Override
        //  public int getIntegerID() {
        //    return id;
        //  }

        //  @Override
        //  public IDbId getDBID() {
        //    return IntegerDBID.this;
        //  }

        //  @Override
        //  public boolean valid() {
        //    return first;
        //  }

        //  @Override
        //  public boolean equals(Object other) {
        //    if (other instanceof IDbId) {
        //      LoggingUtil.warning("Programming error detected: DBIDItr.equals(IDbId). Use sameDBID()!", new Throwable());
        //    }
        //    return super.equals(other);
        //  }

        //  @Override
        //  public boolean sameDBID(DBIDRef other) {
        //    return id == other.getIntegerID();
        //  }

        //  @Override
        //  public int compareDBID(DBIDRef o) {
        //    int anotherVal = o.getIntegerID();
        //    return (id < anotherVal ? -1 : (id == anotherVal ? 0 : 1));
        //  }
        //}

        //@Override
        //public boolean isEmpty() {
        //  return false;
        //}

        /**
         * Dynamic sized serializer, using varint.
         * 
         * @author Erich Schubert
         */
        public class DynamicSerializer : IByteBufferSerializer
        {
            /**
             * Constructor. Protected: use static instance!
             */
            internal DynamicSerializer()
                : base()
            {

            }


            public IDbId FromByteBuffer(ByteBuffer buffer)
            {
                return new Int32DbId(ByteArrayUtil.ReadSignedVarint(buffer));
            }


            public void ToByteBuffer(ByteBuffer buffer, IDbId obj)
            {
                ByteArrayUtil.WriteSignedVarint(buffer, ((Int32DbId)obj).id);
            }


            public int GetByteSize(IDbId obj)
            {
                return ByteArrayUtil.GetSignedVarintSize(((Int32DbId)obj).id);
            }

            public object FromByteBuffer(Type type, ByteBuffer buffer)
            {
                throw new NotImplementedException();
            }

            public void ToByteBuffer(ByteBuffer buffer, object o, Type t)
            {
                throw new NotImplementedException();
            }

            public int GetByteSize(object o, Type type)
            {
                throw new NotImplementedException();
            }
        }

        /**
         * Static sized serializer, using regular integers.
         * 
         * @author Erich Schubert
         */
        public class StaticSerializer : IFixedSizeByteBufferSerializer<IDbId>
        {
            /**
             * Constructor. Protected: use static instance!
             */
            internal StaticSerializer()
                : base()
            {

            }


            public IDbId FromByteBuffer(ByteBuffer buffer)
            {
                return new Int32DbId(buffer.GetInt32());
            }


            public void ToByteBuffer(ByteBuffer buffer, IDbId obj)
            {
                buffer.Write(((Int32DbId)obj).id);
            }


            public int GetByteSize(IDbId obj)
            {
                return GetFixedByteSize();
            }


            public int GetFixedByteSize()
            {
                return ByteArrayUtil.SIZE_INT;
            }

            public object FromByteBuffer(Type type, ByteBuffer buffer)
            {
                throw new NotImplementedException();
            }

            public void ToByteBuffer(ByteBuffer buffer, object o, Type t)
            {
                throw new NotImplementedException();
            }

            public int GetByteSize(object o, Type type)
            {
                throw new NotImplementedException();
            }
        }

        /**
         * The public instance to use for dynamic serialization.
         */
        public readonly static DynamicSerializer dynamicSerializer = new DynamicSerializer();

        /**
         * The public instance to use for static serialization.
         */
        public readonly static StaticSerializer staticSerializer = new StaticSerializer();

        public IDbId DbId
        {
            get { return this; }
        }

        public int Int32Id
        {
            get { return this.id; }
        }

        public bool IsSameDbId(IDbIdRef other)
        {
            return this.id == other.Int32Id;
        }

        //public int CompareDbId(IDbIdRef other)
        //{
        //    int thisVal = this.id;
        //    int otherVal = other.IntegerID;
        //    return thisVal.CompareTo(otherVal);
        //}


        public bool IsEmpty()
        {
            return this.Count <= 0;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            yield return this.id;
        }

        public IDbId this[int index]
        {
            get
            {
                if (index != 0)
                {
                    throw new IndexOutOfRangeException();
                }
                return this;
            }
            set
            {
                throw new InvalidOperationException();
            }
        }

        public int CompareTo(object obj)
        {
            if (obj is IDbIdRef)
            {
                return this.Int32Id.CompareTo(((IDbIdRef)obj).Int32Id);
            }
            throw new ArgumentException("Argument not capable with Int32DbId");
        }


        public int InternalGetIndex()
        {
            return id;
        }


        public IArrayDbIds Slice(int begin, int end)
        {
            throw new NotImplementedException();
        }
    }


}
