namespace BePalV2.Gameplay;

public enum PetSpecies
{
    Coco,       // Mossling: HP 100, Stomach 80, Clean 70. Photosynthesis (+5 HP if Clean > 80)
    Sproutlet,  // HP 90, Stomach 70, Clean 80. Agile Reflex (Train Perfect zone +15%)
    Gloomtail,  // HP 110, Stomach 60, Clean 60. Shadow Barrier (25% dodge dmg reduction, 50% miss boss reduction)
    Toothless   // Acid reptile: HP 60, Stomach 50, Clean 60. Ferocious (2x counter attack damage)
}

public enum CareActionType
{
    Feed,
    Clean,
    Train,
    Heal
}

public enum PrecisionTier
{
    Perfect,
    Good,
    Miss
}

public enum DailyPhase
{
    MorningEvent,
    CareAction,
    DefenseResolution,
    ProgressionSummary
}

public enum StoryEnding
{
    None,
    EndingA_Betrayal,
    EndingB_Protector,
    EndingBad_Foreclosure,
    Ending_VerticalSliceForcedDefeat
}
