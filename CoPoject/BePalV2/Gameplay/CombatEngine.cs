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

    // Toothless Taming (Day 2)
    public float TameGauge { get; private set; } = 0f;

    // Merchant Boss (Day 3) - Slide 54: "เลือด = การกดโจมตีโดน 5 ครั้ง"
    public const int MaxBossHits = 5;
    public float BossMaxHp { get; } = 1000f;
    public float BossHp { get; private set; } = 1000f;
    public int BossHitsRemaining => (int)Math.Ceiling(Math.Max(0f, BossHp) / (BossMaxHp / MaxBossHits));
    public int BossPhase => BossHp > 700f ? 1 : (BossHp > 300f ? 2 : 3);

    // Needle & QTE State
    public float NeedleAngle { get; private set; }
    public float AngularVelocity { get; private set; } = 3.0f;
    public float DodgeZoneCenter { get; private set; } = 1.75f * (float)Math.PI; // Top-right ~315 deg (Slide 38)
    public float DodgeZoneHalfWidth { get; private set; } = 0.30f;

    // Counter / Attack Window
    public bool IsCounterWindowOpen { get; private set; }
    public float CounterWindowTimer { get; private set; }
    public const float CounterWindowDuration = 0.35f;

    public bool IsParryWindowOpen { get; private set; }

    // Consecutive dodges and rewards
    public int ConsecutiveDodges { get; private set; }
    public int GoldEarnedInFight { get; private set; }
    public int GoldStolenFromPlayer { get; private set; } // Slide 54: gold theft on miss

    // Shield hits from Cloudy Glasses
    public int ShieldHitsRemaining { get; private set; }

    public bool IsPlayerDefeated => PlayerHp <= 0f || ActivePet.Health <= 0f;
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

        float speed = mode == CombatMode.MerchantBoss ? 3.0f : 2.5f;
        if (activePet.Species == PetSpecies.Coco)
        {
            speed *= 0.80f; // Coco synergy
        }
        AngularVelocity = speed;

        float width = 0.30f;
        if (activePet.Species == PetSpecies.Sproutlet)
        {
            width *= 1.25f; // Sproutlet synergy (0.30 * 1.25 = 0.375)
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
                GoldEarnedInFight += 2;
            }

            if (Mode == CombatMode.MerchantBoss && BossPhase == 1 && ConsecutiveDodges >= 3)
            {
                ApplyDamageToBoss(100f);
                ConsecutiveDodges = 0;
            }

            // Slide 38: "ปุ่มจะค่อยๆหดสั้นลง" (Dodge window dynamically shrinks each dodge)
            DodgeZoneHalfWidth = Math.Max(0.12f, DodgeZoneHalfWidth * 0.95f);

            // Shift dodge zone to new angle
            DodgeZoneCenter = (DodgeZoneCenter + 1.6f) % (float)(2.0 * Math.PI);
            return true;
        }
        else
        {
            ConsecutiveDodges = 0;
            IsCounterWindowOpen = false;

            // Slide 38 & 54 consequences:
            if (Mode == CombatMode.ToothlessTaming)
            {
                ActivePet.TakeDamage(25f);
                TakePlayerDamage(15f);
            }
            else if (Mode == CombatMode.MerchantBoss)
            {
                GoldStolenFromPlayer += 200;
                TakePlayerDamage(20f);
            }

            return false;
        }
    }

    public bool AttemptCounter()
    {
        if (IsFinished || !IsCounterWindowOpen) return false;

        IsCounterWindowOpen = false;

        if (Mode == CombatMode.ToothlessTaming)
        {
            TameGauge = Math.Min(100f, TameGauge + 25f);
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
            GoldStolenFromPlayer += 200;
            TakePlayerDamage(25f);
            return false;
        }
    }

    private void ApplyDamageToBoss(float damage)
    {
        BossHp = Math.Max(0f, BossHp - damage);
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
            damage *= 0.50f;
        }

        PlayerHp = Math.Max(0f, PlayerHp - damage);
        if (Mode == CombatMode.ToothlessTaming && !ActivePet.HasAcidBurn)
        {
            ActivePet.HasAcidBurn = true;
        }
    }
}
