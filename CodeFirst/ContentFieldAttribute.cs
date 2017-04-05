using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.CodeFirst
{
    /// <summary>
    /// Attribute specifying options for how a specific property should be stored in Contentful.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class ContentFieldAttribute : Attribute
    {
        /// <summary>
        /// The id of the field. Will default to the propertyname if not set.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The name of the field. Will default to the propertyname if not set.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The type to store this property as in Contentful. See <see cref="Contentful.Core.Models.Management.SystemFieldTypes"/> for a list of constants that can be used. 
        /// Will default to a type applicable for the property, e.g. text for string properties, Integer for int properties, Date for DateTime properties etc.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Whether or not this field should be disabled for editing.
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// Whether or not this field should support localization.
        /// </summary>
        public bool Localized { get; set; }

        /// <summary>
        /// Whether or not this field should be completely omitted from the API response from the Contentful Delivery API.
        /// </summary>
        public bool Omitted { get; set; }

        /// <summary>
        /// Whether or not this field is required.
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// If the field is an array and the ItemsType is Link what link type are the items in the array. Entry or Asset.
        /// </summary>
        public string ItemsLinkType { get; set; }

        /// <summary>
        /// If the field is an array, what are the types in the array. Symbol or Link.
        /// </summary>
        public string ItemsType { get; set; }

        /// <summary>
        /// If the field is a Link what type of link. Entry or Asset.
        /// </summary>
        public string LinkType { get; set; }
    }
}
