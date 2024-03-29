namespace bb_api.Models;

public class FarmDatabaseSettings
{
    public string ConnectionString { get; } = Environment.GetEnvironmentVariable("MONGO_CONNECTIONSTRING") ?? "mongodb://root:example@localhost:27017/";

    public string DatabaseName { get; set; } = null!;

    public string FarmsCollectionName { get; set; } = null!;
}