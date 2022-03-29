using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path
{

    private float speed = 2;
    public List<Tile> tilesInPath;
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
    public void DeletePath()
    {
        foreach (Transform child in renderedPath.transform)
        {
            MonoBehaviour.Destroy(child.gameObject);
        }
        renderedPath.positionCount = 0;
    }
    void ClonePath(LineRenderer constantPath)
    {
        Vector3[] positions = new Vector3[temporaryPath.positionCount];
        constantPath.positionCount = temporaryPath.positionCount;
        temporaryPath.GetPositions(positions);
        constantPath.SetPositions(positions);
        Transform[] indices = temporaryPath.GetComponentsInChildren<Transform>();
        foreach (Transform child in indices)
        {
            if (!child.GetComponent<LineRenderer>())
            {
                child.parent = constantPath.transform;
            }
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
    //get and set
}

