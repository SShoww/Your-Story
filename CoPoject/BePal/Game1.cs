using System;
using BePal.Gameplay;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BePal;

public class Game1 : Game
{
    private const float Tau = MathF.PI * 2;
    private readonly GraphicsDeviceManager _graphics;
    private readonly Random _random = new();
    private SpriteBatch _batch = null!;
    private SpriteFont _font = null!;
    private Texture2D _pixel = null!;
    private Texture2D _circle = null!;
    private Texture2D _petIdle = null!;
    private Texture2D _petHappy = null!;
    private Texture2D _petAngry = null!;
    private Texture2D _petImage = null!;
    private PrototypeRun _run = new();
    private Screen _screen = Screen.Menu;
    private KeyboardState _previousKeyboard;
    private MouseState _previousMouse;
    private float _angle;
    private float _qteTime;
    private float _teleportAt;
    private bool _teleported;
    private string _message = "Welcome home.";

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void LoadContent()
    {
        _batch = new SpriteBatch(GraphicsDevice);
        _font = Content.Load<SpriteFont>("PrototypeFont");
        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });
        _circle = CreateCircle(128);
        _petIdle = Content.Load<Texture2D>("pet/spr_pet_idle");
        _petHappy = Content.Load<Texture2D>("pet/spr_pet_happy");
        _petAngry = Content.Load<Texture2D>("pet/spr_pet_angry");
        _petImage = _petIdle;
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState keyboard = Keyboard.GetState();
        MouseState mouse = Mouse.GetState();
        if (keyboard.IsKeyDown(Keys.Escape))
            _screen = _screen is Screen.Menu or Screen.Summary ? ExitScreen() : Screen.Home;

        if (_screen == Screen.Menu) UpdateMenu(mouse);
        else if (_screen == Screen.Help && Clicked(mouse, Button(570))) _screen = Screen.Menu;
        else if (_screen == Screen.Home) UpdateHome(mouse);
        else if (_screen == Screen.Log && Clicked(mouse, new Rectangle(510, 565, 260, 60))) _screen = Screen.Home;
        else if (_screen is Screen.Care or Screen.Dodge) UpdateQte(gameTime, keyboard);
        else if (_screen == Screen.Summary && Clicked(mouse, Button(585))) _screen = Screen.Menu;

        _previousKeyboard = keyboard;
        _previousMouse = mouse;
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(28, 32, 50));
        _batch.Begin();
        if (_screen == Screen.Menu) DrawMenu();
        else if (_screen == Screen.Help) DrawHelp();
        else if (_screen == Screen.Home) DrawHome();
        else if (_screen == Screen.Log) { DrawHome(); DrawLog(); }
        else if (_screen is Screen.Care or Screen.Dodge) DrawQte(_screen == Screen.Dodge);
        else DrawSummary();
        _batch.End();
        base.Draw(gameTime);
    }

    private Screen ExitScreen() { Exit(); return Screen.Menu; }
    private void UpdateMenu(MouseState mouse)
    {
        if (Clicked(mouse, Button(250))) { _run = new PrototypeRun(); _screen = Screen.Home; _message = "Day 1 begins. Click the pet to begin."; }
        else if (Clicked(mouse, Button(330))) _screen = Screen.Help;
        else if (Clicked(mouse, Button(410))) Exit();
    }
    private void UpdateHome(MouseState mouse)
    {
        if (Clicked(mouse, new Rectangle(460, 180, 360, 310))) BeginCare();
        else if (Clicked(mouse, new Rectangle(50, 585, 250, 60)) && _run.CanEndDay) { _run.EndDay(); AdvanceDay("You ended the day safely."); }
        else if (Clicked(mouse, new Rectangle(980, 585, 250, 60))) _screen = Screen.Log;
    }
    private void UpdateQte(GameTime time, KeyboardState keyboard)
    {
        _angle = (_angle + 2.2f * (float)time.ElapsedGameTime.TotalSeconds) % Tau;
        _qteTime += (float)time.ElapsedGameTime.TotalSeconds;
        if (_screen == Screen.Care && _run.ActivePet == PetKind.Trickster && !_teleported && _qteTime >= _teleportAt)
        { _angle = (float)_random.NextDouble() * Tau; _teleported = true; _message = "The marker teleported!"; }
        if (keyboard.IsKeyDown(Keys.Space) && _previousKeyboard.IsKeyUp(Keys.Space))
            if (_screen == Screen.Care) ResolveCare(); else ResolveDodge();
    }
    private void BeginCare() { _petImage = _petIdle; _screen = Screen.Care; ResetQte(); _message = $"Press Space on {Action(_run.ActivePet)}."; }
    private void ResolveCare()
    {
        if (Segment() != ActionIndex(_run.ActivePet)) { Fail("Wrong action. Lost 1 Health."); return; }
        _run.RecordCareSuccess();
        if (_run.ActivePet == PetKind.Attacker && _run.Satisfaction == 2) { _screen = Screen.Dodge; ResetQte(); _message = "Attack! Press Space in the gold Dodge Zone."; }
        else if (_run.Satisfaction == 3) { _run.CompleteSession(); _petImage = _petHappy; _screen = Screen.Home; _message = "Session complete! Satisfaction is full."; }
        else { ResetQte(); _message = "Correct! Satisfaction increased."; }
    }
    private void ResolveDodge()
    {
        if (MathF.Abs(Wrap(_angle, MathF.PI * 1.5f)) > MathF.PI / 5) { Fail("Missed Dodge Zone. Lost 1 Health."); return; }
        _screen = Screen.Care; ResetQte(); _message = "Dodge successful! One final care action remains.";
    }
    private void Fail(string message)
    {
        if (_run.TakeDamage()) { AdvanceDay("Forced Retreat. You recovered."); return; }
        _petImage = _petAngry; ResetQte(); _message = message;
    }
    private void AdvanceDay(string message)
    {
        if (_run.IsComplete) _screen = Screen.Summary;
        else { _petImage = _petIdle; _screen = Screen.Home; _message = $"{message} Day {_run.DayNumber} begins."; }
    }
    private void ResetQte() { _angle = 0; _qteTime = 0; _teleported = false; _teleportAt = .45f + (float)_random.NextDouble() * .85f; }

    private void DrawMenu()
    {
        Center("BePal", new Vector2(640, 120), 1.8f, new Color(255, 203, 139));
        Center("Pet-care timing prototype", new Vector2(640, 175), .85f, Color.LightGray);
        Button(Button(250), "Start Prototype", true); Button(Button(330), "How to Play", true); Button(Button(410), "Quit", true);
    }
    private void DrawHelp()
    {
        Center("How to Play", new Vector2(640, 105), 1.35f, new Color(255, 203, 139));
        Text("Click the pet at home. Press Space on its preferred Care Action.", new Vector2(180, 220), Color.White);
        Text("Attacks use a gold Dodge Zone. Three Care successes complete a session.", new Vector2(180, 280), Color.White);
        Text("Three sessions unlock a Survival Log entry. Zero Health forces a retreat.", new Vector2(180, 340), Color.White);
        Button(Button(570), "Back", true);
    }
    private void DrawHome()
    {
        Rect(new Rectangle(0, 0, 1280, 115), new Color(49, 61, 91));
        Text($"Day {_run.DayNumber} / 5", new Vector2(48, 36), Color.White);
        Text($"Health: {_run.Health} / 3", new Vector2(48, 72), new Color(255, 159, 159));
        Text($"Active Pet: {Name(_run.ActivePet)}", new Vector2(430, 36), Color.White);
        Text($"Sessions today: {_run.SessionsToday}", new Vector2(430, 72), Color.LightGray);
        Pet(new Vector2(640, 330), _run.ActivePet);
        Center($"Click {Name(_run.ActivePet)} to care", new Vector2(640, 475), .85f, Color.White);
        Center(_message, new Vector2(640, 535), .72f, new Color(255, 225, 165));
        Button(new Rectangle(50, 585, 250, 60), "End Day", _run.CanEndDay);
        Button(new Rectangle(980, 585, 250, 60), "Survival Log", true);
    }
    private void DrawLog()
    {
        Rect(new Rectangle(130, 80, 1020, 570), new Color(22, 25, 38));
        Outline(new Rectangle(130, 80, 1020, 570), new Color(255, 203, 139), 3);
        Center("Survival Log", new Vector2(640, 125), 1.25f, new Color(255, 203, 139));
        Log(PetKind.Baseline, 200); Log(PetKind.Attacker, 315); Log(PetKind.Trickster, 430);
        Button(new Rectangle(510, 565, 260, 60), "Close", true);
    }
    private void Log(PetKind pet, int y)
    {
        bool unlocked = _run.IsLogUnlocked(pet);
        string text = unlocked ? $"{Name(pet)}: prefers {Action(pet)}. {Pattern(pet)}" : $"{Name(pet)}: locked ({_run.CompletedSessions(pet)} / 3 sessions)";
        Text(text, new Vector2(205, y), unlocked ? Color.White : Color.Gray);
    }
    private void DrawQte(bool dodge)
    {
        Text($"Day {_run.DayNumber} / 5     Health: {_run.Health} / 3", new Vector2(45, 38), Color.White);
        Center(dodge ? "Dodge QTE" : "Care QTE", new Vector2(640, 90), 1.3f, new Color(255, 203, 139));
        Vector2 c = new(640, 360);
        if (dodge) { Circle(c, 215, new Color(76, 83, 109)); Vector2 zone = c + Dir(MathF.PI * 1.5f) * 155; Circle(zone, 72, new Color(239, 190, 82)); Center("DODGE", zone, .62f, Color.Black); Circle(c, 98, new Color(92, 48, 60)); Center("ATTACK!", c, .75f, Color.White); }
        else DrawCareWheel(c);
        Circle(c + Dir(_angle) * 215, 18, Color.White);
        Center(dodge ? "Press Space inside the gold Dodge Zone" : $"Satisfaction: {_run.Satisfaction} / 3", new Vector2(640, 600), .9f, Color.White);
        Center(_message, new Vector2(640, 650), .72f, new Color(255, 225, 165));
    }
    private void DrawCareWheel(Vector2 c)
    {
        string[] actions = { "Feed", "Play", "Pet", "Observe" };
        Color[] colors = { new(120,190,135), new(111,170,230), new(224,142,188), new(186,137,220) };
        for (int i = 0; i < 4; i++) { Vector2 p = c + Dir(i * MathF.PI / 2 + MathF.PI / 4) * 155; Circle(p, 76, colors[i]); Center(actions[i], p, .7f, Color.White); }
        Circle(c, 95, new Color(49, 61, 91)); Center(Name(_run.ActivePet), c, .75f, Color.White);
    }
    private void DrawSummary()
    {
        Center("Run Summary", new Vector2(640, 115), 1.45f, new Color(255, 203, 139));
        Text($"Forced Retreats: {_run.ForcedRetreats}", new Vector2(420, 260), Color.White);
        Text($"Mossling sessions: {_run.CompletedSessions(PetKind.Baseline)}", new Vector2(420, 310), Color.White);
        Text($"Nibbleclaw sessions: {_run.CompletedSessions(PetKind.Attacker)}", new Vector2(420, 360), Color.White);
        Text($"Blinkbun sessions: {_run.CompletedSessions(PetKind.Trickster)}", new Vector2(420, 410), Color.White);
        Button(Button(585), "Back to Menu", true);
    }
    private void Pet(Vector2 c, PetKind pet)
    {
        Rectangle frame = new(500, 150, 280, 360);
        _batch.Draw(_petImage, frame, Color.White);
        Outline(frame, new Color(255, 203, 139), 3);
    }
    private void Button(Rectangle r, string label, bool enabled) { Rect(r, enabled ? new Color(90,116,168) : new Color(71,74,85)); Outline(r, enabled ? new Color(255,203,139) : Color.Gray, 2); Center(label, r.Center.ToVector2(), .8f, enabled ? Color.White : Color.LightGray); }
    private void Text(string text, Vector2 p, Color c) => _batch.DrawString(_font, text, p, c);
    private void Center(string text, Vector2 p, float s, Color c) => _batch.DrawString(_font, text, p - _font.MeasureString(text) * s / 2, c, 0, Vector2.Zero, s, SpriteEffects.None, 0);
    private void Rect(Rectangle r, Color c) => _batch.Draw(_pixel, r, c);
    private void Outline(Rectangle r, Color c, int t) { Rect(new Rectangle(r.X,r.Y,r.Width,t),c); Rect(new Rectangle(r.X,r.Bottom-t,r.Width,t),c); Rect(new Rectangle(r.X,r.Y,t,r.Height),c); Rect(new Rectangle(r.Right-t,r.Y,t,r.Height),c); }
    private void Circle(Vector2 p, float radius, Color c) => _batch.Draw(_circle, p, null, c, 0, new Vector2(64), radius / 64, SpriteEffects.None, 0);
    private Texture2D CreateCircle(int size)
    {
        Texture2D result = new(GraphicsDevice, size, size); Color[] pixels = new Color[size * size]; float r = size / 2f;
        for (int y = 0; y < size; y++) for (int x = 0; x < size; x++) pixels[y * size + x] = Vector2.Distance(new Vector2(x,y), new Vector2(r)) <= r ? Color.White : Color.Transparent;
        result.SetData(pixels); return result;
    }
    private int Segment() => (int)(_angle / (MathF.PI / 2)) % 4;
    private static int ActionIndex(PetKind pet) => pet switch { PetKind.Baseline => 0, PetKind.Attacker => 1, _ => 2 };
    private static string Action(PetKind pet) => pet switch { PetKind.Baseline => "Feed", PetKind.Attacker => "Play", _ => "Pet" };
    private static string Name(PetKind pet) => pet switch { PetKind.Baseline => "Mossling", PetKind.Attacker => "Nibbleclaw", _ => "Blinkbun" };
    private static string Pattern(PetKind pet) => pet switch { PetKind.Baseline => "Steady wheel.", PetKind.Attacker => "Attacks after two successes.", _ => "Marker teleports once." };
    private static Vector2 Dir(float a) => new(MathF.Cos(a), MathF.Sin(a));
    private static float Wrap(float a, float b)
    {
        float difference = (a - b + MathF.PI) % Tau;
        return (difference < 0 ? difference + Tau : difference) - MathF.PI;
    }
    private bool Clicked(MouseState m, Rectangle r) => m.LeftButton == ButtonState.Pressed && _previousMouse.LeftButton == ButtonState.Released && r.Contains(m.Position);
    private static Rectangle Button(int y) => new(490, y, 300, 60);
    private enum Screen { Menu, Help, Home, Log, Care, Dodge, Summary }
}
