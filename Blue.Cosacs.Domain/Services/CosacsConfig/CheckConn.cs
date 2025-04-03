using System;

namespace Blue.Cosacs.Shared.Services.CosacsConfig
{
    partial class CheckConnRequest
    {
        public string Version;
        public string BranchNo;
        //public string EmpeeNo;
        public DateTime CurrentDate;
        public string MachineName;
        public string Login;
        public string Password;
    }

    partial class CheckConnResponse
    {
        public bool IsCorrectServer;
        public bool DatetimeMismatch;
        public bool IsValidBranch;
        public bool ShouldChangePassword;
        public string WrongServerVersion;
        public string Country;
        public string Storetype;
        public string FullName;
        public int UserId;
        public int[] Permissions;
        public bool IsLocked { get; set; }
    }
}
