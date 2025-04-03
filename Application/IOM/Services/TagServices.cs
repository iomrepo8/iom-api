using IOM.DbContext;
using IOM.Models.ApiControllerModels;
using IOM.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IOM.Services.Interface;

namespace IOM.Services
{
    public class TagServices : ITagServices
    {
        public IList<BaseLookUpModel> TagsLookup()
        {
            using (var ctx = Entities.Create())
            {
                var dataQuery = ctx.Tags.AsQueryable();

                return dataQuery.Select(d => new BaseLookUpModel
                {
                    Id = d.Id,
                    Text = d.Name
                }).ToList();
            }
        }

        public async Task<bool> AssignTagAsync(TagModel userTag)
        {            
            using (var ctx = Entities.Create())
            {
                ctx.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
                var existing = ctx.UserTags.SingleOrDefault(
                    t => t.UserDetailsId == userTag.UserDetailsId && t.TagId == userTag.TagId && t.AttendanceDate == userTag.AttendanceDate);
                string timeFormat = "HH:mm";

                if (existing == null)
                {
                    ctx.UserTags.Add(new UserTag
                    {
                        UserDetailsId = userTag.UserDetailsId,
                        TagId = userTag.TagId,
                        DateCreated = DateTime.UtcNow,
                        Hours = userTag.Hours,
                        AttendanceDate = userTag.AttendanceDate
                    });

                    await ctx.SaveChangesAsync().ConfigureAwait(false);

                    return true;
                }

                return false;
            }
        }

        public async Task RemoveTagAsync(TagModel userTag)
        {
            using (var ctx = Entities.Create())
            {
                var existing = ctx.UserTags.SingleOrDefault(
                    t => t.UserDetailsId == userTag.UserDetailsId && t.TagId == userTag.TagId);

                if (existing != null)
                {
                    ctx.UserTags.Remove(existing);

                    await ctx.SaveChangesAsync().ConfigureAwait(false);
                }
            }
        }

        public async Task SaveTagAsync(TagModel userTag)
        {
            using (var ctx = Entities.Create())
            {
                var existing = ctx.Tags.SingleOrDefault(t => t.Id == userTag.TagId);

                var duplicate = ctx.Tags.SingleOrDefault(t => t.Name == userTag.Name && t.Id != userTag.TagId);

                if(duplicate != null)
                {
                    throw new Exception(Resources.TagDuplicateName);
                }

                if (existing != null)
                {
                    existing.Name = userTag.Name;

                    await ctx.SaveChangesAsync().ConfigureAwait(false);
                }
            }
        }

        public async Task AddTagAsync(TagModel userTag)
        {
            using (var ctx = Entities.Create())
            {
                var duplicate = ctx.Tags.SingleOrDefault(t => t.Name == userTag.Name);

                if (duplicate != null)
                {
                    throw new Exception(Resources.TagDuplicateName);
                }
                else
                {
                    ctx.Tags.Add(new Tag
                    {
                        Name = userTag.Name
                    });
                }

                await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public async Task DeleteTagAsync(int tagId)
        {
            using (var ctx = Entities.Create())
            {
                var transaction = ctx.Database.BeginTransaction();

                var existing = ctx.Tags.SingleOrDefault(t => t.Id == tagId);

                if (existing != null)
                {
                    var userTags = ctx.UserTags.Where(t => t.TagId == tagId);

                    ctx.UserTags.RemoveRange(userTags);

                    ctx.Tags.Remove(existing);

                    await ctx.SaveChangesAsync().ConfigureAwait(false);
                }

                transaction.Commit();
            }
        }
    }
}