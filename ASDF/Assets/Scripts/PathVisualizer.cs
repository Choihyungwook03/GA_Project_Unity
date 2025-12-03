using System.Collections.Generic;
using UnityEngine;

public class PathVisualizer : MonoBehaviour
{
    public GameObject pathMarkerPrefab;
    List<GameObject> markers = new List<GameObject>();

    public void ShowPath(List<Vector2Int> path)
    {
        foreach (var p in path)
        {
            GameObject m = Instantiate(
                pathMarkerPrefab,
                new Vector3(p.x, 0.3f, p.y),
                Quaternion.identity,
                transform
            );
            markers.Add(m);
        }
    }

    public void ClearPath()
    {
        foreach (var m in markers)
            Destroy(m);

        markers.Clear();
    }
}
