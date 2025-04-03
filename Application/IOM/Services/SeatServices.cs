using IOM.DbContext;
using IOM.Helpers;
using IOM.Models;
using IOM.Models.ApiControllerModels;
using IOM.Properties;
using IOM.Utilities;
using Microsoft.AspNet.Identity;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using IOM.Services.Interface;
using System.Security.Principal;
using System.Threading;

namespace IOM.Services
{
    public partial class RepositoryService : IRepositoryService
    {
        public List<AccountSeatModel> GetAvailableAccountSeats(int accountid)
        {
            List<AccountSeatModel> res = new List<AccountSeatModel>();

            using (var ctx = Entities.Create())
            {
                var acc = ctx.Accounts.Where(a => a.Id == accountid && a.IsActive == true).FirstOrDefault();

                if (acc != null)
                {
                    int maxSeat = acc.SeatSlot.HasValue ? acc.SeatSlot.Value : 0;

                    if (maxSeat == 0)
                    {
                        return res;
                    }

                    // get occupied seats
                    var occupiedSeats = ctx.Seats.Where(a => a.AccountId == accountid && a.UserId > 0).Select(a => a.SeatNumber).ToList();

                    for (int i = 1; i <= maxSeat; i++)
                    {
                        if (occupiedSeats.IndexOf(i) == -1)
                        {
                            res.Add(new AccountSeatModel()
                            {
                                SeatCode = SeatNameGenerator(i, acc.SeatCode),
                                SeatSlot = i
                            });
                        }
                    }
                }
            }

            return res;
        }

        public List<SeatDashboardViewModel> GetDashboardSeats(int[] accountIds)
        {
            List<SeatDashboardViewModel> res = new List<SeatDashboardViewModel>();

            using (var ctx = Entities.Create())
            {
                var accountList = new List<Account>();
                var seatList = ctx.Seats.AsEnumerable();
                var userList = ctx.UserDetails.AsEnumerable();

                if (accountIds.Length > 0)
                {
                    accountList = ctx.Accounts.Where(a => a.IsActive == true && a.SeatSlot > 0 && accountIds.Contains(a.Id)).ToList();
                }

                foreach (var acc in accountList)
                {
                    var enumerable = seatList.ToList();
                    var userDetails = userList.ToList();
                    var accSeatSlot = acc.SeatSlot ?? 0;

                    for (var i = 1; i <= accSeatSlot; i++)
                    {
                        if (acc.SeatCode == null)
                        {
                            continue;
                        }

                        var item = new SeatDashboardViewModel
                        {
                            AccountId = acc.Id,
                            AccountName = acc.Name,
                            AccountSeatCode = acc.SeatCode
                        };

                        var userSeat = enumerable.FirstOrDefault(a => a.SeatNumber == i && a.AccountId == acc.Id);

                        item.SeatNumber = i;
                        item.SeatId = SeatNameGenerator(i, acc.SeatCode);

                        if (userSeat == null)
                        {
                            item.SeatStatus = SeatStatuses.Vacant.ToString();
                        }
                        else
                        {
                            item.SeatStatus = userSeat.Status ?? SeatStatuses.Vacant.ToString();
                        }

                        if (userSeat != null)
                        {
                            item.SeatStatus = userSeat.Status;

                            var userdata = userDetails.FirstOrDefault(a => a.Id == userSeat.UserId);

                            if (userdata != null)
                            {
                                item.UserId = userdata.Id;
                                item.StaffPositioon = userdata.Role;
                                item.StaffName = userdata.Name;
                                item.StaffId = userdata.StaffId;
                            }
                        }

                        res.Add(item);
                    }
                }
            }

            return res;
        }

        public async Task<ApiResult> OccupySeat(int accountId, int userid, int seatNumber, string occupyType, string siteUrl,
            string userInAction, CancellationToken cancellationToken)
        {
            SeatStatuses nmstr;

            if (!Enum.TryParse(occupyType, out nmstr))
            {
                return ApiResult.BadRequest("Invalid seat occupation type");
            }

            using (var ctx = Entities.Create())
            {
                var userSeat = ctx.Seats.FirstOrDefault(a => a.AccountId == accountId && a.UserId == userid);
                if (userSeat != null)
                {
                    return ApiResult.BadRequest("User already assigned to a seat.");
                }

                var acc = ctx.Accounts.FirstOrDefault(a => a.Id == accountId);
                var userActionId = ctx.UserDetails.Where(a => a.UserId == userInAction).Select(a => a.Id).FirstOrDefault();

                if (acc != null)
                {
                    var maxSeat = acc.SeatSlot ?? 0;

                    if (maxSeat == 0)
                    {
                        return ApiResult.BadRequest("No available seat(s).");
                    }

                    if (seatNumber > maxSeat)
                    {
                        return ApiResult.BadRequest("Seat is not available");
                    }

                    var seat = ctx.Seats.FirstOrDefault(a => a.AccountId == accountId && a.SeatNumber == seatNumber);

                    if (seat == null)
                    {
                        ctx.Seats.Add(new Seat()
                        {
                            AccountId = accountId,
                            UserId = userid,
                            SeatNumber = seatNumber,
                            Status = occupyType
                        });

                        await ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    }
                    else
                    {
                        if (occupyType == SeatStatuses.Vacant.ToString() && userid > 0)
                        {
                            VacanSeat(accountId, userid, seatNumber, siteUrl, userInAction, out var msg1);

                            if (!string.IsNullOrEmpty(msg1))
                            {
                                return ApiResult.BadRequest(msg1);
                            }
                        }
                        else
                        {
                            seat.Status = occupyType;
                            seat.UserId = userid;
                            await ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                        }
                    }

                    var userDetail = ctx.UserDetails.FirstOrDefault(a => a.Id == userid);

                    var acctMgrs = GetAccountManagersFullDetails(accountId, string.Empty);
                    var systemAdmins = ctx.vw_ActiveUsers.Where(a => a.Role == Globals.SYSAD_RC).ToList();

                    var emailmsg = string.Format("Please be advised that {0} was assigned to {1}'s {2} - {3}",
                        userDetail.Name,
                        acc.Name,
                        acc.SeatCode,
                        seatNumber);

                    var emailBody = EmailBody.SeatStatusUpdate(emailmsg, siteUrl);

                    Task.Run(() => SeatNotifyAndMail(new IdentityMessage()
                        {
                            Subject = "Occupy Seat",
                            Body = emailBody
                        },
                        acctMgrs,
                        systemAdmins,
                        "Occupy seat",
                        userActionId), cancellationToken).Wait(cancellationToken);

                    var userInfo = GetUserDetails(userid);

                    //  start send email to user

                    var emailmsgUser = string.Format("Please be advised that you were assigned to {1}'s {2} - {3}",
                        userDetail.Name,
                        acc.Name,
                        acc.SeatCode,
                        seatNumber);

                    var emailBodyUser = EmailBody.SeatStatusUpdate(emailmsgUser, siteUrl);

                    Task.Run(() => SeatUserEmail(new IdentityMessage()
                        {
                            Subject = "Assigned Seat",
                            Body = emailBodyUser
                        },
                        userInfo.Email,
                        acctMgrs,
                        systemAdmins), cancellationToken).Wait(cancellationToken);

                    // end send user email
                    ctx.Notifications.AddRange(acctMgrs.Select(a => new Notification()
                    {
                        ToUserId = a.UserDetailsId,
                        NoteDate = DateTime.UtcNow,
                        Icon = "fa-bell",
                        Title = "Occupy Seat",
                        NoteType = NoteType.SeatStatus.ToString(),
                        Message = emailmsg
                    }));

                    ctx.Notifications.AddRange(systemAdmins.Select(a => new Notification()
                    {
                        ToUserId = a.UserDetailsId,
                        NoteDate = DateTime.UtcNow,
                        Icon = "fa-bell",
                        Title = "Occupy Seat",
                        NoteType = NoteType.SeatStatus.ToString(),
                        Message = emailmsg
                    }));

                    await ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    return ApiResult.BadRequest("Account is not available.");
                }
            }

            return new ApiResult();
        }

        public bool UpdateStatus(int accountid, int userid, int seatnumber, string occupytype, string siteUrl, string userInAction, out string msg)
        {
            msg = string.Empty;

            SeatStatuses nmstr;
            if (!Enum.TryParse(occupytype, out nmstr))
            {
                msg = "Invalid seat occupation type";
                return false;
            }

            if (userid == 0)
            {
                msg = "Seat is not assigned to a member yet.";
                return false;
            }

            using (var ctx = Entities.Create())
            {
                try
                {
                    var acc = ctx.Accounts.Where(a => a.Id == accountid).FirstOrDefault();
                    var userActionId = ctx.UserDetails.Where(a => a.UserId == userInAction).Select(a => a.Id).FirstOrDefault();

                    if (acc != null)
                    {
                        int maxSeat = acc.SeatSlot.HasValue ? acc.SeatSlot.Value : 0;

                        if (maxSeat == 0)
                        {
                            msg = "No available seat(s).";
                            return false;
                        }

                        if (seatnumber > maxSeat)
                        {
                            msg = "Seat is not available";
                            return false;
                        }

                        var seat = ctx.Seats.Where(a => a.AccountId == accountid && a.SeatNumber == seatnumber).FirstOrDefault();

                        if (seat == null)
                        {
                            ctx.Seats.Add(new Seat()
                            {
                                AccountId = accountid,
                                UserId = userid,
                                SeatNumber = seatnumber,
                                Status = occupytype
                            });

                            ctx.SaveChanges();
                        }
                        else
                        {
                            if (occupytype == SeatStatuses.Vacant.ToString() && userid > 0)
                            {
                                var msg1 = string.Empty;

                                VacanSeat(accountid, userid, seatnumber, siteUrl, userInAction, out msg1);

                                if (msg1 != string.Empty)
                                {
                                    msg = msg1;
                                }
                            }
                            else
                            {
                                seat.Status = occupytype;
                                seat.UserId = userid;
                                ctx.SaveChanges();
                            }
                        }

                        var userdetail = ctx.UserDetails.Where(a => a.Id == userid).FirstOrDefault();

                        var acctmgrs = GetAccountManagersFullDetails(accountid, string.Empty);
                        var systemAdmins = ctx.vw_ActiveUsers.Where(a => a.Role == Globals.SYSAD_RC).ToList();

                        var emailmsg = string.Format("Please be advised that {0} was assigned to {1}'s {2} - {3}",
                            userdetail.Name,
                            acc.Name,
                            acc.SeatCode,
                            seatnumber);

                        string emailBody = EmailBody.SeatStatusUpdate(emailmsg, siteUrl);

                        Task.Run(() => SeatNotifyAndMail(new IdentityMessage()
                            {
                                Subject = "Updated Seat",
                                Body = emailBody
                            },
                            acctmgrs,
                            systemAdmins,
                            "Updated seat",
                            userActionId)).Wait();

                        ctx.Notifications.AddRange(acctmgrs.Select(a => new Notification()
                        {
                            ToUserId = a.UserDetailsId,
                            NoteDate = DateTime.UtcNow,
                            Icon = "fa-bell",
                            Title = "Updated Seat",
                            NoteType = NoteType.SeatStatus.ToString(),
                            Message = emailmsg
                        }));

                        ctx.Notifications.AddRange(systemAdmins.Select(a => new Notification()
                        {
                            ToUserId = a.UserDetailsId,
                            NoteDate = DateTime.UtcNow,
                            Icon = "fa-bell",
                            Title = "Updated Seat",
                            NoteType = NoteType.SeatStatus.ToString(),
                            Message = emailmsg
                        }));

                        ctx.SaveChanges();
                    }
                    else
                    {
                        msg = "Account is not available.";
                        return false;
                    }
                }
                catch (Exception e)
                {
                    msg = e.Message;
                    return false;
                }
            }

            return true;
        }

        public bool VacanSeat(int accountid, int userid, int seatnumber, string siteUrl, string userInaction, out string msg)
        {
            msg = string.Empty;

            using (var ctx = Entities.Create())
            {
                try
                {
                    var userActionId = ctx.UserDetails.Where(a => a.UserId == userInaction).Select(a => a.Id).FirstOrDefault();
                    var seat = ctx.Seats.Where(a => a.AccountId == accountid && a.SeatNumber == seatnumber && a.UserId == userid).FirstOrDefault();

                    if (seat != null)
                    {
                        ctx.Seats.Remove(seat);
                        ctx.SaveChanges();
                    }

                    var userdetail = ctx.UserDetails.Where(a => a.Id == userid).FirstOrDefault();
                    var accountDetail = ctx.Accounts.Where(a => a.Id == accountid).FirstOrDefault();

                    var acctmgrs = GetAccountManagersFullDetails(accountid, string.Empty);
                    var systemAdmins = ctx.vw_ActiveUsers.Where(a => a.Role == Globals.SYSAD_RC).ToList();

                    var emailmsg = string.Format("Please be advised that {0} was removed from {1}'s {2} - {3}",
                        userdetail.Name,
                        accountDetail.Name,
                        accountDetail.SeatCode,
                        seatnumber);

                    string emailBody = EmailBody.SeatStatusUpdate(emailmsg, siteUrl);

                    Task.Run(() => SeatNotifyAndMail(new IdentityMessage()
                        {
                            Subject = "Vacant Seat",
                            Body = emailBody
                        },
                        acctmgrs,
                        systemAdmins,
                        "Vacant seat",
                        userActionId)).Wait();

                    var userInfo = GetUserDetails(userid);

                    //  start send email to user

                    var emailmsgUser = string.Format("Please be advised that you were removed from {1}'s {2} - {3}",
                        userdetail.Name,
                        accountDetail.Name,
                        accountDetail.SeatCode,
                        seatnumber);

                    string emailBodyUser = EmailBody.SeatStatusUpdate(emailmsgUser, siteUrl);

                    Task.Run(() => SeatUserEmail(new IdentityMessage()
                        {
                            Subject = "Vacant Seat",
                            Body = emailBodyUser
                        },
                        userInfo.Email,
                        acctmgrs,
                        systemAdmins)).Wait();

                    // end send user email

                    ctx.Notifications.AddRange(acctmgrs.Select(a => new Notification()
                    {
                        ToUserId = a.UserDetailsId,
                        NoteDate = DateTime.UtcNow,
                        Icon = "fa-bell",
                        Title = "Vacant Seat",
                        NoteType = NoteType.SeatStatus.ToString(),
                        Message = emailmsg
                    }));

                    ctx.Notifications.AddRange(systemAdmins.Select(a => new Notification()
                    {
                        ToUserId = a.UserDetailsId,
                        NoteDate = DateTime.UtcNow,
                        Icon = "fa-bell",
                        Title = "Vacant Seat",
                        NoteType = NoteType.SeatStatus.ToString(),
                        Message = emailmsg
                    }));

                    ctx.SaveChanges();
                }
                catch (Exception e)
                {
                    msg = e.Message;
                    return false;
                }
            }

            return true;
        }

        public async Task SeatNotifyAndMail(IdentityMessage message,
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

        public async Task SeatUserEmail(IdentityMessage message,
            string userEmail,
            List<vw_ActiveUsers> managers,
            List<vw_ActiveUsers> sysAds)
        {
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

            var recipient = userEmail;

            recipients.AddRange(accEmailRecipient);
            recipients.AddRange(systemAdminsEmailRecipient);

            await SendSeatStatusUpdateEmailUser(message, recipient).ConfigureAwait(false);
        }

        public string SeatNameGenerator(int seat, string code)
        {
            if (code.Length == 0)
            {
                code = string.Empty;
            }

            int digits = seat.ToString().Length;
            int maxDigits = 3;
            int prefixCount = maxDigits - digits;

            var prefx = string.Empty;

            for (int i = 0; i < prefixCount; i++)
            {
                prefx += "0";
            }

            return code + " Seat - " + prefx + seat.ToString();
        }
    }
}