using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Fixture
{
    public class MongoDBFixture : IDisposable
    {
        public MongoClient Client { get; }
        private const string connString = "mongodb://localhost:27017/?readPreference=primary&ssl=false";
        public string DbName;
        public MongoDBFixture()
        {
            DbName = $"test_db_{Guid.NewGuid()}";
            MongoClientSettings mongoClientSettings = MongoClientSettings.FromConnectionString(connString);
            mongoClientSettings.LinqProvider = MongoDB.Driver.Linq.LinqProvider.V3;

            mongoClientSettings.WriteConcern = WriteConcern.WMajority;
            mongoClientSettings.ReadPreference = ReadPreference.PrimaryPreferred;
            var mongoClient = new MongoClient(mongoClientSettings);
            Client = mongoClient;
        }

        public void Dispose()
        {
            Client.DropDatabase(DbName);
        }
    }
}
