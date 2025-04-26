using Application.Dtos.EventUsers;
using Domain.Entities;
using Mapster;

namespace Application.Mappings;

public class EventMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<EventRequestDto, Event>()
            .Map(dest => dest.ImagePath, src => string.Empty) 
            .Ignore(dest => dest.Category) 
            .Ignore(dest => dest.Participants);
        
        config.NewConfig<Event, EventRequestDto>();
    }
}