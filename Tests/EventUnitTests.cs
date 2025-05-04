using Application.Abstractions;
using Application.Dtos.Event;
using Application.Services.Event;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.EFCore;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

public class EventServiceEfCoreTests
{
    private static AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("EventTestDb_" + Guid.NewGuid())
            .Options;
        return new AppDbContext(options);
    }

    private static IUnitOfWork GetUnitOfWork(AppDbContext db)
    {
        return new EfcUnitOfWork(db, new NullLogger<EfcUnitOfWork>());
    }

    private static async Task SeedCategory(AppDbContext db, int id = 1, string name = "Cat")
    {
        await db.Categories.AddAsync(new Category { Id = id, Name = name });
        await db.SaveChangesAsync();
    }

    [Fact]
    public async Task AddEventAsync_Saves_Event()
    {
        await using var db = GetInMemoryDbContext();
        await SeedCategory(db);
        var uow = GetUnitOfWork(db);
        var service = new EventService(uow);

        var dto = new EventCreateDto
        {
            Title = "Test",
            Description = "Desc",
            Location = "Loc",
            DateTime = DateTime.Now,
            MaxParticipants = 10,
            CategoryId = 1
        };
        await service.AddEventAsync(dto);

        var fromDb = await db.Events.FirstOrDefaultAsync(x => x.Title == "Test");
        Assert.NotNull(fromDb);
        Assert.Equal("Test", fromDb.Title);
    }

    [Fact]
    public async Task AddEventAsync_InvalidCategory_Throws()
    {
        await using var db = GetInMemoryDbContext();
        var uow = GetUnitOfWork(db);
        var service = new EventService(uow);

        var dto = new EventCreateDto
        {
            Title = "Test",
            Description = "Desc",
            Location = "Loc",
            DateTime = DateTime.Now,
            MaxParticipants = 10,
            CategoryId = 99
        };
        await Assert.ThrowsAsync<NullReferenceException>(() => service.AddEventAsync(dto));
    }

    [Fact]
    public async Task GetEventAsync_Returns_Event()
    {
        await using var db = GetInMemoryDbContext();
        await SeedCategory(db);
        var eventEntity = new Event
        {
            Title = "Test2",
            Description = "Desc2",
            Location = "Loc2",
            DateTime = DateTime.Now,
            MaxParticipants = 11,
            CategoryId = 1
        };
        await db.Events.AddAsync(eventEntity);
        await db.SaveChangesAsync();

        var uow = GetUnitOfWork(db);
        var service = new EventService(uow);

        var fromRepo = await service.GetEventAsync(eventEntity.Id);
        Assert.NotNull(fromRepo);
        Assert.Equal(eventEntity.Title, fromRepo.Title);
    }

    [Fact]
    public async Task GetEventAsync_Returns_Null_For_Missing()
    {
        await using var db = GetInMemoryDbContext();
        var uow = GetUnitOfWork(db);
        var service = new EventService(uow);

        var fromRepo = await service.GetEventAsync(999);
        Assert.Null(fromRepo);
    }

    [Fact]
    public async Task GetAllEventsAsync_ReturnsAll()
    {
        await using var db = GetInMemoryDbContext();
        await SeedCategory(db);
        await db.Events.AddAsync(new Event
        {
            Title = "A",
            Description = "-",
            Location = "-",
            DateTime = DateTime.Now,
            MaxParticipants = 1,
            CategoryId = 1
        });
        await db.Events.AddAsync(new Event
        {
            Title = "B",
            Description = "-",
            Location = "-",
            DateTime = DateTime.Now,
            MaxParticipants = 1,
            CategoryId = 1
        });
        await db.SaveChangesAsync();

        var uow = GetUnitOfWork(db);
        var service = new EventService(uow);

        var result = await service.GetAllEventsAsync();
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetEventsAsync_ByFilter_Works()
    {
        await using var db = GetInMemoryDbContext();
        await SeedCategory(db);
        await db.Events.AddAsync(new Event
        {
            Title = "A",
            Description = "-",
            Location = "One",
            DateTime = DateTime.Now,
            MaxParticipants = 1,
            CategoryId = 1
        });
        await db.Events.AddAsync(new Event
        {
            Title = "B",
            Description = "-",
            Location = "Two",
            DateTime = DateTime.Now,
            MaxParticipants = 1,
            CategoryId = 1
        });
        await db.SaveChangesAsync();

        var uow = GetUnitOfWork(db);
        var service = new EventService(uow);

        var result = await service.GetEventsAsync(e => e.Location == "Two");
        Assert.Single(result);
        Assert.Equal("Two", result.First().Location);
    }

    [Fact]
    public async Task UpdateEventAsync_Updates_Event()
    {
        await using var db = GetInMemoryDbContext();
        await SeedCategory(db, 1, "OldCat");
        await SeedCategory(db, 2, "NewCat");
        var eventEntity = new Event
        {
            Title = "Old",
            Description = "Old",
            Location = "Old",
            DateTime = DateTime.Now,
            MaxParticipants = 1,
            CategoryId = 1
        };
        await db.Events.AddAsync(eventEntity);
        await db.SaveChangesAsync();

        var uow = GetUnitOfWork(db);
        var service = new EventService(uow);

        var updateDto = new EventUpdateDto
        {
            Id = eventEntity.Id,
            Title = "New",
            Description = "New",
            Location = "New",
            DateTime = DateTime.Now,
            MaxParticipants = 2,
            CategoryId = 2
        };
        await service.UpdateEventAsync(updateDto);

        var fromDb = await db.Events.FindAsync(eventEntity.Id);
        Assert.Equal("New", fromDb.Title);
        Assert.Equal(2, fromDb.CategoryId);
    }

    [Fact]
    public async Task UpdateEventAsync_EventNotFound_Throws()
    {
        var db = GetInMemoryDbContext();
        await using var db1 = db.ConfigureAwait(false);
        await SeedCategory(db);
        var uow = GetUnitOfWork(db);
        var service = new EventService(uow);

        var updateDto = new EventUpdateDto
        {
            Id = 42,
            Title = "New",
            Description = "New",
            Location = "New",
            DateTime = DateTime.Now,
            MaxParticipants = 2,
            CategoryId = 1
        };

        await Assert.ThrowsAsync<NullReferenceException>(() => service.UpdateEventAsync(updateDto));
    }

    [Fact]
    public async Task UpdateEventAsync_CategoryNotFound_Throws()
    {
        await using var db = GetInMemoryDbContext();
        await SeedCategory(db);
        var eventEntity = new Event
        {
            Title = "Event",
            Description = "Event",
            Location = "Event",
            DateTime = DateTime.Now,
            MaxParticipants = 1,
            CategoryId = 1
        };
        await db.Events.AddAsync(eventEntity);
        await db.SaveChangesAsync();

        var uow = GetUnitOfWork(db);
        var service = new EventService(uow);

        var updateDto = new EventUpdateDto
        {
            Id = eventEntity.Id,
            Title = "New",
            Description = "New",
            Location = "New",
            DateTime = DateTime.Now,
            MaxParticipants = 2,
            CategoryId = 999
        };

        await Assert.ThrowsAsync<NullReferenceException>(() => service.UpdateEventAsync(updateDto));
    }

    [Fact]
    public async Task DeleteEventAsync_Deletes_Event()
    {
        await using var db = GetInMemoryDbContext();
        await SeedCategory(db);
        var eventEntity = new Event
        {
            Title = "Del",
            Description = "Del",
            Location = "Del",
            DateTime = DateTime.Now,
            MaxParticipants = 1,
            CategoryId = 1
        };
        await db.Events.AddAsync(eventEntity);
        await db.SaveChangesAsync();

        var uow = GetUnitOfWork(db);
        var service = new EventService(uow);

        await service.DeleteEventAsync(eventEntity.Id);
        var fromDb = await db.Events.FindAsync(eventEntity.Id);
        Assert.Null(fromDb);
    }

    [Fact]
    public async Task DeleteEventAsync_EventNotFound_Throws()
    {
        await using var db = GetInMemoryDbContext();
        await SeedCategory(db);
        var uow = GetUnitOfWork(db);
        var service = new EventService(uow);

        await Assert.ThrowsAsync<NullReferenceException>(() => service.DeleteEventAsync(999));
    }

    [Fact]
    public async Task SetEventImageAsync_SavesPath()
    {
        await using var db = GetInMemoryDbContext();
        await SeedCategory(db);
        var eventEntity = new Event
        {
            Title = "Img",
            Description = "Img",
            Location = "Img",
            DateTime = DateTime.Now,
            MaxParticipants = 1,
            CategoryId = 1
        };
        await db.Events.AddAsync(eventEntity);
        await db.SaveChangesAsync();

        var uow = GetUnitOfWork(db);
        var service = new EventService(uow);

        var bytes = new byte[] { 1, 2, 3 };
        var stream = new MemoryStream(bytes);
        var formFile = new FormFile(stream, 0, bytes.Length, "image", "test.png");

        Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images"));

        await service.SetEventImageAsync(eventEntity.Id, formFile);

        var fromDb = await db.Events.FindAsync(eventEntity.Id);
        Assert.NotNull(fromDb.ImagePath);
        Assert.EndsWith(".png", fromDb.ImagePath);
    }

    [Fact]
    public async Task SetEventImageAsync_EventNotFound_Throws()
    {
        await using var db = GetInMemoryDbContext();
        await SeedCategory(db);
        var uow = GetUnitOfWork(db);
        var service = new EventService(uow);

        var bytes = new byte[] { 1, 2, 3 };
        var stream = new MemoryStream(bytes);
        var formFile = new FormFile(stream, 0, bytes.Length, "image", "test.png");

        await Assert.ThrowsAsync<NullReferenceException>(() => service.SetEventImageAsync(999, formFile));
    }

    [Fact]
    public async Task GetEventImageAsync_Returns_Path()
    {
        await using var db = GetInMemoryDbContext();
        await SeedCategory(db);
        var eventEntity = new Event
        {
            Title = "Img",
            Description = "Img",
            Location = "Img",
            DateTime = DateTime.Now,
            MaxParticipants = 1,
            CategoryId = 1,
            ImagePath = "/images/test.png"
        };
        await db.Events.AddAsync(eventEntity);
        await db.SaveChangesAsync();

        var uow = GetUnitOfWork(db);
        var service = new EventService(uow);

        var path = await service.GetEventImageAsync(eventEntity.Id);
        Assert.Equal("/images/test.png", path);
    }

    [Fact]
    public async Task GetEventImageAsync_EventNotFound_Throws()
    {
        await using var db = GetInMemoryDbContext();
        await SeedCategory(db);
        var uow = GetUnitOfWork(db);
        var service = new EventService(uow);

        await Assert.ThrowsAsync<NullReferenceException>(() => service.GetEventImageAsync(999));
    }
}