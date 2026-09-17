using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PlayerMovementTester
{
    public partial class GameForm : Form
    {
        private System.Windows.Forms.Timer gameTimer;
        private Player player;
        private DialogueBox dialogueBox;

        private List<NPC> npcs = new List<NPC>();
        private NPC activeNPC;

        private Safe safe;

        public QuestManager Quests = new QuestManager();

        private bool up, down, left, right;
        private bool eKeyLocked = false;

        public bool HasBefriendedRehan = false;

        public GameForm()
        {
            InitializeComponent();

            this.Text = "NPC Interaction Demo";
            this.ClientSize = new Size(800, 500);
            this.DoubleBuffered = true;

            player = new Player(100, 100);
            dialogueBox = new DialogueBox(this.ClientSize.Width, this.ClientSize.Height);

            SetupNpcRehan();
            SetupNpcSarah();
            SetupSafe();
            SetupQuests();

            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 16; // ~60 FPS
            gameTimer.Tick += GameLoop;
            gameTimer.Start();

            this.KeyDown += OnKeyDown;
            this.KeyUp += OnKeyUp;
            this.Paint += OnPaint;
        }

        // ---------- NPC A: Rehan ----------
        private void SetupNpcRehan()
        {
            var rehan = new NPC("Rehan", 400, 250) { Color = Color.IndianRed };

            rehan.AddNode(new DialogueNode("start", "Halo! Aku NPC pertama di game ini.", nextNodeId: "n2"));
            rehan.AddNode(new DialogueNode("n2", "Perkenalkan, namaku Rehan.", nextNodeId: "n3"));

            rehan.AddNode(new DialogueNode("n3", "Mau jadi teman aku?", choices: new List<DialogueChoice>
            {
                new DialogueChoice(
                    "Iya, aku mau berteman!",
                    nextNodeId: "yes",
                    onSelect: form =>
                    {
                        form.HasBefriendedRehan = true;
                        form.SpawnNpc("Sarah");
                        form.Quests.StartQuest("quest_rehan");
                    }),

                new DialogueChoice(
                    "Nanti dulu deh.",
                    nextNodeId: "no"),

                new DialogueChoice(
                    "Emang di sini ada orang lain?",
                    nextNodeId: "n3",
                    onSelect: form =>
                    {
                        MessageBox.Show(form, "Kamu belum tahu, tapi sebenarnya ada.", "???");
                    })
            }));

            rehan.AddNode(new DialogueNode("yes", "Asik! Sini, kukenalkan temanku.", nextNodeId: null));
            rehan.AddNode(new DialogueNode("no", "Oke, gapapa. Kapan-kapan ya.", nextNodeId: null));

            npcs.Add(rehan);
        }

        // ---------- NPC B: Sarah ----------
        private void SetupNpcSarah()
        {
            var sarah = new NPC("Sarah", 550, 250) { Color = Color.MediumPurple, IsActive = false };

            sarah.AddNode(new DialogueNode("start", "Halo, aku Sarah, temannya Rehan.", nextNodeId: "n2"));
            sarah.AddNode(new DialogueNode("n2", "Salam kenal ya!", nextNodeId: null));

            npcs.Add(sarah);
        }

        public void SpawnNpc(string npcName)
        {
            var target = npcs.Find(n => n.Name == npcName);
            if (target != null) target.IsActive = true;
        }

        // ---------- Brankas: minigame lockpicking ----------
        private void SetupSafe()
        {
            safe = new Safe("Koper Misterius", 150, 350);

            safe.OnUnlocked = form =>
            {
                MessageBox.Show(form, "Klik! Koper berhasil dibuka. Ada sesuatu di dalamnya.", "Berhasil");
                form.Quests.ProgressObjective(form, "quest_koper", "buka_koper");

                // ---- Koper menghilang setelah terbobol ----
                safe.IsVisible = false;
            };

            safe.OnFailed = form =>
            {
                MessageBox.Show(form, "Percobaan habis, koper masih terkunci.", "Gagal");
            };
        }

        // ---------- Quest ----------
        private void SetupQuests()
        {
            var questRehan = new Quest("quest_rehan", "Teman Baru",
                "Berteman dengan Rehan dan cari tahu siapa lagi yang ada di sekitar sini.");
            questRehan.Objectives.Add(new QuestObjective("bertemu_sarah", "Ajak bicara Sarah"));
            questRehan.OnComplete = form =>
            {
                MessageBox.Show(form, "Quest selesai: Teman Baru!", "Quest Selesai");
            };
            Quests.RegisterQuest(questRehan);

            var questKoper = new Quest("quest_koper", "Koper Misterius",
                "Bobol koper misterius yang ditemukan di dekat sini.");
            questKoper.Objectives.Add(new QuestObjective("buka_koper", "Buka koper"));
            questKoper.OnComplete = form =>
            {
                MessageBox.Show(form, "Quest selesai: Koper Misterius!", "Quest Selesai");
            };
            Quests.RegisterQuest(questKoper);

            Quests.StartQuest("quest_koper");
        }

        private void GameLoop(object sender, EventArgs e)
        {
            bool isBusy = dialogueBox.IsVisible || safe.Puzzle.IsRunning;
            if (!isBusy)
            {
                player.Move(up, down, left, right);
            }

            // GameLoop sekarang cuma menggerakkan indikator saja.
            // Deteksi berhasil/gagal dipindah ke OnKeyDown (lihat penjelasan bug-nya).
            if (safe.Puzzle.IsRunning)
            {
                safe.Puzzle.Tick();
            }

            this.Invalidate();
        }

        private NPC GetNearbyNpc()
        {
            foreach (var npc in npcs)
            {
                if (npc.IsPlayerInRange(player.Bounds)) return npc;
            }
            return null;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W || e.KeyCode == Keys.Up) up = true;
            if (e.KeyCode == Keys.S || e.KeyCode == Keys.Down) down = true;
            if (e.KeyCode == Keys.A || e.KeyCode == Keys.Left) left = true;
            if (e.KeyCode == Keys.D || e.KeyCode == Keys.Right) right = true;

            if (e.KeyCode == Keys.E && !eKeyLocked)
            {
                eKeyLocked = true;
                HandleInteraction();
            }

            if (dialogueBox.IsVisible && dialogueBox.IsChoiceMode)
            {
                int number = GetNumberKeyPressed(e.KeyCode);
                if (number >= 1) HandleChoiceSelected(number - 1);
            }

            if (safe.Puzzle.IsRunning)
            {
                if (e.KeyCode == Keys.Space)
                {
                    safe.Puzzle.TryLock();

                    // ---- Fix: cek hasilnya di sini juga, jangan tunggu GameLoop ----
                    // TryLock() bisa langsung mengubah IsRunning jadi false pada
                    // panggilan ini, jadi GameLoop di frame berikutnya sudah
                    // terlambat untuk mendeteksinya.
                    if (safe.Puzzle.IsSolved)
                    {
                        safe.IsLocked = false;
                        safe.OnUnlocked?.Invoke(this);
                    }
                    else if (safe.Puzzle.IsFailed)
                    {
                        safe.OnFailed?.Invoke(this);
                    }
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    safe.Puzzle.IsRunning = false;
                }
            }
        }

        private int GetNumberKeyPressed(Keys key)
        {
            if (key >= Keys.D1 && key <= Keys.D9) return (int)key - (int)Keys.D0;
            if (key >= Keys.NumPad1 && key <= Keys.NumPad9) return (int)key - (int)Keys.NumPad0;
            return -1;
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W || e.KeyCode == Keys.Up) up = false;
            if (e.KeyCode == Keys.S || e.KeyCode == Keys.Down) down = false;
            if (e.KeyCode == Keys.A || e.KeyCode == Keys.Left) left = false;
            if (e.KeyCode == Keys.D || e.KeyCode == Keys.Right) right = false;
            if (e.KeyCode == Keys.E) eKeyLocked = false;
        }

        private void HandleInteraction()
        {
            if (dialogueBox.IsVisible)
            {
                var current = activeNPC?.GetCurrentNode();
                if (current != null && current.HasChoices) return;

                var next = activeNPC?.Advance();
                if (next != null)
                {
                    ShowCurrentNode();
                }
                else
                {
                    dialogueBox.Hide();
                    activeNPC = null;
                }
                return;
            }

            if (safe.Puzzle.IsRunning) return;

            var targetNpc = GetNearbyNpc();
            if (targetNpc != null)
            {
                activeNPC = targetNpc;
                activeNPC.StartDialogue();
                ShowCurrentNode();

                if (targetNpc.Name == "Sarah")
                {
                    Quests.ProgressObjective(this, "quest_rehan", "bertemu_sarah");
                }
                return;
            }

            if (safe.IsLocked && safe.IsVisible && safe.IsPlayerInRange(player.Bounds))
            {
                safe.Puzzle.Start(totalPins: 3, maxAttempts: 3);
            }
        }

        private void HandleChoiceSelected(int index)
        {
            if (activeNPC == null) return;

            var next = activeNPC.SelectChoice(index, this);
            if (next != null)
            {
                ShowCurrentNode();
            }
            else
            {
                dialogueBox.Hide();
                activeNPC = null;
            }
        }

        private void ShowCurrentNode()
        {
            var node = activeNPC.GetCurrentNode();
            if (node == null)
            {
                dialogueBox.Hide();
                activeNPC = null;
                return;
            }

            if (node.HasChoices)
            {
                var choiceTexts = node.Choices.ConvertAll(c => c.Text);
                dialogueBox.ShowChoices(node.Text, choiceTexts);
            }
            else
            {
                dialogueBox.ShowText(node.Text);
            }
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.FromArgb(60, 120, 60));

            foreach (var npc in npcs) npc.Draw(g);
            safe.Draw(g);
            player.Draw(g);

            var nearbyNpc = GetNearbyNpc();
            if (nearbyNpc != null && !dialogueBox.IsVisible)
            {
                g.DrawString("Tekan [E]", new Font("Segoe UI", 9, FontStyle.Bold),
                    Brushes.Yellow, nearbyNpc.X, nearbyNpc.Y - 20);
            }

            if (safe.IsLocked && safe.IsVisible && !safe.Puzzle.IsRunning && safe.IsPlayerInRange(player.Bounds))
            {
                g.DrawString("Tekan [E]", new Font("Segoe UI", 9, FontStyle.Bold),
                    Brushes.Yellow, safe.X, safe.Y - 20);
            }

            dialogueBox.Draw(g);

            if (safe.Puzzle.IsRunning)
            {
                var puzzleArea = new Rectangle(150, 200, 500, 30);
                safe.Puzzle.Draw(g, puzzleArea);
            }

            Quests.Draw(g, 600, 20);
        }
    }
}