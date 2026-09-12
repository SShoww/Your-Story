#nullable enable
using System;
using System.IO;
using BePal.Gameplay;
using BePal.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BePal;

public class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private readonly Random _random = new();
    private SpriteBatch _batch = null!;
    private ScreenManager _screenManager = null!;
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
        Texture2D circle = CreateCircle(128);
        Texture2D petIdle = Content.Load<Texture2D>("pet/spr_pet_idle");
        Texture2D petHappy = Content.Load<Texture2D>("pet/spr_pet_happy");
        Texture2D petAngry = Content.Load<Texture2D>("pet/spr_pet_angry");

        ScreenContext context = new(
            font,
            pixel,
            circle,
            petIdle,
            petHappy,
            petAngry,
            Exit,
            SaveScreenshot);

        _screenManager = new ScreenManager(context);
        _screenManager.ShowMenu();
    }

    protected override void Update(GameTime gameTime)
    {
        _screenManager.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(18, 20, 30));
        Vector2 shakeOffset = _screenManager.Context.ShakeTime > 0
            ? new Vector2(
                ((float)_random.NextDouble() * 2 - 1) * _screenManager.Context.ShakeAmount,
                ((float)_random.NextDouble() * 2 - 1) * _screenManager.Context.ShakeAmount)
            : Vector2.Zero;

        _batch.Begin(transformMatrix: Matrix.CreateTranslation(shakeOffset.X, shakeOffset.Y, 0));
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
        if (_playtestFrame == 3)
        {
            SaveScreenshot("screenshots/01_menu.png");
            _screenManager.Context.Run = new PrototypeRun();
            _screenManager.ShowHome("Day 1 begins. Click the pet to begin.");
        }
        else if (_playtestFrame == 6)
        {
            SaveScreenshot("screenshots/02_home.png");
            _screenManager.BeginCare();
        }
        else if (_playtestFrame == 8)
        {
            _screenManager.Context.ClearTags();
            _screenManager.Context.SpawnTag("PERFECT! +1 SATISFACTION", new Color(16, 48, 28), new Color(110, 245, 150));
            _screenManager.Context.SetPetReaction(_screenManager.Context.PetHappy, 0.75f);
        }
        else if (_playtestFrame == 9)
        {
            SaveScreenshot("screenshots/03_care_qte.png");
            _screenManager.Context.ClearTags();
            _screenManager.SetScreen(new DodgeQteScreen(_screenManager.Context));
            _screenManager.Context.SetPetReaction(_screenManager.Context.PetAngry, 0.75f);
            _screenManager.Context.Message = "Attack! Press Space in the gold Dodge Zone.";
        }
        else if (_playtestFrame == 11)
        {
            _screenManager.Context.SpawnTag("DODGED!", new Color(50, 42, 12), new Color(255, 220, 80));
        }
        else if (_playtestFrame == 12)
        {
            SaveScreenshot("screenshots/04_dodge_qte.png");
            _screenManager.Context.ClearTags();
            _screenManager.ShowHome();
            _screenManager.ShowSurvivalLog();
        }
        else if (_playtestFrame == 15)
        {
            SaveScreenshot("screenshots/05_survival_log.png");
            _screenManager.ShowSummary();
        }
        else if (_playtestFrame == 18)
        {
            SaveScreenshot("screenshots/06_summary.png");
            Exit();
        }
    }

    private Texture2D CreateCircle(int size)
    {
        Texture2D result = new(GraphicsDevice, size, size);
        Color[] pixels = new Color[size * size];
        float r = size / 2f;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                pixels[y * size + x] = Vector2.Distance(new Vector2(x, y), new Vector2(r)) <= r
                    ? Color.White
                    : Color.Transparent;
            }
        }
        result.SetData(pixels);
        return result;
    }
}
