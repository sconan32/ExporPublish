using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Expor.Utilities.Documentation
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Interface)]
    public class ReferenceAttribute : Attribute
    {
        public string Title { get; set; }
        public string Authors { get; set; }
        public string BookTitle { get; set; }
        public string Prefix { get; set; }
        public string Url { get; set; }

        public ReferenceAttribute() { }
        public ReferenceAttribute(string title,
            string authors, string booktitle,
            string prefix = "", string url = "")
        {
            this.Title = title;
            this.Authors = authors;
            this.BookTitle = booktitle;
            this.Prefix = prefix;
            this.Url = url;
        }
    }
}
