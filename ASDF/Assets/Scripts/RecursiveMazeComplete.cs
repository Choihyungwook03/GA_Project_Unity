using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecursiveMazeComplete : MonoBehaviour
{
    [Header("미로 설정")]
    public int width = 21;  // 홀수 추천
    public int height = 21; // 홀수 추천
    public float cellSize = 2f;
    public GameObject cellPrefab;

    private MazeCell[,] grid;
    private Vector2Int start = new Vector2Int(1, 1);
    private Vector2Int goal;
    private List<MazeCell> escapePath;

    void Awake()
    {
        goal = new Vector2Int(width - 2, height - 2);
    }

    void Start()
    {
        StartCoroutine(GenerateMazeCoroutine());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(GenerateMazeCoroutine());

        if (Input.GetKeyDown(KeyCode.R))
            ShowEscapePath();
    }

    #region Maze Generation

    IEnumerator GenerateMazeCoroutine()
    {
        // 기존 셀 제거
        if (grid != null)
        {
            foreach (var c in grid)
                if (c != null) Destroy(c.gameObject);

            grid = null;
            yield return null; // 한 프레임 대기
        }

        // 새 Grid 생성
        grid = new MazeCell[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = new Vector3(x * cellSize, 0, y * cellSize);
                MazeCell cell = Instantiate(cellPrefab, pos, Quaternion.identity, transform).GetComponent<MazeCell>();
                cell.Initialize(x, y);
                cell.ShowAllWalls();
                grid[x, y] = cell;
            }
        }

        // 외곽 벽 방문 처리
        for (int x = 0; x < width; x++)
        {
            grid[x, 0].visited = true;
            grid[x, height - 1].visited = true;
        }
        for (int y = 0; y < height; y++)
        {
            grid[0, y].visited = true;
            grid[width - 1, y].visited = true;
        }

        // 재귀 DFS로 미로 생성
        GenerateMazeRecursive(start.x, start.y);

        Debug.Log("미로 생성 완료!");
        yield return null;
    }

    void GenerateMazeRecursive(int x, int y)
    {
        grid[x, y].visited = true;

        Vector2Int[] dirs = { new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1) };
        Shuffle(dirs);

        foreach (var d in dirs)
        {
            int nx = x + d.x * 2;
            int ny = y + d.y * 2;

            if (nx > 0 && nx < width - 1 && ny > 0 && ny < height - 1 && !grid[nx, ny].visited)
            {
                int mx = x + d.x;
                int my = y + d.y;

                RemoveWallBetween(grid[x, y], grid[mx, my], d);
                GenerateMazeRecursive(nx, ny);
            }
        }
    }

    void RemoveWallBetween(MazeCell a, MazeCell b, Vector2Int dir)
    {
        if (a == null || b == null) return;

        if (dir.x == 1) { a.RemoveWall("right"); b.RemoveWall("left"); }
        else if (dir.x == -1) { a.RemoveWall("left"); b.RemoveWall("right"); }
        else if (dir.y == 1) { a.RemoveWall("top"); b.RemoveWall("bottom"); }
        else if (dir.y == -1) { a.RemoveWall("bottom"); b.RemoveWall("top"); }
    }

    void Shuffle(Vector2Int[] arr)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            int r = Random.Range(i, arr.Length);
            Vector2Int tmp = arr[i];
            arr[i] = arr[r];
            arr[r] = tmp;
        }
    }

    #endregion

    #region Pathfinding

    void ShowEscapePath()
    {
        if (grid == null) return;

        bool[,] visited = new bool[width, height];
        escapePath = new List<MazeCell>();

        if (!DFSFindPath(start.x, start.y, visited, escapePath))
        {
            Debug.LogWarning("탈출 경로 없음");
            return;
        }

        foreach (var cell in escapePath)
            if (cell != null)
                cell.SetColor(Color.green);

        Debug.Log($"경로 표시 완료! 길이: {escapePath.Count}");
    }

    bool DFSFindPath(int x, int y, bool[,] visited, List<MazeCell> path)
    {
        if (x < 0 || y < 0 || x >= width || y >= height) return false;
        if (visited[x, y] || !IsCellOpen(grid[x, y])) return false;

        visited[x, y] = true;
        path.Add(grid[x, y]);

        if (x == goal.x && y == goal.y) return true;

        Vector2Int[] dirs = { new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1) };
        foreach (var d in dirs)
            if (DFSFindPath(x + d.x, y + d.y, visited, path)) return true;

        path.RemoveAt(path.Count - 1);
        return false;
    }

    bool IsCellOpen(MazeCell cell)
    {
        return !cell.leftWall.activeSelf || !cell.rightWall.activeSelf || !cell.topWall.activeSelf || !cell.bottomWall.activeSelf;
    }

    #endregion
}
