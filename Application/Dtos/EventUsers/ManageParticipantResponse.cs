namespace Application.Dtos.EventUsers;

public class ManageParticipantResponse(bool isAdded, string? description = null)
{
    public bool IsAdded { get; set; } = isAdded;
    public string? ErrorDescription { get; set; } = description;
}