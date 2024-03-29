﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories
{

    internal class BaseRepository
    {
        public SqlConnection connection { get; set; }
        public SqlCommand sqlCommand { get; set; }
        public SqlDataReader sqlDataReader { get; set; }
        public List<SqlParameter> sqlParameters { get; set; }

        public BaseRepository()
        {
            this.connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
        }

        public void Connect()
        {
            connection.Open();
        }

        public void Disconnect()
        {
            if (connection == null) return;

            if (connection.State == System.Data.ConnectionState.Open)
            {
                connection.Close();
            }
        }

        public void ExecuteQuery(string query)
        {
            this.sqlCommand = connection.CreateCommand();
            sqlCommand.CommandText = query;
            sqlDataReader = sqlCommand.ExecuteReader();
        }

        public int ExecuteNonQuery(string query)
        {
            this.sqlCommand = connection.CreateCommand();
            sqlCommand.CommandText = query;

            if (sqlParameters != null)
            {
                if (sqlParameters.Count > 0)
                {
                    sqlCommand.Parameters.AddRange(sqlParameters.ToArray());
                }
            }

            int Id = -1;

            sqlCommand.ExecuteNonQuery();

            if (sqlCommand.Parameters.Count > 0 && sqlCommand.Parameters["@Id"].Value != DBNull.Value)
                Id = (int)sqlCommand.Parameters["@Id"].Value;

            sqlParameters = new List<SqlParameter>();

            return Id;
        }

    }
}