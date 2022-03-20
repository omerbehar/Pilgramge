using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileType", menuName = "ScriptableObjects/TileType", order = 1)]
public class TileType : ScriptableObject
{
    //public string name;
    //public GameObject tileVisualPreFab;
    readonly public int ID;
    readonly public Sprite tileImage;
    readonly public bool isWalkable = true;
    readonly public float costToEnter = 1;
}
