namespace Inventory.Service.Exceptions;

public class UnknownItemException(Guid itemId)
    : Exception($"Unknown item: '{itemId}'");