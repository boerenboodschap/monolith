namespace bb_api.Models;

public class ProductDatabaseSettings
{
    public string ConnectionString { get; set; } = Environment.GetEnvironmentVariable("MONGO_CONNECTIONSTRING") ?? "mongodb://root:example@localhost:27017/";

    public string DatabaseName { get; set; } = null!;

    public string ProductsCollectionName { get; set; } = null!;
}