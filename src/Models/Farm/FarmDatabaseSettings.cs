namespace bb_api.Models;

public class FarmDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string FarmsCollectionName { get; set; } = null!;
}