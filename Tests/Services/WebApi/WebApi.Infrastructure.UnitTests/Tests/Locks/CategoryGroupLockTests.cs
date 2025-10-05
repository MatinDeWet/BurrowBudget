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

public class CategoryGroupLockTests
{
    private readonly Mock<BudgetContext> _mockContext;
    private readonly CategoryGroupLock _categoryGroupLock;
    private readonly Guid _testIdentityId;
    private readonly Guid _otherIdentityId;

    public CategoryGroupLockTests()
    {
        _mockContext = new Mock<BudgetContext>();
        _categoryGroupLock = new CategoryGroupLock(_mockContext.Object);
        _testIdentityId = Guid.NewGuid();
        _otherIdentityId = Guid.NewGuid();
    }

    #region HasAccess Tests

    [Fact]
    public async Task HasAccess_WhenUserIdMatchesIdentityId_ReturnsTrue()
    {
        // Arrange
        var categoryGroup = CategoryGroup.Create(
            userId: _testIdentityId,
            name: "Test Category Group");
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        bool result = await _categoryGroupLock.HasAccess(
            categoryGroup,
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
        var categoryGroup = CategoryGroup.Create(
            userId: _otherIdentityId,
            name: "Test Category Group");
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        bool result = await _categoryGroupLock.HasAccess(
            categoryGroup,
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
        var categoryGroup = new CategoryGroup();
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        bool result = await _categoryGroupLock.HasAccess(
            categoryGroup,
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
        var categoryGroup = CategoryGroup.Create(
            userId: _testIdentityId,
            name: "Test Category Group");
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        bool result = await _categoryGroupLock.HasAccess(
            categoryGroup,
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
        var categoryGroup = CategoryGroup.Create(
            userId: _otherIdentityId,
            name: "Test Category Group");
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        bool result = await _categoryGroupLock.HasAccess(
            categoryGroup,
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
        var categoryGroups = new List<CategoryGroup>
        {
            CategoryGroup.Create(_testIdentityId, "User Group 1"),
            CategoryGroup.Create(_testIdentityId, "User Group 2"),
            CategoryGroup.Create(_otherIdentityId, "Other User Group 1"),
            CategoryGroup.Create(_otherIdentityId, "Other User Group 2")
        };

        Mock<DbSet<CategoryGroup>> mockDbSet = categoryGroups.BuildMockDbSet();
        _mockContext.Setup(c => c.Set<CategoryGroup>()).Returns(mockDbSet.Object);

        // Act
        IQueryable<CategoryGroup> result = _categoryGroupLock.Secured(_testIdentityId);
        var resultList = result.ToList();

        // Assert
        resultList.ShouldNotBeNull();
        resultList.Count.ShouldBe(2);
        resultList.ShouldAllBe(cg => cg.UserId == _testIdentityId);
    }

    [Fact]
    public void Secured_WhenNoMatchingItems_ReturnsEmptyQueryable()
    {
        // Arrange
        var categoryGroups = new List<CategoryGroup>
        {
            CategoryGroup.Create(_otherIdentityId, "Other User Group 1"),
            CategoryGroup.Create(_otherIdentityId, "Other User Group 2")
        };

        Mock<DbSet<CategoryGroup>> mockDbSet = categoryGroups.BuildMockDbSet();
        _mockContext.Setup(c => c.Set<CategoryGroup>()).Returns(mockDbSet.Object);

        // Act
        IQueryable<CategoryGroup> result = _categoryGroupLock.Secured(_testIdentityId);
        var resultList = result.ToList();

        // Assert
        resultList.ShouldNotBeNull();
        resultList.ShouldBeEmpty();
    }

    [Fact]
    public void Secured_WhenAllItemsMatch_ReturnsAllItems()
    {
        // Arrange
        var categoryGroups = new List<CategoryGroup>
        {
            CategoryGroup.Create(_testIdentityId, "User Group 1"),
            CategoryGroup.Create(_testIdentityId, "User Group 2"),
            CategoryGroup.Create(_testIdentityId, "User Group 3")
        };

        Mock<DbSet<CategoryGroup>> mockDbSet = categoryGroups.BuildMockDbSet();
        _mockContext.Setup(c => c.Set<CategoryGroup>()).Returns(mockDbSet.Object);

        // Act
        IQueryable<CategoryGroup> result = _categoryGroupLock.Secured(_testIdentityId);
        var resultList = result.ToList();

        // Assert
        resultList.ShouldNotBeNull();
        resultList.Count.ShouldBe(3);
        resultList.ShouldAllBe(cg => cg.UserId == _testIdentityId);
    }

    [Fact]
    public void Secured_ReturnsQueryableThatSupportsLinq()
    {
        // Arrange
        var categoryGroups = new List<CategoryGroup>
        {
            CategoryGroup.Create(_testIdentityId, "Alpha Group"),
            CategoryGroup.Create(_testIdentityId, "Beta Group"),
            CategoryGroup.Create(_testIdentityId, "Gamma Group"),
            CategoryGroup.Create(_otherIdentityId, "Other Group")
        };

        Mock<DbSet<CategoryGroup>> mockDbSet = categoryGroups.BuildMockDbSet();
        _mockContext.Setup(c => c.Set<CategoryGroup>()).Returns(mockDbSet.Object);

        // Act
        IQueryable<CategoryGroup> secured = _categoryGroupLock.Secured(_testIdentityId);
        var result = secured
            .Where(cg => cg.Name.StartsWith('B'))
            .OrderBy(cg => cg.Name)
            .ToList();

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(1);
        result[0].Name.ShouldBe("Beta Group");
    }

    [Fact]
    public void Secured_WithEmptyIdentityId_ReturnsNoItems()
    {
        // Arrange
        var categoryGroups = new List<CategoryGroup>
        {
            CategoryGroup.Create(_testIdentityId, "User Group 1"),
            CategoryGroup.Create(_otherIdentityId, "Other User Group 1")
        };

        Mock<DbSet<CategoryGroup>> mockDbSet = categoryGroups.BuildMockDbSet();
        _mockContext.Setup(c => c.Set<CategoryGroup>()).Returns(mockDbSet.Object);

        // Act
        IQueryable<CategoryGroup> result = _categoryGroupLock.Secured(Guid.Empty);
        var resultList = result.ToList();

        // Assert
        resultList.ShouldNotBeNull();
        resultList.ShouldBeEmpty();
    }

    #endregion
}
