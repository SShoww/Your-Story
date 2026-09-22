using BePalV2.Audio;
using BePalV2.Gameplay;
using BePalV2.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace BePalV2.Screens;

public sealed class DailySummaryScreen : IScreen
{
    private readonly ScreenContext _ctx;
    private KeyboardState _prevKeyboard;
    private MouseState _prevMouse;

    // 1920x1080 Layout Constants
    private readonly Rectangle _proceedBtn = new(580, 890, 760, 60);

    public DailySummaryScreen(ScreenContext ctx)
    {
        _ctx = ctx;
        // Apply daily progression when summary screen opens
        _ctx.Run.ApplyEndOfDayProgression();
    }

    public void Update(GameTime gameTime)
    {
        var kbd = Keyboard.GetState();
        var mouse = Mouse.GetState();
        bool enter = kbd.IsKeyDown(Keys.Enter) && !_prevKeyboard.IsKeyDown(Keys.Enter);
        bool space = kbd.IsKeyDown(Keys.Space) && !_prevKeyboard.IsKeyDown(Keys.Space);
        bool click = mouse.LeftButton == ButtonState.Pressed && _prevMouse.LeftButton == ButtonState.Released;

        if (enter || space || (click && _proceedBtn.Contains(mouse.Position)))
        {
            Proceed();
        }

        _prevKeyboard = kbd;
        _prevMouse = mouse;
    }

    private void Proceed()
    {
        _ctx.Audio.PlayConfirm();
        var run = _ctx.Run;

        if (run.Ending != StoryEnding.None)
        {
            _ctx.ScreenManager.SetScreen(new EndingScreen(_ctx, run.Ending));
        }
        else if (run.DayNumber >= V2RunState.MaxDays)
        {
            _ctx.ScreenManager.SetScreen(new CombatArenaScreen(_ctx, CombatMode.ChapterBoss));
        }
        else
        {
            run.AdvanceToNextDay();
            _ctx.ScreenManager.SetScreen(new BaseHabitatScreen(_ctx));
        }
    }

    public void Draw(GameTime gameTime, SpriteBatch batch)
    {
        batch.FillRectangle(new Rectangle(0, 0, _ctx.ScreenWidth, _ctx.ScreenHeight), new Color(14, 16, 22));

        // Clipboard Document (Centered in 1920x1080)
        Rectangle doc = new(460, 60, 1000, 940);
        batch.FillRectangle(doc, new Color(245, 240, 225)); // Cream paper
        batch.DrawRectangle(doc, new Color(60, 50, 40), 4);

        // Header clip
        Rectangle clip = new(860, 36, 200, 36);
        batch.FillRectangle(clip, new Color(90, 90, 100));
        batch.DrawRectangle(clip, Color.Black, 2);

        var run = _ctx.Run;
        var pet = run.ActivePet;

        // Stamp title (Sized and positioned cleanly according to UI design rules)
        string docTitle = $"DAILY CARE EVALUATION - DAY {run.DayNumber} / 3";
        batch.DrawString(_ctx.Font, docTitle, new Vector2(doc.X + 60, doc.Y + 54), new Color(40, 30, 20), 0f, Vector2.Zero, 1.25f, SpriteEffects.None, 0f);
        batch.DrawLine(doc.X + 60, doc.Y + 104, doc.Right - 60, doc.Y + 104, new Color(160, 140, 120), 2f);

        // Big Grade Stamp Frame
        Color gradeColor = run.LastCareGrade switch
        {
            CareGrade.S => new Color(40, 140, 60),
            CareGrade.A => new Color(60, 110, 180),
            CareGrade.B => new Color(180, 130, 40),
            CareGrade.C => new Color(180, 80, 30),
            _ => new Color(180, 40, 40)
        };

        Rectangle gradeStamp = new(doc.Right - 360, doc.Y + 120, 300, 70);
        batch.FillRectangle(gradeStamp, gradeColor * 0.15f);
        batch.DrawRectangle(gradeStamp, gradeColor, 2);
        string gradeText = $"GRADE: {run.LastCareGrade}";
        Vector2 gSize = _ctx.Font.MeasureString(gradeText);
        batch.DrawString(_ctx.Font, gradeText, new Vector2(gradeStamp.Center.X - (gSize.X * 1.6f) / 2f, gradeStamp.Center.Y - (gSize.Y * 1.6f) / 2f), gradeColor, 0f, Vector2.Zero, 1.6f, SpriteEffects.None, 0f);

        // Subtitle note
        batch.DrawString(_ctx.Font, $"Specimen: {pet.Name} (LV. {pet.Level})", new Vector2(doc.X + 60, doc.Y + 130), new Color(60, 50, 40), 0f, Vector2.Zero, 1.1f, SpriteEffects.None, 0f);
        batch.DrawString(_ctx.Font, $"Shift Evaluation Date: Shift 0{run.DayNumber}", new Vector2(doc.X + 60, doc.Y + 165), new Color(100, 90, 80));

        // Section 1: Financial Statement
        int fy = doc.Y + 225;
        batch.DrawString(_ctx.Font, "--- FINANCIAL SHIFT REPORT ---", new Vector2(doc.X + 60, fy), new Color(70, 50, 30), 0f, Vector2.Zero, 1.15f, SpriteEffects.None, 0f);
        batch.DrawString(_ctx.Font, $"- Daily Shelter Subsidy:          +{EconomyManager.DailySubsidyAmount} G", new Vector2(doc.X + 70, fy + 38), new Color(40, 110, 40));
        batch.DrawString(_ctx.Font, $"- Care Performance Bonus:         +{run.LastGoldReward} G", new Vector2(doc.X + 70, fy + 72), new Color(40, 110, 40));
        batch.DrawString(_ctx.Font, $"- Research Points Earned:         +{run.Economy.PlayerPoints} PTS", new Vector2(doc.X + 70, fy + 106), new Color(30, 80, 140));
        batch.DrawString(_ctx.Font, $"- Total Wallet Balance:            {run.Economy.Gold} G", new Vector2(doc.X + 70, fy + 140), new Color(20, 20, 20));
        if (run.Economy.HasDebt)
        {
            batch.DrawString(_ctx.Font, $"- Outstanding Debt (20% Interest): {run.Economy.Debt} G [WARNING]", new Vector2(doc.X + 70, fy + 174), new Color(180, 40, 40));
        }

        // Section 2: Biological & Habitat Report
        int by = fy + 220;
        batch.DrawString(_ctx.Font, "--- BIOLOGICAL STATUS REPORT ---", new Vector2(doc.X + 60, by), new Color(70, 50, 30), 0f, Vector2.Zero, 1.15f, SpriteEffects.None, 0f);
        batch.DrawString(_ctx.Font, $"- Stomach Natural Decay:          -{run.LastStomachLost}%", new Vector2(doc.X + 70, by + 38), new Color(140, 80, 20));
        batch.DrawString(_ctx.Font, $"- Cleanliness Natural Decay:      -{run.LastCleanLost}%", new Vector2(doc.X + 70, by + 72), new Color(40, 80, 140));
        if (run.LastOvernightDamage > 0f)
        {
            batch.DrawString(_ctx.Font, $"- Sickness Overnight Damage:      -{run.LastOvernightDamage} HP [INFECTION / HUNGER]", new Vector2(doc.X + 70, by + 106), new Color(180, 30, 30));
        }
        if (run.LastPhotosynthesisHeal > 0f)
        {
            batch.DrawString(_ctx.Font, $"- Photosynthesis Trait Heal:      +{run.LastPhotosynthesisHeal} HP [CLEAN BONUS]", new Vector2(doc.X + 70, by + 106), new Color(30, 130, 50));
        }
        batch.DrawString(_ctx.Font, $"- Specimen Current Health:        {(int)pet.Health} / {(int)pet.MaxHealth} HP", new Vector2(doc.X + 70, by + 140), new Color(30, 30, 30));

        // Proceed button (Centered, properly proportioned with CleanUI styling)
        Point mPos = Mouse.GetState().Position;
        string btnLabel = run.DayNumber >= V2RunState.MaxDays ? "VIEW STORY CONCLUSION [ ENTER ]" : $"REST & ADVANCE TO DAY {run.DayNumber + 1} [ ENTER ]";
        CleanUI.DrawButton(batch, _ctx.Font, _proceedBtn, btnLabel, _proceedBtn.Contains(mPos), accent: UITheme.AccentEmerald, isPrimary: true);
    }
}
