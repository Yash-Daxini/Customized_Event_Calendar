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

    internal class GenericRepository<T> : BaseRepository
    {

        public List<T> GetAll(Func<SqlDataReader, T> createObject)
        {
            List<T> list = [];
            string query = QueryBuilder.Read<T>();

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

        public T? GetById(Func<SqlDataReader, T> createObject, int Id)
        {
            string query = QueryBuilder.Read<T>(Id);

            Connect();

            ExecuteQuery(query);

            T? data = default(T);

            if (sqlDataReader.Read())
            {
                data = createObject(sqlDataReader);
                Disconnect();
                return data;
            }

            Disconnect();

            return data;
        }

        public int Insert(T data)
        {
            string query = QueryBuilder.Create<T>(data);

            sqlParameters =
            [
                new SqlParameter { ParameterName = "@Id", SqlDbType = System.Data.SqlDbType.Int, Direction = System.Data.ParameterDirection.Output },
            ];

            Connect();

            int Id = ExecuteNonQuery(query);

            Disconnect();

            return Id;
        }

        public void Update(T data, int Id)
        {
            string query = QueryBuilder.Update<T>(data, Id);

            Connect();

            ExecuteNonQuery(query);

            Disconnect();
        }

        public void Delete(int Id)
        {
            string query = QueryBuilder.Delete<T>(Id);

            Connect();

            ExecuteNonQuery(query);

            Disconnect();
        }
    }
}