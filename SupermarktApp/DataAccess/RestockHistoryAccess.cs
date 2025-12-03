using Dapper;
using Microsoft.Data.Sqlite;
using System;
public static class RestockHistoryAccess
{
    private const string ConnectionString = "Data Source=database.db";

    public static void AddRestockEntry(RestockHistoryModel restockEntry)
    {
        using var db = new SqliteConnection(ConnectionString);
        db.Execute(@"INSERT INTO RestockHistory 
            (ProductID, QuantityAdded, RestockDate, CostPerUnit)
            VALUES (@ProductID, @QuantityAdded, @RestockDate, @CostPerUnit)", restockEntry);
    }
}