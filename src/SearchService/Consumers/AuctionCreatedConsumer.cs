using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;

namespace SearchService.Consumers;

public class AuctionCreatedConsumer : IConsumer<AuctionCreated>
{
    private readonly IMapper _mapper;

    public AuctionCreatedConsumer(IMapper mapper)
    {
        _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<AuctionCreated> context)
    {
        Console.WriteLine($"--> auction created: {context.Message.Id}");

        var item = _mapper.Map<Item>(context.Message);

        if (item.Model == "Foo")
        {
            throw new ArgumentException("Cannot sell car with the model Foo");
        }

        await item.SaveAsync();
    }
}