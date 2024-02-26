using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories
{
    internal static class QueryBuilder
    {
        public static string FormatPropertyValue(object? value)
        {
            if (value == null)
            {
                return "NULL";
            }
            else if (value is string)
            {
                return $"'{value}'";
            }
            else if (value is DateTime)
            {
                return $"'{Convert.ToDateTime(value).ToString("yyyy-MM-ddThh:mm:ss tt")}'";
            }
            else if (value is DateOnly)
            {
                DateTime date = Convert.ToDateTime(value.ToString());
                return $"'{date.ToString("yyyy-MM-dd")}'";
            }
            else if (value is bool)
            {
                return Convert.ToBoolean(value) == true ? "1" : "0";
            }
            else
            {
                return value.ToString();
            }
        }
        public static string GetTableName<T>()
        {
            if (typeof(T).GetTypeInfo().Name.Equals("RecurrencePatternCustom")) return "RecurrencePattern";
            return typeof(T).GetTypeInfo().Name;
        }
        public static PropertyInfo[] GetProperties<T>()
        {
            PropertyInfo[] properties = [.. typeof(T).GetProperties()];
            return properties;
        }
        public static string Read<T>()
        {
            PropertyInfo[] properties = GetProperties<T>();

            string tableName = GetTableName<T>();

            string keys = string.Join(", ", properties.Select(property => property.Name));

            return $"SELECT {keys} FROM [dbo].[{tableName}]";
        }
        public static string Read<T>(int id)
        {
            PropertyInfo[] properties = GetProperties<T>();

            string tableName = GetTableName<T>();

            string keys = string.Join(", ", properties.Select(property => property.Name));

            return $"SELECT {keys} FROM [dbo].[{tableName}] WHERE id={id}";
        }
        public static string Create<T>(T data)
        {
            PropertyInfo[] properties = GetProperties<T>()
                                        .Where(property => !Attribute.IsDefined(property, typeof(NotMappedAttribute)))
                                        .ToArray();

            string tableName = GetTableName<T>();

            string keys = string.Join(", ", properties.Select(property => property.Name));
            string values = string.Join(", ", properties.Select(property => FormatPropertyValue(property.GetValue(data))));

            return $"INSERT INTO [dbo].[{tableName}] ({keys}) VALUES ({values}) SET @Id = SCOPE_IDENTITY()";
        }
        public static string Update<T>(T data, int id)
        {
            PropertyInfo[] properties = GetProperties<T>()
                                        .Where(property => !Attribute.IsDefined(property, typeof(NotMappedAttribute)))
                                        .ToArray();

            string tableName = GetTableName<T>();

            string keysValues = string.Join(", ", properties.Select(property => property.Name + "=" + FormatPropertyValue(property.GetValue(data))));

            return $"UPDATE [dbo].[{tableName}] SET {keysValues} WHERE id={id}";
        }
        public static string Delete<T>(int id)
        {
            string tableName = GetTableName<T>();

            return $"DELETE FROM [dbo].[{tableName}] WHERE id={id}";
        }
    }
}