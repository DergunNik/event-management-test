using System.Security.Cryptography;
using System.Text;
using Application.Options;
using Konscious.Security.Cryptography;
using Microsoft.Extensions.Options;

namespace Application.Services.Auth.Helpers;

public class Argon2PasswordHasher : IPasswordHasher
{
    private readonly HashOptions _options;

    public Argon2PasswordHasher(IOptions<HashOptions> options)
    {
        _options = options.Value;
    }

    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(_options.SaltSize);
        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));
        argon2.Iterations = _options.Iterations;
        argon2.MemorySize = _options.MemorySize;
        argon2.DegreeOfParallelism = _options.DegreeOfParallelism;
        argon2.Salt = salt;
        var hash = argon2.GetBytes(_options.PasswordHashSize);
        return $"{Convert.ToBase64String(hash)}-{Convert.ToBase64String(salt)}";
    }

    public bool Verify(string password, string storedHash)
    {
        var data = storedHash.Split('-');
        var passwordHash = Convert.FromBase64String(data[0]);
        var salt = Convert.FromBase64String(data[1]);
        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));
        argon2.Iterations = _options.Iterations;
        argon2.MemorySize = _options.MemorySize;
        argon2.DegreeOfParallelism = _options.DegreeOfParallelism;
        argon2.Salt = salt;
        var newHash = argon2.GetBytes(_options.PasswordHashSize);
        return passwordHash == newHash;
    }

    public async Task<string> HashAsync(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(_options.SaltSize);
        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));
        argon2.Iterations = _options.Iterations;
        argon2.MemorySize = _options.MemorySize;
        argon2.DegreeOfParallelism = _options.DegreeOfParallelism;
        argon2.Salt = salt;
        var hash = await argon2.GetBytesAsync(_options.PasswordHashSize);
        return $"{Convert.ToBase64String(hash)}-{Convert.ToBase64String(salt)}";
    }

    public async Task<bool> VerifyAsync(string password, string storedHash)
    {
        var data = storedHash.Split('-');
        var passwordHash = Convert.FromBase64String(data[0]);
        var salt = Convert.FromBase64String(data[1]);
        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));
        argon2.Iterations = _options.Iterations;
        argon2.MemorySize = _options.MemorySize;
        argon2.DegreeOfParallelism = _options.DegreeOfParallelism;
        argon2.Salt = salt;
        var newHash = await argon2.GetBytesAsync(_options.PasswordHashSize);
        return passwordHash.SequenceEqual(newHash);
    }
}