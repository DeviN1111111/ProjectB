using Dapper;
using Microsoft.Data.Sqlite;
static class FavoriteListAccess
{
    private const string ConnectionString = "Data Source=database.db";

    public static List<FavoriteListModel> GetAllFavoriteListsByUserId(int userId)
    {
        using var db = new SqliteConnection(ConnectionString);
        const string sql = "SELECT * FROM FavoriteLists WHERE UserId = @UserId";
        return db.Query<FavoriteListModel>(sql, new { UserId = userId }).ToList();
    }
    public static void ChangeListName(int listId, string newName)
    {
        using var db = new SqliteConnection(ConnectionString);

        const string sql = @"
            UPDATE FavoriteLists
            SET Name = @NewName
            WHERE Id = @ListId;
        ";

        db.Execute(sql, new { NewName = newName, ListId = listId });
    }
    public static int CreateList(string name, int userId)
    {
        using var db = new SqliteConnection(ConnectionString);

        const string sql = @"
            INSERT INTO FavoriteLists (Name, UserId)
            VALUES (@Name, @UserId);
            SELECT last_insert_rowid();
        ";

        var id = db.ExecuteScalar<int>(sql, new { Name = name, UserId = userId });
        return id;
    }
    public static List<FavoriteListProductModel> GetProductsByListId(int listId)
    {
        using var db = new SqliteConnection(ConnectionString);

        const string sql = @"
            SELECT *
            FROM FavoriteListProducts
            WHERE FavoriteListId = @FavoriteListId;
        ";

        return db.Query<FavoriteListProductModel>(sql, new { FavoriteListId = listId }).ToList();
    }
    public static void AddProductToList(int listId, int productId, int quantity)
    {
        using var db = new SqliteConnection(ConnectionString);

        const string selectSql = @"
            SELECT Quantity
            FROM FavoriteListProducts
            WHERE FavoriteListId = @FavoriteListId AND ProductId = @ProductId;
        ";

        int? existingQuantity = db.QuerySingleOrDefault<int?>(
            selectSql,
            new { FavoriteListId = listId, ProductId = productId });

        if (existingQuantity is null)
        {
            const string insertSql = @"
                INSERT INTO FavoriteListProducts (FavoriteListId, ProductId, Quantity)
                VALUES (@FavoriteListId, @ProductId, @Quantity);
            ";

            db.Execute(insertSql, new
            {
                FavoriteListId = listId,
                ProductId = productId,
                Quantity = quantity
            });
        }
        else
        {
            var newQuantity = existingQuantity.Value + quantity;

            const string updateSql = @"
                UPDATE FavoriteListProducts
                SET Quantity = @Quantity
                WHERE FavoriteListId = @FavoriteListId AND ProductId = @ProductId;
            ";

            db.Execute(updateSql, new
            {
                FavoriteListId = listId,
                ProductId = productId,
                Quantity = newQuantity
            });
        }
    }
    public static void RemoveProductFromList(int listId, int productId)
    {
        using var db = new SqliteConnection(ConnectionString);

        const string sql = @"
            DELETE FROM FavoriteListProducts
            WHERE FavoriteListId = @FavoriteListId AND ProductId = @ProductId;
        ";

        db.Execute(sql, new { FavoriteListId = listId, ProductId = productId });
    }
    public static void EditProductQuantity(
        List<FavoriteListProductModel> products,
        int listId)
    {
        using var db = new SqliteConnection(ConnectionString);

        const string sql = @"
            UPDATE FavoriteListProducts
            SET Quantity = @Quantity
            WHERE FavoriteListId = @FavoriteListId AND ProductId = @ProductId;
        ";

        foreach (var product in products)
        {
            db.Execute(sql, new
            {
                Quantity = product.Quantity,
                FavoriteListId = listId,
                ProductId = product.ProductId
            });
        }
    }
    public static void RemoveList(int listId)
    {
        using var db = new SqliteConnection(ConnectionString);

        const string sql = @"
            DELETE FROM FavoriteLists
            WHERE Id = @Id;
        ";

        db.Execute(sql, new { Id = listId });
    }
}   