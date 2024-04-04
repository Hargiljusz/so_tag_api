
using DataCommon.Models;
using DataService.Models;


namespace DataService.Services
{
    public interface ITagService
    {
        Task<bool> IsContent();
        Task<bool> InitTags();
        Task<Tag> AddTag(Tag tag);
        Task<List<Tag>> AddTags(IEnumerable<Tag> tags);

        Task<PageResult> PagedSearch(int page, int size = 10, FilterEnum filter = FilterEnum.None);

        Task<bool> DumpTags();
    }

    public record PageResult(List<Tag> Tags, long Count);
}
