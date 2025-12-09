using Dapper;
using Microsoft.Data.Sqlite;
using System;
public static class RestockHistoryAccess
{
    private static readonly IDatabaseFactory _sqlLiteConnection = new SqliteDatabaseFactory("Data Source=database.db");
    private static readonly SqliteConnection _connection = _sqlLiteConnection.GetConnection();

    public static void AddRestockEntry(RestockHistoryModel restockEntry)
    {
        _connection.Execute(@"INSERT INTO RestockHistory 
            (ProductID, QuantityAdded, RestockDate, CostPerUnit)
            VALUES (@ProductID, @QuantityAdded, @RestockDate, @CostPerUnit)", restockEntry);
    }
}