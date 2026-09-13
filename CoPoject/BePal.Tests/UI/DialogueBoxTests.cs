#nullable enable
using System.Collections.Generic;
using BePal.UI;
using Xunit;

namespace BePal.Tests.UI;

public class DialogueBoxTests
{
    [Fact]
    public void StartDialogue_WhenInvoked_EnqueuesLinesAndInitializesFirstLine()
    {
        // Arrange
        var dialogue = new DialogueBox();
        var lines = new[] { "Hello world.", "Second line." };

        // Act
        dialogue.StartDialogue("NARRATOR", lines);

        // Assert
        Assert.Equal("NARRATOR", dialogue.HeaderText);
        Assert.Equal("Hello world.", dialogue.CurrentLine);
        Assert.False(dialogue.IsFinished);
        Assert.False(dialogue.IsFullTextRevealed);
        Assert.True(dialogue.IsActive);
    }

    [Fact]
    public void Update_WhenTyping_AdvancesRevealedCharsBasedOnSpeed()
    {
        // Arrange
        var dialogue = new DialogueBox { TypewriterSpeed = 20f };
        dialogue.StartDialogue("TEST", new[] { "ABCD" });

        // Act - 0.1s at 20 chars/s = 2 chars
        dialogue.Update(0.1f);

        // Assert
        Assert.Equal(2, dialogue.DisplayedText.Length);
        Assert.Equal("AB", dialogue.DisplayedText);
        Assert.False(dialogue.IsFullTextRevealed);

        // Act - another 0.1s
        dialogue.Update(0.1f);
        Assert.Equal("ABCD", dialogue.DisplayedText);
        Assert.True(dialogue.IsFullTextRevealed);
    }

    [Fact]
    public void SkipTypewriter_WhenCalled_ImmediatelyRevealsFullLine()
    {
        // Arrange
        var dialogue = new DialogueBox();
        dialogue.StartDialogue("TEST", new[] { "This is a long sentence." });
        Assert.False(dialogue.IsFullTextRevealed);

        // Act
        dialogue.SkipTypewriter();

        // Assert
        Assert.True(dialogue.IsFullTextRevealed);
        Assert.Equal("This is a long sentence.", dialogue.DisplayedText);
    }

    [Fact]
    public void Advance_WhenTyping_SkipsToFullLineFirst()
    {
        // Arrange
        var dialogue = new DialogueBox();
        dialogue.StartDialogue("TEST", new[] { "Line 1", "Line 2" });
        Assert.False(dialogue.IsFullTextRevealed);

        // Act
        bool stillActive = dialogue.Advance();

        // Assert - still on Line 1, but text fully revealed
        Assert.True(stillActive);
        Assert.Equal("Line 1", dialogue.CurrentLine);
        Assert.True(dialogue.IsFullTextRevealed);
    }

    [Fact]
    public void Advance_WhenFullLine_AdvancesToNextLineOrCompletes()
    {
        // Arrange
        var dialogue = new DialogueBox();
        bool completedInvoked = false;
        dialogue.StartDialogue("TEST", new[] { "Line 1", "Line 2" }, onCompleted: () => completedInvoked = true);

        dialogue.SkipTypewriter();
        Assert.Equal("Line 1", dialogue.CurrentLine);

        // Act 1: Advance from Line 1 to Line 2
        bool hasNext = dialogue.Advance();
        Assert.True(hasNext);
        Assert.Equal("Line 2", dialogue.CurrentLine);
        Assert.False(completedInvoked);

        // Act 2: Advance while Line 2 is typing -> reveals Line 2
        dialogue.Advance();
        Assert.True(dialogue.IsFullTextRevealed);

        // Act 3: Advance past Line 2 -> finishes dialogue
        bool stillHasMore = dialogue.Advance();
        Assert.False(stillHasMore);
        Assert.True(dialogue.IsFinished);
        Assert.False(dialogue.IsActive);
        Assert.True(completedInvoked);
    }

    [Fact]
    public void ShowPrompt_WhenCalled_SetsPromptActiveAndDispatchesChoice()
    {
        // Arrange
        var dialogue = new DialogueBox();
        bool yesChosen = false;
        bool noChosen = false;

        dialogue.ShowPrompt("CARE PROMPT", "Care for Mossling?",
            onYes: () => yesChosen = true,
            onNo: () => noChosen = true);

        // Assert setup
        Assert.True(dialogue.IsPromptActive);
        Assert.True(dialogue.IsActive);
        Assert.Equal("Care for Mossling?", dialogue.PromptQuestion);

        // Act 1: Select YES
        dialogue.SelectYes();
        Assert.True(yesChosen);
        Assert.False(noChosen);
        Assert.False(dialogue.IsActive);

        // Re-show and select NO
        yesChosen = false;
        dialogue.ShowPrompt("CARE PROMPT", "Care for Mossling?",
            onYes: () => yesChosen = true,
            onNo: () => noChosen = true);

        dialogue.SelectNo();
        Assert.False(yesChosen);
        Assert.True(noChosen);
        Assert.False(dialogue.IsActive);
    }

    [Fact]
    public void Close_WhenCalled_ResetsActiveState()
    {
        // Arrange
        var dialogue = new DialogueBox();
        dialogue.StartDialogue("TEST", new[] { "Some line" });
        Assert.True(dialogue.IsActive);

        // Act
        dialogue.Close();

        // Assert
        Assert.False(dialogue.IsActive);
        Assert.True(dialogue.IsFinished);
        Assert.Empty(dialogue.CurrentLine);
    }

    [Fact]
    public void WrapText_WhenTextExceedsMaxWidth_SplitsIntoMultipleLines()
    {
        // Arrange: 1 char = 10px width
        float Measure(string s) => s.Length * 10f;
        string input = "One two three four five six";

        // Act: max width 120px (allows up to 12 chars per line)
        List<string> wrapped = DialogueBox.WrapText(input, 120f, Measure);

        // Assert
        Assert.True(wrapped.Count > 1);
        foreach (string line in wrapped)
        {
            Assert.True(Measure(line) <= 120f);
        }
        Assert.Equal("One two", wrapped[0]);
        Assert.Equal("three four", wrapped[1]);
        Assert.Equal("five six", wrapped[2]);
    }
}
