namespace Blue.Cosacs.Merchandising.Infrastructure
{
    using System;

    using global::Elmah;

    public class Log : ILog
    {
        public void Exception(Exception ex)
        {
            ErrorSignal.FromCurrentContext().Raise(ex);
        }

        public void Exception(Exception ex, string message)
        {
            ErrorSignal.FromCurrentContext().Raise(new Exception(message, ex));
        }

        public void Exception(string message)
        {
            ErrorSignal.FromCurrentContext().Raise(new Exception(message));
        }
    }
}
