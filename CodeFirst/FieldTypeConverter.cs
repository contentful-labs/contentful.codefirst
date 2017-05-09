using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using System;
using System.Collections;
using System.Collections.Generic;
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
            if(type == typeof(int) || type == typeof(int?))
            {
                return SystemFieldTypes.Integer;
            }

            if(type == typeof(string))
            {
                return SystemFieldTypes.Text;
            }

            if(type == typeof(float) || type == typeof(decimal) || type == typeof(double) || type == typeof(float?) || type == typeof(decimal?) || type == typeof(double?))
            {
                return SystemFieldTypes.Number;
            }

            if(type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return SystemFieldTypes.Date;
            }

            if(type == typeof(bool) || type == typeof(bool?))
            {
                return SystemFieldTypes.Boolean;
            }

            if(type == typeof(Asset) || type == typeof(ManagementAsset) || (type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(Entry<>)))
            {
                return SystemFieldTypes.Link;
            }

            if(typeof(ICollection).IsAssignableFrom(type))
            {
                return SystemFieldTypes.Array;
            }

            if (type == typeof(Location))
            {
                return SystemFieldTypes.Location;
            }

            return SystemFieldTypes.Object;
        }
    }
}
