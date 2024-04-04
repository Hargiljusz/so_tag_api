using DataCommon.Models;
using DataService.Models;
using DataService.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Controllers;
using WebAPI.Models;
using Xunit.Abstractions;

namespace Test
{
    public class TagControllerTest
    {
        private Mock<ITagService> mockTagService;
        private readonly ITestOutputHelper output;
        private readonly TagController tagController;

        public TagControllerTest(ITestOutputHelper output)
        {
            this.mockTagService = new Mock<ITagService>();
            this.output = output;
            this.tagController = new TagController(this.mockTagService.Object);
        }

        [Theory]
        [InlineData(3)]
        [InlineData(7)]
        [InlineData(10)]
        public async Task Test1_SearchMaxFirst10Tags(int size)
        {
            //Arrange
            var tags = new List<DataCommon.Models.Tag>()
            {
                new DataCommon.Models.Tag
                    {
                        Name = "tag_test1",
                        Count = 235,
                        Collectives = Enumerable.Empty<Collective>().ToList()
                    },
                new DataCommon.Models.Tag
                    {
                        Name = "tag_test2",
                        Count = 234,
                        Collectives = Enumerable.Empty<Collective>().ToList()
                    },
                new DataCommon.Models.Tag
                    {
                        Name = "tag_test3",
                        Count = 34,
                        Collectives = Enumerable.Empty<Collective>().ToList()
                    },
                new DataCommon.Models.Tag
                    {
                        Name = "tag_test4",
                        Count = 4,
                        Collectives = Enumerable.Empty<Collective>().ToList()
                    },
                new DataCommon.Models.Tag
                    {
                        Name = "tag_test5",
                        Count = 1,
                        Collectives = Enumerable.Empty<Collective>().ToList()
                    },
                new DataCommon.Models.Tag
                    {
                        Name = "tag_test6",
                        Count = 12,
                        Collectives = Enumerable.Empty<Collective>().ToList()
                    },
                new DataCommon.Models.Tag
                    {
                        Name = "tag_test7",
                        Count = 12340,
                        Collectives = Enumerable.Empty<Collective>().ToList()
                    },
                new DataCommon.Models.Tag
                    {
                        Name = "tag_test8",
                        Count = 35,
                        Collectives = Enumerable.Empty<Collective>().ToList()
                    },
                new DataCommon.Models.Tag
                    {
                        Name = "tag_test9",
                        Count = 574,
                        Collectives = Enumerable.Empty<Collective>().ToList()
                    },
                new DataCommon.Models.Tag
                    {
                        Name = "tag_test10",
                        Count = 965,
                        Collectives = Enumerable.Empty<Collective>().ToList()
                    }
            };
            
            mockTagService.Setup(ts => ts.IsContent()).ReturnsAsync(true);
            mockTagService.Setup(ts => ts.PagedSearch(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<FilterEnum>())).ReturnsAsync(new PageResult(tags.Take(5).ToList(), tags.Count));
            //Act
            var result = await tagController.Index(size, 0, FilterEnum.ParticipationAsc);
            OkObjectResult? okObjectResult = result.Result as OkObjectResult;
            //Assert
            Assert.NotNull(okObjectResult);

            PageWrapper<DataCommon.Models.Tag>? pageWrapper = okObjectResult.Value as PageWrapper<DataCommon.Models.Tag>;
            Assert.NotNull(pageWrapper);
            Assert.Equal(5, pageWrapper.Content.Count());
        }

        [Fact]
        public async Task Test2_Search2Tags_WhenSkipped6Tags_SortedByNameDesc()
        {
            //Arrange
            var tags = new List<DataCommon.Models.Tag>()
            {
                new DataCommon.Models.Tag
                    {
                        Name = "tag_test1",
                        Count = 235,
                        Collectives = Enumerable.Empty<Collective>().ToList()
                    },
                new DataCommon.Models.Tag
                    {
                        Name = "tag_test2",
                        Count = 234,
                        Collectives = Enumerable.Empty<Collective>().ToList()
                    },
                new DataCommon.Models.Tag
                    {
                        Name = "tag_test3",
                        Count = 34,
                        Collectives = Enumerable.Empty<Collective>().ToList()
                    },
                new DataCommon.Models.Tag
                    {
                        Name = "tag_test4",
                        Count = 4,
                        Collectives = Enumerable.Empty<Collective>().ToList()
                    },
                new DataCommon.Models.Tag
                    {
                        Name = "tag_test5",
                        Count = 1,
                        Collectives = Enumerable.Empty<Collective>().ToList()
                    },
                new DataCommon.Models.Tag
                    {
                        Name = "tag_test6",
                        Count = 12,
                        Collectives = Enumerable.Empty<Collective>().ToList()
                    },
                new DataCommon.Models.Tag
                    {
                        Name = "tag_test7",
                        Count = 12340,
                        Collectives = Enumerable.Empty<Collective>().ToList()
                    },
                new DataCommon.Models.Tag
                    {
                        Name = "tag_test8",
                        Count = 35,
                        Collectives = Enumerable.Empty<Collective>().ToList()
                    },
                new DataCommon.Models.Tag
                    {
                        Name = "tag_test9",
                        Count = 574,
                        Collectives = Enumerable.Empty<Collective>().ToList()
                    },
                new DataCommon.Models.Tag
                    {
                        Name = "tag_test10",
                        Count = 965,
                        Collectives = Enumerable.Empty<Collective>().ToList()
                    }
            };
            var take = 2;
            mockTagService.Setup(ts => ts.IsContent()).ReturnsAsync(true);
            mockTagService.Setup(ts => ts.PagedSearch(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<FilterEnum>())).ReturnsAsync(new PageResult(tags.Skip(6).Take(take).OrderByDescending(t=>t.Name).ToList(), tags.Count));
            //Act
            var result = await tagController.Index(take, 3, FilterEnum.ParticipationAsc);
            OkObjectResult? okObjectResult = result.Result as OkObjectResult;
            //Assert
            Assert.NotNull(okObjectResult);

            PageWrapper<DataCommon.Models.Tag>? pageWrapper = okObjectResult.Value as PageWrapper<DataCommon.Models.Tag>;
            Assert.NotNull(pageWrapper);
            Assert.Equal(take, pageWrapper.Content.Count());
            Assert.Equal("tag_test8", pageWrapper.Content.FirstOrDefault()?.Name);

        }

        [Fact]
        public async Task Test3_SearchTags_WhenEmptyCollection()
        {
            var tags = new List<DataCommon.Models.Tag>()
            {
                new DataCommon.Models.Tag
                    {
                        Name = "tag_test1",
                        Count = 235,
                        Collectives = Enumerable.Empty<Collective>().ToList()
                    },
                new DataCommon.Models.Tag
                    {
                        Name = "tag_test2",
                        Count = 234,
                        Collectives = Enumerable.Empty<Collective>().ToList()
                    },
                new DataCommon.Models.Tag
                    {
                        Name = "tag_test3",
                        Count = 34,
                        Collectives = Enumerable.Empty<Collective>().ToList()
                    },
                new DataCommon.Models.Tag
                    {
                        Name = "tag_test4",
                        Count = 4,
                        Collectives = Enumerable.Empty<Collective>().ToList()
                    },
                new DataCommon.Models.Tag
                    {
                        Name = "tag_test5",
                        Count = 1,
                        Collectives = Enumerable.Empty<Collective>().ToList()
                    },
                new DataCommon.Models.Tag
                    {
                        Name = "tag_test6",
                        Count = 12,
                        Collectives = Enumerable.Empty<Collective>().ToList()
                    },
                new DataCommon.Models.Tag
                    {
                        Name = "tag_test7",
                        Count = 12340,
                        Collectives = Enumerable.Empty<Collective>().ToList()
                    },
                new DataCommon.Models.Tag
                    {
                        Name = "tag_test8",
                        Count = 35,
                        Collectives = Enumerable.Empty<Collective>().ToList()
                    },
                new DataCommon.Models.Tag
                    {
                        Name = "tag_test9",
                        Count = 574,
                        Collectives = Enumerable.Empty<Collective>().ToList()
                    },
                new DataCommon.Models.Tag
                    {
                        Name = "tag_test10",
                        Count = 965,
                        Collectives = Enumerable.Empty<Collective>().ToList()
                    }
            };
            mockTagService.Setup(ts => ts.IsContent()).ReturnsAsync(false);
            mockTagService.Setup(ts => ts.InitTags()).Callback(() => output.WriteLine("Initialize tags"));
            mockTagService.Setup(ts => ts.PagedSearch(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<FilterEnum>())).ReturnsAsync(new PageResult(tags, tags.Count));
            //Act
            var result = await tagController.Index(10, 0, FilterEnum.ParticipationAsc);
            OkObjectResult? okObjectResult = result.Result as OkObjectResult;
            //Assert
            Assert.NotNull(okObjectResult);

            PageWrapper<DataCommon.Models.Tag>? pageWrapper = okObjectResult.Value as PageWrapper<DataCommon.Models.Tag>;
            Assert.NotNull(pageWrapper);
            Assert.Equal(10, pageWrapper.Content.Count());
        }
    }
}
