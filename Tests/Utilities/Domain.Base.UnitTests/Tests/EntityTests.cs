using Domain.Base.Contracts;
using Domain.Base.Implementation;
using Shouldly;

namespace Domain.Base.UnitTests.Tests;

public class EntityTests
{
    private class TestEntity : Entity<int>
    {
        public TestEntity(int id)
        {
            Id = id;
        }
    }

    private class TestEntityWithStringId : Entity<string>
    {
        public TestEntityWithStringId(string id)
        {
            Id = id;
        }
    }

    [Fact]
    public void Entity_WhenCreated_ShouldHaveDateCreatedSetToUtcNow()
    {
        // Arrange
        DateTimeOffset beforeCreation = DateTimeOffset.UtcNow;

        // Act
        var entity = new TestEntity(1);
        DateTimeOffset afterCreation = DateTimeOffset.UtcNow;

        // Assert
        entity.DateCreated.ShouldBeGreaterThanOrEqualTo(beforeCreation);
        entity.DateCreated.ShouldBeLessThanOrEqualTo(afterCreation);
        entity.DateCreated.Offset.ShouldBe(TimeSpan.Zero); // UTC offset should be zero
    }

    [Fact]
    public void Entity_WhenCreatedWithIntId_ShouldSetIdCorrectly()
    {
        // Arrange
        const int expectedId = 42;

        // Act
        var entity = new TestEntity(expectedId);

        // Assert
        entity.Id.ShouldBe(expectedId);
    }

    [Fact]
    public void Entity_WhenCreatedWithStringId_ShouldSetIdCorrectly()
    {
        // Arrange
        const string expectedId = "test-id-123";

        // Act
        var entity = new TestEntityWithStringId(expectedId);

        // Assert
        entity.Id.ShouldBe(expectedId);
    }

    [Fact]
    public void Entity_ShouldImplementIEntityInterface()
    {
        // Arrange & Act
        var entity = new TestEntity(1);

        // Assert
        entity.ShouldBeAssignableTo<IEntity>();
        entity.ShouldBeAssignableTo<IEntity<int>>();
    }

    [Fact]
    public void Entity_DateCreated_ShouldBeReadOnly()
    {
        // Arrange
        var entity1 = new TestEntity(1);
        DateTimeOffset firstDateCreated = entity1.DateCreated;

        // Act
        Thread.Sleep(1); // Small delay to ensure different timestamps
        var entity2 = new TestEntity(2);

        // Assert
        entity1.DateCreated.ShouldBe(firstDateCreated); // Should not change
        entity2.DateCreated.ShouldBeGreaterThan(firstDateCreated);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    public void Entity_WithVariousIntIds_ShouldSetCorrectly(int id)
    {
        // Act
        var entity = new TestEntity(id);

        // Assert
        entity.Id.ShouldBe(id);
    }

    [Theory]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("test-123")]
    [InlineData("very-long-string-identifier-with-special-characters-!@#$%")]
    public void Entity_WithVariousStringIds_ShouldSetCorrectly(string id)
    {
        // Act
        var entity = new TestEntityWithStringId(id);

        // Assert
        entity.Id.ShouldBe(id);
    }
}
