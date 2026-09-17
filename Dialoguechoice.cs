using System;
using PlayerMovementTester;

public class DialogueChoice
{
    public string Text;
    public string NextNodeId;
    public Action<GameForm> OnSelect;

    public DialogueChoice(string text, string nextNodeId = null, Action<GameForm> onSelect = null)
    {
        Text = text;
        NextNodeId = nextNodeId;
        OnSelect = onSelect;
    }
}