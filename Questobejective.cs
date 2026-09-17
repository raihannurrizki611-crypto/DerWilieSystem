using System;

public class QuestObjective
{
    public string Id;
    public string Description;
    public int RequiredCount;
    public int CurrentCount;

    public bool IsCompleted => CurrentCount >= RequiredCount;

    public QuestObjective(string id, string description, int requiredCount = 1)
    {
        Id = id;
        Description = description;
        RequiredCount = requiredCount;
        CurrentCount = 0;
    }

    public void Progress(int amount = 1)
    {
        CurrentCount = Math.Min(RequiredCount, CurrentCount + amount);
    }
}
