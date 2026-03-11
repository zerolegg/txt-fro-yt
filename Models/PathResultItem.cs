namespace ChordPathFinder.Models;

public sealed class PathResultItem
{
    public PathResultItem(string title)
    {
        Title = title;
    }

    public string Title { get; }

    public IReadOnlyList<string> Nodes { get; private set; } = Array.Empty<string>();

    public string DisplayPath => string.Join("\n↑\n", Nodes.Reverse());

    public void SetNodes(IReadOnlyList<string> nodes)
    {
        Nodes = nodes;
    }
}
