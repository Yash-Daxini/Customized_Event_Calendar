using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Domain.Interfaces
{
    internal interface IQuerySupplier
    {
        public List<SqlParameter> GenerateParameterList(Dictionary<string, object> parameters)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            foreach (var key in parameters.Keys)
            {
                sqlParameters.Add(new SqlParameter { ParameterName = key, Value = parameters[key] });
            }
            sqlParameters.Add(new SqlParameter { ParameterName = "@Id", SqlDbType = System.Data.SqlDbType.Int, Direction = System.Data.ParameterDirection.Output });
            return sqlParameters;
        }
        public string Read();
        public string ReadById();
        public string Create();
        public string Update();
        public string Delete();

    }
}