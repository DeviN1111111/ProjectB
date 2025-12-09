using Microsoft.Data.Sqlite;

public class SqliteDatabaseFactory : IDatabaseFactory
{
    private readonly string _connectionString;

    private SqliteConnection? _connection;

    public SqliteDatabaseFactory(string connectionString = "Data Source=database.db;Cache=Shared;")
    {
        _connectionString = connectionString;
    }

    public SqliteConnection GetConnection()
    {
        if(_connection == null)
        {
            _connection = new SqliteConnection(_connectionString);
            _connection.Open();
        }
        return _connection;
    }
}