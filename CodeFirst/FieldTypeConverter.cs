using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Contentful.CodeFirst
{
    /// <summary>
    /// Class to convert a <see cref="Type"/> to a suitable field type.
    /// </summary>
    public static class FieldTypeConverter
    {
        /// <summary>
        /// Converts a type to a suitable Contentful field type.
        /// </summary>
        /// <param name="type">The type to convert into a field type.</param>
        /// <returns>The field type.</returns>
        public static string Convert(Type type)
        {
            if (type == typeof(int) || type == typeof(int?))
            {
                return SystemFieldTypes.Integer;
            }

            if (type == typeof(string))
            {
                return SystemFieldTypes.Text;
            }

            if (type == typeof(float) || type == typeof(decimal) || type == typeof(double) || type == typeof(float?) || type == typeof(decimal?) || type == typeof(double?))
            {
                return SystemFieldTypes.Number;
            }

            if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return SystemFieldTypes.Date;
            }

            if (type == typeof(bool) || type == typeof(bool?))
            {
                return SystemFieldTypes.Boolean;
            }

            if (IsAsset(type) || IsEntry(type))
            {
                return SystemFieldTypes.Link;
            }

            if (typeof(ICollection).IsAssignableFrom(type))
            {
                return SystemFieldTypes.Array;
            }

            if (type == typeof(Location))
            {
                return SystemFieldTypes.Location;
            }

            return SystemFieldTypes.Object;
        }

        /// <summary>
        /// Converts a type to a LinkType.
        /// </summary>
        /// <param name="type">The type to convert.</param>
        /// <returns>The linktype or null.</returns>
        public static string ConvertLinkType(Type type)
        {
            if (IsAsset(type))
            {
                return SystemLinkTypes.Asset;
            }
            else if (IsEntry(type))
            {
                return SystemLinkTypes.Entry;
            }
            return null;
        }

        /// <summary>
        /// Converts a type to an itemtype for a Contentful array.
        /// </summary>
        /// <param name="type">The type to convert.</param>
        /// <returns>An itemtype or null.</returns>
        public static string ConvertItemType(Type type)
        {
            Type itemType = GetItemType(type);
            if (itemType != null)
            {
                string convertedItemType = Convert(itemType);

                // currently only a list of strings (short strings) or links are supported 
                if (convertedItemType != SystemFieldTypes.Link)
                    return SystemFieldTypes.Symbol;
                else
                    return convertedItemType;
            }
            return null;
        }

        /// <summary>
        /// Converts a type to an item link type.
        /// </summary>
        /// <param name="type">The type to convert.</param>
        /// <returns>The item link type or null.</returns>
        public static string ConvertItemLinkType(Type type)
        {
            Type itemType = GetItemType(type);
            if (itemType != null)
            {
                return ConvertLinkType(itemType);
            }
            return null;
        }

        /// <summary>
        /// Returns whether a type is an asset or not.
        /// </summary>
        /// <param name="type">The type to evaluate.</param>
        /// <returns>True if the type is an asset, else false.</returns>
        public static bool IsAsset(Type type)
        {
            return type == typeof(Asset) || type == typeof(ManagementAsset);
        }

        /// <summary>
        /// Returns whether a type is an Entry or not.
        /// </summary>
        /// <param name="type">The type to evaluate.</param>
        /// <returns>True if the type is an Entry, else false.</returns>
        public static bool IsEntry(Type type)
        {
            return (type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(Entry<>)) || type.GetTypeInfo().IsDefined(typeof(ContentTypeAttribute));
        }

        /// <summary>
        /// Gets the item type for a type.
        /// </summary>
        /// <param name="type">The type to get an item type for.</param>
        /// <returns>The item type or null.</returns>
        public static Type GetItemType(Type type)
        {
            if (typeof(ICollection).IsAssignableFrom(type))
            {
                Type itemType = null;
                if (type.IsConstructedGenericType)
                    itemType = type.GetGenericArguments().First();
                if (type.IsArray)
                    itemType = type.GetElementType();

                return itemType;
            }
            return null;
        }
    }
}
