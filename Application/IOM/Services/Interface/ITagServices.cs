using System.Collections.Generic;
using System.Threading.Tasks;
using IOM.Models.ApiControllerModels;

namespace IOM.Services.Interface
{
    public interface ITagServices
    {
        IList<BaseLookUpModel> TagsLookup();
        Task<bool> AssignTagAsync(TagModel userTag);
        Task RemoveTagAsync(TagModel userTag);
        Task SaveTagAsync(TagModel userTag);
        Task AddTagAsync(TagModel userTag);
        Task DeleteTagAsync(int tagId);
    }
}