using IOM.DbContext;
using IOM.Helpers;
using IOM.Models;
using IOM.Models.ApiControllerModels;
using IOM.Properties;
using IOM.Utilities;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using IOM.Services.Interface;
using System.Web.UI.WebControls;

namespace IOM.Services
{
    public partial class RepositoryService : IRepositoryService
    {
        public IList<AccountLookUp> AccountsLookup(string username)
        {
            using (var ctx = Entities.Create())
            {
                IEnumerable<AccountModel> dataQuery = GetAssignedAccountsRaw(username, false)
                    .Where(a => a.IsActive == true).ToList();

                return dataQuery.Select(d => new AccountLookUp
                {
                    Id = d.Id,
                    Text = d.Name
                }).ToList();
            }
        }

        public AccountDataModel GetAccountDetails(int accountId, string username)
        {

            //Get Primary ids of Assigned teams of currently logged in user
            IList<int> assignedTeams = GetAssignedTeamsList(username, false).Select(e => e.Id).ToList();

            //Get user info of currently logged in user
            var userInfo = GetCurrentUserInfo(username);

            IList<int> assignedUsers = UserList(userInfo.UserDetailsId)
                .Select(u => u.UserDetailsId).ToList();

            using (var ctx = Entities.Create())
            {
                var accountSeats = ctx.Seats.Where(a => a.AccountId == accountId).AsEnumerable();

                List<TeamUsersModel> leadAgents = new List<TeamUsersModel>();
                List<TeamUsersModel> agents = new List<TeamUsersModel>();

                IEnumerable<TeamUsersModel> accountAgents = new List<TeamUsersModel>();
                IEnumerable<TeamUsersModel> accountLAs = new List<TeamUsersModel>();

                IList<UserModel> accountManagers = new List<UserModel>();
                IList<UserModel> teamManagers = new List<UserModel>();
                IList<UserModel> clients = new List<UserModel>();

                IList<TeamModel> teams
                    = (from t in ctx.Teams
                       from s in ctx.EmployeeShifts.Where(e => t.ShiftId == e.Id).DefaultIfEmpty()
                       where assignedTeams.Contains(t.Id) && t.AccountId == accountId && t.IsDeleted != true
                                      && t.IsActive == true
                       select new TeamModel
                       {
                           Id = t.Id,
                           Name = t.Name,
                           Description = t.Description,
                           ShiftId = s.Id,
                           ShiftName = s.Name
                       }).ToList();

                if (userInfo.RoleCode != Globals.AGENT_RC)
                {
                    var teamAgts = (from tm in ctx.TeamMembers
                                    join t in ctx.Teams on tm.TeamId equals t.Id
                                    where assignedTeams.Contains(tm.TeamId)
                                    && tm.IsDeleted != true
                                    && assignedUsers.Contains(tm.UserDetailsId)
                                    && t.AccountId == accountId
                                    select new { UserDetailsId = tm.UserDetailsId, TeamId = tm.TeamId }).ToList();

                    var accountAgts = ctx.AccountMembers
                        .Where(a => a.AccountId == accountId && a.IsDeleted != true && assignedUsers.Contains(a.UserDetailsId))
                         .Select(a => new { UserDetailsId = a.UserDetailsId, AccountId = a.AccountId }).ToList();

                    leadAgents.AddRange((from ag in accountAgts
                                         join u in ctx.vw_ActiveUsers on ag.UserDetailsId equals u.UserDetailsId
                                         where u.Role == Globals.LEAD_AGENT_RC
                                         select new TeamUsersModel
                                         {
                                             UserDetailsId = u.UserDetailsId,
                                             NetUserId = u.NetUserId,
                                             FullName = u.FullName,
                                             TeamName = string.Empty,
                                             TeamId = 0,
                                             Email = u.Email,
                                             StaffId = u.StaffId,
                                             Seat = accountSeats.Where(a => a.UserId == u.UserDetailsId).Select(a => a.SeatNumber).FirstOrDefault()
                                         }).ToList());

                    leadAgents.AddRange((from tg in teamAgts
                                         join u in ctx.vw_ActiveUsers on tg.UserDetailsId equals u.UserDetailsId
                                         join t in ctx.Teams on tg.TeamId equals t.Id
                                         where u.Role == Globals.LEAD_AGENT_RC
                                         select new TeamUsersModel
                                         {
                                             UserDetailsId = u.UserDetailsId,
                                             NetUserId = u.NetUserId,
                                             FullName = u.FullName,
                                             TeamName = t.Name,
                                             TeamId = t.Id,
                                             Email = u.Email,
                                             StaffId = u.StaffId,
                                             Seat = accountSeats.Where(a => a.UserId == u.UserDetailsId).Select(a => a.SeatNumber).FirstOrDefault()
                                         }).ToList());

                    accountLAs
                        = (from am in ctx.AccountMembers
                           join u in ctx.vw_ActiveUsers on am.UserDetailsId equals u.UserDetailsId
                           where assignedUsers.Contains(u.UserDetailsId)
                                && am.AccountId == accountId && am.IsDeleted != true
                                && u.Role == Globals.LEAD_AGENT_RC
                           select new TeamUsersModel
                           {
                               UserDetailsId = u.UserDetailsId,
                               NetUserId = u.NetUserId,
                               FullName = u.FullName,
                               TeamName = string.Empty,
                               TeamId = 0,
                               Email = u.Email,
                               StaffId = u.StaffId,
                               Seat = accountSeats.Where(a => a.UserId == u.UserDetailsId).Select(a => a.SeatNumber).FirstOrDefault()
                           }).ToList();

                    agents.AddRange((from ag in accountAgts
                                     join u in ctx.vw_ActiveUsers on ag.UserDetailsId equals u.UserDetailsId
                                     where u.Role == Globals.AGENT_RC
                                     select new TeamUsersModel
                                     {
                                         UserDetailsId = u.UserDetailsId,
                                         NetUserId = u.NetUserId,
                                         FullName = u.FullName,
                                         TeamName = string.Empty,
                                         TeamId = 0,
                                         Email = u.Email,
                                         StaffId = u.StaffId,
                                         Seat = accountSeats.Where(a => a.UserId == u.UserDetailsId).Select(a => a.SeatNumber).FirstOrDefault()
                                     }).ToList());

                    agents.AddRange((from tg in teamAgts
                                     join u in ctx.vw_ActiveUsers on tg.UserDetailsId equals u.UserDetailsId
                                     join t in ctx.Teams on tg.TeamId equals t.Id
                                     where u.Role == Globals.AGENT_RC
                                     select new TeamUsersModel
                                     {
                                         UserDetailsId = u.UserDetailsId,
                                         NetUserId = u.NetUserId,
                                         FullName = u.FullName,
                                         TeamName = t.Name,
                                         TeamId = t.Id,
                                         Email = u.Email,
                                         StaffId = u.StaffId,
                                         Seat = accountSeats.Where(a => a.UserId == u.UserDetailsId).Select(a => a.SeatNumber).FirstOrDefault()
                                     }).ToList());

                    accountManagers
                        = (from am in ctx.AccountMembers
                           join u in ctx.vw_ActiveUsers on am.UserDetailsId equals u.UserDetailsId
                           where assignedUsers.Contains(u.UserDetailsId)
                                && am.AccountId == accountId && am.IsDeleted != true
                                && u.Role == Globals.ACCOUNT_MANAGER_RC
                           select new UserModel
                           {
                               UserDetailsId = u.UserDetailsId,
                               NetUserId = u.NetUserId,
                               FirstName = u.FirstName,
                               LastName = u.LastName,
                               RoleCode = u.Role,
                               FullName = u.FullName,
                               Email = u.Email

                           }).ToList();

                    teamManagers
                        = (from am in ctx.AccountMembers
                           join u in ctx.vw_ActiveUsers on am.UserDetailsId equals u.UserDetailsId
                           where assignedUsers.Contains(u.UserDetailsId)
                                && am.IsDeleted != true && am.AccountId == accountId
                                && u.Role == Globals.TEAM_MANAGER_RC
                           select new UserModel
                           {
                               UserDetailsId = u.UserDetailsId,
                               NetUserId = u.NetUserId,
                               FirstName = u.FirstName,
                               LastName = u.LastName,
                               RoleCode = u.Role,
                               FullName = u.FullName,
                               Email = u.Email,
                               StaffId = u.StaffId,
                               Seat = accountSeats.Where(a => a.UserId == u.UserDetailsId).Select(a => a.SeatNumber).FirstOrDefault()
                           }).ToList();

                    clients
                        = (from am in ctx.AccountMembers
                           join u in ctx.vw_ActiveUsers on am.UserDetailsId equals u.UserDetailsId
                           where assignedUsers.Contains(u.UserDetailsId)
                                && am.IsDeleted != true && am.AccountId == accountId
                                && u.Role == Globals.CLIENT_RC
                           select new UserModel
                           {
                               UserDetailsId = u.UserDetailsId,
                               NetUserId = u.NetUserId,
                               FirstName = u.FirstName,
                               LastName = u.LastName,
                               RoleCode = u.Role,
                               FullName = u.FullName,
                               Email = u.Email
                           }).ToList();
                }
                else
                {
                    agents
                        = (from tm in ctx.TeamMembers
                           join t in ctx.Teams on tm.TeamId equals t.Id
                           join u in ctx.vw_ActiveUsers on tm.UserDetailsId equals u.UserDetailsId
                           where assignedTeams.Contains(tm.TeamId)
                                && assignedUsers.Contains(u.UserDetailsId)
                                && t.AccountId == accountId && u.Role == Globals.AGENT_RC && t.IsDeleted != true
                                && tm.IsDeleted != true
                                && u.UserDetailsId == userInfo.UserDetailsId
                           select new TeamUsersModel
                           {
                               UserDetailsId = u.UserDetailsId,
                               NetUserId = u.NetUserId,
                               FullName = u.FullName,
                               TeamName = t.Name,
                               TeamId = t.Id,
                               Email = u.Email,
                               StaffId = u.StaffId,
                               Seat = accountSeats.Where(a => a.UserId == u.UserDetailsId).Select(a => a.SeatNumber).FirstOrDefault()
                           }).ToList();

                    accountAgents
                        = (from am in ctx.AccountMembers
                           join u in ctx.vw_ActiveUsers on am.UserDetailsId equals u.UserDetailsId
                           where am.AccountId == accountId
                                && assignedUsers.Contains(u.UserDetailsId)
                                && am.IsDeleted != true && u.Role == Globals.AGENT_RC
                                && u.UserDetailsId == userInfo.UserDetailsId
                           select new TeamUsersModel
                           {
                               UserDetailsId = u.UserDetailsId,
                               NetUserId = u.NetUserId,
                               FullName = u.FullName,
                               TeamName = string.Empty,
                               TeamId = 0,
                               Email = u.Email,
                               StaffId = u.StaffId,
                               Seat = accountSeats.Where(a => a.UserId == u.UserDetailsId).Select(a => a.SeatNumber).FirstOrDefault()
                           }).AsEnumerable();
                }

                agents.AddRange(accountAgents);
                leadAgents.AddRange(accountLAs);

                IList<TeamUsersModel> laCompiledList = CompileList(leadAgents);
                IList<TeamUsersModel> agCompiledList = CompileList(agents);

                AccountDataModel data = ctx.Accounts.Where(a => a.Id == accountId && a.IsDeleted != true)
                    .Select(e => new AccountDataModel
                    {
                        Id = e.Id,
                        Name = e.Name,
                        ContactPerson = e.ContactPerson,
                        EmailAddress = e.EmailAddress,
                        OfficeAddress = e.OfficeAddress,
                        Website = e.Website,
                        Created = e.Created,
                        SeatCode = e.SeatCode,
                        SeatSlot = e.SeatSlot


                    }).SingleOrDefault();

                data.ClientPOCs = clients;
                data.AccountManagers = accountManagers;
                data.TeamManagers = teamManagers;
                data.Teams = teams;
                data.Supervisors = laCompiledList;
                data.Agents = agCompiledList;

                return data;
            }
        }

        #region Team Managers

        public IList<LookUpModel> GetTeamManagers(int accountId, string query = "")
        {
            using (var ctx = Entities.Create())
            {
                query = query.ToLower();

                IList<LookUpModel> dataQuery = (from u in ctx.vw_ActiveUsers
                                                where u.Role == Globals.TEAM_MANAGER_RC
                                                select new LookUpModel
                                                {
                                                    Id = u.UserDetailsId,
                                                    Text = u.FullName
                                                }).ToList();

                var currentTeamManagerIds = (from am in ctx.AccountMembers
                                             join u in ctx.vw_ActiveUsers on am.UserDetailsId equals u.UserDetailsId
                                             where u.Role == Globals.TEAM_MANAGER_RC && am.IsDeleted != true
                                               && am.AccountId == accountId
                                             select am.UserDetailsId).ToList();

                dataQuery = dataQuery.Where(d => !currentTeamManagerIds.Contains(d.Id)).ToList();

                if (query != null)
                {
                    dataQuery = dataQuery.Where(d => d.Text.Contains(query)).ToList();
                }

                return dataQuery;
            }
        }

        public void RemoveTeamManager(AccountMemberModel manager)
        {
            using (var ctx = Entities.Create())
            {
                var existing = (from am in ctx.AccountMembers
                                join u in ctx.vw_ActiveUsers on am.UserDetailsId equals u.UserDetailsId
                                where u.Role == Globals.TEAM_MANAGER_RC && am.IsDeleted != true
                                    && am.UserDetailsId == manager.UserDetailsId
                                    && am.AccountId == manager.AccountId
                                select am)
                                .SingleOrDefault();

                if (existing != null)
                {
                    existing.IsDeleted = true;

                    RemoveTeamMembers(existing.AccountId, manager.UserDetailsId);
                }

                ctx.SaveChanges();
            }
        }

        public async Task AddManagerAsync(AccountMemberModel manager)
        {
            using (var ctx = Entities.Create())
            {
                foreach (var userDetailsId in manager.UserIds)
                {
                    var user = await ctx.UserDetails
                        .SingleOrDefaultAsync(u => u.Id == userDetailsId).ConfigureAwait(false);

                    if (user.Role != Globals.TEAM_MANAGER_RC)
                    {
                        var exception = new Exception(Resources.RoleLA)
                        {
                            Source = ExceptionType.Thrown.ToString()
                        };
                        throw exception;
                    }

                    var existingTm = await (from am in ctx.AccountMembers
                                            join u in ctx.vw_ActiveUsers on am.UserDetailsId equals u.UserDetailsId
                                            where u.Role == Globals.TEAM_MANAGER_RC
                                                  && am.UserDetailsId == userDetailsId
                                                  && am.AccountId == manager.AccountId
                                            select am).SingleOrDefaultAsync().ConfigureAwait(false);

                    if (existingTm != null)
                    {
                        existingTm.IsDeleted = false;

                        //  start send email to user

                        var systemAdmins = ctx.vw_ActiveUsers.Where(a => a.Role == Globals.SYSAD_RC).ToList();

                        var acc = ctx.Accounts.Where(a => a.Id == manager.AccountId).FirstOrDefault();

                        //int rUDId = int.Parse(userDetailsId);

                        var userdetail = ctx.UserDetails.Where(a => a.Id == userDetailsId).FirstOrDefault();

                        var userInfo = GetUserDetails(userDetailsId);

                        var emailmsgUser = string.Format("Please be advised that {0} was added to {1}",
                            userdetail.Name,
                            acc.Name);

                        string emailBodyUser = EmailBody.RemoveMember(emailmsgUser);

                        Task.Run(() => AddMemberEmail(new IdentityMessage()
                        {
                            Subject = "Added To Account",
                            Body = emailBodyUser
                        },
                        userInfo.Email,
                        systemAdmins)).Wait();

                        var emailmsgUser2 = string.Format("Please be advised that you were added to {1}",
                            userdetail.Name,
                            acc.Name);

                        string emailBodyUser2 = EmailBody.RemoveMember(emailmsgUser2);

                        Task.Run(() => AddMemberEmail2(new IdentityMessage()
                        {
                            Subject = "Added To Account",
                            Body = emailBodyUser2
                        },
                        userInfo.Email,
                        systemAdmins)).Wait();

                        // end send user email
                    }
                    else
                    {
                        ctx.AccountMembers.Add(new AccountMember
                        {
                            AccountId = manager.AccountId,
                            UserDetailsId = userDetailsId,
                            Created = DateTime.UtcNow
                        });

                        //  start send email to user

                        var systemAdmins = ctx.vw_ActiveUsers.Where(a => a.Role == Globals.SYSAD_RC).ToList();

                        var acc = ctx.Accounts.Where(a => a.Id == manager.AccountId).FirstOrDefault();

                        //int rUDId = int.Parse(userDetailsId);

                        var userdetail = ctx.UserDetails.Where(a => a.Id == userDetailsId).FirstOrDefault();

                        var userInfo = GetUserDetails(userDetailsId);

                        var emailmsgUser = string.Format("Please be advised that {0} was added to {1}",
                            userdetail.Name,
                            acc.Name);

                        string emailBodyUser = EmailBody.RemoveMember(emailmsgUser);

                        Task.Run(() => AddMemberEmail(new IdentityMessage()
                        {
                            Subject = "Added To Account",
                            Body = emailBodyUser
                        },
                        userInfo.Email,
                        systemAdmins)).Wait();

                        var emailmsgUser2 = string.Format("Please be advised that you were added to {1}",
                            userdetail.Name,
                            acc.Name);

                        string emailBodyUser2 = EmailBody.RemoveMember(emailmsgUser2);

                        Task.Run(() => AddMemberEmail2(new IdentityMessage()
                        {
                            Subject = "Added To Account",
                            Body = emailBodyUser2
                        },
                        userInfo.Email,
                        systemAdmins)).Wait();

                        // end send user email

                    }
                }

                await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
        }
        #endregion

        #region Lead Agents

        public IList<LookUpModel> GetLeadAgents(int accountId)
        {
            using (var ctx = Entities.Create())
            {
                IList<LookUpModel> dataQuery = (from u in ctx.vw_ActiveUsers
                                                where u.Role == Globals.LEAD_AGENT_RC
                                                select new LookUpModel
                                                {
                                                    Id = u.UserDetailsId,
                                                    Text = u.FullName
                                                }).ToList();

                var currentLAIds = (from am in ctx.AccountMembers
                                    join u in ctx.vw_ActiveUsers on am.UserDetailsId equals u.UserDetailsId
                                    where u.Role == Globals.LEAD_AGENT_RC && am.IsDeleted != true
                                    && am.AccountId == accountId
                                    select am.UserDetailsId).ToList();

                dataQuery = dataQuery.Where(d => !currentLAIds.Contains(d.Id)).ToList();

                return dataQuery;
            }
        }

        public void RemoveLeadAgent(AccountMemberModel leadAgent)
        {
            using (var ctx = Entities.Create())
            {
                var existing = (from am in ctx.AccountMembers
                                join u in ctx.vw_ActiveUsers on am.UserDetailsId equals u.UserDetailsId
                                where u.Role == Globals.LEAD_AGENT_RC && am.IsDeleted != true
                                    && am.UserDetailsId == leadAgent.UserDetailsId
                                    && am.AccountId == leadAgent.AccountId
                                select am)
                                .SingleOrDefault();

                if (existing != null)
                {
                    existing.IsDeleted = true;

                    RemoveTeamMembers(existing.AccountId, leadAgent.UserDetailsId);
                }

                ctx.SaveChanges();
            }
        }

        public async Task AddLeadAgentAsync(AccountMemberModel amModel)
        {
            using (var ctx = Entities.Create())
            {
                foreach (var userDetailsId in amModel.UserIds)
                {
                    var am = await ctx.AccountMembers
                        .SingleOrDefaultAsync(e =>
                            e.AccountId == amModel.AccountId && e.UserDetailsId == userDetailsId)
                        .ConfigureAwait(false);

                    if (am == null)
                    {
                        var userRole = await ctx.UserDetails.Where(u => u.Id == userDetailsId)
                            .Select(d => d.Role).SingleOrDefaultAsync().ConfigureAwait(false);

                        if (userRole != Globals.LEAD_AGENT_RC)
                        {
                            var exception = new Exception(Resources.RoleAccountManager)
                            {
                                Source = ExceptionType.Thrown.ToString()
                            };
                            throw exception;
                        }

                        ctx.AccountMembers.Add(new AccountMember
                        {
                            UserDetailsId = userDetailsId,
                            AccountId = amModel.AccountId,
                            Created = DateTime.UtcNow
                        });

                        //  start send email to user

                        var systemAdmins = ctx.vw_ActiveUsers.Where(a => a.Role == Globals.SYSAD_RC).ToList();

                        var acc = ctx.Accounts.Where(a => a.Id == amModel.AccountId).FirstOrDefault();

                        //int rUDId = int.Parse(userDetailsId);

                        var userdetail = ctx.UserDetails.Where(a => a.Id == userDetailsId).FirstOrDefault();

                        var userInfo = GetUserDetails(userDetailsId);

                        var emailmsgUser = string.Format("Please be advised that {0} was added to {1}",
                            userdetail.Name,
                            acc.Name);

                        string emailBodyUser = EmailBody.RemoveMember(emailmsgUser);

                        Task.Run(() => AddMemberEmail(new IdentityMessage()
                        {
                            Subject = "Added To Account",
                            Body = emailBodyUser
                        },
                        userInfo.Email,
                        systemAdmins)).Wait();

                        var emailmsgUser2 = string.Format("Please be advised that you were added to {1}",
                            userdetail.Name,
                            acc.Name);

                        string emailBodyUser2 = EmailBody.RemoveMember(emailmsgUser2);

                        Task.Run(() => AddMemberEmail2(new IdentityMessage()
                        {
                            Subject = "Added To Account",
                            Body = emailBodyUser2
                        },
                        userInfo.Email,
                        systemAdmins)).Wait();

                        // end send user email

                    }
                    else
                    {
                        am.IsDeleted = false;

                        //  start send email to user

                        var systemAdmins = ctx.vw_ActiveUsers.Where(a => a.Role == Globals.SYSAD_RC).ToList();

                        var acc = ctx.Accounts.Where(a => a.Id == amModel.AccountId).FirstOrDefault();

                        //int rUDId = int.Parse(userDetailsId);

                        var userdetail = ctx.UserDetails.Where(a => a.Id == userDetailsId).FirstOrDefault();

                        var userInfo = GetUserDetails(userDetailsId);

                        var emailmsgUser = string.Format("Please be advised that {0} was added to {1}",
                            userdetail.Name,
                            acc.Name);

                        string emailBodyUser = EmailBody.RemoveMember(emailmsgUser);

                        Task.Run(() => AddMemberEmail(new IdentityMessage()
                        {
                            Subject = "Added To Account",
                            Body = emailBodyUser
                        },
                        userInfo.Email,
                        systemAdmins)).Wait();

                        var emailmsgUser2 = string.Format("Please be advised that you were added to {1}",
                            userdetail.Name,
                            acc.Name);

                        string emailBodyUser2 = EmailBody.RemoveMember(emailmsgUser2);

                        Task.Run(() => AddMemberEmail2(new IdentityMessage()
                        {
                            Subject = "Added To Account",
                            Body = emailBodyUser2
                        },
                        userInfo.Email,
                        systemAdmins)).Wait();

                        // end send user email
                    }
                }

                await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
        }
        #endregion

        #region Account Managers

        public IList<LookUpModel> GetAccountManagers(int accountId, string key = "")
        {
            using (var ctx = Entities.Create())
            {
                IList<int> existingAM = (from am in ctx.AccountMembers
                                         join u in ctx.vw_ActiveUsers on am.UserDetailsId equals u.UserDetailsId
                                         where am.AccountId == accountId && u.Role == Globals.ACCOUNT_MANAGER_RC
                                            && am.IsDeleted != true
                                         select u.UserDetailsId).ToList();

                IList<LookUpModel> dataQuery = (from u in ctx.vw_ActiveUsers
                                                where u.Role == Globals.ACCOUNT_MANAGER_RC
                                                    && !existingAM.Contains(u.UserDetailsId)
                                                select new LookUpModel
                                                {
                                                    Id = u.UserDetailsId,
                                                    Text = u.FullName,
                                                    Description = u.NetUserId
                                                }).ToList();

                if (key != null && key.Length > 0)
                {
                    dataQuery = dataQuery.Where(d => d.Text.Contains(key)).ToList();
                }

                return dataQuery;
            }
        }

        public IList<UserBasicInfoModel> GetAccountManagers(List<int> accountId)
        {
            using (var ctx = Entities.Create())
            {
                IList<UserBasicInfoModel> dataQuery = (from am in ctx.AccountMembers
                                                       join u in ctx.vw_ActiveUsers on am.UserDetailsId equals u.UserDetailsId
                                                       where accountId.Contains(am.AccountId) && u.Role == Globals.ACCOUNT_MANAGER_RC
                                                          && am.IsDeleted != true
                                                       select new UserBasicInfoModel
                                                       {
                                                           UserDetailsId = u.UserDetailsId,
                                                           Name = u.FullName,
                                                           UserId = u.NetUserId,
                                                           Email = u.Email
                                                       }).ToList();

                return dataQuery;
            }
        }

        public List<vw_ActiveUsers> GetAccountManagersFullDetails(int accountId, string key = "")
        {
            using (var ctx = Entities.Create())
            {
                IList<int> existingAM = (from am in ctx.AccountMembers
                                         join u in ctx.vw_ActiveUsers on am.UserDetailsId equals u.UserDetailsId
                                         where am.AccountId == accountId && u.Role == Globals.ACCOUNT_MANAGER_RC
                                            && am.IsDeleted != true
                                         select u.UserDetailsId).ToList();


                var dataQuery = (from u in ctx.vw_ActiveUsers
                                 where u.Role == Globals.ACCOUNT_MANAGER_RC
                                 && existingAM.Contains(u.UserDetailsId)
                                 select u).ToList();

                if (key != null && key.Length > 0)
                {
                    dataQuery = dataQuery.Where(d => d.FullName.Contains(key)).ToList();
                }

                return dataQuery;
            }
        }

        public async Task AddAccountManagerAsync(AccountMemberModel amModel)
        {
            using (var ctx = Entities.Create())
            {
                foreach (var userDetailsId in amModel.UserIds)
                {
                    var am = await ctx.AccountMembers
                        .SingleOrDefaultAsync(e => e.AccountId == amModel.AccountId && e.UserDetailsId == userDetailsId)
                        .ConfigureAwait(false);

                    if (am == null)
                    {
                        var userRole = await ctx.UserDetails.Where(u => u.Id == userDetailsId)
                            .Select(d => d.Role).SingleOrDefaultAsync().ConfigureAwait(false);

                        if (userRole != Globals.ACCOUNT_MANAGER_RC)
                        {
                            var exception = new Exception(Resources.RoleAccountManager)
                            {
                                Source = ExceptionType.Thrown.ToString()
                            };
                            throw exception;
                        }

                        ctx.AccountMembers.Add(new AccountMember
                        {
                            UserDetailsId = userDetailsId,
                            AccountId = amModel.AccountId,
                            Created = DateTime.UtcNow
                        });

                        //  start send email to user

                        var systemAdmins = ctx.vw_ActiveUsers.Where(a => a.Role == Globals.SYSAD_RC).ToList();

                        var acc = ctx.Accounts.Where(a => a.Id == amModel.AccountId).FirstOrDefault();

                        //int rUDId = int.Parse(userDetailsId);

                        var userdetail = ctx.UserDetails.Where(a => a.Id == userDetailsId).FirstOrDefault();

                        var userInfo = GetUserDetails(userDetailsId);

                        var emailmsgUser = string.Format("Please be advised that {0} was added to {1}",
                            userdetail.Name,
                            acc.Name);

                        string emailBodyUser = EmailBody.RemoveMember(emailmsgUser);

                        Task.Run(() => AddMemberEmail(new IdentityMessage()
                        {
                            Subject = "Added To Account",
                            Body = emailBodyUser
                        },
                        userInfo.Email,
                        systemAdmins)).Wait();

                        var emailmsgUser2 = string.Format("Please be advised that you were added to {1}",
                            userdetail.Name,
                            acc.Name);

                        string emailBodyUser2 = EmailBody.RemoveMember(emailmsgUser2);

                        Task.Run(() => AddMemberEmail2(new IdentityMessage()
                        {
                            Subject = "Added To Account",
                            Body = emailBodyUser2
                        },
                        userInfo.Email,
                        systemAdmins)).Wait();

                        // end send user email

                    }
                    else
                    {
                        am.IsDeleted = false;

                        //  start send email to user

                        var systemAdmins = ctx.vw_ActiveUsers.Where(a => a.Role == Globals.SYSAD_RC).ToList();

                        var acc = ctx.Accounts.Where(a => a.Id == amModel.AccountId).FirstOrDefault();

                        //int rUDId = int.Parse(userDetailsId);

                        var userdetail = ctx.UserDetails.Where(a => a.Id == userDetailsId).FirstOrDefault();

                        var userInfo = GetUserDetails(userDetailsId);

                        var emailmsgUser = string.Format("Please be advised that {0} was added to {1}",
                            userdetail.Name,
                            acc.Name);

                        string emailBodyUser = EmailBody.RemoveMember(emailmsgUser);

                        Task.Run(() => AddMemberEmail(new IdentityMessage()
                        {
                            Subject = "Added To Account",
                            Body = emailBodyUser
                        },
                        userInfo.Email,
                        systemAdmins)).Wait();

                        var emailmsgUser2 = string.Format("Please be advised that you were added to {1}",
                            userdetail.Name,
                            acc.Name);

                        string emailBodyUser2 = EmailBody.RemoveMember(emailmsgUser2);

                        Task.Run(() => AddMemberEmail2(new IdentityMessage()
                        {
                            Subject = "Added To Account",
                            Body = emailBodyUser2
                        },
                        userInfo.Email,
                        systemAdmins)).Wait();

                        // end send user email
                    }
                }

                await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public void RemoveAccountManager(AccountMemberModel amModel)
        {
            using (var ctx = Entities.Create())
            {
                var am = (from a in ctx.AccountMembers
                          join u in ctx.vw_ActiveUsers on a.UserDetailsId equals u.UserDetailsId
                          where a.AccountId == amModel.AccountId && a.UserDetailsId == amModel.UserDetailsId
                            && u.Role == Globals.ACCOUNT_MANAGER_RC
                          select a)
                          .SingleOrDefault();

                if (am == null)
                {
                    throw new Exception(Resources.DataNotFound)
                    {
                        Source = ExceptionType.Thrown.ToString()
                    };
                }
                else
                {
                    am.IsDeleted = true;

                    RemoveTeamMembers(am.AccountId, amModel.UserDetailsId);
                }

                ctx.SaveChanges();
            }
        }
        #endregion

        #region Clients

        public IList<LookUpModel> GetClients(string key, int accountId)
        {
            using (var ctx = Entities.Create())
            {
                IList<int> existingAM = (from am in ctx.AccountMembers
                                         join u in ctx.vw_ActiveUsers on am.UserDetailsId equals u.UserDetailsId
                                         where am.AccountId == accountId && u.Role == Globals.CLIENT_RC
                                            && am.IsDeleted != true
                                         select u.UserDetailsId).ToList();

                IList<LookUpModel> dataQuery = (from u in ctx.vw_ActiveUsers
                                                where u.Role == Globals.CLIENT_RC
                                                    && !existingAM.Contains(u.UserDetailsId)
                                                select new LookUpModel
                                                {
                                                    Id = u.UserDetailsId,
                                                    Text = u.FullName,
                                                    Description = u.NetUserId
                                                }).ToList();

                if (key != null && key.Length > 0)
                {
                    dataQuery = dataQuery.Where(d => d.Text.Contains(key)).ToList();
                }

                return dataQuery;
            }
        }

        public async Task AddClientAsync(Personel client)
        {
            using (var ctx = Entities.Create())
            {
                foreach (var userDetailsId in client.UserIds)
                {
                    var cl = await ctx.AccountMembers
                        .SingleOrDefaultAsync(e =>
                            e.AccountId == client.AccountId && e.UserDetailsId == userDetailsId)
                        .ConfigureAwait(false);

                    if (cl == null)
                    {
                        var userRole = await ctx.UserDetails.Where(u => u.Id == userDetailsId)
                            .Select(d => d.Role).SingleOrDefaultAsync().ConfigureAwait(false);

                        if (userRole != Globals.CLIENT_RC)
                        {
                            Exception exception = new Exception(Resources.RoleClient)
                            {
                                Source = ExceptionType.Thrown.ToString()
                            };
                            throw exception;
                        }

                        ctx.AccountMembers.Add(new AccountMember
                        {
                            UserDetailsId = userDetailsId,
                            AccountId = client.AccountId,
                            Created = DateTime.UtcNow
                        });

                        //  start send email to user

                        var systemAdmins = ctx.vw_ActiveUsers.Where(a => a.Role == Globals.SYSAD_RC).ToList();

                        var acc = ctx.Accounts.Where(a => a.Id == client.AccountId).FirstOrDefault();

                        //int rUDId = int.Parse(userDetailsId);

                        var userdetail = ctx.UserDetails.Where(a => a.Id == userDetailsId).FirstOrDefault();

                        var userInfo = GetUserDetails(userDetailsId);

                        var emailmsgUser = string.Format("Please be advised that {0} was added to {1}",
                            userdetail.Name,
                            acc.Name);

                        string emailBodyUser = EmailBody.RemoveMember(emailmsgUser);

                        Task.Run(() => AddMemberEmail(new IdentityMessage()
                        {
                            Subject = "Added To Account",
                            Body = emailBodyUser
                        },
                        userInfo.Email,
                        systemAdmins)).Wait();

                        var emailmsgUser2 = string.Format("Please be advised that you were added to {1}",
                            userdetail.Name,
                            acc.Name);

                        string emailBodyUser2 = EmailBody.RemoveMember(emailmsgUser2);

                        Task.Run(() => AddMemberEmail2(new IdentityMessage()
                        {
                            Subject = "Added To Account",
                            Body = emailBodyUser2
                        },
                        userInfo.Email,
                        systemAdmins)).Wait();

                        // end send user email

                    }
                    else
                    {
                        cl.IsDeleted = false;

                        //  start send email to user

                        var systemAdmins = ctx.vw_ActiveUsers.Where(a => a.Role == Globals.SYSAD_RC).ToList();

                        var acc = ctx.Accounts.Where(a => a.Id == client.AccountId).FirstOrDefault();

                        //int rUDId = int.Parse(userDetailsId);

                        var userdetail = ctx.UserDetails.Where(a => a.Id == userDetailsId).FirstOrDefault();

                        var userInfo = GetUserDetails(userDetailsId);

                        var emailmsgUser = string.Format("Please be advised that {0} was added to {1}",
                            userdetail.Name,
                            acc.Name);

                        string emailBodyUser = EmailBody.RemoveMember(emailmsgUser);

                        Task.Run(() => AddMemberEmail(new IdentityMessage()
                        {
                            Subject = "Added To Account",
                            Body = emailBodyUser
                        },
                        userInfo.Email,
                        systemAdmins)).Wait();

                        var emailmsgUser2 = string.Format("Please be advised that you were added to {1}",
                            userdetail.Name,
                            acc.Name);

                        string emailBodyUser2 = EmailBody.RemoveMember(emailmsgUser2);

                        Task.Run(() => AddMemberEmail2(new IdentityMessage()
                        {
                            Subject = "Added To Account",
                            Body = emailBodyUser2
                        },
                        userInfo.Email,
                        systemAdmins)).Wait();

                        // end send user email
                    }
                }

                await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public void RemoveClient(AccountMemberModel amModel)
        {
            using (var ctx = Entities.Create())
            {
                var am = (from a in ctx.AccountMembers
                          join u in ctx.vw_ActiveUsers on a.UserDetailsId equals u.UserDetailsId
                          where a.AccountId == amModel.AccountId && a.UserDetailsId == amModel.UserDetailsId
                            && u.Role == Globals.CLIENT_RC
                          select a)
                          .SingleOrDefault();

                if (am == null)
                {
                    throw new Exception(Resources.DataNotFound)
                    {
                        Source = ExceptionType.Thrown.ToString()
                    };
                }
                else
                {
                    am.IsDeleted = true;

                    RemoveTeamMembers(am.AccountId, amModel.UserDetailsId);
                }

                ctx.SaveChanges();
            }
        }
        #endregion

        public void RemoveAgent(AccountUserModel accUserModel)
        {
            using (var ctx = Entities.Create())
            {

                var accountId = accUserModel.AccountId;

                (from am in ctx.AccountMembers
                 join u in ctx.vw_ActiveUsers on am.UserDetailsId equals u.UserDetailsId
                 where am.AccountId == accUserModel.AccountId && u.Role == Globals.AGENT_RC
                 && u.NetUserId == accUserModel.UserId
                 select am)
                 .ToList()
                 .ForEach(d =>
                 {
                     d.IsDeleted = true;

                     RemoveTeamMembers(accountId, d.UserDetailsId);

                 });

                var teamIds = ctx.Teams.Where(t => t.AccountId == accUserModel.AccountId)
                    .Select(d => d.Id).ToList();

                (from tm in ctx.TeamMembers
                 join u in ctx.vw_ActiveUsers on tm.UserDetailsId equals u.UserDetailsId
                 where teamIds.Any(t => t == tm.TeamId) && u.Role == Globals.AGENT_RC
                 && u.NetUserId == accUserModel.UserId
                 select tm).ForEach(d =>
                 {
                     d.IsDeleted = true;
                 });

                /*int.TryParse(accUserModel.UserId, out var _uId);
                var user = ctx.UserDetails.SingleOrDefault(u => u.Id == _uId);
                var userId = user.Id;*/

                ctx.SaveChanges();
            }
        }

        public async Task AddAgentAsync(AddAgentRequest addAgentRequest)
        {
            using (var ctx = Entities.Create())
            {
                foreach (var userDetailsId in addAgentRequest.UserIds)
                {
                    var user = await ctx.UserDetails
                        .SingleOrDefaultAsync(u => u.UserId == userDetailsId).ConfigureAwait(false);

                    if (user.Role != Globals.AGENT_RC)
                    {
                        throw new Exception(Resources.RoleAgent)
                        {
                            Source = ExceptionType.Thrown.ToString()
                        };
                    }

                    var accountMember = ctx.AccountMembers
                        .SingleOrDefault(a => a.AccountId == addAgentRequest.AccountId && a.UserDetailsId == user.Id);

                    if (accountMember == null)
                    {
                        ctx.AccountMembers.Add(new AccountMember
                        {
                            AccountId = addAgentRequest.AccountId,
                            UserDetailsId = user.Id,
                            Created = DateTimeUtility.Instance.DateTimeNow()
                        });

                        //  start send email to user

                        var systemAdmins = ctx.vw_ActiveUsers.Where(a => a.Role == Globals.SYSAD_RC).ToList();

                        var acc = ctx.Accounts.Where(a => a.Id == addAgentRequest.AccountId).FirstOrDefault();

                        int rUDId = int.Parse(userDetailsId);

                        var userdetail = ctx.UserDetails.Where(a => a.Id == rUDId).FirstOrDefault();

                        var userInfo = GetUserDetails(rUDId);

                        var emailmsgUser = string.Format("Please be advised that {0} was added to {1}",
                            userdetail.Name,
                            acc.Name);

                        string emailBodyUser = EmailBody.RemoveMember(emailmsgUser);

                        Task.Run(() => AddMemberEmail(new IdentityMessage()
                        {
                            Subject = "Added To Account",
                            Body = emailBodyUser
                        },
                        userInfo.Email,
                        systemAdmins)).Wait();

                        var emailmsgUser2 = string.Format("Please be advised that you were added to {1}",
                            userdetail.Name,
                            acc.Name);

                        string emailBodyUser2 = EmailBody.RemoveMember(emailmsgUser2);

                        Task.Run(() => AddMemberEmail2(new IdentityMessage()
                        {
                            Subject = "Added To Account",
                            Body = emailBodyUser2
                        },
                        userInfo.Email,
                        systemAdmins)).Wait();

                        // end send user email

                    }
                    else
                    {
                        accountMember.IsDeleted = false;

                        //  start send email to user

                        var systemAdmins = ctx.vw_ActiveUsers.Where(a => a.Role == Globals.SYSAD_RC).ToList();

                        var acc = ctx.Accounts.Where(a => a.Id == addAgentRequest.AccountId).FirstOrDefault();

                        int rUDId = int.Parse(userDetailsId);

                        var userdetail = ctx.UserDetails.Where(a => a.Id == rUDId).FirstOrDefault();

                        var userInfo = GetUserDetails(rUDId);

                        var emailmsgUser = string.Format("Please be advised that {0} was added to {1}",
                            userdetail.Name,
                            acc.Name);

                        string emailBodyUser = EmailBody.RemoveMember(emailmsgUser);

                        Task.Run(() => AddMemberEmail(new IdentityMessage()
                        {
                            Subject = "Added To Account",
                            Body = emailBodyUser
                        },
                        userInfo.Email,
                        systemAdmins)).Wait();

                        var emailmsgUser2 = string.Format("Please be advised that you were added to {1}",
                            userdetail.Name,
                            acc.Name);

                        string emailBodyUser2 = EmailBody.RemoveMember(emailmsgUser2);

                        Task.Run(() => AddMemberEmail2(new IdentityMessage()
                        {
                            Subject = "Added To Account",
                            Body = emailBodyUser2
                        },
                        userInfo.Email,
                        systemAdmins)).Wait();

                        // end send user email
                    }

                    foreach (var teamId in addAgentRequest.TeamIds)
                    {
                        var agent = await (from tm in ctx.TeamMembers
                                           join u in ctx.vw_ActiveUsers on tm.UserDetailsId equals u.UserDetailsId
                                           where tm.TeamId == teamId && u.Role == Globals.AGENT_RC &&
                                                 u.NetUserId == addAgentRequest.UserId
                                           select tm).SingleOrDefaultAsync().ConfigureAwait(false);

                        if (agent == null)
                        {
                            agent = new TeamMember
                            {
                                UserDetailsId = user.Id,
                                TeamId = teamId,
                                CreatedDateUtc = DateTime.UtcNow
                            };

                            ctx.TeamMembers.Add(agent);
                        }
                        else
                        {
                            agent.IsDeleted = false;
                        }
                    }
                }

                await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public IList<LookUpModel> GetAssignedAccountsList(string username, bool includeDeleted, string query = "")
        {
            var userInfo = GetCurrentUserInfo(username);
            query = query.ToUpperInvariant();

            if (userInfo.RoleCode == "SA")
            {
                using (var ctx = Entities.Create())
                {
                    if (!includeDeleted)
                    {
                        return ctx.Accounts.Where(a => a.IsDeleted != true).Select(a => new LookUpModel()
                        {
                            Id = a.Id,
                            Text = a.Name
                        }).ToList();
                    }
                    else
                    {
                        return ctx.Accounts.Select(a => new LookUpModel()
                        {
                            Id = a.Id,
                            Text = a.Name
                        }).ToList();
                    }
                }
            }
            else
            {
                IEnumerable<AccountModel> dataQuery = GetAssignedAccountsRaw(username, includeDeleted, query);

                return dataQuery.Select(d => new LookUpModel
                {
                    Id = d.Id,
                    Text = d.Name
                }).ToList();
            }
        }

        public IEnumerable<AccountModel> GetAssignedAccountsRaw(string username, bool includeDeleted, string query = "")
        {
            var userInfo = GetCurrentUserInfo(username);
            query = query.ToUpperInvariant();

            using (var ctx = Entities.Create())
            {
                IEnumerable<AccountModel> dataQuery = (from a in ctx.Accounts
                                                       select new AccountModel
                                                       {
                                                           Id = a.Id,
                                                           Name = a.Name,
                                                           IsActive = a.IsActive,
                                                           IsDeleted = a.IsDeleted
                                                       }).ToList().AsQueryable();

                if (!includeDeleted)
                {
                    dataQuery = dataQuery.Where(a => a.IsDeleted != true);
                }

                if (query.Length > 0)
                {
                    try
                    {
                        dataQuery = dataQuery.Where(e => e.Name.ToUpperInvariant().Contains(query) ||
                                                     query.Contains(e.Name.ToUpperInvariant()));
                    }
                    catch (Exception)
                    {

                        throw;
                    }

                }

                switch (userInfo.RoleCode)
                {
                    case Globals.AGENT_RC:
                    case Globals.TEAM_MANAGER_RC:
                    case Globals.LEAD_AGENT_RC:
                    case Globals.ACCOUNT_MANAGER_RC:
                        var agentAcc = ctx.AccountMembers
                        .Where(a => a.UserDetailsId == userInfo.UserDetailsId && a.IsDeleted != true)
                        .Select(a => a.AccountId)
                        .ToList();

                        dataQuery = dataQuery.Where(e => agentAcc.Any(t => t == e.Id));
                        break;
                    case Globals.CLIENT_RC:
                        var accountIds = GetMembers(userInfo.UserDetailsId, Globals.CLIENT_RC)
                            .Select(d => d.AccountId);

                        dataQuery = dataQuery.Where(e => accountIds.Any(t => t == e.Id));
                        break;
                }

                return dataQuery.IOMDistinctBy(d => d.Id).AsQueryable();
            }
        }

        public IList<BaseModel> GetAccounts(int userDetailsId)
        {
            using (var ctx = Entities.Create())
            {
                var result
                    = (from am in ctx.AccountMembers
                       join a in ctx.Accounts on am.AccountId equals a.Id
                       where am.UserDetailsId == userDetailsId && am.IsDeleted != true
                         && a.IsActive == true
                       select new BaseModel
                       {
                           Id = a.Id,
                           Name = a.Name
                       }).ToList();

                return result;
            }
        }

        public void ChangeAgentAccount(ChangeAccountModel model)
        {
            using (var ctx = Entities.Create())
            {
                var user = ctx.UserDetails.SingleOrDefault(u => u.Id == model.UserDetailsId);
                ServiceUtility.NullCheck(user, Resources.UserNotFound);

                var oldAGAccounts = (from am in ctx.AccountMembers
                                     join u in ctx.vw_ActiveUsers on am.UserDetailsId equals u.UserDetailsId
                                     where u.Role == user.Role && am.UserDetailsId == model.UserDetailsId
                                     select am).ToList();

                oldAGAccounts
                    .ForEach(r =>
                    {
                        (from tm in ctx.TeamMembers
                         join t in ctx.Teams on tm.TeamId equals t.Id
                         join a in ctx.Accounts on t.AccountId equals a.Id
                         where a.Id == r.AccountId && tm.UserDetailsId == user.Id
                         select tm)
                         .ForEach(t =>
                         {
                             t.IsDeleted = true;
                         });

                        r.IsDeleted = true;
                    });

                model.AccountIds.ForEach(a =>
                {
                    var existing = ctx.AccountMembers
                        .SingleOrDefault(am => am.AccountId == a
                            && am.UserDetailsId == model.UserDetailsId);

                    if (existing != null)
                    {
                        existing.IsDeleted = false;
                    }
                    else
                    {
                        ctx.AccountMembers.Add(new AccountMember
                        {
                            UserDetailsId = model.UserDetailsId,
                            AccountId = a,
                            Created = DateTimeUtility.Instance.DateTimeNow()
                        });
                    }
                });

                ctx.SaveChanges();
            }
        }

        public void SaveAccount(AccountDataModel accountModel)
        {
            if (accountModel == null)
            {
                throw new ArgumentNullException(nameof(accountModel));
            }

            using (var ctx = Entities.Create())
            {
                var transaction = ctx.Database.BeginTransaction();

                if (accountModel.Id > 0)
                {
                    if (IsNameDuplicate(accountModel.Name, accountId: accountModel.Id))
                    {
                        throw new Exception(Resources.AccountNameDuplicate);
                    }

                    var _existingAccount = ctx.Accounts.Where(a => a.Id == accountModel.Id).SingleOrDefault();

                    var _existingAccMgrs = (from am in ctx.AccountMembers
                                            join u in ctx.vw_ActiveUsers on am.UserDetailsId equals u.UserDetailsId
                                            where am.AccountId == accountModel.Id && u.Role == Globals.ACCOUNT_MANAGER_RC
                                            select am).AsEnumerable();

                    var _existingClientPOCs = (from am in ctx.AccountMembers
                                               join u in ctx.vw_ActiveUsers on am.UserDetailsId equals u.UserDetailsId
                                               where am.AccountId == accountModel.Id && u.Role == Globals.CLIENT_RC
                                               select am).AsEnumerable();

                    ctx.AccountMembers.RemoveRange(_existingAccMgrs);
                    ctx.AccountMembers.RemoveRange(_existingClientPOCs);

                    _existingAccount.Name = accountModel.Name;
                    _existingAccount.ContactPerson = accountModel.ContactPerson;
                    _existingAccount.EmailAddress = accountModel.EmailAddress;
                    _existingAccount.OfficeAddress = accountModel.OfficeAddress;
                    _existingAccount.Website = accountModel.Website;

                    if (_existingAccount.SeatSlot > accountModel.SeatSlot)
                    {
                        var excessSeats = ctx.Seats.Where(a => a.AccountId == accountModel.Id && a.SeatNumber > accountModel.SeatSlot).ToList();

                        ctx.Seats.RemoveRange(excessSeats);
                    }

                    _existingAccount.SeatSlot = accountModel.SeatSlot;
                    _existingAccount.SeatCode = accountModel.SeatCode;

                    ctx.SaveChanges();

                    foreach (UserModel accMgr in accountModel.AccountManagers)
                    {
                        ctx.AccountMembers.Add(new AccountMember
                        {
                            AccountId = accountModel.Id,
                            UserDetailsId = accMgr.UserDetailsId,
                            Created = DateTime.UtcNow
                        });
                    }

                    foreach (UserModel clientPOC in accountModel.ClientPOCs)
                    {
                        ctx.AccountMembers.Add(new AccountMember
                        {
                            AccountId = accountModel.Id,
                            UserDetailsId = clientPOC.UserDetailsId,
                            Created = DateTime.UtcNow
                        });
                    }
                }
                else
                {
                    if (IsNameDuplicate(accountModel.Name))
                    {
                        throw new Exception(Resources.AccountNameDuplicate);
                    }

                    var a = new Account
                    {
                        Name = accountModel.Name,
                        ContactPerson = accountModel.ContactPerson,
                        EmailAddress = accountModel.EmailAddress,
                        OfficeAddress = accountModel.OfficeAddress,
                        Website = accountModel.Website,
                        Created = DateTimeUtility.Instance.DateTimeNow(),
                        IsActive = true,
                        SeatCode = accountModel.SeatCode,
                        SeatSlot = accountModel.SeatSlot
                    };

                    ctx.Accounts.Add(a);

                    ctx.SaveChanges();

                    foreach (UserModel accMgr in accountModel.AccountManagers)
                    {
                        ctx.AccountMembers.Add(new AccountMember
                        {
                            AccountId = a.Id,
                            UserDetailsId = accMgr.UserDetailsId,
                            Created = DateTime.UtcNow
                        });
                    }

                    foreach (UserModel clientPOC in accountModel.ClientPOCs)
                    {
                        ctx.AccountMembers.Add(new AccountMember
                        {
                            AccountId = a.Id,
                            UserDetailsId = clientPOC.UserDetailsId,
                            Created = DateTime.UtcNow
                        });
                    }
                }

                ctx.SaveChanges();

                transaction.Commit();
            }
        }

        public void SaveAccountSeatDetails(AccountDataModel accountModel, string userActionId, string siteurl)
        {
            if (accountModel == null)
            {
                throw new ArgumentNullException(nameof(accountModel));
            }

            List<EmailAddress> recipients = new List<EmailAddress>();

            using (var ctx = Entities.Create())
            {
                var transaction = ctx.Database.BeginTransaction();
                var userAction = ctx.UserDetails.Where(a => a.UserId == userActionId).FirstOrDefault();

                if (accountModel.Id > 0)
                {
                    var account = ctx.Accounts.Where(a => a.Id == accountModel.Id).SingleOrDefault();
                    if (account.SeatSlot > accountModel.SeatSlot)
                    {
                        var excessSeats = ctx.Seats.Where(a => a.AccountId == accountModel.Id && a.SeatNumber > accountModel.SeatSlot).ToList();

                        ctx.Seats.RemoveRange(excessSeats);
                    }

                    account.SeatSlot = accountModel.SeatSlot;
                    account.SeatCode = accountModel.SeatCode;

                    ctx.SaveChanges();

                    var accountDetail = ctx.Accounts.Where(a => a.Id == accountModel.Id).FirstOrDefault();

                    var acctmgrs = GetAccountManagersFullDetails(accountModel.Id, string.Empty);
                    var systemAdmins = ctx.vw_ActiveUsers.Where(a => a.Role == Globals.SYSAD_RC).ToList();

                    var emailmsg = string.Format("Please be advised that {0} edited the seats data for {1}",
                        userAction.Name,
                        accountDetail.Name);

                    string emailBody = EmailBody.SeatStatusUpdate(emailmsg, siteurl);

                    Task.Run(() => NotifyAndMail(new IdentityMessage()
                    {
                        Subject = "Account Seat Details Update",
                        Body = emailBody
                    },
                    acctmgrs,
                    systemAdmins,
                    "Account Seat Details Update",
                    userAction.Id)).Wait();
                }

                transaction.Commit();
            }
        }

        public void DeleteAccount(int accountId)
        {
            using (var ctx = Entities.Create())
            {
                var accMgrs
                    = (from am in ctx.AccountMembers
                       join u in ctx.vw_ActiveUsers on am.UserDetailsId equals u.UserDetailsId
                       where am.AccountId == accountId && u.Role == Globals.ACCOUNT_MANAGER_RC
                       select am).ToList();

                var clientPOCs
                    = (from am in ctx.AccountMembers
                       join u in ctx.vw_ActiveUsers on am.UserDetailsId equals u.UserDetailsId
                       where am.AccountId == accountId && u.Role == Globals.CLIENT_RC
                       select am).ToList();

                foreach (var accMgr in accMgrs)
                {
                    accMgr.IsDeleted = true;
                }

                foreach (var cp in clientPOCs)
                {
                    cp.IsDeleted = true;
                }

                ctx.SaveChanges();

                var account = ctx.Accounts.Where(a => a.Id == accountId).SingleOrDefault();

                if (account == null)
                {
                    throw new NullReferenceException(Resources.AccountNotFound)
                    {
                        Source = ExceptionType.Thrown.ToString()
                    };
                }
                else
                {
                    account.IsDeleted = true;
                    account.IsActive = false;
                }

                ctx.SaveChanges();
            }
        }

        public void DeactivateAccount(int accountId)
        {
            using (var ctx = Entities.Create())
            {
                var account = ctx.Accounts.SingleOrDefault(t => t.Id == accountId);

                if (account != null)
                {
                    ctx.Teams.Where(a => a.AccountId == accountId).ToList()
                        .ForEach(t => t.IsActive = false);

                    account.IsActive = false;

                    ctx.SaveChanges();
                }
            }
        }

        public void ActivateAccount(int accountId)
        {
            using (var ctx = Entities.Create())
            {
                var account = ctx.Accounts.SingleOrDefault(t => t.Id == accountId);

                if (account != null)
                {
                    ctx.Teams.Where(a => a.AccountId == accountId).ToList()
                        .ForEach(t => t.IsActive = true);

                    account.IsActive = true;

                    ctx.SaveChanges();
                }
            }
        }

        public AccountDataModel GetAccount(int accountId)
        {
            using (var ctx = Entities.Create())
            {
                return ctx.Accounts.Where(a => a.Id == accountId)
                    .Select(e => new AccountDataModel
                    {
                        Id = e.Id,
                        Name = e.Name,
                        ContactPerson = e.ContactPerson,
                        EmailAddress = e.EmailAddress,
                        OfficeAddress = e.OfficeAddress,
                        Website = e.Website,
                        Created = e.Created,
                        SeatCode = e.SeatCode,
                        SeatSlot = e.SeatSlot,
                        AccountManagers
                            = (from am in ctx.AccountMembers
                               join u in ctx.vw_ActiveUsers on am.UserDetailsId equals u.UserDetailsId
                               where am.AccountId == accountId && u.Role == Globals.ACCOUNT_MANAGER_RC
                               select new UserModel
                               {
                                   UserDetailsId = u.UserDetailsId,
                                   NetUserId = u.NetUserId,
                                   FirstName = u.FirstName,
                                   LastName = u.LastName,
                                   RoleCode = u.Role
                               }).ToList(),
                        ClientPOCs
                            = (from am in ctx.AccountMembers
                               join u in ctx.vw_ActiveUsers on am.UserDetailsId equals u.UserDetailsId
                               where am.AccountId == accountId && u.Role == Globals.CLIENT_RC
                               select new UserModel
                               {
                                   UserDetailsId = u.UserDetailsId,
                                   NetUserId = u.NetUserId,
                                   FirstName = u.FirstName,
                                   LastName = u.LastName,
                                   RoleCode = u.Role
                               }).ToList()
                    }).SingleOrDefault();
            }
        }

        public int GetAccountsCount(string username)
        {
            var accounts = GetAssignedAccountsRaw(username, false)
               .Where(a => a.IsActive == true && a.IsDeleted != true)
               .Select(e => new
               {
                   e.Id
               });
            return accounts.Count();
        }

        public AccountDashboard GetAccountsDashboardData(string username, bool showInactive = false)
        {
            var userInfo = GetCurrentUserInfo(username);

            using (Entities ctx = Entities.Create())
            {
                IEnumerable<TeamRawModel> assignedTeams = GetAssignedTeamsRaw(username, false).ToList();

                IEnumerable<AccountModel> assignedAccounts = GetAssignedAccountsRaw(username,
                    false).ToList();
                IEnumerable<UsersRawModel> users = GetAssignedUsersRaw(username,
                    showInactive).ToList();

                if (!showInactive)
                {
                    assignedAccounts = assignedAccounts.Where(a => a.IsActive == true);
                    assignedTeams = assignedTeams.Where(t => t.IsActive == true);
                }

                IEnumerable<int> assignedTeamIds = assignedTeams.Select(t => t.Id).ToList();
                IEnumerable<int> assignedAccountIds = assignedAccounts.Select(t => t.Id).ToList();

                var agentsRoleCodes = new List<string> {
                    Globals.AGENT_RC, Globals.LEAD_AGENT_RC
                };

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

                var accounts = ctx.Accounts.Where(a => assignedAccountIds.Any(t => t == a.Id)
                    && a.IsDeleted != true).ToList();
                var teams = ctx.Teams.Where(t => assignedTeamIds.Any(d => d == t.Id)
                    && assignedAccountIds.Contains(t.AccountId) && t.IsDeleted != true).ToList();

                var accountMembers = (from am in ctx.AccountMembers
                                      join u in ctx.vw_TaskClocker on am.UserDetailsId equals u.UserDetailsId
                                      where am.IsDeleted != true
                                      select new
                                      {
                                          am.UserDetailsId,
                                          am.AccountId
                                      }).ToList();

                var filledSeatsCount = ctx.Seats.Where(a => a.Status == SeatStatuses.Filled.ToString()).Count();

                return new AccountDashboard
                {
                    TotalAccounts = assignedAccounts.Count(),
                    TotalTeams = assignedTeams.Count(),
                    TotalAgents = users.Count(),
                    TotalPaidSeats = filledSeatsCount,
                    AccountsList = accounts.Select(a => new AccountsListModel
                    {
                        Id = (int)a.Id,
                        Name = a.Name,
                        IsActive = a.IsActive,
                        SeatCode = a.SeatCode,
                        Seat = a.SeatSlot,
                        AgentCount = accountMembers.Count(am => am.AccountId == a.Id
                                                                && users.Any(u => u.Id == am.UserDetailsId)),
                        TeamCount = teams.Count(t => t.AccountId == a.Id)
                    }
                    ).ToList()
                };
            }
        }

        public bool IsNameDuplicate(string name, int accountId = 0)
        {
            using (var ctx = Entities.Create())
            {
                var query = ctx.Accounts.Where(a => a.Name == name);

                if (accountId > 0)
                {
                    query = query.Where(a => a.Id != accountId);
                }

                return query.Any();
            }
        }

        public IList<TeamUsersModel> CompileList(IList<TeamUsersModel> list)
        {
            var compiledList = new List<TeamUsersModel>();

            foreach (var item in list)
            {
                var existing = compiledList.SingleOrDefault(s => item.UserDetailsId == s.UserDetailsId);

                if (existing != null)
                {
                    if (item.TeamId > 0 && !existing.Teams.Any(t => t.Id == item.TeamId))
                    {
                        existing.Teams.Add(new BaseModel
                        {
                            Id = item.TeamId,
                            Name = item.TeamName ?? ""
                        });
                    }
                }
                else
                {
                    if (item.TeamId > 0)
                    {
                        item.Teams.Add(new BaseModel
                        {
                            Id = item.TeamId,
                            Name = item.TeamName
                        });
                    }

                    compiledList.Add(item);
                }
            }

            return compiledList;
        }

        public async Task NotifyAndMail(IdentityMessage message,
            List<vw_ActiveUsers> managers,
            List<vw_ActiveUsers> sysAds,
            string action,
            int userDetailsId)
        {


            using (var ctx = Entities.Create())
            {
                foreach (var mgr in managers)
                {
                    ctx.Notifications.Add(new Notification
                    {
                        ToUserId = mgr.UserDetailsId,
                        NoteDate = DateTimeUtility.Instance.DateTimeNow(),
                        Message = message.Body,
                        Title = message.Subject,
                        Icon = Resources.ReminderIcon,
                        NoteType = NoteType.SeatStatus.ToString()
                    });
                }

                foreach (var sa in sysAds)
                {
                    ctx.Notifications.Add(new Notification
                    {
                        ToUserId = sa.UserDetailsId,
                        NoteDate = DateTimeUtility.Instance.DateTimeNow(),
                        Message = message.Body,
                        Title = message.Subject,
                        Icon = Resources.ReminderIcon,
                        NoteType = NoteType.SeatStatus.ToString()
                    });
                }
            }


            List<EmailAddress> recipients = new List<EmailAddress>();
            var accEmailRecipient = managers
                        .Select(r => new EmailAddress()
                        {
                            Name = r.FullName,
                            Email = r.Email
                        }).ToList();

            var systemAdminsEmailRecipient = sysAds.Where(a => a.Role == Globals.SYSAD_RC)
                .Select(a => new EmailAddress()
                {
                    Name = a.FullName,
                    Email = a.Email
                }).ToList();

            recipients.AddRange(accEmailRecipient);
            recipients.AddRange(systemAdminsEmailRecipient);

            await SendSeatStatusUpdateEmail(message, recipients, action, userDetailsId).ConfigureAwait(false);
        }

        public async Task SendSeatStatusUpdateEmail(IdentityMessage message,
            List<EmailAddress> recipients,
            string action,
            int userDetailsId)
        {
            var refCode = ServiceUtility.GetRandomCode();
            message.Subject = $"Ref: {refCode} | {message.Subject}";
            message.Destination = string.Join(", ", recipients.Select(r => r.Email).ToArray());

            var systemLog = new SystemLog
            {
                LogDate = DateTime.UtcNow,
                ActorUserId = userDetailsId,
                ActionType = action,
                Entity = "Seat",
                RequestBody = (new JavaScriptSerializer()).Serialize(message),
                EODEmailReference = refCode
            };

            await SendGridMailServices.Instance.SendMultipleAsync(message, recipients)
                .ConfigureAwait(false);

            SaveToDb(systemLog);
        }

        public async Task SendSeatStatusUpdateEmailUser(IdentityMessage message,
            string recipient)
        {

            var emailSubject = message.Subject;
            var emailBody = message.Body;

            var emailRecipient = recipient;

            await SendGridMailServices.Instance.SendAsync(new IdentityMessage
            {
                Subject = emailSubject,
                Body = emailBody,
                Destination = emailRecipient
            }).ConfigureAwait(false);

        }

        public async Task AddMemberEmail(IdentityMessage message,
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

        public async Task AddMemberEmail2(IdentityMessage message,
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

        public async Task SendRemoveMemberEmailUser(IdentityMessage message,
            string recipient,
             List<EmailAddress> recipients)
        {

            var emailSubject = message.Subject;
            var emailBody = message.Body;

            var emailRecipient = recipient;

            /*await SendGridMailServices.Instance.SendAsync(new IdentityMessage
            {
                Subject = emailSubject,
                Body = emailBody,
                Destination = emailRecipient
            }).ConfigureAwait(false);*/

            /*var emailmsgUser = string.Format("Please be advised that {0} was removed from {1}",
                    userdetail.Name,
                    acc.Name);

            string emailBodyUser = EmailBody.RemoveMember(emailmsgUser);*/

            message.Destination = string.Join(", ", recipients.Select(r => r.Email).ToArray());

            await SendGridMailServices.Instance.SendMultipleAsync(message, recipients)
                .ConfigureAwait(false);

        }

        public async Task SendRemoveMemberEmailUser2(IdentityMessage message,
            string recipient,
             List<EmailAddress> recipients)
        {

            var emailSubject = message.Subject;
            var emailBody = message.Body;

            var emailRecipient = recipient;

            await SendGridMailServices.Instance.SendAsync(new IdentityMessage
            {
                Subject = emailSubject,
                Body = emailBody,
                Destination = emailRecipient
            }).ConfigureAwait(false);

            var recp = recipients;

            /*var emailmsgUser = string.Format("Please be advised that {0} was removed from {1}",
                    userdetail.Name,
                    acc.Name);

            string emailBodyUser = EmailBody.RemoveMember(emailmsgUser);*/

            /*message.Destination = string.Join(", ", recipients.Select(r => r.Email).ToArray());

            await SendGridMailServices.Instance.SendMultipleAsync(message, recipients)
                .ConfigureAwait(false);*/

        }
    }
}
