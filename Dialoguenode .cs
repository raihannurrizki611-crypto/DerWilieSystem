using System.Collections.Generic;

public class DialogueNode
{
    public string Id;
    public string Text;
    public string NextNodeId;
    public List<DialogueChoice> Choices;

    public bool HasChoices => Choices != null && Choices.Count > 0;

    public DialogueNode(string id, string text, string nextNodeId = null, List<DialogueChoice> choices = null)
    {
        Id = id;
        Text = text;
        NextNodeId = nextNodeId;
        Choices = choices;
    }
} 