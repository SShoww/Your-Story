#nullable enable
using System;
using Microsoft.Xna.Framework;

namespace BePal.Screens;

/// <summary>
/// Handles aspect ratio preservation, letterboxing/pillarboxing,
/// and virtual-to-screen coordinate mapping.
/// </summary>
public static class ViewportScaler
{
    public const int VirtualWidth = 1280;
    public const int VirtualHeight = 720;

    public static Rectangle CalculateDestinationRectangle(int screenW, int screenH, int virtualW = VirtualWidth, int virtualH = VirtualHeight)
    {
        if (screenW <= 0 || screenH <= 0 || virtualW <= 0 || virtualH <= 0)
            return Rectangle.Empty;

        float scale = Math.Min((float)screenW / virtualW, (float)screenH / virtualH);
        int w = (int)Math.Round(virtualW * scale);
        int h = (int)Math.Round(virtualH * scale);
        int x = (screenW - w) / 2;
        int y = (screenH - h) / 2;
        return new Rectangle(x, y, w, h);
    }

    public static Point ScreenToVirtual(Point screenPos, Rectangle destRect, int virtualW = VirtualWidth, int virtualH = VirtualHeight)
    {
        if (destRect.Width <= 0 || destRect.Height <= 0)
            return screenPos;

        int vx = (int)Math.Round((screenPos.X - destRect.X) * (float)virtualW / destRect.Width);
        int vy = (int)Math.Round((screenPos.Y - destRect.Y) * (float)virtualH / destRect.Height);
        return new Point(vx, vy);
    }
}
