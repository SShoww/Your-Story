using BePalV2.Audio;
using BePalV2.Gameplay;
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

    private readonly Rectangle _proceedBtn = new(480, 590, 320, 50);

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
            // Default completion if survived 3 days without betrayal
            _ctx.ScreenManager.SetScreen(new EndingScreen(_ctx, StoryEnding.EndingB_Protector));
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

        // Clipboard Document
        Rectangle doc = new(300, 40, 680, 630);
        batch.FillRectangle(doc, new Color(245, 240, 225)); // Cream paper
        batch.DrawRectangle(doc, new Color(60, 50, 40), 3);

        // Header clip
        Rectangle clip = new(570, 25, 140, 25);
        batch.FillRectangle(clip, new Color(90, 90, 100));
        batch.DrawRectangle(clip, Color.Black, 1);

        var run = _ctx.Run;
        var pet = run.ActivePet;

        // Stamp title
        batch.DrawString(_ctx.Font, $"DAILY CARE EVALUATION - DAY {run.DayNumber} / 3", new Vector2(doc.X + 40, doc.Y + 30), new Color(40, 30, 20), 0f, Vector2.Zero, 1.3f, SpriteEffects.None, 0f);
        batch.DrawLine(doc.X + 40, doc.Y + 70, doc.Right - 40, doc.Y + 70, new Color(160, 140, 120), 2f);

        // Big Grade Stamp
        Color gradeColor = run.LastCareGrade switch
        {
            CareGrade.S => new Color(40, 140, 60),
            CareGrade.A => new Color(60, 110, 180),
            CareGrade.B => new Color(180, 130, 40),
            CareGrade.C => new Color(180, 80, 30),
            _ => new Color(180, 40, 40)
        };
        batch.DrawString(_ctx.Font, $"CARE GRADE: {run.LastCareGrade}", new Vector2(doc.X + 40, doc.Y + 85), gradeColor, 0f, Vector2.Zero, 1.8f, SpriteEffects.None, 0f);

        // Section 1: Financial Statement
        int fy = doc.Y + 160;
        batch.DrawString(_ctx.Font, "--- FINANCIAL SHIFT REPORT ---", new Vector2(doc.X + 40, fy), new Color(80, 60, 40));
        batch.DrawString(_ctx.Font, $"- Daily Shelter Subsidy:      +{EconomyManager.DailySubsidyAmount} G", new Vector2(doc.X + 50, fy + 30), new Color(40, 100, 40));
        batch.DrawString(_ctx.Font, $"- Care Performance Bonus:     +{run.LastGoldReward} G", new Vector2(doc.X + 50, fy + 58), new Color(40, 100, 40));
        batch.DrawString(_ctx.Font, $"- Total Wallet Balance:        {run.Economy.Gold} G", new Vector2(doc.X + 50, fy + 86), new Color(20, 20, 20));
        if (run.Economy.HasDebt)
        {
            batch.DrawString(_ctx.Font, $"- Outstanding Debt (20% Int):  {run.Economy.Debt} G [WARNING]", new Vector2(doc.X + 50, fy + 114), new Color(180, 40, 40));
        }

        // Section 2: Biological & Habitat Report
        int by = fy + 155;
        batch.DrawString(_ctx.Font, "--- BIOLOGICAL STATUS REPORT ---", new Vector2(doc.X + 40, by), new Color(80, 60, 40));
        batch.DrawString(_ctx.Font, $"- Stomach Natural Decay:      -{run.LastStomachLost}%", new Vector2(doc.X + 50, by + 30), new Color(140, 80, 20));
        batch.DrawString(_ctx.Font, $"- Cleanliness Natural Decay:  -{run.LastCleanLost}%", new Vector2(doc.X + 50, by + 58), new Color(40, 80, 140));
        if (run.LastOvernightDamage > 0f)
        {
            batch.DrawString(_ctx.Font, $"- Sickness Overnight Damage:  -{run.LastOvernightDamage} HP [INFECTION/HUNGER]", new Vector2(doc.X + 50, by + 86), new Color(180, 30, 30));
        }
        if (run.LastPhotosynthesisHeal > 0f)
        {
            batch.DrawString(_ctx.Font, $"- Photosynthesis Trait Heal:  +{run.LastPhotosynthesisHeal} HP [CLEAN BONUS]", new Vector2(doc.X + 50, by + 114), new Color(30, 130, 50));
        }
        batch.DrawString(_ctx.Font, $"- Pet Current Health:          {(int)pet.Health} / {(int)pet.MaxHealth} HP", new Vector2(doc.X + 50, by + 142), new Color(30, 30, 30));

        // Proceed button
        batch.FillRectangle(_proceedBtn, new Color(40, 120, 70));
        batch.DrawRectangle(_proceedBtn, Color.Black, 2);
        string btnLabel = run.DayNumber >= V2RunState.MaxDays ? "VIEW STORY CONCLUSION [ ENTER ]" : $"REST & ADVANCE TO DAY {run.DayNumber + 1} [ ENTER ]";
        Vector2 bSize = _ctx.Font.MeasureString(btnLabel);
        batch.DrawString(_ctx.Font, btnLabel, new Vector2(_proceedBtn.Center.X - bSize.X / 2f, _proceedBtn.Center.Y - bSize.Y / 2f), Color.White);
    }
}
