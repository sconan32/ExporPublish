using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Persistent
{

    public abstract class AbstractExternalizablePage : IExternalizablePage
    {
        /**
         * Serial version
         */
        //private static long serialVersionUID = 1;

        /**
         * The unique id if this page.
         */
        private int id;

        /**
         * The dirty flag of this page.
         */
        private bool dirty;

        /**
         * Empty constructor for Externalizable interface.
         */
        public AbstractExternalizablePage() :
            base()
        {
            this.id = -1;
        }

        /**
         * Returns the unique id of this Page.
         * 
         * @return the unique id of this Page
         */

        public int GetPageID()
        {
            return id;
        }

        /**
         * Sets the unique id of this Page.
         * 
         * @param id the id to be set
         */

        public void SetPageID(int id)
        {
            this.id = id;
        }

        /**
         * Returns true if this page is dirty, false otherwise.
         * 
         * @return true if this page is dirty, false otherwise
         */

        public bool IsDirty()
        {
            return dirty;
        }

        /**
         * Sets the dirty flag of this page.
         * 
         * @param dirty the dirty flag to be set
         */

        public void SetDirty(bool dirty)
        {
            this.dirty = dirty;
        }

        /**
         * The object implements the writeExternal method to save its contents by
         * calling the methods of DataOutput for its primitive values or calling the
         * writeObject method of MemoryStream for objects, strings, and arrays.
         * 
         * @param out the stream to write the object to
         * @throws java.io.IOException Includes any I/O exceptions that may occur
         * @serialData Overriding methods should use this tag to describe the data
         *             layout of this Externalizable object. List the sequence of
         *             element types and, if possible, relate the element to a
         *             public/protected field and/or method of this Externalizable
         *             class.
         */

        //public void writeExternal(MemoryStream sout)  {
        //  sout.writeInt(id);
        //}

        ///**
        // * The object implements the readExternal method to restore its contents by
        // * calling the methods of DataInput for primitive types and readObject for
        // * objects, strings and arrays. The readExternal method must read the values
        // * in the same sequence and with the same types as were written by
        // * writeExternal.
        // * 
        // * @param in the stream to read data from in order to restore the object
        // * @throws java.io.IOException if I/O errors occur
        // * @throws ClassNotFoundException If the class for an object being restored
        // *         cannot be found.
        // */

        //public void readExternal(ObjectInput sin)  {
        //  id = sin.readInt();
        //}

        /**
         * Returns a string representation of the object.
         * 
         * @return a string representation of the object
         */

        public override String ToString()
        {
            return (id.ToString());
        }

        /**
         * Indicates whether some other object is "equal to" this one.
         * 
         * @param o the object to be tested
         * @return true, if o is an AbstractNode and has the same id and the same
         *         entries as this node.
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

            AbstractExternalizablePage that = (AbstractExternalizablePage)o;

            return id == that.GetPageID();
        }

        /**
         * Returns as hash code value for this node the id of this node.
         * 
         * @return the id of this node
         */

        public override int GetHashCode()
        {
            return id;
        }


        public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
