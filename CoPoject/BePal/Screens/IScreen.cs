#nullable enable
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BePal.Screens;

/// <summary>
/// Defines the lifecycle for a modular screen view in BePal.
/// </summary>
public interface IScreen
{
    /// <summary>
    /// Gets whether this screen renders as a modal overlay on top of the underlying screen.
    /// </summary>
    bool IsOverlay => false;

    /// <summary>
    /// Updates screen state, timers, and input polling.
    /// </summary>
    void Update(GameTime gameTime);

    /// <summary>
    /// Renders screen visuals via the provided SpriteBatch pass.
    /// </summary>
    void Draw(GameTime gameTime, SpriteBatch spriteBatch);
}
