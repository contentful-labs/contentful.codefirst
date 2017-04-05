using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.CodeFirst
{
    /// <summary>
    /// Indicates that a class should be used as a blueprint for a content type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class ContentTypeAttribute : Attribute
    {
        /// <summary>
        /// The id of the content type. Will default to the class name.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The name of the content type. Will default to the class name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The id of the field to use as a displayfield for the content type.
        /// </summary>
        public string DisplayField { get; set; }

        /// <summary>
        /// The description of the content type.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The order in which this content type should be created by the Contentful API. This can be relevant if you have content types depending on each other.
        /// </summary>
        public int Order { get; set; }
    }
}
