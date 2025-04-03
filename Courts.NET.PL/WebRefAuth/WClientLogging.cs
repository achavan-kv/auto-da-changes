namespace STL.PL.WS4
{
    partial class Authentication : IAuthentication { }

    public partial class WClientLogging
    {
        public WClientLogging(bool custom)
        {
            this.AuthenticationValue = this.Setup<Authentication>(timeout: 200000);
        }
    }
}
