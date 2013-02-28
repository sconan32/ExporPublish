using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.Documentation
{

    public sealed class DocumentationUtil
    {
        /**
         * Get a useful title from a class, either by reading the
         * "title" annotation, or by using the class name.
         * 
         * @param c Class
         * @return title
         */
        public static String GetTitle(Type c)
        {
            TitleAttribute title = (TitleAttribute)c.GetCustomAttributes(typeof(TitleAttribute), false)[0];
            if (title != null && title.Title != "")
            {
                return title.Title;
            }
            return c.Name;
        }

        /**
         * Get a class description if defined, an empty string otherwise.
         * 
         * @param c Class
         * @return description or the emtpy string
         */
        public static String GetDescription(Type c)
        {
            DescriptionAttribute desc = (DescriptionAttribute)c.GetCustomAttributes(typeof(DescriptionAttribute), false)[0];
            if (desc != null)
            {
                return desc.Description;
            }
            return "";
        }

        /**
         * Get the reference annotation of a class, or {@code null}.
         * 
         * @param c Class
         * @return Reference or {@code null}
         */
        public static ReferenceAttribute GetReference(Type c)
        {
            ReferenceAttribute ref1 = (ReferenceAttribute)c.GetCustomAttributes(typeof(ReferenceAttribute), false)[0];
            return ref1;
        }
    }
}
