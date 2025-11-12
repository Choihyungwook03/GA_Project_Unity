using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public static MazeGenerator Instance;

    [Header("미로 설정")]
    public int width = 21;  // 홀수 추천
    public int height = 21; // 홀수 추천
    public float cellSize = 1f;
    public GameObject cellPrefab;

    private MazeCell[,] grid;

    void Awake() { Instance = this; }

    void Start() { StartCoroutine(GenerateMazeRoutine()); }
    void Update() { if (Input.GetKeyDown(KeyCode.Space)) StartCoroutine(GenerateMazeRoutine()); }

    public MazeCell GetCell(int x, int z)
    {
        if (x < 0 || x >= width || z < 0 || z >= height) return null;
        return grid[x, z];
    }

    IEnumerator GenerateMazeRoutine()
    {
        // 기존 셀 제거
        if (grid != null)
        {
            foreach (var c in grid) if (c != null) Destroy(c.gameObject);
        }

        // 생성
        grid = new MazeCell[width, height];
        for (int x = 0; x < width; x++)
            for (int z = 0; z < height; z++)
            {
                Vector3 pos = new Vector3(x * cellSize, 0, z * cellSize);
                MazeCell cell = Instantiate(cellPrefab, pos, Quaternion.identity, transform).GetComponent<MazeCell>();
                cell.Initialize(x, z);
                grid[x, z] = cell;
            }

        yield return null;

        // 미로 재귀 생성
        GenerateMazeRecursive(1, 1);

        yield return null;

        // 탈출 가능 확인
        AIPathfinder pathfinder = FindObjectOfType<AIPathfinder>();
        if (pathfinder != null)
        {
            List<MazeCell> path = pathfinder.FindPathBFS(grid[1, 1], grid[width - 2, height - 2]);
            if (path == null)
            {
                Debug.Log("탈출 불가, 다시 생성...");
                yield return new WaitForSeconds(0.05f);
                StartCoroutine(GenerateMazeRoutine());
                yield break;
            }
            else
            {
                Debug.Log("탈출 가능한 미로 생성 완료!");
            }
        }
    }

    void GenerateMazeRecursive(int x, int z)
    {
        grid[x, z].visited = true;

        List<Vector2Int> dirs = new List<Vector2Int> { new(1, 0), new(-1, 0), new(0, 1), new(0, -1) };
        // 랜덤 섞기
        for (int i = 0; i < dirs.Count; i++)
        {
            Vector2Int tmp = dirs[i];
            int r = Random.Range(i, dirs.Count);
            dirs[i] = dirs[r];
            dirs[r] = tmp;
        }

        foreach (Vector2Int dir in dirs)
        {
            int nx = x + dir.x * 2;
            int nz = z + dir.y * 2;

            if (nx > 0 && nx < width - 1 && nz > 0 && nz < height - 1 && !grid[nx, nz].visited)
            {
                MazeCell middle = grid[x + dir.x, z + dir.y];
                if (dir.x == 1) { grid[x, z].RemoveWall("right"); middle.RemoveWall("left"); }
                else if (dir.x == -1) { grid[x, z].RemoveWall("left"); middle.RemoveWall("right"); }
                else if (dir.y == 1) { grid[x, z].RemoveWall("top"); middle.RemoveWall("bottom"); }
                else if (dir.y == -1) { grid[x, z].RemoveWall("bottom"); middle.RemoveWall("top"); }

                GenerateMazeRecursive(nx, nz);
            }
        }
    }
}
