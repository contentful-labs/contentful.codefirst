using Contentful.Core.Search;
using Contentful.Core.Models.Management;
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


    /// <summary>
    /// Specifies that this property should have a date range validation in Contentful.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
    public class DateRangeAttribute : ContentfulValidationAttribute
    {
        /// <summary>
        /// The minimum date (yyyy-MM-dd)
        /// </summary>
        public string Min { get; set; }

        /// <summary>
        /// The maximum date (yyyy-MM-dd)
        /// </summary>
        public string Max { get; set; }
    }

    /// <summary>
    /// Specifies that this property should have a file size validation in Contentful.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
    public class FileSizeAttribute : ContentfulValidationAttribute
    {
        protected int? min = null;
        /// <summary>
        /// The minimum file size
        /// </summary>
        public int Min
        {
            get
            {
                return this.min.GetValueOrDefault();
            }
            set
            {
                this.min = value;
            }
        }

        protected int? max = null;

        /// <summary>
        /// The maximum file size
        /// </summary>
        public int Max
        {
            get
            {
                return this.max.GetValueOrDefault();
            }
            set
            {
                this.max = value;
            }
        }

        /// <summary>
        /// The unit to use for minimum file size. See <see cref="Contentful.Core.Models.Management.SystemFileSizeUnits"/> for a list of constants that can be used. 
        /// </summary>
        public string MinUnit { get; set; }

        /// <summary>
        /// The unit to use for maximum file size. See <see cref="Contentful.Core.Models.Management.SystemFileSizeUnits"/> for a list of constants that can be used. 
        /// </summary>
        public string MaxUnit { get; set; }

        public IFieldValidator Validator
        {
            get
            {
                return new FileSizeValidator(min, max, MinUnit, MaxUnit, HelpText);
            }
        }
    }
}
