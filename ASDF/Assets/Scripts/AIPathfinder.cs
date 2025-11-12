using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPathfinder : MonoBehaviour
{
    public Color pathPreviewColor = Color.green;
    private List<MazeCell> currentPath;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ShowEscapePath();
        }
    }

    public List<MazeCell> FindPathBFS(MazeCell start, MazeCell end)
    {
        Queue<MazeCell> queue = new Queue<MazeCell>();
        Dictionary<MazeCell, MazeCell> parent = new Dictionary<MazeCell, MazeCell>();

        foreach (MazeCell cell in MazeGenerator.Instance.GetComponentsInChildren<MazeCell>())
            cell.visited = false;

        start.visited = true;
        queue.Enqueue(start);
        parent[start] = null;

        while (queue.Count > 0)
        {
            MazeCell cur = queue.Dequeue();
            if (cur == end) break;

            foreach (MazeCell n in GetNeighbors(cur))
            {
                if (!n.visited)
                {
                    n.visited = true;
                    queue.Enqueue(n);
                    parent[n] = cur;
                }
            }
        }

        if (!parent.ContainsKey(end)) return null;

        List<MazeCell> path = new List<MazeCell>();
        MazeCell node = end;
        while (node != null)
        {
            path.Add(node);
            node = parent[node];
        }
        path.Reverse();
        return path;
    }

    List<MazeCell> GetNeighbors(MazeCell cell)
    {
        List<MazeCell> list = new List<MazeCell>();
        MazeGenerator gen = MazeGenerator.Instance;

        if (cell.x > 0 && !cell.leftWall.activeSelf) list.Add(gen.GetCell(cell.x - 1, cell.z));
        if (cell.x < gen.width - 1 && !cell.rightWall.activeSelf) list.Add(gen.GetCell(cell.x + 1, cell.z));
        if (cell.z > 0 && !cell.bottomWall.activeSelf) list.Add(gen.GetCell(cell.x, cell.z - 1));
        if (cell.z < gen.height - 1 && !cell.topWall.activeSelf) list.Add(gen.GetCell(cell.x, cell.z + 1));

        return list;
    }

    public void ShowEscapePath()
    {
        MazeGenerator gen = MazeGenerator.Instance;
        MazeCell start = gen.GetCell(1, 1);
        MazeCell end = gen.GetCell(gen.width - 2, gen.height - 2);

        currentPath = FindPathBFS(start, end);
        if (currentPath == null) { Debug.LogWarning("경로 없음"); return; }

        foreach (MazeCell cell in currentPath)
            cell.SetColor(pathPreviewColor);

        Debug.Log($"탈출 경로 표시 완료 ({currentPath.Count}칸)");
    }
}
