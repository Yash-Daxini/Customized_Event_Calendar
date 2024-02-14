using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories
{
    internal class QueryBuilder
    {
        public static string FormatPropertyValue(object? value)
        {
            if (value == null)
            {
                return "NULL";
            }
            else if (value is string || value is DateTime)
            {
                return $"'{value}'";
            }
            else
            {
                return value.ToString();
            }
        }
        public static string Read<T>(string tableName)
        {
            PropertyInfo[] properties = [.. typeof(T).GetProperties()];
            string keys = string.Join(", ", properties.Select(property => property.Name));
            return $"SELECT {keys} FROM [dbo].[{tableName}]";
        }
        public static string Read<T>(string tableName, int id)
        {
            PropertyInfo[] properties = [.. typeof(T).GetProperties()];
            string keys = string.Join(", ", properties.Select(property => property.Name));
            return $"SELECT {keys} FROM [dbo].[{tableName}] WHERE id={id}";
        }
        public static string Create<T>(string tableName, T data)
        {
            PropertyInfo[] properties = typeof(T).GetProperties().Where(property => !Attribute.IsDefined(property, typeof(NotMappedAttribute)))
                                          .ToArray();
            string keys = string.Join(", ", properties.Select(property => property.Name));
            string values = string.Join(", ", properties.Select(property => FormatPropertyValue(property.GetValue(data))));

            return $"INSERT INTO [dbo].[{tableName}] ({keys}) VALUES ({values}) SET @Id = SCOPE_IDENTITY()";
        }
        public static string Update<T>(string tableName, T data, int id)
        {
            PropertyInfo[] properties = typeof(T).GetProperties().Where(property => !Attribute.IsDefined(property, typeof(NotMappedAttribute)))
                                          .ToArray();
            string keysValues = string.Join(", ", properties.Select(property => property.Name + "=" + FormatPropertyValue(property.GetValue(data))));

            return $"UPDATE [dbo].[{tableName}] SET {keysValues} WHERE id={id}";
        }
        public static string Delete<T>(string tableName, int id)
        {
            return $"DELETE FROM [dbo].[{tableName}] WHERE id={id}";
        }
    }
}