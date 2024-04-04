using DataCommon.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCommon
{
    public class MongoDbClient : IDbClient
    {
        private readonly IMongoCollection<Models.Tag> _tags;
        private readonly IMongoDatabase _database;
        private readonly IMongoClient _client;

        public MongoDbClient(IConfiguration configuration)
        {
            var mongoConfiguration = configuration.GetSection("MongoDB").Get<MongoDatabaseSettings>();
            MongoClientSettings mongoClientSettings = MongoClientSettings.FromConnectionString(mongoConfiguration.ConnectionURI);
            mongoClientSettings.LinqProvider = MongoDB.Driver.Linq.LinqProvider.V3;

            mongoClientSettings.WriteConcern = WriteConcern.WMajority;
            mongoClientSettings.ReadPreference = ReadPreference.PrimaryPreferred;
            var mongoClient = new MongoClient(mongoClientSettings);
            _client = mongoClient;
            var database = _client.GetDatabase(mongoConfiguration!.DatabaseName);

            _database = database;

            _tags = database.GetCollection<Models.Tag>(IDbClient.TagCollectionName);
        }
        public MongoDbClient(string dbName, IMongoClient client)
        {
            _client = client;
            var database = _client.GetDatabase(dbName);

            _database = database;

            _tags = database.GetCollection<Models.Tag>(IDbClient.TagCollectionName);
            

            //if (mongoConfiguration.CreateIndexes) { CreateIndexes(); }
        }


        public async Task DropAllCollections()
        {
            await _database.DropCollectionAsync(IDbClient.TagCollectionName);
        }

        public IMongoDatabase GetDatabase() => _database;

        public IMongoCollection<Models.Tag> GetTagsCollection() => _tags;
    }
}
