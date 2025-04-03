namespace Blue.Cosacs.Test.Merchandising.Validation
{
    using System.Collections.Generic;
    using Blue.Cosacs.Merchandising.Models;
    using Blue.Cosacs.Merchandising.Publishers;
    using Ploeh.AutoFixture;

    public class GoodsOnLoanCreateModelTests : ValidationTestFixture<GoodsOnLoanCreateModel>
    {
        public override IEnumerable<GoodsOnLoanCreateModel> CreateGoodModels()
        {
            yield return Fixture.Build<GoodsOnLoanCreateModel>()
                .Create();
        }

        public override IEnumerable<GoodsOnLoanCreateModel> CreateBadModels()
        {
            yield return Fixture.Build<GoodsOnLoanCreateModel>()
                .Without(x => x.Title)
                .Create();

            yield return Fixture.Build<GoodsOnLoanCreateModel>()
                .Without(x => x.FirstName)
                .Create();

            yield return Fixture.Build<GoodsOnLoanCreateModel>()
                .Without(x => x.LastName)
                .Create();

            yield return Fixture.Build<GoodsOnLoanCreateModel>()
                .Without(x => x.AddressLine1)
                .Create();

            yield return Fixture.Build<GoodsOnLoanCreateModel>()
                .With(x => x.Title, "ABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX")
                .Create();

            yield return Fixture.Build<GoodsOnLoanCreateModel>()
                .With(x => x.FirstName, "ABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJX")
                .Create();

            yield return Fixture.Build<GoodsOnLoanCreateModel>()
                .With(x => x.LastName, "ABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJX")
                .Create();

            yield return Fixture.Build<GoodsOnLoanCreateModel>()
                .With(x => x.AddressLine1, "ABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJX")
                .Create();

            yield return Fixture.Build<GoodsOnLoanCreateModel>()
                .With(x => x.TownCity, "ABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJX")
                .Create();

            yield return Fixture.Build<GoodsOnLoanCreateModel>()
                .With(x => x.PostCode, "ABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJX")
                .Create();
        }
    }
}
