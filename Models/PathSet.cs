namespace ChordPathFinder.Models;

public sealed class PathSet
{
    public IReadOnlyList<string> ShortestPath { get; init; } = Array.Empty<string>();

    public IReadOnlyList<string> SmoothPath { get; init; } = Array.Empty<string>();

    public IReadOnlyList<IReadOnlyList<string>> CreativePaths { get; init; } = Array.Empty<IReadOnlyList<string>>();
}
