using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.EFCore;
using Microsoft.EntityFrameworkCore;

public class EfcRepositoryTests
{
    private static AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("RepoTestDb_" + System.Guid.NewGuid())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddAsync_SavesEntity()
    {
        await using var db = GetInMemoryDbContext();
        var repo = new EfcRepository<Event>(db);

        var ev = new Event
        {
            Title = "RepoTest",
            Description = "Desc",
            Location = "Loc",
            DateTime = System.DateTime.Now,
            MaxParticipants = 5,
            CategoryId = 1
        };
        await repo.AddAsync(ev);
        await db.SaveChangesAsync();

        var found = await db.Events.FirstOrDefaultAsync(x => x.Title == "RepoTest");
        Assert.NotNull(found);
        Assert.Equal("RepoTest", found.Title);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsEntity_WhenExists()
    {
        await using var db = GetInMemoryDbContext();
        var ev = new Event
        {
            Title = "RepoTest2",
            Description = "Desc2",
            Location = "Loc2",
            DateTime = System.DateTime.Now,
            MaxParticipants = 10,
            CategoryId = 2
        };
        await db.Events.AddAsync(ev);
        await db.SaveChangesAsync();

        var repo = new EfcRepository<Event>(db);
        var found = await repo.GetByIdAsync(ev.Id);

        Assert.NotNull(found);
        Assert.Equal(ev.Title, found.Title);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
    {
        await using var db = GetInMemoryDbContext();
        var repo = new EfcRepository<Event>(db);

        var found = await repo.GetByIdAsync(999);
        Assert.Null(found);
    }

    [Fact]
    public async Task AddAsync_ThrowsException_WhenRequiredFieldIsNull()
    {
        await using var db = GetInMemoryDbContext();
        var repo = new EfcRepository<Event>(db);
        var ev = new Event
        {
            Title = null,
            Description = "Desc", 
            Location = "Loc", 
            DateTime = DateTime.UtcNow, 
            MaxParticipants = 10, 
            CategoryId = 1
        };

        await repo.AddAsync(ev);

        await Assert.ThrowsAsync<DbUpdateException>(async () =>
        {
            await db.SaveChangesAsync();
        });
    }
}