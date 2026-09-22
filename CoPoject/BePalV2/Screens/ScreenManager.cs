using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BePalV2.Screens;

public sealed class ScreenManager
{
    private readonly Stack<IScreen> _screenStack = new();
    public ScreenContext Context { get; }
    public StripeWipeTransition? ActiveTransition { get; private set; }

    public float DefaultTransitionDuration { get; set; } = 0.65f;
    public Action? OnExitGame { get; set; }

    public IScreen? ActiveScreen => _screenStack.Count > 0 ? _screenStack.Peek() : null;
    public int ScreenCount => _screenStack.Count;
    public bool IsTransitionActive => ActiveTransition != null && ActiveTransition.IsActive;

    public ScreenManager(ScreenContext context)
    {
        Context = context;
        Context.ScreenManager = this;
    }

    public void ExitGame()
    {
        OnExitGame?.Invoke();
    }

    public void SetScreen(IScreen screen, bool transition = true, float? duration = null)
    {
        if (!transition)
        {
            _screenStack.Clear();
            _screenStack.Push(screen);
            return;
        }

        var outgoing = ActiveScreen;
        float dur = duration ?? DefaultTransitionDuration;

        ActiveTransition = new StripeWipeTransition(
            fromScreen: outgoing,
            toScreen: screen,
            duration: dur,
            onMidpoint: () =>
            {
                _screenStack.Clear();
                _screenStack.Push(screen);
            },
            onComplete: () =>
            {
                ActiveTransition = null;
            }
        );
    }

    public void PushOverlay(IScreen overlayScreen)
    {
        _screenStack.Push(overlayScreen);
    }

    public void PopOverlay()
    {
        if (_screenStack.Count > 1)
        {
            _screenStack.Pop();
        }
    }

    public void Update(GameTime gameTime)
    {
        if (ActiveTransition != null && ActiveTransition.IsActive)
        {
            ActiveTransition.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            return;
        }

        if (_screenStack.Count > 0)
        {
            _screenStack.Peek().Update(gameTime);
        }
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        if (_screenStack.Count == 0) return;

        var screensToDraw = new List<IScreen>();
        foreach (var screen in _screenStack)
        {
            screensToDraw.Add(screen);
            if (!screen.IsOverlay)
            {
                break;
            }
        }
        screensToDraw.Reverse();

        foreach (var screen in screensToDraw)
        {
            screen.Draw(gameTime, spriteBatch);
        }

        if (ActiveTransition != null && ActiveTransition.IsActive)
        {
            ActiveTransition.Draw(spriteBatch, Context.Pixel, Context.ScreenWidth, Context.ScreenHeight);
        }
    }
}
