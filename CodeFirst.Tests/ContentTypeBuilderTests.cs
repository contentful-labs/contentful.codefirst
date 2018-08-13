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
                    Assert.Equal("Person", f.ContentType.Name);
                    Assert.Equal("Person", f.ContentType.SystemProperties.Id);
                    Assert.Null(f.ContentType.DisplayField);
                    Assert.Null(f.ContentType.Description);
                    Assert.Equal(3, f.ContentType.Fields.Count);
                },
                (f) =>
                {
                    Assert.Equal("SomethingElse", f.ContentType.Name);
                    Assert.Equal("something", f.ContentType.SystemProperties.Id);
                    Assert.Equal("field1", f.ContentType.DisplayField);
                    Assert.Equal("Some description", f.ContentType.Description);
                    Assert.Equal(6, f.ContentType.Fields.Count);
                }
                );
        }

        [Fact]
        public async Task CreatingContentTypesShouldCallCorrectMethods()
        {
            //Arrange
            var contentTypeInfo = new ContentTypeInformation()
            {
                ContentType = new ContentType()
                {
                    Name = "Test",
                    SystemProperties = new SystemProperties { Id = "FFff", Version = 7 }
                }
            };

            var config = new ContentfulCodeFirstConfiguration();

            var client = new Mock<IContentfulManagementClient>();

            //Act
            var contentTypes = await ContentTypeBuilder.CreateContentTypes(new[] { contentTypeInfo }, config, client.Object);

            //Assert
            client.Verify(c => c.GetContentTypes(null, default(CancellationToken)), Times.Once, "Did not receive call to get all contentTypes.");
            client.Verify(c => c.CreateOrUpdateContentType(contentTypeInfo.ContentType, 
                null, 
                null, //We expect version to be null here since the content type did not previously exist.
                default(CancellationToken)), Times.Once, "Did not receive a call to update contenttype.");
            client.Verify(c => c.ActivateContentType(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), default(CancellationToken)), Times.Never,
                "Activate should not have been called since configuration value PublishAutomatically is false.");
        }

        [Fact]
        public async Task CreatingContentTypesShouldCallActivateMethodIfConfigIsSet()
        {
            //Arrange
            var contentTypeInfo = new ContentTypeInformation()
            {
                ContentType = new ContentType()
                {
                    Name = "Test",
                    SystemProperties = new SystemProperties { Id = "FFff", Version = 7 }
                }
            };
            var config = new ContentfulCodeFirstConfiguration { PublishAutomatically = true };

            var client = new Mock<IContentfulManagementClient>();
            client.Setup(c => c.CreateOrUpdateContentType(contentTypeInfo.ContentType, null, null, default(CancellationToken))).ReturnsAsync(contentTypeInfo.ContentType);

            //Act
            var contentTypes = await ContentTypeBuilder.CreateContentTypes(new[] { contentTypeInfo }, config, client.Object);

            //Assert
            client.Verify(c => c.GetContentTypes(null, default(CancellationToken)), Times.Once, "Did not receive call to get all contentTypes.");
            client.Verify(c => c.CreateOrUpdateContentType(contentTypeInfo.ContentType,
                null,
                null, //We expect version to be null here since the content type did not previously exist.
                default(CancellationToken)), Times.Once, "Did not receive a call to update contenttype.");
            client.Verify(c => c.ActivateContentType(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), default(CancellationToken)), Times.Once,
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
            Assert.Single(contentTypes);
            Assert.Equal(6, first.ContentType.Fields.Count);
            Assert.Single(first.ContentType.Fields[2].Validations);
            Assert.Single(first.ContentType.Fields[2].Items.Validations);
            Assert.IsType<SizeValidator>(first.ContentType.Fields[2].Validations[0]);
            Assert.IsType<LinkContentTypeValidator>(first.ContentType.Fields[2].Items.Validations[0]);
            Assert.Equal("Array", first.ContentType.Fields[2].Type);
            Assert.Equal("Entry", first.ContentType.Fields[2].Items.LinkType);
            Assert.Equal("Link", first.ContentType.Fields[2].Items.Type);
            Assert.Equal("Person", string.Join(",", (first.ContentType.Fields[2].Items.Validations[0] as LinkContentTypeValidator).ContentTypeIds));
            Assert.Equal("Text", first.ContentType.Fields[1].Type);
            Assert.Equal(4, first.ContentType.Fields[1].Validations.Count);
            Assert.Single(first.ContentType.Fields[4].Validations);
            Assert.IsType<DateRangeValidator>(first.ContentType.Fields[4].Validations[0]);
            Assert.Equal(3, first.ContentType.Fields[3].Validations.Count);
            Assert.IsType<FileSizeValidator>(first.ContentType.Fields[3].Validations[1]);
            Assert.Null((first.ContentType.Fields[3].Validations[1] as FileSizeValidator).Max);
            Assert.Equal(1048576, (first.ContentType.Fields[3].Validations[1] as FileSizeValidator).Min);
            Assert.IsType<ImageSizeValidator>(first.ContentType.Fields[3].Validations[2]);
            Assert.Null((first.ContentType.Fields[3].Validations[2] as ImageSizeValidator).MaxWidth);
            Assert.Null((first.ContentType.Fields[3].Validations[2] as ImageSizeValidator).MaxHeight);
            Assert.Equal(200, (first.ContentType.Fields[3].Validations[2] as ImageSizeValidator).MinWidth);
            Assert.Equal(200, (first.ContentType.Fields[3].Validations[2] as ImageSizeValidator).MinWidth);
            Assert.Empty(first.ContentType.Fields[5].Validations);
            Assert.Single(first.ContentType.Fields[5].Items.Validations);
            Assert.IsType<LinkContentTypeValidator>(first.ContentType.Fields[5].Items.Validations[0]);
            Assert.Equal("Array", first.ContentType.Fields[5].Type);
            Assert.Equal("Entry", first.ContentType.Fields[5].Items.LinkType);
            Assert.Equal("Link", first.ContentType.Fields[5].Items.Type);
            Assert.Equal("Person", string.Join(",", (first.ContentType.Fields[5].Items.Validations[0] as LinkContentTypeValidator).ContentTypeIds));

            Assert.Collection(first.ContentType.Fields[1].Validations,
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
            var firstField = first.ContentType.Fields.First();
            var secondField = first.ContentType.Fields.Skip(1).First();
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

        [Fact]
        public void CreatingAContentTypeShouldYieldCorrectAppearances()
        {
            //Arrange
            var type = typeof(TestClasses.ClassWithAttributes);

            //Act
            var contentTypes = ContentTypeBuilder.InitializeContentTypes(new[] { type });
            var first = contentTypes.First();
            //Assert
            Assert.Single(contentTypes);
            Assert.Collection(first.InterfaceControls, 
                (i) => { Assert.Equal("rating", i.WidgetId); },
                (i) => { Assert.Equal("singleLine", i.WidgetId); }
                );
        }
    }
}
