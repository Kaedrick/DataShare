using DataShare.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace DataShare.Api.Tests.Helpers;

public static class TestDatabase
{
    public static AppDbContext Create()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}
