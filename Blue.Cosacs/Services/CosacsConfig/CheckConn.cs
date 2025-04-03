using Blue.Cosacs.Repositories;
using Blue.Cosacs.Shared.Services.CosacsConfig;

namespace Blue.Cosacs.Services.CosacsConfig
{
	partial class Server 
    {
        public CheckConnResponse Call(CheckConnRequest request)
        {
            return new ConfigRepository().GetStartUpInfo(request);
        }
    }
}
