using System.IO;
using BePalV2.Audio;
using BePalV2.Gameplay;
using BePalV2.Screens;
using BePalV2.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BePalV2;

public class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _batch = null!;
    private ScreenManager _screenManager = null!;
    private AudioManager? _audioManager;
    private ScreenContext _context = null!;
    private readonly string[] _args;
    private int _playtestFrame;
    private KeyboardState _prevKeyboard;
    private MouseState _prevMouse;

    public ScreenManager ScreenManager => _screenManager;

    public Game1(string[]? args = null)
    {
        _args = args ?? Array.Empty<string>();
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        Window.IsBorderless = true;
        try
        {
            var display = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
            Window.Position = new Point(Math.Max(0, (display.Width - 1280) / 2), Math.Max(0, (display.Height - 720) / 2));
        }
        catch
        {
            // Fallback for headless/CI environments
        }
    }

    protected override void LoadContent()
    {
        _batch = new SpriteBatch(GraphicsDevice);
        SpriteFont font = Content.Load<SpriteFont>("PrototypeFont");
        Texture2D pixel = new(GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });

        Texture2D? petIdle = null;
        Texture2D? petHappy = null;
        Texture2D? petAngry = null;

        try
        {
            petIdle = Content.Load<Texture2D>("pet/spr_pet_idle");
            petHappy = Content.Load<Texture2D>("pet/spr_pet_happy");
            petAngry = Content.Load<Texture2D>("pet/spr_pet_angry");
        }
        catch
        {
            // Tolerate missing pet textures in tests
        }

        _audioManager = new AudioManager(Content);

        _context = new ScreenContext
        {
            GraphicsDevice = GraphicsDevice,
            SpriteBatch = _batch,
            Font = font,
            Pixel = pixel,
            PetIdleTex = petIdle,
            PetHappyTex = petHappy,
            PetAngryTex = petAngry,
            Audio = _audioManager,
            Run = new V2RunState(PetSpecies.Coco)
        };

        _screenManager = new ScreenManager(_context);

        bool auto = Array.Exists(_args, a => a is "--screenshot" or "--playtest") ||
                    Environment.GetEnvironmentVariable("BEPAL_SCREENSHOT") == "1";
        if (auto)
        {
            _audioManager.IsMuted = true;
        }

        _screenManager.SetScreen(new MainMenuScreen(_context), transition: false);
    }

    protected override void UnloadContent()
    {
        _audioManager?.Dispose();
        base.UnloadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState kstate = Keyboard.GetState();

        if (kstate.IsKeyDown(Keys.F11) && !_prevKeyboard.IsKeyDown(Keys.F11))
        {
            _graphics.ToggleFullScreen();
        }

        _screenManager.Update(gameTime);
        base.Update(gameTime);

        _prevKeyboard = kstate;
        _prevMouse = Mouse.GetState();
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(UITheme.BgDeep);

        _batch.Begin();
        _screenManager.Draw(gameTime, _batch);

        MouseState mstate = Mouse.GetState();
        bool mouseClicked = mstate.LeftButton == ButtonState.Released && _prevMouse.LeftButton == ButtonState.Pressed;
        CleanUI.DrawWindowHeader(_batch, _context.Font, 1280, mstate.Position, mouseClicked,
            onToggleFullscreen: () => _graphics.ToggleFullScreen(),
            onClose: () => Exit());

        _batch.End();

        base.Draw(gameTime);
        CheckPlaytestAndScreenshot();
    }

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
        bool auto = Array.Exists(_args, a => a is "--screenshot" or "--playtest") ||
                    Environment.GetEnvironmentVariable("BEPAL_SCREENSHOT") == "1";
        if (!auto) return;

        _playtestFrame++;

        switch (_playtestFrame)
        {
            case 2:
                // 01: Main menu matching Slide 4 (Spacebar, WASD, mouse graphic)
                SaveScreenshot("screenshots/v2/01_menu.png");
                _screenManager.SetScreen(new ChoosePetScreen(_context, showIntro: false), transition: false);
                break;

            case 5:
                // 02: Pet selection matching Slide 9 (3 starter cards)
                SaveScreenshot("screenshots/v2/02_choose_pet.png");
                _context.Run = new V2RunState(PetSpecies.Coco);
                _screenManager.SetScreen(new BaseHabitatScreen(_context), transition: false);
                break;

            case 8:
                // 03: Base habitat room matching Slide 13 (door, doctor, desk, bench, active pet)
                SaveScreenshot("screenshots/v2/03_habitat_room.png");
                _screenManager.SetScreen(new CareQteScreen(_context, CareActionType.Train), transition: false);
                break;

            case 11:
                // 04: Care QTE wheel matching Slide 15/17 (4 nodes, attempts counter, action behavior)
                SaveScreenshot("screenshots/v2/04_care_qte.png");
                var roomLog = new BaseHabitatScreen(_context);
                roomLog.OpenLogModal(0);
                _screenManager.SetScreen(roomLog, transition: false);
                break;

            case 14:
                // 05: Survival Logbook matching Slide 22/25 (pet discovery & disaster tabs)
                SaveScreenshot("screenshots/v2/05_survival_log.png");
                var roomUpg = new BaseHabitatScreen(_context);
                roomUpg.OpenUpgradeModal();
                _screenManager.SetScreen(roomUpg, transition: false);
                break;

            case 17:
                // 06: Upgrade Station matching Slide 27 (3 upgrade cards)
                SaveScreenshot("screenshots/v2/06_upgrade_station.png");
                _screenManager.SetScreen(new CombatArenaScreen(_context, CombatMode.ToothlessTaming), transition: false);
                break;

            case 20:
                // 07: Day 2 Toothless Taming Combat Wheel matching Slide 38
                SaveScreenshot("screenshots/v2/07_toothless_arena.png");
                _screenManager.SetScreen(new ShopModalScreen(_context), transition: false);
                break;

            case 23:
                // 08: Day 3 Merchant Shop matching Slide 59 (5 items & merchant speech)
                SaveScreenshot("screenshots/v2/08_merchant_shop.png");
                _screenManager.SetScreen(new CombatArenaScreen(_context, CombatMode.MerchantBoss), transition: false);
                break;

            case 26:
                // 09: Day 3 Merchant Boss Fight matching Slide 54 (5-hit gauge & gold theft)
                SaveScreenshot("screenshots/v2/09_boss_battle.png");
                _screenManager.SetScreen(new DailySummaryScreen(_context), transition: false);
                break;

            case 29:
                // 10: Daily Debriefing and Shift Summary matching Slide 30
                SaveScreenshot("screenshots/v2/10_summary_report.png");
                Exit();
                break;
        }
    }
}
