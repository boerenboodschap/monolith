using bb_api.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Microsoft.EntityFrameworkCore;

namespace bb_api.Services;

public class FarmsService
{
    private readonly IMongoCollection<Farm> _farmsCollection;

    public FarmsService()
    {
        var mongoClient = new MongoClient(
            Environment.GetEnvironmentVariable("MONGO_CONNECTIONSTRING") ?? "mongodb://root:example@localhost:27017/"
        );

        var mongoDatabase = mongoClient.GetDatabase(
            Environment.GetEnvironmentVariable("MONGO_DATABASE") ?? "BoerenBoodschap"
        );

        _farmsCollection = mongoDatabase.GetCollection<Farm>(
            Environment.GetEnvironmentVariable("MONGO_FARMS_COLLECTION") ?? "Farms"
        );
    }

    public async Task<List<Farm>> GetAsync(int page, int pageSize, string name, string userId, double posX, double posY, int radius)
    {
        var filterBuilder = Builders<Farm>.Filter.Empty;

        if (!string.IsNullOrWhiteSpace(name))
        {
            filterBuilder = Builders<Farm>.Filter.Where(x => x.Name.ToLower().Contains(name.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(userId))
        {
            filterBuilder = Builders<Farm>.Filter.Where(x => x.FarmerId.Equals(userId));
        }

        if (posX != 0 && posY != 0 && radius != 0)
        {
            // smart logic on filtering by location
        }

        var farms = await _farmsCollection.Find(filterBuilder)
                                .Skip((page - 1) * pageSize)
                                .Limit(pageSize)
                                .ToListAsync();

        return farms;
    }

    public async Task<Farm?> GetAsync(string id) =>
        await _farmsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<Farm?> GetByFarmerIdAsync(string farmerId) =>
        await _farmsCollection.Find(x => x.FarmerId == farmerId).FirstOrDefaultAsync();

    public async Task CreateAsync(Farm newFarm) =>
        await _farmsCollection.InsertOneAsync(newFarm);

    public async Task UpdateAsync(string id, Farm updatedFarm) =>
        await _farmsCollection.ReplaceOneAsync(x => x.Id == id, updatedFarm);

    public async Task RemoveAsync(string id) =>
        await _farmsCollection.DeleteOneAsync(x => x.Id == id);
}