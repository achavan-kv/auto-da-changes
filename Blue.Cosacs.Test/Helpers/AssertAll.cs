using System;
using System.Collections.Generic;
using System.Linq;

namespace Blue.Cosacs.Test.Helpers
{
    using System.Reflection;
    using NUnit.Framework;

    public static class AssertAll
    {
        public static void Succeed(params Action[] assertions)
        {
            var errors = new List<Exception>();

            foreach (var assertion in assertions)
            {
                try
                {
                    assertion();
                }
                catch (Exception ex)
                {
                    errors.Add(ex);
                }
            }

            if (errors.Any())
            {
                var ex = new AssertionException(
                    string.Join(Environment.NewLine, errors.Select(e => e.Message)),
                    errors.First());

                // Use stack trace from the first exception to ensure first
                // failed Assert is one click away
                ReplaceStackTrace(ex, errors.First().StackTrace);

                throw ex;
            }
        }

        private static void ReplaceStackTrace(Exception exception, string stackTrace)
        {
            var remoteStackTraceString = typeof(Exception).GetField("_remoteStackTraceString",BindingFlags.Instance | BindingFlags.NonPublic);
            remoteStackTraceString.SetValue(exception, stackTrace);
        }
    }
}
