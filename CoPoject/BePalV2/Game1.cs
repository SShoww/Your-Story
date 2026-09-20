using System.IO;
using BePalV2.Audio;
using BePalV2.Gameplay;
using BePalV2.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

    public ScreenManager ScreenManager => _screenManager;

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
        _screenManager.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(18, 20, 30));

        _batch.Begin();
        _screenManager.Draw(gameTime, _batch);
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
                SaveScreenshot("screenshots/v2/01_menu.png");
                _screenManager.SetScreen(new ChoosePetScreen(_context), transition: false);
                break;

            case 5:
                SaveScreenshot("screenshots/v2/02_choose_pet.png");
                _context.Run = new V2RunState(PetSpecies.Coco);
                _screenManager.SetScreen(new BaseHabitatScreen(_context), transition: false);
                break;

            case 8:
                SaveScreenshot("screenshots/v2/03_habitat_room.png");
                _screenManager.SetScreen(new CareQteScreen(_context, CareActionType.Feed), transition: false);
                break;

            case 11:
                SaveScreenshot("screenshots/v2/04_care_qte.png");
                _screenManager.SetScreen(new CalmingQteScreen(_context), transition: false);
                break;

            case 14:
                SaveScreenshot("screenshots/v2/05_thunderstorm.png");
                _screenManager.SetScreen(new CombatArenaScreen(_context, CombatMode.ToothlessTaming), transition: false);
                break;

            case 17:
                SaveScreenshot("screenshots/v2/06_toothless_arena.png");
                _context.Run.UnlockToothless();
                _screenManager.SetScreen(new ShopModalScreen(_context), transition: false);
                break;

            case 20:
                SaveScreenshot("screenshots/v2/07_merchant_shop.png");
                _screenManager.SetScreen(new CombatArenaScreen(_context, CombatMode.MerchantBoss), transition: false);
                break;

            case 23:
                SaveScreenshot("screenshots/v2/08_boss_battle.png");
                _screenManager.SetScreen(new DailySummaryScreen(_context), transition: false);
                break;

            case 26:
                SaveScreenshot("screenshots/v2/09_summary_report.png");
                _screenManager.SetScreen(new EndingScreen(_context, StoryEnding.EndingB_Protector), transition: false);
                break;

            case 29:
                SaveScreenshot("screenshots/v2/10_ending.png");
                Exit();
                break;
        }
    }
}
