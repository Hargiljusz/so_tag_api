using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DataService.Services;
using DataCommon;
using DataCommon.Models;
using MongoDB.Driver;
using Xunit.Abstractions;
using Amazon.SecurityToken.Model;
using Moq.Protected;
using System.Net.Http.Headers;
using System.Text.Json;
using DataService.Models;

namespace Test
{
    public class TagServiceTest
    {
        private readonly Mock<IHttpClientFactory> mockFactory;
        private readonly ITagService tagService;
        private readonly Mock<IDbClient> mockDbClient;
        private readonly ITestOutputHelper output;

        public TagServiceTest(ITestOutputHelper output)
        {
            this.mockFactory = new Mock<IHttpClientFactory>();
            this.mockDbClient = new Mock<IDbClient>();
            this.tagService = new TagServiceDb(mockDbClient.Object, mockFactory.Object);
            this.output = output;
        }

        [Fact]
        public async Task Test1_AddNewTag_ShouldReturnAddedTagWithId()
        {
            //Arrange
            var mockTagCollection = new Mock<IMongoCollection<DataCommon.Models.Tag>>();
            Guid guid = Guid.NewGuid();
            var tag = new DataCommon.Models.Tag()
            {
                Name = "tag_test1",
                Count = 1234,
                Collectives = Enumerable.Empty<Collective>().ToList()
            };

            mockTagCollection.Setup(tc => tc.InsertOneAsync(tag, default, CancellationToken.None)).Callback(() =>
            {
                tag.ID = guid.ToString();
            });
            mockDbClient.Setup(db => db.GetTagsCollection()).Returns(mockTagCollection.Object);
            //Act
            var result = await tagService.AddTag(tag);

            //Assert
            Assert.NotEqual("", result.ID);
            output.WriteLine($"Id: {result.ID}, Tag-Name: {result.Name}");

        }

        [Fact]
        public async Task Test2_AddNewTags_ShouldReturnAddedTagsWithId()
        {
            //Arrange
            var mockTagCollection = new Mock<IMongoCollection<DataCommon.Models.Tag>>();
            
            var tags = new List<DataCommon.Models.Tag>()
            {
                new DataCommon.Models.Tag
                    {
                        Name = "tag_test1",
                        Count = 1234,
                        Collectives = Enumerable.Empty<Collective>().ToList()
                    },
                new DataCommon.Models.Tag
                    {
                        Name = "tag_test2",
                        Count = 1234,
                        Collectives = Enumerable.Empty<Collective>().ToList()
                    },
                new DataCommon.Models.Tag
                    {
                        Name = "tag_test3",
                        Count = 1234,
                        Collectives = Enumerable.Empty<Collective>().ToList()
                    }
            };

            mockTagCollection.Setup(tc => tc.InsertManyAsync(tags, default, CancellationToken.None)).Callback(() =>
            {
                tags.ForEach(t => t.ID = Guid.NewGuid().ToString());
            });
            mockDbClient.Setup(db => db.GetTagsCollection()).Returns(mockTagCollection.Object);
            //Act
            var result = await tagService.AddTags(tags);

            //Assert
            Assert.True(tags.All(t => t.ID != null));
            tags.ForEach(t =>
            {
                output.WriteLine($"Id: {t.ID}, Tag-Name: {t.Name}");
            });

        }

        [Fact]
        public async Task Test3_DropAllCollectionInDB_ShouldReturnTrue()
        {
            //Arrange
            
            mockDbClient.Setup(db => db.DropAllCollections());
            //Act
            var result = await tagService.DumpTags();

            //Assert
            Assert.True(result);

        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        public async Task Test4_InitTags_ShouldReturnsAndSaveMax10Tags(int size)
        {
            //Arrange
            var mockTagCollection = new Mock<IMongoCollection<DataCommon.Models.Tag>>();
            mockTagCollection.Setup(tc => tc.EstimatedDocumentCountAsync(default, CancellationToken.None)).ReturnsAsync(0);
            mockDbClient.Setup(db => db.GetTagsCollection()).Returns(mockTagCollection.Object);

            var tags = new List<DataCommon.Models.Tag>()
            {
                new DataCommon.Models.Tag
                    {
                        Name = "tag_test1",
                        Count = 1234,
                        Collectives = Enumerable.Empty<Collective>().ToList()
                    },
                new DataCommon.Models.Tag
                    {
                        Name = "tag_test2",
                        Count = 1234,
                        Collectives = Enumerable.Empty<Collective>().ToList()
                    },
                new DataCommon.Models.Tag
                    {
                        Name = "tag_test3",
                        Count = 1234,
                        Collectives = Enumerable.Empty<Collective>().ToList()
                    },
                new DataCommon.Models.Tag
                    {
                        Name = "tag_test4",
                        Count = 1234,
                        Collectives = Enumerable.Empty<Collective>().ToList()
                    },
                new DataCommon.Models.Tag
                    {
                        Name = "tag_test5",
                        Count = 1234,
                        Collectives = Enumerable.Empty<Collective>().ToList()
                    },
                new DataCommon.Models.Tag
                    {
                        Name = "tag_test6",
                        Count = 1234,
                        Collectives = Enumerable.Empty<Collective>().ToList()
                    },
                new DataCommon.Models.Tag
                    {
                        Name = "tag_test7",
                        Count = 1234,
                        Collectives = Enumerable.Empty<Collective>().ToList()
                    },
                new DataCommon.Models.Tag
                    {
                        Name = "tag_test8",
                        Count = 1234,
                        Collectives = Enumerable.Empty<Collective>().ToList()
                    },
                new DataCommon.Models.Tag
                    {
                        Name = "tag_test9",
                        Count = 1234,
                        Collectives = Enumerable.Empty<Collective>().ToList()
                    },
                new DataCommon.Models.Tag
                    {
                        Name = "tag_test10",
                        Count = 1234,
                        Collectives = Enumerable.Empty<Collective>().ToList()
                    }
            };
            var soWrapper = new SOWrapper<DataCommon.Models.Tag>(tags.Take(size).ToList(),true,-1,-1);


            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            HttpResponseMessage result = new HttpResponseMessage();
            result.StatusCode = System.Net.HttpStatusCode.OK;
            result.Content = new StringContent(JsonSerializer.Serialize(soWrapper), Encoding.UTF8, "application/json");

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(result)
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object);

            mockFactory.Setup(_ => _.CreateClient("so_tags")).Returns(httpClient);

            //Act
           var val = await tagService.InitTags();

            //Assert
            Assert.True(val);

        }

        [Fact]
        public async Task Test5_CheckisIsContent_WhenIsEmptyTagCollection_ShouldReturnFalse()
        {
            //Arrange
            long tagCollectionCount = 0;
            var mockTagCollection = new Mock<IMongoCollection<DataCommon.Models.Tag>>();
            mockTagCollection.Setup(tc => tc.EstimatedDocumentCountAsync(default, CancellationToken.None)).ReturnsAsync(tagCollectionCount);
            mockDbClient.Setup(db => db.GetTagsCollection()).Returns(mockTagCollection.Object);

            //Act
            var result = await tagService.IsContent();

            //Assert
            Assert.False(result);

        }

        [Fact]
        public async Task Test6_CheckisIsContent_WhenIsNonEmptyTagCollection_ShouldReturnTrue()
        {
            //Arrange
            long tagCollectionCount = 10;
            var mockTagCollection = new Mock<IMongoCollection<DataCommon.Models.Tag>>();
            mockTagCollection.Setup(tc => tc.EstimatedDocumentCountAsync(default, CancellationToken.None)).ReturnsAsync(tagCollectionCount);
            mockDbClient.Setup(db => db.GetTagsCollection()).Returns(mockTagCollection.Object);

            //Act
            var result = await tagService.IsContent();

            //Assert
            Assert.True(result);
        }

    }

}
