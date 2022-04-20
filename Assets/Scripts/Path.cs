using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path
{

    private float speed = 2;
    public List<Tile> tilesInPath;
    public LineRenderer renderedPath;//displayed path
    //private LineRenderer constantPath;
    //private LineRenderer setPath;

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
    private Transform[] UnparentDots(Transform[] dots)
    {
        Transform[] newDots = new Transform[dots.Length];
        for (int i = 1; i < dots.Length; i++)
        {
            dots[i].parent = dots[i].parent.parent.parent;
            newDots[i] = dots[i];
        }
        return newDots;
    }
    private void ParentDots(Transform[] newDots)
    {
        for (int i = 1; i < newDots.Length; i++)
        {
            newDots[i].parent = renderedPath.transform;
        }
    }
    private void RemovePathPoints(int pointsToRemove)
    {
        Vector3[] positions = new Vector3[renderedPath.positionCount];
        Vector3[] newPositions = new Vector3[renderedPath.positionCount - pointsToRemove];
        renderedPath.GetPositions(positions);
        for (int i = 0; i < renderedPath.positionCount - pointsToRemove; i++)
        {
            newPositions[i] = positions[i + pointsToRemove];
        }
        renderedPath.SetPositions(newPositions);
        Transform[] dots = renderedPath.GetComponentsInChildren<Transform>();
        if (dots.Length > 1)
        {
            MonoBehaviour.Destroy(dots[1].gameObject);
        }
    }

    internal void NextMove(int v)
    {
        RemovePathPoints(1);
    }
    public void DeletePath()//delete tiles in path and line renderer 
    {
        foreach (Transform child in renderedPath.transform)
        {
            MonoBehaviour.Destroy(child.gameObject);
        }
        renderedPath.positionCount = 0;
    }
    void ClonePath(LineRenderer constantPath)
    {
        Vector3[] positions = new Vector3[renderedPath.positionCount];
        constantPath.positionCount = renderedPath.positionCount;
        renderedPath.GetPositions(positions);
        constantPath.SetPositions(positions);
        Transform[] indices = renderedPath.GetComponentsInChildren<Transform>();
        foreach (Transform child in indices)
        {
            if (!child.GetComponent<LineRenderer>())
            {
                child.parent = constantPath.transform;
            }
        }
    }
    public void MoveUnit()
    {
        Transform[] dots = renderedPath.GetComponentsInChildren<Transform>();
        Transform[] newDots = UnparentDots(dots);
        ParentDots(newDots);
    }
    
    public void GeneratePath()
    {

        renderedPath.positionCount = tilesInPath.Count;//set line renderer length
        foreach (Transform child in renderedPath.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        int currentNode = 0;
        while (currentNode < tilesInPath.Count - 1)
        {
            Vector3 currentLocation = _map.GetComponent<MapCreator>().hexToWorldCoord((int)tilesInPath[currentNode].GetTilePosition().x, (int)tilesInPath[currentNode].GetTilePosition().y);
            Vector3 nextLocation = _map.GetComponent<MapCreator>().hexToWorldCoord((int)tilesInPath[currentNode + 1].GetTilePosition().x, (int)tilesInPath[currentNode + 1].GetTilePosition().y);
            renderedPath.SetPosition(currentNode, currentLocation);
            renderedPath.SetPosition(currentNode + 1, nextLocation);
            GameObject dot = GameObject.Instantiate(_dotBetweenStepsPreFab);
            dot.transform.position = nextLocation;
            dot.transform.parent = renderedPath.transform;
            dot.name = parentUnit.transform.name + "_dot_" + currentNode;
            currentNode++;
        }
    }
    public void SetPathCurrent()
    {
        renderedPath.sortingLayerName = "TemporaryPath";
        renderedPath.startColor = Color.red;
        renderedPath.endColor = Color.red;
        renderedPath.startWidth = 0.03f;
        renderedPath.endWidth = 0.03f;
        renderedPath.useWorldSpace = true;
        renderedPath.material = new Material(Shader.Find("Sprites/Default"));
    }
    public void SetPathSaved()
    {
        renderedPath.startColor = Color.black;
        renderedPath.endColor = Color.black;
        renderedPath.startWidth = 0.03f;
        renderedPath.endWidth = 0.03f;
        renderedPath.useWorldSpace = true;
        renderedPath.material = new Material(Shader.Find("Sprites/Default"));
    }


    //get and set
    public int GetPathPositionCount()
    {
        return renderedPath.positionCount;
    }
    public void SetPathPositionCount(int positionCount)
    {
        renderedPath.positionCount = positionCount;
    }
}

