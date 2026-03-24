namespace Identity.Service.Dtos;

public record RegisterUserDto(
    string Username,
    string Email,
    string Password,
    decimal StartingGil = 0);