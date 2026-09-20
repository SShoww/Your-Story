using BePalV2.Audio;
using BePalV2.Gameplay;
using Microsoft.Xna.Framework.Graphics;

namespace BePalV2.Screens;

public sealed class ScreenContext
{
    public GraphicsDevice GraphicsDevice { get; set; } = null!;
    public SpriteBatch SpriteBatch { get; set; } = null!;
    public SpriteFont Font { get; set; } = null!;
    public Texture2D Pixel { get; set; } = null!;
    public Texture2D? PetIdleTex { get; set; }
    public Texture2D? PetHappyTex { get; set; }
    public Texture2D? PetAngryTex { get; set; }

    public IAudioService Audio { get; set; } = new NullAudioService();
    public V2RunState Run { get; set; } = new();
    public ScreenManager ScreenManager { get; set; } = null!;

    public int ScreenWidth => 1280;
    public int ScreenHeight => 720;
}
