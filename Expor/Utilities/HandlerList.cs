using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities
{
    public sealed class HandlerList<H>
    {
        /**
         * List with registered Handlers. The list is kept in backwards order, that is
         * the later entrys take precedence.
         */
        private List<KeyValuePair<Type, H>> handlers = new List<KeyValuePair<Type, H>>();

        /**
         * Insert a handler to the beginning of the stack.
         * 
         * @param restrictionClass restriction class
         * @param handler handler
         */
        public void InsertHandler(Type restrictionClass, H handler)
        {
            // note that the handlers list is kept in a list that is traversed in
            // backwards order.
            handlers.Add(new KeyValuePair<Type, H>(restrictionClass, handler));
        }

        /**
         * Find a matching handler for the given object
         * 
         * @param o object to find handler for
         * @return handler for the object. null if no handler was found.
         */
        public H GetHandler(Object o)
        {
            if (o == null)
            {
                return default(H);
            }
            // note that we start at the end of the list.
            int retval = handlers.FindLastIndex(
                (t) =>
                {
                    if (t.Key.IsInstanceOfType(o))
                    {
                        return true;
                    }
                    return false;
                }
            );
            if (retval >= 0)
            {
                return handlers[retval].Value;
            }
            return default(H);

        }
    }
}
