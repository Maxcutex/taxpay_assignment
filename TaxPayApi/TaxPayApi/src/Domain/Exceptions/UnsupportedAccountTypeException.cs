namespace TaxPayApi.Domain.Exceptions;

public class UnsupportedAccountTypeException : Exception
{
    public UnsupportedAccountTypeException(string type)
        : base($"account type \"{type}\" is unsupported.")
    {
    }
}