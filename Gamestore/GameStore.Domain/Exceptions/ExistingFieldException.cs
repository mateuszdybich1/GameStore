namespace GameStore.Domain.Exceptions;

public class ExistingFieldException(string message) : Exception(message)
{
}
