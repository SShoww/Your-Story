namespace BePalV2.Gameplay;

public enum CombatMode
{
    ToothlessTaming,
    MerchantBoss
}

public sealed class CombatEngine
{
    public CombatMode Mode { get; }
    public PetEntity ActivePet { get; }
    public bool HasToothlessAlly { get; }
    public ItemDefinition? EquippedItem { get; }

    public float PlayerMaxHp { get; } = 100f;
    public float PlayerHp { get; private set; } = 100f;

    // Toothless Taming
    public float TameGauge { get; private set; } = 0f;

    // Merchant Boss
    public float BossMaxHp { get; } = 1000f;
    public float BossHp { get; private set; } = 1000f;
    public int BossPhase => BossHp > 700f ? 1 : (BossHp > 300f ? 2 : 3);

    // Needle & QTE State
    public float NeedleAngle { get; private set; }
    public float AngularVelocity { get; private set; } = 3.0f;
    public float DodgeZoneCenter { get; private set; } = 1.5f * (float)Math.PI;
    public float DodgeZoneHalfWidth { get; private set; } = 0.30f;

    // Counter / Parry Window
    public bool IsCounterWindowOpen { get; private set; }
    public float CounterWindowTimer { get; private set; }
    public const float CounterWindowDuration = 0.30f;

    public bool IsParryWindowOpen { get; private set; }

    // Consecutive dodges for Phase 1 Greed's Splash
    public int ConsecutiveDodges { get; private set; }
    public int GoldEarnedInFight { get; private set; }

    // Shield hits from Cloudy Glasses
    public int ShieldHitsRemaining { get; private set; }

    public bool IsPlayerDefeated => PlayerHp <= 0f;
    public bool IsCombatWon => Mode == CombatMode.ToothlessTaming ? TameGauge >= 100f : BossHp <= 0f;
    public bool IsFinished => IsPlayerDefeated || IsCombatWon;

    public CombatEngine(
        CombatMode mode,
        PetEntity activePet,
        bool hasToothlessAlly = false,
        ItemDefinition? equipped = null,
        float consumableDodgeBonus = 0f,
        int initialShieldHits = 0)
    {
        Mode = mode;
        ActivePet = activePet;
        HasToothlessAlly = hasToothlessAlly;
        EquippedItem = equipped;
        ShieldHitsRemaining = initialShieldHits;

        // Base velocity & dodge width
        float speed = mode == CombatMode.MerchantBoss ? 3.0f : 2.5f;
        if (activePet.Species == PetSpecies.Coco)
        {
            // Coco Synergy: reduces needle speed by 20%
            speed *= 0.80f;
        }
        AngularVelocity = speed;

        float width = mode == CombatMode.MerchantBoss && BossPhase == 2 ? 0.15f : 0.30f;
        if (activePet.Species == PetSpecies.Sproutlet)
        {
            // Sproutlet Synergy: +25% dodge zone width
            width *= 1.25f;
        }
        if (consumableDodgeBonus > 0f)
        {
            width *= (1.0f + consumableDodgeBonus);
        }
        DodgeZoneHalfWidth = width;
    }

    public void Update(float dt)
    {
        if (IsFinished) return;

        NeedleAngle = (NeedleAngle + AngularVelocity * dt) % (float)(2.0 * Math.PI);
        if (NeedleAngle < 0f) NeedleAngle += (float)(2.0 * Math.PI);

        // Counter window countdown
        if (IsCounterWindowOpen)
        {
            CounterWindowTimer -= dt;
            if (CounterWindowTimer <= 0f)
            {
                IsCounterWindowOpen = false;
            }
        }
    }

    public bool IsNeedleInDodgeZone()
    {
        float diff = Math.Abs(NeedleAngle - DodgeZoneCenter);
        float tau = (float)(2.0 * Math.PI);
        diff = Math.Min(diff, tau - diff);
        return diff <= DodgeZoneHalfWidth;
    }

    public bool AttemptDodge()
    {
        if (IsFinished) return false;

        bool success = IsNeedleInDodgeZone();
        if (success)
        {
            ConsecutiveDodges++;
            IsCounterWindowOpen = true;
            CounterWindowTimer = CounterWindowDuration;

            if (Mode == CombatMode.MerchantBoss && BossPhase == 2)
            {
                // Gold Gatling reward: +2 gold per dodge
                GoldEarnedInFight += 2;
            }

            if (Mode == CombatMode.MerchantBoss && BossPhase == 1 && ConsecutiveDodges >= 3)
            {
                // Greed's Splash counter: 100 dmg
                ApplyDamageToBoss(100f);
                ConsecutiveDodges = 0;
            }

            // Shift dodge zone to new angle for dynamic challenge
            DodgeZoneCenter = (DodgeZoneCenter + 1.8f) % (float)(2.0 * Math.PI);
            return true;
        }
        else
        {
            ConsecutiveDodges = 0;
            IsCounterWindowOpen = false;
            TakePlayerDamage(Mode == CombatMode.MerchantBoss ? 20f : 15f);
            return false;
        }
    }

    public bool AttemptCounter()
    {
        if (IsFinished || !IsCounterWindowOpen) return false;

        IsCounterWindowOpen = false;
        float baseCounter = 25f;

        if (Mode == CombatMode.ToothlessTaming)
        {
            // Tame gauge +25%
            TameGauge = Math.Min(100f, TameGauge + baseCounter);
            return true;
        }
        else
        {
            float counterDmg = 80f;
            if (HasToothlessAlly || ActivePet.Species == PetSpecies.Toothless)
            {
                counterDmg *= 2.0f; // 2x Toothless counter synergy
            }
            if (EquippedItem?.CounterDamageBonus > 0f)
            {
                counterDmg *= (1.0f + EquippedItem.CounterDamageBonus);
            }
            ApplyDamageToBoss(counterDmg);
            return true;
        }
    }

    public bool AttemptParry()
    {
        if (IsFinished || Mode != CombatMode.MerchantBoss || BossPhase != 3) return false;

        // In Phase 3, purple parry window when needle is near target
        if (IsNeedleInDodgeZone())
        {
            float parryDmg = 150f;
            if (HasToothlessAlly) parryDmg *= 2.0f;
            if (EquippedItem?.CounterDamageBonus > 0f) parryDmg *= (1.0f + EquippedItem.CounterDamageBonus);
            ApplyDamageToBoss(parryDmg);
            return true;
        }
        else
        {
            TakePlayerDamage(25f);
            return false;
        }
    }

    public void TeleportNeedle()
    {
        if (Mode == CombatMode.MerchantBoss && BossPhase == 3)
        {
            NeedleAngle = (NeedleAngle + (float)Math.PI) % (float)(2.0 * Math.PI);
        }
    }

    private void ApplyDamageToBoss(float damage)
    {
        BossHp = Math.Max(0f, BossHp - damage);
        if (BossPhase == 2)
        {
            DodgeZoneHalfWidth = 0.15f * (ActivePet.Species == PetSpecies.Sproutlet ? 1.25f : 1.0f);
        }
    }

    private void TakePlayerDamage(float rawDamage)
    {
        if (ShieldHitsRemaining > 0)
        {
            ShieldHitsRemaining--;
            return;
        }

        float damage = rawDamage;
        if (ActivePet.Species == PetSpecies.Gloomtail)
        {
            damage *= 0.50f; // 50% damage reduction
        }

        PlayerHp = Math.Max(0f, PlayerHp - damage);
        if (Mode == CombatMode.ToothlessTaming && !ActivePet.HasAcidBurn)
        {
            ActivePet.HasAcidBurn = true;
        }
    }
}
