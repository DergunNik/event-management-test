using Application.Abstractions;
using Application.Dtos.Event;
using Domain.Entities;
using Mapster;

namespace Application.Mappings;

public class EventMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<EventCreateDto, Event>()
            .Map(dest => dest.ImagePath, src => string.Empty)
            .Ignore(dest => dest.Category)
            .Ignore(dest => dest.Participants);

        config.NewConfig<Event, EventCreateDto>();

        config.NewConfig<EventUpdateDto, Event>()
            .Ignore(dest => dest.Category)
            .Ignore(dest => dest.Participants);

        config.NewConfig<Event, EventUpdateDto>();

        config.NewConfig<EventDto, Event>()
            .Ignore(dest => dest.Category)
            .Ignore(dest => dest.Participants);

        config.NewConfig<Event, EventDto>();

        config.NewConfig<PagedResult<Event>, EventPageDto>()
            .Ignore(dest => dest.Events);
    }
}