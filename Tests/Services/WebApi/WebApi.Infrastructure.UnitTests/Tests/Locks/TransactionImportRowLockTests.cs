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

public class TransactionImportRowLockTests
{
    private readonly Mock<BudgetContext> _mockContext;
    private readonly TransactionImportRowLock _rowLock;
    private readonly Guid _testIdentityId;
    private readonly Guid _otherIdentityId;
    private readonly Guid _testAccountId;
    private readonly Guid _otherAccountId;
    private readonly Guid _testBatchId;
    private readonly Guid _otherBatchId;

    public TransactionImportRowLockTests()
    {
        _mockContext = new Mock<BudgetContext>();
        _rowLock = new TransactionImportRowLock(_mockContext.Object);
        _testIdentityId = Guid.NewGuid();
        _otherIdentityId = Guid.NewGuid();
        _testAccountId = Guid.NewGuid();
        _otherAccountId = Guid.NewGuid();
        _testBatchId = Guid.NewGuid();
        _otherBatchId = Guid.NewGuid();
    }

    #region HasAccess Tests

    [Fact]
    public async Task HasAccess_WhenBatchAccountBelongsToUser_ReturnsTrue()
    {
        // Arrange
        var row = TransactionImportRow.Create(
            _testBatchId,
            1,
            "{\"date\":\"2025-01-15\",\"amount\":\"100.00\"}",
            new DateOnly(2025, 1, 15),
            "100.00",
            "Test Transaction",
            "DEBIT",
            null,
            null,
            null,
            null,
            "FIT123");

        var accounts = new List<Account>
        {
            Account.Create(_testIdentityId, "Test Account", AccountTypeEnum.Checking)
        };
        accounts[0].GetType().GetProperty("Id")!.SetValue(accounts[0], _testAccountId);

        var batches = new List<TransactionImportBatch>
        {
            TransactionImportBatch.Create(_testAccountId)
        };
        batches[0].GetType().GetProperty("Id")!.SetValue(batches[0], _testBatchId);

        Mock<DbSet<Account>> mockAccountDbSet = accounts.BuildMockDbSet();
        Mock<DbSet<TransactionImportBatch>> mockBatchDbSet = batches.BuildMockDbSet();
        _mockContext.Setup(c => c.Set<Account>()).Returns(mockAccountDbSet.Object);
        _mockContext.Setup(c => c.Set<TransactionImportBatch>()).Returns(mockBatchDbSet.Object);
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        bool result = await _rowLock.HasAccess(
            row,
            _testIdentityId,
            RepositoryOperationEnum.Read,
            cancellationToken);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public async Task HasAccess_WhenBatchAccountBelongsToOtherUser_ReturnsFalse()
    {
        // Arrange
        var row = TransactionImportRow.Create(
            _otherBatchId,
            1,
            "{\"date\":\"2025-01-15\",\"amount\":\"100.00\"}",
            new DateOnly(2025, 1, 15),
            "100.00",
            "Test Transaction",
            "DEBIT",
            null,
            null,
            null,
            null,
            "FIT123");

        var accounts = new List<Account>
        {
            Account.Create(_otherIdentityId, "Other Account", AccountTypeEnum.Checking)
        };
        accounts[0].GetType().GetProperty("Id")!.SetValue(accounts[0], _otherAccountId);

        var batches = new List<TransactionImportBatch>
        {
            TransactionImportBatch.Create(_otherAccountId)
        };
        batches[0].GetType().GetProperty("Id")!.SetValue(batches[0], _otherBatchId);

        Mock<DbSet<Account>> mockAccountDbSet = accounts.BuildMockDbSet();
        Mock<DbSet<TransactionImportBatch>> mockBatchDbSet = batches.BuildMockDbSet();
        _mockContext.Setup(c => c.Set<Account>()).Returns(mockAccountDbSet.Object);
        _mockContext.Setup(c => c.Set<TransactionImportBatch>()).Returns(mockBatchDbSet.Object);
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        bool result = await _rowLock.HasAccess(
            row,
            _testIdentityId,
            RepositoryOperationEnum.Read,
            cancellationToken);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public async Task HasAccess_WhenImportBatchIdIsEmpty_ReturnsFalse()
    {
        // Arrange
        var row = new TransactionImportRow();
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        bool result = await _rowLock.HasAccess(
            row,
            _testIdentityId,
            RepositoryOperationEnum.Read,
            cancellationToken);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public async Task HasAccess_WhenBatchDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var row = TransactionImportRow.Create(
            _testBatchId,
            1,
            "{\"date\":\"2025-01-15\",\"amount\":\"100.00\"}",
            new DateOnly(2025, 1, 15),
            "100.00",
            "Test Transaction");

        var accounts = new List<Account>();
        var batches = new List<TransactionImportBatch>();

        Mock<DbSet<Account>> mockAccountDbSet = accounts.BuildMockDbSet();
        Mock<DbSet<TransactionImportBatch>> mockBatchDbSet = batches.BuildMockDbSet();
        _mockContext.Setup(c => c.Set<Account>()).Returns(mockAccountDbSet.Object);
        _mockContext.Setup(c => c.Set<TransactionImportBatch>()).Returns(mockBatchDbSet.Object);
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        bool result = await _rowLock.HasAccess(
            row,
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
        var row = TransactionImportRow.Create(
            _testBatchId,
            1,
            "{\"date\":\"2025-01-15\",\"amount\":\"100.00\"}",
            new DateOnly(2025, 1, 15),
            "100.00",
            "Test Transaction");

        var accounts = new List<Account>
        {
            Account.Create(_testIdentityId, "Test Account", AccountTypeEnum.Savings)
        };
        accounts[0].GetType().GetProperty("Id")!.SetValue(accounts[0], _testAccountId);

        var batches = new List<TransactionImportBatch>
        {
            TransactionImportBatch.Create(_testAccountId)
        };
        batches[0].GetType().GetProperty("Id")!.SetValue(batches[0], _testBatchId);

        Mock<DbSet<Account>> mockAccountDbSet = accounts.BuildMockDbSet();
        Mock<DbSet<TransactionImportBatch>> mockBatchDbSet = batches.BuildMockDbSet();
        _mockContext.Setup(c => c.Set<Account>()).Returns(mockAccountDbSet.Object);
        _mockContext.Setup(c => c.Set<TransactionImportBatch>()).Returns(mockBatchDbSet.Object);
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        bool result = await _rowLock.HasAccess(
            row,
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
        var row = TransactionImportRow.Create(
            _otherBatchId,
            1,
            "{\"date\":\"2025-01-15\",\"amount\":\"100.00\"}",
            new DateOnly(2025, 1, 15),
            "100.00",
            "Test Transaction");

        var accounts = new List<Account>
        {
            Account.Create(_otherIdentityId, "Other Account", AccountTypeEnum.CreditCard)
        };
        accounts[0].GetType().GetProperty("Id")!.SetValue(accounts[0], _otherAccountId);

        var batches = new List<TransactionImportBatch>
        {
            TransactionImportBatch.Create(_otherAccountId)
        };
        batches[0].GetType().GetProperty("Id")!.SetValue(batches[0], _otherBatchId);

        Mock<DbSet<Account>> mockAccountDbSet = accounts.BuildMockDbSet();
        Mock<DbSet<TransactionImportBatch>> mockBatchDbSet = batches.BuildMockDbSet();
        _mockContext.Setup(c => c.Set<Account>()).Returns(mockAccountDbSet.Object);
        _mockContext.Setup(c => c.Set<TransactionImportBatch>()).Returns(mockBatchDbSet.Object);
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        bool result = await _rowLock.HasAccess(
            row,
            _testIdentityId,
            operation,
            cancellationToken);

        // Assert
        result.ShouldBeFalse();
    }

    #endregion

    #region Secured Tests

    [Fact]
    public void Secured_ReturnsOnlyRowsForUserBatches()
    {
        // Arrange
        var accounts = new List<Account>
        {
            Account.Create(_testIdentityId, "User Account", AccountTypeEnum.Checking),
            Account.Create(_otherIdentityId, "Other User Account", AccountTypeEnum.Cash)
        };
        accounts[0].GetType().GetProperty("Id")!.SetValue(accounts[0], _testAccountId);
        accounts[1].GetType().GetProperty("Id")!.SetValue(accounts[1], _otherAccountId);

        var batches = new List<TransactionImportBatch>
        {
            TransactionImportBatch.Create(_testAccountId),
            TransactionImportBatch.Create(_otherAccountId)
        };
        batches[0].GetType().GetProperty("Id")!.SetValue(batches[0], _testBatchId);
        batches[1].GetType().GetProperty("Id")!.SetValue(batches[1], _otherBatchId);

        var rows = new List<TransactionImportRow>
        {
            TransactionImportRow.Create(_testBatchId, 1, "{\"desc\":\"Row 1\"}", new DateOnly(2025, 1, 15), "100.00", "Row 1"),
            TransactionImportRow.Create(_testBatchId, 2, "{\"desc\":\"Row 2\"}", new DateOnly(2025, 1, 16), "200.00", "Row 2"),
            TransactionImportRow.Create(_testBatchId, 3, "{\"desc\":\"Row 3\"}", new DateOnly(2025, 1, 17), "300.00", "Row 3"),
            TransactionImportRow.Create(_otherBatchId, 1, "{\"desc\":\"Row 4\"}", new DateOnly(2025, 1, 18), "400.00", "Row 4")
        };

        Mock<DbSet<Account>> mockAccountDbSet = accounts.BuildMockDbSet();
        Mock<DbSet<TransactionImportBatch>> mockBatchDbSet = batches.BuildMockDbSet();
        Mock<DbSet<TransactionImportRow>> mockRowDbSet = rows.BuildMockDbSet();
        _mockContext.Setup(c => c.Set<Account>()).Returns(mockAccountDbSet.Object);
        _mockContext.Setup(c => c.Set<TransactionImportBatch>()).Returns(mockBatchDbSet.Object);
        _mockContext.Setup(c => c.Set<TransactionImportRow>()).Returns(mockRowDbSet.Object);

        // Act
        IQueryable<TransactionImportRow> result = _rowLock.Secured(_testIdentityId);
        var resultList = result.ToList();

        // Assert
        resultList.ShouldNotBeNull();
        resultList.Count.ShouldBe(3);
        resultList.ShouldAllBe(r => r.ImportBatchId == _testBatchId);
    }

    [Fact]
    public void Secured_WhenNoMatchingRows_ReturnsEmptyQueryable()
    {
        // Arrange
        var accounts = new List<Account>
        {
            Account.Create(_otherIdentityId, "Other User Account", AccountTypeEnum.Checking)
        };
        accounts[0].GetType().GetProperty("Id")!.SetValue(accounts[0], _otherAccountId);

        var batches = new List<TransactionImportBatch>
        {
            TransactionImportBatch.Create(_otherAccountId)
        };
        batches[0].GetType().GetProperty("Id")!.SetValue(batches[0], _otherBatchId);

        var rows = new List<TransactionImportRow>
        {
            TransactionImportRow.Create(_otherBatchId, 1, "{\"desc\":\"Row 1\"}", new DateOnly(2025, 1, 15), "100.00", "Row 1")
        };

        Mock<DbSet<Account>> mockAccountDbSet = accounts.BuildMockDbSet();
        Mock<DbSet<TransactionImportBatch>> mockBatchDbSet = batches.BuildMockDbSet();
        Mock<DbSet<TransactionImportRow>> mockRowDbSet = rows.BuildMockDbSet();
        _mockContext.Setup(c => c.Set<Account>()).Returns(mockAccountDbSet.Object);
        _mockContext.Setup(c => c.Set<TransactionImportBatch>()).Returns(mockBatchDbSet.Object);
        _mockContext.Setup(c => c.Set<TransactionImportRow>()).Returns(mockRowDbSet.Object);

        // Act
        IQueryable<TransactionImportRow> result = _rowLock.Secured(_testIdentityId);
        var resultList = result.ToList();

        // Assert
        resultList.ShouldNotBeNull();
        resultList.ShouldBeEmpty();
    }

    [Fact]
    public void Secured_WhenAllRowsMatch_ReturnsAllRows()
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
        batches[0].GetType().GetProperty("Id")!.SetValue(batches[0], _testBatchId);

        var rows = new List<TransactionImportRow>
        {
            TransactionImportRow.Create(_testBatchId, 1, "{\"desc\":\"Row 1\"}", new DateOnly(2025, 1, 15), "100.00", "Row 1"),
            TransactionImportRow.Create(_testBatchId, 2, "{\"desc\":\"Row 2\"}", new DateOnly(2025, 1, 16), "200.00", "Row 2"),
            TransactionImportRow.Create(_testBatchId, 3, "{\"desc\":\"Row 3\"}", new DateOnly(2025, 1, 17), "300.00", "Row 3")
        };

        Mock<DbSet<Account>> mockAccountDbSet = accounts.BuildMockDbSet();
        Mock<DbSet<TransactionImportBatch>> mockBatchDbSet = batches.BuildMockDbSet();
        Mock<DbSet<TransactionImportRow>> mockRowDbSet = rows.BuildMockDbSet();
        _mockContext.Setup(c => c.Set<Account>()).Returns(mockAccountDbSet.Object);
        _mockContext.Setup(c => c.Set<TransactionImportBatch>()).Returns(mockBatchDbSet.Object);
        _mockContext.Setup(c => c.Set<TransactionImportRow>()).Returns(mockRowDbSet.Object);

        // Act
        IQueryable<TransactionImportRow> result = _rowLock.Secured(_testIdentityId);
        var resultList = result.ToList();

        // Assert
        resultList.ShouldNotBeNull();
        resultList.Count.ShouldBe(3);
        resultList.ShouldAllBe(r => r.ImportBatchId == _testBatchId);
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
            TransactionImportBatch.Create(_testAccountId)
        };
        batches[0].GetType().GetProperty("Id")!.SetValue(batches[0], _testBatchId);

        var rows = new List<TransactionImportRow>
        {
            TransactionImportRow.Create(_testBatchId, 1, "{\"desc\":\"Alpha\"}", new DateOnly(2025, 1, 15), "100.00", "Alpha"),
            TransactionImportRow.Create(_testBatchId, 2, "{\"desc\":\"Beta\"}", new DateOnly(2025, 1, 16), "200.00", "Beta"),
            TransactionImportRow.Create(_testBatchId, 3, "{\"desc\":\"Gamma\"}", new DateOnly(2025, 1, 17), "300.00", "Gamma")
        };

        Mock<DbSet<Account>> mockAccountDbSet = accounts.BuildMockDbSet();
        Mock<DbSet<TransactionImportBatch>> mockBatchDbSet = batches.BuildMockDbSet();
        Mock<DbSet<TransactionImportRow>> mockRowDbSet = rows.BuildMockDbSet();
        _mockContext.Setup(c => c.Set<Account>()).Returns(mockAccountDbSet.Object);
        _mockContext.Setup(c => c.Set<TransactionImportBatch>()).Returns(mockBatchDbSet.Object);
        _mockContext.Setup(c => c.Set<TransactionImportRow>()).Returns(mockRowDbSet.Object);

        // Act
        IQueryable<TransactionImportRow> secured = _rowLock.Secured(_testIdentityId);
        var result = secured
            .Where(r => r.RawDescription!.StartsWith('B'))
            .OrderBy(r => r.RawDescription)
            .ToList();

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(1);
        result[0].RawDescription.ShouldBe("Beta");
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
        batches[0].GetType().GetProperty("Id")!.SetValue(batches[0], _testBatchId);

        var rows = new List<TransactionImportRow>
        {
            TransactionImportRow.Create(_testBatchId, 1, "{\"desc\":\"Row 1\"}", new DateOnly(2025, 1, 15), "100.00", "Row 1")
        };

        Mock<DbSet<Account>> mockAccountDbSet = accounts.BuildMockDbSet();
        Mock<DbSet<TransactionImportBatch>> mockBatchDbSet = batches.BuildMockDbSet();
        Mock<DbSet<TransactionImportRow>> mockRowDbSet = rows.BuildMockDbSet();
        _mockContext.Setup(c => c.Set<Account>()).Returns(mockAccountDbSet.Object);
        _mockContext.Setup(c => c.Set<TransactionImportBatch>()).Returns(mockBatchDbSet.Object);
        _mockContext.Setup(c => c.Set<TransactionImportRow>()).Returns(mockRowDbSet.Object);

        // Act
        IQueryable<TransactionImportRow> result = _rowLock.Secured(Guid.Empty);
        var resultList = result.ToList();

        // Assert
        resultList.ShouldNotBeNull();
        resultList.ShouldBeEmpty();
    }

    #endregion
}
