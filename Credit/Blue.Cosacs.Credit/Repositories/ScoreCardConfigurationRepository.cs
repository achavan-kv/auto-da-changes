using Blue.Cosacs.Credit.Extensions;
using Blue.Cosacs.Credit.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Blue.Cosacs.Credit.Repositories
{
    public class ScoreCardConfigurationRepository : Blue.Cosacs.Credit.Repositories.IScoreCardConfigurationRepository
    {
        public IEnumerable<ScoringConfiguration> GetSoreCardConfig()
        {
            using (var scope = Context.Read())
            {
                return scope.Context.ScoreCardConfiguration.ToList().Select(s => new Model.ScoringConfiguration(s)).ToList();
            }
        }

        public void Save(Model.ScoreCard scoringCard, int user, IClock clock)
        {
            using (var scope = Context.Write())
            {
                scope.Context.ScoreCard.Add(new ScoreCard()
                {
                    CardType = scoringCard.Name,
                    Card = scoringCard.SerializeToXml(),
                    CreatedBy = user,
                    CreatedOn = clock.Now
                });
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public IEnumerable<Model.ScoreCard> GetRules()
        {
            var scoreCards = new List<Model.ScoreCard>();
            var serializer = new XmlSerializer(typeof(Model.ScoreCard));
            using (var scope = Context.Read())
            {
                var ids = (from sc in scope.Context.ScoreCard
                             group sc by sc.CardType into g
                             select g.Max(e => e.Id)).ToList();

                var cards = (from sc in scope.Context.ScoreCard
                             where ids.Contains(sc.Id)
                             select sc.Card).ToList();
                cards.ForEach(c =>
                {
                    using (TextReader reader = new StringReader(c))
                    {
                        scoreCards.Add((Model.ScoreCard)serializer.Deserialize(reader));
                    }
                });
            }
            return scoreCards;
        }
    }
}
