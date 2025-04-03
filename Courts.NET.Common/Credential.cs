namespace STL.Common.Static
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Web;
    using System.Threading;

    public struct Credential
    {
        private static int win_userId;
        private static string win_name;
        private static string win_pass;

        [ThreadStatic] private static int web_userId;
        [ThreadStatic] private static string web_name;
        [ThreadStatic] private static string web_pass;

        public static int UserId 
        { 
            get 
            {
                return (HttpContext.Current == null ? win_userId : web_userId);
            }
            set 
            {
                if (HttpContext.Current == null)
                    win_userId = value;
                else
                    web_userId = value;
            }
        }

        public static string Name
        {
            get
            {
                return (HttpContext.Current == null ? win_name : web_name);
            }
            set
            {
                if (HttpContext.Current == null)
                    win_name = value;
                else
                    web_name = value;
            }
        }

        public static string Password
        {
            get
            {
                return (HttpContext.Current == null ? win_pass : web_pass);
            }
            set
            {
                if (HttpContext.Current == null)
                    win_pass = value;
                else
                    web_pass = value;
            }
        }
       
        /// <summary>
        /// DO NOT USE ON THE WEB/SERVER
        /// </summary>
        public static string User { get; set; }
        /// <summary>
        /// DO NOT USE ON THE WEB/SERVER
        /// </summary>
       // public static string Password { get; set; }
        /// <summary>
        /// DO NOT USE ON THE WEB/SERVER
        /// </summary>
        public static string Cookie { get; set; }
        /// <summary>
        /// DO NOT USE ON THE WEB/SERVER
        /// </summary>
        public static string[] Roles { get; set; }
        /// <summary>
        /// DO NOT USE ON THE WEB/SERVER
        /// </summary>
        private static List<int> Permissions { get; set; }

        /// <summary>
        /// Warning: do not use this if yu do not know what you are doing!
        /// </summary>
        public static void SetPermissions(int[] ids)
        {
            Permissions = new List<int>(ids);
            Permissions.Sort();
        }

        /// <summary>
        /// DO NOT USE ON THE WEB/SERVER
        /// </summary>
        public static bool IsInRole(string role)
        {
            bool found = false;
            if (role == "N")			//if administrator
                found = true;
            else
            {
                foreach (string r in Roles)
                {
                    if (r == role)
                    {
                        found = true;
                        break;
                    }
                }
            }
            return found;
        }

        /// <summary>
        /// DO NOT USE ON THE WEB/SERVER
        /// </summary>
        public static bool HasPermission(System.Enum value)
        {
            return Permissions.BinarySearch((int)((object)value)) >= 0;
        }
    }
}