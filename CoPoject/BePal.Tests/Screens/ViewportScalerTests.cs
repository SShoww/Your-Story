#nullable enable
using BePal.Screens;
using Microsoft.Xna.Framework;
using Xunit;

namespace BePal.Tests.Screens;

public class ViewportScalerTests
{
    [Fact]
    public void CalculateDestinationRectangle_ExactResolution_ReturnsFullRectangle()
    {
        Rectangle rect = ViewportScaler.CalculateDestinationRectangle(1280, 720);
        Assert.Equal(new Rectangle(0, 0, 1280, 720), rect);
    }

    [Fact]
    public void CalculateDestinationRectangle_1080p_ScalesProportionally()
    {
        Rectangle rect = ViewportScaler.CalculateDestinationRectangle(1920, 1080);
        Assert.Equal(new Rectangle(0, 0, 1920, 1080), rect);
    }

    [Fact]
    public void CalculateDestinationRectangle_1440p_ScalesProportionally()
    {
        Rectangle rect = ViewportScaler.CalculateDestinationRectangle(2560, 1440);
        Assert.Equal(new Rectangle(0, 0, 2560, 1440), rect);
    }

    [Fact]
    public void CalculateDestinationRectangle_16by10_LetterboxesTopAndBottom()
    {
        // 1920x1200: max 16:9 fit is 1920x1080, centered vertically with 60px padding
        Rectangle rect = ViewportScaler.CalculateDestinationRectangle(1920, 1200);
        Assert.Equal(new Rectangle(0, 60, 1920, 1080), rect);
    }

    [Fact]
    public void CalculateDestinationRectangle_Ultrawide_PillarboxesLeftAndRight()
    {
        // 2560x1080 (21:9): max height is 1080, width is 1920, centered horizontally with 320px padding
        Rectangle rect = ViewportScaler.CalculateDestinationRectangle(2560, 1080);
        Assert.Equal(new Rectangle(320, 0, 1920, 1080), rect);
    }

    [Theory]
    [InlineData(0, 1080)]
    [InlineData(1920, 0)]
    [InlineData(-100, 720)]
    public void CalculateDestinationRectangle_InvalidDimensions_ReturnsEmpty(int w, int h)
    {
        Rectangle rect = ViewportScaler.CalculateDestinationRectangle(w, h);
        Assert.Equal(Rectangle.Empty, rect);
    }

    [Fact]
    public void ScreenToVirtual_1080p_MapsCenterCorrectly()
    {
        Rectangle dest = new(0, 0, 1920, 1080);
        Point virtualPos = ViewportScaler.ScreenToVirtual(new Point(960, 540), dest);
        Assert.Equal(new Point(640, 360), virtualPos);
    }

    [Fact]
    public void ScreenToVirtual_Letterboxed_MapsPaddedCoordinatesOutsideCanvas()
    {
        // 1920x1200 with 60px top bar
        Rectangle dest = new(0, 60, 1920, 1080);

        // Clicking in top letterbox bar (y=30)
        Point virtualPos = ViewportScaler.ScreenToVirtual(new Point(960, 30), dest);
        Assert.True(virtualPos.Y < 0);

        // Clicking center of active area (y=600)
        Point centerPos = ViewportScaler.ScreenToVirtual(new Point(960, 600), dest);
        Assert.Equal(new Point(640, 360), centerPos);
    }

    [Fact]
    public void ScreenToVirtual_EmptyDestination_ReturnsOriginalPoint()
    {
        Point original = new(100, 200);
        Point result = ViewportScaler.ScreenToVirtual(original, Rectangle.Empty);
        Assert.Equal(original, result);
    }
}
