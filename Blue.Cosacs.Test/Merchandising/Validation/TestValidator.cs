namespace Blue.Cosacs.Test.Merchandising.Validation
{
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;
    using System.Linq;
    using Artemis.Runtime.Generic;
    using NUnit.Framework;

    public class TestValidator<TModel>
    {
        private readonly IEnumerable<TModel> models;
        private readonly bool shouldPass;

        public TestValidator(IEnumerable<TModel> models, bool shouldPass)
        {
            this.models = models;
            this.shouldPass = shouldPass;
        }

        private void ValidateObject(TModel model)
        {
            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            var pass = Validator.TryValidateObject(model, context, results, true);

            Assert.AreEqual(pass, shouldPass, string.Format("Validation {0} when it should have {1}. \r\n{2}", pass ? "passed" : "failed", shouldPass ? "passed" : "failed", string.Join("\r\n", results.Select(r => r.MemberNames.First() + ": " + r.ErrorMessage))));
        }

        public void Run()
        {
            models.ForEach(ValidateObject);
        }
    }
}
