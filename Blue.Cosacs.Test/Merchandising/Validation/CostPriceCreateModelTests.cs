namespace Blue.Cosacs.Test.Merchandising.Validation
{
    using System.Collections.Generic;
    using Blue.Cosacs.Merchandising.Models;
    using Blue.Cosacs.Merchandising.Publishers;
    using Ploeh.AutoFixture;

    public class CostPriceCreateModelTests : ValidationTestFixture<CostPriceCreateModel>
    {
        public override IEnumerable<CostPriceCreateModel> CreateGoodModels()
        {
            yield return Fixture.Build<CostPriceCreateModel>()
                .Create();
        }

        public override IEnumerable<CostPriceCreateModel> CreateBadModels()
        {
            yield return Fixture.Build<CostPriceCreateModel>()
                .Without(x => x.ProductId)
                .Create();

            yield return Fixture.Build<CostPriceCreateModel>()
                .Without(x => x.SupplierCost)
                .Create();

            yield return Fixture.Build<CostPriceCreateModel>()
                .Without(x => x.LastLandedCost)
                .Create();

            yield return Fixture.Build<CostPriceCreateModel>()
                .Without(x => x.SupplierCurrency)
                .Create();

            yield return Fixture.Build<CostPriceCreateModel>()
                .With(x => x.SupplierCost, -1)
                .Create();

            yield return Fixture.Build<CostPriceCreateModel>()
                .With(x => x.LastLandedCost, -1)
                .Create();
        }
    }
}
