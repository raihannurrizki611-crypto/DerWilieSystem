using System;
using System.Drawing;
using PlayerMovementTester;

public class Safe
{
    public string Name;
    public int X, Y;
    public int Width = 40, Height = 32;
    public int InteractionRadius = 60;

    public bool IsLocked = true;

    // Sengaja dipisah dari IsLocked: IsLocked mengatur bisa/tidaknya
    // di-interact, IsVisible mengatur digambar/tidaknya. Dipisah supaya
    // nanti kamu bisa bikin varian lain, misal koper kebuka tapi tetap
    // kelihatan (isinya kosong), tanpa perlu ubah struktur class ini.
    public bool IsVisible = true;

    public LockpickPuzzle Puzzle = new LockpickPuzzle();

    public Action<GameForm> OnUnlocked;
    public Action<GameForm> OnFailed;

    public Rectangle Bounds => new Rectangle(X, Y, Width, Height);

    public Safe(string name, int startX, int startY)
    {
        Name = name;
        X = startX;
        Y = startY;
    }

    public bool IsPlayerInRange(Rectangle playerBounds)
    {
        if (!IsLocked) return false;
        Rectangle zone = new Rectangle(
            X - InteractionRadius,
            Y - InteractionRadius,
            Width + InteractionRadius * 2,
            Height + InteractionRadius * 2
        );
        return zone.IntersectsWith(playerBounds);
    }

    public void Draw(Graphics g)
    {
        if (!IsVisible) return;
        g.FillRectangle(IsLocked ? Brushes.SaddleBrown : Brushes.Goldenrod, Bounds);
        g.DrawRectangle(Pens.Black, Bounds);
    }
}
