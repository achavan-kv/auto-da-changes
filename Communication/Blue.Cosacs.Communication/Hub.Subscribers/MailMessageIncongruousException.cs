using System;

namespace Blue.Cosacs.Communication.Hub.Subscribers
{
    public sealed class MailMessageIncongruousException : Exception
    {
        public MailMessageIncongruousException(string incongruousField)
            : base(string.Format("The field {0} is null or empty. It should be set either at MaillMessage level or at individual Mail level.", incongruousField))
        {
        }
    }
}
