using IOM.Models.ApiControllerModels;
using IOM.Properties;
using IOM.Services;
using System.Threading.Tasks;
using System.Web.Http;
using IOM.Services.Interface;
using Microsoft.AspNet.Identity;
using System;

namespace IOM.Controllers.WebApi
{
    [RoutePrefix("tag")]
    [Authorize]
    public class TagController : ApiController
    {
        private readonly ITagServices _tagServices;
        private readonly IRepositoryService _repositoryService;
        public TagController(ITagServices tagServices, IRepositoryService repositoryService)
        {
            _tagServices = tagServices;
            _repositoryService = repositoryService;
        }

        [HttpGet]
        [Route("list")]
        public ApiResult GetTags()
        {
            var result = new ApiResult
            {
                data = _tagServices.TagsLookup()
            };

            return result;
        }

        [HttpPost]
        [Route("assign")]
        public async Task<ApiResult> AssignTag(TagModel userTag)
        {
            var result = new ApiResult
            {
                isSuccessful = await _tagServices.AssignTagAsync(userTag).ConfigureAwait(false)
            };

            if (result.isSuccessful && userTag.AttendanceDate != null)
            {
                _repositoryService.UpdateWorkedHours(userTag.AttendanceDate, userTag.UserDetailsId, User.Identity.GetUserId());
            }
            return result;

        }

        [HttpPost]
        [Route("remove-assign")]
        public async Task<ApiResult> RemoveTag(TagModel userTag)
        {
            var result = new ApiResult();

            await _tagServices.RemoveTagAsync(userTag).ConfigureAwait(false);

            result.message = Resources.TagSuccessRemove;

            return result;
        }

        [HttpPost]
        [Route("save")]
        public async Task<ApiResult> SaveTag(TagModel userTag)
        {
            var result = new ApiResult();

            await _tagServices.SaveTagAsync(userTag).ConfigureAwait(false);

            result.message = Resources.TagSuccessAdd;

            return result;
        }

        [HttpPost]
        [Route("add")]
        public async Task<ApiResult> AddTag(TagModel userTag)
        {
            var result = new ApiResult();

            await _tagServices.AddTagAsync(userTag).ConfigureAwait(false);

            result.message = Resources.TagSuccessUpdate;

            return result;
        }

        [HttpDelete]
        [Route("delete/{tagId}")]
        public async Task<ApiResult> DeleteTag(int tagId)
        {
            var result = new ApiResult();

            await _tagServices.DeleteTagAsync(tagId).ConfigureAwait(false);

            result.message = Resources.TagSuccessDelete;

            return result;
        }
    }
}