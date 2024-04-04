using Test.Fixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection.Extensions;
using DataCommon;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using WebAPI.Models;
using DataCommon.Models;
using Xunit.Abstractions;

namespace Test
{
    public class InetgrationTests : IClassFixture<MongoDBFixture>, IDisposable
    {
        private readonly MongoDBFixture _fixture;
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;
        public InetgrationTests(MongoDBFixture fixture, ITestOutputHelper output)
        {
            _output = output;
            _fixture = fixture;
            var appFactory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        services.RemoveAll<IDbClient>();
                        services.AddSingleton<IDbClient>(_ => new MongoDbClient(_fixture.DbName, _fixture.Client));
                    });
                });
            _client = appFactory.CreateClient();
        }

        public void Dispose()
        {
            _fixture.Dispose();
        }

        [Theory]
        [InlineData(3,3)]
        [InlineData(7, 10)]
        [InlineData(10, 43)]
        public async Task SkipAndGet_ShouldFirstFetchAllTagsFromSO_API(int size,int page)
        {

            // Arrange

            //Act
            var res = await _client.GetAsync($"/?pageSize={size}&pageNumber={page}");
            res.EnsureSuccessStatusCode();

            var content = await res.Content.ReadAsStringAsync();

            var tags = JsonSerializer.Deserialize<PageWrapper<Tag>>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            // Assert

            Assert.True(res.IsSuccessStatusCode);
            Assert.NotNull(tags);
            Assert.NotEmpty(tags.Content);
            Assert.Equal(size, tags.Content.Count());
            Assert.Equal(page, tags.PageNumber);
            Assert.Equal(2500, tags.TotalPageCount);

        }

        [Fact]
        public async Task Get100_SortedByParticipationDesc()
        {

            // Arrange


            //Act
            var res = await _client.GetAsync("/?pageSize=100&pageNumber=0&filter=ParticipationDesc");
            res.EnsureSuccessStatusCode();

            var content = await res.Content.ReadAsStringAsync();

            var tags = JsonSerializer.Deserialize<PageWrapper<Tag>>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            // Assert

            Assert.True(res.IsSuccessStatusCode);
            Assert.NotNull(tags);
            Assert.NotEmpty(tags.Content);
            Assert.Equal(100, tags.Content.Count());
            Assert.Equal(2500, tags.TotalPageCount);
            Assert.Equal(0, tags.PageNumber);

        }


        [Fact]
        public async Task RefetchTags_ShouldReturnTrue()
        {

            // Arrange


            //Act
            var res = await _client.GetAsync("/refetch");
            res.EnsureSuccessStatusCode();

            var content = await res.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<bool>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            // Assert

            Assert.True(res.IsSuccessStatusCode);
            Assert.NotNull(result);

        }


        [Fact]
        public async Task GetFirst10TagsSortedByNameDesc_ShouldFirstFetchNextRefetchAndThen_Skip275TagsAndTake25SortedByNameAsc()
        {

            // Arrange


            //Act
            var res1 = await _client.GetAsync("/?pageSize=10&pageNumber=0&filter=NameDesc");
            res1.EnsureSuccessStatusCode();

            var res2 = await _client.GetAsync("/refetch");
            res2.EnsureSuccessStatusCode();

            var res3 = await _client.GetAsync("/?pageSize=25&pageNumber=11&filter=NameDesc");
            res3.EnsureSuccessStatusCode();


            var content1 = await res1.Content.ReadAsStringAsync();
            var content2 = await res2.Content.ReadAsStringAsync();
            var content3 = await res3.Content.ReadAsStringAsync();

            var tags1 = JsonSerializer.Deserialize<PageWrapper<Tag>>(content1, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            var tags2 = JsonSerializer.Deserialize<PageWrapper<Tag>>(content3, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var boolRes = JsonSerializer.Deserialize<bool>(content2, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            // Assert

            Assert.True(res1.IsSuccessStatusCode);
            Assert.True(res2.IsSuccessStatusCode);
            Assert.True(res3.IsSuccessStatusCode);
            Assert.NotNull(tags1);
            Assert.NotNull(tags2);

            Assert.True(boolRes);
            Assert.Equal(10, tags1.Content.Count());
            Assert.Equal(25, tags2.Content.Count());
        }
    }


}
