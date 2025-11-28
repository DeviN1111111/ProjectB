using Dapper;
using Microsoft.Data.Sqlite;
using System.Text.Json;
static class FavoriteListAccess
{
    private const string ConnectionString = "Data Source=database.db";

    public static List<FavoriteListModel> GetAllFavoriteListsByUserId(int userId)
    {
        using var db = new SqliteConnection(ConnectionString);
        const string sql = "SELECT * FROM FavoriteList WHERE UserId = @UserId";
        return db.Query<FavoriteListModel>(sql, new { UserId = userId }).ToList();
    }
    public static void AddProductToList(int listId, string updatedJson)
    {
        using var db = new SqliteConnection(ConnectionString);

        const string sql = @"
            UPDATE FavoriteLists
            SET Products = @Products
            WHERE Id = @ListId;
        ";

        db.Execute(sql, new { Products = updatedJson, ListId = listId });
    }
    public static string? GetProductsJson(int listId)
    {
        using var db = new SqliteConnection(ConnectionString);

        const string sql = "SELECT Products FROM FavoriteLists WHERE Id = @Id;";
        return db.QuerySingleOrDefault<string>(sql, new { Id = listId });
    }
    public static void RemoveProductFromList(int listId, string updatedJson)
    {
        using var db = new SqliteConnection(ConnectionString);

        const string sql = @"
            UPDATE FavoriteLists
            SET Products = @Products
            WHERE Id = @ListId;
        ";

        db.Execute(sql, new { Products = updatedJson, ListId = listId });
    }
}   