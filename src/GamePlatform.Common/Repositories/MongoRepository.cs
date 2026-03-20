using MongoDB.Driver;

namespace GamePlatform.Common.Repositories;


public class MongoRepository<T>(IMongoDatabase database, string collectionName) : IRepository<T> where T: IEntity
{
    private readonly IMongoCollection<T> _dbCollection = database.GetCollection<T>(collectionName);
    private readonly FilterDefinitionBuilder<T> _filterBuilder = Builders<T>.Filter;

    public async Task<IReadOnlyCollection<T>> GetAllAsync() =>
        await _dbCollection.Find(_filterBuilder.Empty).ToListAsync();

    public async Task<T?> GetAsync(Guid id) =>
        await _dbCollection.Find(_filterBuilder.Eq(e => e.Id, id)).FirstOrDefaultAsync();

    public async Task CreateAsync(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await _dbCollection.InsertOneAsync(entity);
    }

    public async Task UpdateAsync(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await _dbCollection.ReplaceOneAsync(_filterBuilder.Eq(e => e.Id, entity.Id), entity);
    }

    public async Task RemoveAsync(Guid id) =>
        await _dbCollection.DeleteOneAsync(_filterBuilder.Eq(e => e.Id, id));
}