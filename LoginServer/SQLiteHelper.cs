using System.Data;
using System.Data.SQLite;

namespace LoginServer
{
    class SQLiteHelper
    {
        static string _connectionString = "Data Source = " + LoginServer.Instance.ApplicationPath + "\\bin\\db\\Data.db";

        SQLiteConnection _connection;

        public SQLiteHelper()
        {
            _connection = new SQLiteConnection(_connectionString);

            LoginServer.Log(_connectionString);
        }
        public SQLiteHelper(string connectionString)
            : this()
        {
            _connectionString = connectionString;
        }

        public void Open()
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();
        }
        public void Close()
        {
            if (_connection.State != ConnectionState.Closed)
                _connection.Close();
        }

        public DataSet Query(string sql)
        {
            return Query(sql, null);
        }

        public DataSet Query(string sql, params SQLiteParameter[] parameters)
        {
            this.Open();
            SQLiteCommand command = ExcDbCommand(sql, parameters);
            DataSet ds = new DataSet();
            SQLiteDataAdapter da = new SQLiteDataAdapter(command);
            da.Fill(ds);
            this.Close();
            return ds;
        }

        public bool Exc(string sql)
        {
            return Exc(sql, null);
        }

        public bool Exc(string sql, params SQLiteParameter[] parameters)
        {
            SQLiteCommand command = ExcDbCommand(sql, parameters);
            this.Open();
            int result = command.ExecuteNonQuery();
            this.Close();

            return result > 0;
        }

        public SQLiteDataReader Read(string sql)
        {
            return Read(sql, null);
        }

        public SQLiteDataReader Read(string sql, params SQLiteParameter[] parameters)
        {
            SQLiteCommand command = ExcDbCommand(sql, parameters);
            this.Open();
            SQLiteDataReader reader = command.ExecuteReader();
            //this.Close();
            return reader;
        }

        SQLiteCommand ExcDbCommand(string sql, SQLiteParameter[] parameters)
        {
            SQLiteCommand command = new SQLiteCommand(sql, _connection);
            command.CommandType = CommandType.Text;
            if (parameters == null || parameters.Length == 0)
                return command;
            foreach (SQLiteParameter param in parameters)
            {
                if (param != null)
                    command.Parameters.Add(param);
            }
            return command;
        }

        public bool Search(string sql, SQLiteParameter[] parameters)
        {
            SQLiteCommand command = ExcDbCommand(sql, parameters);
            this.Open();
            object result = command.ExecuteScalar();
            this.Close();
            return result != null;
        }

        public bool Search(string sql)
        {
            return Search(sql, null);
        }
    }
}
