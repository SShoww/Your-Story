#nullable enable
using System;
using System.Collections.Generic;
using BePal.Gameplay;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BePal.Screens;

/// <summary>
/// Manages the active screen stack, transitions, input propagation, and rendering passes.
/// </summary>
public sealed class ScreenManager
{
    private readonly Stack<IScreen> _screens = new();
    public ScreenContext Context { get; }

    public IScreen? CurrentScreen => _screens.Count > 0 ? _screens.Peek() : null;
    public int StackCount => _screens.Count;

    public ScreenManager(ScreenContext context)
    {
        Context = context;
        Context.Manager = this;
    }

    public void SetScreen(IScreen screen)
    {
        _screens.Clear();
        _screens.Push(screen);
    }

    public void PushScreen(IScreen screen)
    {
        _screens.Push(screen);
    }

    public void PopScreen()
    {
        if (_screens.Count > 1)
        {
            _screens.Pop();
        }
    }

    public void ShowMenu()
    {
        SetScreen(new MainMenuScreen(Context));
    }

    public void ShowPrologue()
    {
        SetScreen(new PrologueScreen(Context));
    }

    public void ShowDoorstep()
    {
        SetScreen(new DoorstepScreen(Context));
    }

    public void ShowHome(string? message = null)
    {
        if (message != null) Context.Message = message;
        SetScreen(new PanoramicRoomScreen(Context, 0));
    }

    public void ShowRoom(int initialWall = 0, string? message = null)
    {
        if (message != null) Context.Message = message;
        SetScreen(new PanoramicRoomScreen(Context, initialWall));
    }

    public void BeginCare()
    {
        Context.ResetPetReaction();
        PetDefinition pet = PetCatalog.Get(Context.Run.ActivePet);
        Context.Message = $"Time your Spacebar when the needle enters {pet.Pattern.PreferredAction}!";
        SetScreen(new CareQteScreen(Context));
    }

    public void BeginDodge()
    {
        Context.SetPetReaction(Context.PetAngry, 0.75f);
        Context.TriggerShake(0.3f, 8f);
        Context.SpawnTag("WARNING: ATTACK INCOMING!", new Color(60, 15, 20), new Color(255, 200, 70));
        Context.Message = "ATTACK! Press Space inside the gold Dodge Zone!";
        SetScreen(new DodgeQteScreen(Context));
    }

    public void ShowSurvivalLog()
    {
        PushScreen(new SurvivalLogScreen(Context));
    }

    public void ShowSummary()
    {
        SetScreen(new SummaryScreen(Context));
    }

    public void Fail(string message)
    {
        Context.Run.TakeDamage();
        if (Context.Run.Health <= 0)
        {
            AdvanceDay("Forced retreat! You collapsed and were rushed to safety.");
        }
        else
        {
            Context.Message = message;
            SetScreen(new PanoramicRoomScreen(Context, 0));
        }
    }

    public void AdvanceDay(string message)
    {
        if (Context.Run.IsComplete)
        {
            ShowSummary();
            return;
        }

        Context.ResetPetReaction();
        Context.Run.EndDay();
        Context.Message = $"Day {Context.Run.DayNumber} begins. {message}";
        ShowDoorstep();
    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (Context.ReactionTime > 0)
        {
            Context.ReactionTime -= dt;
            if (Context.ReactionTime <= 0)
                Context.PetImage = Context.PetIdle;
        }

        if (Context.ShakeTime > 0)
            Context.ShakeTime -= dt;

        for (int i = Context.Tags.Count - 1; i >= 0; i--)
        {
            Context.Tags[i].Lifetime -= dt;
            Context.Tags[i].Position.Y -= 35f * dt;
            if (Context.Tags[i].Lifetime <= 0)
                Context.Tags.RemoveAt(i);
        }

        KeyboardState keyboard = Keyboard.GetState();
        MouseState mouse = Mouse.GetState();
        Context.Keyboard = keyboard;
        Context.Mouse = mouse;

        if (Context.IsKeyPressed(Keys.Escape))
        {
            if (CurrentScreen is MainMenuScreen or SummaryScreen)
            {
                Context.ExitGame();
                return;
            }
            if (_screens.Count > 1)
            {
                PopScreen();
            }
            else
            {
                ShowHome();
            }
        }

        if (Context.IsKeyPressed(Keys.F12))
        {
            string stamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            Context.SaveScreenshotAction($"screenshots/bepal_{stamp}.png");
            Context.SpawnTag("SCREENSHOT CAPTURED", new Color(24, 45, 65), Color.White);
        }

        CurrentScreen?.Update(gameTime);

        Context.PreviousKeyboard = keyboard;
        Context.PreviousMouse = mouse;
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        if (CurrentScreen == null) return;

        if (CurrentScreen.IsOverlay && _screens.Count > 1)
        {
            IScreen[] stackArray = _screens.ToArray();
            Array.Reverse(stackArray);
            foreach (IScreen screen in stackArray)
            {
                screen.Draw(gameTime, spriteBatch);
            }
        }
        else
        {
            CurrentScreen.Draw(gameTime, spriteBatch);
        }
    }
}
