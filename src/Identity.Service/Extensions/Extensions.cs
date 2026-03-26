using Identity.Service.Dtos;
using Identity.Service.Models;

namespace Identity.Service.Extensions;

public static class DtoExtensions
{
    public static UserDto AsDto(this ApplicationUser user) =>
        new(
            user.Id,
            user.UserName!,
            user.Email!,
            user.Gil,
            user.CreatedOn);
}