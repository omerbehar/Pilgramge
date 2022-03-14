using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileType 
{
    public string name;
    public GameObject tileVisualPreFab;
    public bool isWalkable = true;
    public float costToEnter = 1;
}
