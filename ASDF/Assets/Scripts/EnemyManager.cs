using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int enemyCount = 3;

    public List<Vector2Int> enemyPositions = new List<Vector2Int>();

    public void SpawnEnemies(int[,] map)
    {
        enemyPositions.Clear();

        int w = map.GetLength(0);
        int h = map.GetLength(1);

        for (int i = 0; i < enemyCount; i++)
        {
            Vector2Int pos;

            while (true)
            {
                int x = Random.Range(0, w);
                int y = Random.Range(0, h);

                if (map[x, y] != 0)  
                {
                    pos = new Vector2Int(x, y);
                    enemyPositions.Add(pos);

                    Instantiate(enemyPrefab, new Vector3(x, 0.5f, y), Quaternion.identity);
                    break;
                }
            }
        }
    }
}
