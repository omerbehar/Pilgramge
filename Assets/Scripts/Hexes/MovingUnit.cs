using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingUnit : GeneralUnit
{
    protected float speed = 2;

    public Path currentPath = null;
    protected Path savedPath = null;
    //protected LineRenderer temporaryPath;
    //protected LineRenderer constantPath;
    //protected LineRenderer setPath;
    public bool isPathSet = false;
    protected GameObject fogOfWarMask;
    protected GameObject fogOfWarMap;
    //public GameObject dotBetweenStepsPreFab;
    protected int movementPerTurn;
    protected int movementLeftThisTurn;

    public GameObject mouse;
    // Start is called before the first frame update
    protected void Start()
    {
        base.Start();
        fogOfWarMask = Resources.Load<GameObject>("Prefabs/FogOfWarMask");
        fogOfWarMap = GameObject.Find("FogOfWarMap");

        clearFogOfWar();
        //temporaryPath = new GameObject("temporaryPath").AddComponent<LineRenderer>();
        //temporaryPath.transform.parent = transform;
        //constantPath = new GameObject("constantPath").AddComponent<LineRenderer>();
        //constantPath.transform.parent = transform;
    
    }

    // Update is called once per frame
    protected void Update()
    {
        base.Update();
        if (currentPath != null && !isPathSet)
        {
            currentPath.DrawPath();
        }
    }

    private void clearFogOfWar()
    {
        GameObject fogOfWarMask = Instantiate(this.fogOfWarMask, transform);
        fogOfWarMask.transform.parent = fogOfWarMap.transform;
    }

    //private void DefinePaths()
    //{
    //    if (temporaryPath)
    //    {
    //        temporaryPath.sortingLayerName = "TemporaryPath";
    //        temporaryPath.startColor = Color.red;
    //        temporaryPath.endColor = Color.red;
    //        temporaryPath.startWidth = 0.03f;
    //        temporaryPath.endWidth = 0.03f;
    //        temporaryPath.useWorldSpace = true;
    //        temporaryPath.material = new Material(Shader.Find("Sprites/Default"));
    //    }
    //    if (constantPath)
    //    {
    //        constantPath.startColor = Color.black;
    //        constantPath.endColor = Color.black;
    //        constantPath.startWidth = 0.03f;
    //        constantPath.endWidth = 0.03f;
    //        constantPath.useWorldSpace = true;
    //        constantPath.material = new Material(Shader.Find("Sprites/Default"));
    //    }
    //}

    //public void createTemporaryPath()
    //{

    //    DefinePaths();
        
    //    temporaryPath.positionCount = currentPath.Count;
    //    foreach (Transform child in temporaryPath.transform)
    //    {
    //        GameObject.Destroy(child.gameObject);
    //    }
    //    int currentNode = 0;
    //    while (currentNode < currentPath.Count - 1)
    //    {
    //        Vector3 currentLocation = _map.GetComponent<MapCreator>().hexToWorldCoord((int)currentPath[currentNode].GetTilePosition().x, (int)currentPath[currentNode].GetTilePosition().y);
    //        Vector3 nextLocation = _map.GetComponent<MapCreator>().hexToWorldCoord((int)currentPath[currentNode+1].GetTilePosition().x, (int)currentPath[currentNode+1].GetTilePosition().y);
    //        temporaryPath.SetPosition(currentNode, currentLocation);
    //        temporaryPath.SetPosition(currentNode + 1, nextLocation);
    //        GameObject dot = Instantiate(dotBetweenStepsPreFab);
    //        dot.transform.position = nextLocation;
    //        dot.transform.parent = temporaryPath.transform;
    //        dot.name = transform.name + "_dot_" + currentNode;
    //        currentNode++;
    //    }
    //}

    public void NextMovement()
    {
        int movementCost = 1; //change later to consider real cost
        //return if path empty
        if (savedPath == null || movementLeftThisTurn < movementCost)
        {
            return;
        }
        //remove current position from path
        savedPath.tilesInPath.RemoveAt(0);
        movementLeftThisTurn -= movementCost;
        //move to next position on path
        StartCoroutine(MoveUnit());

        //clear moved path (now its one point only, can be easily changed to adjust for bigger movements)
        //RemovePathPoints(constantPath, 1);
        savedPath.NextMove(1);
        //update unit position
        tileX = (int)savedPath.tilesInPath[0].GetTilePosition().x;
        tileY = (int)savedPath.tilesInPath[0].GetTilePosition().y;
        clearFogOfWar();
    }
    private IEnumerator MoveUnit()
    {
        mouse.GetComponent<Mouse>().UpdateIsNextTurnActive(1);
        //smooth movement
        Vector2 dirVector = _map.GetComponent<MapCreator>().hexToWorldCoord((int)savedPath.tilesInPath[0].GetTilePosition().x, (int)savedPath.tilesInPath[0].GetTilePosition().y) - transform.position;   
        while (dirVector.magnitude > 0.01)
        {
            //unparent dots so they dont move with the unit
            savedPath.MoveUnit();
            Vector2 veolcity = dirVector.normalized * speed * Time.deltaTime;
            veolcity = Vector2.ClampMagnitude(veolcity, dirVector.magnitude);
            transform.Translate(veolcity);
            dirVector = _map.GetComponent<MapCreator>().hexToWorldCoord((int)savedPath.tilesInPath[0].GetTilePosition().x, (int)savedPath.tilesInPath[0].GetTilePosition().y) - transform.position;
            //ParentDots(newDots);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        mouse.GetComponent<Mouse>().UpdateIsNextTurnActive(-1);
        
        //if path has one node than after moving you reached target and can clear path
        if (savedPath.tilesInPath.Count == 1)
        {
            savedPath.DeletePath();
            savedPath = null;
        }
    }

    //private Transform[] UnparentDots(Transform[] dots)
    //{
    //    Transform[] newDots = new Transform[dots.Length];
    //    for (int i = 1; i < dots.Length; i++)
    //    {
    //        dots[i].parent = dots[i].parent.parent.parent;
    //        newDots[i] = dots[i];
    //    }
    //    return newDots;
    //}
    //private void ParentDots(Transform[] newDots)
    //{
    //    for (int i = 1; i < newDots.Length; i++)
    //    {
    //        newDots[i].parent = constantPath.transform;
    //    }
    //}
    //private void RemovePathPoints(LineRenderer path, int pointsToRemove)
    //{
    //    Vector3[] positions = new Vector3[path.positionCount];
    //    Vector3[] newPositions = new Vector3[path.positionCount - pointsToRemove];
    //    path.GetPositions(positions);
    //    for (int i = 0; i < path.positionCount - pointsToRemove; i++)
    //    {
    //        newPositions[i] = positions[i + pointsToRemove];
    //    }
    //    path.SetPositions(newPositions);
    //    Transform[] dots = path.GetComponentsInChildren<Transform>();
    //    if (dots.Length > 1)
    //    {
    //        Destroy(dots[1].gameObject);
    //    }
    //}

    public void SetPath()
    {
        isPathSet = true;
        savedPath = currentPath;
        currentPath = null;
        savedPath.DeletePath(); ;
        ClonePath();
        currentPath.DeletePath();
        NextMovement(); //moves first step
    }

   

    //public void deleteConstantPath()
    //{
    //    foreach (Transform child in constantPath.transform)
    //    {
    //        Destroy(child.gameObject);
    //    }
    //    constantPath.positionCount = 0;
    //}

    //public void DeleteTemporaryPath()
    //{
    //    foreach (Transform child in temporaryPath.transform)
    //    {
    //        Destroy(child.gameObject);
    //    }
    //    temporaryPath.positionCount = 0;
    //}
    public void UnSelect()//override unselect + remove temp path
    {
        base.UnSelect();
        if (!isPathSet)
        {
            currentPath = null;
        }
        currentPath.DeletePath();
    }
    void ClonePath()
    {
        Vector3[] positions = new Vector3[currentPath.GetPathPositionCount()];
        savedPath.SetPathPositionCount(currentPath.GetPathPositionCount());
        currentPath.renderedPath.GetPositions(positions);
        savedPath.renderedPath.SetPositions(positions);
        Transform[] indices = currentPath.renderedPath.GetComponentsInChildren<Transform>();
        foreach (Transform child in indices)
        {
            if (!child.GetComponent<LineRenderer>())
            {
                child.parent = savedPath.renderedPath.transform;
            }
        }
    }

    internal void ResetMovementLeft()
    {
        movementLeftThisTurn = movementPerTurn;
    }
}
