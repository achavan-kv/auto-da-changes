namespace Blue.Cosacs.Merchandising.Repositories 
{
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using Blue.Cosacs.Merchandising.Models;

    public interface ITicketingRepository
    {
        IEnumerable<TicketingModel> Get();

        List<TicketingViewModel> GetTickets();
    }

    public class TicketingRepository : ITicketingRepository
    {
        private readonly IPromotionRepository promo;
        private readonly Settings merchandiseSettings;
        private readonly Config.Settings configSettings;

        public TicketingRepository(IPromotionRepository promo, Settings merchandiseSettings, Config.Settings configSettings)
        {
            this.promo = promo;
            this.merchandiseSettings = merchandiseSettings;
            this.configSettings = configSettings;
        }

        public IEnumerable<TicketingModel> Get()
        {
            using (var scope = Context.Read())
            {
                var result = scope.Context.TicketExtractView;
                var tickets = Mapper.Map<IList<TicketingModel>>(result).ToList();

                var components = scope.Context.ComponentView.ToList();
                tickets.ForEach(t => t.Components = components.Where(c => c.ParentId == t.ProductId).ToList());

                return tickets;
            }
        }

        private decimal ApplyTax(decimal price, decimal taxRate)
        {
            return merchandiseSettings.TaxInclusive ? price * (1 + taxRate) : price;
        }

        private string RoundedPriceString(decimal price)
        {
            return decimal.Round(price, configSettings.DecimalPlaces).ToString("F" + configSettings.DecimalPlaces);
        }

        public List<TicketingViewModel> GetTickets()
        {
            var tickets = Get();
            var ticketResults = new List<TicketingViewModel>();
            var promos = promo.GetProductPromotions().ToList();

            foreach (var ticket in tickets)
            {
                var promoPrice = new PromotionalPrice
                {
                    Fascia = ticket.Fascia,
                    ProductId = ticket.ProductId,
                    SKU = ticket.SKU,
                    Hierarchy = ticket.Hierarchy,
                    LocationId = ticket.LocationId,
                    NormalCashPrice = ticket.NormalCashPrice,
                    NormalRegularPrice = ticket.NormalRegularPrice,
                    NormalDutyFreePrice = ticket.DutyFreePrice,
                    TaxRate = ticket.TaxRate
                };

                var applicablePromo = promo.CalculatePromotions(promoPrice, promos);

                var t = new TicketingViewModel
                {
                    SKU = ticket.SKU,
                    ModelNumber =ticket.VendorStyleLong,
                    Brand = ticket.BrandName,
                    Fascia = ticket.Fascia,
                    Location = ticket.LocationName,
                    LongDescription = ticket.LongDescription,
                    POSDescription = ticket.POSDescription,
                    EffectiveDate = applicablePromo != null ? applicablePromo.StartDate.ToString("yyyy-MM-dd") : ticket.EffectiveDate,
                    CurrentCashPrice = RoundedPriceString(applicablePromo != null && applicablePromo.PromoCashPrice != null ? applicablePromo.PromoCashPrice.Value : ApplyTax(ticket.NormalCashPrice, ticket.TaxRate)),
                    CurrentRegularPrice = RoundedPriceString(applicablePromo != null && applicablePromo.PromoRegularPrice != null ? applicablePromo.PromoRegularPrice.Value : ApplyTax(ticket.CurrentRegularPrice, ticket.TaxRate)),
                    DutyFreePrice = RoundedPriceString(applicablePromo != null && applicablePromo.PromoDutyFreePrice != null ? applicablePromo.PromoDutyFreePrice.Value : ApplyTax(ticket.DutyFreePrice, ticket.TaxRate)),
                    SetCode = ticket.SetCode,
                    SetDescription = ticket.SetDescription,
                    NormalCashPrice = RoundedPriceString(ApplyTax(ticket.NormalCashPrice, ticket.TaxRate)),
                    NormalRegularPrice = RoundedPriceString(ApplyTax(ticket.NormalRegularPrice, ticket.TaxRate))
                };

                var comps = ticket.Components.Select(c => c.SKU + " - " + c.LongDescription).ToList();
                while (comps.Count < 10)
                {
                    comps.Add(string.Empty);
                }

                if (comps.Count > 10)
                {
                    comps.RemoveRange(10, comps.Count()-10);
                }
                
                t.Components = string.Join(",", comps.ToArray());

                var features = ticket.Features.Select(f => f.Value).ToList();
                while (features.Count < 10)
                {
                    features.Add(string.Empty);
                }

                if (features.Count > 10)
                {
                    features.RemoveRange(10, features.Count() - 10);
                }

                t.Features = string.Join(",", features.ToArray());
                ticketResults.Add(t);
             }
            return ticketResults;
        }
    }
}
