using JobRunner.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobRunner.Core.Interfaces
{
    public interface ITagService
    {
        Task<IReadOnlyList<ITag>> GetAllTagsAsync();
        Task<ITag?> GetTagByIdAsync(Guid id);
        Task<ITag> CreateTagAsync(string name, string color);
        Task<ITag?> UpdateTagAsync(Guid id, string name, string color);
        Task<bool> DeleteTagAsync(Guid id);
    }
}
