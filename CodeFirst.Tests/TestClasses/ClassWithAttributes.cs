using Contentful.CodeFirst;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contentful.Core.Search;
using System;
using System.Collections.Generic;
using System.Text;
using static Contentful.CodeFirst.FieldAppearanceAttribute;

namespace CodeFirst.Tests.TestClasses
{
    [ContentType(Name = "SomethingElse", Id = "something", Description = "Some description", DisplayField = "field1", Order = 25)]
    public class ClassWithAttributes
    {
        [ContentField(Id = "fieldOne", Name = "First field", Omitted = true, Disabled = true, Localized = true, Required = true, Type = SystemFieldTypes.Symbol)]
        [RatingAppearance(7, "This film gets 5 out of 7 stars!")]
        public int Field1 { get; set; }

        [Unique]
        [Range(Max = 10, Min = 2)]
        [InValues(Values = new [] { "banana", "pear", "tommy gun" })]
        [Regex(Expression = "ss", Flags = "gi")]
        [FieldAppearance(SystemWidgetIds.SingleLine, "Help me!")]
        public string Field2 { get; set; }

        [ContentField(Id="collection", ItemsLinkType="Entry", ItemsType = "Link")]
        [Size(HelpText = "Too many or too few!", Max = 5, Min = 2)]
        [LinkContentType("Person")]
        public List<Person> Field3 { get; set; }

        [MimeType(MimeTypes = new MimeTypeRestriction[] { MimeTypeRestriction.Image })]
        [FileSize(Min = 1, MinUnit = SystemFileSizeUnits.MB)]
        [ImageSize(MinWidth = 200, MinHeight = 200)]
        public Asset Asset { get; set; }

        [IgnoreContentField]
        public int IgnoredProp { get; set; }

        [DateRange(Min = "2017-01-01")]
        public DateTime Field4 { get; set; }

        [ContentField(Id = "collection")]
        [LinkContentType("Person")]
        public List<Person> Field5 { get; set; }
    }

    [ContentType]
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }
}
