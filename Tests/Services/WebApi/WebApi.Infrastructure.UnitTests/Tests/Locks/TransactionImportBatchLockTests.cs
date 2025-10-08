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

public class TransactionImportBatchLockTests
{
    private readonly Mock<BudgetContext> _mockContext;
    private readonly TransactionImportBatchLock _batchLock;
    private readonly Guid _testIdentityId;
    private readonly Guid _otherIdentityId;
    private readonly Guid _testAccountId;
    private readonly Guid _otherAccountId;

    public TransactionImportBatchLockTests()
    {
        _mockContext = new Mock<BudgetContext>();
        _batchLock = new TransactionImportBatchLock(_mockContext.Object);
        _testIdentityId = Guid.NewGuid();
        _otherIdentityId = Guid.NewGuid();
        _testAccountId = Guid.NewGuid();
        _otherAccountId = Guid.NewGuid();
    }

    #region HasAccess Tests

    [Fact]
    public async Task HasAccess_WhenAccountBelongsToUser_ReturnsTrue()
    {
        // Arrange
        var batch = TransactionImportBatch.Create(_testAccountId);
        var accounts = new List<Account>
        {
            Account.Create(_testIdentityId, "Test Account", AccountTypeEnum.Checking)
        };
        accounts[0].GetType().GetProperty("Id")!.SetValue(accounts[0], _testAccountId);

        Mock<DbSet<Account>> mockDbSet = accounts.BuildMockDbSet();
        _mockContext.Setup(c => c.Set<Account>()).Returns(mockDbSet.Object);
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        bool result = await _batchLock.HasAccess(
            batch,
            _testIdentityId,
            RepositoryOperationEnum.Read,
            cancellationToken);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public async Task HasAccess_WhenAccountBelongsToOtherUser_ReturnsFalse()
    {
        // Arrange
        var batch = TransactionImportBatch.Create(_otherAccountId);
        var accounts = new List<Account>
        {
            Account.Create(_otherIdentityId, "Other Account", AccountTypeEnum.Checking)
        };
        accounts[0].GetType().GetProperty("Id")!.SetValue(accounts[0], _otherAccountId);

        Mock<DbSet<Account>> mockDbSet = accounts.BuildMockDbSet();
        _mockContext.Setup(c => c.Set<Account>()).Returns(mockDbSet.Object);
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        bool result = await _batchLock.HasAccess(
            batch,
            _testIdentityId,
            RepositoryOperationEnum.Read,
            cancellationToken);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public async Task HasAccess_WhenAccountIdIsEmpty_ReturnsFalse()
    {
        // Arrange
        var batch = new TransactionImportBatch();
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        bool result = await _batchLock.HasAccess(
            batch,
            _testIdentityId,
            RepositoryOperationEnum.Read,
            cancellationToken);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public async Task HasAccess_WhenAccountDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var batch = TransactionImportBatch.Create(_testAccountId);
        var accounts = new List<Account>();

        Mock<DbSet<Account>> mockDbSet = accounts.BuildMockDbSet();
        _mockContext.Setup(c => c.Set<Account>()).Returns(mockDbSet.Object);
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        bool result = await _batchLock.HasAccess(
            batch,
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
        var batch = TransactionImportBatch.Create(_testAccountId);
        var accounts = new List<Account>
        {
            Account.Create(_testIdentityId, "Test Account", AccountTypeEnum.Savings)
        };
        accounts[0].GetType().GetProperty("Id")!.SetValue(accounts[0], _testAccountId);

        Mock<DbSet<Account>> mockDbSet = accounts.BuildMockDbSet();
        _mockContext.Setup(c => c.Set<Account>()).Returns(mockDbSet.Object);
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        bool result = await _batchLock.HasAccess(
            batch,
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
        var batch = TransactionImportBatch.Create(_otherAccountId);
        var accounts = new List<Account>
        {
            Account.Create(_otherIdentityId, "Other Account", AccountTypeEnum.CreditCard)
        };
        accounts[0].GetType().GetProperty("Id")!.SetValue(accounts[0], _otherAccountId);

        Mock<DbSet<Account>> mockDbSet = accounts.BuildMockDbSet();
        _mockContext.Setup(c => c.Set<Account>()).Returns(mockDbSet.Object);
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        bool result = await _batchLock.HasAccess(
            batch,
            _testIdentityId,
            operation,
            cancellationToken);

        // Assert
        result.ShouldBeFalse();
    }

    #endregion

    #region Secured Tests

    [Fact]
    public void Secured_ReturnsOnlyBatchesForUserAccounts()
    {
        // Arrange
        var accounts = new List<Account>
        {
            Account.Create(_testIdentityId, "User Account 1", AccountTypeEnum.Checking),
            Account.Create(_testIdentityId, "User Account 2", AccountTypeEnum.Savings),
            Account.Create(_otherIdentityId, "Other User Account", AccountTypeEnum.Cash)
        };
        accounts[0].GetType().GetProperty("Id")!.SetValue(accounts[0], _testAccountId);
        accounts[1].GetType().GetProperty("Id")!.SetValue(accounts[1], Guid.NewGuid());
        accounts[2].GetType().GetProperty("Id")!.SetValue(accounts[2], _otherAccountId);

        var batches = new List<TransactionImportBatch>
        {
            TransactionImportBatch.Create(_testAccountId),
            TransactionImportBatch.Create(_testAccountId),
            TransactionImportBatch.Create(_otherAccountId),
            TransactionImportBatch.Create(_otherAccountId)
        };

        Mock<DbSet<Account>> mockAccountDbSet = accounts.BuildMockDbSet();
        Mock<DbSet<TransactionImportBatch>> mockBatchDbSet = batches.BuildMockDbSet();
        _mockContext.Setup(c => c.Set<Account>()).Returns(mockAccountDbSet.Object);
        _mockContext.Setup(c => c.Set<TransactionImportBatch>()).Returns(mockBatchDbSet.Object);

        // Act
        IQueryable<TransactionImportBatch> result = _batchLock.Secured(_testIdentityId);
        var resultList = result.ToList();

        // Assert
        resultList.ShouldNotBeNull();
        resultList.Count.ShouldBe(2);
        resultList.ShouldAllBe(b => b.AccountId == _testAccountId);
    }

    [Fact]
    public void Secured_WhenNoMatchingBatches_ReturnsEmptyQueryable()
    {
        // Arrange
        var accounts = new List<Account>
        {
            Account.Create(_otherIdentityId, "Other User Account", AccountTypeEnum.Checking)
        };
        accounts[0].GetType().GetProperty("Id")!.SetValue(accounts[0], _otherAccountId);

        var batches = new List<TransactionImportBatch>
        {
            TransactionImportBatch.Create(_otherAccountId),
            TransactionImportBatch.Create(_otherAccountId)
        };

        Mock<DbSet<Account>> mockAccountDbSet = accounts.BuildMockDbSet();
        Mock<DbSet<TransactionImportBatch>> mockBatchDbSet = batches.BuildMockDbSet();
        _mockContext.Setup(c => c.Set<Account>()).Returns(mockAccountDbSet.Object);
        _mockContext.Setup(c => c.Set<TransactionImportBatch>()).Returns(mockBatchDbSet.Object);

        // Act
        IQueryable<TransactionImportBatch> result = _batchLock.Secured(_testIdentityId);
        var resultList = result.ToList();

        // Assert
        resultList.ShouldNotBeNull();
        resultList.ShouldBeEmpty();
    }

    [Fact]
    public void Secured_WhenAllBatchesMatch_ReturnsAllBatches()
    {
        // Arrange
        var accounts = new List<Account>
        {
            Account.Create(_testIdentityId, "User Account", AccountTypeEnum.Checking)
        };
        accounts[0].GetType().GetProperty("Id")!.SetValue(accounts[0], _testAccountId);

        var batches = new List<TransactionImportBatch>
        {
            TransactionImportBatch.Create(_testAccountId),
            TransactionImportBatch.Create(_testAccountId),
            TransactionImportBatch.Create(_testAccountId)
        };

        Mock<DbSet<Account>> mockAccountDbSet = accounts.BuildMockDbSet();
        Mock<DbSet<TransactionImportBatch>> mockBatchDbSet = batches.BuildMockDbSet();
        _mockContext.Setup(c => c.Set<Account>()).Returns(mockAccountDbSet.Object);
        _mockContext.Setup(c => c.Set<TransactionImportBatch>()).Returns(mockBatchDbSet.Object);

        // Act
        IQueryable<TransactionImportBatch> result = _batchLock.Secured(_testIdentityId);
        var resultList = result.ToList();

        // Assert
        resultList.ShouldNotBeNull();
        resultList.Count.ShouldBe(3);
        resultList.ShouldAllBe(b => b.AccountId == _testAccountId);
    }

    [Fact]
    public void Secured_ReturnsQueryableThatSupportsLinq()
    {
        // Arrange
        var accounts = new List<Account>
        {
            Account.Create(_testIdentityId, "User Account", AccountTypeEnum.Checking)
        };
        accounts[0].GetType().GetProperty("Id")!.SetValue(accounts[0], _testAccountId);

        var batches = new List<TransactionImportBatch>
        {
            TransactionImportBatch.Create(_testAccountId),
            TransactionImportBatch.Create(_testAccountId),
            TransactionImportBatch.Create(_testAccountId),
            TransactionImportBatch.Create(_otherAccountId)
        };

        Mock<DbSet<Account>> mockAccountDbSet = accounts.BuildMockDbSet();
        Mock<DbSet<TransactionImportBatch>> mockBatchDbSet = batches.BuildMockDbSet();
        _mockContext.Setup(c => c.Set<Account>()).Returns(mockAccountDbSet.Object);
        _mockContext.Setup(c => c.Set<TransactionImportBatch>()).Returns(mockBatchDbSet.Object);

        // Act
        IQueryable<TransactionImportBatch> secured = _batchLock.Secured(_testIdentityId);
        var result = secured
            .Where(b => b.AccountId == _testAccountId)
            .Take(2)
            .ToList();

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
        result.ShouldAllBe(b => b.AccountId == _testAccountId);
    }

    [Fact]
    public void Secured_WithEmptyIdentityId_ReturnsNoItems()
    {
        // Arrange
        var accounts = new List<Account>
        {
            Account.Create(_testIdentityId, "User Account", AccountTypeEnum.Checking)
        };
        accounts[0].GetType().GetProperty("Id")!.SetValue(accounts[0], _testAccountId);

        var batches = new List<TransactionImportBatch>
        {
            TransactionImportBatch.Create(_testAccountId)
        };

        Mock<DbSet<Account>> mockAccountDbSet = accounts.BuildMockDbSet();
        Mock<DbSet<TransactionImportBatch>> mockBatchDbSet = batches.BuildMockDbSet();
        _mockContext.Setup(c => c.Set<Account>()).Returns(mockAccountDbSet.Object);
        _mockContext.Setup(c => c.Set<TransactionImportBatch>()).Returns(mockBatchDbSet.Object);

        // Act
        IQueryable<TransactionImportBatch> result = _batchLock.Secured(Guid.Empty);
        var resultList = result.ToList();

        // Assert
        resultList.ShouldNotBeNull();
        resultList.ShouldBeEmpty();
    }

    #endregion
}
