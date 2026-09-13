#nullable enable
using System;
using System.Collections.Generic;
using BePal.Gameplay;
using BePal.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace BePal.Screens;

/// <summary>
/// 4-Wall Panoramic Shelter room screen (Samsara Room style) supporting 360-degree rotation,
/// interactive object inspection, ambient behavior cues, and care initiation.
/// </summary>
public sealed class PanoramicRoomScreen : IScreen
{
    private static readonly Rectangle PetFrame = new(530, 165, 220, 290);
    private static readonly Rectangle CareButtonRect = new(490, 480, 300, 48);
    private static readonly Rectangle EndDayButtonRect = new(50, 595, 250, 60);
    private static readonly Rectangle SurvivalLogButtonRect = new(980, 595, 250, 60);

    private static readonly Rectangle LeftArrowRect = new(24, 320, 56, 72);
    private static readonly Rectangle RightArrowRect = new(1200, 320, 56, 72);
    private static readonly Rectangle DialogueBounds = new(160, 470, 960, 210);

    // Wall 1 inspectable rectangles
    private static readonly Rectangle PantryShelfRect = new(260, 180, 240, 260);
    private static readonly Rectangle WaterBasinRect = new(550, 240, 180, 180);
    private static readonly Rectangle DisposalBinRect = new(790, 280, 190, 220);

    // Wall 2 inspectable rectangles
    private static readonly Rectangle LogDeskRect = new(280, 220, 300, 240);
    private static readonly Rectangle NoticeBoardRect = new(680, 160, 340, 240);

    // Wall 3 inspectable rectangles
    private static readonly Rectangle FrontDoorRect = new(510, 130, 260, 390);
    private static readonly Rectangle WindowRect = new(220, 180, 210, 250);
    private static readonly Rectangle ShiftClockRect = new(850, 190, 210, 180);
    private static readonly Rectangle Wall4EndDayRect = new(510, 545, 260, 60);

    private readonly ScreenContext _context;
    private readonly PanoramicRoomModel _model = new();
    private readonly DialogueBox _dialogueBox = new();

    public PanoramicRoomModel Model => _model;
    public DialogueBox Dialogue => _dialogueBox;

    public PanoramicRoomScreen(ScreenContext context, int initialWall = 0)
    {
        _context = context;
        _model.SetWall(initialWall);
    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_dialogueBox.IsActive)
        {
            _dialogueBox.UpdateInput(dt, _context.Keyboard, _context.PreviousKeyboard, _context.Mouse, _context.PreviousMouse);
            return;
        }

        // Navigation rotation
        bool rotateLeft = _context.IsButtonClicked(LeftArrowRect) ||
                          _context.IsKeyPressed(Keys.A) ||
                          _context.IsKeyPressed(Keys.Left);

        bool rotateRight = _context.IsButtonClicked(RightArrowRect) ||
                           _context.IsKeyPressed(Keys.D) ||
                           _context.IsKeyPressed(Keys.Right);

        if (rotateLeft)
        {
            _model.RotateLeft();
            return;
        }
        if (rotateRight)
        {
            _model.RotateRight();
            return;
        }

        // Wall specific interactions
        switch (_model.CurrentWallIndex)
        {
            case 0:
                UpdateWall0PetZone();
                break;
            case 1:
                UpdateWall1PrepAndPantry();
                break;
            case 2:
                UpdateWall2StudyDesk();
                break;
            case 3:
                UpdateWall3FrontDoor();
                break;
        }
    }

    private void UpdateWall0PetZone()
    {
        PetDefinition activePet = PetCatalog.Get(_context.Run.ActivePet);

        if (_context.IsButtonClicked(PetFrame) || _context.IsButtonClicked(CareButtonRect))
        {
            _dialogueBox.ShowPrompt(
                "CARE INITIATION",
                $"Begin Pet-Care Session for {activePet.Name}?",
                onYes: () => _context.Manager.BeginCare(),
                onNo: () => _dialogueBox.Close());
        }
        else if (_context.IsButtonClicked(EndDayButtonRect) && _context.Run.CanEndDay)
        {
            _context.Manager.AdvanceDay("You ended the day safely.");
        }
        else if (_context.IsButtonClicked(SurvivalLogButtonRect))
        {
            _context.Manager.ShowSurvivalLog();
        }
    }

    private void UpdateWall1PrepAndPantry()
    {
        PetDefinition activePet = PetCatalog.Get(_context.Run.ActivePet);

        if (_context.IsButtonClicked(PantryShelfRect))
        {
            _dialogueBox.StartDialogue("PANTRY SHELVES", new[]
            {
                "Glass jars labeled 'ORGANIC COMPOST' and 'RAW MINERAL ROOTS'.",
                $"Dietary note: {activePet.Name} exhibits positive attraction toward earthy, nutrient-rich feed."
            });
        }
        else if (_context.IsButtonClicked(WaterBasinRect))
        {
            _dialogueBox.StartDialogue("WATER BASIN", new[]
            {
                "A clean ceramic basin with cool filtered spring water.",
                "Ensures stable hydration levels during containment."
            });
        }
        else if (_context.IsButtonClicked(DisposalBinRect))
        {
            _dialogueBox.StartDialogue("DISPOSAL BIN", new[]
            {
                "Discarded synthetic pet kibble, thoroughly rejected and spat out.",
                "Abnormal creatures consistently scorn artificial laboratory pellets."
            });
        }
    }

    private void UpdateWall2StudyDesk()
    {
        if (_context.IsButtonClicked(LogDeskRect))
        {
            _dialogueBox.StartDialogue("SURVIVAL LOG DESK", new[]
            {
                "Opening the researcher's field notebook..."
            }, onCompleted: () => _context.Manager.ShowSurvivalLog());
        }
        else if (_context.IsButtonClicked(NoticeBoardRect))
        {
            _dialogueBox.StartDialogue("NOTICE BOARD", new[]
            {
                "CONFIDENTIAL PROTOCOL: DAYCARE CONTAINMENT",
                "1. Complete at least one Pet-Care Session per day to fulfill basic requirements.",
                "2. Observe real-time physical cues closely before confirming care actions.",
                "3. Three successful sessions unlock a creature's complete Survival Log profile."
            });
        }
    }

    private void UpdateWall3FrontDoor()
    {
        if (_context.IsButtonClicked(Wall4EndDayRect) || (_context.IsButtonClicked(EndDayButtonRect) && _context.Run.CanEndDay))
        {
            if (_context.Run.CanEndDay)
            {
                _context.Manager.AdvanceDay("You ended the day safely.");
            }
            else
            {
                _dialogueBox.StartDialogue("SHIFT NOTICE", new[]
                {
                    "You cannot end your shift yet.",
                    "Complete at least 1 Pet-Care Session today before clocking out."
                });
            }
        }
        else if (_context.IsButtonClicked(FrontDoorRect))
        {
            _dialogueBox.StartDialogue("FRONT ENTRANCE", new[]
            {
                "Heavy solid oak door with reinforced steel lockbars.",
                "Delivery couriers leave crates on the porch outside at dawn and depart immediately."
            });
        }
        else if (_context.IsButtonClicked(WindowRect))
        {
            _dialogueBox.StartDialogue("PORCH WINDOW", new[]
            {
                "Dense fog covers the perimeter grounds.",
                "No human presence can be seen. Silence surrounds the facility."
            });
        }
        else if (_context.IsButtonClicked(ShiftClockRect))
        {
            _dialogueBox.StartDialogue("SHIFT MONITOR", new[]
            {
                $"Shift Record: Day {_context.Run.DayNumber} of 5.",
                $"Sessions Completed Today: {_context.Run.SessionsToday} | Player Health: {_context.Run.Health} HP."
            });
        }
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        _context.DrawHUD(spriteBatch);

        // Draw Wall Title Banner and room dots
        DrawWallBanner(spriteBatch);

        // Draw Nav Arrows
        DrawNavigationArrows(spriteBatch);

        // Draw specific wall contents
        switch (_model.CurrentWallIndex)
        {
            case 0:
                DrawWall0PetZone(spriteBatch);
                break;
            case 1:
                DrawWall1PrepAndPantry(spriteBatch);
                break;
            case 2:
                DrawWall2StudyDesk(spriteBatch);
                break;
            case 3:
                DrawWall3FrontDoor(spriteBatch);
                break;
        }

        // Draw DialogueBox on top if active
        if (_dialogueBox.IsActive)
        {
            _dialogueBox.Draw(spriteBatch, _context.Font, _context.Pixel, DialogueBounds);
        }
    }

    private void DrawWallBanner(SpriteBatch spriteBatch)
    {
        string title = _model.GetWallTitle(_model.CurrentWallIndex);
        Rectangle bannerRect = new(340, 72, 600, 36);
        spriteBatch.FillRectangle(new RectangleF(bannerRect.X, bannerRect.Y, bannerRect.Width, bannerRect.Height), new Color(18, 24, 38, 230));
        spriteBatch.DrawRectangle(new RectangleF(bannerRect.X, bannerRect.Y, bannerRect.Width, bannerRect.Height), new Color(255, 203, 139), 1.5f);
        _context.DrawCenterText(spriteBatch, title, new Vector2(640, 90), 0.72f, new Color(255, 225, 165));

        // Draw 4 room position indicator dots
        for (int i = 0; i < 4; i++)
        {
            Vector2 dotPos = new(590 + i * 34, 120);
            Color dotColor = (i == _model.CurrentWallIndex) ? new Color(255, 203, 139) : new Color(80, 85, 105);
            spriteBatch.FillRectangle(new RectangleF(dotPos.X - 6, dotPos.Y - 6, 12, 12), dotColor);
        }
    }

    private void DrawNavigationArrows(SpriteBatch spriteBatch)
    {
        Point mousePos = _context.Mouse.Position;
        bool hoverLeft = LeftArrowRect.Contains(mousePos);
        bool hoverRight = RightArrowRect.Contains(mousePos);

        Color leftBg = hoverLeft ? new Color(75, 95, 140) : new Color(30, 36, 52, 210);
        Color rightBg = hoverRight ? new Color(75, 95, 140) : new Color(30, 36, 52, 210);

        spriteBatch.FillRectangle(new RectangleF(LeftArrowRect.X, LeftArrowRect.Y, LeftArrowRect.Width, LeftArrowRect.Height), leftBg);
        spriteBatch.DrawRectangle(new RectangleF(LeftArrowRect.X, LeftArrowRect.Y, LeftArrowRect.Width, LeftArrowRect.Height), new Color(255, 203, 139), 1.5f);
        _context.DrawCenterText(spriteBatch, "<", new Vector2(LeftArrowRect.X + LeftArrowRect.Width / 2f, LeftArrowRect.Y + LeftArrowRect.Height / 2f), 1.2f, Color.White);

        spriteBatch.FillRectangle(new RectangleF(RightArrowRect.X, RightArrowRect.Y, RightArrowRect.Width, RightArrowRect.Height), rightBg);
        spriteBatch.DrawRectangle(new RectangleF(RightArrowRect.X, RightArrowRect.Y, RightArrowRect.Width, RightArrowRect.Height), new Color(255, 203, 139), 1.5f);
        _context.DrawCenterText(spriteBatch, ">", new Vector2(RightArrowRect.X + RightArrowRect.Width / 2f, RightArrowRect.Y + RightArrowRect.Height / 2f), 1.2f, Color.White);
    }

    private void DrawWall0PetZone(SpriteBatch spriteBatch)
    {
        Vector2 c = new(640, 310);
        Rectangle frame = new((int)c.X - 110, (int)c.Y - 145, 220, 290);
        spriteBatch.FillRectangle(new RectangleF(frame.X, frame.Y, frame.Width, frame.Height), new Color(14, 16, 24, 210));
        spriteBatch.Draw(_context.PetImage, new Rectangle((int)c.X - 90, (int)c.Y - 125, 180, 250), Color.White);
        spriteBatch.DrawRectangle(new RectangleF(frame.X, frame.Y, frame.Width, frame.Height), new Color(255, 203, 139), 2f);

        PetDefinition activePet = PetCatalog.Get(_context.Run.ActivePet);
        _context.DrawButton(spriteBatch, CareButtonRect, $"CARE FOR {activePet.Name.ToUpperInvariant()}", true);

        // Behavior cue banner
        string cueText = PanoramicRoomModel.GetPetBehaviorCue(_context.Run.ActivePet);
        Rectangle cueRect = new(240, 440, 800, 30);
        spriteBatch.FillRectangle(new RectangleF(cueRect.X, cueRect.Y, cueRect.Width, cueRect.Height), new Color(25, 30, 48, 220));
        _context.DrawCenterText(spriteBatch, cueText, new Vector2(640, 455), 0.65f, new Color(150, 225, 255));

        _context.DrawCenterText(spriteBatch, _context.Message, new Vector2(640, 550), 0.76f, new Color(255, 225, 165));
        _context.DrawButton(spriteBatch, EndDayButtonRect, "End Day", _context.Run.CanEndDay);
        _context.DrawButton(spriteBatch, SurvivalLogButtonRect, "Survival Log", true);
    }

    private void DrawWall1PrepAndPantry(SpriteBatch spriteBatch)
    {
        // Pantry Shelf
        DrawInspectableCard(spriteBatch, PantryShelfRect, "PANTRY SHELVES", "Feed Clues & Roots", new Color(42, 65, 50));

        // Water Basin
        DrawInspectableCard(spriteBatch, WaterBasinRect, "WATER BASIN", "Clean Spring Water", new Color(38, 55, 82));

        // Disposal Bin
        DrawInspectableCard(spriteBatch, DisposalBinRect, "DISPOSAL BIN", "Rejected Pellets", new Color(65, 42, 42));

        _context.DrawCenterText(spriteBatch, "Click any station to inspect ingredients and dietary preferences.", new Vector2(640, 560), 0.75f, Color.LightGray);
    }

    private void DrawWall2StudyDesk(SpriteBatch spriteBatch)
    {
        // Survival Log Book
        DrawInspectableCard(spriteBatch, LogDeskRect, "SURVIVAL LOG", "Click to Open Notebook", new Color(60, 50, 35));

        // Notice Board
        DrawInspectableCard(spriteBatch, NoticeBoardRect, "NOTICE BOARD", "Facility Directives & Memos", new Color(45, 45, 65));

        _context.DrawCenterText(spriteBatch, "Review logged specimens and research directives.", new Vector2(640, 560), 0.75f, Color.LightGray);
    }

    private void DrawWall3FrontDoor(SpriteBatch spriteBatch)
    {
        // Porch Window
        DrawInspectableCard(spriteBatch, WindowRect, "WINDOW", "Morning Fog", new Color(32, 40, 52));

        // Front Door
        DrawInspectableCard(spriteBatch, FrontDoorRect, "FRONT DOOR", "Barred From Inside", new Color(55, 38, 30));

        // Shift Clock
        DrawInspectableCard(spriteBatch, ShiftClockRect, "SHIFT MONITOR", $"Day {_context.Run.DayNumber} / 5", new Color(45, 52, 60));

        // End Day Button
        _context.DrawButton(spriteBatch, Wall4EndDayRect, "End Day Shift", _context.Run.CanEndDay);

        string hint = _context.Run.CanEndDay
            ? "Daily requirement fulfilled! You may end your shift safely or continue care."
            : "Complete at least 1 Pet-Care Session today before ending the shift.";
        Color hintColor = _context.Run.CanEndDay ? new Color(110, 245, 150) : new Color(245, 140, 140);
        _context.DrawCenterText(spriteBatch, hint, new Vector2(640, 625), 0.72f, hintColor);
    }

    private void DrawInspectableCard(SpriteBatch spriteBatch, Rectangle bounds, string header, string subtitle, Color baseColor)
    {
        bool hovered = bounds.Contains(_context.Mouse.Position);
        Color bg = hovered ? Color.Lerp(baseColor, Color.White, 0.15f) : baseColor;
        Color border = hovered ? Color.White : new Color(255, 203, 139);

        spriteBatch.FillRectangle(new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height), bg);
        spriteBatch.DrawRectangle(new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height), border, hovered ? 2.5f : 1.5f);

        Vector2 center = new(bounds.X + bounds.Width / 2f, bounds.Y + bounds.Height / 2f);
        _context.DrawCenterText(spriteBatch, header, new Vector2(center.X, center.Y - 14), 0.8f, new Color(255, 225, 165));
        _context.DrawCenterText(spriteBatch, subtitle, new Vector2(center.X, center.Y + 16), 0.65f, Color.LightGray);

        if (hovered)
        {
            _context.DrawCenterText(spriteBatch, "[ Click to Inspect ]", new Vector2(center.X, bounds.Y + bounds.Height - 20), 0.6f, Color.Yellow);
        }
    }
}
