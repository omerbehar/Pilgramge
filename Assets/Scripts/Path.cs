using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path
{

    private float speed = 2;
    private List<Tile> tilesInPath;
    private LineRenderer renderedPath;
    //private LineRenderer constantPath;
    private LineRenderer setPath;

    //references
    private GameObject _dotBetweenStepsPreFab;
    private MapCreator _map;
    private MovingUnit parentUnit;

    public Path(MovingUnit unit)
    {
        parentUnit = unit;
        _map = GameObject.FindObjectOfType<MapCreator>();
        _dotBetweenStepsPreFab = Resources.Load<GameObject>("Prefabs/RedCircle");
        InitializeLineRenderer();
    }
    public void InitializeLineRenderer()
    {
        renderedPath = new GameObject("Path").AddComponent<LineRenderer>();
        renderedPath.transform.parent = parentUnit.transform;
        renderedPath.sortingLayerName = "TemporaryPath";
        renderedPath.startColor = Color.red;
        renderedPath.endColor = Color.red;
        renderedPath.startWidth = 0.03f;
        renderedPath.endWidth = 0.03f;
        renderedPath.useWorldSpace = true;
        renderedPath.material = new Material(Shader.Find("Sprites/Default"));
        
    }
    public void DrawPath()
    {
        renderedPath.positionCount = tilesInPath.Count;
        foreach (Transform child in renderedPath.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        int currentTile = 0;
        while (currentTile < tilesInPath.Count - 1)
        {
            Vector3 currentLocation = _map.hexToWorldCoord((int)tilesInPath[currentTile].GetTilePosition().x, (int)tilesInPath[currentTile].GetTilePosition().y);
            Vector3 nextLocation = _map.hexToWorldCoord((int)tilesInPath[currentTile + 1].GetTilePosition().x, (int)tilesInPath[currentTile + 1].GetTilePosition().y);
            renderedPath.SetPosition(currentTile, currentLocation);
            renderedPath.SetPosition(currentTile + 1, nextLocation);
            GameObject dot = MonoBehaviour.Instantiate(_dotBetweenStepsPreFab);
            dot.transform.position = nextLocation;
            dot.transform.parent = renderedPath.transform;
            dot.name = parentUnit.transform.name + "_dot_" + currentTile;
            currentTile++;
        }
    }
    //private void MakePathConstant()
    //{
    //    constantPath.startColor = Color.black;
    //    constantPath.endColor = Color.black;
    //    constantPath.startWidth = 0.03f;
    //    constantPath.endWidth = 0.03f;
    //    constantPath.useWorldSpace = true;
    //    constantPath.material = new Material(Shader.Find("Sprites/Default"));
        
    //}
}

