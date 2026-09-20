namespace BePalV2.Gameplay;

public enum ItemCategory
{
    Consumable,
    Equipment,
    Medicine
}

public sealed record ItemDefinition(
    string Id,
    string Name,
    string Description,
    int Price,
    ItemCategory Category,
    int HealthRestore = 0,
    int StomachRestore = 0,
    int EnergyRestore = 0,
    float PerfectZoneBonus = 0f,
    float DodgeZoneBonus = 0f,
    float CounterDamageBonus = 0f,
    float CleanDecayReduction = 0f,
    int ActionStomachBurnExtra = 0,
    int ShieldHits = 0,
    float TrainExpMultiplier = 1f
)
{
    public static readonly ItemDefinition CrabApple = new(
        "crab_apple", "Crab Apple", "Juicy wild apple. Restores 18 HP and 20 Stomach (0 AP).",
        Price: 25, Category: ItemCategory.Consumable, HealthRestore: 18, StomachRestore: 20
    );

    public static readonly ItemDefinition SeaTea = new(
        "sea_tea", "Sea Tea", "Crisp ocean herbal tea. Expands Dodge Zone by +20% for 1 day.",
        Price: 18, Category: ItemCategory.Consumable, DodgeZoneBonus: 0.20f
    );

    public static readonly ItemDefinition CloudyGlasses = new(
        "cloudy_glasses", "Cloudy Glasses", "Smoky lens charm. Grants 2 hits invulnerability shield.",
        Price: 30, Category: ItemCategory.Consumable, ShieldHits: 2
    );

    public static readonly ItemDefinition TornNotebook = new(
        "torn_notebook", "Torn Notebook", "Old research diary. Train EXP permanently boosted +50%.",
        Price: 55, Category: ItemCategory.Consumable, TrainExpMultiplier: 1.50f
    );

    public static readonly ItemDefinition CaffeineTonic = new(
        "caffeine_tonic", "Caffeine Tonic", "Stimulant brew. Instantly restores +2 AP.",
        Price: 40, Category: ItemCategory.Consumable, EnergyRestore: 2
    );

    public static readonly ItemDefinition FirstAidKit = new(
        "first_aid_kit", "First Aid Kit", "Standard medical supplies. Enables 1 AP Heal and restores 30 HP.",
        Price: 20, Category: ItemCategory.Medicine, HealthRestore: 30
    );

    public static readonly ItemDefinition BalletShoes = new(
        "ballet_shoes", "Ballet Shoes", "Satin slippers. Perfect Zone +15%, but extra 5 Stomach burn per action.",
        Price: 50, Category: ItemCategory.Equipment, PerfectZoneBonus: 0.15f, ActionStomachBurnExtra: 5
    );

    public static readonly ItemDefinition ToyKnife = new(
        "toy_knife", "Toy Knife", "Plastic dull blade. Counter-attack damage increased +35%.",
        Price: 45, Category: ItemCategory.Equipment, CounterDamageBonus: 0.35f
    );

    public static readonly ItemDefinition FadedRibbon = new(
        "faded_ribbon", "Faded Ribbon", "Protective silken charm. Natural Clean decay reduced by 30%.",
        Price: 35, Category: ItemCategory.Equipment, CleanDecayReduction: 0.30f
    );

    public static IReadOnlyList<ItemDefinition> AllCatalog { get; } = new List<ItemDefinition>
    {
        CrabApple,
        SeaTea,
        CloudyGlasses,
        TornNotebook,
        CaffeineTonic,
        FirstAidKit,
        BalletShoes,
        ToyKnife,
        FadedRibbon
    };
}
