namespace Ordering.Domain.Exceptions;
public class DomainException(string message) : Exception($"Domain Exceptions: \"{message}\" throws from Domain Layer.")
{
}
