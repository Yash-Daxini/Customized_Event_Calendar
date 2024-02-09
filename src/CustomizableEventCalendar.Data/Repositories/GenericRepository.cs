using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Entities;
using CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Interfaces;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories
{
    internal class GenericRepository : BaseRepository
    {
        public List<T> Read<T>(IQuerySupplier queries, Func<SqlDataReader, T> createObject)
        {
            List<T> list = new List<T>();
            Connect();
            ExecuteQuery(queries.Read());
            while (sqlDataReader.Read())
            {
                T data = createObject(sqlDataReader);
                list.Add(data);
            }
            Disconnect();
            return list;
        }
        public T? Read<T>(IQuerySupplier queries, Func<SqlDataReader, T> createObject, int Id)
        {
            Connect();
            sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(new SqlParameter { ParameterName = "@Id", SqlDbType = System.Data.SqlDbType.Int, Value = Id });
            ExecuteQuery(queries.ReadById());
            T? data = default(T);
            if (sqlDataReader.Read())
            {
                data = createObject(sqlDataReader);
                return data;
            }
            Disconnect();
            return data;
        }
        public int Create<T>(IQuerySupplier queries, Dictionary<string, object> parameters)
        {
            Connect();

            sqlParameters = queries.GenerateParameterList(parameters);
            int Id = ExecuteNonQuery(queries.Create());

            Disconnect();
            return Id;
        }
        public void Update<T>(IQuerySupplier queries, Dictionary<string, object> parameters, int Id)
        {
            Connect();
            sqlParameters = queries.GenerateParameterList(parameters);
            sqlParameters.Add(new SqlParameter { ParameterName = "@UpdateId", SqlDbType = System.Data.SqlDbType.Int, Value = Id });

            ExecuteNonQuery(queries.Update());
            Disconnect();
        }
        public void Delete<T>(IQuerySupplier queries, int Id)
        {
            Connect();
            sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(new SqlParameter { ParameterName = "@Id", SqlDbType = System.Data.SqlDbType.Int, Value = Id });
            ExecuteNonQuery(queries.Delete());

            Disconnect();
        }
    }
}