using Identity.Service.Dtos;
using Identity.Service.Models;

namespace Identity.Service;

public static class Extensions
{
    public static UserDto AsDto(this ApplicationUser user) =>
        new(
            user.Id,
            user.UserName!,
            user.Email!,
            user.Gil,
            user.CreatedOn);
}