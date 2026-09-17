using System.Drawing;
using System.Windows.Forms;

public class Player
{
    public int X, Y;
    public int Width = 32, Height = 32;
    public int Speed = 5;

    public Rectangle Bounds => new Rectangle(X, Y, Width, Height);

    public Player(int startX, int startY)
    {
        X = startX;
        Y = startY;
    }

    // Dipanggil tiap frame di game loop, berdasarkan tombol yang lagi ditekan
    public void Move(bool up, bool down, bool left, bool right)
    {
        if (up) Y -= Speed;
        if (down) Y += Speed;
        if (left) X -= Speed;
        if (right) X += Speed;
    }

    public void Draw(Graphics g)
    {
        // Placeholder: nanti ganti g.DrawImage(sprite, Bounds)
        g.FillRectangle(Brushes.CornflowerBlue, Bounds);
    }
}