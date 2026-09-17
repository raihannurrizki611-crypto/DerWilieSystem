using System.Drawing;
using System.Collections.Generic;
using PlayerMovementTester;

public class NPC
{
    public string Name;
    public int X, Y;
    public int Width = 32, Height = 32;
    public int InteractionRadius = 60;
    public Color Color = Color.IndianRed;

    public bool IsActive = true;

    private readonly Dictionary<string, DialogueNode> nodes = new Dictionary<string, DialogueNode>();
    private string startNodeId;
    private string currentNodeId;

    public Rectangle Bounds => new Rectangle(X, Y, Width, Height);

    public NPC(string name, int startX, int startY)
    {
        Name = name;
        X = startX;
        Y = startY;
    }

    public void AddNode(DialogueNode node)
    {
        nodes[node.Id] = node;
        if (startNodeId == null) startNodeId = node.Id;
    }

    public void SetStartNode(string id) => startNodeId = id;

    public bool IsPlayerInRange(Rectangle playerBounds)
    {
        if (!IsActive) return false;
        Rectangle zone = new Rectangle(
            X - InteractionRadius,
            Y - InteractionRadius,
            Width + InteractionRadius * 2,
            Height + InteractionRadius * 2
        );
        return zone.IntersectsWith(playerBounds);
    }

    public void StartDialogue()
    {
        currentNodeId = startNodeId;
    }

    public DialogueNode GetCurrentNode()
    {
        if (currentNodeId == null || !nodes.ContainsKey(currentNodeId)) return null;
        return nodes[currentNodeId];
    }

    public DialogueNode Advance()
    {
        var current = GetCurrentNode();
        if (current == null) return null;
        currentNodeId = current.NextNodeId;
        return GetCurrentNode();
    }

    public DialogueNode SelectChoice(int index, GameForm context)
    {
        var current = GetCurrentNode();
        if (current == null || !current.HasChoices) return null;
        if (index < 0 || index >= current.Choices.Count) return null;

        var choice = current.Choices[index];
        choice.OnSelect?.Invoke(context);
        currentNodeId = choice.NextNodeId;
        return GetCurrentNode();
    }

    public void Draw(Graphics g)
    {
        if (!IsActive) return;
        g.FillRectangle(new SolidBrush(Color), Bounds);
    }
}