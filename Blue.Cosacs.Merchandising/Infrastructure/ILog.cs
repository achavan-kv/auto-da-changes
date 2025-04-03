namespace Blue.Cosacs.Merchandising.Infrastructure
{
    using System;

    public interface ILog
    {
        void Exception(Exception ex);

        void Exception(Exception ex, string message);

        void Exception(string message);
    }
}