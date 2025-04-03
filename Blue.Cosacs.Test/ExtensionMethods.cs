using System.Data.Entity;
using System.Linq;
using Moq;

namespace Blue.Cosacs.Test
{
    internal static class ExtensionMethods
    {
        public static void SetuptQuerableMethods<T>(this Mock<DbSet<T>> mockedObject, IQueryable<T> source)
            where T : class
        {
            mockedObject.As<IQueryable<T>>().Setup(m => m.Provider).Returns(source.Provider);
            mockedObject.As<IQueryable<T>>().Setup(m => m.Expression).Returns(source.Expression);
            mockedObject.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(source.ElementType);
            mockedObject.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(source.GetEnumerator());
        }
    }
}
