#nullable enable
using BePal.Gameplay;
using BePal.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace BePal.Screens;

/// <summary>
/// Morning delivery porch scene for Days 2–5 where the player inspects the courier crate
/// and receives the new daily abnormal pet before entering the shelter.
/// </summary>
public sealed class DoorstepScreen : IScreen
{
    private static readonly Rectangle SkipButtonRect = new(1080, 24, 160, 44);
    private static readonly Rectangle DialogueBounds = new(140, 480, 1000, 210);
    private static readonly Rectangle CrateFrameRect = new(480, 150, 320, 300);

    private readonly ScreenContext _context;
    private readonly DialogueBox _dialogueBox = new();

    public DialogueBox Dialogue => _dialogueBox;

    public DoorstepScreen(ScreenContext context)
    {
        _context = context;
        _dialogueBox.TypewriterSpeed = 38f;
        _context.Audio.PlayBoxOpen();

        PetDefinition activePet = PetCatalog.Get(_context.Run.ActivePet);
        _dialogueBox.StartDialogue(
            $"MORNING DELIVERY - DAY {_context.Run.DayNumber}",
            NarrativeScripts.GetDoorstepLines(_context.Run.DayNumber, _context.Run.ActivePet),
            onCompleted: EnterShelter);
    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_context.IsButtonClicked(SkipButtonRect) || _context.IsKeyPressed(Keys.Escape))
        {
            EnterShelter();
            return;
        }

        _dialogueBox.UpdateInput(dt, _context.Keyboard, _context.PreviousKeyboard, _context.Mouse, _context.PreviousMouse);

        if (_dialogueBox.IsFinished)
        {
            EnterShelter();
        }
    }

    private void EnterShelter()
    {
        PetDefinition pet = PetCatalog.Get(_context.Run.ActivePet);
        _context.Manager.ShowHome($"Day {_context.Run.DayNumber} begins. Care for {pet.Name} or inspect the shelter.");
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        // Porch background ambiance
        spriteBatch.FillRectangle(new RectangleF(0, 0, 1280, 720), new Color(18, 22, 32));

        // Header text
        PetDefinition activePet = PetCatalog.Get(_context.Run.ActivePet);
        _context.DrawCenterText(spriteBatch, $"DAY {_context.Run.DayNumber}: MORNING COURIER ARRIVAL", new Vector2(640, 55), 1.2f, new Color(255, 203, 139));
        _context.DrawCenterText(spriteBatch, $"Incoming Specimen: {activePet.Name} | Hazard Lv {activePet.HazardLevel}", new Vector2(640, 95), 0.75f, Color.LightGray);

        // Skip button
        _context.DrawButton(spriteBatch, SkipButtonRect, "Enter Shelter >>", true);

        // Crate Frame
        Color crateBorderColor = activePet.HazardLevel switch
        {
            1 => new Color(110, 220, 130),
            2 => new Color(255, 120, 100),
            3 => new Color(210, 140, 255),
            _ => Color.Gray
        };

        spriteBatch.FillRectangle(new RectangleF(CrateFrameRect.X, CrateFrameRect.Y, CrateFrameRect.Width, CrateFrameRect.Height), new Color(26, 28, 38));
        spriteBatch.DrawRectangle(new RectangleF(CrateFrameRect.X, CrateFrameRect.Y, CrateFrameRect.Width, CrateFrameRect.Height), crateBorderColor, 3f);

        // Pet Preview in crate
        Rectangle petRect = new(CrateFrameRect.X + 70, CrateFrameRect.Y + 25, 180, 240);
        spriteBatch.Draw(_context.PetIdle, petRect, Color.White);

        // Manifest Badge
        Rectangle badgeRect = new(CrateFrameRect.X - 10, CrateFrameRect.Bottom - 36, CrateFrameRect.Width + 20, 30);
        spriteBatch.FillRectangle(new RectangleF(badgeRect.X, badgeRect.Y, badgeRect.Width, badgeRect.Height), new Color(35, 42, 58));
        spriteBatch.DrawRectangle(new RectangleF(badgeRect.X, badgeRect.Y, badgeRect.Width, badgeRect.Height), crateBorderColor, 1.5f);
        _context.DrawCenterText(spriteBatch, $"MANIFEST: {activePet.Name.ToUpperInvariant()} (LV {activePet.HazardLevel})", new Vector2(640, badgeRect.Y + 15), 0.65f, Color.White);

        // Dialogue Box
        _dialogueBox.Draw(spriteBatch, _context.Font, _context.Pixel, DialogueBounds);
    }
}
