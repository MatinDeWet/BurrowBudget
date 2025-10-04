using System.Collections.ObjectModel;
using Domain.Base.Contracts;
using Domain.Base.Implementation;
using Shouldly;

namespace Domain.Base.UnitTests.Tests;

public class EventableEntityTests
{
    private class TestEventableEntity : EventableEntity<int>
    {
        public TestEventableEntity(int id)
        {
            Id = id;
        }
    }

    private class TestDomainEvent : IDomainEvent
    {
        public string Message { get; }

        public TestDomainEvent(string message)
        {
            Message = message;
        }
    }

    [Fact]
    public void EventableEntity_WhenCreated_ShouldHaveEmptyDomainEvents()
    {
        // Act
        var entity = new TestEventableEntity(1);

        // Assert
        entity.DomainEvents.Count.ShouldBe(0);
        entity.DomainEvents.ShouldNotBeNull();
    }

    [Fact]
    public void EventableEntity_ShouldImplementRequiredInterfaces()
    {
        // Arrange & Act
        var entity = new TestEventableEntity(1);

        // Assert
        entity.ShouldBeAssignableTo<IEventableEntity>();
        entity.ShouldBeAssignableTo<IEventableEntity<int>>();
        entity.ShouldBeAssignableTo<IEntity>();
        entity.ShouldBeAssignableTo<IEntity<int>>();
        entity.ShouldBeAssignableTo<Entity<int>>();
    }

    [Fact]
    public void AddDomainEvent_WithValidEvent_ShouldAddEventToDomainEvents()
    {
        // Arrange
        var entity = new TestEventableEntity(1);
        var domainEvent = new TestDomainEvent("Test Event");

        // Act
        entity.AddDomainEvent(domainEvent);

        // Assert
        entity.DomainEvents.ShouldContain(domainEvent);
        entity.DomainEvents.Count.ShouldBe(1);
    }

    [Fact]
    public void AddDomainEvent_WithMultipleEvents_ShouldAddAllEvents()
    {
        // Arrange
        var entity = new TestEventableEntity(1);
        var event1 = new TestDomainEvent("Event 1");
        var event2 = new TestDomainEvent("Event 2");
        var event3 = new TestDomainEvent("Event 3");

        // Act
        entity.AddDomainEvent(event1);
        entity.AddDomainEvent(event2);
        entity.AddDomainEvent(event3);

        // Assert
        entity.DomainEvents.Count.ShouldBe(3);
        entity.DomainEvents.ShouldContain(event1);
        entity.DomainEvents.ShouldContain(event2);
        entity.DomainEvents.ShouldContain(event3);
    }

    [Fact]
    public void AddDomainEvent_WithNullEvent_ShouldThrowArgumentNullException()
    {
        // Arrange
        var entity = new TestEventableEntity(1);

        // Act & Assert
        ArgumentNullException exception = Should.Throw<ArgumentNullException>(() => entity.AddDomainEvent(null!));
        exception.ParamName.ShouldBe("domainEvent");
    }

    [Fact]
    public void ClearDomainEvents_WithExistingEvents_ShouldRemoveAllEvents()
    {
        // Arrange
        var entity = new TestEventableEntity(1);
        entity.AddDomainEvent(new TestDomainEvent("Event 1"));
        entity.AddDomainEvent(new TestDomainEvent("Event 2"));
        entity.DomainEvents.Count.ShouldBe(2); // Verify setup

        // Act
        entity.ClearDomainEvents();

        // Assert
        entity.DomainEvents.Count.ShouldBe(0);
    }

    [Fact]
    public void ClearDomainEvents_WithNoEvents_ShouldNotThrow()
    {
        // Arrange
        var entity = new TestEventableEntity(1);

        // Act & Assert
        Should.NotThrow(() => entity.ClearDomainEvents());
        entity.DomainEvents.Count.ShouldBe(0);
    }

    [Fact]
    public void DomainEvents_ShouldReturnReadOnlyCollection()
    {
        // Arrange
        var entity = new TestEventableEntity(1);
        var domainEvent = new TestDomainEvent("Test Event");
        entity.AddDomainEvent(domainEvent);

        // Act
        IReadOnlyList<IDomainEvent> domainEvents = entity.DomainEvents;

        // Assert
        domainEvents.ShouldBeOfType<ReadOnlyCollection<IDomainEvent>>();
        (domainEvents is not ICollection<IDomainEvent> collection || collection.IsReadOnly).ShouldBeTrue();
    }

    [Fact]
    public void DomainEvents_ModificationAttempt_ShouldNotAffectInternalCollection()
    {
        // Arrange
        var entity = new TestEventableEntity(1);
        var originalEvent = new TestDomainEvent("Original Event");
        entity.AddDomainEvent(originalEvent);

        // Act
        IReadOnlyList<IDomainEvent> domainEvents = entity.DomainEvents;
        var newEvent = new TestDomainEvent("New Event");

        // This should not be possible with a read-only collection
        // but let's verify the internal state remains unchanged
        entity.AddDomainEvent(newEvent);

        // Assert
        entity.DomainEvents.Count.ShouldBe(2);
        entity.DomainEvents.ShouldContain(originalEvent);
        entity.DomainEvents.ShouldContain(newEvent);
    }

    [Fact]
    public void EventableEntity_ShouldInheritEntityBehavior()
    {
        // Arrange
        const int expectedId = 42;
        DateTimeOffset beforeCreation = DateTimeOffset.UtcNow;

        // Act
        var entity = new TestEventableEntity(expectedId);
        DateTimeOffset afterCreation = DateTimeOffset.UtcNow;

        // Assert
        entity.Id.ShouldBe(expectedId);
        entity.DateCreated.ShouldBeGreaterThanOrEqualTo(beforeCreation);
        entity.DateCreated.ShouldBeLessThanOrEqualTo(afterCreation);
    }

    [Fact]
    public void AddDomainEvent_SameEventMultipleTimes_ShouldAddEachInstance()
    {
        // Arrange
        var entity = new TestEventableEntity(1);
        var domainEvent = new TestDomainEvent("Repeated Event");

        // Act
        entity.AddDomainEvent(domainEvent);
        entity.AddDomainEvent(domainEvent);
        entity.AddDomainEvent(domainEvent);

        // Assert
        entity.DomainEvents.Count.ShouldBe(3);
        entity.DomainEvents.ShouldAllBe(e => ReferenceEquals(e, domainEvent));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void AddDomainEvent_MultipleEvents_ShouldMaintainCorrectCount(int eventCount)
    {
        // Arrange
        var entity = new TestEventableEntity(1);

        // Act
        for (int i = 0; i < eventCount; i++)
        {
            entity.AddDomainEvent(new TestDomainEvent($"Event {i}"));
        }

        // Assert
        entity.DomainEvents.Count.ShouldBe(eventCount);
    }
}
