﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Indexes.Tree
{

    public abstract class AbstractDirectoryEntry : IDirectoryEntry
    {
        /**
         * Holds the id of the object (node or data object) represented by this entry.
         */
        private int id;

        /**
         * Empty constructor for serialization purposes.
         */
        public AbstractDirectoryEntry()
        {
            // empty constructor
        }

        /**
         * Provides a new AbstractEntry with the specified id.
         * 
         * @param id the id of the object (node or data object) represented by this
         *        entry.
         */
        protected AbstractDirectoryEntry(int id)
        {
            this.id = id;
        }


        public bool IsLeafEntry()
        {
            return false;
        }

        /**
         * Returns the id of the node or data object that is represented by this
         * entry.
         * 
         * @return the id of the node or data object that is represented by this entry
         */

        public int GetPageID()
        {
            return id;
        }

        /**
         * Returns the id of the node or data object that is represented by this
         * entry.
         * 
         * @return the id of the node or data object that is represented by this entry
         */

        public int GetEntryID()
        {
            return id;
        }

        /**
         * Sets the id of the node or data object that is represented by this entry.
         * 
         * @param id the id to be set
         */
        // Should be set by the constructor, then .
        /*public  void setPageID(int id) {
          this.id = id;
        }*/

        /**
         * Writes the id of the object (node or data object) that is represented by
         * this entry to the specified stream.
         */

        //public void writeExternal(ObjectOutput out) throws IOException {
        //  out.writeInt(id);
        //}

        /**
         * Restores the id of the object (node or data object) that is represented by
         * this entry from the specified stream.
         * 
         * @throws ClassNotFoundException If the class for an object being restored
         *         cannot be found.
         */

        //public void readExternal(ObjectInput in) throws IOException, ClassNotFoundException {
        //  this.id = in.readInt();
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

            AbstractDirectoryEntry that = (AbstractDirectoryEntry)o;

            return id == that.id;
        }

        /**
         * Returns as hash code for the entry its id.
         * 
         * @return the id of the entry
         */

        public override int GetHashCode()
        {
            return id;
        }

        /**
         * Returns the id as a string representation of this entry.
         * 
         * @return a string representation of this entry
         */

        public override String ToString()
        {
            return "n_" + id;
        }

        public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
