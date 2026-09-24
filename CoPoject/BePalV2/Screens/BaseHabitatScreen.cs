using BePalV2.Audio;
using BePalV2.Gameplay;
using BePalV2.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace BePalV2.Screens;

public sealed class BaseHabitatScreen : IScreen
{
    private readonly ScreenContext _ctx;
    private readonly DialogueBox _dialogue = new();
    private KeyboardState _prevKeyboard;
    private MouseState _prevMouse;

    private float _breathTimer;

    // Room Interactive Zones (1920x1080)
    private readonly Rectangle _doorRect = new(860, 260, 200, 460);
    private readonly Rectangle _upgradeStationRect = new(120, 420, 220, 220);
    private readonly Rectangle _survivalDeskRect = new(380, 560, 240, 160);
    private readonly Rectangle _doctorRect = new(130, 680, 180, 200);
    private readonly Rectangle _petStageRect = new(1440, 480, 320, 280);

    // Bottom action buttons (Quick Bag & Shop only - Care actions moved to QTE)
    private readonly Rectangle _bagBtn = new(1640, 970, 220, 56);
    private readonly Rectangle _shopBtn = new(1380, 970, 220, 56);

    // Modal Active States
    private bool _showDoctorModal;
    private bool _doctorInReviveStep;
    private string? _doctorFeedback;

    private bool _showUpgradeModal;
    private string? _upgradeFeedback;

    private bool _showLogModal;
    private int _logActiveTab = 0; // 0 = pet discovery, 1 = Disaster

    private bool _showDoorModal;

    // Doctor modal buttons
    private readonly Rectangle _doctorYesBtn = new(720, 580, 200, 52);
    private readonly Rectangle _doctorNoBtn = new(1000, 580, 200, 52);
    private readonly Rectangle _doctorReviveBtn = new(760, 620, 400, 56);
    private readonly Rectangle _doctorCloseBtn = new(860, 740, 200, 50);

    // Upgrade cards (1920x1080)
    private readonly Rectangle _upgCard1 = new(420, 280, 320, 460);
    private readonly Rectangle _upgCard2 = new(800, 280, 320, 460);
    private readonly Rectangle _upgCard3 = new(1180, 280, 320, 460);
    private readonly Rectangle _upgCloseBtn = new(860, 780, 200, 50);

    // Survival log tabs & close
    private readonly Rectangle _tabPetDiscovery = new(420, 210, 240, 50);
    private readonly Rectangle _tabDisaster = new(690, 210, 240, 50);
    private readonly Rectangle _logCloseBtn = new(860, 800, 200, 50);

    // Door event choices
    private readonly Rectangle _doorOption1Btn = new(660, 640, 280, 56);
    private readonly Rectangle _doorOption2Btn = new(980, 640, 280, 56);

    public BaseHabitatScreen(ScreenContext ctx)
    {
        _ctx = ctx;
        if (_ctx.Run == null)
        {
            _ctx.Run = new V2RunState();
        }

        SetupMorningBriefing();
    }

    public void OpenLogModal(int tab = 0)
    {
        _showLogModal = true;
        _logActiveTab = tab;
        _dialogue.Close();
    }

    public void OpenUpgradeModal()
    {
        _showUpgradeModal = true;
        _dialogue.Close();
    }

    public void OpenDoctorModal()
    {
        _showDoctorModal = true;
        _doctorInReviveStep = false;
        _dialogue.Close();
    }

    private void SetupMorningBriefing()
    {
        var run = _ctx.Run;
        if (run.CurrentPhase == DailyPhase.MorningEvent)
        {
            switch (run.DayNumber)
            {
                case 1:
                    _dialogue.StartDialogue(
                        "CHIEF OVERSEER",
                        new[] { "Welcome to Shelter Sector 7. A violent thunderstorm is approaching tonight. Tend to your pet's needs before the storm hits!" },
                        () => run.SetPhase(DailyPhase.CareAction)
                    );
                    break;
                case 2:
                    _dialogue.StartDialogue(
                        "SECURITY PROTOCOL",
                        new[] { "Morning alert! Strange noises detected at the outer hatch. A wild creature is knocking outside." },
                        () => run.SetPhase(DailyPhase.CareAction)
                    );
                    break;
                case 3:
                    _dialogue.StartDialogue(
                        "TRADING POST DISPATCH",
                        new[] { "Final day of operation. A shady Traveling Merchant has parked his wagon outside your facility door." },
                        () => run.SetPhase(DailyPhase.CareAction)
                    );
                    break;
            }
        }
    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _breathTimer += dt * 3.2f;

        var kbd = Keyboard.GetState();
        var mouse = Mouse.GetState();
        Point mPos = mouse.Position;
        bool click = mouse.LeftButton == ButtonState.Pressed && _prevMouse.LeftButton == ButtonState.Released;

        // Dialogue Box takes priority
        if (_dialogue.IsActive)
        {
            _dialogue.UpdateInput(dt, kbd, _prevKeyboard, mouse, _prevMouse);
            _prevKeyboard = kbd;
            _prevMouse = mouse;
            return;
        }

        // Modals
        if (_showDoctorModal)
        {
            UpdateDoctorModal(kbd, click, mPos);
        }
        else if (_showUpgradeModal)
        {
            UpdateUpgradeModal(kbd, click, mPos);
        }
        else if (_showLogModal)
        {
            UpdateLogModal(kbd, click, mPos);
        }
        else if (_showDoorModal)
        {
            UpdateDoorModal(kbd, click, mPos);
        }
        else
        {
            UpdateRoomInteractions(kbd, click, mPos);
        }

        _prevKeyboard = kbd;
        _prevMouse = mouse;
    }

    private void UpdateDoctorModal(KeyboardState kbd, bool click, Point mPos)
    {
        if (kbd.IsKeyDown(Keys.Escape) && !_prevKeyboard.IsKeyDown(Keys.Escape))
        {
            _showDoctorModal = false;
            _doctorInReviveStep = false;
            _doctorFeedback = null;
            _ctx.Audio.PlayConfirm();
        }

        if (click)
        {
            if (!_doctorInReviveStep)
            {
                if (_doctorYesBtn.Contains(mPos))
                {
                    _doctorInReviveStep = true;
                    _ctx.Audio.PlayConfirm();
                }
                else if (_doctorNoBtn.Contains(mPos) || _doctorCloseBtn.Contains(mPos))
                {
                    _showDoctorModal = false;
                    _doctorFeedback = null;
                    _ctx.Audio.PlayConfirm();
                }
            }
            else
            {
                if (_doctorReviveBtn.Contains(mPos))
                {
                    var pet = _ctx.Run.ActivePet;
                    if (pet.Health > 0)
                    {
                        _doctorFeedback = "Pet is not fainted! Revive not needed.";
                        _ctx.Audio.PlayWarning();
                    }
                    else if (_ctx.Run.Economy.CanAfford(500))
                    {
                        _ctx.Run.Economy.SpendGold(500);
                        pet.Revive(1);
                        _doctorFeedback = "Pet revived to 1 HP! (Spent 500 G)";
                        _ctx.Audio.PlaySuccess();
                    }
                    else
                    {
                        _ctx.Run.Economy.IssueEmergencyLoan(500);
                        _ctx.Run.Economy.SpendGold(500);
                        pet.Revive(1);
                        _doctorFeedback = "500G Loan issued! Pet revived to 1 HP.";
                        _ctx.Audio.PlaySuccess();
                    }
                }

                if (_doctorCloseBtn.Contains(mPos))
                {
                    _showDoctorModal = false;
                    _doctorInReviveStep = false;
                    _doctorFeedback = null;
                    _ctx.Audio.PlayConfirm();
                }
            }
        }
    }

    private void UpdateUpgradeModal(KeyboardState kbd, bool click, Point mPos)
    {
        if ((kbd.IsKeyDown(Keys.Escape) && !_prevKeyboard.IsKeyDown(Keys.Escape)) ||
            (click && _upgCloseBtn.Contains(mPos)))
        {
            _showUpgradeModal = false;
            _upgradeFeedback = null;
            _ctx.Audio.PlayConfirm();
        }

        if (click)
        {
            var eco = _ctx.Run.Economy;
            // Card 1: QTE Upgrade (Cost: 60 Player Points)
            if (_upgCard1.Contains(mPos))
            {
                if (eco.SpendPlayerPoints(60))
                {
                    _upgradeFeedback = "Purchased QTE Upgrade (+15% Needle Zone)! (Spent 60 PTS)";
                    _ctx.Audio.PlaySuccess();
                }
                else
                {
                    _upgradeFeedback = "Not enough Research Points! Needs 60 PTS.";
                    _ctx.Audio.PlayWarning();
                }
            }
            // Card 2: Energy Upgrade (Cost: 150 Gold + 80 Player Points)
            else if (_upgCard2.Contains(mPos))
            {
                if (eco.CanAfford(150) && eco.CanAffordPoints(80))
                {
                    eco.SpendGold(150);
                    eco.SpendPlayerPoints(80);
                    _ctx.Run.Energy.UpgradeMaxEnergy(2);
                    _upgradeFeedback = "Purchased Energy Upgrade (+2 AP)! (150 G + 80 PTS)";
                    _ctx.Audio.PlaySuccess();
                }
                else
                {
                    _upgradeFeedback = "Needs both 150 Gold AND 80 Research Points to upgrade!";
                    _ctx.Audio.PlayWarning();
                }
            }
            // Card 3: Care Booster (Cost: 70 Player Points)
            else if (_upgCard3.Contains(mPos))
            {
                if (eco.SpendPlayerPoints(70))
                {
                    _upgradeFeedback = "Purchased Care Booster (+50% Stat Gains)! (Spent 70 PTS)";
                    _ctx.Audio.PlaySuccess();
                }
                else
                {
                    _upgradeFeedback = "Not enough Research Points! Needs 70 PTS.";
                    _ctx.Audio.PlayWarning();
                }
            }
        }
    }

    private void UpdateLogModal(KeyboardState kbd, bool click, Point mPos)
    {
        if ((kbd.IsKeyDown(Keys.Escape) && !_prevKeyboard.IsKeyDown(Keys.Escape)) ||
            (click && _logCloseBtn.Contains(mPos)))
        {
            _showLogModal = false;
            _ctx.Audio.PlayConfirm();
        }

        if (click)
        {
            if (_tabPetDiscovery.Contains(mPos))
            {
                _logActiveTab = 0;
                _ctx.Audio.PlayConfirm();
            }
            else if (_tabDisaster.Contains(mPos))
            {
                _logActiveTab = 1;
                _ctx.Audio.PlayConfirm();
            }
        }
    }

    private void UpdateDoorModal(KeyboardState kbd, bool click, Point mPos)
    {
        if (kbd.IsKeyDown(Keys.Escape) && !_prevKeyboard.IsKeyDown(Keys.Escape))
        {
            _showDoorModal = false;
            _ctx.Audio.PlayConfirm();
        }

        if (click)
        {
            if (_ctx.Run.DayNumber == 2)
            {
                if (_doorOption1Btn.Contains(mPos))
                {
                    _ctx.Run.ResolveDay2Encounter(chooseTame: false, tameSuccess: false);
                    _showDoorModal = false;
                    _ctx.Audio.PlayConfirm();
                }
                else if (_doorOption2Btn.Contains(mPos))
                {
                    if (!CombatEngine.CanPetFight(_ctx.Run.ActivePet, out string? refusal))
                    {
                        _dialogue.ShowPrompt("COMBAT REFUSAL", refusal!, () => _dialogue.Close());
                        _showDoorModal = false;
                        _ctx.Audio.PlayWarning();
                        return;
                    }

                    _showDoorModal = false;
                    _ctx.Audio.PlayConfirm();
                    _ctx.ScreenManager.SetScreen(new CombatArenaScreen(_ctx, CombatMode.ToothlessTaming));
                }
            }
            else if (_ctx.Run.DayNumber == 3)
            {
                if (_doorOption1Btn.Contains(mPos))
                {
                    _ctx.Run.AcceptMerchantBuyout();
                    _showDoorModal = false;
                    _ctx.Audio.PlaySuccess();
                    _ctx.ScreenManager.SetScreen(new EndingScreen(_ctx, StoryEnding.EndingA_Betrayal));
                }
                else if (_doorOption2Btn.Contains(mPos))
                {
                    if (!CombatEngine.CanPetFight(_ctx.Run.ActivePet, out string? refusal))
                    {
                        _dialogue.ShowPrompt("COMBAT REFUSAL", refusal!, () => _dialogue.Close());
                        _showDoorModal = false;
                        _ctx.Audio.PlayWarning();
                        return;
                    }

                    _showDoorModal = false;
                    _ctx.Audio.PlayConfirm();
                    _ctx.ScreenManager.SetScreen(new CombatArenaScreen(_ctx, CombatMode.MerchantBoss));
                }
            }
        }
    }

    private void UpdateRoomInteractions(KeyboardState kbd, bool click, Point mPos)
    {
        var run = _ctx.Run;
        bool hasKnockEvent = (run.DayNumber == 2 && !run.Day2EncounterResolved) ||
                             (run.DayNumber == 3 && !run.Day3BossDefeated);

        // Press E for door action or End Day
        if (kbd.IsKeyDown(Keys.E) && !_prevKeyboard.IsKeyDown(Keys.E))
        {
            if (hasKnockEvent)
            {
                _showDoorModal = true;
                _ctx.Audio.PlayConfirm();
            }
            else
            {
                ProceedToEndOfDay();
            }
        }

        // Quick Bag [B] and Shop [S]
        if (kbd.IsKeyDown(Keys.B) && !_prevKeyboard.IsKeyDown(Keys.B)) OpenBag();
        else if (kbd.IsKeyDown(Keys.S) && !_prevKeyboard.IsKeyDown(Keys.S)) OpenShop();

        // Spacebar or Click on Pet -> Launch Care QTE!
        if (kbd.IsKeyDown(Keys.Space) && !_prevKeyboard.IsKeyDown(Keys.Space))
        {
            TryLaunchCareQte();
            return;
        }

        if (click)
        {
            // Center Door
            if (_doorRect.Contains(mPos))
            {
                if (hasKnockEvent)
                {
                    _showDoorModal = true;
                    _ctx.Audio.PlayConfirm();
                }
                else
                {
                    ProceedToEndOfDay();
                }
            }
            // Upgrade Station
            else if (_upgradeStationRect.Contains(mPos))
            {
                _showUpgradeModal = true;
                _ctx.Audio.PlayConfirm();
            }
            // Survival Desk
            else if (_survivalDeskRect.Contains(mPos))
            {
                _showLogModal = true;
                _ctx.Audio.PlayConfirm();
            }
            // Doctor
            else if (_doctorRect.Contains(mPos))
            {
                _showDoctorModal = true;
                _doctorInReviveStep = false;
                _doctorFeedback = null;
                _ctx.Audio.PlayConfirm();
            }
            // Active Pet -> Launch Care QTE!
            else if (_petStageRect.Contains(mPos))
            {
                TryLaunchCareQte();
            }
            // Quick Buttons
            else if (_bagBtn.Contains(mPos)) OpenBag();
            else if (_shopBtn.Contains(mPos)) OpenShop();
        }
    }

    private void TryLaunchCareQte()
    {
        var run = _ctx.Run;
        if (run.Energy.CurrentEnergy <= 0)
        {
            _ctx.Audio.PlayWarning();
            _dialogue.ShowPrompt("OUT OF ENERGY", "You have exhausted all daily Energy Points (AP). Proceed to the shelter door to end the shift!", () => _dialogue.Close());
            return;
        }

        var pet = run.ActivePet;
        if (pet.Health <= 0)
        {
            _ctx.Audio.PlayWarning();
            _dialogue.ShowPrompt("PET FAINTED", $"{pet.Name} has fainted! Visit the Doctor NPC to revive your specimen before administering care.", () => _dialogue.Close());
            return;
        }

        _ctx.Audio.PlayConfirm();
        _ctx.ScreenManager.SetScreen(new CareQteScreen(_ctx));
    }

    private void OpenBag()
    {
        _ctx.Audio.PlayConfirm();
        _ctx.ScreenManager.PushOverlay(new InventoryOverlayScreen(_ctx));
    }

    private void OpenShop()
    {
        _ctx.Audio.PlayConfirm();
        _ctx.ScreenManager.PushOverlay(new ShopModalScreen(_ctx));
    }

    private void ProceedToEndOfDay()
    {
        var run = _ctx.Run;
        if (run.DayNumber == 1 && !run.Day1CalmingCompleted)
        {
            _ctx.ScreenManager.SetScreen(new CalmingQteScreen(_ctx));
        }
        else
        {
            _ctx.ScreenManager.SetScreen(new DailySummaryScreen(_ctx));
        }
    }

    public void Draw(GameTime gameTime, SpriteBatch batch)
    {
        Point mPos = Mouse.GetState().Position;

        // 1. Wallpaper Background
        batch.FillRectangle(new Rectangle(0, 0, _ctx.ScreenWidth, _ctx.ScreenHeight), new Color(18, 22, 30));
        Color seamColor = new(26, 32, 44);
        for (int x = 0; x <= _ctx.ScreenWidth; x += 120)
        {
            batch.DrawLine(x, 70, x, 780, seamColor, 1);
        }

        // 2. Hardwood Floor
        Color floorBase = new(28, 22, 18);
        batch.FillRectangle(new Rectangle(0, 780, _ctx.ScreenWidth, 300), floorBase);
        batch.DrawLine(0, 780, _ctx.ScreenWidth, 780, UITheme.BorderSubtle, 2f);

        Color plankColor = new(38, 32, 28);
        for (int y = 780; y <= 1080; y += 45)
        {
            batch.DrawLine(0, y, _ctx.ScreenWidth, y, plankColor, 1f);
        }

        // 3. Interactive Room Elements
        DrawInteractiveDoor(batch, mPos);
        DrawUpgradeStation(batch, mPos);
        DrawSurvivalDesk(batch, mPos);
        DrawDoctorCharacter(batch, mPos);
        DrawActivePet(batch, mPos);

        // 4. Bottom Controls (Bag & Shop only)
        DrawBottomBar(batch, mPos);

        // 5. Top Bar HUD
        DrawTopBar(batch);

        // 6. Dialogue Box
        if (_dialogue.IsActive)
        {
            _dialogue.Draw(batch, _ctx.Font, _ctx.Pixel, new Rectangle(360, 720, 1200, 240));
        }

        // 7. Modals
        if (_showDoctorModal) DrawDoctorModal(batch, mPos);
        if (_showUpgradeModal) DrawUpgradeModal(batch, mPos);
        if (_showLogModal) DrawLogModal(batch, mPos);
        if (_showDoorModal) DrawDoorModal(batch, mPos);
    }

    private void DrawTopBar(SpriteBatch batch)
    {
        var run = _ctx.Run;
        Rectangle topBar = new(0, 0, _ctx.ScreenWidth, 70);
        CleanUI.DrawPanel(batch, topBar, UITheme.BgPanel, UITheme.BorderSubtle, borderWidth: 1, shadow: true);

        // Gold Pill
        Rectangle goldPill = new(40, 15, 170, 40);
        CleanUI.DrawPanel(batch, goldPill, UITheme.BgCardRecessed, UITheme.BorderSubtle, borderWidth: 1, shadow: false);
        batch.FillRectangle(new Rectangle(goldPill.X + 10, goldPill.Y + 10, 20, 20), UITheme.AccentGold);
        batch.DrawString(_ctx.Font, $"{run.Economy.Gold} G", new Vector2(goldPill.X + 40, goldPill.Y + 8), UITheme.AccentGold);

        // Research Points Pill
        Rectangle ptsPill = new(230, 15, 190, 40);
        CleanUI.DrawPanel(batch, ptsPill, UITheme.BgCardRecessed, UITheme.BorderSubtle, borderWidth: 1, shadow: false);
        batch.FillRectangle(new Rectangle(ptsPill.X + 10, ptsPill.Y + 10, 20, 20), UITheme.AccentCyan);
        batch.DrawString(_ctx.Font, $"{run.Economy.PlayerPoints} PTS", new Vector2(ptsPill.X + 40, ptsPill.Y + 8), UITheme.AccentCyan);

        // Day Number Badge
        Rectangle dayPill = new(600, 15, 160, 40);
        CleanUI.DrawBadge(batch, _ctx.Font, dayPill, $"DAY {run.DayNumber}", new Color(34, 42, 58), UITheme.AccentGold);

        // EXP Progress Bar
        Rectangle expBarBg = new(800, 18, 380, 34);
        float expRatio = Math.Clamp((float)run.ActivePet.CurrentExp / run.ActivePet.MaxExp, 0f, 1f);
        CleanUI.DrawProgressBar(batch, _ctx.Font, expBarBg, expRatio, UITheme.AccentEmerald, leftText: "EXP", rightText: $"LV. {run.ActivePet.Level}");

        // Top-Right: Energy Battery Pips
        int maxEnergy = run.Energy.MaxEnergy;
        int curEnergy = run.Energy.CurrentEnergy;
        int boltStartX = 1360;
        for (int i = 0; i < maxEnergy; i++)
        {
            Rectangle boltRect = new(boltStartX + (i * 44), 16, 34, 38);
            bool filled = i < curEnergy;
            Color boltBg = filled ? UITheme.AccentCyan : UITheme.BgCardRecessed;
            Color boltBorder = filled ? Color.White : UITheme.BorderSubtle;

            batch.FillRectangle(boltRect, boltBg);
            batch.DrawRectangle(boltRect, boltBorder, 1);

            Vector2 eSz = _ctx.Font.MeasureString("E");
            Color textCol = filled ? Color.Black : UITheme.TextMuted;
            batch.DrawString(_ctx.Font, "E", new Vector2(boltRect.Center.X - eSz.X / 2f, boltRect.Center.Y - eSz.Y / 2f), textCol);
        }
    }

    private void DrawInteractiveDoor(SpriteBatch batch, Point mPos)
    {
        bool hovered = _doorRect.Contains(mPos);
        Color doorBg = hovered ? new Color(42, 36, 32) : new Color(34, 28, 24);
        Color doorBorder = hovered ? UITheme.AccentGold : UITheme.BorderSubtle;

        CleanUI.DrawPanel(batch, _doorRect, doorBg, doorBorder, borderWidth: hovered ? 2 : 1, shadow: true);
        batch.DrawLine(_doorRect.Center.X, _doorRect.Y, _doorRect.Center.X, _doorRect.Bottom, UITheme.BorderSubtle, 2f);

        // Window panes
        Rectangle leftWindow = new(_doorRect.X + 24, _doorRect.Y + 50, 64, 120);
        Rectangle rightWindow = new(_doorRect.Right - 88, _doorRect.Y + 50, 64, 120);
        CleanUI.DrawPanel(batch, leftWindow, UITheme.BgCardRecessed, UITheme.BorderSubtle, borderWidth: 1, shadow: false);
        CleanUI.DrawPanel(batch, rightWindow, UITheme.BgCardRecessed, UITheme.BorderSubtle, borderWidth: 1, shadow: false);

        batch.FillRectangle(new Rectangle(_doorRect.Center.X - 16, _doorRect.Center.Y + 30, 8, 20), UITheme.AccentGold);
        batch.FillRectangle(new Rectangle(_doorRect.Center.X + 8, _doorRect.Center.Y + 30, 8, 20), UITheme.AccentGold);

        var run = _ctx.Run;
        bool hasKnockEvent = (run.DayNumber == 2 && !run.Day2EncounterResolved) ||
                             (run.DayNumber == 3 && !run.Day3BossDefeated);

        if (hasKnockEvent)
        {
            int bob = (int)(Math.Sin(_breathTimer * 2f) * 6f);
            Rectangle bubble = new(_doorRect.Center.X - 110, _doorRect.Y - 70 + bob, 220, 50);
            CleanUI.DrawPanel(batch, bubble, UITheme.BgPanelHover, UITheme.AccentCoral, borderWidth: 2, shadow: true);
            string knockText = "Knock Knock !!";
            Vector2 kSize = _ctx.Font.MeasureString(knockText);
            batch.DrawString(_ctx.Font, knockText, new Vector2(bubble.Center.X - kSize.X / 2f, bubble.Center.Y - kSize.Y / 2f), UITheme.AccentCoral);
        }
        else if (run.Energy.CurrentEnergy == 0)
        {
            int bob = (int)(Math.Sin(_breathTimer * 1.5f) * 4f);
            Rectangle endBubble = new(_doorRect.Center.X - 100, _doorRect.Y - 65 + bob, 200, 48);
            CleanUI.DrawPanel(batch, endBubble, new Color(24, 48, 72), UITheme.AccentCyan, borderWidth: 1, shadow: true);
            string endText = "[E] End Shift";
            Vector2 eSize = _ctx.Font.MeasureString(endText);
            batch.DrawString(_ctx.Font, endText, new Vector2(endBubble.Center.X - eSize.X / 2f, endBubble.Center.Y - eSize.Y / 2f), UITheme.TextPrimary);
        }
    }

    private void DrawUpgradeStation(SpriteBatch batch, Point mPos)
    {
        bool hovered = _upgradeStationRect.Contains(mPos);
        Color bg = hovered ? UITheme.BgPanelHover : UITheme.BgPanel;
        Color border = hovered ? UITheme.AccentGold : UITheme.BorderSubtle;

        CleanUI.DrawPanel(batch, _upgradeStationRect, bg, border, borderWidth: hovered ? 2 : 1, shadow: true);
        batch.FillRectangle(new Rectangle(_upgradeStationRect.X, _upgradeStationRect.Y, _upgradeStationRect.Width, 4), UITheme.AccentGold);

        Vector2 tSz = _ctx.Font.MeasureString("UPGRADES");
        batch.DrawString(_ctx.Font, "UPGRADES", new Vector2(_upgradeStationRect.Center.X - tSz.X / 2f, _upgradeStationRect.Y + 24), UITheme.AccentGold);

        Rectangle iconRect = new(_upgradeStationRect.Center.X - 35, _upgradeStationRect.Y + 80, 70, 60);
        CleanUI.DrawPanel(batch, iconRect, UITheme.BgCardRecessed, UITheme.BorderSubtle, borderWidth: 1, shadow: false);
        batch.DrawString(_ctx.Font, "[ + ]", new Vector2(iconRect.X + 16, iconRect.Y + 16), UITheme.AccentGold);
    }

    private void DrawSurvivalDesk(SpriteBatch batch, Point mPos)
    {
        bool hovered = _survivalDeskRect.Contains(mPos);
        Color bg = hovered ? UITheme.BgPanelHover : UITheme.BgPanel;
        Color border = hovered ? UITheme.AccentCyan : UITheme.BorderSubtle;

        CleanUI.DrawPanel(batch, _survivalDeskRect, bg, border, borderWidth: hovered ? 2 : 1, shadow: true);

        Rectangle bookRect = new(_survivalDeskRect.Center.X - 60, _survivalDeskRect.Y + 20, 120, 60);
        CleanUI.DrawPanel(batch, bookRect, new Color(24, 38, 54), UITheme.AccentCyan, borderWidth: 1, shadow: false);
        batch.DrawLine(bookRect.Center.X, bookRect.Y, bookRect.Center.X, bookRect.Bottom, UITheme.TextMuted, 1f);

        string bookLabel = "SURVIVAL LOG";
        Vector2 bSize = _ctx.Font.MeasureString(bookLabel);
        batch.DrawString(_ctx.Font, bookLabel, new Vector2(_survivalDeskRect.Center.X - bSize.X / 2f, _survivalDeskRect.Bottom - 36), hovered ? UITheme.TextPrimary : UITheme.TextSecondary);
    }

    private void DrawDoctorCharacter(SpriteBatch batch, Point mPos)
    {
        bool hovered = _doctorRect.Contains(mPos);
        Color bg = hovered ? UITheme.BgPanelHover : UITheme.BgPanel;
        Color border = hovered ? UITheme.AccentCoral : UITheme.BorderSubtle;

        CleanUI.DrawPanel(batch, _doctorRect, bg, border, borderWidth: hovered ? 2 : 1, shadow: true);

        // Medical Cross
        batch.FillRectangle(new Rectangle(_doctorRect.Center.X - 8, _doctorRect.Y + 24, 16, 44), UITheme.AccentCoral);
        batch.FillRectangle(new Rectangle(_doctorRect.Center.X - 22, _doctorRect.Y + 38, 44, 16), UITheme.AccentCoral);

        string docLabel = "MEDIC (500 G)";
        Vector2 dSize = _ctx.Font.MeasureString("MEDIC (500 G)");
        batch.DrawString(_ctx.Font, docLabel, new Vector2(_doctorRect.Center.X - dSize.X / 2f, _doctorRect.Y + 110), UITheme.TextSecondary);
    }

    private void DrawActivePet(SpriteBatch batch, Point mPos)
    {
        var pet = _ctx.Run.ActivePet;
        bool hovered = _petStageRect.Contains(mPos);

        int bobY = (int)(Math.Sin(_breathTimer) * 6f);
        int petH = _petStageRect.Height;
        int petW = (int)(petH * (1298f / 1731f)); // 210px (3:4 ratio)
        int petX = _petStageRect.Center.X - petW / 2;
        Rectangle petBox = new(petX, _petStageRect.Y + bobY, petW, petH);

        // Floor shadow ellipse
        batch.FillRectangle(new Rectangle(_petStageRect.X + 30, _petStageRect.Bottom - 20, _petStageRect.Width - 60, 16), Color.Black * 0.4f);

        if (_ctx.PetIdleTex != null)
        {
            batch.Draw(_ctx.PetIdleTex, petBox, Color.White);
        }
        else
        {
            batch.FillRectangle(petBox, new Color(75, 140, 80));
        }

        if (hovered)
        {
            batch.DrawRectangle(petBox, UITheme.AccentGold, 2);
            string prompt = "Click Pet for Care QTE!";
            Vector2 pSize = _ctx.Font.MeasureString(prompt);
            batch.DrawString(_ctx.Font, prompt, new Vector2(petBox.Center.X - pSize.X / 2f, petBox.Y - 36), UITheme.AccentGold);
        }

        // Pet Name tag
        string nameTag = $"{pet.Name} (LV. {pet.Level})";
        Vector2 nSize = _ctx.Font.MeasureString(nameTag);
        batch.DrawString(_ctx.Font, nameTag, new Vector2(petBox.Center.X - nSize.X / 2f, petBox.Bottom + 8), UITheme.TextPrimary);

        // Mini HP Bar
        Rectangle hpBg = new(petBox.Center.X - 80, petBox.Bottom + 36, 160, 22);
        CleanUI.DrawProgressBar(batch, _ctx.Font, hpBg, pet.Health / 100f, UITheme.AccentCoral, leftText: null, rightText: $"{pet.Health} HP");
    }

    private void DrawBottomBar(SpriteBatch batch, Point mPos)
    {
        // Quick Bag & Shop buttons only (Care shortcuts removed per requirements)
        CleanUI.DrawButton(batch, _ctx.Font, _shopBtn, "Shop", _shopBtn.Contains(mPos), accent: UITheme.AccentPurple, hotkey: "[ S ]");
        CleanUI.DrawButton(batch, _ctx.Font, _bagBtn, "Bag / Item", _bagBtn.Contains(mPos), accent: UITheme.AccentEmerald, hotkey: "[ B ]");
    }

    private void DrawDoctorModal(SpriteBatch batch, Point mPos)
    {
        CleanUI.DrawModalBackdrop(batch, _ctx.ScreenWidth, _ctx.ScreenHeight, alpha: 0.8f);

        Rectangle modal = new(560, 240, 800, 580);
        CleanUI.DrawPanel(batch, modal, UITheme.BgPanel, UITheme.BorderLight, borderWidth: 1, shadow: true);
        batch.FillRectangle(new Rectangle(modal.X, modal.Y, modal.Width, 4), UITheme.AccentCoral);

        batch.DrawString(_ctx.Font, "FACILITY MEDICAL STATION", new Vector2(modal.X + 40, modal.Y + 36), UITheme.AccentCoral, 0f, Vector2.Zero, 1.3f, SpriteEffects.None, 0f);
        batch.DrawLine(modal.X + 40, modal.Y + 80, modal.Right - 40, modal.Y + 80, UITheme.BorderSubtle, 1f);

        if (!_doctorInReviveStep)
        {
            string bubble = "\"Overseer, abnormal creatures require emergency revive if Health reaches zero.\nWould you like to review medical containment?\"";
            batch.DrawString(_ctx.Font, bubble, new Vector2(modal.X + 50, modal.Y + 140), UITheme.TextPrimary);

            CleanUI.DrawButton(batch, _ctx.Font, _doctorYesBtn, "YES", _doctorYesBtn.Contains(mPos), accent: UITheme.AccentEmerald, isPrimary: true);
            CleanUI.DrawButton(batch, _ctx.Font, _doctorNoBtn, "NO", _doctorNoBtn.Contains(mPos), accent: UITheme.AccentCoral);
        }
        else
        {
            string prompt = "Select Specimen to Revive (Fainted Pets at 0 HP)";
            batch.DrawString(_ctx.Font, prompt, new Vector2(modal.X + 50, modal.Y + 120), UITheme.AccentGold);

            var pet = _ctx.Run.ActivePet;
            if (pet.Health <= 0)
            {
                batch.DrawString(_ctx.Font, $"Fainted Specimen: {pet.Name} (0/100 HP)", new Vector2(modal.X + 50, modal.Y + 170), UITheme.AccentCoral);
                batch.DrawString(_ctx.Font, "Emergency Revive Cost: 500 G", new Vector2(modal.X + 50, modal.Y + 210), UITheme.TextSecondary);
                CleanUI.DrawButton(batch, _ctx.Font, _doctorReviveBtn, "REVIVE TO 1 HP (500 G)", _doctorReviveBtn.Contains(mPos), accent: UITheme.AccentCoral, isPrimary: true);
            }
            else
            {
                batch.DrawString(_ctx.Font, $"All specimens healthy! Active: {pet.Name} ({pet.Health}/100 HP)", new Vector2(modal.X + 50, modal.Y + 180), UITheme.AccentEmerald);
            }

            if (!string.IsNullOrEmpty(_doctorFeedback))
            {
                batch.DrawString(_ctx.Font, _doctorFeedback, new Vector2(modal.X + 50, modal.Y + 300), UITheme.AccentGold);
            }

            CleanUI.DrawButton(batch, _ctx.Font, _doctorCloseBtn, "CLOSE", _doctorCloseBtn.Contains(mPos), accent: UITheme.BorderSubtle, hotkey: "[ ESC ]");
        }
    }

    private void DrawUpgradeModal(SpriteBatch batch, Point mPos)
    {
        CleanUI.DrawModalBackdrop(batch, _ctx.ScreenWidth, _ctx.ScreenHeight, alpha: 0.8f);

        Rectangle modal = new(360, 160, 1200, 760);
        CleanUI.DrawPanel(batch, modal, UITheme.BgPanel, UITheme.BorderLight, borderWidth: 1, shadow: true);
        batch.FillRectangle(new Rectangle(modal.X, modal.Y, modal.Width, 4), UITheme.AccentGold);

        batch.DrawString(_ctx.Font, "SHELTER UPGRADE STATION", new Vector2(modal.X + 40, modal.Y + 32), UITheme.AccentGold, 0f, Vector2.Zero, 1.35f, SpriteEffects.None, 0f);
        // Header displays both Gold and Player Points
        string walletText = $"Gold: {_ctx.Run.Economy.Gold} G    |    Points: {_ctx.Run.Economy.PlayerPoints} PTS";
        Vector2 wSize = _ctx.Font.MeasureString(walletText);
        batch.DrawString(_ctx.Font, walletText, new Vector2(modal.Right - wSize.X - 40, modal.Y + 36), UITheme.AccentCyan);
        batch.DrawLine(modal.X + 40, modal.Y + 80, modal.Right - 40, modal.Y + 80, UITheme.BorderSubtle, 1f);

        // 3 Cards with generous width (320px) and clean padding
        DrawUpgradeCard(batch, _upgCard1, "QTE Upgrade", "Cost: 60 PTS\n\nWidens needle zones by\n+15% and slows rotation.", _upgCard1.Contains(mPos));
        DrawUpgradeCard(batch, _upgCard2, "Energy Upgrade", "Cost: 150 G + 80 PTS\n\nIncreases daily energy\nreserves by +2 AP.", _upgCard2.Contains(mPos));
        DrawUpgradeCard(batch, _upgCard3, "Progress Booster", "Cost: 70 PTS\n\nIncreases stat gains by\n+50% per care session.", _upgCard3.Contains(mPos));

        if (!string.IsNullOrEmpty(_upgradeFeedback))
        {
            batch.DrawString(_ctx.Font, _upgradeFeedback, new Vector2(modal.X + 50, modal.Bottom - 80), UITheme.AccentGold);
        }

        CleanUI.DrawButton(batch, _ctx.Font, _upgCloseBtn, "CLOSE", _upgCloseBtn.Contains(mPos), accent: UITheme.BorderSubtle, hotkey: "[ ESC ]");
    }

    private void DrawUpgradeCard(SpriteBatch batch, Rectangle card, string title, string description, bool hovered)
    {
        Color bg = hovered ? UITheme.BgPanelHover : UITheme.BgCardRecessed;
        Color border = hovered ? UITheme.AccentGold : UITheme.BorderSubtle;

        CleanUI.DrawPanel(batch, card, bg, border, borderWidth: hovered ? 2 : 1, shadow: false);

        Rectangle header = new(card.X, card.Y, card.Width, 48);
        CleanUI.DrawPanel(batch, header, UITheme.BgPanel, UITheme.BorderSubtle, borderWidth: 0, shadow: false);
        batch.DrawString(_ctx.Font, title, new Vector2(card.X + 20, card.Y + 14), UITheme.AccentGold, 0f, Vector2.Zero, 1.15f, SpriteEffects.None, 0f);

        string[] descLines = description.Split('\n');
        float dy = card.Y + 68;
        float lineScale = 0.90f;
        foreach (string line in descLines)
        {
            if (!string.IsNullOrEmpty(line))
            {
                batch.DrawString(_ctx.Font, line, new Vector2(card.X + 20, dy), UITheme.TextSecondary, 0f, Vector2.Zero, lineScale, SpriteEffects.None, 0f);
            }
            dy += _ctx.Font.LineSpacing * lineScale * 1.0f;
        }

        Rectangle btn = new(card.X + 20, card.Bottom - 58, card.Width - 40, 44);
        CleanUI.DrawButton(batch, _ctx.Font, btn, "UPGRADE", hovered, accent: UITheme.AccentGold, isPrimary: true);
    }

    private void DrawLogModal(SpriteBatch batch, Point mPos)
    {
        CleanUI.DrawModalBackdrop(batch, _ctx.ScreenWidth, _ctx.ScreenHeight, alpha: 0.8f);

        Rectangle modal = new(360, 140, 1200, 800);
        CleanUI.DrawPanel(batch, modal, UITheme.BgPanel, UITheme.BorderLight, borderWidth: 1, shadow: true);
        batch.FillRectangle(new Rectangle(modal.X, modal.Y, modal.Width, 4), UITheme.AccentCyan);

        // Header Title (Sized nicely to fit frame)
        batch.DrawString(_ctx.Font, "SURVIVAL LOGBOOK (Specimen Archives)", new Vector2(modal.X + 40, modal.Y + 30), UITheme.AccentCyan, 0f, Vector2.Zero, 1.25f, SpriteEffects.None, 0f);

        // Tabs
        CleanUI.DrawButton(batch, _ctx.Font, _tabPetDiscovery, "Pet Discovery", _tabPetDiscovery.Contains(mPos), accent: _logActiveTab == 0 ? UITheme.AccentCyan : null, isPrimary: _logActiveTab == 0);
        CleanUI.DrawButton(batch, _ctx.Font, _tabDisaster, "Disasters", _tabDisaster.Contains(mPos), accent: _logActiveTab == 1 ? UITheme.AccentCyan : null, isPrimary: _logActiveTab == 1);

        batch.DrawLine(modal.X + 40, 275, modal.Right - 40, 275, UITheme.BorderSubtle, 1f);

        if (_logActiveTab == 0)
        {
            var pet = _ctx.Run.ActivePet;
            Rectangle picRect = new(modal.X + 60, 310, 240, 240);
            CleanUI.DrawPanel(batch, picRect, UITheme.BgCardRecessed, UITheme.BorderSubtle, borderWidth: 1, shadow: false);
            if (_ctx.PetIdleTex != null)
            {
                int sprH = 200;
                int sprW = (int)(sprH * (1298f / 1731f)); // 150px
                Rectangle sprRect = new(picRect.Center.X - sprW / 2, picRect.Center.Y - sprH / 2, sprW, sprH);
                batch.Draw(_ctx.PetIdleTex, sprRect, Color.White);
            }

            int sx = modal.X + 340;
            // Clean separation: Name on row 1, Level on row 2 (NO OVERLAP!)
            batch.DrawString(_ctx.Font, $"NAME: {pet.Name}", new Vector2(sx, 310), UITheme.AccentGold, 0f, Vector2.Zero, 1.25f, SpriteEffects.None, 0f);
            batch.DrawString(_ctx.Font, $"LEVEL: {pet.Level}", new Vector2(sx, 350), UITheme.TextSecondary, 0f, Vector2.Zero, 1.15f, SpriteEffects.None, 0f);

            int barW = 480;
            CleanUI.DrawProgressBar(batch, _ctx.Font, new Rectangle(sx, 400, barW, 26), pet.Health / 100f, UITheme.AccentCoral, leftText: "HEALTH", rightText: $"{pet.Health}/100");
            CleanUI.DrawProgressBar(batch, _ctx.Font, new Rectangle(sx, 440, barW, 26), pet.Stomach / 100f, UITheme.AccentGold, leftText: "STOMACH", rightText: $"{pet.Stomach}%");
            CleanUI.DrawProgressBar(batch, _ctx.Font, new Rectangle(sx, 480, barW, 26), pet.Clean / 100f, UITheme.AccentCyan, leftText: "CLEANLINESS", rightText: $"{pet.Clean}%");

            batch.DrawString(_ctx.Font, "Abnormal Traits: Photosynthetic skin, elevated curiosity.", new Vector2(sx, 535), UITheme.TextPrimary);
            batch.DrawString(_ctx.Font, "Favorite Routine: Train in morning, Clean in evening.", new Vector2(sx, 575), UITheme.TextSecondary);
        }
        else
        {
            Rectangle picRect = new(modal.X + 60, 310, 240, 240);
            CleanUI.DrawPanel(batch, picRect, UITheme.BgCardRecessed, UITheme.BorderSubtle, borderWidth: 1, shadow: false);
            Vector2 stmSz = _ctx.Font.MeasureString("[ STORM ]");
            batch.DrawString(_ctx.Font, "[ STORM ]", new Vector2(picRect.Center.X - stmSz.X / 2f, picRect.Center.Y - stmSz.Y / 2f), UITheme.AccentGold);

            int sx = modal.X + 340;
            batch.DrawString(_ctx.Font, "DISASTER: THUNDER STORM", new Vector2(sx, 310), UITheme.AccentGold, 0f, Vector2.Zero, 1.3f, SpriteEffects.None, 0f);

            string desc = "Severe atmospheric storm inducing panic in abnormal pets.\n" +
                          "During the storm, creatures experience severe anxiety spikes.\n" +
                          "Failing to calm them before nightfall will inflict massive stress damage.";
            batch.DrawString(_ctx.Font, desc, new Vector2(sx, 360), UITheme.TextSecondary);

            batch.DrawString(_ctx.Font, "Countermeasure Protocol: Emergency Calming QTE at end of Day 1.", new Vector2(sx, 480), UITheme.AccentCyan);
        }

        CleanUI.DrawButton(batch, _ctx.Font, _logCloseBtn, "CLOSE", _logCloseBtn.Contains(mPos), accent: UITheme.BorderSubtle, hotkey: "[ ESC ]");
    }

    private void DrawDoorModal(SpriteBatch batch, Point mPos)
    {
        CleanUI.DrawModalBackdrop(batch, _ctx.ScreenWidth, _ctx.ScreenHeight, alpha: 0.8f);

        Rectangle modal = new(510, 220, 900, 640);
        CleanUI.DrawPanel(batch, modal, UITheme.BgPanel, UITheme.BorderLight, borderWidth: 1, shadow: true);
        batch.FillRectangle(new Rectangle(modal.X, modal.Y, modal.Width, 4), UITheme.AccentGold);

        if (_ctx.Run.DayNumber == 2)
        {
            batch.DrawString(_ctx.Font, "DAY 2 - MORNING ENCOUNTER: TOOTHLESS", new Vector2(modal.X + 40, modal.Y + 36), UITheme.AccentGold, 0f, Vector2.Zero, 1.3f, SpriteEffects.None, 0f);
            batch.DrawLine(modal.X + 40, modal.Y + 80, modal.Right - 40, modal.Y + 80, UITheme.BorderSubtle, 1f);

            string dialogue = "\"Arrrrrrhrhrhrhhrrhrhrha\"";
            Vector2 dSize = _ctx.Font.MeasureString(dialogue);
            batch.DrawString(_ctx.Font, dialogue, new Vector2(modal.Center.X - (dSize.X * 1.3f) / 2f, modal.Y + 150), UITheme.AccentEmerald, 0f, Vector2.Zero, 1.3f, SpriteEffects.None, 0f);

            string desc = "An acid-dripping wild beast stands outside your shelter door!\nWill you chase it away or attempt to tame it as your companion?";
            batch.DrawString(_ctx.Font, desc, new Vector2(modal.X + 80, modal.Y + 230), UITheme.TextSecondary);

            CleanUI.DrawButton(batch, _ctx.Font, _doorOption1Btn, "CHASE AWAY", _doorOption1Btn.Contains(mPos), accent: UITheme.AccentCoral);
            CleanUI.DrawButton(batch, _ctx.Font, _doorOption2Btn, "TAME CREATURE", _doorOption2Btn.Contains(mPos), accent: UITheme.AccentEmerald, isPrimary: true);
        }
        else if (_ctx.Run.DayNumber == 3)
        {
            batch.DrawString(_ctx.Font, "DAY 3 - MORNING ENCOUNTER: MERCHANT", new Vector2(modal.X + 40, modal.Y + 36), UITheme.AccentGold, 0f, Vector2.Zero, 1.3f, SpriteEffects.None, 0f);
            batch.DrawLine(modal.X + 40, modal.Y + 80, modal.Right - 40, modal.Y + 80, UITheme.BorderSubtle, 1f);

            string dialogue = "\"I'm quite interested in your toothless... will you sell?\"";
            Vector2 dSize = _ctx.Font.MeasureString(dialogue);
            batch.DrawString(_ctx.Font, dialogue, new Vector2(modal.Center.X - (dSize.X * 1.15f) / 2f, modal.Y + 150), UITheme.AccentGold, 0f, Vector2.Zero, 1.15f, SpriteEffects.None, 0f);

            string desc = "The shady collector offers an astronomical buyout for Toothless!\nIf you refuse, he will attack to confiscate your specimen!";
            batch.DrawString(_ctx.Font, desc, new Vector2(modal.X + 80, modal.Y + 230), UITheme.TextSecondary);

            CleanUI.DrawButton(batch, _ctx.Font, _doorOption1Btn, "ACCEPT BUYOUT", _doorOption1Btn.Contains(mPos), accent: UITheme.AccentGold);
            CleanUI.DrawButton(batch, _ctx.Font, _doorOption2Btn, "REFUSE / FIGHT", _doorOption2Btn.Contains(mPos), accent: UITheme.AccentCoral, isPrimary: true);
        }
    }
}
