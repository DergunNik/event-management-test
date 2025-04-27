namespace Application.Dtos.Participant;

public class ManageParticipantResponse(bool success, string? description = null)
{
    public bool Success { get; set; } = success;
    public string? ErrorDescription { get; set; } = description;
}