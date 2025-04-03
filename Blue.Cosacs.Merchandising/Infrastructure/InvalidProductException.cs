namespace Blue.Cosacs.Merchandising.Infrastructure
{
    using System;

    public class InvalidProductException : Exception
    {
        public InvalidProductException(string message, Exception innerException = null)
            : base(message, innerException)
        {
        }
    }
}