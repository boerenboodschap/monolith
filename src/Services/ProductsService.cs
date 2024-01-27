using bb_api.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Microsoft.EntityFrameworkCore;

namespace bb_api.Services;

public class ProductsService
{
    private readonly IMongoCollection<Product> _productsCollection;

    public ProductsService(
        IOptions<ProductDatabaseSettings> ProductDatabaseSettings)
    // {
    //     var mongoClient = new MongoClient(
    //         ProductDatabaseSettings.Value.ConnectionString);

    //     var mongoDatabase = mongoClient.GetDatabase(
    //         ProductDatabaseSettings.Value.DatabaseName);

    //     _productsCollection = mongoDatabase.GetCollection<Product>(
    //         ProductDatabaseSettings.Value.ProductsCollectionName);
    // }
    {
        var mongoClient = new MongoClient(
            Environment.GetEnvironmentVariable("MONGO_CONNECTIONSTRING") ?? "mongodb://root:example@localhost:27017/");

        var mongoDatabase = mongoClient.GetDatabase(
            "BoerenBoodschap");

        _productsCollection = mongoDatabase.GetCollection<Product>(
            "Products");
    }

    public async Task<List<Product>> GetAsync(int page, int pageSize, string name, string farmId)
    {
        var filterBuilder = Builders<Product>.Filter.Empty;

        if (!string.IsNullOrWhiteSpace(name))
        {
            filterBuilder = Builders<Product>.Filter.Where(x => x.Name.ToLower().Contains(name.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(farmId))
        {
            filterBuilder = Builders<Product>.Filter.Where(x => x.FarmId.Equals(farmId));
        }

        var products = await _productsCollection.Find(filterBuilder)
                                .Skip((page - 1) * pageSize)
                                .Limit(pageSize)
                                .ToListAsync();

        return products;
    }

    public async Task<Product?> GetAsync(string id) =>
        await _productsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<List<Product>> GetByFarmIdAsync(string farmId) =>
        await _productsCollection.Find(x => x.FarmId == farmId).ToListAsync();

    public async Task CreateAsync(Product newProduct) =>
        await _productsCollection.InsertOneAsync(newProduct);

    public async Task UpdateAsync(string id, Product updatedProduct) =>
        await _productsCollection.ReplaceOneAsync(x => x.Id == id, updatedProduct);

    public async Task RemoveAsync(string id) =>
        await _productsCollection.DeleteOneAsync(x => x.Id == id);
}