using System;
using System.Drawing;

// Minigame "lockpick" generik: bar dengan indikator yang bergerak bolak-balik,
// player harus menekan tombol saat indikator ada di zona hijau (sweet spot).
// Tidak terikat ke satu brankas tertentu -> bisa dipakai ulang lewat class Safe.
public class LockpickPuzzle
{
    public int TotalPins = 3;
    public int CurrentPin = 0;

    public int MaxAttempts = 3;
    public int AttemptsLeft;

    // Posisi indikator dalam skala 0 (kiri) - 1 (kanan)
    public float Position = 0f;
    private float direction = 1f;
    public float Speed = 0.02f; // per tick, makin besar makin cepat/susah

    // Zona target (sweet spot), diacak tiap pin
    public float TargetStart;
    public float TargetWidth = 0.12f;

    public bool IsRunning = false;
    public bool IsSolved = false;
    public bool IsFailed = false;

    private readonly Random rng = new Random();

    public void Start(int totalPins = 3, int maxAttempts = 3)
    {
        TotalPins = totalPins;
        MaxAttempts = maxAttempts;
        AttemptsLeft = maxAttempts;
        CurrentPin = 0;
        Speed = 0.02f;
        TargetWidth = 0.12f;
        IsRunning = true;
        IsSolved = false;
        IsFailed = false;
        RollNewTarget();
    }

    private void RollNewTarget()
    {
        TargetStart = (float)(rng.NextDouble() * (1 - TargetWidth));
        Position = 0f;
        direction = 1f;
    }

    // Dipanggil tiap frame selagi puzzle berjalan
    public void Tick()
    {
        if (!IsRunning) return;

        Position += Speed * direction;
        if (Position >= 1f) { Position = 1f; direction = -1f; }
        if (Position <= 0f) { Position = 0f; direction = 1f; }
    }

    // Dipanggil saat player menekan tombol "coba kunci"
    public void TryLock()
    {
        if (!IsRunning) return;

        bool hit = Position >= TargetStart && Position <= TargetStart + TargetWidth;
        if (hit)
        {
            CurrentPin++;
            if (CurrentPin >= TotalPins)
            {
                IsRunning = false;
                IsSolved = true;
            }
            else
            {
                // Makin ke pin berikutnya, makin susah
                Speed += 0.005f;
                TargetWidth = Math.Max(0.06f, TargetWidth - 0.015f);
                RollNewTarget();
            }
        }
        else
        {
            AttemptsLeft--;
            if (AttemptsLeft <= 0)
            {
                IsRunning = false;
                IsFailed = true;
            }
            else
            {
                RollNewTarget();
            }
        }
    }

    public void Draw(Graphics g, Rectangle area)
    {
        g.FillRectangle(Brushes.DimGray, area);
        g.DrawRectangle(Pens.White, area);

        // Zona target (hijau)
        int zoneX = area.X + (int)(TargetStart * area.Width);
        int zoneW = (int)(TargetWidth * area.Width);
        g.FillRectangle(Brushes.LimeGreen, zoneX, area.Y, zoneW, area.Height);

        // Indikator (garis merah)
        int indicatorX = area.X + (int)(Position * area.Width);
        g.FillRectangle(Brushes.Red, indicatorX - 2, area.Y - 6, 4, area.Height + 12);

        string info = $"Pin {CurrentPin + 1}/{TotalPins}   Sisa percobaan: {AttemptsLeft}";
        g.DrawString(info, new Font("Segoe UI", 9, FontStyle.Bold), Brushes.White, area.X, area.Y - 24);

        string hint = "Tekan [Space] saat indikator di zona hijau, [Esc] batal";
        g.DrawString(hint, new Font("Segoe UI", 8, FontStyle.Italic), Brushes.LightGray, area.X, area.Bottom + 6);
    }
}