using System;
using System.Collections.Generic;
using System.IO;
using BePal.Gameplay;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
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
    private readonly string[] _args;
    private int _playtestFrame;

    private float _shakeTime;
    private float _shakeAmount = 8f;
    private float _reactionTime;
    private readonly List<FloatingTag> _tags = new();

    private static readonly string[] Actions = { "Feed", "Play", "Pet", "Observe" };
    private static readonly string[] ActionNeeds = { "Appetite", "Recreation", "Intimacy", "Observation" };
    private static readonly Color[] ActionColors =
    {
        new(72, 205, 130),
        new(65, 170, 245),
        new(245, 115, 165),
        new(180, 125, 245)
    };
    public Game1(string[]? args = null)
    {
        _args = args ?? Array.Empty<string>();
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
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (_reactionTime > 0)
        {
            _reactionTime -= dt;
            if (_reactionTime <= 0)
                _petImage = _petIdle;
        }
        if (_shakeTime > 0) _shakeTime -= dt;

        for (int i = _tags.Count - 1; i >= 0; i--)
        {
            _tags[i].Lifetime -= dt;
            _tags[i].Position.Y -= 35f * dt;
            if (_tags[i].Lifetime <= 0) _tags.RemoveAt(i);
        }

        KeyboardState keyboard = Keyboard.GetState();
        MouseState mouse = Mouse.GetState();
        if (keyboard.IsKeyDown(Keys.Escape))
            _screen = _screen is Screen.Menu or Screen.Summary ? ExitScreen() : Screen.Home;
        if (keyboard.IsKeyDown(Keys.F12) && _previousKeyboard.IsKeyUp(Keys.F12))
        {
            string stamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            SaveScreenshot($"screenshots/bepal_{stamp}.png");
            SpawnTag("SCREENSHOT CAPTURED", new Color(24, 45, 65), Color.White);
        }
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
        GraphicsDevice.Clear(new Color(18, 20, 30));
        Vector2 shakeOffset = _shakeTime > 0
            ? new Vector2(((float)_random.NextDouble() * 2 - 1) * _shakeAmount, ((float)_random.NextDouble() * 2 - 1) * _shakeAmount)
            : Vector2.Zero;

        _batch.Begin(transformMatrix: Matrix.CreateTranslation(shakeOffset.X, shakeOffset.Y, 0));
        if (_screen == Screen.Menu) DrawMenu();
        else if (_screen == Screen.Help) DrawHelp();
        else if (_screen == Screen.Home) DrawHome();
        else if (_screen == Screen.Log) { DrawHome(); DrawLog(); }
        else if (_screen is Screen.Care or Screen.Dodge)
        {
            DrawQte(_screen == Screen.Dodge);
            DrawFloatingTags();
        }
        else DrawSummary();

        _batch.End();
        base.Draw(gameTime);
        CheckPlaytestAndScreenshot();
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
        if (Clicked(mouse, new Rectangle(530, 165, 220, 290)) || Clicked(mouse, new Rectangle(490, 480, 300, 48))) BeginCare();
        else if (Clicked(mouse, new Rectangle(50, 595, 250, 60)) && _run.CanEndDay) { _run.EndDay(); AdvanceDay("You ended the day safely."); }
        else if (Clicked(mouse, new Rectangle(980, 595, 250, 60))) _screen = Screen.Log;
    }
    private void UpdateQte(GameTime time, KeyboardState keyboard)
    {
        _angle = (_angle + 2.2f * (float)time.ElapsedGameTime.TotalSeconds) % Tau;
        _qteTime += (float)time.ElapsedGameTime.TotalSeconds;
        if (_screen == Screen.Care && _run.ActivePet == PetKind.Trickster && !_teleported && _qteTime >= _teleportAt)
        {
            _angle = (float)_random.NextDouble() * Tau;
            _teleported = true;
            TriggerShake(0.18f, 5f);
            SpawnTag("MARKER TELEPORTED!", new Color(45, 25, 55), new Color(230, 160, 255));
            _message = "The marker teleported!";
        }
        if (keyboard.IsKeyDown(Keys.Space) && _previousKeyboard.IsKeyUp(Keys.Space))
        {
            if (_screen == Screen.Care) ResolveCare(); else ResolveDodge();
        }
    }

    private void BeginCare()
    {
        _petImage = _petIdle;
        _reactionTime = 0;
        _screen = Screen.Care;
        ResetQte();
        _message = $"Time your Spacebar when the needle enters {Action(_run.ActivePet)}!";
    }

    private int? GetHoveredActionIndex()
    {
        for (int i = 0; i < 4; i++)
        {
            float centerA = i * MathF.PI / 2f + MathF.PI / 4f;
            float diff = MathF.Abs(Wrap(_angle, centerA));
            if (diff <= MathF.PI / 6f) return i;
        }
        return null;
    }

    private void ResolveCare()
    {
        int? actionIdx = GetHoveredActionIndex();
        if (actionIdx == null)
        {
            TriggerShake(0.24f, 9f);
            _petImage = _petAngry;
            _reactionTime = 0.65f;
            SpawnTag("MISSED! -1 HP", new Color(45, 12, 18), new Color(255, 80, 80));
            Fail("Hit the dead zone! Lost 1 Health.");
            return;
        }

        int targetIdx = ActionIndex(_run.ActivePet);
        if (actionIdx.Value == targetIdx)
        {
            _run.RecordCareSuccess();
            _petImage = _petHappy;
            _reactionTime = 0.75f;
            SpawnTag("PERFECT! +1 SATISFACTION", new Color(16, 48, 28), new Color(110, 245, 150));

            if (_run.ActivePet == PetKind.Attacker && _run.Satisfaction == 2)
            {
                _screen = Screen.Dodge;
                ResetQte();
                _petImage = _petAngry;
                TriggerShake(0.3f, 8f);
                SpawnTag("WARNING: ATTACK INCOMING!", new Color(60, 15, 20), new Color(255, 200, 70));
                _message = "ATTACK! Press Space inside the gold Dodge Zone!";
            }
            else if (_run.Satisfaction >= 3)
            {
                _run.CompleteSession();
                _screen = Screen.Home;
                _petImage = _petHappy;
                _reactionTime = 1.2f;
                SpawnTag("SESSION COMPLETE!", new Color(25, 45, 60), new Color(255, 230, 130));
                _message = "Session complete! Satisfaction is full.";
            }
            else
            {
                ResetQte();
                _message = "Correct action! Keep going.";
            }
        }
        else
        {
            TriggerShake(0.24f, 9f);
            _petImage = _petAngry;
            _reactionTime = 0.65f;
            string chosen = Actions[actionIdx.Value];
            SpawnTag($"REJECTED: {chosen.ToUpperInvariant()}! -1 HP", new Color(45, 12, 18), new Color(255, 80, 80));
            Fail($"Disliked {chosen}! Lost 1 Health.");
        }
    }

    private void ResolveDodge()
    {
        float dodgeCenter = MathF.PI * 1.5f;
        float diff = MathF.Abs(Wrap(_angle, dodgeCenter));
        if (diff <= MathF.PI / 6f)
        {
            _screen = Screen.Care;
            ResetQte();
            _petImage = _petIdle;
            _reactionTime = 0;
            SpawnTag("DODGED!", new Color(50, 42, 12), new Color(255, 220, 80));
            _message = "Attack evaded! One final care action remains.";
        }
        else
        {
            TriggerShake(0.35f, 13f);
            _petImage = _petAngry;
            _reactionTime = 0.75f;
            SpawnTag("HIT BY ATTACK! -1 HP", new Color(65, 10, 15), new Color(255, 70, 70));
            Fail("Failed to dodge attack! Lost 1 Health.");
        }
    }

    private void Fail(string message)
    {
        if (_run.TakeDamage())
        {
            AdvanceDay("Forced Retreat. You recovered.");
            return;
        }
        ResetQte();
        _message = message;
    }

    private void AdvanceDay(string message)
    {
        if (_run.IsComplete) _screen = Screen.Summary;
        else
        {
            _petImage = _petIdle;
            _screen = Screen.Home;
            _message = $"{message} Day {_run.DayNumber} begins.";
        }
    }

    private void ResetQte()
    {
        _angle = 0;
        _qteTime = 0;
        _teleported = false;
        _teleportAt = 0.45f + (float)_random.NextDouble() * 0.85f;
    }

    private void TriggerShake(float time = 0.22f, float amount = 8f)
    {
        _shakeTime = time;
        _shakeAmount = amount;
    }

    private void SpawnTag(string text, Color bg, Color fg)
    {
        float rot = ((float)_random.NextDouble() - 0.5f) * 0.18f;
        _tags.Add(new FloatingTag
        {
            Text = text,
            Position = new Vector2(640, 235),
            BgColor = bg,
            TextColor = fg,
            Lifetime = 0.85f,
            MaxLifetime = 0.85f,
            Rotation = rot
        });
    }

    private sealed class FloatingTag
    {
        public string Text { get; set; } = "";
        public Vector2 Position;
        public Color BgColor;
        public Color TextColor;
        public float Lifetime = 0.85f;
        public float MaxLifetime = 0.85f;
        public float Rotation;
    }

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
        DrawHUD();

        Vector2 c = new(640, 310);
        Rectangle frame = new((int)c.X - 110, (int)c.Y - 145, 220, 290);
        _batch.FillRectangle(new RectangleF(frame.X, frame.Y, frame.Width, frame.Height), new Color(14, 16, 24, 210));
        _batch.Draw(_petImage, new Rectangle((int)c.X - 90, (int)c.Y - 125, 180, 250), Color.White);
        _batch.DrawRectangle(new RectangleF(frame.X, frame.Y, frame.Width, frame.Height), new Color(255, 203, 139), 2f);

        Rectangle careBtn = new(490, 480, 300, 48);
        Button(careBtn, $"CARE FOR {Name(_run.ActivePet).ToUpperInvariant()}", true);

        Center(_message, new Vector2(640, 550), 0.76f, new Color(255, 225, 165));
        Button(new Rectangle(50, 595, 250, 60), "End Day", _run.CanEndDay);
        Button(new Rectangle(980, 595, 250, 60), "Survival Log", true);
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
        Vector2 c = new(640, 360);
        float trackRadius = 215f;

        // 1. Central Creature Display with Vignette Backdrop
        _batch.FillRectangle(new RectangleF(c.X - 110, c.Y - 145, 220, 290), new Color(14, 16, 24, 210));
        _batch.Draw(_petImage, new Rectangle((int)c.X - 90, (int)c.Y - 125, 180, 250), Color.White);

        if (dodge)
        {
            _batch.DrawRectangle(new RectangleF(c.X - 95, c.Y - 130, 190, 260), new Color(255, 65, 75), 3f);
            RectangleF alertRect = new(c.X - 80, c.Y - 26, 160, 48);
            _batch.FillRectangle(alertRect, new Color(175, 25, 35));
            _batch.DrawRectangle(alertRect, Color.White, 2f);
            Center("ATTACK!", new Vector2(c.X, c.Y - 2), 1.05f, Color.White);
        }
        else
        {
            _batch.DrawRectangle(new RectangleF(c.X - 95, c.Y - 130, 190, 260), new Color(42, 50, 72), 2f);
        }

        // 2. Circular Ring Track (Death Spiral style)
        Color trackBg = dodge ? new Color(55, 22, 30) : new Color(34, 40, 58);
        Color trackBorder = dodge ? new Color(115, 35, 45) : new Color(22, 26, 38);
        _batch.DrawCircle(c, trackRadius, 64, trackBg, 18f);
        _batch.DrawCircle(c, trackRadius - 10f, 64, trackBorder, 2f);
        _batch.DrawCircle(c, trackRadius + 10f, 64, trackBorder, 2f);

        // 3. Action Sectors or Dodge Zone
        if (dodge)
        {
            float dodgeCenter = MathF.PI * 1.5f;
            float dodgeSpan = MathF.PI / 3f;
            bool inDodge = MathF.Abs(Wrap(_angle, dodgeCenter)) <= MathF.PI / 6f;
            Color dodgeColor = inDodge ? Color.White : new Color(255, 215, 65);

            _batch.DrawArc(c, trackRadius, dodgeCenter - dodgeSpan / 2f, dodgeSpan, 32, dodgeColor, inDodge ? 24f : 20f);

            Vector2 badgePos = c + Dir(dodgeCenter) * (trackRadius + 56f);
            DrawScrapBadge(badgePos, "DODGE ZONE", "SPACE TO EVADE", dodgeColor, Color.White, inDodge);
        }
        else
        {
            int? hovered = GetHoveredActionIndex();
            for (int i = 0; i < 4; i++)
            {
                float centerA = i * MathF.PI / 2f + MathF.PI / 4f;
                float span = MathF.PI / 3f;
                bool isHovered = hovered == i;
                Color baseCol = ActionColors[i];
                Color arcCol = isHovered ? Color.Lerp(baseCol, Color.White, 0.45f) : baseCol;

                _batch.DrawArc(c, trackRadius, centerA - span / 2f, span, 32, arcCol, isHovered ? 24f : 18f);

                Vector2 badgePos = c + Dir(centerA) * (trackRadius + 58f);
                DrawScrapBadge(badgePos, Actions[i].ToUpperInvariant(), ActionNeeds[i], arcCol, Color.White, isHovered);
            }
        }

        // 4. Rotating Needle Marker
        Vector2 innerPt = c + Dir(_angle) * (trackRadius - 22f);
        Vector2 outerPt = c + Dir(_angle) * (trackRadius + 24f);
        _batch.DrawLine(innerPt, outerPt, Color.White, 8f);
        _batch.DrawCircle(outerPt, 4f, 16, Color.Gold, 2f);

        // 5. HUD & Status
        DrawHUD();
        DrawSatisfactionAndPrompt(dodge);
    }

    private void DrawScrapBadge(Vector2 center, string title, string subtitle, Color accentColor, Color textColor, bool highlight)
    {
        Vector2 titleSize = _font.MeasureString(title) * 0.72f;
        Vector2 subSize = _font.MeasureString(subtitle) * 0.52f;
        float w = MathF.Max(titleSize.X, subSize.X) + 24f;
        float h = 46f;
        RectangleF rect = new(center.X - w / 2f, center.Y - h / 2f, w, h);

        Color bg = highlight ? new Color(48, 56, 80) : new Color(24, 27, 40);
        _batch.FillRectangle(rect, bg);
        _batch.DrawRectangle(rect, highlight ? Color.White : accentColor, highlight ? 3f : 2f);

        Center(title, new Vector2(center.X, center.Y - 8f), 0.70f, highlight ? Color.White : accentColor);
        Center(subtitle, new Vector2(center.X, center.Y + 11f), 0.50f, Color.LightGray);
    }

    private void DrawHUD()
    {
        // Health Badge Top-Left (Death Spiral style)
        RectangleF hpRect = new(36, 20, 260, 70);
        _batch.FillRectangle(hpRect, new Color(22, 25, 38));
        _batch.DrawRectangle(hpRect, new Color(255, 100, 110), 2f);
        string hearts = "";
        for (int i = 1; i <= 3; i++) hearts += i <= _run.Health ? "[X] " : "[ ] ";
        Center($"HEALTH {_run.Health}.0", new Vector2(hpRect.Center.X, hpRect.Y + 20f), 0.85f, Color.White);
        Center($"LIVES: {hearts.Trim()}", new Vector2(hpRect.Center.X, hpRect.Y + 48f), 0.72f, new Color(255, 140, 140));

        // Pet Info Top-Right
        RectangleF petRect = new(860, 20, 384, 70);
        _batch.FillRectangle(petRect, new Color(22, 25, 38));
        _batch.DrawRectangle(petRect, new Color(255, 203, 139), 2f);
        string hazard = _run.ActivePet switch
        {
            PetKind.Baseline => "Hazard Lv 1 (Physical)",
            PetKind.Attacker => "Hazard Lv 2 (Aggressive)",
            _ => "Hazard Lv 3 (Teleporting)"
        };
        Center($"DAY {_run.DayNumber} / 5", new Vector2(petRect.Center.X, petRect.Y + 20f), 0.85f, Color.White);
        Center($"{Name(_run.ActivePet).ToUpperInvariant()} - {hazard}", new Vector2(petRect.Center.X, petRect.Y + 48f), 0.68f, new Color(255, 203, 139));
    }
    private void DrawSatisfactionAndPrompt(bool dodge)
    {
        Vector2 c = new(640, 610);
        if (dodge)
        {
            Center("PRESS SPACE INSIDE THE GOLD DODGE ZONE TO EVADE", c, 0.85f, new Color(255, 215, 65));
        }
        else
        {
            string pips = "SATISFACTION: ";
            for (int i = 1; i <= 3; i++) pips += i <= _run.Satisfaction ? "[#] " : "[.] ";
            pips += $"({_run.Satisfaction} / 3)";
            Center(pips, c, 0.95f, new Color(110, 240, 160));
        }

        Center(_message, new Vector2(640, 655), 0.72f, new Color(255, 225, 165));
    }

    private void DrawFloatingTags()
    {
        for (int i = 0; i < _tags.Count; i++)
        {
            var tag = _tags[i];
            float alpha = MathF.Min(1f, tag.Lifetime / 0.22f);
            Vector2 size = _font.MeasureString(tag.Text) * 0.80f;
            RectangleF r = new(tag.Position.X - size.X / 2f - 16f, tag.Position.Y - size.Y / 2f - 8f, size.X + 32f, size.Y + 16f);

            _batch.FillRectangle(r, tag.BgColor * alpha);
            _batch.DrawRectangle(r, tag.TextColor * alpha, 2f);
            Center(tag.Text, tag.Position, 0.78f, tag.TextColor * alpha);
        }
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
    private void Outline(Rectangle r, Color c, int t) => _batch.DrawRectangle(new RectangleF(r.X, r.Y, r.Width, r.Height), c, t);
    private void Circle(Vector2 p, float radius, Color c) => _batch.DrawCircle(p, radius, 32, c, 2f);
    private void Arc(Vector2 p, float radius, float start, float sweep, Color c, float t = 2f) => _batch.DrawArc(p, radius, start, sweep, 32, c, t);
    public void SaveScreenshot(string path)
    {
        string? dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
        int w = GraphicsDevice.PresentationParameters.BackBufferWidth;
        int h = GraphicsDevice.PresentationParameters.BackBufferHeight;
        using Texture2D target = new(GraphicsDevice, w, h);
        Color[] data = new Color[w * h];
        GraphicsDevice.GetBackBufferData(data);
        target.SetData(data);
        using FileStream fs = File.Create(path);
        target.SaveAsPng(fs, w, h);
    }

    private void CheckPlaytestAndScreenshot()
    {
        bool auto = Array.Exists(_args, a => a is "--screenshot" or "--playtest") || Environment.GetEnvironmentVariable("BEPAL_SCREENSHOT") == "1";
        if (!auto) return;
        _playtestFrame++;
        if (_playtestFrame == 3)
        {
            SaveScreenshot("screenshots/01_menu.png");
            _run = new PrototypeRun();
            _screen = Screen.Home;
            _message = "Day 1 begins. Click the pet to begin.";
        }
        else if (_playtestFrame == 6)
        {
            SaveScreenshot("screenshots/02_home.png");
            BeginCare();
        }
        else if (_playtestFrame == 8)
        {
            _tags.Clear();
            SpawnTag("PERFECT! +1 SATISFACTION", new Color(16, 48, 28), new Color(110, 245, 150));
            _petImage = _petHappy;
        }
        else if (_playtestFrame == 9)
        {
            SaveScreenshot("screenshots/03_care_qte.png");
            _tags.Clear();
            _screen = Screen.Dodge;
            ResetQte();
            _petImage = _petAngry;
            _message = "Attack! Press Space in the gold Dodge Zone.";
        }
        else if (_playtestFrame == 11)
        {
            SpawnTag("DODGED!", new Color(50, 42, 12), new Color(255, 220, 80));
        }
        else if (_playtestFrame == 12)
        {
            SaveScreenshot("screenshots/04_dodge_qte.png");
            _tags.Clear();
            _screen = Screen.Log;
        }
        else if (_playtestFrame == 15)
        {
            SaveScreenshot("screenshots/05_survival_log.png");
            _screen = Screen.Summary;
        }
        else if (_playtestFrame == 18)
        {
            SaveScreenshot("screenshots/06_summary.png");
            Exit();
        }
    }
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
