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

    // Room Interactive Zones
    private readonly Rectangle _doorRect = new(540, 190, 200, 340);
    private readonly Rectangle _upgradeStationRect = new(60, 300, 140, 130);
    private readonly Rectangle _survivalDeskRect = new(220, 430, 150, 90);
    private readonly Rectangle _doctorRect = new(70, 460, 110, 130);
    private readonly Rectangle _petStageRect = new(840, 390, 220, 200);

    // Bottom action buttons (Quick Bag & Shop)
    private readonly Rectangle _bagBtn = new(1060, 650, 180, 46);
    private readonly Rectangle _shopBtn = new(860, 650, 180, 46);

    // Shortcut care buttons along bottom-left
    private readonly Rectangle _trainShortcut = new(50, 650, 145, 46);
    private readonly Rectangle _feedShortcut = new(210, 650, 145, 46);
    private readonly Rectangle _cleanShortcut = new(370, 650, 145, 46);
    private readonly Rectangle _healShortcut = new(530, 650, 145, 46);

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
    private readonly Rectangle _doctorYesBtn = new(450, 410, 160, 46);
    private readonly Rectangle _doctorNoBtn = new(670, 410, 160, 46);
    private readonly Rectangle _doctorReviveBtn = new(490, 450, 300, 50);
    private readonly Rectangle _doctorCloseBtn = new(560, 530, 160, 42);

    // Upgrade cards
    private readonly Rectangle _upgCard1 = new(230, 220, 250, 320);
    private readonly Rectangle _upgCard2 = new(515, 220, 250, 320);
    private readonly Rectangle _upgCard3 = new(800, 220, 250, 320);
    private readonly Rectangle _upgCloseBtn = new(560, 570, 160, 44);

    // Survival log tabs & close
    private readonly Rectangle _tabPetDiscovery = new(220, 160, 190, 42);
    private readonly Rectangle _tabDisaster = new(430, 160, 190, 42);
    private readonly Rectangle _logCloseBtn = new(560, 580, 160, 44);

    // Door event choices
    private readonly Rectangle _doorOption1Btn = new(430, 470, 190, 50);
    private readonly Rectangle _doorOption2Btn = new(660, 470, 190, 50);

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
        if (run.CurrentPhase != DailyPhase.MorningEvent) return;

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
                break;
            case 3:
                break;
        }
    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _breathTimer += dt * 3f;

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

        // Modal Handling
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
                else if (_doctorNoBtn.Contains(mPos))
                {
                    _showDoctorModal = false;
                    _ctx.Audio.PlayConfirm();
                }
            }
            else
            {
                var pet = _ctx.Run.ActivePet;
                if (pet.Health <= 0 && _doctorReviveBtn.Contains(mPos))
                {
                    if (_ctx.Run.Economy.SpendGold(500))
                    {
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
            if (_upgCard1.Contains(mPos))
            {
                if (_ctx.Run.Economy.SpendGold(200))
                {
                    _upgradeFeedback = "Purchased QTE Upgrade (+15% needle zone)!";
                    _ctx.Audio.PlaySuccess();
                }
                else
                {
                    _upgradeFeedback = "Not enough Gold! Needs 200 G.";
                    _ctx.Audio.PlayWarning();
                }
            }
            else if (_upgCard2.Contains(mPos))
            {
                if (_ctx.Run.Economy.SpendGold(300))
                {
                    _ctx.Run.Energy.AddBonus(2);
                    _upgradeFeedback = "Purchased Energy Upgrade (+2 AP)!";
                    _ctx.Audio.PlaySuccess();
                }
                else
                {
                    _upgradeFeedback = "Not enough Gold! Needs 300 G.";
                    _ctx.Audio.PlayWarning();
                }
            }
            else if (_upgCard3.Contains(mPos))
            {
                if (_ctx.Run.Economy.SpendGold(250))
                {
                    _upgradeFeedback = "Purchased Care Booster (+50% Stat Gains)!";
                    _ctx.Audio.PlaySuccess();
                }
                else
                {
                    _upgradeFeedback = "Not enough Gold! Needs 250 G.";
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
                // Day 2 Toothless: Option 1 = Chase, Option 2 = Tame
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
                // Day 3 Merchant: Option 1 = Yes (Sell), Option 2 = No (Fight)
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
        bool energyDepleted = run.Energy.CurrentEnergy == 0;

        // Press E for door action or End Day (NewGDD.txt)
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

        // Shortcut Keys: 1=Train, 2=Feed, 3=Clean, 4=Heal
        if (kbd.IsKeyDown(Keys.D1) && !_prevKeyboard.IsKeyDown(Keys.D1)) TryCareAction(CareActionType.Train);
        else if (kbd.IsKeyDown(Keys.D2) && !_prevKeyboard.IsKeyDown(Keys.D2)) TryCareAction(CareActionType.Feed);
        else if (kbd.IsKeyDown(Keys.D3) && !_prevKeyboard.IsKeyDown(Keys.D3)) TryCareAction(CareActionType.Clean);
        else if (kbd.IsKeyDown(Keys.D4) && !_prevKeyboard.IsKeyDown(Keys.D4)) TryCareAction(CareActionType.Heal);
        else if (kbd.IsKeyDown(Keys.B) && !_prevKeyboard.IsKeyDown(Keys.B)) OpenBag();
        else if (kbd.IsKeyDown(Keys.S) && !_prevKeyboard.IsKeyDown(Keys.S)) OpenShop();

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
            // Active Pet -> Care QTE
            else if (_petStageRect.Contains(mPos))
            {
                TryCareAction(CareActionType.Train);
            }
            // Quick Buttons
            else if (_bagBtn.Contains(mPos)) OpenBag();
            else if (_shopBtn.Contains(mPos)) OpenShop();
            // Care shortcuts
            else if (_trainShortcut.Contains(mPos)) TryCareAction(CareActionType.Train);
            else if (_feedShortcut.Contains(mPos)) TryCareAction(CareActionType.Feed);
            else if (_cleanShortcut.Contains(mPos)) TryCareAction(CareActionType.Clean);
            else if (_healShortcut.Contains(mPos)) TryCareAction(CareActionType.Heal);
        }
    }

    private void TryCareAction(CareActionType action)
    {
        if (_ctx.Run.Energy.Spend(1))
        {
            _ctx.Audio.PlayConfirm();
            _ctx.ScreenManager.SetScreen(new CareQteScreen(_ctx, action));
        }
        else
        {
            _ctx.Audio.PlayWarning();
        }
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

        // 1. Back Wall
        batch.FillRectangle(new Rectangle(0, 0, _ctx.ScreenWidth, 530), UITheme.BgDeep);

        // Subtle vertical room seams
        Color seamColor = new(24, 28, 38, 120);
        for (int x = 160; x < _ctx.ScreenWidth; x += 180)
        {
            batch.DrawLine(x, 56, x, 530, seamColor, 1);
        }

        // 2. Hardwood Floor
        Color floorBase = new(28, 24, 22);
        batch.FillRectangle(new Rectangle(0, 530, _ctx.ScreenWidth, 190), floorBase);
        batch.DrawLine(0, 530, _ctx.ScreenWidth, 530, UITheme.BorderSubtle, 2f);

        Color plankColor = new(38, 32, 28);
        for (int y = 530; y <= 720; y += 38)
        {
            batch.DrawLine(0, y, _ctx.ScreenWidth, y, plankColor, 1f);
        }

        // 3. Interactive Room Elements
        DrawInteractiveDoor(batch, mPos);
        DrawUpgradeStation(batch, mPos);
        DrawSurvivalDesk(batch, mPos);
        DrawDoctorCharacter(batch, mPos);
        DrawActivePet(batch, mPos);

        // 4. Bottom Controls & Shortcuts
        DrawBottomBar(batch, mPos);

        // 5. Top Bar HUD
        DrawTopBar(batch);

        // 6. Dialogue Box
        if (_dialogue.IsActive)
        {
            _dialogue.Draw(batch, _ctx.Font, _ctx.Pixel, new Rectangle(140, 480, 1000, 180));
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
        Rectangle topBar = new(0, 0, _ctx.ScreenWidth, 54);
        CleanUI.DrawPanel(batch, topBar, UITheme.BgPanel, UITheme.BorderSubtle, borderWidth: 1, shadow: true);

        // Top-Left: Gold Coin + Amount
        Rectangle goldPill = new(24, 12, 130, 30);
        CleanUI.DrawPanel(batch, goldPill, UITheme.BgCardRecessed, UITheme.BorderSubtle, borderWidth: 1, shadow: false);
        // Coin icon dot
        batch.FillRectangle(new Rectangle(goldPill.X + 8, goldPill.Y + 7, 16, 16), UITheme.AccentGold);
        batch.DrawString(_ctx.Font, $"{run.Economy.Gold} G", new Vector2(goldPill.X + 32, goldPill.Y + 5), UITheme.AccentGold);

        // Top-Center: Day X Pill
        Rectangle dayPill = new(400, 12, 120, 30);
        CleanUI.DrawBadge(batch, _ctx.Font, dayPill, $"DAY {run.DayNumber}", new Color(34, 42, 58), UITheme.AccentGold);

        // Center-Right: Player EXP Bar
        Rectangle expBarBg = new(540, 16, 260, 22);
        CleanUI.DrawProgressBar(batch, _ctx.Font, expBarBg, 0.45f, UITheme.AccentEmerald, leftText: "EXP", rightText: "LV. 1");

        // Top-Right: Energy Battery Pips
        int maxEnergy = run.Energy.MaxEnergy;
        int curEnergy = run.Energy.CurrentEnergy;
        int boltStartX = 840;
        for (int i = 0; i < maxEnergy; i++)
        {
            Rectangle boltRect = new(boltStartX + (i * 32), 14, 24, 26);
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

        // Double door seam
        batch.DrawLine(_doorRect.Center.X, _doorRect.Y, _doorRect.Center.X, _doorRect.Bottom, UITheme.BorderSubtle, 2f);

        // Window panes
        Rectangle leftWindow = new(_doorRect.X + 24, _doorRect.Y + 40, 64, 90);
        Rectangle rightWindow = new(_doorRect.Right - 88, _doorRect.Y + 40, 64, 90);
        CleanUI.DrawPanel(batch, leftWindow, UITheme.BgCardRecessed, UITheme.BorderSubtle, borderWidth: 1, shadow: false);
        CleanUI.DrawPanel(batch, rightWindow, UITheme.BgCardRecessed, UITheme.BorderSubtle, borderWidth: 1, shadow: false);

        // Door handles
        batch.FillRectangle(new Rectangle(_doorRect.Center.X - 16, _doorRect.Center.Y + 25, 8, 16), UITheme.AccentGold);
        batch.FillRectangle(new Rectangle(_doorRect.Center.X + 8, _doorRect.Center.Y + 25, 8, 16), UITheme.AccentGold);

        // Event Prompts
        var run = _ctx.Run;
        bool hasKnock = (run.DayNumber == 2 && !run.Day2EncounterResolved) ||
                        (run.DayNumber == 3 && !run.Day3BossDefeated);
        if (hasKnock)
        {
            // Comic-style Knock Knock bubble with subtle floating bob
            int bob = (int)(Math.Sin(_breathTimer * 1.5f) * 3f);
            Rectangle bubble = new(_doorRect.Center.X - 100, _doorRect.Y - 60 + bob, 200, 46);
            CleanUI.DrawPanel(batch, bubble, UITheme.BgPanelHover, UITheme.AccentCoral, borderWidth: 2, shadow: true);

            string knockText = "Knock Knock !!";
            Vector2 kSize = _ctx.Font.MeasureString(knockText);
            batch.DrawString(_ctx.Font, knockText, new Vector2(bubble.Center.X - kSize.X / 2f, bubble.Center.Y - kSize.Y / 2f), UITheme.AccentCoral);
        }
        else if (run.Energy.CurrentEnergy == 0)
        {
            // [E] End Day prompt
            int bob = (int)(Math.Sin(_breathTimer * 1.5f) * 3f);
            Rectangle endBubble = new(_doorRect.Center.X - 90, _doorRect.Y - 55 + bob, 180, 42);
            CleanUI.DrawPanel(batch, endBubble, new Color(24, 48, 72), UITheme.AccentCyan, borderWidth: 1, shadow: true);

            string endText = "[E] End Day";
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
        batch.FillRectangle(new Rectangle(_upgradeStationRect.X, _upgradeStationRect.Y, _upgradeStationRect.Width, 3), UITheme.AccentGold);

        Vector2 tSz = _ctx.Font.MeasureString("UPGRADES");
        batch.DrawString(_ctx.Font, "UPGRADES", new Vector2(_upgradeStationRect.Center.X - tSz.X / 2f, _upgradeStationRect.Y + 16), UITheme.AccentGold);

        // Stylized wrench / gear outline
        Rectangle iconRect = new(_upgradeStationRect.Center.X - 25, _upgradeStationRect.Y + 54, 50, 44);
        CleanUI.DrawPanel(batch, iconRect, UITheme.BgCardRecessed, UITheme.BorderSubtle, borderWidth: 1, shadow: false);
        batch.DrawString(_ctx.Font, "[ + ]", new Vector2(iconRect.X + 11, iconRect.Y + 12), UITheme.AccentGold);
    }

    private void DrawSurvivalDesk(SpriteBatch batch, Point mPos)
    {
        bool hovered = _survivalDeskRect.Contains(mPos);
        Color bg = hovered ? UITheme.BgPanelHover : UITheme.BgPanel;
        Color border = hovered ? UITheme.AccentCyan : UITheme.BorderSubtle;

        CleanUI.DrawPanel(batch, _survivalDeskRect, bg, border, borderWidth: hovered ? 2 : 1, shadow: true);

        // Book graphic on desk
        Rectangle bookRect = new(_survivalDeskRect.Center.X - 44, _survivalDeskRect.Y + 14, 88, 48);
        CleanUI.DrawPanel(batch, bookRect, new Color(24, 38, 54), UITheme.AccentCyan, borderWidth: 1, shadow: false);
        batch.DrawLine(bookRect.Center.X, bookRect.Y, bookRect.Center.X, bookRect.Bottom, UITheme.TextMuted, 1f);

        string bookLabel = "SURVIVAL LOG";
        Vector2 bSize = _ctx.Font.MeasureString(bookLabel);
        batch.DrawString(_ctx.Font, bookLabel, new Vector2(_survivalDeskRect.Center.X - bSize.X / 2f, _survivalDeskRect.Bottom - 24), hovered ? UITheme.TextPrimary : UITheme.TextSecondary);
    }

    private void DrawDoctorCharacter(SpriteBatch batch, Point mPos)
    {
        bool hovered = _doctorRect.Contains(mPos);
        Color bg = hovered ? UITheme.BgPanelHover : UITheme.BgPanel;
        Color border = hovered ? UITheme.AccentCoral : UITheme.BorderSubtle;

        CleanUI.DrawPanel(batch, _doctorRect, bg, border, borderWidth: hovered ? 2 : 1, shadow: true);

        // Medical Cross
        batch.FillRectangle(new Rectangle(_doctorRect.Center.X - 5, _doctorRect.Y + 16, 10, 32), UITheme.AccentCoral);
        batch.FillRectangle(new Rectangle(_doctorRect.Center.X - 16, _doctorRect.Y + 27, 32, 10), UITheme.AccentCoral);

        string docLabel = "MEDIC\n(Revive)";
        Vector2 dSize = _ctx.Font.MeasureString("MEDIC");
        batch.DrawString(_ctx.Font, docLabel, new Vector2(_doctorRect.Center.X - dSize.X / 2f, _doctorRect.Y + 62), UITheme.TextSecondary);
    }

    private void DrawActivePet(SpriteBatch batch, Point mPos)
    {
        var pet = _ctx.Run.ActivePet;
        bool hovered = _petStageRect.Contains(mPos);

        // Breathing motion
        int bobY = (int)(Math.Sin(_breathTimer) * 4f);
        Rectangle petBox = new(_petStageRect.X, _petStageRect.Y + bobY, _petStageRect.Width, _petStageRect.Height);

        // Floor shadow ellipse
        batch.FillRectangle(new Rectangle(_petStageRect.X + 20, _petStageRect.Bottom - 16, _petStageRect.Width - 40, 12), Color.Black * 0.4f);

        // Pet Texture
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
            string prompt = "Click to Care QTE!";
            Vector2 pSize = _ctx.Font.MeasureString(prompt);
            batch.DrawString(_ctx.Font, prompt, new Vector2(petBox.Center.X - pSize.X / 2f, petBox.Y - 26), UITheme.AccentGold);
        }

        // Pet Name tag
        string nameTag = $"{pet.Name} (LV. {pet.Level})";
        Vector2 nSize = _ctx.Font.MeasureString(nameTag);
        batch.DrawString(_ctx.Font, nameTag, new Vector2(petBox.Center.X - nSize.X / 2f, petBox.Bottom + 4), UITheme.TextPrimary);

        // Mini HP Bar
        Rectangle hpBg = new(petBox.Center.X - 60, petBox.Bottom + 26, 120, 14);
        CleanUI.DrawProgressBar(batch, _ctx.Font, hpBg, pet.Health / 100f, UITheme.AccentCoral, leftText: null, rightText: $"{pet.Health} HP");
    }

    private void DrawBottomBar(SpriteBatch batch, Point mPos)
    {
        // Shortcut action buttons
        CleanUI.DrawButton(batch, _ctx.Font, _trainShortcut, "Train", _trainShortcut.Contains(mPos), accent: UITheme.AccentGold, hotkey: "[ 1 ]");
        CleanUI.DrawButton(batch, _ctx.Font, _feedShortcut, "Feed", _feedShortcut.Contains(mPos), accent: UITheme.AccentGold, hotkey: "[ 2 ]");
        CleanUI.DrawButton(batch, _ctx.Font, _cleanShortcut, "Clean", _cleanShortcut.Contains(mPos), accent: UITheme.AccentCyan, hotkey: "[ 3 ]");
        CleanUI.DrawButton(batch, _ctx.Font, _healShortcut, "Heal", _healShortcut.Contains(mPos), accent: UITheme.AccentEmerald, hotkey: "[ 4 ]");

        // Quick Bag & Shop buttons
        CleanUI.DrawButton(batch, _ctx.Font, _shopBtn, "Shop", _shopBtn.Contains(mPos), accent: UITheme.AccentPurple, hotkey: "[ S ]");
        CleanUI.DrawButton(batch, _ctx.Font, _bagBtn, "Bag / Item", _bagBtn.Contains(mPos), accent: UITheme.AccentEmerald, hotkey: "[ B ]");
    }

    private void DrawDoctorModal(SpriteBatch batch, Point mPos)
    {
        CleanUI.DrawModalBackdrop(batch, _ctx.ScreenWidth, _ctx.ScreenHeight, alpha: 0.75f);

        Rectangle modal = new(340, 170, 600, 420);
        CleanUI.DrawPanel(batch, modal, UITheme.BgPanel, UITheme.BorderLight, borderWidth: 1, shadow: true);
        batch.FillRectangle(new Rectangle(modal.X, modal.Y, modal.Width, 3), UITheme.AccentCoral);

        batch.DrawString(_ctx.Font, "FACILITY MEDICAL STATION", new Vector2(modal.X + 30, modal.Y + 24), UITheme.AccentCoral, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0f);
        batch.DrawLine(modal.X + 30, modal.Y + 56, modal.Right - 30, modal.Y + 56, UITheme.BorderSubtle, 1f);

        if (!_doctorInReviveStep)
        {
            string bubble = "\"Does your specimen require medical revival?\"";
            Vector2 bSize = _ctx.Font.MeasureString(bubble);
            batch.DrawString(_ctx.Font, bubble, new Vector2(modal.Center.X - bSize.X / 2f, modal.Y + 130), UITheme.TextPrimary);

            CleanUI.DrawButton(batch, _ctx.Font, _doctorYesBtn, "YES", _doctorYesBtn.Contains(mPos), accent: UITheme.AccentEmerald, isPrimary: true);
            CleanUI.DrawButton(batch, _ctx.Font, _doctorNoBtn, "NO", _doctorNoBtn.Contains(mPos), accent: UITheme.AccentCoral);
        }
        else
        {
            string prompt = "Select Specimen to Revive (Fainted Pets at 0 HP)";
            batch.DrawString(_ctx.Font, prompt, new Vector2(modal.X + 40, modal.Y + 80), UITheme.AccentGold);

            var pet = _ctx.Run.ActivePet;
            if (pet.Health <= 0)
            {
                batch.DrawString(_ctx.Font, $"Fainted Specimen: {pet.Name} (0/100 HP)", new Vector2(modal.X + 40, modal.Y + 120), UITheme.AccentCoral);
                batch.DrawString(_ctx.Font, "Emergency Revive Cost: 500 G", new Vector2(modal.X + 40, modal.Y + 150), UITheme.TextSecondary);

                CleanUI.DrawButton(batch, _ctx.Font, _doctorReviveBtn, "REVIVE TO 1 HP (500 G)", _doctorReviveBtn.Contains(mPos), accent: UITheme.AccentCoral, isPrimary: true);
            }
            else
            {
                batch.DrawString(_ctx.Font, $"All specimens healthy! Active: {pet.Name} ({pet.Health}/100 HP)", new Vector2(modal.X + 40, modal.Y + 140), UITheme.AccentEmerald);
            }

            if (!string.IsNullOrEmpty(_doctorFeedback))
            {
                batch.DrawString(_ctx.Font, _doctorFeedback, new Vector2(modal.X + 40, modal.Y + 220), UITheme.AccentGold);
            }

            CleanUI.DrawButton(batch, _ctx.Font, _doctorCloseBtn, "CLOSE", _doctorCloseBtn.Contains(mPos), accent: UITheme.BorderSubtle, hotkey: "[ ESC ]");
        }
    }

    private void DrawUpgradeModal(SpriteBatch batch, Point mPos)
    {
        CleanUI.DrawModalBackdrop(batch, _ctx.ScreenWidth, _ctx.ScreenHeight, alpha: 0.75f);

        Rectangle modal = new(180, 110, 920, 530);
        CleanUI.DrawPanel(batch, modal, UITheme.BgPanel, UITheme.BorderLight, borderWidth: 1, shadow: true);
        batch.FillRectangle(new Rectangle(modal.X, modal.Y, modal.Width, 3), UITheme.AccentGold);

        batch.DrawString(_ctx.Font, "SHELTER UPGRADE STATION", new Vector2(modal.X + 30, modal.Y + 24), UITheme.AccentGold, 0f, Vector2.Zero, 1.3f, SpriteEffects.None, 0f);
        batch.DrawString(_ctx.Font, $"Gold: {_ctx.Run.Economy.Gold} G", new Vector2(modal.Right - 160, modal.Y + 28), UITheme.AccentGold);
        batch.DrawLine(modal.X + 30, modal.Y + 62, modal.Right - 30, modal.Y + 62, UITheme.BorderSubtle, 1f);

        // 3 Cards
        DrawUpgradeCard(batch, _upgCard1, "QTE Upgrade", "Cost: 200 G\n\nWidens needle hit zones by +15% and slows rotation speed.", _upgCard1.Contains(mPos));
        DrawUpgradeCard(batch, _upgCard2, "Energy Upgrade", "Cost: 300 G\n\nIncreases daily energy reserves by +2 AP.", _upgCard2.Contains(mPos));
        DrawUpgradeCard(batch, _upgCard3, "Progress Bar Upgrade", "Cost: 250 G\n\nIncreases stat gains by +50% per successful care session.", _upgCard3.Contains(mPos));

        if (!string.IsNullOrEmpty(_upgradeFeedback))
        {
            batch.DrawString(_ctx.Font, _upgradeFeedback, new Vector2(modal.X + 40, modal.Bottom - 95), UITheme.AccentGold);
        }

        CleanUI.DrawButton(batch, _ctx.Font, _upgCloseBtn, "CLOSE", _upgCloseBtn.Contains(mPos), accent: UITheme.BorderSubtle, hotkey: "[ ESC ]");
    }

    private void DrawUpgradeCard(SpriteBatch batch, Rectangle card, string title, string description, bool hovered)
    {
        Color bg = hovered ? UITheme.BgPanelHover : UITheme.BgCardRecessed;
        Color border = hovered ? UITheme.AccentGold : UITheme.BorderSubtle;

        CleanUI.DrawPanel(batch, card, bg, border, borderWidth: hovered ? 2 : 1, shadow: false);

        Rectangle header = new(card.X, card.Y, card.Width, 38);
        CleanUI.DrawPanel(batch, header, UITheme.BgPanel, UITheme.BorderSubtle, borderWidth: 0, shadow: false);
        batch.DrawString(_ctx.Font, title, new Vector2(card.X + 16, card.Y + 10), UITheme.AccentGold);

        batch.DrawString(_ctx.Font, description, new Vector2(card.X + 16, card.Y + 54), UITheme.TextSecondary);

        Rectangle btn = new(card.X + 16, card.Bottom - 48, card.Width - 32, 34);
        CleanUI.DrawButton(batch, _ctx.Font, btn, "UPGRADE", hovered, accent: UITheme.AccentGold, isPrimary: true);
    }

    private void DrawLogModal(SpriteBatch batch, Point mPos)
    {
        CleanUI.DrawModalBackdrop(batch, _ctx.ScreenWidth, _ctx.ScreenHeight, alpha: 0.75f);

        Rectangle modal = new(180, 90, 920, 560);
        CleanUI.DrawPanel(batch, modal, UITheme.BgPanel, UITheme.BorderLight, borderWidth: 1, shadow: true);
        batch.FillRectangle(new Rectangle(modal.X, modal.Y, modal.Width, 3), UITheme.AccentCyan);

        // Header
        batch.DrawString(_ctx.Font, "SURVIVAL LOGBOOK (Specimen Archives)", new Vector2(modal.X + 30, modal.Y + 20), UITheme.AccentCyan, 0f, Vector2.Zero, 1.3f, SpriteEffects.None, 0f);

        // Tabs
        CleanUI.DrawButton(batch, _ctx.Font, _tabPetDiscovery, "Pet Discovery", _tabPetDiscovery.Contains(mPos), accent: _logActiveTab == 0 ? UITheme.AccentCyan : null, isPrimary: _logActiveTab == 0);
        CleanUI.DrawButton(batch, _ctx.Font, _tabDisaster, "Disasters", _tabDisaster.Contains(mPos), accent: _logActiveTab == 1 ? UITheme.AccentCyan : null, isPrimary: _logActiveTab == 1);

        batch.DrawLine(modal.X + 30, 212, modal.Right - 30, 212, UITheme.BorderSubtle, 1f);

        if (_logActiveTab == 0)
        {
            // Pet Discovery Tab
            var pet = _ctx.Run.ActivePet;
            Rectangle picRect = new(modal.X + 40, 240, 160, 160);
            CleanUI.DrawPanel(batch, picRect, UITheme.BgCardRecessed, UITheme.BorderSubtle, borderWidth: 1, shadow: false);
            if (_ctx.PetIdleTex != null) batch.Draw(_ctx.PetIdleTex, picRect, Color.White);

            int sx = modal.X + 230;
            batch.DrawString(_ctx.Font, $"NAME: {pet.Name}", new Vector2(sx, 240), UITheme.AccentGold, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0f);
            batch.DrawString(_ctx.Font, $"LV. {pet.Level}", new Vector2(sx + 240, 240), UITheme.TextSecondary, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0f);

            int barW = 320;
            CleanUI.DrawProgressBar(batch, _ctx.Font, new Rectangle(sx, 280, barW, 20), pet.Health / 100f, UITheme.AccentCoral, leftText: "HEALTH", rightText: $"{pet.Health}/100");
            CleanUI.DrawProgressBar(batch, _ctx.Font, new Rectangle(sx, 310, barW, 20), pet.Stomach / 100f, UITheme.AccentGold, leftText: "STOMACH", rightText: $"{pet.Stomach}%");
            CleanUI.DrawProgressBar(batch, _ctx.Font, new Rectangle(sx, 340, barW, 20), pet.Clean / 100f, UITheme.AccentCyan, leftText: "CLEANLINESS", rightText: $"{pet.Clean}%");

            batch.DrawString(_ctx.Font, "Abnormal Traits: Photosynthetic skin, elevated curiosity.", new Vector2(sx, 385), UITheme.TextPrimary);
            batch.DrawString(_ctx.Font, "Favorite Routine: Train in morning, Clean in evening.", new Vector2(sx, 415), UITheme.TextSecondary);
        }
        else
        {
            // Disaster Tab
            Rectangle picRect = new(modal.X + 40, 240, 180, 160);
            CleanUI.DrawPanel(batch, picRect, UITheme.BgCardRecessed, UITheme.BorderSubtle, borderWidth: 1, shadow: false);
            Vector2 stmSz = _ctx.Font.MeasureString("[ STORM ]");
            batch.DrawString(_ctx.Font, "[ STORM ]", new Vector2(picRect.Center.X - stmSz.X / 2f, picRect.Center.Y - stmSz.Y / 2f), UITheme.AccentGold);

            int sx = modal.X + 250;
            batch.DrawString(_ctx.Font, "DISASTER: THUNDER STORM", new Vector2(sx, 240), UITheme.AccentGold, 0f, Vector2.Zero, 1.3f, SpriteEffects.None, 0f);

            string desc = "Severe atmospheric storm inducing panic in abnormal pets.\n" +
                          "During the storm, abnormal creatures experience severe anxiety spikes.\n" +
                          "Failing to calm them before nightfall will inflict massive stress damage.";
            batch.DrawString(_ctx.Font, desc, new Vector2(sx, 285), UITheme.TextSecondary);

            batch.DrawString(_ctx.Font, "Countermeasure Protocol: Emergency Calming QTE at end of Day 1.", new Vector2(sx, 385), UITheme.AccentCyan);
        }

        CleanUI.DrawButton(batch, _ctx.Font, _logCloseBtn, "CLOSE", _logCloseBtn.Contains(mPos), accent: UITheme.BorderSubtle, hotkey: "[ ESC ]");
    }

    private void DrawDoorModal(SpriteBatch batch, Point mPos)
    {
        CleanUI.DrawModalBackdrop(batch, _ctx.ScreenWidth, _ctx.ScreenHeight, alpha: 0.75f);

        Rectangle modal = new(280, 130, 720, 460);
        CleanUI.DrawPanel(batch, modal, UITheme.BgPanel, UITheme.BorderLight, borderWidth: 1, shadow: true);
        batch.FillRectangle(new Rectangle(modal.X, modal.Y, modal.Width, 3), UITheme.AccentGold);

        if (_ctx.Run.DayNumber == 2)
        {
            batch.DrawString(_ctx.Font, "DAY 2 - MORNING ENCOUNTER: TOOTHLESS", new Vector2(modal.X + 30, modal.Y + 24), UITheme.AccentGold, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0f);
            batch.DrawLine(modal.X + 30, modal.Y + 58, modal.Right - 30, modal.Y + 58, UITheme.BorderSubtle, 1f);

            string dialogue = "\"Arrrrrrhrhrhrhhrrhrhrha\"";
            Vector2 dSize = _ctx.Font.MeasureString(dialogue);
            batch.DrawString(_ctx.Font, dialogue, new Vector2(modal.Center.X - (dSize.X * 1.3f) / 2f, modal.Y + 110), UITheme.AccentEmerald, 0f, Vector2.Zero, 1.3f, SpriteEffects.None, 0f);

            string desc = "An acid-dripping wild beast stands outside your shelter door!\nWill you chase it away or attempt to tame it as your companion?";
            batch.DrawString(_ctx.Font, desc, new Vector2(modal.X + 50, modal.Y + 170), UITheme.TextSecondary);

            CleanUI.DrawButton(batch, _ctx.Font, _doorOption1Btn, "CHASE AWAY", _doorOption1Btn.Contains(mPos), accent: UITheme.AccentCoral);
            CleanUI.DrawButton(batch, _ctx.Font, _doorOption2Btn, "TAME CREATURE", _doorOption2Btn.Contains(mPos), accent: UITheme.AccentEmerald, isPrimary: true);
        }
        else if (_ctx.Run.DayNumber == 3)
        {
            batch.DrawString(_ctx.Font, "DAY 3 - MORNING ENCOUNTER: MERCHANT", new Vector2(modal.X + 30, modal.Y + 24), UITheme.AccentGold, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0f);
            batch.DrawLine(modal.X + 30, modal.Y + 58, modal.Right - 30, modal.Y + 58, UITheme.BorderSubtle, 1f);

            string dialogue = "\"I'm quite interested in your toothless... will you sell?\"";
            Vector2 dSize = _ctx.Font.MeasureString(dialogue);
            batch.DrawString(_ctx.Font, dialogue, new Vector2(modal.Center.X - (dSize.X * 1.1f) / 2f, modal.Y + 110), UITheme.AccentGold, 0f, Vector2.Zero, 1.1f, SpriteEffects.None, 0f);

            string desc = "The shady collector offers an astronomical buyout for Toothless!\nIf you refuse, he will attack to confiscate your specimen!";
            batch.DrawString(_ctx.Font, desc, new Vector2(modal.X + 50, modal.Y + 170), UITheme.TextSecondary);

            CleanUI.DrawButton(batch, _ctx.Font, _doorOption1Btn, "ACCEPT BUYOUT", _doorOption1Btn.Contains(mPos), accent: UITheme.AccentGold);
            CleanUI.DrawButton(batch, _ctx.Font, _doorOption2Btn, "REFUSE / FIGHT", _doorOption2Btn.Contains(mPos), accent: UITheme.AccentCoral, isPrimary: true);
        }
    }
}
