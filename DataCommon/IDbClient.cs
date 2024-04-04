using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataCommon
{
    public interface IDbClient
    {
        public const string TagCollectionName = "Tags";
        public static List<string> CollectionNames =>
        [
            TagCollectionName
        ];
        IMongoCollection<Models.Tag> GetTagsCollection();
        IMongoDatabase GetDatabase();
        Task DropAllCollections();

    }
}
