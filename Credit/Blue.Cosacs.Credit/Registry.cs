using Blue.Cosacs.Credit.Repositories;
using Blue.Cosacs.Credit.Repositories.Interfaces;
using Blue.Cosacs.Credit.Repositories.Reindex;
using Blue.Networking;
namespace Blue.Cosacs.Credit
{
    public class Registry : StructureMap.Configuration.DSL.Registry
    {
        public Registry()
        {
            For<IClock>().Singleton().Add<DateTimeClock>();
            For<ISanctionStage1Repository>().Add<SanctionStage1Repository>();
            var settings = new Blue.Cosacs.Credit.Settings();
            Blue.Config.Settings.Register(this, settings);
            For<Blue.Config.ISettingsReader>().Singleton().Use(settings);
            For<Blue.Cosacs.Credit.Repositories.IProposalRepository>().Add<Blue.Cosacs.Credit.Repositories.ProposalRepository>();
            For<ISanctionStage2Repository>().Add<SanctionStage2Repository>();
            For<ICustomizeFieldsRepository>().Add<CustomizeFieldsRepository>();
            For<ICustomerRepository>().Add<CustomerRepository>();
            For<IDocumentConfirmationRepository>().Add<DocumentConfirmationRepository>();
            For<IAccountRepository>().Add<AccountRepository>();
            For<IScoringBandMatrixRepository>().Add<ScoringBandMatrixRepository>();
            For<IScoreCardConfigurationRepository>().Add<ScoreCardConfigurationRepository>();
            For<ITermsTypeRepository>().Add<TermsTypeRepository>();
            For<IHttpClientJson>().Use<HttpClientJsonRelative>();
            For<IHttpClient>().Use<HttpClient>();
            For<ITermsTypeSolrIndex>().Add<TermsTypeSolrIndex>();
            For<ICustomerSolrIndex>().Add<CustomerSolrIndex>();
        }
    }
}
