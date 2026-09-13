#nullable enable
using System;
using BePal.Gameplay;
using BePal.Screens;
using Xunit;

namespace BePal.Tests.Screens;

public class PanoramicRoomTests
{
    [Fact]
    public void WallRotation_RotateRight_WrapsAroundFourWalls()
    {
        // Arrange
        var model = new PanoramicRoomModel();
        Assert.Equal(0, model.CurrentWallIndex);

        // Act & Assert
        model.RotateRight();
        Assert.Equal(1, model.CurrentWallIndex);

        model.RotateRight();
        Assert.Equal(2, model.CurrentWallIndex);

        model.RotateRight();
        Assert.Equal(3, model.CurrentWallIndex);

        model.RotateRight();
        Assert.Equal(0, model.CurrentWallIndex);
    }

    [Fact]
    public void WallRotation_RotateLeft_WrapsAroundBackwards()
    {
        // Arrange
        var model = new PanoramicRoomModel();
        Assert.Equal(0, model.CurrentWallIndex);

        // Act & Assert
        model.RotateLeft();
        Assert.Equal(3, model.CurrentWallIndex);

        model.RotateLeft();
        Assert.Equal(2, model.CurrentWallIndex);

        model.RotateLeft();
        Assert.Equal(1, model.CurrentWallIndex);

        model.RotateLeft();
        Assert.Equal(0, model.CurrentWallIndex);
    }

    [Fact]
    public void GetWallName_ReturnsExpectedZoneTitles()
    {
        var model = new PanoramicRoomModel();

        Assert.Contains("Pet Zone", model.GetWallTitle(0), StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Prep & Pantry", model.GetWallTitle(1), StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Study Desk", model.GetWallTitle(2), StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Front Door", model.GetWallTitle(3), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void GetInspectablesForWall_ReturnsItemsSpecificToCurrentWall()
    {
        var model = new PanoramicRoomModel();

        var wall0Items = model.GetItemsForWall(0);
        var wall1Items = model.GetItemsForWall(1);
        var wall2Items = model.GetItemsForWall(2);
        var wall3Items = model.GetItemsForWall(3);

        Assert.Contains(wall0Items, item => item.Id == "pet");
        Assert.Contains(wall1Items, item => item.Id == "pantry");
        Assert.Contains(wall1Items, item => item.Id == "water");
        Assert.Contains(wall1Items, item => item.Id == "disposal");
        Assert.Contains(wall2Items, item => item.Id == "log");
        Assert.Contains(wall2Items, item => item.Id == "board");
        Assert.Contains(wall3Items, item => item.Id == "door");
        Assert.Contains(wall3Items, item => item.Id == "window");
    }

    [Fact]
    public void GetActivePetCue_ReturnsDescriptiveClueForActivePet()
    {
        var run = new PrototypeRun();
        var cue = PanoramicRoomModel.GetPetBehaviorCue(run.ActivePet);

        Assert.False(string.IsNullOrEmpty(cue));
        Assert.Contains("Mossling", cue);
    }
}
