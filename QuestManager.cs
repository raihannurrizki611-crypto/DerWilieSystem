using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using PlayerMovementTester;

// "Kantor pusat" quest. NPC, minigame, atau sistem lain manapun cukup
// manggil StartQuest/ProgressObjective dari sini -> mereka tidak perlu
// tahu apa-apa soal isi quest itu sendiri. Ini yang bikin QuestManager
// reusable untuk quest apa pun, bukan cuma untuk Rehan atau si koper.
public class QuestManager
{
    private readonly Dictionary<string, Quest> quests = new Dictionary<string, Quest>();

    public void RegisterQuest(Quest quest) => quests[quest.Id] = quest;

    public void StartQuest(string questId)
    {
        if (quests.TryGetValue(questId, out var quest) && quest.State == QuestState.NotStarted)
        {
            quest.State = QuestState.Active;
        }
    }

    // Dipanggil dari mana saja (trigger event NPC, hasil minigame, dll)
    // setiap kali sesuatu yang relevan dengan sebuah objective terjadi.
    public void ProgressObjective(GameForm form, string questId, string objectiveId, int amount = 1)
    {
        if (!quests.TryGetValue(questId, out var quest)) return;
        if (quest.State != QuestState.Active) return;

        var objective = quest.GetObjective(objectiveId);
        objective?.Progress(amount);

        if (quest.AllObjectivesCompleted)
        {
            quest.State = QuestState.Completed;
            quest.OnComplete?.Invoke(form); // ---- Trigger Event quest selesai ----
        }
    }

    public IEnumerable<Quest> ActiveQuests => quests.Values.Where(q => q.State == QuestState.Active);

    public void Draw(Graphics g, int x, int y)
    {
        var titleFont = new Font("Segoe UI", 9, FontStyle.Bold);
        var itemFont = new Font("Segoe UI", 8);
        int lineY = y;

        foreach (var quest in ActiveQuests)
        {
            g.DrawString(quest.Title, titleFont, Brushes.White, x, lineY);
            lineY += 18;

            foreach (var objective in quest.Objectives)
            {
                string mark = objective.IsCompleted ? "[v]" : "[ ]";
                string text = objective.RequiredCount > 1
                    ? $"{mark} {objective.Description} ({objective.CurrentCount}/{objective.RequiredCount})"
                    : $"{mark} {objective.Description}";

                var brush = objective.IsCompleted ? Brushes.LightGreen : Brushes.LightGray;
                g.DrawString(text, itemFont, brush, x + 10, lineY);
                lineY += 16;
            }

            lineY += 8;
        }
    }
}
