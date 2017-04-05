using Contentful.CodeFirst;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace CodeFirst.Tests
{
    public class FieldTypeConverterTests
    {
        [Theory]
        [InlineData(typeof(int), "Integer")]
        [InlineData(typeof(int?), "Integer")]
        [InlineData(typeof(float), "Number")]
        [InlineData(typeof(float?), "Number")]
        [InlineData(typeof(decimal), "Number")]
        [InlineData(typeof(decimal?), "Number")]
        [InlineData(typeof(double), "Number")]
        [InlineData(typeof(double?), "Number")]
        [InlineData(typeof(string), "Text")]
        [InlineData(typeof(DateTime), "Date")]
        [InlineData(typeof(DateTime?), "Date")]
        [InlineData(typeof(bool), "Boolean")]
        [InlineData(typeof(bool?), "Boolean")]
        [InlineData(typeof(List<string>), "Array")]
        [InlineData(typeof(Array), "Array")]
        [InlineData(typeof(Dictionary<string,string>), "Array")]
        [InlineData(typeof(object), "Object")]
        [InlineData(typeof(InvalidTimeZoneException), "Object")]
        [InlineData(typeof(Asset), "Link")]
        [InlineData(typeof(ManagementAsset), "Link")]
        [InlineData(typeof(Entry<dynamic>), "Link")]
        [InlineData(typeof(Entry<int>), "Link")]
        public void ConvertingPrimitiveTypesShouldYieldCorrectResult(Type type, string exptected)
        {
            //Arrange

            //Act
            var result = FieldTypeConverter.Convert(type);
            //Assert
            Assert.Equal(exptected, result);
        }
    }
}
