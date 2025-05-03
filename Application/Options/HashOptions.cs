namespace Application.Options;

public class HashOptions
{
    public int SaltSize { get; set; }
    public int PasswordHashSize { get; set; }
    public int Iterations { get; set; }
    public int MemorySize { get; set; }
    public int DegreeOfParallelism { get; set; }
}