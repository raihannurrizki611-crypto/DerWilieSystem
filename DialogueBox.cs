using System.Drawing;
using System.Collections.Generic;

public class DialogueBox
{
    public bool IsVisible = false;
    public string CurrentText = "";
    public List<string> CurrentChoices = null;

    private Rectangle boxArea;

    public bool IsChoiceMode => CurrentChoices != null && CurrentChoices.Count > 0;

    public DialogueBox(int formWidth, int formHeight)
    {
        boxArea = new Rectangle(20, formHeight - 150, formWidth - 40, 130);
    }

    public void ShowText(string text)
    {
        CurrentText = text;
        CurrentChoices = null;
        IsVisible = true;
    }

    public void ShowChoices(string text, List<string> choiceTexts)
    {
        CurrentText = text;
        CurrentChoices = choiceTexts;
        IsVisible = true;
    }

    public void Hide()
    {
        IsVisible = false;
        CurrentText = "";
        CurrentChoices = null;
    }

    public void Draw(Graphics g)
    {
        if (!IsVisible) return;

        g.FillRectangle(new SolidBrush(Color.FromArgb(220, 30, 30, 30)), boxArea);
        g.DrawRectangle(Pens.White, boxArea);

        var textFont = new Font("Segoe UI", 12);
        g.DrawString(CurrentText, textFont, Brushes.White, boxArea.X + 15, boxArea.Y + 15);

        if (IsChoiceMode)
        {
            var choiceFont = new Font("Segoe UI", 10, FontStyle.Bold);
            int y = boxArea.Y + 50;
            for (int i = 0; i < CurrentChoices.Count; i++)
            {
                g.DrawString($"[{i + 1}] {CurrentChoices[i]}", choiceFont, Brushes.Yellow,
                    boxArea.X + 15, y);
                y += 22;
            }
        }
        else
        {
            g.DrawString("Tekan [E] untuk lanjut...", new Font("Segoe UI", 8, FontStyle.Italic),
                Brushes.LightGray, boxArea.X + 15, boxArea.Bottom - 20);
        }
    }
}