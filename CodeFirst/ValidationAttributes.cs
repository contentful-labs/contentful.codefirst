using Contentful.Core.Search;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.CodeFirst
{
    /// <summary>
    /// Base class for validation attributes.
    /// </summary>
    public abstract class ContentfulValidationAttribute : Attribute
    {
        /// <summary>
        /// The helptext to be displayed for the validation message in Contentful.
        /// </summary>
        public string HelpText { get; set; }
    }

    /// <summary>
    /// Specifies that this property should have a size validation on Contentful.
    /// </summary>
    public class SizeAttribute : ContentfulValidationAttribute
    {
        /// <summary>
        /// The maximum number.
        /// </summary>
        public int Max { get; set; }

        /// <summary>
        /// The minimum number.
        /// </summary>
        public int Min { get; set; }
    }

    /// <summary>
    /// Specifies that this property should have a range validation in Contentful.
    /// </summary>
    public class RangeAttribute : ContentfulValidationAttribute
    {
        /// <summary>
        /// The maximum number in the range.
        /// </summary>
        public int Max { get; set; }

        /// <summary>
        /// The minimum number in the range.
        /// </summary>
        public int Min { get; set; }
    }

    /// <summary>
    /// Specifies that this property should have a content type validation in Contentful. Only applicable for Entry and Array of Entry fields.
    /// </summary>
    public class LinkContentTypeAttribute : ContentfulValidationAttribute
    {
        /// <summary>
        /// Creates a new instance of a LinkContentTypeAttribute.
        /// </summary>
        /// <param name="contentTypeIds">The ids of the content types to restrict the field for in Contentful.</param>
        public LinkContentTypeAttribute(params string[] contentTypeIds)
        {
            ContentTypeIds = contentTypeIds;
        }

        /// <summary>
        /// The ids of the content types to restrict the field for in Contentful.
        /// </summary>
        public string[] ContentTypeIds { get; set; }
    }

    /// <summary>
    /// Specifies that this property should have an in values validation in Contentful.
    /// </summary>
    public class InValuesAttribute : ContentfulValidationAttribute
    {
        /// <summary>
        /// Creates a new instance of InValuesAttribute.
        /// </summary>
        /// <param name="values">The values allowed for this field in Contentful.</param>
        public InValuesAttribute(params string[] values)
        {
            Values = values;
        }

        /// <summary>
        /// The values allowed for this field in Contentful.
        /// </summary>
        public string[] Values { get; set; }
    }

    /// <summary>
    /// Specifies that this property must be of a specific mime type.
    /// </summary>
    public class MimeTypeAttribute : ContentfulValidationAttribute
    {
        /// <summary>
        /// The mime type groups to restrict the field by in Contentful.
        /// </summary>
        public MimeTypeRestriction[] MimeTypes { get; set; }
    }

    /// <summary>
    /// Specifies that this property should have a regex validation in Contentful.
    /// </summary>
    public class RegexAttribute : ContentfulValidationAttribute
    {
        /// <summary>
        /// The expression the field must match in Contentful.
        /// </summary>
        public string Expression { get; set; }

        /// <summary>
        /// The flags of the expression.
        /// </summary>
        public string Flags { get; set; }
    }

    /// <summary>
    /// Specifies that this property should have a unique field validation in Contentful.
    /// </summary>
    public class UniqueAttribute : ContentfulValidationAttribute
    {
    }
}
