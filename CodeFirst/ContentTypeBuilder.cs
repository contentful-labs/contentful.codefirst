using Contentful.Core;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Contentful.CodeFirst
{
    /// <summary>
    /// The main class of Contentful.CodeFirst. Allows you to configure and call the Contentful API to create content types from your code.
    /// </summary>
    public class ContentTypeBuilder
    {
        /// <summary>
        /// Loads all types with a <see cref="ContentTypeAttribute"/> in a given assembly.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly to load types from.</param>
        /// <returns>An enumerable of types loaded.</returns>
        public static IEnumerable<Type> LoadTypes(string assemblyName)
        {
            return LoadTypes(Assembly.Load(new AssemblyName(assemblyName)));
        }

        /// <summary>
        /// Loads all types with a <see cref="ContentTypeAttribute"/> in a given assembly.
        /// </summary>
        /// <param name="assembly">The assembly to load types from.</param>
        /// <returns>An enumerable of types loaded.</returns>
        public static IEnumerable<Type> LoadTypes(Assembly assembly)
        {
            var types = assembly.ExportedTypes.Where(c => c.GetTypeInfo().IsClass && c.GetTypeInfo().GetCustomAttribute<ContentTypeAttribute>() != null);

            return types;
        }

        /// <summary>
        /// Initializes content types ready to be sent to the Contentful API from a number of types.
        /// </summary>
        /// <param name="types">The types to be transformed into content types. Could be chained to the output of the <see cref="LoadTypes(Assembly)"/> method.</param>
        /// <returns>An enumerable of <see cref="ContentType"/> ready to be created/updated in Contentful.</returns>
        public static IEnumerable<ContentType> InitializeContentTypes(IEnumerable<Type> types)
        {
            types = types.OrderBy(c => c.GetTypeInfo().GetCustomAttribute<ContentTypeAttribute>()?.Order ?? 0);

            foreach (var type in types)
            {
                var attribute = type.GetTypeInfo().GetCustomAttribute<ContentTypeAttribute>();

                var id = attribute.Id ?? type.Name;
                var name = attribute.Name ?? type.Name;

                var displayField = attribute.DisplayField;
                var description = attribute.Description;

                var contentType = new ContentType()
                {
                    SystemProperties = new SystemProperties
                    {
                        Id = id
                    },

                    Name = name,

                    DisplayField = displayField,

                    Description = description,

                    Fields = new List<Field>()
                };
                foreach (var prop in type.GetProperties())
                {
                    if (prop.GetCustomAttribute<IgnoreContentFieldAttribute>() != null)
                    {
                        continue;
                    }

                    var fieldAttribute = prop.GetCustomAttribute<ContentFieldAttribute>() ?? new ContentFieldAttribute();
                    var field = new Field()
                    {
                        Id = fieldAttribute.Id ?? prop.Name,
                        Name = fieldAttribute.Name ?? prop.Name,
                        Type = fieldAttribute.Type ?? FieldTypeConverter.Convert(prop.PropertyType),
                        Disabled = fieldAttribute.Disabled,
                        Omitted = fieldAttribute.Omitted,
                        Required = fieldAttribute.Required,
                        LinkType = fieldAttribute.LinkType,
                        Validations = new List<IFieldValidator>()
                    };
                    var validationAttributes = prop.GetCustomAttributes<ContentfulValidationAttribute>();

                    foreach (var validation in validationAttributes)
                    {
                        if (validation is SizeAttribute)
                        {
                            field.Validations.Add(new SizeValidator((validation as SizeAttribute).Min, (validation as SizeAttribute).Max, validation.HelpText));
                        }

                        if (validation is RangeAttribute)
                        {
                            field.Validations.Add(new RangeValidator((validation as RangeAttribute).Min, (validation as RangeAttribute).Max, validation.HelpText));
                        }

                        if (validation is LinkContentTypeAttribute)
                        {
                            field.Validations.Add(new LinkContentTypeValidator((validation as LinkContentTypeAttribute).ContentTypeIds, validation.HelpText));
                        }

                        if (validation is InValuesAttribute)
                        {
                            field.Validations.Add(new InValuesValidator((validation as InValuesAttribute).Values, validation.HelpText));
                        }

                        if (validation is MimeTypeAttribute)
                        {
                            field.Validations.Add(new MimeTypeValidator((validation as MimeTypeAttribute).MimeTypes, validation.HelpText));
                        }

                        if (validation is RegexAttribute)
                        {
                            field.Validations.Add(new RegexValidator((validation as RegexAttribute).Expression, (validation as RegexAttribute).Flags, validation.HelpText));
                        }

                        if(validation is UniqueAttribute)
                        {
                            field.Validations.Add(new UniqueValidator());
                        }

                        if (validation is DateRangeAttribute)
                        {
                            field.Validations.Add(new DateRangeValidator((validation as DateRangeAttribute).Min, (validation as DateRangeAttribute).Max, validation.HelpText));
                        }

                        if (validation is FileSizeAttribute)
                        {
                            field.Validations.Add((validation as FileSizeAttribute).Validator);
                        }
                    }

                    if (typeof(ICollection).IsAssignableFrom(prop.PropertyType))
                    {
                        field.Items = new Schema()
                        {
                            LinkType = fieldAttribute.ItemsLinkType,
                            Type = fieldAttribute.ItemsType
                        };
                    }

                    contentType.Fields.Add(field);
                }

                yield return contentType;
            }
        }

        /// <summary>
        /// Creates a number of content types in Contentful.
        /// </summary>
        /// <param name="contentTypes">The content types to create.</param>
        /// <param name="configuration">The configuration for the creation process.</param>
        /// <param name="client">The optional client to use for creation.</param>
        /// <returns>A list of created or updated content types.</returns>
        public static async Task<List<ContentType>> CreateContentTypes(IEnumerable<ContentType> contentTypes, ContentfulCodeFirstConfiguration configuration, IContentfulManagementClient client = null)
        {
            var managementClient = client;

            if (managementClient == null)
            {
                var httpClient = new HttpClient();
                managementClient = new ContentfulManagementClient(httpClient, configuration.ApiKey, configuration.SpaceId);
            }

            var createdTypes = new List<ContentType>();
            
            var existingContentTypes = (await managementClient.GetContentTypesAsync()).ToList();

            if (configuration.ForceUpdateContentTypes == false)
            {
                //remove any pre-existing content types from the list to be created.
                contentTypes = contentTypes.Where(c => !existingContentTypes.Any(x => x.SystemProperties.Id == c.SystemProperties.Id));
            }

            foreach (var contentType in contentTypes)
            {
                //make sure to add correct version for existing content types
                contentType.SystemProperties.Version = existingContentTypes.FirstOrDefault(c => c.SystemProperties.Id == contentType.SystemProperties.Id)?.SystemProperties.Version;

                var createdContentType = await managementClient.CreateOrUpdateContentTypeAsync(contentType, version: contentType.SystemProperties.Version);

                if (configuration.PublishAutomatically)
                {
                    createdContentType = await managementClient.ActivateContentTypeAsync(createdContentType.SystemProperties.Id, createdContentType.SystemProperties.Version ?? 1);
                }

                createdTypes.Add(createdContentType);
            }

            return createdTypes;
        }

        /// <summary>
        /// Creates content types in Contentful from any types with a <see cref="ContentTypeAttribute"/> found in an Assembly.
        /// </summary>
        /// <param name="assemblyName">The assembly to load types from.</param>
        /// <param name="configuration">The configuration for the creation process.</param>
        /// <param name="client">The optional client to use for creation.</param>
        /// <returns>A list of created or updated content types.</returns>
        public static async Task<List<ContentType>> CreateContentTypesFromAssembly(string assemblyName, ContentfulCodeFirstConfiguration configuration, IContentfulManagementClient client = null)
        {
            var types = LoadTypes(assemblyName);
            var contentTypesToCreate = InitializeContentTypes(types);
            var createdContentTypes = await CreateContentTypes(contentTypesToCreate, configuration, client);
            return createdContentTypes;
        }
    }
}
