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

public class TransactionImportFileLockTests
{
    private readonly Mock<BudgetContext> _mockContext;
    private readonly TransactionImportFileLock _fileLock;
    private readonly Guid _testIdentityId;
    private readonly Guid _otherIdentityId;
    private readonly Guid _testAccountId;
    private readonly Guid _otherAccountId;
    private readonly Guid _testBatchId;
    private readonly Guid _otherBatchId;

    public TransactionImportFileLockTests()
    {
        _mockContext = new Mock<BudgetContext>();
        _fileLock = new TransactionImportFileLock(_mockContext.Object);
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
        var file = TransactionImportFile.Create(
            _testBatchId,
            "test.csv",
            "test",
            ".csv",
            "text/csv",
            "imports",
            "blob-name",
            1024);

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
        bool result = await _fileLock.HasAccess(
            file,
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
        var file = TransactionImportFile.Create(
            _otherBatchId,
            "test.csv",
            "test",
            ".csv",
            "text/csv",
            "imports",
            "blob-name",
            1024);

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
        bool result = await _fileLock.HasAccess(
            file,
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
        var file = new TransactionImportFile();
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        bool result = await _fileLock.HasAccess(
            file,
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
        var file = TransactionImportFile.Create(
            _testBatchId,
            "test.csv",
            "test",
            ".csv",
            "text/csv",
            "imports",
            "blob-name",
            1024);

        var accounts = new List<Account>();
        var batches = new List<TransactionImportBatch>();

        Mock<DbSet<Account>> mockAccountDbSet = accounts.BuildMockDbSet();
        Mock<DbSet<TransactionImportBatch>> mockBatchDbSet = batches.BuildMockDbSet();
        _mockContext.Setup(c => c.Set<Account>()).Returns(mockAccountDbSet.Object);
        _mockContext.Setup(c => c.Set<TransactionImportBatch>()).Returns(mockBatchDbSet.Object);
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        bool result = await _fileLock.HasAccess(
            file,
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
        var file = TransactionImportFile.Create(
            _testBatchId,
            "test.csv",
            "test",
            ".csv",
            "text/csv",
            "imports",
            "blob-name",
            1024);

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
        bool result = await _fileLock.HasAccess(
            file,
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
        var file = TransactionImportFile.Create(
            _otherBatchId,
            "test.csv",
            "test",
            ".csv",
            "text/csv",
            "imports",
            "blob-name",
            1024);

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
        bool result = await _fileLock.HasAccess(
            file,
            _testIdentityId,
            operation,
            cancellationToken);

        // Assert
        result.ShouldBeFalse();
    }

    #endregion

    #region Secured Tests

    [Fact]
    public void Secured_ReturnsOnlyFilesForUserBatches()
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

        var files = new List<TransactionImportFile>
        {
            TransactionImportFile.Create(_testBatchId, "file1.csv", "file1", ".csv", "text/csv", "imports", "blob1", 1024),
            TransactionImportFile.Create(_testBatchId, "file2.csv", "file2", ".csv", "text/csv", "imports", "blob2", 2048),
            TransactionImportFile.Create(_otherBatchId, "file3.csv", "file3", ".csv", "text/csv", "imports", "blob3", 3072)
        };

        Mock<DbSet<Account>> mockAccountDbSet = accounts.BuildMockDbSet();
        Mock<DbSet<TransactionImportBatch>> mockBatchDbSet = batches.BuildMockDbSet();
        Mock<DbSet<TransactionImportFile>> mockFileDbSet = files.BuildMockDbSet();
        _mockContext.Setup(c => c.Set<Account>()).Returns(mockAccountDbSet.Object);
        _mockContext.Setup(c => c.Set<TransactionImportBatch>()).Returns(mockBatchDbSet.Object);
        _mockContext.Setup(c => c.Set<TransactionImportFile>()).Returns(mockFileDbSet.Object);

        // Act
        IQueryable<TransactionImportFile> result = _fileLock.Secured(_testIdentityId);
        var resultList = result.ToList();

        // Assert
        resultList.ShouldNotBeNull();
        resultList.Count.ShouldBe(2);
        resultList.ShouldAllBe(f => f.ImportBatchId == _testBatchId);
    }

    [Fact]
    public void Secured_WhenNoMatchingFiles_ReturnsEmptyQueryable()
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

        var files = new List<TransactionImportFile>
        {
            TransactionImportFile.Create(_otherBatchId, "file1.csv", "file1", ".csv", "text/csv", "imports", "blob1", 1024)
        };

        Mock<DbSet<Account>> mockAccountDbSet = accounts.BuildMockDbSet();
        Mock<DbSet<TransactionImportBatch>> mockBatchDbSet = batches.BuildMockDbSet();
        Mock<DbSet<TransactionImportFile>> mockFileDbSet = files.BuildMockDbSet();
        _mockContext.Setup(c => c.Set<Account>()).Returns(mockAccountDbSet.Object);
        _mockContext.Setup(c => c.Set<TransactionImportBatch>()).Returns(mockBatchDbSet.Object);
        _mockContext.Setup(c => c.Set<TransactionImportFile>()).Returns(mockFileDbSet.Object);

        // Act
        IQueryable<TransactionImportFile> result = _fileLock.Secured(_testIdentityId);
        var resultList = result.ToList();

        // Assert
        resultList.ShouldNotBeNull();
        resultList.ShouldBeEmpty();
    }

    [Fact]
    public void Secured_WhenAllFilesMatch_ReturnsAllFiles()
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

        var files = new List<TransactionImportFile>
        {
            TransactionImportFile.Create(_testBatchId, "file1.csv", "file1", ".csv", "text/csv", "imports", "blob1", 1024),
            TransactionImportFile.Create(_testBatchId, "file2.csv", "file2", ".csv", "text/csv", "imports", "blob2", 2048),
            TransactionImportFile.Create(_testBatchId, "file3.csv", "file3", ".csv", "text/csv", "imports", "blob3", 3072)
        };

        Mock<DbSet<Account>> mockAccountDbSet = accounts.BuildMockDbSet();
        Mock<DbSet<TransactionImportBatch>> mockBatchDbSet = batches.BuildMockDbSet();
        Mock<DbSet<TransactionImportFile>> mockFileDbSet = files.BuildMockDbSet();
        _mockContext.Setup(c => c.Set<Account>()).Returns(mockAccountDbSet.Object);
        _mockContext.Setup(c => c.Set<TransactionImportBatch>()).Returns(mockBatchDbSet.Object);
        _mockContext.Setup(c => c.Set<TransactionImportFile>()).Returns(mockFileDbSet.Object);

        // Act
        IQueryable<TransactionImportFile> result = _fileLock.Secured(_testIdentityId);
        var resultList = result.ToList();

        // Assert
        resultList.ShouldNotBeNull();
        resultList.Count.ShouldBe(3);
        resultList.ShouldAllBe(f => f.ImportBatchId == _testBatchId);
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

        var files = new List<TransactionImportFile>
        {
            TransactionImportFile.Create(_testBatchId, "alpha.csv", "alpha", ".csv", "text/csv", "imports", "blob1", 1024),
            TransactionImportFile.Create(_testBatchId, "beta.csv", "beta", ".csv", "text/csv", "imports", "blob2", 2048),
            TransactionImportFile.Create(_testBatchId, "gamma.csv", "gamma", ".csv", "text/csv", "imports", "blob3", 3072)
        };

        Mock<DbSet<Account>> mockAccountDbSet = accounts.BuildMockDbSet();
        Mock<DbSet<TransactionImportBatch>> mockBatchDbSet = batches.BuildMockDbSet();
        Mock<DbSet<TransactionImportFile>> mockFileDbSet = files.BuildMockDbSet();
        _mockContext.Setup(c => c.Set<Account>()).Returns(mockAccountDbSet.Object);
        _mockContext.Setup(c => c.Set<TransactionImportBatch>()).Returns(mockBatchDbSet.Object);
        _mockContext.Setup(c => c.Set<TransactionImportFile>()).Returns(mockFileDbSet.Object);

        // Act
        IQueryable<TransactionImportFile> secured = _fileLock.Secured(_testIdentityId);
        var result = secured
            .Where(f => f.FileName.StartsWith('b'))
            .OrderBy(f => f.FileName)
            .ToList();

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(1);
        result[0].FileName.ShouldBe("beta");
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

        var files = new List<TransactionImportFile>
        {
            TransactionImportFile.Create(_testBatchId, "file1.csv", "file1", ".csv", "text/csv", "imports", "blob1", 1024)
        };

        Mock<DbSet<Account>> mockAccountDbSet = accounts.BuildMockDbSet();
        Mock<DbSet<TransactionImportBatch>> mockBatchDbSet = batches.BuildMockDbSet();
        Mock<DbSet<TransactionImportFile>> mockFileDbSet = files.BuildMockDbSet();
        _mockContext.Setup(c => c.Set<Account>()).Returns(mockAccountDbSet.Object);
        _mockContext.Setup(c => c.Set<TransactionImportBatch>()).Returns(mockBatchDbSet.Object);
        _mockContext.Setup(c => c.Set<TransactionImportFile>()).Returns(mockFileDbSet.Object);

        // Act
        IQueryable<TransactionImportFile> result = _fileLock.Secured(Guid.Empty);
        var resultList = result.ToList();

        // Assert
        resultList.ShouldNotBeNull();
        resultList.ShouldBeEmpty();
    }

    #endregion
}
