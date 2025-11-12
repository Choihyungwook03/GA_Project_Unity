using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell : MonoBehaviour
{
    public GameObject leftWall;
    public GameObject rightWall;
    public GameObject topWall;
    public GameObject bottomWall;
    public GameObject floor;

    [HideInInspector] public bool visited = false;
    [HideInInspector] public int x;
    [HideInInspector] public int z;

    public void Initialize(int xPos, int zPos)
    {
        x = xPos;
        z = zPos;
        visited = false;
        ShowAllWalls();
    }

    public void ShowAllWalls()
    {
        if (leftWall != null) leftWall.SetActive(true);
        if (rightWall != null) rightWall.SetActive(true);
        if (topWall != null) topWall.SetActive(true);
        if (bottomWall != null) bottomWall.SetActive(true);
        if (floor != null) floor.SetActive(true);
    }

    public void RemoveWall(string direction)
    {
        switch (direction)
        {
            case "left":
                if (leftWall != null) leftWall.SetActive(false);
                break;
            case "right":
                if (rightWall != null) rightWall.SetActive(false);
                break;
            case "top":
                if (topWall != null) topWall.SetActive(false);
                break;
            case "bottom":
                if (bottomWall != null) bottomWall.SetActive(false);
                break;
        }
    }

    public void SetColor(Color color)
    {
        if (floor != null)
            floor.GetComponent<Renderer>().material.color = color;
    }
}
