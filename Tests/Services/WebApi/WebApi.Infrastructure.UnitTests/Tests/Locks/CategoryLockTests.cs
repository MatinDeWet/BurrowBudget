using Domain.Core.Entities;
using Microsoft.EntityFrameworkCore;
using MockQueryable;
using MockQueryable.Moq;
using Moq;
using Repository.Core.Enums;
using Shouldly;
using WebApi.Infrastructure.Data.Contexts;
using WebApi.Infrastructure.Locks;

namespace WebApi.Infrastructure.UnitTests.Tests.Locks;

public class CategoryLockTests
{
    private readonly Mock<BudgetContext> _mockContext;
    private readonly CategoryLock _categoryLock;
    private readonly Guid _testIdentityId;
    private readonly Guid _otherIdentityId;
    private readonly Guid _categoryGroupId;

    public CategoryLockTests()
    {
        _mockContext = new Mock<BudgetContext>();
        _categoryLock = new CategoryLock(_mockContext.Object);
        _testIdentityId = Guid.NewGuid();
        _otherIdentityId = Guid.NewGuid();
        _categoryGroupId = Guid.NewGuid();
    }

    #region HasAccess Tests

    [Fact]
    public async Task HasAccess_WhenUserIdMatchesIdentityId_ReturnsTrue()
    {
        // Arrange
        var category = Category.Create(
            userId: _testIdentityId,
            name: "Test Category",
            categoryGroupId: _categoryGroupId);
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        bool result = await _categoryLock.HasAccess(
            category,
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
        var category = Category.Create(
            userId: _otherIdentityId,
            name: "Test Category",
            categoryGroupId: _categoryGroupId);
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        bool result = await _categoryLock.HasAccess(
            category,
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
        var category = new Category();
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        bool result = await _categoryLock.HasAccess(
            category,
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
        var category = Category.Create(
            userId: _testIdentityId,
            name: "Test Category",
            categoryGroupId: _categoryGroupId);
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        bool result = await _categoryLock.HasAccess(
            category,
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
        var category = Category.Create(
            userId: _otherIdentityId,
            name: "Test Category",
            categoryGroupId: _categoryGroupId);
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        bool result = await _categoryLock.HasAccess(
            category,
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
        var categories = new List<Category>
        {
            Category.Create(_testIdentityId, "User Category 1", _categoryGroupId),
            Category.Create(_testIdentityId, "User Category 2", _categoryGroupId),
            Category.Create(_otherIdentityId, "Other User Category 1", _categoryGroupId),
            Category.Create(_otherIdentityId, "Other User Category 2", _categoryGroupId)
        };

        Mock<DbSet<Category>> mockDbSet = categories.BuildMockDbSet();
        _mockContext.Setup(c => c.Set<Category>()).Returns(mockDbSet.Object);

        // Act
        IQueryable<Category> result = _categoryLock.Secured(_testIdentityId);
        var resultList = result.ToList();

        // Assert
        resultList.ShouldNotBeNull();
        resultList.Count.ShouldBe(2);
        resultList.ShouldAllBe(c => c.UserId == _testIdentityId);
    }

    [Fact]
    public void Secured_WhenNoMatchingItems_ReturnsEmptyQueryable()
    {
        // Arrange
        var categories = new List<Category>
        {
            Category.Create(_otherIdentityId, "Other User Category 1", _categoryGroupId),
            Category.Create(_otherIdentityId, "Other User Category 2", _categoryGroupId)
        };

        Mock<DbSet<Category>> mockDbSet = categories.BuildMockDbSet();
        _mockContext.Setup(c => c.Set<Category>()).Returns(mockDbSet.Object);

        // Act
        IQueryable<Category> result = _categoryLock.Secured(_testIdentityId);
        var resultList = result.ToList();

        // Assert
        resultList.ShouldNotBeNull();
        resultList.ShouldBeEmpty();
    }

    [Fact]
    public void Secured_WhenAllItemsMatch_ReturnsAllItems()
    {
        // Arrange
        var categories = new List<Category>
        {
            Category.Create(_testIdentityId, "User Category 1", _categoryGroupId),
            Category.Create(_testIdentityId, "User Category 2", _categoryGroupId),
            Category.Create(_testIdentityId, "User Category 3", _categoryGroupId)
        };

        Mock<DbSet<Category>> mockDbSet = categories.BuildMockDbSet();
        _mockContext.Setup(c => c.Set<Category>()).Returns(mockDbSet.Object);

        // Act
        IQueryable<Category> result = _categoryLock.Secured(_testIdentityId);
        var resultList = result.ToList();

        // Assert
        resultList.ShouldNotBeNull();
        resultList.Count.ShouldBe(3);
        resultList.ShouldAllBe(c => c.UserId == _testIdentityId);
    }

    [Fact]
    public void Secured_ReturnsQueryableThatSupportsLinq()
    {
        // Arrange
        var categories = new List<Category>
        {
            Category.Create(_testIdentityId, "Alpha Category", _categoryGroupId),
            Category.Create(_testIdentityId, "Beta Category", _categoryGroupId),
            Category.Create(_testIdentityId, "Gamma Category", _categoryGroupId),
            Category.Create(_otherIdentityId, "Other Category", _categoryGroupId)
        };

        Mock<DbSet<Category>> mockDbSet = categories.BuildMockDbSet();
        _mockContext.Setup(c => c.Set<Category>()).Returns(mockDbSet.Object);

        // Act
        IQueryable<Category> secured = _categoryLock.Secured(_testIdentityId);
        var result = secured
            .Where(c => c.Name.StartsWith('B'))
            .OrderBy(c => c.Name)
            .ToList();

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(1);
        result[0].Name.ShouldBe("Beta Category");
    }

    [Fact]
    public void Secured_WithEmptyIdentityId_ReturnsNoItems()
    {
        // Arrange
        var categories = new List<Category>
        {
            Category.Create(_testIdentityId, "User Category 1", _categoryGroupId),
            Category.Create(_otherIdentityId, "Other User Category 1", _categoryGroupId)
        };

        Mock<DbSet<Category>> mockDbSet = categories.BuildMockDbSet();
        _mockContext.Setup(c => c.Set<Category>()).Returns(mockDbSet.Object);

        // Act
        IQueryable<Category> result = _categoryLock.Secured(Guid.Empty);
        var resultList = result.ToList();

        // Assert
        resultList.ShouldNotBeNull();
        resultList.ShouldBeEmpty();
    }

    #endregion
}
