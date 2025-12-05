using Microsoft.Data.Sqlite;

public interface IDatabaseFactory
{
    SqliteConnection GetConnection();
}