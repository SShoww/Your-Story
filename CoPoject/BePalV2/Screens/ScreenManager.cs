using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BePalV2.Screens;

public sealed class ScreenManager
{
    private readonly Stack<IScreen> _screenStack = new();
    public ScreenContext Context { get; }
    public StripeWipeTransition? ActiveTransition { get; private set; }

    public float DefaultTransitionDuration { get; set; } = 0.65f;

    public IScreen? ActiveScreen => _screenStack.Count > 0 ? _screenStack.Peek() : null;
    public int ScreenCount => _screenStack.Count;
    public bool IsTransitionActive => ActiveTransition != null && ActiveTransition.IsActive;

    public ScreenManager(ScreenContext context)
    {
        Context = context;
        Context.ScreenManager = this;
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
        if (_screenStack.Count > 1 && _screenStack.Peek().IsOverlay)
        {
            _screenStack.Pop();
        }
    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (ActiveTransition != null && ActiveTransition.IsActive)
        {
            ActiveTransition.Update(dt);
            return; // Gate input during transition
        }

        if (_screenStack.Count > 0)
        {
            _screenStack.Peek().Update(gameTime);
        }
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        if (_screenStack.Count == 0) return;

        // Find bottom-most screen to start drawing from if overlays exist
        var screens = _screenStack.ToArray(); // Top is [0]
        Array.Reverse(screens); // Bottom is [0]

        foreach (var scr in screens)
        {
            scr.Draw(gameTime, spriteBatch);
        }

        // Transition on top
        if (ActiveTransition != null && ActiveTransition.IsActive)
        {
            ActiveTransition.Draw(spriteBatch, Context.Pixel, Context.ScreenWidth, Context.ScreenHeight);
        }
    }
}
