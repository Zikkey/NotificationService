namespace Bridge.Shared.Models.Responses;

public class PacketProcessed
{
    public bool IsProcessed { get; set; }
    public string? ErrorMessage { get; set; }
}
