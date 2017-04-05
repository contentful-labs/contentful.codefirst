using Contentful.CodeFirst;
using Contentful.Core.Models.Management;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeFirst.Tests.TestClasses
{
    [ContentType(Name = "SomethingElse", Id = "something", Description = "Some description", DisplayField = "field1", Order = 25)]
    public class ClassWithAttributes
    {
        [ContentField(Id = "fieldOne", Name = "First field", Omitted = true, Disabled = true, Localized = true, Required = true, Type = SystemFieldTypes.Symbol)]
        public int Field1 { get; set; }

        [Unique]
        [Range(Max = 10, Min = 2)]
        [InValues(Values = new [] { "banana", "pear", "tommy gun" })]
        [Regex(Expression = "ss", Flags = "gi")]
        public string Field2 { get; set; }

        [ContentField(Id="collection", ItemsLinkType="Entry", ItemsType = "Person")]
        [Size(HelpText = "Too many or too few!", Max = 5, Min = 2)]
        [LinkContentType("Person")]
        public List<Person> Field3 { get; set; }

        [IgnoreContentField]
        public int IgnoredProp { get; set; }
    }

    [ContentType]
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }
}
