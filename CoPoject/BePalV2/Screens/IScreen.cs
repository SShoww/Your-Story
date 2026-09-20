using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BePalV2.Screens;

public interface IScreen
{
    bool IsOverlay => false;
    void Update(GameTime gameTime);
    void Draw(GameTime gameTime, SpriteBatch spriteBatch);
}
