using Domain.Core.Entities;
using Domain.Core.Enums;
using Microsoft.EntityFrameworkCore;
using MockQueryable;
using MockQueryable.Moq;
using Moq;
using Repository.Core.Enums;
using Shouldly;
using WebApi.Infrastructure.Data.Contexts;
using WebApi.Infrastructure.Locks;

namespace WebApi.Infrastructure.UnitTests.Tests.Locks;

public class AccountLockTests
{
    private readonly Mock<BudgetContext> _mockContext;
    private readonly AccountLock _accountLock;
    private readonly Guid _testIdentityId;
    private readonly Guid _otherIdentityId;

    public AccountLockTests()
    {
        _mockContext = new Mock<BudgetContext>();
        _accountLock = new AccountLock(_mockContext.Object);
        _testIdentityId = Guid.NewGuid();
        _otherIdentityId = Guid.NewGuid();
    }

    #region HasAccess Tests

    [Fact]
    public async Task HasAccess_WhenUserIdMatchesIdentityId_ReturnsTrue()
    {
        // Arrange
        var account = Account.Create(
            userId: _testIdentityId,
            name: "Test Account",
            accountType: AccountTypeEnum.Checking);
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        bool result = await _accountLock.HasAccess(
            account,
            _testIdentityId,
            RepositoryOperationEnum.Read,
            cancellationToken);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public async Task HasAccess_WhenUserIdDoesNotMatchIdentityId_ReturnsFalse()
    {
        // Arrange
        var account = Account.Create(
            userId: _otherIdentityId,
            name: "Test Account",
            accountType: AccountTypeEnum.Checking);
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        bool result = await _accountLock.HasAccess(
            account,
            _testIdentityId,
            RepositoryOperationEnum.Read,
            cancellationToken);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public async Task HasAccess_WhenUserIdIsEmpty_ReturnsFalse()
    {
        // Arrange
        var account = new Account();
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        bool result = await _accountLock.HasAccess(
            account,
            _testIdentityId,
            RepositoryOperationEnum.Read,
            cancellationToken);

        // Assert
        result.ShouldBeFalse();
    }

    [Theory]
    [InlineData(RepositoryOperationEnum.Read)]
    [InlineData(RepositoryOperationEnum.Insert)]
    [InlineData(RepositoryOperationEnum.Update)]
    [InlineData(RepositoryOperationEnum.Delete)]
    public async Task HasAccess_WithMatchingIdentity_ReturnsTrueForAllOperations(RepositoryOperationEnum operation)
    {
        // Arrange
        var account = Account.Create(
            userId: _testIdentityId,
            name: "Test Account",
            accountType: AccountTypeEnum.Savings);
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        bool result = await _accountLock.HasAccess(
            account,
            _testIdentityId,
            operation,
            cancellationToken);

        // Assert
        result.ShouldBeTrue();
    }

    [Theory]
    [InlineData(RepositoryOperationEnum.Read)]
    [InlineData(RepositoryOperationEnum.Insert)]
    [InlineData(RepositoryOperationEnum.Update)]
    [InlineData(RepositoryOperationEnum.Delete)]
    public async Task HasAccess_WithNonMatchingIdentity_ReturnsFalseForAllOperations(RepositoryOperationEnum operation)
    {
        // Arrange
        var account = Account.Create(
            userId: _otherIdentityId,
            name: "Test Account",
            accountType: AccountTypeEnum.CreditCard);
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        bool result = await _accountLock.HasAccess(
            account,
            _testIdentityId,
            operation,
            cancellationToken);

        // Assert
        result.ShouldBeFalse();
    }

    #endregion

    #region Secured Tests

    [Fact]
    public void Secured_ReturnsOnlyItemsMatchingIdentityId()
    {
        // Arrange
        var accounts = new List<Account>
        {
            Account.Create(_testIdentityId, "User Account 1", AccountTypeEnum.Checking),
            Account.Create(_testIdentityId, "User Account 2", AccountTypeEnum.Savings),
            Account.Create(_otherIdentityId, "Other User Account 1", AccountTypeEnum.Cash),
            Account.Create(_otherIdentityId, "Other User Account 2", AccountTypeEnum.CreditCard)
        };

        Mock<DbSet<Account>> mockDbSet = accounts.BuildMockDbSet();
        _mockContext.Setup(c => c.Set<Account>()).Returns(mockDbSet.Object);

        // Act
        IQueryable<Account> result = _accountLock.Secured(_testIdentityId);
        var resultList = result.ToList();

        // Assert
        resultList.ShouldNotBeNull();
        resultList.Count.ShouldBe(2);
        resultList.ShouldAllBe(a => a.UserId == _testIdentityId);
    }

    [Fact]
    public void Secured_WhenNoMatchingItems_ReturnsEmptyQueryable()
    {
        // Arrange
        var accounts = new List<Account>
        {
            Account.Create(_otherIdentityId, "Other User Account 1", AccountTypeEnum.Checking),
            Account.Create(_otherIdentityId, "Other User Account 2", AccountTypeEnum.Savings)
        };

        Mock<DbSet<Account>> mockDbSet = accounts.BuildMockDbSet();
        _mockContext.Setup(c => c.Set<Account>()).Returns(mockDbSet.Object);

        // Act
        IQueryable<Account> result = _accountLock.Secured(_testIdentityId);
        var resultList = result.ToList();

        // Assert
        resultList.ShouldNotBeNull();
        resultList.ShouldBeEmpty();
    }

    [Fact]
    public void Secured_WhenAllItemsMatch_ReturnsAllItems()
    {
        // Arrange
        var accounts = new List<Account>
        {
            Account.Create(_testIdentityId, "User Account 1", AccountTypeEnum.Checking),
            Account.Create(_testIdentityId, "User Account 2", AccountTypeEnum.Savings),
            Account.Create(_testIdentityId, "User Account 3", AccountTypeEnum.Investment)
        };

        Mock<DbSet<Account>> mockDbSet = accounts.BuildMockDbSet();
        _mockContext.Setup(c => c.Set<Account>()).Returns(mockDbSet.Object);

        // Act
        IQueryable<Account> result = _accountLock.Secured(_testIdentityId);
        var resultList = result.ToList();

        // Assert
        resultList.ShouldNotBeNull();
        resultList.Count.ShouldBe(3);
        resultList.ShouldAllBe(a => a.UserId == _testIdentityId);
    }

    [Fact]
    public void Secured_ReturnsQueryableThatSupportsLinq()
    {
        // Arrange
        var accounts = new List<Account>
        {
            Account.Create(_testIdentityId, "Alpha Account", AccountTypeEnum.Checking),
            Account.Create(_testIdentityId, "Beta Account", AccountTypeEnum.Savings),
            Account.Create(_testIdentityId, "Gamma Account", AccountTypeEnum.Investment),
            Account.Create(_otherIdentityId, "Other Account", AccountTypeEnum.Cash)
        };

        Mock<DbSet<Account>> mockDbSet = accounts.BuildMockDbSet();
        _mockContext.Setup(c => c.Set<Account>()).Returns(mockDbSet.Object);

        // Act
        IQueryable<Account> secured = _accountLock.Secured(_testIdentityId);
        var result = secured
            .Where(a => a.Name.StartsWith('B'))
            .OrderBy(a => a.Name)
            .ToList();

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(1);
        result[0].Name.ShouldBe("Beta Account");
    }

    [Fact]
    public void Secured_WithEmptyIdentityId_ReturnsNoItems()
    {
        // Arrange
        var accounts = new List<Account>
        {
            Account.Create(_testIdentityId, "User Account 1", AccountTypeEnum.Checking),
            Account.Create(_otherIdentityId, "Other User Account 1", AccountTypeEnum.Savings)
        };

        Mock<DbSet<Account>> mockDbSet = accounts.BuildMockDbSet();
        _mockContext.Setup(c => c.Set<Account>()).Returns(mockDbSet.Object);

        // Act
        IQueryable<Account> result = _accountLock.Secured(Guid.Empty);
        var resultList = result.ToList();

        // Assert
        resultList.ShouldNotBeNull();
        resultList.ShouldBeEmpty();
    }

    #endregion
}
