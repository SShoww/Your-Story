#nullable enable
using BePal.Gameplay;
using BePal.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace BePal.Screens;

/// <summary>
/// Day 1 Prologue sequence introducing the player's abnormal daycare shelter,
/// the mysterious crate delivery, and unboxing Mossling.
/// </summary>
public sealed class PrologueScreen : IScreen
{
    private static readonly Rectangle SkipButtonRect = new(1080, 24, 160, 44);
    private static readonly Rectangle DialogueBounds = new(140, 480, 1000, 210);
    private static readonly Rectangle CrateFrameRect = new(490, 160, 300, 290);

    private readonly ScreenContext _context;
    private readonly DialogueBox _dialogueBox = new();

    public DialogueBox Dialogue => _dialogueBox;

    public PrologueScreen(ScreenContext context)
    {
        _context = context;
        _dialogueBox.TypewriterSpeed = 38f;
        _dialogueBox.StartDialogue(
            "DAYCARE PROLOGUE - DAY 1",
            NarrativeScripts.GetPrologueLines(),
            onCompleted: CompletePrologue);
    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_context.IsButtonClicked(SkipButtonRect) || _context.IsKeyPressed(Keys.Escape))
        {
            CompletePrologue();
            return;
        }

        _dialogueBox.UpdateInput(dt, _context.Keyboard, _context.PreviousKeyboard, _context.Mouse, _context.PreviousMouse);

        if (_dialogueBox.IsFinished)
        {
            CompletePrologue();
        }
    }

    private void CompletePrologue()
    {
        _context.Manager.ShowHome("Day 1 begins. Inspect the shelter or click Mossling to begin care.");
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        // Background ambiance
        spriteBatch.FillRectangle(new RectangleF(0, 0, 1280, 720), new Color(14, 16, 26));

        // Top title
        _context.DrawCenterText(spriteBatch, "PROLOGUE: THE MORNING COURIER", new Vector2(640, 60), 1.25f, new Color(255, 203, 139));
        _context.DrawCenterText(spriteBatch, "Subject Delivery #001: Mossling", new Vector2(640, 100), 0.75f, Color.LightGray);

        // Skip button
        _context.DrawButton(spriteBatch, SkipButtonRect, "Skip Intro >>", true);

        // Central Crate & Creature display
        spriteBatch.FillRectangle(new RectangleF(CrateFrameRect.X, CrateFrameRect.Y, CrateFrameRect.Width, CrateFrameRect.Height), new Color(28, 22, 18));
        spriteBatch.DrawRectangle(new RectangleF(CrateFrameRect.X, CrateFrameRect.Y, CrateFrameRect.Width, CrateFrameRect.Height), new Color(190, 140, 90), 3f);

        // Crate wood slat accents
        for (int y = CrateFrameRect.Y + 45; y < CrateFrameRect.Bottom - 20; y += 50)
        {
            spriteBatch.DrawLine(new Vector2(CrateFrameRect.X + 8, y), new Vector2(CrateFrameRect.Right - 8, y), new Color(100, 75, 50), 2f);
        }

        // Pet inside crate
        Rectangle petRect = new(CrateFrameRect.X + 60, CrateFrameRect.Y + 25, 180, 240);
        spriteBatch.Draw(_context.PetIdle, petRect, Color.White);

        // Hazard seal tape banner across bottom of crate
        Rectangle tapeRect = new(CrateFrameRect.X - 20, CrateFrameRect.Bottom - 45, CrateFrameRect.Width + 40, 32);
        spriteBatch.FillRectangle(new RectangleF(tapeRect.X, tapeRect.Y, tapeRect.Width, tapeRect.Height), new Color(220, 170, 30));
        spriteBatch.DrawRectangle(new RectangleF(tapeRect.X, tapeRect.Y, tapeRect.Width, tapeRect.Height), Color.Black, 1.5f);
        _context.DrawCenterText(spriteBatch, "[ ! CAUTION: LIVE ABNORMAL SPECIMEN ! ]", new Vector2(640, tapeRect.Y + 16), 0.65f, Color.Black);

        // Render DialogueBox
        _dialogueBox.Draw(spriteBatch, _context.Font, _context.Pixel, DialogueBounds);
    }
}
