using DataCommon.Models;
using DataCommon;
using DataService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using MongoDB.Driver;
using System.Text.Json;
using static MongoDB.Bson.Serialization.Serializers.SerializerHelper;
using System.Drawing;
using System.Net.Sockets;

namespace DataService.Services
{
    public class TagServiceDb : ITagService
    {
        private readonly IDbClient _dbClient;
        private readonly IHttpClientFactory _httpClientFactory;

        public TagServiceDb(IDbClient dbClient, IHttpClientFactory httpClientFactory)
        {
            _dbClient = dbClient;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<DataCommon.Models.Tag> AddTag(DataCommon.Models.Tag tag)
        {
            await _dbClient.GetTagsCollection().InsertOneAsync(tag);
            return tag;
        }

        public async Task<List<DataCommon.Models.Tag>> AddTags(IEnumerable<DataCommon.Models.Tag> tags)
        {
            var lTags = tags.ToList();
            await _dbClient.GetTagsCollection().InsertManyAsync(lTags);
            return lTags;
        }

        public async Task<bool> DumpTags()
        {
            try
            {
                await _dbClient.DropAllCollections();
                return true;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return false;
            }

        }

        public async Task<bool> InitTags()
        {
            var count = await _dbClient.GetTagsCollection().EstimatedDocumentCountAsync();
            int popularCounter = 0;
            if (count != 0)
            {
                return false;
            }
            var client = _httpClientFactory.CreateClient("so_tags");


            var tasks = Enumerable.Range(1, 25).Select(idx => new
            {
                Id = idx,
                //From stackapps:
                //Pass this as key when making requests against the Stack Exchange API to receive a higher request quota.
                //This is not considered a secret,and may be safely embed in client side code or distributed binaries.
                Task = client.GetAsync($"https://api.stackexchange.com/2.3/tags?page={idx}&pagesize=100&order=desc&sort=popular&site=stackoverflow&key=8xo8Cvq07ksvtRNteUe6YQ((")
            }).ToList();

            var fetchResult = await Task.WhenAll(tasks.Select(t => t.Task));

            var deserializationTasks = fetchResult.Select(async task => JsonSerializer.Deserialize<SOWrapper<DataCommon.Models.Tag>>(await task.Content.ReadAsStringAsync()));

            var deserializationResult = await Task.WhenAll(deserializationTasks);

            var listofTags = deserializationResult
                .SelectMany(t => t?.Items ?? Enumerable.Empty<DataCommon.Models.Tag>().ToList())
                .ToList();

            popularCounter = listofTags.Aggregate(0, (acc, x) => acc + x.Count);

            listofTags.ForEach(t =>
            {
                t.Percentage = (t.Count * 1f) / popularCounter;
            });

            await this.AddTags(listofTags);

            return true;
        }

        public async Task<bool> IsContent()
        {
            return (await _dbClient.GetTagsCollection().EstimatedDocumentCountAsync()) > 0;
        }

        public async Task<PageResult> PagedSearch(int page, int size = 10, FilterEnum filter = FilterEnum.None)
        {
            SortDefinition<DataCommon.Models.Tag>? sort;


            sort = filter switch
            {
                FilterEnum.None => null,
                FilterEnum.NameAsc => Builders<DataCommon.Models.Tag>.Sort.Ascending(t => t.Name),
                FilterEnum.ParticipationAsc => Builders<DataCommon.Models.Tag>.Sort.Ascending(t => t.Percentage),
                FilterEnum.NameDesc => Builders<DataCommon.Models.Tag>.Sort.Descending(t => t.Name),
                FilterEnum.ParticipationDesc => Builders<DataCommon.Models.Tag>.Sort.Descending(t => t.Percentage),
                _ => null
            };



            var query = _dbClient.GetTagsCollection()
                .Find(_ => true);

            if (sort is not null)
            {
                query = query.Sort(sort);
            }

            var tagsTask = query.Skip(page * size).Limit(size).ToListAsync();
            var countTask = _dbClient.GetTagsCollection().EstimatedDocumentCountAsync();
            await Task.WhenAll(tagsTask, countTask);
            var tags = tagsTask.Result;
            var count = countTask.Result;

            return new PageResult(tags, count);
        }

    }
}
