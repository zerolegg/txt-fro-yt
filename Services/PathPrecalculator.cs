using ChordPathFinder.Models;

namespace ChordPathFinder.Services;

public sealed class PathPrecalculator
{
    private readonly PathFinder _pathFinder;

    public PathPrecalculator(PathFinder pathFinder)
    {
        _pathFinder = pathFinder;
    }

    public IReadOnlyDictionary<(string start, string end), PathSet> BuildCache(IReadOnlyList<string> chordNames)
    {
        var cache = new Dictionary<(string start, string end), PathSet>();

        foreach (var start in chordNames)
        {
            foreach (var end in chordNames)
            {
                var pathSet = new PathSet
                {
                    ShortestPath = _pathFinder.FindShortestPath(start, end),
                    SmoothPath = _pathFinder.FindSmoothPath(start, end),
                    CreativePaths = _pathFinder.FindCreativePaths(start, end)
                };

                cache[(start, end)] = pathSet;
            }
        }

        return cache;
    }
}
