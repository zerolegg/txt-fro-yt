using ChordPathFinder.Models;

namespace ChordPathFinder.Services;

public sealed class PathFinder
{
    private readonly IReadOnlyDictionary<string, IReadOnlyList<string>> _graph;

    public PathFinder(IReadOnlyDictionary<string, IReadOnlyList<string>> graph)
    {
        _graph = graph;
    }

    public IReadOnlyList<string> FindShortestPath(string start, string end)
    {
        if (start == end)
        {
            return new[] { start };
        }

        var queue = new Queue<string>();
        var visited = new HashSet<string> { start };
        var parent = new Dictionary<string, string?> { [start] = null };

        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            foreach (var next in _graph[current])
            {
                if (!visited.Add(next))
                {
                    continue;
                }

                parent[next] = current;
                if (next == end)
                {
                    return BuildPath(parent, end);
                }

                queue.Enqueue(next);
            }
        }

        return Array.Empty<string>();
    }

    public IReadOnlyList<string> FindSmoothPath(string start, string end)
    {
        if (start == end)
        {
            return new[] { start };
        }

        var distances = _graph.Keys.ToDictionary(node => node, _ => int.MaxValue);
        var previous = new Dictionary<string, string?>();
        var queue = new PriorityQueue<string, int>();

        distances[start] = 0;
        previous[start] = null;
        queue.Enqueue(start, 0);

        while (queue.Count > 0)
        {
            queue.TryDequeue(out var current, out var currentDistance);
            if (current is null || currentDistance > distances[current])
            {
                continue;
            }

            if (current == end)
            {
                return BuildPath(previous, end);
            }

            foreach (var neighbor in _graph[current])
            {
                var weight = 1;
                var candidate = distances[current] + weight;
                if (candidate >= distances[neighbor])
                {
                    continue;
                }

                distances[neighbor] = candidate;
                previous[neighbor] = current;
                queue.Enqueue(neighbor, candidate);
            }
        }

        return Array.Empty<string>();
    }

    public IReadOnlyList<IReadOnlyList<string>> FindCreativePaths(string start, string end, int maxDepth = 7, int maxResults = 3)
    {
        if (start == end)
        {
            return new List<IReadOnlyList<string>> { new[] { start } };
        }

        var results = new List<IReadOnlyList<string>>();
        var path = new List<string> { start };
        var visited = new HashSet<string> { start };

        void Dfs(string current)
        {
            if (results.Count >= maxResults || path.Count > maxDepth)
            {
                return;
            }

            if (current == end)
            {
                results.Add(path.ToList());
                return;
            }

            foreach (var neighbor in _graph[current])
            {
                if (visited.Contains(neighbor))
                {
                    continue;
                }

                visited.Add(neighbor);
                path.Add(neighbor);
                Dfs(neighbor);
                path.RemoveAt(path.Count - 1);
                visited.Remove(neighbor);

                if (results.Count >= maxResults)
                {
                    return;
                }
            }
        }

        Dfs(start);
        return results;
    }

    private static IReadOnlyList<string> BuildPath(IReadOnlyDictionary<string, string?> parents, string end)
    {
        var path = new List<string>();
        var current = end;

        while (current is not null)
        {
            path.Add(current);
            if (!parents.TryGetValue(current, out var parent))
            {
                break;
            }

            current = parent!;
        }

        path.Reverse();
        return path;
    }
}
