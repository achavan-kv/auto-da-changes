using Moq;
using System.Data.Entity;
using System.Linq;

namespace Blue.Cosacs.NonStocks.Test.Extensions
{
    public static class Extensions
    {
        public static void SetupQueryableMethods<T>(this Mock<DbSet<T>> mockObject,
            IQueryable<T> source) where T : class
        {
            mockObject.As<IQueryable<T>>().Setup(m => m.Provider).Returns(source.Provider);
            mockObject.As<IQueryable<T>>().Setup(m => m.Expression).Returns(source.Expression);
            mockObject.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(source.ElementType);
            mockObject.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(source.GetEnumerator());
        }
    }
}