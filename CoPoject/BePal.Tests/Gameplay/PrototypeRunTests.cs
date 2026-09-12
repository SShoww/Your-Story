#nullable enable
using BePal.Gameplay;
using Xunit;

namespace BePal.Tests.Gameplay;

public class PrototypeRunTests
{
    [Fact]
    public void TakeDamage_WhenHealthAboveZero_DecrementsHealthAndReturnsFalse()
    {
        // Arrange
        var run = new PrototypeRun();
        Assert.Equal(3, run.Health);

        // Act
        bool forcedRetreat = run.TakeDamage();

        // Assert
        Assert.False(forcedRetreat);
        Assert.Equal(2, run.Health);
        Assert.Equal(0, run.ForcedRetreats);
    }

    [Fact]
    public void TakeDamage_WhenHealthReachesZero_TriggersForcedRetreatRestoresHealthAndAdvancesDay()
    {
        // Arrange
        var run = new PrototypeRun();
        Assert.Equal(1, run.DayNumber);

        // Act
        run.TakeDamage(); // 3 -> 2
        run.TakeDamage(); // 2 -> 1
        bool forcedRetreat = run.TakeDamage(); // 1 -> 0 triggers retreat

        // Assert
        Assert.True(forcedRetreat);
        Assert.Equal(3, run.Health);
        Assert.Equal(1, run.ForcedRetreats);
        Assert.Equal(2, run.DayNumber);
    }

    [Fact]
    public void RecordCareSuccess_WhenInvoked_IncrementsSatisfaction()
    {
        // Arrange
        var run = new PrototypeRun();
        Assert.Equal(0, run.Satisfaction);

        // Act
        run.RecordCareSuccess();

        // Assert
        Assert.Equal(1, run.Satisfaction);

        // Act
        run.RecordCareSuccess();

        // Assert
        Assert.Equal(2, run.Satisfaction);
    }

    [Fact]
    public void CompleteSession_WhenInvoked_IncrementsSessionsTodayAndResetsSatisfaction()
    {
        // Arrange
        var run = new PrototypeRun();
        run.RecordCareSuccess();
        run.RecordCareSuccess();
        Assert.Equal(2, run.Satisfaction);
        Assert.Equal(0, run.SessionsToday);

        // Act
        run.CompleteSession();

        // Assert
        Assert.Equal(0, run.Satisfaction);
        Assert.Equal(1, run.SessionsToday);
        Assert.Equal(1, run.CompletedSessions(PetKind.Baseline));
    }

    [Fact]
    public void IsLogUnlocked_WhenCompletedSessionsUnderThree_ReturnsFalse()
    {
        // Arrange
        var run = new PrototypeRun();

        // Act
        run.CompleteSession();
        run.CompleteSession();

        // Assert
        Assert.Equal(2, run.CompletedSessions(PetKind.Baseline));
        Assert.False(run.IsLogUnlocked(PetKind.Baseline));
    }

    [Fact]
    public void IsLogUnlocked_WhenCompletedSessionsAtLeastThree_ReturnsTrue()
    {
        // Arrange
        var run = new PrototypeRun();

        // Act
        run.CompleteSession();
        run.CompleteSession();
        run.CompleteSession();

        // Assert
        Assert.Equal(3, run.CompletedSessions(PetKind.Baseline));
        Assert.True(run.IsLogUnlocked(PetKind.Baseline));
    }

    [Fact]
    public void EndDay_WhenCanEndDay_AdvancesDayAndResetsDailyValues()
    {
        // Arrange
        var run = new PrototypeRun();
        run.RecordCareSuccess();
        run.CompleteSession();
        Assert.True(run.CanEndDay);
        Assert.Equal(1, run.DayNumber);
        Assert.Equal(1, run.SessionsToday);

        // Act
        run.EndDay();

        // Assert
        Assert.Equal(2, run.DayNumber);
        Assert.Equal(3, run.Health);
        Assert.Equal(0, run.Satisfaction);
        Assert.Equal(0, run.SessionsToday);
        Assert.False(run.CanEndDay);
    }
}
