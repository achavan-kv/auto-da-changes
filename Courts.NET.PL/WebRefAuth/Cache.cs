using System;
namespace STL.PL.WCache
{
    public partial class Cache
    {
        public Cache(bool custom)
        {
            this.Url = STL.Common.Static.Config.Url + this.GetType().Name + ".asmx";
        }
    }
}
