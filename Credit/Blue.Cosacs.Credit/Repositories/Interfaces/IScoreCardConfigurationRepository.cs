namespace Blue.Cosacs.Credit.Repositories
{
    public interface IScoreCardConfigurationRepository
    {
        System.Collections.Generic.IEnumerable<Blue.Cosacs.Credit.Model.ScoreCard> GetRules();
        System.Collections.Generic.IEnumerable<Blue.Cosacs.Credit.Model.ScoringConfiguration> GetSoreCardConfig();
        void Save(Blue.Cosacs.Credit.Model.ScoreCard scoringCard, int user, Blue.IClock clock);
    }
}
