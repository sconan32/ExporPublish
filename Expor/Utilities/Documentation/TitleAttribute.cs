using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.Documentation
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TitleAttribute : Attribute
    {
        public string Title { get; private set; }
        public TitleAttribute(string title)
        {
            this.Title = title;
        }

    }
}
