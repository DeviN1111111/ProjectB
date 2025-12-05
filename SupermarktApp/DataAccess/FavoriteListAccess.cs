using Dapper;
using Microsoft.Data.Sqlite;
static class FavoriteListAccess
{
    private static readonly IDatabaseFactory _sqlLiteConnection = new SqliteDatabaseFactory("Data Source=database.db");
    private static readonly SqliteConnection _connection = _sqlLiteConnection.GetConnection();

    public static List<FavoriteListModel> GetAllFavoriteListsByUserId(int userId)
    {
        const string sql = "SELECT * FROM FavoriteLists WHERE UserId = @UserId";
        return _connection.Query<FavoriteListModel>(sql, new { UserId = userId }).ToList();
    }
    public static void ChangeListName(int listId, string newName)
    {

        const string sql = @"
            UPDATE FavoriteLists
            SET Name = @NewName
            WHERE Id = @ListId;
        ";

        _connection.Execute(sql, new { NewName = newName, ListId = listId });
    }
    public static int CreateList(string name, int userId)
    {

        const string sql = @"
            INSERT INTO FavoriteLists (Name, UserId)
            VALUES (@Name, @UserId);
            SELECT last_insert_rowid();
        ";

        var id = _connection.ExecuteScalar<int>(sql, new { Name = name, UserId = userId });
        return id;
    }
    public static List<FavoriteListProductModel> GetProductsByListId(int listId)
    {

        const string sql = @"
            SELECT *
            FROM FavoriteListProducts
            WHERE FavoriteListId = @FavoriteListId;
        ";

        return _connection.Query<FavoriteListProductModel>(sql, new { FavoriteListId = listId }).ToList();
    }
    public static void AddProductToList(int listId, int productId, int quantity)
    {

        const string selectSql = @"
            SELECT Quantity
            FROM FavoriteListProducts
            WHERE FavoriteListId = @FavoriteListId AND ProductId = @ProductId;
        ";

        int? existingQuantity = _connection.QuerySingleOrDefault<int?>(
            selectSql,
            new { FavoriteListId = listId, ProductId = productId });

        if (existingQuantity is null)
        {
            const string insertSql = @"
                INSERT INTO FavoriteListProducts (FavoriteListId, ProductId, Quantity)
                VALUES (@FavoriteListId, @ProductId, @Quantity);
            ";

            _connection.Execute(insertSql, new
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

            _connection.Execute(updateSql, new
            {
                FavoriteListId = listId,
                ProductId = productId,
                Quantity = newQuantity
            });
        }
    }
    public static void RemoveProductFromList(int listId, int productId)
    {

        const string sql = @"
            DELETE FROM FavoriteListProducts
            WHERE FavoriteListId = @FavoriteListId AND ProductId = @ProductId;
        ";

        _connection.Execute(sql, new { FavoriteListId = listId, ProductId = productId });
    }
    public static void EditProductQuantity(
        List<FavoriteListProductModel> products,
        int listId)
    {

        const string sql = @"
            UPDATE FavoriteListProducts
            SET Quantity = @Quantity
            WHERE FavoriteListId = @FavoriteListId AND ProductId = @ProductId;
        ";

        foreach (var product in products)
        {
            _connection.Execute(sql, new
            {
                Quantity = product.Quantity,
                FavoriteListId = listId,
                ProductId = product.ProductId
            });
        }
    }
    public static void RemoveList(int listId)
    {

        const string sql = @"
            DELETE FROM FavoriteLists
            WHERE Id = @Id;
        ";

        _connection.Execute(sql, new { Id = listId });
    }
}   