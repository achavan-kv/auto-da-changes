using STL.PL.WebRefAuth;
namespace STL.PL.WS5
{
    partial class Authentication : IAuthentication { }

    public partial class WStaticDataManager
    {
        public WStaticDataManager(bool custom)
        {
            Setup();
        }

        public void Setup()
        {
            this.AuthenticationValue = this.Setup<Authentication>(timeout: 200000);
        }

        protected new object[] Invoke(string methodName, object[] parameters)
        {
            return new CheckCache().Get(methodName, parameters, () =>
            {
                var result = base.Invoke(methodName, parameters);
                this.PostInvoke(AuthenticationValue);
                return result;
            });
        }

        protected new void InvokeAsync(string methodName, object[] parameters, System.Threading.SendOrPostCallback callback)
        {
            base.InvokeAsync(methodName, parameters, (state) => { this.PostInvoke(AuthenticationValue); if (callback != null) callback(state); });
        }

        protected new void InvokeAsync(string methodName, object[] parameters, System.Threading.SendOrPostCallback callback, object userState)
        {
            base.InvokeAsync(methodName, parameters, (state) => { this.PostInvoke(AuthenticationValue); if (callback != null) callback(state); }, userState);
        }
    }
}
