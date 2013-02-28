using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Databases.Ids;

namespace Socona.Expor.Indexes.Tree
{

    public abstract class AbstractLeafEntry : ILeafEntry
    {
        /**
         * Holds the id of the object (node or data object) represented by this entry.
         */
        private IDbId id;

        /**
         * Empty constructor for serialization purposes.
         */
        public AbstractLeafEntry()
        {
            // empty constructor
        }

        /**
         * Provides a new AbstractEntry with the specified id.
         * 
         * @param id the id of the object (node or data object) represented by this
         *        entry.
         */
        protected AbstractLeafEntry(IDbId id)
        {
            this.id = id;
        }


        public virtual bool IsLeafEntry()
        {
            return true;
        }


        public IDbId GetDbId()
        {
            return id;
        }

        /**
         * Writes the id of the object (node or data object) that is represented by
         * this entry to the specified stream.
         */

        //public void writeExternal(MemoryStream sout) {
        //  sout.writeInt(id.IntegerID);
        //}

        ///**
        // * Restores the id of the object (node or data object) that is represented by
        // * this entry from the specified stream.
        // * 
        // * @throws ClassNotFoundException If the class for an object being restored
        // *         cannot be found.
        // */

        //public void readExternal(ObjectInput sin)  {
        //  this.id = DbIdUtil.ImportInt(sin.readInt());
        //}

        /**
         * Indicates whether some other object is "equal to" this one.
         * 
         * @param o the object to be tested
         * @return true, if o is an AbstractEntry and has the same id as this entry.
         */

        public override bool Equals(Object o)
        {
            if (this == o)
            {
                return true;
            }
            if (o == null || GetType() != o.GetType())
            {
                return false;
            }

            AbstractLeafEntry that = (AbstractLeafEntry)o;

            return id == that.id;
        }

        /**
         * Returns as hash code for the entry its id.
         * 
         * @return the id of the entry
         */

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        /**
         * Returns the id as a string representation of this entry.
         * 
         * @return a string representation of this entry
         */

        public override String ToString()
        {
            return "o_" + id;
        }

        public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
