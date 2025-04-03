using IOM.DbContext;
using IOM.Helpers;
using IOM.Models.ApiControllerModels;
using IOM.Properties;
using IOM.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using IOM.Services.Interface;
using IOM.Models;
using Microsoft.AspNet.Identity;
using SendGrid.Helpers.Mail;

namespace IOM.Services
{
    public partial class RepositoryService : IRepositoryService
    {
        public TeamDashboard GetTeamsDashboardData(int[] accountIds, int[] teamids, string username, bool showInactive = false)
        {
            var userInfo = GetCurrentUserInfo(username);

            using (var ctx = Entities.Create())
            {
                IEnumerable<TeamRawModel> assignedTeams = GetAssignedTeamsRaw(username, false).ToList();
                IEnumerable<AccountModel> assignedAccounts = GetAssignedAccountsRaw(username, false).ToList();
                IEnumerable<UsersRawModel> users = GetAssignedUsersRaw(username).ToList();

                if (!showInactive)
                {
                    assignedTeams = assignedTeams.Where(t => t.IsActive == true);
                }

                if (teamids.Length > 0)
                {
                    assignedTeams = assignedTeams.Where(a => teamids.Contains(a.Id));
                }

                IEnumerable<int> assignedAccountIds = assignedAccounts.Select(t => t.Id).ToList();
                IEnumerable<int> assignedTeamIds = assignedTeams.Where(e => assignedAccountIds
                    .Contains(e.AccountId)).Select(t => t.Id).ToList();

                var teams = (from t in ctx.Teams
                             join a in ctx.Accounts on t.AccountId equals a.Id
                             where t.IsDeleted != true && assignedTeamIds.Any(at => at == t.Id)
                             select new
                             {
                                 t.Id,
                                 TeamName = t.Name,
                                 t.AccountId,
                                 AccountName = a.Name,
                                 t.IsActive
                             }).ToList();

                var agentsRoleCodes = new List<string> {
                    Globals.AGENT_RC, Globals.LEAD_AGENT_RC
                };

                teams = teams.Where(d => assignedAccountIds.Contains(d.AccountId)).ToList();

                if (accountIds.Length > 0)
                {
                    teams = teams.Where(d => accountIds.Contains(d.AccountId)).ToList();
                }

                switch (userInfo.RoleCode)
                {
                    /**
                     * IOM-518: Nobody should be able to see other users, or their time entries, 
                     *  of the same level or higher. 
                     */
                    case Globals.AGENT_RC:
                        users = users.Where(a => a.Id == userInfo.UserDetailsId);
                        break;
                    default:
                        users = users.Where(a => agentsRoleCodes.Contains(a.RoleCode));
                        break;
                }

                var teamMembers = (from tm in ctx.TeamMembers
                                   join u in ctx.vw_TaskClocker on tm.UserDetailsId equals u.UserDetailsId
                                   where tm.IsDeleted != true
                                   select new
                                   {
                                       tm.UserDetailsId,
                                       tm.TeamId
                                   })
                                  .ToList();

                return new TeamDashboard
                {
                    TotalTeams = assignedTeamIds.Count(),
                    TotalAgents = users.Count(),
                    TeamsList = teams.Select(e => new TeamListModel
                    {
                        Id = e.Id,
                        Name = e.TeamName,
                        AgentCount = teamMembers.Count(tm => tm.TeamId == e.Id
                                                             && users.Any(u => u.Id == tm.UserDetailsId)),
                        AccountName = e.AccountName,
                        AccountId = e.AccountId,
                        IsActive = e.IsActive
                    }).ToList()
                };
            }
        }

        public int GetTeamsCount(string username)
        {
            IEnumerable<int> teams = GetAssignedTeamsRaw(username, false)
                .Where(t => t.IsActive == true)
                .Select(e => e.Id);
            return teams.Count();
        }

        public IList<TeamLookUp> TeamsLookup(string username)
        {
            using (var ctx = Entities.Create())
            {
                var dataQuery = GetAssignedTeamsRaw(username, false)
                    .Select(d => new TeamLookUp
                    {
                        Id = d.Id,
                        Text = d.Name
                    }).ToList();

                return dataQuery;
            }
        }

        public IList<TeamLookUp> GetTeamsByAccounts(int[] accountIds, string username)
        {
            IList<TeamLookUp> assignTeams = GetAssignedTeamsRaw(username, false)
                .Where(t => t.IsActive == true)
                .Select(t => new TeamLookUp
                {
                    Id = t.Id,
                    Text = t.Name,
                    AccountId = t.AccountId
                })
                .ToList();

            if (accountIds.Length > 0)
                assignTeams = assignTeams.Where(t => accountIds.Contains(t.AccountId)).ToList();

            return assignTeams;
        }

        public IList<TeamLookUp> GetAllTeamsByAccounts(List<int> accountIds)
        {
            using (var ctx = Entities.Create())
            {
                return ctx.Teams.Where(a => accountIds.Contains(a.AccountId) && a.IsActive == true)
                    .Select(t => new TeamLookUp
                    {
                        Id = t.Id,
                        Text = t.Name,
                        AccountId = t.AccountId
                    }).ToList();
            }
        }

        public IList<LookUpModel> GetAssignedTeamsList(string username, bool includeDeleted, string query = "")
        {
            IEnumerable<TeamRawModel> dataQuery = GetAssignedTeamsRaw(username, includeDeleted, query);

            return dataQuery.Select(d => new LookUpModel
            {
                Id = d.Id,
                Text = d.Name
            }).ToList();
        }

        public IEnumerable<TeamRawModel> GetAssignedTeamsRaw(string username, bool includeDeleted, string query = "")
        {
            var userInfo = GetCurrentUserInfo(username);
            query = query.ToUpperInvariant();

            using (var ctx = Entities.Create())
            {
                var dataQuery = (from t in ctx.Teams
                                 join a in ctx.Accounts on t.AccountId equals a.Id
                                 where a.IsDeleted != true
                                 select new TeamRawModel
                                 {
                                     Id = t.Id,
                                     Name = t.Name,
                                     IsActive = t.IsActive,
                                     AccountId = a.Id,
                                     IsDeleted = t.IsDeleted
                                 }).ToList().AsQueryable();

                if (!includeDeleted)
                {
                    dataQuery = dataQuery.Where(a => a.IsDeleted != true);
                }

                if (query.Length > 0)
                {
                    dataQuery = dataQuery.Where(e => e.Name.ToUpperInvariant().Contains(query) ||
                                                     query.Contains(e.Name.ToUpperInvariant()));
                }

                var teamIds = ctx.TeamMembers
                            .Where(tm => tm.UserDetailsId == userInfo.UserDetailsId && tm.IsDeleted != true)
                            .Select(ta => ta.TeamId).ToList();

                var accountIds = ctx.AccountMembers
                    .Where(a => a.UserDetailsId == userInfo.UserDetailsId && a.IsDeleted != true)
                    .Select(a => a.AccountId).ToList();

                if (userInfo.RoleCode == Globals.SYSAD_RC)
                {
                    teamIds = ctx.Teams.Select(d => d.Id).ToList();

                    accountIds = ctx.Accounts.Select(a => a.Id).ToList();
                }

                switch (userInfo.RoleCode)
                {
                    case Globals.AGENT_RC:
                    case Globals.LEAD_AGENT_RC:
                    case Globals.TEAM_MANAGER_RC:
                    case Globals.CLIENT_RC:
                        dataQuery = dataQuery.Where(d => teamIds.Any(t => t == d.Id));
                        break;
                }

                dataQuery = dataQuery.Where(d => accountIds.Any(a => a == d.AccountId));

                return dataQuery.IOMDistinctBy(d => d.Id).AsEnumerable();
            }
        }

        public TeamDetailModel GetTeamDetails(int teamId, string username)
        {
            var userInfo = GetCurrentUserInfo(username);
            var isNameMasked = userInfo.IsNameMasked;

            IList<int> assignedUsers = UserList(userInfo.UserDetailsId)
                .Select(u => u.UserDetailsId).ToList();

            using (var ctx = Entities.Create())
            {
                TeamDetailModel team
                    = (from t in ctx.Teams
                       join a in ctx.Accounts on t.AccountId equals a.Id
                       from s in ctx.EmployeeShifts.Where(e => t.ShiftId.HasValue && e.Id == t.ShiftId).DefaultIfEmpty()
                       where t.Id == teamId && t.IsDeleted != true && a.IsDeleted != true
                       select new TeamDetailModel
                       {
                           Id = t.Id,
                           Name = t.Name,
                           AccountId = a.Id,
                           AccountName = a.Name,
                           Description = t.Description,
                           ShiftId = s.Id,
                           ShiftName = s.Name,
                           Tasks = (from tt in ctx.IOMTeamTasks
                                    join t in ctx.IOMTasks on tt.TaskId equals t.Id
                                    where tt.TeamId == teamId
                                    select new TaskModel
                                    {
                                        Id = tt.Id,
                                        Name = t.Name,
                                        Description = t.Description,
                                        IsActive = t.IsActive
                                    }).ToList(),
                           DayOffs = ctx.TeamDayOffs.Where(d => d.TeamId == teamId)
                                       .Select(e => new DayOff
                                       {
                                           Day = e.Day
                                       }).ToList(),
                           Holidays = ctx.TeamHolidays.Where(h => h.TeamId == teamId)
                                       .Select(e => new Holiday
                                       {
                                           HolidayDate = e.HolidayDate,
                                           Title = e.Title,
                                           Description = e.Description
                                       }).OrderBy(o => o.HolidayDate).ToList()
                       }).SingleOrDefault();

                var accountSeats = ctx.Seats.Where(a => a.AccountId == team.AccountId).AsEnumerable();

                IList<int> accountMemberIds
                    = (from am in ctx.AccountMembers
                       where am.AccountId == team.AccountId && am.IsDeleted != true
                       select am.UserDetailsId).ToList();

                IList<UserModel> agents
                    = (from tm in ctx.TeamMembers
                       join u in ctx.vw_ActiveUsers on tm.UserDetailsId equals u.UserDetailsId
                       where assignedUsers.Contains(u.UserDetailsId)
                            && tm.TeamId == team.Id && u.Role == Globals.AGENT_RC
                            && tm.IsDeleted != true
                       select new UserModel
                       {
                           UserDetailsId = u.UserDetailsId,
                           NetUserId = u.NetUserId,
                           FullName = u.FullName,
                           Email = u.Email,
                           StaffId = u.StaffId,
                           Seat = accountSeats.Where(a => a.UserId == u.UserDetailsId).Select(a => a.SeatNumber).FirstOrDefault()
                       }).ToList();

                foreach (var item in agents)
                {
                    if (userInfo.IsNameMasked)
                    {
                        item.FullName = Resources.NameMasked;
                        item.Email = Resources.NameMasked;
                    }
                }

                IList<UserModel> leadAgents
                    = (from tm in ctx.TeamMembers
                       join u in ctx.vw_ActiveUsers on tm.UserDetailsId equals u.UserDetailsId
                       where assignedUsers.Contains(u.UserDetailsId)
                            && tm.TeamId == team.Id && u.Role == Globals.LEAD_AGENT_RC
                            && tm.IsDeleted != true
                       select new UserModel
                       {
                           UserDetailsId = u.UserDetailsId,
                           NetUserId = u.NetUserId,
                           FullName = u.FullName,
                           Email = u.Email,
                           StaffId = u.StaffId,
                           Seat = accountSeats.Where(a => a.UserId == u.UserDetailsId).Select(a => a.SeatNumber).FirstOrDefault()
                       }).ToList();

                leadAgents = leadAgents.Where(l => accountMemberIds.Any(a => a == l.UserDetailsId)).ToList();

                foreach (var item in leadAgents)
                {
                    if (userInfo.IsNameMasked)
                    {
                        item.FullName = Resources.NameMasked;
                        item.Email = Resources.NameMasked;
                    }
                }

                IList<UserModel> clients
                    = (from tm in ctx.TeamMembers
                       join u in ctx.vw_ActiveUsers on tm.UserDetailsId equals u.UserDetailsId
                       where assignedUsers.Contains(u.UserDetailsId)
                            && tm.TeamId == team.Id && u.Role == Globals.CLIENT_RC
                            && tm.IsDeleted != true
                       select new UserModel
                       {
                           UserDetailsId = u.UserDetailsId,
                           NetUserId = u.NetUserId,
                           FullName = u.FullName,
                           Email = u.Email
                       }).ToList();

                IList<UserModel> teamManager
                    = (from tm in ctx.TeamMembers
                       join u in ctx.vw_ActiveUsers on tm.UserDetailsId equals u.UserDetailsId
                       where assignedUsers.Contains(u.UserDetailsId)
                            && tm.TeamId == team.Id && u.Role == Globals.TEAM_MANAGER_RC
                            && tm.IsDeleted != true
                       select new UserModel
                       {
                           UserDetailsId = u.UserDetailsId,
                           NetUserId = u.NetUserId,
                           FullName = u.FullName,
                           Email = u.Email,
                           StaffId = u.StaffId,
                           Seat = accountSeats.Where(a => a.UserId == u.UserDetailsId).Select(a => a.SeatNumber).FirstOrDefault()
                       }).ToList();

                teamManager = teamManager.Where(l => accountMemberIds.Any(a => a == l.UserDetailsId)).ToList();

                team.TaskGroup
                    = (from ttg in ctx.TeamTaskGroups
                       join tg in ctx.TaskGroups on ttg.GroupId equals tg.Id
                       where ttg.TeamId == teamId
                       select new BaseModel
                       {
                           Id = tg.Id,
                           Name = tg.Name,
                           Description = tg.Description
                       }).ToList();

                team.LeadAgents = leadAgents;
                team.Agents = agents;
                team.Clients = clients;
                team.Managers = teamManager;

                return team;
            }
        }

        public IList<BaseModel> GetTeams(int userDetailsId)
        {
            using (var ctx = Entities.Create())
            {
                var result
                    = (from tm in ctx.TeamMembers
                       join t in ctx.Teams on tm.TeamId equals t.Id
                       where tm.UserDetailsId == userDetailsId && tm.IsDeleted != true
                         && t.IsActive == true
                       select new BaseModel
                       {
                           Id = t.Id,
                           Name = t.Name
                       }).ToList();

                return result;
            }
        }

        public async Task AddClientAsync(TeamMemberModel client)
        {
            using (var ctx = Entities.Create())
            {
                foreach (var userDetailsId in client.UserIds)
                {
                    var user = await ctx.UserDetails.SingleOrDefaultAsync(u => u.Id == userDetailsId)
                        .ConfigureAwait(false);

                    if (user != null && user.Role != Globals.CLIENT_RC)
                    {
                        var exception = new Exception(Resources.RoleClient)
                        {
                            Source = ExceptionType.Thrown.ToString()
                        };
                        throw exception;
                    }

                    var existingCl = await (from tm in ctx.TeamMembers
                        join u in ctx.vw_ActiveUsers on tm.UserDetailsId equals u.UserDetailsId
                        where tm.TeamId == client.TeamId && tm.UserDetailsId == userDetailsId
                        select tm).SingleOrDefaultAsync().ConfigureAwait(false);

                    if (existingCl != null)
                    {
                        existingCl.IsDeleted = false;
                    }
                    else
                    {
                        ctx.TeamMembers.Add(new TeamMember
                        {
                            UserDetailsId = userDetailsId,
                            TeamId = client.TeamId,
                            CreatedDateUtc = DateTime.UtcNow
                        });
                    }
                }

                await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public void UpdateDayoffHoliday(DayOffHolidayModel dayOffHoliday)
        {
            using (var ctx = Entities.Create())
            {
                #region Remove old data
                var oldDayOffData = ctx.TeamDayOffs.Where(d => d.TeamId == dayOffHoliday.TeamId);
                ctx.TeamDayOffs.RemoveRange(oldDayOffData);

                var oldHolidayData = ctx.TeamHolidays.Where(h => h.TeamId == dayOffHoliday.TeamId);
                ctx.TeamHolidays.RemoveRange(oldHolidayData);

                ctx.SaveChanges();
                #endregion Remove old data

                #region Insert new data
                foreach (var dayOff in dayOffHoliday.DayOffs)
                {
                    ctx.TeamDayOffs.Add(new TeamDayOff
                    {
                        Day = dayOff.Day,
                        TeamId = dayOffHoliday.TeamId,
                        CreateDate = DateTimeUtility.Instance.DateTimeNow()
                    });
                }

                foreach (var holiday in dayOffHoliday.Holidays)
                {
                    ctx.TeamHolidays.Add(new TeamHoliday
                    {
                        HolidayDate = holiday.HolidayDate.Date,
                        TeamId = dayOffHoliday.TeamId,
                        Title = holiday.Title,
                        Description = holiday.Description,
                        CreateDate = DateTimeUtility.Instance.DateTimeNow()
                    });
                }

                ctx.SaveChanges();

                #endregion Insert new data
            }
        }

        public void ChangeAgentTeams(ChangeAccountModel model)
        {
            using (var ctx = Entities.Create())
            {
                var user = ctx.UserDetails.SingleOrDefault(u => u.Id == model.UserDetailsId);
                ServiceUtility.NullCheck(user, Resources.UserNotFound);

                var tAgentsData = (from tm in ctx.TeamMembers
                                   join u in ctx.vw_ActiveUsers on tm.UserDetailsId equals u.UserDetailsId
                                   where tm.UserDetailsId == model.UserDetailsId && u.Role == user.Role
                                   select tm).AsQueryable();

                foreach (var delAgent in tAgentsData)
                {
                    delAgent.IsDeleted = true;
                }

                foreach (var item in model.TeamIds)
                {
                    bool alreadyExist = false;

                    foreach (var delAgent in tAgentsData)
                    {
                        if (item == delAgent.TeamId)
                        {
                            delAgent.IsDeleted = false;
                            alreadyExist = true;
                            break;
                        }
                    }

                    if (alreadyExist) continue;

                    ctx.TeamMembers.Add(new TeamMember
                    {
                        TeamId = item,
                        UserDetailsId = user.Id,
                        CreatedDateUtc = DateTime.UtcNow
                    });
                }

                ctx.SaveChanges();
            }
        }

        public void RemoveAgent(TeamMemberModel agentEDModel)
        {
            using (var ctx = Entities.Create())
            {
                var agent = (from tm in ctx.TeamMembers
                             join u in ctx.vw_ActiveUsers on tm.UserDetailsId equals u.UserDetailsId
                             where tm.TeamId == agentEDModel.TeamId && u.Role == Globals.AGENT_RC
                             && u.UserDetailsId == agentEDModel.UserDetailsId
                             select tm).SingleOrDefault();

                if (agent == null)
                {
                    throw new Exception(Resources.AgentNotFound)
                    {
                        Source = ExceptionType.Thrown.ToString()
                    };
                }

                var agentinfo = GetUserDetails(agentEDModel.UserDetailsId);
                var usertasks = ctx.UserTasks.Where(a => a.TeamId == agentEDModel.TeamId && a.UserId == agentinfo.NetUserId);

                ctx.UserTasks.RemoveRange(usertasks);

                agent.IsDeleted = true;

                ctx.SaveChanges();
            }
        }

        public void RemoveClient(TeamMemberModel client)
        {
            using (var ctx = Entities.Create())
            {
                var existingCL = (from tm in ctx.TeamMembers
                                  join u in ctx.vw_ActiveUsers on tm.UserDetailsId equals u.UserDetailsId
                                  where tm.TeamId == client.TeamId && u.UserDetailsId == client.UserDetailsId
                                       && tm.IsDeleted != true && u.Role == Globals.CLIENT_RC
                                  select tm)
                            .SingleOrDefault();

                if (existingCL == null)
                {
                    Exception exception = new Exception(Resources.ClientNotFound)
                    {
                        Source = ExceptionType.Thrown.ToString()
                    };
                    throw exception;
                }

                existingCL.IsDeleted = true;

                ctx.SaveChanges();
            }
        }

        public async Task AddAgentAsync(TeamMemberModel addModel)
        {
            using (var ctx = Entities.Create())
            {
                foreach (var userDetailsId in addModel.UserIds)
                {
                    var user = await ctx.UserDetails
                        .SingleOrDefaultAsync(u => u.Id == userDetailsId).ConfigureAwait(false);

                    if (user != null && user.Role != Globals.AGENT_RC)
                    {
                        throw new Exception(Resources.RoleAgent)
                        {
                            Source = ExceptionType.Thrown.ToString()
                        };
                    }

                    var agent = await (from tm in ctx.TeamMembers
                        join u in ctx.vw_ActiveUsers on tm.UserDetailsId equals u.UserDetailsId
                        where tm.TeamId == addModel.TeamId && u.Role == Globals.AGENT_RC
                                                           && u.UserDetailsId == userDetailsId
                        select tm).SingleOrDefaultAsync().ConfigureAwait(false);

                    if (agent != null)
                    {
                        agent.IsDeleted = false;
                    }
                    else
                    {
                        ctx.TeamMembers.Add(new TeamMember
                        {
                            UserDetailsId = userDetailsId,
                            TeamId = addModel.TeamId,
                            CreatedDateUtc = DateTime.UtcNow
                        });
                    }

                    var team = await ctx.Teams.SingleOrDefaultAsync(t => t.Id == addModel.TeamId).ConfigureAwait(false);

                    var accDetail = await ctx.AccountMembers
                        .SingleOrDefaultAsync(a => a.UserDetailsId == user.Id && a.AccountId == team.AccountId)
                        .ConfigureAwait(false);

                    if (accDetail == null)
                    {
                        if (user != null)
                            ctx.AccountMembers.Add(new AccountMember
                            {
                                UserDetailsId = user.Id,
                                AccountId = team.AccountId,
                                Created = DateTimeUtility.Instance.DateTimeNow()
                            });
                    }
                    else
                    {
                        accDetail.IsDeleted = false;
                    }
                }

                await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public object AgentsLookup(int teamId, string query)
        {
            using (var ctx = Entities.Create())
            {
                query = query.ToLower();

                var excluded = GetTeamMembers(teamId, Globals.AGENT_RC)
                    .Select(t => t.UserDetailsId).ToList();

                var team = ctx.Teams.SingleOrDefault(t => t.Id == teamId);

                var dataQuery = (from ud in ctx.vw_ActiveUsers
                    join a in ctx.AccountMembers on ud.UserDetailsId equals a.UserDetailsId
                    where ud.Role == Globals.AGENT_RC && team.AccountId == a.AccountId && a.IsDeleted != true
                    select new
                    {
                        ud.UserDetailsId,
                        ud.NetUserId,
                        ud.FullName,
                        ud.Email
                    });

                dataQuery = dataQuery.Where(d => !excluded.Contains(d.UserDetailsId));

                if (query.Length > 0)
                {
                    dataQuery = dataQuery.Where(e => e.FullName.ToLower().Contains(query) ||
                                                     query.Contains(e.FullName.ToLower()));
                }

                return dataQuery.Select(d => new
                {
                    Id = d.UserDetailsId,
                    StringId = d.NetUserId,
                    Text = d.FullName
                }).AsEnumerable().IOMDistinctBy(x => new { x.Id, x.StringId, x.Text }).ToList();
            }
        }

        public void RemoveTeamMembers(int accountId, int userDetailsId)
        {
            using (var ctx = Entities.Create())
            {
                var teamIds = ctx.Teams.Where(t => t.AccountId == accountId).Select(d => d.Id).ToList();

                ctx.TeamMembers.Where(t => teamIds.Any(i => i == t.TeamId) && t.UserDetailsId == userDetailsId)
                    .ToList()
                    .ForEach(e =>
                    {
                        e.IsDeleted = true;
                        e.UpdatedDateUtc = DateTime.UtcNow;
                    });

                //  start send email to user

                var systemAdmins = ctx.vw_ActiveUsers.Where(a => a.Role == Globals.SYSAD_RC).ToList();

                var acc = ctx.Accounts.Where(a => a.Id == accountId).FirstOrDefault();

                var userdetail = ctx.UserDetails.Where(a => a.Id == userDetailsId).FirstOrDefault();

                var userInfo = GetUserDetails(userDetailsId);

                var emailmsgUser = string.Format("Please be advised that {0} was removed from {1}",
                    userdetail.Name,
                    acc.Name);

                string emailBodyUser = EmailBody.RemoveMember(emailmsgUser);

                Task.Run(() => RemoveMemberEmail(new IdentityMessage()
                {
                    Subject = "Removed From Account",
                    Body = emailBodyUser
                },
                userInfo.Email,
                systemAdmins)).Wait();

                var emailmsgUser2 = string.Format("Please be advised that you were removed from {1}",
                    userdetail.Name,
                    acc.Name);

                string emailBodyUser2 = EmailBody.RemoveMember(emailmsgUser2);

                Task.Run(() => RemoveMemberEmail2(new IdentityMessage()
                {
                    Subject = "Removed From Account",
                    Body = emailBodyUser2
                },
                userInfo.Email,
                systemAdmins)).Wait();

                // end send user email

                ctx.SaveChanges();
            }
        }

        #region Lead Agents

        public async Task AddLeadAgentAsync(TeamMemberModel leadAgent)
        {
            using (var ctx = Entities.Create())
            {
                foreach (var userDetailsId in leadAgent.UserIds)
                {
                    var user = await ctx.UserDetails.SingleOrDefaultAsync(u => u.Id == userDetailsId)
                        .ConfigureAwait(false);

                    if (user.Role != Globals.LEAD_AGENT_RC)
                    {
                        var exception = new Exception(Resources.RoleLA)
                        {
                            Source = ExceptionType.Thrown.ToString()
                        };
                        throw exception;
                    }

                    var existingLa = await (from tm in ctx.TeamMembers
                        join u in ctx.vw_ActiveUsers on tm.UserDetailsId equals u.UserDetailsId
                        where tm.TeamId == leadAgent.TeamId && tm.UserDetailsId == userDetailsId
                        select tm).SingleOrDefaultAsync().ConfigureAwait(false);

                    if (existingLa != null)
                    {
                        existingLa.IsDeleted = false;
                    }
                    else
                    {
                        ctx.TeamMembers.Add(new TeamMember
                        {
                            UserDetailsId = userDetailsId,
                            TeamId = leadAgent.TeamId,
                            CreatedDateUtc = DateTime.UtcNow
                        });
                    }
                }

                await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public IList<LookUpModel> LeadAgents(int teamId, string query = "")
        {
            using (var ctx = Entities.Create())
            {
                query = query.ToLower();

                Team team = ctx.Teams.SingleOrDefault(t => t.Id == teamId);
                ServiceUtility.NullCheck(team);

                IList<LookUpModel> dataQuery = (from am in ctx.AccountMembers
                                                join u in ctx.vw_ActiveUsers on am.UserDetailsId equals u.UserDetailsId
                                                where u.Role == Globals.LEAD_AGENT_RC && am.IsDeleted != true
                                                    && am.AccountId == team.AccountId
                                                select new LookUpModel
                                                {
                                                    Id = u.UserDetailsId,
                                                    Text = u.FullName
                                                }).ToList();

                var currentLAIds = (from tm in ctx.TeamMembers
                                    join u in ctx.vw_ActiveUsers on tm.UserDetailsId equals u.UserDetailsId
                                    where u.Role == Globals.LEAD_AGENT_RC && tm.IsDeleted != true
                                    && tm.TeamId == teamId
                                    select tm.UserDetailsId).ToList();

                dataQuery = dataQuery.Where(d => !currentLAIds.Contains(d.Id)).ToList();

                if (query != null)
                {
                    dataQuery = dataQuery.Where(d => d.Text.Contains(query)).ToList();
                }

                return dataQuery;
            }
        }

        public void RemoveLeadAgent(TeamMemberModel leadAgent)
        {
            using (var ctx = Entities.Create())
            {
                var existingLA = (from tm in ctx.TeamMembers
                                  join u in ctx.vw_ActiveUsers on tm.UserDetailsId equals u.UserDetailsId
                                  where tm.TeamId == leadAgent.TeamId && u.UserDetailsId == leadAgent.UserDetailsId
                                       && tm.IsDeleted != true && u.Role == Globals.LEAD_AGENT_RC
                                  select tm)
                            .SingleOrDefault();

                if (existingLA == null)
                {
                    Exception exception = new Exception(Resources.LANotFound)
                    {
                        Source = ExceptionType.Thrown.ToString()
                    };
                    throw exception;
                }

                existingLA.IsDeleted = true;

                ctx.SaveChanges();
            }
        }
        #endregion

        #region Team Managers

        public object GetManagers(int teamId)
        {
            using (var ctx = Entities.Create())
            {
                
                var excluded = GetMembers(teamId, Globals.TEAM_MANAGER_RC)
                    .Select(t => t.UserDetailsId).ToList();
                
                var team = ctx.Teams.SingleOrDefault(t => t.Id == teamId);

                var dataQuery = (from am in ctx.AccountMembers
                                 join u in ctx.vw_ActiveUsers on am.UserDetailsId equals u.UserDetailsId
                                 where u.Role == Globals.TEAM_MANAGER_RC && am.AccountId == team.AccountId && am.IsDeleted != true
                                 select new
                                 {
                                     Id = u.UserDetailsId,
                                     u.NetUserId,
                                     Text = u.FullName,
                                     u.Email
                                 });
                
                return dataQuery.Where(d => !excluded.Contains(d.Id))
                    .AsEnumerable()
                    .IOMDistinctBy(x => x.Id)
                    .ToList();
            }
        }

        public void RemoveManager(TeamMemberModel manager)
        {
            using (var ctx = Entities.Create())
            {
                TeamMember existingManager = ctx.TeamMembers.SingleOrDefault(m => m.UserDetailsId == manager.UserDetailsId && m.TeamId == manager.TeamId);

                if (existingManager != null)
                {
                    existingManager.UpdatedDateUtc = DateTime.UtcNow;
                    existingManager.IsDeleted = true;
                }

                ctx.SaveChanges();
            }
        }

        public async Task AddManagerAsync(TeamMemberModel manager)
        {
            using (var ctx = Entities.Create())
            {
                foreach (var userDetailsId in manager.UserIds)
                {
                    var existingManager = await ctx.TeamMembers
                        .SingleOrDefaultAsync(m => m.UserDetailsId == userDetailsId && m.TeamId == manager.TeamId)
                        .ConfigureAwait(false);

                    if (existingManager != null)
                    {
                        existingManager.UpdatedDateUtc = DateTime.UtcNow;
                        existingManager.IsDeleted = false;
                    }
                    else
                    {
                        ctx.TeamMembers.Add(new TeamMember
                        {
                            UserDetailsId = userDetailsId,
                            TeamId = manager.TeamId,
                            CreatedDateUtc = DateTime.UtcNow
                        });
                    }
                }

                await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
        }
        #endregion

        public async Task AssignTask(TeamTaskBase addTeamTask, string username)
        {
            var userInfo = GetCurrentUserInfo(username);

            using (var ctx = Entities.Create())
            {
                var existingTeamTask = (from tt in ctx.IOMTeamTasks
                                        where tt.TeamId == addTeamTask.TeamId && tt.TaskId == addTeamTask.TaskId
                                        select tt).SingleOrDefault();

                if (existingTeamTask == null)
                {
                    ctx.IOMTeamTasks.Add(new IOMTeamTask
                    {
                        TeamId = addTeamTask.TeamId,
                        TaskId = addTeamTask.TaskId,
                        CreatedBy = username,
                        CreatedDate = DateTime.UtcNow
                    });

                    #region notification
                    List<int> notifRecipientIds = new List<int>();
                    var task = ctx.IOMTasks.Where(a => a.Id == addTeamTask.TaskId).FirstOrDefault();
                    var team = ctx.Teams.Where(a => addTeamTask.TeamId == a.Id).FirstOrDefault();

                    // get managers
                    var notifTitle = $"A Task was assigned to team {team.Name} by {userInfo.RoleName}";
                    var notifMsg = $"Please be advised that {userInfo.FullName} had assigned the Task <b>{task.Name}</b> with Task ID <b>{task.TaskNumber}</b> to team {team.Name}.";

                    List<int> accIds = new List<int>();
                    accIds.Add(team.AccountId);
                    var accMngrs = GetAccountManagers(accIds);
                    var sysAdmins = GetAllSystemAdmins();

                    notifRecipientIds.AddRange(accMngrs.Select(a => a.UserDetailsId).ToList());
                    notifRecipientIds.AddRange(sysAdmins.Select(a => a.UserDetailsId).ToList());
                    notifRecipientIds.AddRange(ctx.sp_GetTeamManagersandSupervisors(team.Id).Select(a => a.UserId)
                        .ToList());

                    //// remove duplicates
                    notifRecipientIds = notifRecipientIds.Distinct().ToList();

                    ctx.Notifications.AddRange(notifRecipientIds.Select(a => new Notification()
                    {
                        ToUserId = a,
                        NoteDate = DateTime.UtcNow,
                        Icon = "fa-bell",
                        Title = notifTitle,
                        NoteType = NoteType.ReminderAgentStatus.ToString(),
                        Message = notifMsg
                    }).ToList());
                    #endregion
                }

                /**
                 * TODO: Assign task to team members
                 */

                await ctx.SaveChangesAsync();
            }
        }

        public async Task RemoveTask(int teamTaskId, string username)
        {
            var userInfo = GetCurrentUserInfo(username);

            using (var ctx = Entities.Create())
            {
                var existingTeamTask = (from tt in ctx.IOMTeamTasks
                                        where tt.Id == teamTaskId
                                        select tt).SingleOrDefault();

                if (existingTeamTask != null)
                {
                    ctx.IOMTeamTasks.Remove(existingTeamTask);

                    #region notification
                    List<int> notifRecipientIds = new List<int>();
                    var task = ctx.IOMTasks.Where(a => a.Id == existingTeamTask.TaskId).FirstOrDefault();
                    var team = ctx.Teams.Where(a => existingTeamTask.TeamId == a.Id).FirstOrDefault();

                    // get managers
                    var notifTitle = $"A Task was removed from team {team.Name} by {userInfo.RoleName}";
                    var notifMsg = $"Please be advised that {userInfo.FullName} removed the Task <b>{task.Name}</b> with Task ID <b>{task.TaskNumber}</b> from team {team.Name}.";
                    
                    List<int> accIds = new List<int>();
                    accIds.Add(team.AccountId);
                    var accMngrs = GetAccountManagers(accIds);
                    var sysAdmins = GetAllSystemAdmins();

                    notifRecipientIds.AddRange(accMngrs.Select(a => a.UserDetailsId).ToList());
                    notifRecipientIds.AddRange(sysAdmins.Select(a => a.UserDetailsId).ToList());
                    notifRecipientIds.AddRange(ctx.sp_GetTeamManagersandSupervisors(team.Id).Select(a => a.UserId)
                        .ToList());

                    //// remove duplicates
                    notifRecipientIds = notifRecipientIds.Distinct().ToList();

                    ctx.Notifications.AddRange(notifRecipientIds.Select(a => new Notification()
                    {
                        ToUserId = a,
                        NoteDate = DateTime.UtcNow,
                        Icon = "fa-bell",
                        Title = notifTitle,
                        NoteType = NoteType.ReminderAgentStatus.ToString(),
                        Message = notifMsg
                    }).ToList());
                    #endregion
                }

                /**
                * TODO: Unassign task to team members
                */

                await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public IList<LookUpModel> GetClients(int teamId, string query = "")
        {
            using (var ctx = Entities.Create())
            {
                query = query.ToLower();

                Team team = ctx.Teams.SingleOrDefault(t => t.Id == teamId);
                ServiceUtility.NullCheck(team);

                IList<LookUpModel> dataQuery = (from am in ctx.AccountMembers
                                                join u in ctx.vw_ActiveUsers on am.UserDetailsId equals u.UserDetailsId
                                                where u.Role == Globals.CLIENT_RC && am.IsDeleted != true
                                                    && am.AccountId == team.AccountId
                                                select new LookUpModel
                                                {
                                                    Id = u.UserDetailsId,
                                                    Text = u.FullName
                                                }).ToList();

                var currentCLIds = (from tm in ctx.TeamMembers
                                    join u in ctx.vw_ActiveUsers on tm.UserDetailsId equals u.UserDetailsId
                                    where u.Role == Globals.CLIENT_RC && tm.IsDeleted != true
                                    && tm.TeamId == teamId
                                    select tm.UserDetailsId).ToList();

                dataQuery = dataQuery.Where(d => !currentCLIds.Contains(d.Id)).ToList();

                if (query != null)
                {
                    dataQuery = dataQuery.Where(d => d.Text.Contains(query)).ToList();
                }

                return dataQuery;
            }
        }

        public void SaveTeam(TeamModel teamModel)
        {
            using (var ctx = Entities.Create())
            {
                var existingTeamName = ctx.Teams
                    .Where(t => t.AccountId == teamModel.AccountId
                             && t.Name == teamModel.Name && t.Id != teamModel.Id && t.IsDeleted != true)
                    .SingleOrDefault();

                if (existingTeamName != null)
                {
                    throw new Exception(message: Resources.TeamNameExist)
                    {
                        Source = ExceptionType.Thrown.ToString()
                    };
                }

                if (teamModel.Id > 0)
                {
                    var _existingTeam = ctx.Teams.Where(t => t.Id == teamModel.Id).SingleOrDefault();

                    _existingTeam.Name = teamModel.Name;
                    _existingTeam.Description = teamModel.Description;
                    _existingTeam.ShiftId = teamModel.ShiftId;
                }
                else
                {
                    ctx.Teams.Add(new Team
                    {
                        Name = teamModel.Name,
                        Description = teamModel.Description,
                        AccountId = (int)teamModel.AccountId,
                        ShiftId = teamModel.ShiftId,
                        IsActive = true,
                        Created = DateTime.UtcNow
                    });
                }

                ctx.SaveChanges();
            }
        }

        public TeamModel GetTeam(int teamId)
        {
            using (var ctx = Entities.Create())
            {
                return (from t in ctx.Teams

                        join a in ctx.Accounts on t.AccountId equals a.Id
                        from s in ctx.EmployeeShifts.Where(e => t.ShiftId == e.Id).DefaultIfEmpty()
                        where t.Id == teamId && t.IsDeleted != true && a.IsDeleted != true
                        select new TeamModel
                        {
                            Id = t.Id,
                            Name = t.Name,
                            AccountId = a.Id,
                            AccountName = a.Name,
                            Description = t.Description,
                            ShiftId = s.Id,
                            ShiftName = s.Name
                        }).SingleOrDefault();
            }
        }

        public void DeleteTeam(int teamId)
        {
            using (var ctx = Entities.Create())
            {
                ctx.sp_DeleteTeam(teamId);
            }
        }

        public void DeactivateTeam(int teamId)
        {
            using (var ctx = Entities.Create())
            {
                var team = ctx.Teams.SingleOrDefault(t => t.Id == teamId);

                if (team != null)
                {
                    team.IsActive = false;
                    ctx.SaveChanges();
                }
            }
        }

        public void ActivateTeam(int teamId)
        {
            using (var ctx = Entities.Create())
            {
                var team = ctx.Teams.SingleOrDefault(t => t.Id == teamId);

                if (team != null)
                {
                    team.IsActive = true;
                    ctx.SaveChanges();
                }
            }
        }

        public bool IsDeletable(int teamId)
        {
            using (var ctx = Entities.Create())
            {
                var teamCount = (from th in ctx.TaskHistories
                                 join tt in ctx.TeamTasks on th.TaskId equals tt.Id
                                 where tt.TeamId == teamId
                                 select th.Id).ToList().Count;

                return teamCount == 0;
            }
        }

        public IList<LookUpModel> GetTeamsByAccountId(int accountId, string query)
        {
            query = query.ToLower(CultureInfo.CurrentCulture);

            using (var ctx = Entities.Create())
            {
                var dataQuery = (from t in ctx.Teams
                                 from a in ctx.Accounts.Where(e => e.Id == t.AccountId).DefaultIfEmpty()
                                 where a.Id == accountId && t.IsActive == true && t.IsDeleted != true
                                 select new
                                 {
                                     t.Id,
                                     t.Name,
                                     AccountId = (int?)a.Id
                                 });

                dataQuery = dataQuery.IOMDistinctBy(d => d.Id).AsQueryable();

                return dataQuery.Select(d => new LookUpModel
                {
                    Id = d.Id,
                    Text = d.Name
                }).ToList();
            }
        }

        public async Task RemoveMemberEmail(IdentityMessage message,
            string userEmail,
            List<vw_ActiveUsers> sysAds)
        {

            List<EmailAddress> recipients = new List<EmailAddress>();

            var systemAdminsEmailRecipient = sysAds.Where(a => a.Role == Globals.SYSAD_RC)
                .Select(a => new EmailAddress()
                {
                    Name = a.FullName,
                    Email = a.Email
                }).ToList();

            var recipient = userEmail;

            recipients.AddRange(systemAdminsEmailRecipient);

            await SendRemoveMemberEmailUser(message, recipient, recipients).ConfigureAwait(false);
        }

        public async Task RemoveMemberEmail2(IdentityMessage message,
            string userEmail,
            List<vw_ActiveUsers> sysAds)
        {

            List<EmailAddress> recipients = new List<EmailAddress>();

            var systemAdminsEmailRecipient = sysAds.Where(a => a.Role == Globals.SYSAD_RC)
                .Select(a => new EmailAddress()
                {
                    Name = a.FullName,
                    Email = a.Email
                }).ToList();

            var recipient = userEmail;

            recipients.AddRange(systemAdminsEmailRecipient);

            await SendRemoveMemberEmailUser2(message, recipient, recipients).ConfigureAwait(false);
        }
    }
}
