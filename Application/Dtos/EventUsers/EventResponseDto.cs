using Microsoft.AspNetCore.Http;

namespace Application.Dtos.EventUsers;

public class EventResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }
    public DateTime DateTime { get; set; }
    public int MaxParticipants { get; set; }
    public IFormFile? Image { get; set; } 
    public string CategoryName { get; set; }
}