using DataCommon.Models;
using DataService.Models;
using DataService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;


namespace WebAPI.Controllers
{
    
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagSerevice;

        public TagController(ITagService tagSerevice)
        {
            _tagSerevice = tagSerevice;
        }

        [HttpGet("/")]
        public async Task<ActionResult<PageWrapper<Tag>>> Index(int pageSize = 10, int pageNumber  = 0, FilterEnum filter = FilterEnum.None)
        {
            if(! (await _tagSerevice.IsContent()))
            {
                await _tagSerevice.InitTags();
            }
            var result = await  _tagSerevice.PagedSearch(pageNumber, pageSize, filter);
            return Ok(new PageWrapper<Tag>(result.Tags,pageSize,pageNumber,result.Count));
        }


        [HttpGet("/refetch")]
        public async Task<ActionResult<bool>> Refetch()
        {
            var dumpResult = await _tagSerevice.DumpTags();

            if (!dumpResult)
            {
                return Ok(dumpResult);
            }
            var initResult = await _tagSerevice.InitTags();

            if (!initResult)
            {
                return Ok(initResult);
            }
            return Ok(initResult);
        }
    }
}
