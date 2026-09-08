using System.Collections.Generic;

namespace BePal.Gameplay;

public sealed class PrototypeRun
{
    private static readonly PetKind[] DaySchedule =
    {
        PetKind.Baseline,
        PetKind.Attacker,
        PetKind.Trickster,
        PetKind.Attacker,
        PetKind.Trickster
    };

    private readonly Dictionary<PetKind, int> _completedSessions = new();

    public int DayNumber { get; private set; } = 1;
    public int Health { get; private set; } = 3;
    public int Satisfaction { get; private set; }
    public int SessionsToday { get; private set; }
    public int ForcedRetreats { get; private set; }

    public PetKind ActivePet => DaySchedule[DayNumber - 1];
    public bool IsComplete => DayNumber > DaySchedule.Length;
    public bool CanEndDay => SessionsToday > 0;

    public int CompletedSessions(PetKind pet) => _completedSessions.GetValueOrDefault(pet);

    public bool IsLogUnlocked(PetKind pet) => CompletedSessions(pet) >= 3;

    public void RecordCareSuccess()
    {
        Satisfaction++;
    }

    public void CompleteSession()
    {
        Satisfaction = 0;
        SessionsToday++;
        _completedSessions[ActivePet] = CompletedSessions(ActivePet) + 1;
    }

    public bool TakeDamage()
    {
        Health--;

        if (Health > 0)
        {
            return false;
        }

        ForcedRetreats++;
        AdvanceDay();
        return true;
    }

    public void EndDay()
    {
        AdvanceDay();
    }

    private void AdvanceDay()
    {
        DayNumber++;
        Health = 3;
        Satisfaction = 0;
        SessionsToday = 0;
    }
}
