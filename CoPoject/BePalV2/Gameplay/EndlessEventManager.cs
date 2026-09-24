namespace BePalV2.Gameplay;

public enum DailyEventType
{
    SafeDayTutorial,
    ToothlessEncounter,
    MerchantEncounter,
    Thunderstorm,
    WildIncursion,
    TravelingMerchant
}

public sealed class EndlessEventManager
{
    private readonly Random _random;

    public EndlessEventManager(int? seed = null)
    {
        _random = seed.HasValue ? new Random(seed.Value) : new Random();
    }

    public DailyEventType DetermineMorningEvent(int dayNumber)
    {
        if (dayNumber <= 1) return DailyEventType.SafeDayTutorial;
        if (dayNumber == 2) return DailyEventType.ToothlessEncounter;
        if (dayNumber == 3) return DailyEventType.MerchantEncounter;

        // Day 4+: Endless random event generation
        int roll = _random.Next(3);
        return roll switch
        {
            0 => DailyEventType.Thunderstorm,
            1 => DailyEventType.WildIncursion,
            _ => DailyEventType.TravelingMerchant
        };
    }

    public string GetEventBriefing(DailyEventType eventType, int dayNumber) => eventType switch
    {
        DailyEventType.SafeDayTutorial =>
            "Welcome to Shelter Sector 7. Today is your orientation day. Care for your pet and learn to budget your 6 AP energy!",
        DailyEventType.ToothlessEncounter =>
            "Morning alert! Strange noises and purple acid puddles detected outside. A wild creature is knocking at the door.",
        DailyEventType.MerchantEncounter =>
            "A mysterious Traveling Merchant has arrived in a wagon, seeking rare specimens and offering supplies.",
        DailyEventType.Thunderstorm =>
            $"Shift 0{dayNumber} Alert: A severe cosmic thunderstorm has battered the facility! All pets have been contaminated with mud (Cleanliness dropped to 50%).",
        DailyEventType.WildIncursion =>
            $"Shift 0{dayNumber} Alert: A hostile abnormal beast is clawing at the outer airlock! Prepare your pet for combat.",
        DailyEventType.TravelingMerchant =>
            $"Shift 0{dayNumber} Arrival: The Traveling Merchant has returned to barter rare supplies.",
        _ => "Routine shelter operations."
    };
}
