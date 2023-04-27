using System;
using System.Data.SqlClient;

namespace BankInter.DAO.BancoInter
{
    public class TransactSQL : IDisposable
    {
        private readonly SqlConnection _connection;

        private string _tableName;
        private string _columns;
        private string _values;
        private string _whereClause;
        private string _orderByClause;

        public TransactSQL(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
        }

        public TransactSQL Table(string tableName)
        {
            _tableName = tableName;
            return this;
        }

        public TransactSQL Columns(string columns)
        {
            _columns = columns;
            return this;
        }

        public TransactSQL Values(string values)
        {
            _values = values;
            return this;
        }

        public TransactSQL Where(string whereClause)
        {
            _whereClause = whereClause;
            return this;
        }

        public TransactSQL OrderBy(string orderByClause)
        {
            _orderByClause = orderByClause;
            return this;
        }

        public int Add(string v, string fl_gn, bool v1)
        {
            var query = $"INSERT INTO {_tableName} ({_columns}) VALUES ({_values})";
            var command = new SqlCommand(query, _connection);
            command.Parameters.AddWithValue("@v", v);
            command.Parameters.AddWithValue("@fl_gn", fl_gn);
            command.Parameters.AddWithValue("@v1", v1);
            _connection.Open();
            var result = command.ExecuteNonQuery();
            _connection.Close();
            return result;
        }

        public int Add(string v, string fl_gn)
        {
            var query = $"INSERT INTO {_tableName} ({_columns}) VALUES ({_values})";
            var command = new SqlCommand(query, _connection);
            command.Parameters.AddWithValue("@v", v);
            command.Parameters.AddWithValue("@fl_gn", fl_gn);
            _connection.Open();
            var result = command.ExecuteNonQuery();
            _connection.Close();
            return result;
        }

        public int Update(string v)
        {
            var query = $"UPDATE {_tableName} SET {_columns} {_whereClause}";
            var command = new SqlCommand(query, _connection);
            command.Parameters.AddWithValue("@v", v);
            _connection.Open();
            var result = command.ExecuteNonQuery();
            _connection.Close();
            return result;
        }

        public SqlDataReader Exec()
        {
            var query = $"SELECT {_columns} FROM {_tableName} {_whereClause} {_orderByClause}";
            var command = new SqlCommand(query, _connection);
            _connection.Open();
            var reader = command.ExecuteReader();
            return reader;
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}