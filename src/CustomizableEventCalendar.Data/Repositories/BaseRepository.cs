// Ignore Spelling: sql
using System.Data.SqlClient;
namespace CustomizableEventCalendar.src.CustomizableEventCalendar.Data.Repositories
{

    internal class BaseRepository
    {
        public SqlConnection connection { get; set; }
        public SqlCommand sqlCommand { get; set; }
        public SqlDataReader sqlDataReader { get; set; }
        public List<SqlParameter> sqlParameters { get; set; }

        public BaseRepository() => this.connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);

        public void Connect()
        {
            try
            {
                connection.Open();
            }
            catch (Exception)
            {
                throw new Exception("Oops! It seems there's an issue connecting to the database right now. We're working on it to get things back up and running smoothly. Thank you for your patience!");
            }
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
            try
            {
                this.sqlCommand = connection.CreateCommand();
                sqlCommand.CommandText = query;
                sqlDataReader = sqlCommand.ExecuteReader();
            }
            catch
            {
                throw new Exception("Sorry, we're unable to retrieve the data at the moment. Please try again later or contact support if the issue persists.");
            }
        }

        public int ExecuteNonQuery(string query)
        {
            try
            {
                this.sqlCommand = connection.CreateCommand();
                sqlCommand.CommandText = query;

                if (sqlParameters != null && sqlParameters.Count > 0)
                {
                    sqlCommand.Parameters.AddRange([.. sqlParameters]);
                }

                int Id = -1;

                sqlCommand.ExecuteNonQuery();

                if (sqlCommand.Parameters.Count > 0 && sqlCommand.Parameters["@Id"].Value != DBNull.Value)
                    Id = (int)sqlCommand.Parameters["@Id"].Value;

                sqlParameters = [];

                return Id;

            }
            catch
            {
                throw new Exception("Oops ! Operation unsuccessful. Please try again later.");
            }
        }

    }
}