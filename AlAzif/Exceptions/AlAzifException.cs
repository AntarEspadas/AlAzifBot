namespace AlAzif.Exceptions;

public class AlAzifException : Exception
{
    public AlAzifException()
    {
    }

    public AlAzifException(string message) : base(message)
    {
    }

    public AlAzifException(string message, Exception inner) : base(message, inner)
    {
    }
}