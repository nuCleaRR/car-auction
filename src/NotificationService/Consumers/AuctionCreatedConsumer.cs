using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace NotificationService;

public class AuctionCreatedConsumer : IConsumer<AuctionCreatedConsumer>
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public AuctionCreatedConsumer(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task Consume(ConsumeContext<AuctionCreatedConsumer> context)
    {
        Console.WriteLine("---> auction created message received");

        await _hubContext.Clients.All.SendAsync("AuctionCreated", context.Message);
    }
}
