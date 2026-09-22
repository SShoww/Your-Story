using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace BePalV2.UI;

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
    public float TypewriterSpeed { get; set; } = 40f;

    public bool IsActive { get; private set; }
    public bool IsPromptActive { get; private set; }
    public string? PromptQuestion { get; private set; }

    public bool IsFullTextRevealed => _revealedChars >= _currentLine.Length;
    public bool IsTyping => IsActive && !IsPromptActive && !IsFullTextRevealed;
    public bool IsFinished => !IsActive;

    public Rectangle YesButtonRect { get; set; } = new(480, 580, 150, 48);
    public Rectangle NoButtonRect { get; set; } = new(650, 580, 150, 48);
    public Rectangle NextButtonRect { get; set; } = new(1060, 600, 140, 40);

    public void StartDialogue(string? header, IEnumerable<string> lines, Action? onCompleted = null)
    {
        HeaderText = header;
        _dialogueLines.Clear();
        foreach (var line in lines)
        {
            _dialogueLines.Enqueue(line);
        }

        _onCompleted = onCompleted;
        _onYes = null;
        _onNo = null;
        IsPromptActive = false;
        PromptQuestion = null;
        IsActive = true;

        NextLine();
    }

    public void ShowPrompt(string? header, string question, Action onYes, Action? onNo = null)
    {
        HeaderText = header;
        _dialogueLines.Clear();
        _currentLine = question;
        _revealedChars = question.Length;
        PromptQuestion = question;
        _onYes = onYes;
        _onNo = onNo;
        IsPromptActive = true;
        IsActive = true;
    }

    public void SkipTypewriter()
    {
        if (IsTyping)
        {
            _revealedChars = _currentLine.Length;
        }
    }

    public bool Advance()
    {
        if (!IsActive || IsPromptActive) return false;

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

        Close();
        return false;
    }

    public void SelectYes()
    {
        if (!IsPromptActive) return;
        var action = _onYes;
        Close();
        action?.Invoke();
    }

    public void SelectNo()
    {
        if (!IsPromptActive) return;
        var action = _onNo;
        Close();
        action?.Invoke();
    }

    public void Close()
    {
        IsActive = false;
        IsPromptActive = false;
        _currentLine = string.Empty;
        _dialogueLines.Clear();
        var callback = _onCompleted;
        _onCompleted = null;
        _onYes = null;
        _onNo = null;
        callback?.Invoke();
    }

    private void NextLine()
    {
        _currentLine = _dialogueLines.Count > 0 ? _dialogueLines.Dequeue() : string.Empty;
        _revealedChars = 0;
        _charTimer = 0f;
    }

    public void Update(float dt)
    {
        if (!IsActive || IsPromptActive || IsFullTextRevealed) return;

        _charTimer += dt;
        float interval = 1f / Math.Max(1f, TypewriterSpeed);
        while (_charTimer >= interval && _revealedChars < _currentLine.Length)
        {
            _charTimer -= interval;
            _revealedChars++;
        }
    }

    public void UpdateInput(float dt, KeyboardState keyboard, KeyboardState prevKeyboard, MouseState mouse, MouseState prevMouse)
    {
        if (!IsActive) return;

        Update(dt);

        bool spacePressed = keyboard.IsKeyDown(Keys.Space) && !prevKeyboard.IsKeyDown(Keys.Space);
        bool enterPressed = keyboard.IsKeyDown(Keys.Enter) && !prevKeyboard.IsKeyDown(Keys.Enter);
        bool clickPressed = mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released;
        Point mousePos = mouse.Position;

        if (IsPromptActive)
        {
            if (keyboard.IsKeyDown(Keys.Y) && !prevKeyboard.IsKeyDown(Keys.Y))
            {
                SelectYes();
                return;
            }
            if (keyboard.IsKeyDown(Keys.N) && !prevKeyboard.IsKeyDown(Keys.N))
            {
                SelectNo();
                return;
            }

            if (clickPressed)
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
            return;
        }

        if (spacePressed || enterPressed || (clickPressed && NextButtonRect.Contains(mousePos)))
        {
            Advance();
        }
    }

    public void Draw(SpriteBatch batch, SpriteFont font, Texture2D pixel, Rectangle bounds)
    {
        if (!IsActive) return;

        // Dynamically anchor buttons inside provided bounds
        NextButtonRect = new(bounds.Right - 160, bounds.Bottom - 52, 130, 38);
        YesButtonRect = new(bounds.Center.X - 160, bounds.Bottom - 52, 140, 38);
        NoButtonRect = new(bounds.Center.X + 20, bounds.Bottom - 52, 140, 38);

        // Background box
        batch.FillRectangle(bounds, new Color(14, 16, 24, 235));
        batch.DrawRectangle(bounds, new Color(200, 200, 220), 2);

        // Header
        if (!string.IsNullOrEmpty(HeaderText))
        {
            Rectangle headerBounds = new(bounds.X + 24, bounds.Y - 14, (int)font.MeasureString(HeaderText).X + 20, 26);
            batch.FillRectangle(headerBounds, new Color(30, 34, 48));
            batch.DrawRectangle(headerBounds, new Color(120, 140, 180), 1);
            batch.DrawString(font, HeaderText, new Vector2(bounds.X + 34, bounds.Y - 10), Color.Gold);
        }

        // Text with word wrapping
        int textX = bounds.X + 30;
        int textY = bounds.Y + 28;
        float maxTextWidth = bounds.Width - 60;
        string displayed = DisplayedText;

        string[] words = displayed.Split(' ');
        string currentLine = "";
        float curY = textY;
        foreach (string word in words)
        {
            string testLine = string.IsNullOrEmpty(currentLine) ? word : $"{currentLine} {word}";
            if (font.MeasureString(testLine).X > maxTextWidth && !string.IsNullOrEmpty(currentLine))
            {
                batch.DrawString(font, currentLine, new Vector2(textX, curY), Color.White);
                curY += font.LineSpacing * 0.9f;
                currentLine = word;
            }
            else
            {
                currentLine = testLine;
            }
        }
        if (!string.IsNullOrEmpty(currentLine))
        {
            batch.DrawString(font, currentLine, new Vector2(textX, curY), Color.White);
        }

        // Prompt Buttons or Next button
        if (IsPromptActive)
        {
            DrawPromptButton(batch, font, pixel, YesButtonRect, "[ Y ] YES", new Color(40, 120, 60));
            DrawPromptButton(batch, font, pixel, NoButtonRect, "[ N ] NO", new Color(140, 40, 40));
        }
        else
        {
            string nextLabel = IsFullTextRevealed ? "[ NEXT > ]" : "[ SKIP ]";
            Color nextBg = IsFullTextRevealed ? new Color(40, 80, 140) : new Color(60, 60, 70);
            DrawPromptButton(batch, font, pixel, NextButtonRect, nextLabel, nextBg);
        }
    }

    private static void DrawPromptButton(SpriteBatch batch, SpriteFont font, Texture2D pixel, Rectangle bounds, string label, Color bgColor)
    {
        batch.FillRectangle(bounds, bgColor);
        batch.DrawRectangle(bounds, Color.White, 1);
        Vector2 size = font.MeasureString(label);
        float maxW = Math.Max(1f, bounds.Width - 12);
        float maxH = Math.Max(1f, bounds.Height - 6);
        float scale = Math.Min(1f, Math.Min(maxW / size.X, maxH / size.Y));
        Vector2 pos = new(bounds.Center.X - (size.X * scale) / 2f, bounds.Center.Y - (size.Y * scale) / 2f);
        batch.DrawString(font, label, pos, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
    }
}
