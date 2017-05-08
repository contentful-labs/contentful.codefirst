using Contentful.CodeFirst;
using Contentful.Core;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Moq;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CodeFirst.Tests
{
    public class ContentTypeBuilderTests
    {
        [Fact]
        public void LoadingContentTypesShouldYieldCorrectNumberOfTypes()
        {
            //Arrange

            //Act
            var types = ContentTypeBuilder.LoadTypes("CodeFirst.Tests").ToList();

            //Assert
            Assert.Collection(types, (f) => { Assert.Equal("ClassWithAttributes", f.Name); }, (f) => { Assert.Equal("Person", f.Name); });
        }

        [Fact]
        public void InitializingContentTypesShouldYieldCorrectResult()
        {
            //Arrange
            var types = ContentTypeBuilder.LoadTypes("CodeFirst.Tests").ToList();

            //Act
            var contentTypes = ContentTypeBuilder.InitializeContentTypes(types);

            //Assert
            Assert.Collection(contentTypes,
                (f) =>
                {
                    Assert.Equal("Person", f.Name);
                    Assert.Equal("Person", f.SystemProperties.Id);
                    Assert.Null(f.DisplayField);
                    Assert.Null(f.Description);
                    Assert.Equal(3, f.Fields.Count);
                },
                (f) =>
                {
                    Assert.Equal("SomethingElse", f.Name);
                    Assert.Equal("something", f.SystemProperties.Id);
                    Assert.Equal("field1", f.DisplayField);
                    Assert.Equal("Some description", f.Description);
                    Assert.Equal(5, f.Fields.Count);
                }
                );
        }

        [Fact]
        public async Task CreatingContentTypesShouldCallCorrectMethods()
        {
            //Arrange
            var contentType = new ContentType()
            {
                Name = "Test",
                SystemProperties = new SystemProperties { Id = "FFff", Version = 7 }
            };
            var config = new ContentfulCodeFirstConfiguration();

            var client = new Mock<IContentfulManagementClient>();

            //Act
            var contentTypes = await ContentTypeBuilder.CreateContentTypes(new[] { contentType }, config, client.Object);

            //Assert
            client.Verify(c => c.GetContentTypesAsync(null, default(CancellationToken)), Times.Once, "Did not receive call to get all contentTypes.");
            client.Verify(c => c.CreateOrUpdateContentTypeAsync(contentType, 
                null, 
                null, //We expect version to be null here since the content type did not previously exist.
                default(CancellationToken)), Times.Once, "Did not receive a call to update contenttype.");
            client.Verify(c => c.ActivateContentTypeAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), default(CancellationToken)), Times.Never,
                "Activate should not have been called since configuration value PublishAutomatically is false.");
        }

        [Fact]
        public async Task CreatingContentTypesShouldCallActivateMethodIfConfigIsSet()
        {
            //Arrange
            var contentType = new ContentType()
            {
                Name = "Test",
                SystemProperties = new SystemProperties { Id = "FFff", Version = 7 }
            };
            var config = new ContentfulCodeFirstConfiguration { PublishAutomatically = true };

            var client = new Mock<IContentfulManagementClient>();
            client.Setup(c => c.CreateOrUpdateContentTypeAsync(contentType, null, null, default(CancellationToken))).ReturnsAsync(contentType);

            //Act
            var contentTypes = await ContentTypeBuilder.CreateContentTypes(new[] { contentType }, config, client.Object);

            //Assert
            client.Verify(c => c.GetContentTypesAsync(null, default(CancellationToken)), Times.Once, "Did not receive call to get all contentTypes.");
            client.Verify(c => c.CreateOrUpdateContentTypeAsync(contentType,
                null,
                null, //We expect version to be null here since the content type did not previously exist.
                default(CancellationToken)), Times.Once, "Did not receive a call to update contenttype.");
            client.Verify(c => c.ActivateContentTypeAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), default(CancellationToken)), Times.Once,
                "Activate should have been called since configuration value PublishAutomatically is true.");
        }

        [Fact]
        public void CreatingContentTypeWithFieldValidationsShouldYieldCorrectResult()
        {
            //Arrange
            var type = typeof(TestClasses.ClassWithAttributes);

            //Act
            var contentTypes = ContentTypeBuilder.InitializeContentTypes(new[] { type });
            var first = contentTypes.First();
            //Assert
            Assert.Equal(1, contentTypes.Count());
            Assert.Equal(5, first.Fields.Count);
            Assert.Equal(2, first.Fields[2].Validations.Count);
            Assert.IsType<SizeValidator>(first.Fields[2].Validations[0]);
            Assert.IsType<LinkContentTypeValidator>(first.Fields[2].Validations[1]);
            Assert.Equal("Array", first.Fields[2].Type);
            Assert.Equal("Entry", first.Fields[2].Items.LinkType);
            Assert.Equal("Person", first.Fields[2].Items.Type);
            Assert.Equal("Text", first.Fields[1].Type);
            Assert.Equal(4, first.Fields[1].Validations.Count);
            Assert.Equal(1, first.Fields[4].Validations.Count);
            Assert.IsType<DateRangeValidator>(first.Fields[4].Validations[0]);

            Assert.Collection(first.Fields[1].Validations,
                (f) => { Assert.IsType<UniqueValidator>(f); },
                (f) => { Assert.IsType<RangeValidator>(f); },
                (f) => { Assert.IsType<InValuesValidator>(f); },
                (f) => { Assert.IsType<RegexValidator>(f); }
                );
        }

        [Fact]
        public void CreatingContentTypeWithValuesShouldYieldCorrectResult()
        {
            //Arrange
            var type = typeof(TestClasses.ClassWithAttributes);

            //Act
            var contentTypes = ContentTypeBuilder.InitializeContentTypes(new[] { type });
            var first = contentTypes.First();
            var firstField = first.Fields.First();
            var secondField = first.Fields.Skip(1).First();
            //Assert
            Assert.Equal("Symbol", firstField.Type);
            Assert.Equal("Text", secondField.Type);
            Assert.True(firstField.Omitted);
            Assert.False(secondField.Omitted);
            Assert.True(firstField.Disabled);
            Assert.False(secondField.Disabled);
            Assert.Equal("fieldOne", firstField.Id);
            Assert.Equal("Field2", secondField.Id);
            Assert.Equal("First field", firstField.Name);
            Assert.Equal("Field2", secondField.Name);
        }
    }
}
