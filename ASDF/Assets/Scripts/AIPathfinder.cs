using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPathfinder : MonoBehaviour
{
    [Header("AI 설정")]
    public float moveSpeed = 3f;
    public Color aiColor = Color.blue;

    [Header("경로 시각화")]
    public bool showPath = true;
    public Color pathPreviewColor = Color.green;

    private List<MazeCell> currentPath;
    private int pathIndex = 0;
    private bool isMoving = false;
    private Vector3 targetPosition;

    void Start()
    {
        GetComponent<Renderer>().material.color = aiColor;
        targetPosition = transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isMoving)
        {
            StartPathfinding();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetPosition();
        }

        if (isMoving)
        {
            MoveAlongPath();
        }
    }

    public void ResetAI()
    {
        isMoving = false;
        pathIndex = 0;

        currentPath = null;    
        targetPosition = transform.position;
    }

    List<MazeCell> GetAccessibleNeighbors(MazeCell cell)
    {
        List<MazeCell> neighbors = new List<MazeCell>();
        MazeGenerator gen = MazeGenerator.Instance;

        if (cell.x > 0 && !cell.leftWall.activeSelf)
            neighbors.Add(gen.GetCell(cell.x - 1, cell.z));

        if (cell.x < gen.width - 1 && !cell.rightWall.activeSelf)
            neighbors.Add(gen.GetCell(cell.x + 1, cell.z));

        if (cell.z > 0 && !cell.bottomWall.activeSelf)
            neighbors.Add(gen.GetCell(cell.x, cell.z - 1));

        if (cell.z < gen.height - 1 && !cell.topWall.activeSelf)
            neighbors.Add(gen.GetCell(cell.x, cell.z + 1));

        return neighbors;
    }

    void ResetVisited()
    {
        MazeGenerator gen = MazeGenerator.Instance;

        for (int x = 0; x < gen.width; x++)
        {
            for (int z = 0; z < gen.height; z++)
            {
                MazeCell cell = gen.GetCell(x, z);
                if (cell != null)
                    cell.visited = false;
            }
        }
    }

    public void ResetPosition()
    {
        transform.position = new Vector3(0, transform.position.y, 0);
        targetPosition = transform.position;

        isMoving = false;
        pathIndex = 0;

        if (currentPath != null)
        {
            foreach (MazeCell cell in currentPath)
            {
                if (cell != null)
                    cell.SetColor(Color.white);
            }
        }
        currentPath = null;
    }

    List<MazeCell> FindPathBFS(MazeCell start, MazeCell end)
    {
        ResetVisited();

        Queue<MazeCell> queue = new Queue<MazeCell>();
        Dictionary<MazeCell, MazeCell> parent = new Dictionary<MazeCell, MazeCell>();

        start.visited = true;
        queue.Enqueue(start);
        parent[start] = null;

        while (queue.Count > 0)
        {
            MazeCell current = queue.Dequeue();

            if (current == end)
                break;

            foreach (MazeCell next in GetAccessibleNeighbors(current))
            {
                if (!next.visited)
                {
                    next.visited = true;
                    parent[next] = current;
                    queue.Enqueue(next);
                }
            }
        }

        if (!parent.ContainsKey(end))
            return null;

        List<MazeCell> path = new List<MazeCell>();
        MazeCell cur = end;

        while (cur != null)
        {
            path.Add(cur);
            cur = parent[cur];
        }

        path.Reverse();
        return path;
    }


    public void StartPathfinding()
    {
        MazeGenerator gen = MazeGenerator.Instance;

        int startX = Mathf.RoundToInt(transform.position.x / gen.cellSize);
        int startZ = Mathf.RoundToInt(transform.position.z / gen.cellSize);

        MazeCell start = gen.GetCell(startX, startZ);
        MazeCell end = gen.GetCell(gen.width - 1, gen.height - 1);

        if (start == null || end == null)
        {
            Debug.LogError("시작점 또는 목표가 존재하지 않습니다.");
            return;
        }

        currentPath = FindPathBFS(start, end);

        if (currentPath != null)
        {
            if (showPath)
                ShowPathPreview();

            pathIndex = 0;
            isMoving = true;
        }
        else
        {
            Debug.LogError("경로를 찾지 못했습니다.");
        }
    }

    void ShowPathPreview()
    {
        foreach (MazeCell cell in currentPath)
        {
            if (cell != null)
                cell.SetColor(pathPreviewColor);
        }
    }

    void MoveAlongPath()
    {
        if (currentPath == null || pathIndex >= currentPath.Count)
        {
            isMoving = false;
            return;
        }

        MazeCell targetCell = currentPath[pathIndex];

        if (targetCell == null)
        {
            isMoving = false;
            return;
        }

        targetPosition = new Vector3(
            targetCell.x * MazeGenerator.Instance.cellSize,
            transform.position.y,
            targetCell.z * MazeGenerator.Instance.cellSize
        );

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            transform.position = targetPosition;
            pathIndex++;
        }
    }
}
