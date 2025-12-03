using System.Collections.Generic;
using UnityEngine;

public class AStarExample : MonoBehaviour
{
    int[,] map =
    {
        {0, 0, 0, 0, 0},
        {0, 1, 1, 1, 0},
        {0, 1, 2, 1, 0},
        {0, 1, 3, 1, 0},
        {0, 0, 0, 0, 0}
    };

    Vector2Int start = new Vector2Int(1, 1);
    Vector2Int goal = new Vector2Int(3, 3);

    List<Vector2Int> lastPath;

    void Start()
    {
        if (!IsReachableDFS(start, goal))
        {
            Debug.Log("탈출 불가능 → 맵 다시 생성해야 함");
            return;
        }

        lastPath = AStar(map, start, goal);

        if (lastPath == null)
            Debug.Log("경로 없음");
        else
        {
            Debug.Log("A* 경로:");
            foreach (var p in lastPath)
                Debug.Log(p);
        }
    }

    List<Vector2Int> AStar(int[,] map, Vector2Int start, Vector2Int goal)
    {
        int w = map.GetLength(0);
        int h = map.GetLength(1);

        int[,] gCost = new int[w, h];
        bool[,] visited = new bool[w, h];
        Vector2Int?[,] parent = new Vector2Int?[w, h];

        for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
                gCost[x, y] = int.MaxValue;

        gCost[start.x, start.y] = 0;

        List<Vector2Int> open = new List<Vector2Int>();
        open.Add(start);

        Vector2Int[] dirs =
        {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1)
        };

        while (open.Count > 0)
        {
            int bestIndex = 0;
            int bestF = F(open[0], gCost, goal);

            for (int i = 1; i < open.Count; i++)
            {
                int f = F(open[i], gCost, goal);
                if (f < bestF)
                {
                    bestF = f;
                    bestIndex = i;
                }
            }

            Vector2Int cur = open[bestIndex];
            open.RemoveAt(bestIndex);

            if (visited[cur.x, cur.y]) continue;
            visited[cur.x, cur.y] = true;

            
            if (cur == goal)
                return Reconstruct(parent, start, goal);

            foreach (var d in dirs)
            {
                int nx = cur.x + d.x;
                int ny = cur.y + d.y;

                if (!InBounds(map, nx, ny)) continue;
                if (map[nx, ny] == 0) continue; 
                if (visited[nx, ny]) continue;

                int newG = gCost[cur.x, cur.y] + TileCost(map[nx, ny]);

                if (newG < gCost[nx, ny])
                {
                    gCost[nx, ny] = newG;
                    parent[nx, ny] = cur;

                    if (!open.Contains(new Vector2Int(nx, ny)))
                        open.Add(new Vector2Int(nx, ny));
                }
            }
        }

        return null;
    }

    int F(Vector2Int pos, int[,] gCost, Vector2Int goal)
    {
        return gCost[pos.x, pos.y] + H(pos, goal);
    }

    int H(Vector2Int a, Vector2Int b)
    {
        int h = Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y); // 맨해튼

        if (IsNearWall(a))
            h += 2;

        return h;
    }

    bool IsNearWall(Vector2Int p)
    {
        Vector2Int[] around =
        {
            new Vector2Int(1,0), new Vector2Int(-1,0),
            new Vector2Int(0,1), new Vector2Int(0,-1)
        };

        foreach (var d in around)
        {
            int nx = p.x + d.x;
            int ny = p.y + d.y;

            if (InBounds(map, nx, ny) && map[nx, ny] == 0)
                return true; 
        }
        return false;
    }

    int TileCost(int tile)
    {
        switch (tile)
        {
            case 1: return 1; 
            case 2: return 3; 
            case 3: return 5;  
            default: return int.MaxValue;
        }
    }

    bool IsReachableDFS(Vector2Int start, Vector2Int goal)
    {
        bool[,] visited = new bool[map.GetLength(0), map.GetLength(1)];
        return DFS(start, goal, visited);
    }

    bool DFS(Vector2Int cur, Vector2Int goal, bool[,] visited)
    {
        if (cur == goal) return true;
        visited[cur.x, cur.y] = true;

        Vector2Int[] dirs =
        {
            new Vector2Int(1,0), new Vector2Int(-1,0),
            new Vector2Int(0,1), new Vector2Int(0,-1)
        };

        foreach (var d in dirs)
        {
            int nx = cur.x + d.x;
            int ny = cur.y + d.y;

            if (!InBounds(map, nx, ny)) continue;
            if (map[nx, ny] == 0) continue;
            if (visited[nx, ny]) continue;

            if (DFS(new Vector2Int(nx, ny), goal, visited))
                return true;
        }
        return false;
    }

    bool InBounds(int[,] map, int x, int y)
    {
        return x >= 0 && y >= 0 && x < map.GetLength(0) && y < map.GetLength(1);
    }

    List<Vector2Int> Reconstruct(Vector2Int?[,] parent, Vector2Int start, Vector2Int goal)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int? cur = goal;

        while (cur.HasValue)
        {
            path.Add(cur.Value);
            if (cur.Value == start) break;
            cur = parent[cur.Value.x, cur.Value.y];
        }

        path.Reverse();
        return path;
    }
    void OnDrawGizmos()
    {
        if (lastPath == null) return;

        Gizmos.color = Color.green;
        foreach (var p in lastPath)
            Gizmos.DrawCube(new Vector3(p.x, 0, p.y), new Vector3(0.8f, 0.1f, 0.8f));
    }
}
