using System;
using System.Collections.Generic;
using PlayerMovementTester;

public enum QuestState
{
    NotStarted,
    Active,
    Completed
}

// Satu quest: judul, deskripsi, kumpulan objective, dan status.
// OnComplete pakai pola yang sama seperti DialogueChoice.OnSelect dan
// Safe.OnUnlocked -> reward/efek quest selesai ditentukan dari luar,
// class Quest sendiri tidak perlu tahu isinya apa.
public class Quest
{
    public string Id;
    public string Title;
    public string Description;
    public List<QuestObjective> Objectives = new List<QuestObjective>();
    public QuestState State = QuestState.NotStarted;

    public Action<GameForm> OnComplete;

    public bool AllObjectivesCompleted => Objectives.TrueForAll(o => o.IsCompleted);

    public Quest(string id, string title, string description)
    {
        Id = id;
        Title = title;
        Description = description;
    }

    public QuestObjective GetObjective(string objectiveId) => Objectives.Find(o => o.Id == objectiveId);
}