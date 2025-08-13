
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using smartquote.api.Data;
using smartquote.api.Entities;
using smartquote.api.Repositories;
using smartquote.api.Repositories.Interfaces;

namespace smartquote.tests.Unit.Repositories;

public class UnitOfWorkTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly SmartQuoteDbContext _dbContext;

    public UnitOfWorkTests()
    {
        var options = new DbContextOptionsBuilder<SmartQuoteDbContext>()
          .UseInMemoryDatabase(databaseName: "SmartQuoteTestDb")
            .Options;

        _dbContext = new SmartQuoteDbContext(options);
        _unitOfWork = new UnitOfWork(_dbContext);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldCommitChanges_WhenCalled()
    {
        var user = new User
        {
            FirstName = "Test",
            LastName = "User",
            FullName = "Test User",
            Email = "test@example.com",
            PasswordHash = "hashedpassword",
        };

        await _unitOfWork.Users.AddAsync(user);
        var result = await _unitOfWork.SaveChangesAsync();
        var savedUser = await _unitOfWork.Users.GetByEmailAsync(user.Email);

        result.Should().Be(1);
        savedUser.Should().NotBeNull();
        savedUser.FullName.Should().Be("Test User");
        savedUser.Email.Should().Be("test@example.com");
        savedUser.PasswordHash.Should().Be("hashedpassword");

        await _unitOfWork.DisposeAsync();
    }
}