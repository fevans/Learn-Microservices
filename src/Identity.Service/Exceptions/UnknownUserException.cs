namespace Identity.Service.Exceptions;

public class UnknownUserException(Guid userId)
    : Exception($"Unknown user: '{userId}'");