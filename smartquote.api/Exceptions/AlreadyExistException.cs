namespace smartquote.api.Exceptions;

public class AlreadyExistException : Exception
{
    public AlreadyExistException()
        : base("The specified resource already exists.")
    {
    }

    public AlreadyExistException(string message)
       : base(message)
    {
    }

    public AlreadyExistException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
