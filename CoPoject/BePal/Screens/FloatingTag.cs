#nullable enable
using Microsoft.Xna.Framework;

namespace BePal.Screens;

/// <summary>
/// Transient floating tag effect for QTE results and notifications.
/// </summary>
public sealed class FloatingTag
{
    public string Text { get; set; } = string.Empty;
    public Vector2 Position;
    public Color BgColor;
    public Color TextColor;
    public float Lifetime = 0.85f;
    public float MaxLifetime = 0.85f;
    public float Rotation;
}
