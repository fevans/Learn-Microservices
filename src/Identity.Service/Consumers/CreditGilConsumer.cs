using GamePlatform.Identity.Contracts;
using Identity.Service.Exceptions;
using Identity.Service.Models;
using MassTransit;
using Microsoft.AspNetCore.Identity;

namespace Identity.Service.Consumers;

public class CreditGilConsumer(UserManager<ApplicationUser> userManager)
    : IConsumer<CreditGil>
{
    public async Task Consume(ConsumeContext<CreditGil> context)
    {
        var message = context.Message;

        var user = await userManager.FindByIdAsync(message.UserId.ToString());

        if (user is null)
        {
            throw new UnknownUserException(message.UserId);
        }

        if (user.MessageIds.Contains(context.MessageId!.Value))
        {
            // Idempotency guard
            await context.Publish(new GilCredited(message.CorrelationId));
            return;
        }

        user.Gil      += message.Gil;
        user.GilSpent -= message.Gil;
        user.MessageIds.Add(context.MessageId!.Value);

        await userManager.UpdateAsync(user);

        await context.Publish(new GilCredited(message.CorrelationId));
    }
}