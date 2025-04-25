namespace Application.Services;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string storedHash);
    Task<string> HashAsync(string password);
    Task<bool> VerifyAsync(string password, string storedHash);
}