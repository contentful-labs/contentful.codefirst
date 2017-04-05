# contentful.codefirst

[https://www.contentful.com][1] is a content management platform for web applications, mobile apps and connected devices. It allows you to create, edit & manage content in the cloud and publish it anywhere via powerful API. Contentful offers tools for managing editorial teams and enabling cooperation between organizations.

This is a helper package that allows you to automatically create content types from your c# classes.

## Setup

We recommend you use the NuGet package manager to add Contentful.codefirst to your .Net application using the following command in your NuGet package manager console.

```csharp
Install-Package contentful.codefirst -prerelease
```

## Usage

The codefirst package can inspect your assemblies, load suitable models and create content types. To mark a class as a content type simply add a `ContentType` attribute to it.

```csharp
[ContentType]
public class BlogPost {
  public string Title { get; set; }
  public string Body { get; set; }
  public DateTime Published { get; set; }
  public string Author { get; set; }
}
```

This marks the `BlogPost` class as a content type and it can be automatically loaded by the `ContentTypeBuilder`.

```csharp
var configuration = new ContentfulCodeFirstConfiguration 
{
  ApiKey = "<management_api_key>",
  SpaceId = "<space_id>"
};

var createdContentTypes = ContentTypeBuilder.CreateContentTypesFromAssembly("AssemblyName", configuration);
```

Imagining that the `BlogPost` class would be part of the `AssemblyName` assembly this would create a content type in Contentful by the name BlogPost with 4 fields.

IMAGE OF CONTENT TYPE HERE

As you can see the name and id of the content type has defaulted to the name of class. You can control this by specifying a number of properties on the `ContentType` attribute.

```csharp
[ContentType(Name = "A blogpost", Id = "blogPost", DisplayField = "Title", Description = "A simple blog post content type")]
public class BlogPost {
  ...omitted for brevity.
}
```

Here we specify that the content type created from the `BlogPost` class should have the name "A blogpost" and the id "blogPost" as opposed to the default "BlogPost" for both values.
We also specify which field should be the displayfield for the content type and give the content type a description. All of this meta data will be passed along to the created content type.

In a similar manner you can control the fields created from the properties of a class.

```csharp
[ContentType(Name = "A blogpost", Id = "blogPost", DisplayField = "title", Description = "A simple blog post content type")]
public class BlogPost {

  [ContentField(Id = "title", Name = "The title!", Type = SystemFieldTypes.Symbol, Disabled = false, Omitted = false, Localized = true,  Required = true)]
  public string Title { get; set; }
  ...omitted for brevity.
}
```

We have now set the Id of the `Title` property to "title" and changed its name to "The title!". We've also specified a type for the field.
This corresponds to the [available field types in Contentful](https://www.contentful.com/developers/docs/concepts/data-model/#fields). We're using
the helper class `SystemFieldTypes` from the Contentful.Net SDK to select a suitable field type. We also specify that the field should not be disabled 
for editing or omitted from the API response. False are the default values for all properties which means they could be excluded if not set to true. We 
also set `Localized` and `Required` to true to make sure the title of our blogpost can be localized and is a mandatory field when creating entries.

By default all public properties of a class is turned into fields of your content type. If you wish to ignore a certain field decorate it with
a `IgnoreContentField` attribute.

```csharp
[ContentType]
public class BlogPost {
  ...omitted for brevity.
  [IgnoreContentField]
  public string InternalId { get; set;}
}
```

The `InternalId` property will now be ignored when creating fields for the content type.

## Validations

To be continued...
