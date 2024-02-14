using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories
{
    internal class GenericRepository : BaseRepository
    {
        public List<T> Read<T>(Func<SqlDataReader, T> createObject)
        {
            List<T> list = new List<T>();
            string tableName = typeof(T).GetTypeInfo().Name;
            string query = QueryBuilder.Read<T>(tableName);
            Connect();
            ExecuteQuery(query);
            while (sqlDataReader.Read())
            {
                T data = createObject(sqlDataReader);
                list.Add(data);
            }
            Disconnect();
            return list;
        }
        public T? Read<T>(Func<SqlDataReader, T> createObject, int Id)
        {
            string tableName = typeof(T).GetTypeInfo().Name;
            string query = QueryBuilder.Read<T>(tableName, Id);
            Connect();
            ExecuteQuery(query);
            T? data = default(T);
            if (sqlDataReader.Read())
            {
                data = createObject(sqlDataReader);
                return data;
            }
            Disconnect();
            return data;
        }
        public int Create<T>(T data)
        {
            string tableName = typeof(T).GetTypeInfo().Name;
            string query = QueryBuilder.Create<T>(tableName, data);
            sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(new SqlParameter { ParameterName = "@Id", SqlDbType = System.Data.SqlDbType.Int, Direction = System.Data.ParameterDirection.Output });
            Connect();
            int Id = ExecuteNonQuery(query);
            Disconnect();
            return Id;
        }
        public void Update<T>(T data, int Id)
        {

            string tableName = typeof(T).GetTypeInfo().Name;
            string query = QueryBuilder.Update<T>(tableName, data, Id);

            Connect();
            ExecuteNonQuery(query);
            Disconnect();
        }
        public void Delete<T>(int Id)
        {

            string tableName = typeof(T).GetTypeInfo().Name;
            string query = QueryBuilder.Read<T>(tableName, Id);

            Connect();
            ExecuteNonQuery(query);
            Disconnect();
        }
    }
}