using UnityEngine;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{
    public int width = 21;
    public int height = 21;

    public GameObject wallPrefab;
    public GameObject groundPrefab;
    public GameObject forestPrefab;
    public GameObject mudPrefab;

    public GameObject enemyPrefab;
    public int enemyCount = 3;

    public GameObject pathMarkerPrefab;

    Tile[,] tiles;
    int[,] map;

    Vector2Int start;
    Vector2Int end;

    public AStarPathfinder pathfinder;

    public Transform tileRoot;

    List<GameObject> markers = new List<GameObject>();
    List<Transform> enemies = new List<Transform>();

    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        if (tileRoot != null)
            for (int i = tileRoot.childCount - 1; i >= 0; i--)
                Destroy(tileRoot.GetChild(i).gameObject);

        enemies.Clear();

        map = new int[width, height];
        tiles = new Tile[width, height];

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                int r = Random.Range(0, 5);
                if (r == 0) map[x, y] = 0;        
                else if (r < 3) map[x, y] = 1;   
                else if (r == 3) map[x, y] = 2;  
                else map[x, y] = 3;              
            }

        start = new Vector2Int(1, 1);
        end = new Vector2Int(width - 2, height - 2);
        map[start.x, start.y] = 1;
        map[end.x, end.y] = 1;

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                GameObject prefab = groundPrefab;
                if (map[x, y] == 0) prefab = wallPrefab;
                else if (map[x, y] == 2) prefab = forestPrefab;
                else if (map[x, y] == 3) prefab = mudPrefab;

                Instantiate(prefab, new Vector3(x, 0, y), Quaternion.identity, tileRoot);
            }

        SpawnEnemies();

        pathfinder.enemies = enemies.ToArray();
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            Vector2Int pos;

            do
            {
                pos = new Vector2Int(
                    Random.Range(1, width - 1),
                    Random.Range(1, height - 1)
                );
            } while (map[pos.x, pos.y] == 0);

            GameObject e = Instantiate(enemyPrefab, new Vector3(pos.x, 0.5f, pos.y), Quaternion.identity);
            enemies.Add(e.transform);
        }
    }

    public void FindPathButton()
    {
        ClearMarkers();
        List<Vector2Int> path = pathfinder.FindPath(map, start, end);

        if (path == null)
        {
            Debug.Log("±æ ¾øÀ½");
            return;
        }

        foreach (var p in path)
        {
            GameObject m = Instantiate(pathMarkerPrefab, new Vector3(p.x, 0.5f, p.y), Quaternion.identity);
            markers.Add(m);
        }
    }

    void ClearMarkers()
    {
        foreach (var m in markers)
            Destroy(m);
        markers.Clear();
    }
}
