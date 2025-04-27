using System.Security.Cryptography;
using System.Text;
using Application.Options;
using Konscious.Security.Cryptography;
using Microsoft.Extensions.Options;

namespace Application.Services;

public class Argon2PasswordHasher(IOptions<HashOptions> options) : IPasswordHasher
{
    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(options.Value.SaltSize);
        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));
        argon2.Iterations = options.Value.Iterations;
        argon2.MemorySize = options.Value.MemorySize;
        argon2.DegreeOfParallelism = options.Value.DegreeOfParallelism;
        argon2.Salt = salt;
        var hash = argon2.GetBytes(options.Value.PasswordHashSize); 
        return $"{Convert.ToBase64String(hash)}-{Convert.ToBase64String(salt)}";
    }
    
    public bool Verify(string password, string storedHash)
    {
        var data = storedHash.Split('-');
        var passwordHash = Convert.FromBase64String(data[0]);
        var salt = Convert.FromBase64String(data[1]);
        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));
        argon2.Iterations = options.Value.Iterations;
        argon2.MemorySize = options.Value.MemorySize;
        argon2.DegreeOfParallelism = options.Value.DegreeOfParallelism;
        argon2.Salt = salt;
        var newHash = argon2.GetBytes(options.Value.PasswordHashSize);
        return passwordHash == newHash;
    }
    
    public async Task<string> HashAsync(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(options.Value.SaltSize);
        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));
        argon2.Iterations = options.Value.Iterations;
        argon2.MemorySize = options.Value.MemorySize;
        argon2.DegreeOfParallelism = options.Value.DegreeOfParallelism;
        argon2.Salt = salt;
        var hash = await argon2.GetBytesAsync(options.Value.PasswordHashSize); 
        return $"{Convert.ToBase64String(hash)}-{Convert.ToBase64String(salt)}";
    }

    public async Task<bool> VerifyAsync(string password, string storedHash)
    {
        var data = storedHash.Split('-');
        var passwordHash = Convert.FromBase64String(data[0]);
        var salt = Convert.FromBase64String(data[1]);
        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));
        argon2.Iterations = options.Value.Iterations;
        argon2.MemorySize = options.Value.MemorySize;
        argon2.DegreeOfParallelism = options.Value.DegreeOfParallelism;
        argon2.Salt = salt;
        var newHash = await argon2.GetBytesAsync(options.Value.PasswordHashSize);
        return passwordHash.SequenceEqual(newHash);
    }
}