namespace Blue.Cosacs.Merchandising.Publishers
{
    using System.Collections.Generic;
    using Blue.Cosacs.Test.Merchandising.Validation;
    using NUnit.Framework;
    using Ploeh.AutoFixture;

    [TestFixture]
    public abstract class ValidationTestFixture<TModel>
    {
        private readonly Fixture fixture = new Fixture();
        private TestValidator<TModel> goodValidator;
        private TestValidator<TModel> badValidator;
        
        [SetUp]
        public void SetUp()
        {
            goodValidator = new TestValidator<TModel>(CreateGoodModels(), shouldPass: true);
            badValidator = new TestValidator<TModel>(CreateBadModels(), shouldPass: false);
        }

        public abstract IEnumerable<TModel> CreateGoodModels();
        public abstract IEnumerable<TModel> CreateBadModels();

        public Fixture Fixture
        {
            get { return fixture; }
        }

        [Test]
        public void Good_Models_AreValid()
        {
            goodValidator.Run();
        }

        [Test]
        public void Bad_Models_AreInvalid()
        {
            badValidator.Run();
        }
    }
}