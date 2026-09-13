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
        run.RecordCareSuccess();
        Assert.Equal(1, run.Satisfaction);
        Assert.Equal(1, run.DayNumber);

        // Act
        run.TakeDamage(); // 3 -> 2
        run.TakeDamage(); // 2 -> 1
        bool forcedRetreat = run.TakeDamage(); // 1 -> 0 triggers retreat

        // Assert
        Assert.True(forcedRetreat);
        Assert.Equal(3, run.Health); // Restored
        Assert.Equal(0, run.Satisfaction); // Reset
        Assert.Equal(1, run.ForcedRetreats);
        Assert.Equal(2, run.DayNumber); // Day advanced exactly once
    }

    [Fact]
    public void RecordCareSuccess_WhenInvoked_IncrementsSatisfaction()
    {
        // Arrange
        var run = new PrototypeRun();
        Assert.Equal(0, run.Satisfaction);

        // Act & Assert
        run.RecordCareSuccess();
        Assert.Equal(1, run.Satisfaction);

        run.RecordCareSuccess();
        Assert.Equal(2, run.Satisfaction);

        run.RecordCareSuccess();
        Assert.Equal(3, run.Satisfaction);
    }

    [Fact]
    public void CompleteSession_WhenInvoked_IncrementsSessionsTodayAndResetsSatisfaction()
    {
        // Arrange
        var run = new PrototypeRun();
        run.RecordCareSuccess();
        run.RecordCareSuccess();
        run.RecordCareSuccess();
        Assert.Equal(3, run.Satisfaction);
        Assert.Equal(0, run.SessionsToday);
        Assert.Equal(0, run.CompletedSessions(run.ActivePet));

        // Act
        run.CompleteSession();

        // Assert
        Assert.Equal(0, run.Satisfaction);
        Assert.Equal(1, run.SessionsToday);
        Assert.Equal(1, run.CompletedSessions(PetKind.Baseline));
        Assert.True(run.CanEndDay);
    }

    [Fact]
    public void IsLogUnlocked_WhenCompletedSessionsUnderThree_ReturnsFalse()
    {
        // Arrange
        var run = new PrototypeRun();
        Assert.False(run.IsLogUnlocked(PetKind.Baseline));

        // Act & Assert
        run.CompleteSession();
        Assert.False(run.IsLogUnlocked(PetKind.Baseline));

        run.CompleteSession();
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

    [Fact]
    public void EndDay_SequentialCalls_AdvanceDaysIncrementallyWithoutSkipping()
    {
        // Arrange
        var run = new PrototypeRun();
        Assert.Equal(1, run.DayNumber);

        // Act & Assert Day 1 -> 2
        run.EndDay();
        Assert.Equal(2, run.DayNumber);

        // Act & Assert Day 2 -> 3
        run.EndDay();
        Assert.Equal(3, run.DayNumber);

        // Act & Assert Day 3 -> 4
        run.EndDay();
        Assert.Equal(4, run.DayNumber);

        // Act & Assert Day 4 -> 5
        run.EndDay();
        Assert.Equal(5, run.DayNumber);

        // Act & Assert Day 5 -> 6 (Complete)
        run.EndDay();
        Assert.Equal(6, run.DayNumber);
        Assert.True(run.IsComplete);
    }

    [Fact]
    public void ActivePet_WhenDayExceedsSchedule_DoesNotThrowAndReturnsLastScheduledPet()
    {
        // Arrange
        var run = new PrototypeRun();
        for (int i = 0; i < 5; i++)
        {
            run.EndDay();
        }
        Assert.Equal(6, run.DayNumber);
        Assert.True(run.IsComplete);

        // Act & Assert - must not throw IndexOutOfRangeException
        PetKind activePet = run.ActivePet;
        Assert.Equal(PetKind.Trickster, activePet);
    }

    [Fact]
    public void TakeDamage_WhenHealthReachesZeroOnDay5_SetsIsComplete()
    {
        // Arrange
        var run = new PrototypeRun();
        for (int i = 1; i < 5; i++)
        {
            run.EndDay();
        }
        Assert.Equal(5, run.DayNumber);
        Assert.False(run.IsComplete);

        // Act
        run.TakeDamage();
        run.TakeDamage();
        bool retreat = run.TakeDamage();

        // Assert
        Assert.True(retreat);
        Assert.Equal(6, run.DayNumber);
        Assert.True(run.IsComplete);
    }

    [Fact]
    public void FullFiveDayPlaythrough_WithMixedCareAndRetreats_CompletesCleanly()
    {
        // Day 1: Mossling (Baseline)
        var run = new PrototypeRun();
        Assert.Equal(1, run.DayNumber);
        Assert.Equal(PetKind.Baseline, run.ActivePet);
        run.RecordCareSuccess();
        run.CompleteSession();
        Assert.True(run.CanEndDay);
        run.EndDay(); // Day 1 -> 2

        // Day 2: Nibbleclaw (Attacker) - Takes 3 damage, forces retreat
        Assert.Equal(2, run.DayNumber);
        Assert.Equal(PetKind.Attacker, run.ActivePet);
        run.TakeDamage();
        run.TakeDamage();
        bool retreat = run.TakeDamage();
        Assert.True(retreat);
        Assert.Equal(1, run.ForcedRetreats);
        Assert.Equal(3, run.Health);
        Assert.Equal(3, run.DayNumber); // Day 2 -> 3

        // Day 3: Blinkbun (Trickster) - Completes care
        Assert.Equal(3, run.DayNumber);
        Assert.Equal(PetKind.Trickster, run.ActivePet);
        run.RecordCareSuccess();
        run.CompleteSession();
        run.EndDay(); // Day 3 -> 4

        // Day 4: Nibbleclaw (Attacker)
        Assert.Equal(4, run.DayNumber);
        Assert.Equal(PetKind.Attacker, run.ActivePet);
        run.RecordCareSuccess();
        run.CompleteSession();
        run.EndDay(); // Day 4 -> 5

        // Day 5: Blinkbun (Trickster)
        Assert.Equal(5, run.DayNumber);
        Assert.Equal(PetKind.Trickster, run.ActivePet);
        run.RecordCareSuccess();
        run.CompleteSession();
        run.EndDay(); // Day 5 -> 6 (Complete)

        // Post-game status
        Assert.Equal(6, run.DayNumber);
        Assert.True(run.IsComplete);
        Assert.Equal(PetKind.Trickster, run.ActivePet); // Safe lookup
    }
}
