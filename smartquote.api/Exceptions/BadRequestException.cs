namespace smartquote.api.Exceptions;

public class BadRequestException : Exception
{
    public BadRequestException()
       : base("Your request could not be processed. Please check your input and try again.")
    {
    }

    public BadRequestException(string message)
       : base(message)
    {
    }

    public BadRequestException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
