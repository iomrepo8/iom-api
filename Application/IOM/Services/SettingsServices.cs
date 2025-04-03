using IOM.DbContext;
using IOM.Models.ApiControllerModels;
using IOM.Properties;
using IOM.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IOM.Helpers;
using IOM.Models;
using IOM.Models.ApiControllerModels.Settings;
using IOM.Services.Interface;

namespace IOM.Services
{
    public partial class RepositoryService : IRepositoryService
    {
        public IList<LocationModel> GetLocations()
        {
            using (var ctx = Entities.Create())
            {
                return ctx.Locations.Select(l => new LocationModel
                {
                    Id = l.Id,
                    Name = l.Name,
                    Description = l.Description
                }).ToList();
            }
        }

        public void SaveLocation(LocationModel location, string username)
        {
            using (var ctx = Entities.Create())
            {
                var existingLocation
                    = ctx.Locations
                        .SingleOrDefault(l => l.Name == location.Name
                                              && l.Description == location.Description);

                if (location.Id > 0)
                {
                    existingLocation = ctx.Locations.SingleOrDefault(l => l.Id == location.Id);

                    existingLocation.Name = location.Name;
                    existingLocation.Description = location.Description;
                    existingLocation.LastUpdateDate = DateTime.UtcNow;
                    existingLocation.UpdatedBy = username;
                }
                else
                {
                    ctx.Locations.Add(new Location
                    {
                        Name = location.Name,
                        Description = location.Description,
                        CreatedBy = username,
                        CreateDate = DateTime.UtcNow
                    });
                }

                ctx.SaveChanges();
            }
        }

        public async Task DeleteIPWhitelist(int id, string name)
        {
            using (var ctx = Entities.Create())
            {
                var ipAddress = await ctx.IpWhitelists.SingleOrDefaultAsync(i => i.Id == id)
                    .ConfigureAwait(false);

                if (ipAddress != null)
                {
                    ctx.IpWhitelists.Remove(ipAddress);
                }
            }
        }

        public async Task<ApiResult> SaveIP(IPWhitelist ipAddress, string username)
        {
            var result = new ApiResult();

            var userInfo = GetCurrentUserInfo(username);
            using (var ctx = Entities.Create())
            {
                if (ipAddress.Id > 0)
                {
                    var existingIP = await ctx.IpWhitelists.SingleOrDefaultAsync(i => i.Id == ipAddress.Id)
                        .ConfigureAwait(false);

                    existingIP.IPAddress = ipAddress.IPAddress;
                    existingIP.Alias = ipAddress.Alias;
                    existingIP.UpdatedBy = userInfo.UserDetailsId;
                    existingIP.UpdatedDate = DateTime.UtcNow;
                }
                else
                {
                    var existingIP = await ctx.IpWhitelists.SingleOrDefaultAsync(i => i.IPAddress == ipAddress.IPAddress)
                        .ConfigureAwait(false);

                    if(existingIP != null)
                    {
                        result.isSuccessful = false;
                        result.message = Resources.IPWhitelistEntryError;

                        return result;
                    }

                    ctx.IpWhitelists.Add(new IpWhitelist
                    {
                        Alias = ipAddress.Alias,
                        IPAddress = ipAddress.IPAddress,
                        CreatedBy = userInfo.UserDetailsId,
                        CreatedDate = DateTime.UtcNow,
                        UpdatedDate = null
                    });
                }

                await ctx.SaveChangesAsync().ConfigureAwait(false);

                result.message = ipAddress.Id > 0 ?
                    Resources.IPWhitelistEntrySuccessUpdate : Resources.IPWhitelistEntrySuccessAdd;

                return result;
            }
        }

        public async Task<IList<IPWhitelist>> GetIPWhitelist()
        {
            using (var ctx = Entities.Create())
            {
                var dataQuery = await (from i in ctx.IpWhitelists
                                       join uCreate in ctx.UserDetails on i.CreatedBy equals uCreate.Id
                                       from uUpdate in ctx.UserDetails.Where(u => u.Id == i.UpdatedBy).DefaultIfEmpty()
                                       select new IPWhitelist
                                       {
                                           Id = i.Id,
                                           Alias = i.Alias,
                                           IPAddress = i.IPAddress,
                                           CreatedBy = uCreate.FirstName + " " + uCreate.LastName,
                                           UpdatedBy = uUpdate != null ? uUpdate.FirstName + " " + uUpdate.LastName : null,
                                       }).ToListAsync().ConfigureAwait(false);
                return dataQuery;
            }
        }

        public void DeleteLocation(int id, string name)
        {
            using (var ctx = Entities.Create())
            {

            }
        }

        public async Task<IList<UserNotificationSettingResult>> GetUserNotificationSettings(int userDetailsId)
        {
            using (var ctx = Entities.Create())
            {
                var userInfo = GetUserDetails(userDetailsId);
                
                var dataQuery = await (from un in ctx.UserNotificationSettings
                                       join ud in ctx.UserDetails on un.UserDetailsId equals ud.Id
                                       join nt in ctx.NotificationTypes on un.NotificationTypeId equals nt.Id
                                       where un.UserDetailsId == userDetailsId
                                       select new UserNotificationSettingResult
                                       {
                                           Id = un.Id,
                                           Name = nt.Name,
                                           Description = nt.Description,
                                           NotificationTypeId = nt.Id,
                                           UserDetailsId = ud.Id,
                                           IsAllowed = un.IsAllowed
                                       }).ToListAsync().ConfigureAwait(false);

                switch (userInfo.RoleCode) 
                {
                    case Globals.ACCOUNT_MANAGER_RC:
                    case Globals.TEAM_MANAGER_RC:
                    case Globals.LEAD_AGENT_RC:
                        dataQuery = dataQuery.Where(d =>
                            EmailNotificationSettingsProvide.RoleSetting[userInfo.RoleCode]
                                .Contains((EmailNotificationType) d.NotificationTypeId)).ToList();
                        break;
                }
                
                return dataQuery;
            }
        }

        public async Task UpdateUserNotificationSettings(IList<UserNotificationSettingResult> notificationSettings)
        {
            using (var ctx = Entities.Create())
            {
                foreach (var notification in notificationSettings)
                {
                    var userNotificationSetting =
                        await ctx.UserNotificationSettings.SingleOrDefaultAsync(n => n.Id == notification.Id)
                            .ConfigureAwait(false);
                            
                    if (userNotificationSetting != null) userNotificationSetting.IsAllowed = notification.IsAllowed;
                }
                
                await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public async Task<IList<NotificationSettingModel>> GetNotificationSettingsAsync(CancellationToken cancellationToken)
        {
            using (var ctx = Entities.Create())
            {
                var dataQuery = await (from n in ctx.NotificationSettings
                    select new NotificationSettingModel
                    {
                        Id = n.Id,
                        Name = n.Name,
                        Action = n.Action,
                        Subject = n.Subject,
                        Type = n.Type,
                        Message = n.Message
                    }).ToListAsync(cancellationToken).ConfigureAwait(false);
                return dataQuery;
            }
        }

        public async Task<IList<NotificationRecipientRoleModel>> GetNotificationRoleRecipientsAsync(int notificationSettingsId, CancellationToken cancellationToken)
        {
            using (var ctx = Entities.Create())
            {
                var dataQuery = await (from n in ctx.NotificationRecipientRoles
                        join r in ctx.AspNetRoles on n.RoleId equals r.Id
                        where n.NotificationSettingId == notificationSettingsId
                        select new NotificationRecipientRoleModel
                    {
                        Id = n.Id,
                        NotificationSettingId = n.NotificationSettingId,
                        RoleId = n.RoleId,
                        RoleName = r.Name
                    })
                    .ToListAsync(cancellationToken).ConfigureAwait(false);
                
                return dataQuery;
            }
        }

        public async Task UpdateNotificationSettingAsync(NotificationSettingModel notificationSetting, CancellationToken cancellationToken)
        {
            using (var ctx = Entities.Create())
            {
                var notificationSettingData = await ctx.NotificationSettings
                    .SingleOrDefaultAsync(d => d.Id == notificationSetting.Id, cancellationToken).ConfigureAwait(false);

                if (notificationSettingData != null)
                {
                    var oldRecipients = await ctx.NotificationRecipientRoles.Where(r => r.NotificationSettingId == notificationSetting.Id)
                        .ToListAsync(cancellationToken).ConfigureAwait(false);
                    ctx.NotificationRecipientRoles.RemoveRange(oldRecipients);

                    if (notificationSetting.RecipientRoles != null)
                    {
                        foreach (var roleRecipient in notificationSetting.RecipientRoles)
                        {
                            ctx.NotificationRecipientRoles.Add(new NotificationRecipientRole
                            {
                                RoleId = roleRecipient.Id,
                                NotificationSettingId = notificationSetting.Id
                            });
                        }
                    }

                    await ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                }
            }
        }

        public async Task<NotificationSettingModel> GetNotificationSettingsAsync(NotificationAction notificationAction,
            CancellationToken cancellationToken)
        {
            using (var ctx = Entities.Create())
            {
                return await ctx.NotificationSettings.Select(d => new NotificationSettingModel
                    {
                         Id = d.Id,
                         Action = d.Action,
                         Subject = d.Subject,
                         Message = d.Message
                    })
                    .SingleOrDefaultAsync(n => n.Action == notificationAction.ToString(), cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }
}