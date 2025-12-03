using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinder : MonoBehaviour
{
    public List<Vector2Int> FindPath(int[,] map, Vector2Int start, Vector2Int end)
    {
        int w = map.GetLength(0);
        int h = map.GetLength(1);

        int[,] g = new int[w, h];
        bool[,] visited = new bool[w, h];
        Vector2Int?[,] parent = new Vector2Int?[w, h];

        for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
                g[x, y] = int.MaxValue;

        g[start.x, start.y] = 0;

        List<Vector2Int> open = new List<Vector2Int>();
        open.Add(start);

        Vector2Int[] dirs =
        {
            new Vector2Int(1,0), new Vector2Int(-1,0),
            new Vector2Int(0,1), new Vector2Int(0,-1)
        };

        while (open.Count > 0)
        {
            int best = 0;
            int bestF = g[open[0].x, open[0].y] + H(open[0], end);

            for (int i = 1; i < open.Count; i++)
            {
                int f = g[open[i].x, open[i].y] + H(open[i], end);
                if (f < bestF)
                {
                    bestF = f;
                    best = i;
                }
            }

            Vector2Int cur = open[best];
            open.RemoveAt(best);

            if (visited[cur.x, cur.y]) continue;
            visited[cur.x, cur.y] = true;

            if (cur == end)
                return Reconstruct(parent, start, end);

            foreach (var d in dirs)
            {
                int nx = cur.x + d.x;
                int ny = cur.y + d.y;

                if (!InBounds(map, nx, ny)) continue;
                if (map[nx, ny] == 0) continue;

                int newG = g[cur.x, cur.y] + TileCost(map[nx, ny]);

                if (newG < g[nx, ny])
                {
                    g[nx, ny] = newG;
                    parent[nx, ny] = cur;

                    if (!open.Contains(new Vector2Int(nx, ny)))
                        open.Add(new Vector2Int(nx, ny));
                }
            }
        }

        return null;
    }

    int H(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    int TileCost(int t)
    {
        switch (t)
        {
            case 1: return 1;
            case 2: return 3;
            case 3: return 5;
            default: return 999999;
        }
    }

    bool InBounds(int[,] map, int x, int y)
    {
        return x >= 0 && y >= 0 && x < map.GetLength(0) && y < map.GetLength(1);
    }

    List<Vector2Int> Reconstruct(Vector2Int?[,] parent, Vector2Int start, Vector2Int end)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int? cur = end;

        while (cur.HasValue)
        {
            path.Add(cur.Value);
            if (cur.Value == start) break;
            cur = parent[cur.Value.x, cur.Value.y];
        }

        path.Reverse();
        return path;
    }
}
