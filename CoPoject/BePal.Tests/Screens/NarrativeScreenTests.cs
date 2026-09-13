#nullable enable
using BePal.Gameplay;
using BePal.Screens;
using Xunit;

namespace BePal.Tests.Screens;

public class NarrativeScreenTests
{
    [Fact]
    public void PrologueNarrative_ContainsExpectedStoryBeats()
    {
        var lines = NarrativeScripts.GetPrologueLines();

        Assert.NotEmpty(lines);
        Assert.True(lines.Count >= 4);
        Assert.Contains(lines, l => l.Contains("Mossling", System.StringComparison.OrdinalIgnoreCase));
        Assert.Contains(lines, l => l.Contains("crate", System.StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void DoorstepNarrative_ForNibbleclaw_MentionsClawsAndHazard()
    {
        var lines = NarrativeScripts.GetDoorstepLines(2, PetKind.Attacker);

        Assert.NotEmpty(lines);
        Assert.Contains(lines, l => l.Contains("Nibbleclaw", System.StringComparison.OrdinalIgnoreCase));
        Assert.Contains(lines, l => l.Contains("claw", System.StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void DoorstepNarrative_ForBlinkbun_MentionsTeleportAndOzone()
    {
        var lines = NarrativeScripts.GetDoorstepLines(3, PetKind.Trickster);

        Assert.NotEmpty(lines);
        Assert.Contains(lines, l => l.Contains("Blinkbun", System.StringComparison.OrdinalIgnoreCase));
    }

    [Theory]
    [InlineData(1, PetKind.Baseline)]
    [InlineData(2, PetKind.Attacker)]
    [InlineData(3, PetKind.Trickster)]
    [InlineData(4, PetKind.Baseline)]
    [InlineData(5, PetKind.Attacker)]
    public void DoorstepNarrative_ReturnsNonEmptyLinesForAllDays(int day, PetKind pet)
    {
        var lines = NarrativeScripts.GetDoorstepLines(day, pet);
        Assert.NotEmpty(lines);
    }
}
