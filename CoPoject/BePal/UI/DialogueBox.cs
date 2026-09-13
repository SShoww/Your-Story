#nullable enable
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace BePal.UI;

/// <summary>
/// Reusable dialogue and inspection component supporting typewriter character reveal,
/// quick-skip, line advancing, and [YES]/[NO] choice prompts.
/// </summary>
public class DialogueBox
{
    private readonly Queue<string> _dialogueLines = new();
    private string _currentLine = string.Empty;
    private float _charTimer;
    private int _revealedChars;
    private Action? _onCompleted;
    private Action? _onYes;
    private Action? _onNo;

    public string? HeaderText { get; set; }
    public string CurrentLine => _currentLine;
    public string DisplayedText => _currentLine.Length > 0 ? _currentLine.Substring(0, Math.Min(_revealedChars, _currentLine.Length)) : string.Empty;
    public float TypewriterSpeed { get; set; } = 35f;

    public bool IsActive { get; private set; }
    public bool IsPromptActive { get; private set; }
    public string? PromptQuestion { get; private set; }

    public bool IsFullTextRevealed => _revealedChars >= _currentLine.Length;
    public bool IsTyping => IsActive && !IsPromptActive && !IsFullTextRevealed;
    public bool IsFinished => !IsActive;

    public Rectangle YesButtonRect { get; set; } = new(480, 600, 140, 48);
    public Rectangle NoButtonRect { get; set; } = new(660, 600, 140, 48);
    public Rectangle NextButtonRect { get; set; } = new(1080, 620, 120, 40);

    public void StartDialogue(string? header, IEnumerable<string> lines, Action? onCompleted = null)
    {
        _dialogueLines.Clear();
        foreach (string line in lines)
        {
            _dialogueLines.Enqueue(line);
        }

        HeaderText = header;
        _onCompleted = onCompleted;
        IsPromptActive = false;
        PromptQuestion = null;
        _onYes = null;
        _onNo = null;

        if (_dialogueLines.Count > 0)
        {
            IsActive = true;
            NextLine();
        }
        else
        {
            Close();
        }
    }

    public void ShowPrompt(string? header, string question, Action onYes, Action? onNo = null)
    {
        _dialogueLines.Clear();
        HeaderText = header;
        _currentLine = question;
        PromptQuestion = question;
        _revealedChars = question.Length;
        _onYes = onYes;
        _onNo = onNo;
        IsPromptActive = true;
        IsActive = true;
    }

    public void SkipTypewriter()
    {
        if (IsActive && !IsPromptActive)
        {
            _revealedChars = _currentLine.Length;
        }
    }

    public bool Advance()
    {
        if (!IsActive) return false;

        if (IsPromptActive) return true;

        if (!IsFullTextRevealed)
        {
            SkipTypewriter();
            return true;
        }

        if (_dialogueLines.Count > 0)
        {
            NextLine();
            return true;
        }

        Action? callback = _onCompleted;
        Close();
        callback?.Invoke();
        return false;
    }

    public void SelectYes()
    {
        if (!IsPromptActive) return;
        Action? yes = _onYes;
        Close();
        yes?.Invoke();
    }

    public void SelectNo()
    {
        if (!IsPromptActive) return;
        Action? no = _onNo;
        Close();
        no?.Invoke();
    }

    public void Close()
    {
        IsActive = false;
        IsPromptActive = false;
        _dialogueLines.Clear();
        _currentLine = string.Empty;
        _revealedChars = 0;
        PromptQuestion = null;
        _onCompleted = null;
        _onYes = null;
        _onNo = null;
    }

    private void NextLine()
    {
        _currentLine = _dialogueLines.Dequeue();
        _revealedChars = 0;
        _charTimer = 0f;
    }

    public void Update(float dt)
    {
        if (!IsActive || IsPromptActive) return;

        if (!IsFullTextRevealed)
        {
            _charTimer += dt * TypewriterSpeed;
            if (_charTimer >= 1f)
            {
                int charsToAdd = (int)_charTimer;
                _revealedChars = Math.Min(_currentLine.Length, _revealedChars + charsToAdd);
                _charTimer -= charsToAdd;
            }
        }
    }

    public void UpdateInput(float dt, KeyboardState keyboard, KeyboardState prevKeyboard, MouseState mouse, MouseState prevMouse)
    {
        if (!IsActive) return;

        Update(dt);

        bool spacePressed = keyboard.IsKeyDown(Keys.Space) && prevKeyboard.IsKeyUp(Keys.Space);
        bool leftClicked = mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released;
        Point mousePos = mouse.Position;

        if (IsPromptActive)
        {
            if (leftClicked)
            {
                if (YesButtonRect.Contains(mousePos))
                {
                    SelectYes();
                    return;
                }
                if (NoButtonRect.Contains(mousePos))
                {
                    SelectNo();
                    return;
                }
            }
            if (keyboard.IsKeyDown(Keys.Y) && prevKeyboard.IsKeyUp(Keys.Y))
            {
                SelectYes();
                return;
            }
            if (keyboard.IsKeyDown(Keys.N) && prevKeyboard.IsKeyUp(Keys.N))
            {
                SelectNo();
                return;
            }
        }
        else
        {
            if (spacePressed || (leftClicked && (NextButtonRect.Contains(mousePos) || mousePos.Y >= 500)))
            {
                Advance();
            }
        }
    }

    public static List<string> WrapText(string text, float maxLineWidth, Func<string, float> measureString)
    {
        List<string> lines = new();
        if (string.IsNullOrEmpty(text)) return lines;

        string[] words = text.Split(' ');
        string currentLine = "";

        foreach (string word in words)
        {
            string testLine = string.IsNullOrEmpty(currentLine) ? word : $"{currentLine} {word}";
            if (measureString(testLine) <= maxLineWidth)
            {
                currentLine = testLine;
            }
            else
            {
                if (!string.IsNullOrEmpty(currentLine))
                    lines.Add(currentLine);
                currentLine = word;
            }
        }

        if (!string.IsNullOrEmpty(currentLine))
            lines.Add(currentLine);

        return lines;
    }

    public static List<string> WrapText(SpriteFont font, string text, float maxLineWidth, float fontScale = 1.0f) =>
        WrapText(text, maxLineWidth, s => font.MeasureString(s).X * fontScale);

    public void Draw(SpriteBatch batch, SpriteFont font, Texture2D pixel, Rectangle bounds)
    {
        if (!IsActive) return;

        // Draw backdrop
        batch.FillRectangle(new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height), new Color(14, 18, 28, 240));
        batch.DrawRectangle(new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height), new Color(255, 203, 139), 2f);

        // Draw Header
        if (!string.IsNullOrEmpty(HeaderText))
        {
            Rectangle headerBounds = new(bounds.X + 24, bounds.Y - 16, Math.Min(320, bounds.Width - 48), 32);
            batch.FillRectangle(new RectangleF(headerBounds.X, headerBounds.Y, headerBounds.Width, headerBounds.Height), new Color(35, 42, 65));
            batch.DrawRectangle(new RectangleF(headerBounds.X, headerBounds.Y, headerBounds.Width, headerBounds.Height), new Color(255, 203, 139), 1.5f);
            batch.DrawString(font, HeaderText, new Vector2(headerBounds.X + 12, headerBounds.Y + 6), new Color(255, 225, 165), 0f, Vector2.Zero, 0.72f, SpriteEffects.None, 0f);
        }

        // Draw text with wrapping
        float fontScale = 0.82f;
        float lineHeight = font.LineSpacing * fontScale * 0.95f;
        List<string> wrappedLines = WrapText(font, DisplayedText, bounds.Width - 64, fontScale);
        for (int i = 0; i < wrappedLines.Count; i++)
        {
            Vector2 linePos = new(bounds.X + 32, bounds.Y + 28 + i * lineHeight);
            batch.DrawString(font, wrappedLines[i], linePos, Color.White, 0f, Vector2.Zero, fontScale, SpriteEffects.None, 0f);
        }

        // Prompt or Next button
        if (IsPromptActive)
        {
            DrawPromptButton(batch, font, pixel, YesButtonRect, "[ YES ]", new Color(48, 120, 72));
            DrawPromptButton(batch, font, pixel, NoButtonRect, "[ NO ]", new Color(130, 45, 45));
        }
        else
        {
            string nextLabel = IsFullTextRevealed ? "NEXT =>" : "SKIP >>";
            Color btnColor = IsFullTextRevealed ? new Color(70, 95, 145) : new Color(50, 55, 70);
            batch.FillRectangle(new RectangleF(NextButtonRect.X, NextButtonRect.Y, NextButtonRect.Width, NextButtonRect.Height), btnColor);
            batch.DrawRectangle(new RectangleF(NextButtonRect.X, NextButtonRect.Y, NextButtonRect.Width, NextButtonRect.Height), new Color(255, 203, 139), 1f);
            Vector2 sz = font.MeasureString(nextLabel) * 0.65f;
            Vector2 labelPos = new(NextButtonRect.X + (NextButtonRect.Width - sz.X) / 2f, NextButtonRect.Y + (NextButtonRect.Height - sz.Y) / 2f);
            batch.DrawString(font, nextLabel, labelPos, Color.White, 0f, Vector2.Zero, 0.65f, SpriteEffects.None, 0f);
        }
    }

    private static void DrawPromptButton(SpriteBatch batch, SpriteFont font, Texture2D pixel, Rectangle bounds, string label, Color bgColor)
    {
        batch.FillRectangle(new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height), bgColor);
        batch.DrawRectangle(new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height), new Color(255, 203, 139), 2f);
        Vector2 sz = font.MeasureString(label) * 0.75f;
        Vector2 labelPos = new(bounds.X + (bounds.Width - sz.X) / 2f, bounds.Y + (bounds.Height - sz.Y) / 2f);
        batch.DrawString(font, label, labelPos, Color.White, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0f);
    }
}
