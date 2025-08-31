namespace smartquote.api.Exceptions;

public class EmailNotConfirmedException : Exception
{
    public EmailNotConfirmedException()
        : base("Email address has not been confirmed.") { }

    public EmailNotConfirmedException(string message)
        : base(message) { }

    public EmailNotConfirmedException(string message, Exception innerException)
        : base(message, innerException) { }
}
