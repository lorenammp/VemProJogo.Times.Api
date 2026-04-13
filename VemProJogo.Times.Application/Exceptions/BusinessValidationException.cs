namespace VemProJogo.Times.Application.Exceptions;

public sealed class BusinessValidationException : Exception
{
    public BusinessValidationException(string message)
        : base(message)
    {
    }
}
