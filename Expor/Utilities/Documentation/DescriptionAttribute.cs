using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.Documentation
{
    [AttributeUsage(AttributeTargets.Class)]
   public  class DescriptionAttribute:Attribute
    {
        public string Description { get; private set; }

        public DescriptionAttribute(string description)
        {
            this.Description = description;
        }
    }
}
