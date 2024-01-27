using bb_api.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Microsoft.EntityFrameworkCore;

namespace bb_api.Services;

public class FarmsService
{
    private readonly IMongoCollection<Farm> _farmsCollection;

    public FarmsService(
        IOptions<FarmDatabaseSettings> FarmDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            FarmDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            FarmDatabaseSettings.Value.DatabaseName);

        _farmsCollection = mongoDatabase.GetCollection<Farm>(
            FarmDatabaseSettings.Value.FarmsCollectionName);
    }

    public async Task<List<Farm>> GetAsync(int page, int pageSize, string name, string userId)
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

        var farms = await _farmsCollection.Find(filterBuilder)
                                .Skip((page - 1) * pageSize)
                                .Limit(pageSize)
                                .ToListAsync();

        return farms;
    }

    public async Task<Farm?> GetAsync(string id) =>
        await _farmsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<Farm> GetByFarmerIdAsync(string farmerId) =>
        await _farmsCollection.Find(x => x.FarmerId == farmerId).FirstOrDefaultAsync();

    public async Task CreateAsync(Farm newFarm) =>
        await _farmsCollection.InsertOneAsync(newFarm);

    public async Task UpdateAsync(string id, Farm updatedFarm) =>
        await _farmsCollection.ReplaceOneAsync(x => x.Id == id, updatedFarm);

    public async Task RemoveAsync(string id) =>
        await _farmsCollection.DeleteOneAsync(x => x.Id == id);
}