namespace ChordPathFinder.Models;

public sealed class ChordDefinition
{
    public ChordDefinition(string name, IReadOnlyList<string> transitions)
    {
        Name = name;
        Transitions = transitions;
    }

    public string Name { get; }

    public IReadOnlyList<string> Transitions { get; }
}
