# contentful.codefirst

[Contentful](https://www.contentful.com) is a content management platform for web applications, mobile apps and connected devices. It allows you to create, edit & manage content in the cloud and publish it anywhere via powerful API. Contentful offers tools for managing editorial teams and enabling cooperation between organizations.

This is a helper package that allows you to automatically create content types from your c# classes.

## Setup

We recommend you use the NuGet package manager to add Contentful.codefirst to your .Net application using the following command in your NuGet package manager console.

```csharp
Install-Package contentful.codefirst
```

## Usage

The codefirst package can inspect your assemblies, load suitable models and create content types. To mark a class as a content type add a `ContentType` attribute to it.

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

![contenttype](https://user-images.githubusercontent.com/1835323/32037997-3ffc4dc8-ba28-11e7-9fe6-2589dce43470.PNG)

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

To add validations to fields you add a `ValidationAttribute` to the property.

```csharp
[ContentType(Name = "A blogpost", Id = "blogPost", DisplayField = "title", Description = "A simple blog post content type")]
public class BlogPost {

  [ContentField(Id = "title", Name = "The title!", Type = SystemFieldTypes.Symbol, Disabled = false, Omitted = false, Localized = true,  Required = true)]
  [Unique]
  public string Title { get; set; }
  ...omitted for brevity.
}
```

This example adds a unique validation to the title field.

All possible validations in Contentful are supported with the following attributes.

- `Size` &mdash; to control the maximum and minimum size of a field.
- `Range` &mdash; to control that the value of a field is within a specific range.
- `LinkContentType` &mdash; to control that links to other entries are of a certain content type.
- `InValues` &mdash; to control that a fields value is in a given set of values.
- `MimeType` &mdash; to control that an asset field file is of a certain mime type group.
- `Regex` &mdash; to control that the value of a field adheres to the specified regular expression.
- `Unique` &mdash; to verify that the value of a field is unique amongst the entries.

## Links and collections

If you do not specify the `Type` for a field Contentful.CodeFirst will use the best suited type for your property. A string will be `Text` an int will be `Integer` and an arbitrary object will be `Object`. If the property is of type `ICollection` the type in Contentful will be `Array`.

When modeling a link to another entry you must specify the type yourself.

```csharp
[ContentType]
public class BlogPost {
  ...omitted for brevity.
  [ContentField(Type = SystemFieldTypes.Link, LinkType = "Entry")]
  public Author Author { get; set; }
}
```

This changes our Author property from a string to an Author and with a `ContentField` attribute we specify that this is a `Link` field to another entry. If we wanted to control what types of entries can be added to the field we'd add a `LinkContentType` attribute.

```csharp
[ContentType]
public class BlogPost {
  ...omitted for brevity.
  [ContentField(Type = SystemFieldTypes.Link, LinkType = "Entry")]
  [LinkContentType("author")]
  public Author Author { get; set; }
}
```

Collection of entries or assets works the same way, but you have to specify the type of items in the collection instead of for the field.

```csharp
[ContentType]
public class BlogPost {
  ...omitted for brevity.
  [ContentField(ItemsType = SystemFieldTypes.Link, ItemsLinkType = "Entry")]
  [LinkContentType("author")]
  public List<Author> Author { get; set; }
}
```

Note how the `Type` property of the `ContentField` attribute can be omitted as the type `Array` is inferred from the `List` type.

## Appearance of fields

If you wish to control how a field appears in the Contentful web app you can use the `FieldAppearance` attribute.

```csharp
[ContentType]
public class BlogPost {
  ...omitted for brevity.
  [FieldAppearance(extensionId: SystemWidgetIds.SingleLine, helpText: "The title of the blog post")]
  public string Title { get; set; }
}
```

There are specialized appearance attributes for boolean, ratings and datepicker fields respectively.

```csharp
[ContentType]
public class BlogPost {
  ...omitted for brevity.
  [BooleanAppearance(trueLabel: "Yes", falseLabel: "No", helpText: "Is this blog post published or not?")]
  public bool Published { get; set; }
}
```

```csharp
[ContentType]
public class Movie {
  [RatingAppearance(7, "This film gets 5 out of 7 stars!")]
  public int Rating { get; set; }
}
```

```csharp
[ContentType]
public class Movie {
  [DatePickerAppearance(dateFormat: EditorInterfaceDateFormat.timeZ, clockFormat: "am", helpText: "The release date.")]
  public DateTime ReleaseDate { get; set; }
}
```

You can easily create your own appearances by creating an attribute that inherits from `FieldAppearanceAttribute`.
