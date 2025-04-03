using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Blue.Cosacs.Shared;
using Blue.Cosacs.Shared.Services.CosacsConfig;

namespace Blue.Cosacs.Repositories
{
    public class ConfigRepository
    {
        public string CountryCode()
        {
            using (var ctx = Context.Create())
                return (from c in ctx.Country
                        select c.countrycode).First().Trim();
        }

        public List<CountryMaintenance> CultureCode()
        {
            using (var ctx = Context.Create())
                return ctx.CountryMaintenance
                       .Where(c => c.CodeName == "Culture" || c.CodeName == "currencysymbolforprint").AnsiToList(ctx);
        }

        public CheckConnResponse GetStartUpInfo(CheckConnRequest request)
        {
            var datemismatch = false;
            var serverVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            if (request.CurrentDate.Subtract(DateTime.Now).Duration().Minutes > 5)
                datemismatch = true;

            if (request.Version != serverVersion)
                return new CheckConnResponse
                {
                    IsCorrectServer = false,
                    WrongServerVersion = serverVersion,
                    DatetimeMismatch = datemismatch
                };

            return Context.ExecuteTx((ctx, connection, transaction) =>
            {
                var country = ctx.Country.FirstOrDefault();

                var branch = ctx.Branch
                                .Where(b => b.branchno == Convert.ToInt16(request.BranchNo)).AnsiFirstOrDefault(ctx);
                //var courtsperson = ctx.CourtsPerson
                //                   .Where(c => c.empeeno == Convert.ToInt32(request.EmpeeNo)).AnsiFirstOrDefault(ctx);
                var user = new Blue.Admin.UserRepository(EventStore.Instance).LoadByLogin(request.Login);

                var accountlocking = ctx.AccountLocking
                                     .Where(a => a.lockedby == user.Id).AnsiToList(ctx);
                var customerlocking = ctx.CustomerLocking           // #10135 jec 
                                     .Where(c => c.LockedBy == user.Id).AnsiToList(ctx);

                if (branch != null)
                {
                    foreach (var rows in accountlocking)
                        ctx.AccountLocking.DeleteOnSubmit(rows);
                    // #10135 jec 
                    foreach (var rows in customerlocking)
                        ctx.CustomerLocking.DeleteOnSubmit(rows);
                }

                ctx.SubmitChanges();

                var validateResult = new Blue.Admin.UserPasswordValidation(
                    new Blue.Admin.UserRepository(EventStore.Instance),
                    new Blue.DateTimeClock()).Validate(request.Login, request.Password);
                //new STL.Common.WebServiceAuthenticationEvent(HttpContext.Current, request.Login, user.Id, request.Password, null, request.c
                if (validateResult.IsValid)
                {
                    EventStore.Instance.Log(new
                    {
                        Login = request.Login,
                        BranchNo = request.BranchNo,
                        MachineName = request.MachineName
                    },
                    "ClientLogIn",
                    EventCategory.Security,
                    new
                    {
                        Login = request.Login,
                        Machine = request.MachineName
                    });
                }

                return new CheckConnResponse
                {
                    Country = country.countrycode.Trim(),    //IP - 18/04/12 - #9494
                    Storetype = branch != null ? branch.StoreType : String.Empty,
                    FullName = user.FullName, // courtsperson.empeename,
                    ShouldChangePassword = validateResult.UserMustChangePassword,
                    IsValidBranch = branch != null,
                    UserId = user.Id,
                    Permissions = validateResult.User.PermissionIds,
                    IsLocked = validateResult.IsLocked
                };
            });
        }

        public bool CheckBranch(string branchNo)
        {
            return Context.ExecuteTx((ctx, connection, transaction) =>
            {
                return ctx.Branch.Any(b => b.branchno == Convert.ToInt16(branchNo));
            });
        }

        public void LogOff(string userName, string machineName, int userId)
        {
            using (var ctx = Context.Create())
            {
                // #10135 jec - remove all locks when user logs off
                var accountlocking = ctx.AccountLocking
                                     .Where(a => a.lockedby == STL.Common.Static.Credential.UserId).AnsiToList(ctx);
                var customerlocking = ctx.CustomerLocking
                                     .Where(c => c.LockedBy == STL.Common.Static.Credential.UserId).AnsiToList(ctx);

                foreach (var rows in accountlocking)
                    ctx.AccountLocking.DeleteOnSubmit(rows);

                foreach (var rows in customerlocking)
                    ctx.CustomerLocking.DeleteOnSubmit(rows);

                ctx.SubmitChanges();
            };

            EventStore.Instance.Log(new { UserId = userId, MachineName = machineName, UserName = userName }, "ClientLogOff", EventCategory.Security, new { UserId = userId, MachineName = machineName, UserName = userName });
        }

        public string GetSystemDrive()
        {
            using (var ctx = Context.Create())
            {
                return ctx.CountryMaintenance
                    .Where(e => e.CodeName == "systemdrive")
                    .FirstOrDefault()
                    .Value;
            }
        }

    }
}

