using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingUnit : HexUnit
{
    public Vector3 destination;
    protected float speed = 2;

    public List<Node> currentPath = null;
    protected List<Node> savedPath = null;
    protected LineRenderer temporaryPath;
    protected LineRenderer constantPath;
    protected LineRenderer setPath;
    public bool isPathSet = false;
    protected GameObject fogOfWarMask;
    protected GameObject fogOfWarMap;
    public GameObject dotBetweenStepsPreFab;

    public GameObject mouse;
    // Start is called before the first frame update
    protected void Start()
    {
        base.Start();
        fogOfWarMask = Resources.Load<GameObject>("Prefabs/FogOfWarMask");
        fogOfWarMap = GameObject.Find("FogOfWarMap");

        clearFogOfWar();
        destination = transform.position;
        temporaryPath = new GameObject("temporaryPath").AddComponent<LineRenderer>();
        temporaryPath.transform.parent = transform;
        constantPath = new GameObject("constantPath").AddComponent<LineRenderer>();
        constantPath.transform.parent = transform;
    
    }

    // Update is called once per frame
    protected void Update()
    {
        base.Update();
        if (currentPath != null && !isPathSet)
        {
            createTemporaryPath();
        }
    }

    private void clearFogOfWar()
    {
        GameObject fogOfWarMask = Instantiate(this.fogOfWarMask, transform);
        fogOfWarMask.transform.parent = fogOfWarMap.transform;
    }

    private void DefinePaths()
    {
        if (temporaryPath)
        {
            temporaryPath.sortingLayerName = "TemporaryPath";
            temporaryPath.startColor = Color.red;
            temporaryPath.endColor = Color.red;
            temporaryPath.startWidth = 0.03f;
            temporaryPath.endWidth = 0.03f;
            temporaryPath.useWorldSpace = true;
            temporaryPath.material = new Material(Shader.Find("Sprites/Default"));
        }
        if (constantPath)
        {
            constantPath.startColor = Color.black;
            constantPath.endColor = Color.black;
            constantPath.startWidth = 0.03f;
            constantPath.endWidth = 0.03f;
            constantPath.useWorldSpace = true;
            constantPath.material = new Material(Shader.Find("Sprites/Default"));
        }
    }

    public void createTemporaryPath()
    {

        DefinePaths();
        
        temporaryPath.positionCount = currentPath.Count;
        foreach (Transform child in temporaryPath.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        int currentNode = 0;
        while (currentNode < currentPath.Count - 1)
        {
            Vector3 currentLocation = map.GetComponent<MapCreator>().hexToWorldCoord(currentPath[currentNode].x, currentPath[currentNode].y);
            Vector3 nextLocation = map.GetComponent<MapCreator>().hexToWorldCoord(currentPath[currentNode + 1].x, currentPath[currentNode + 1].y);
            temporaryPath.SetPosition(currentNode, currentLocation);
            temporaryPath.SetPosition(currentNode + 1, nextLocation);
            GameObject dot = Instantiate(dotBetweenStepsPreFab);
            dot.transform.position = nextLocation;
            dot.transform.parent = temporaryPath.transform;
            dot.name = transform.name + "_dot_" + currentNode;
            currentNode++;
        }
    }

    public void NextMovement()
    {
        //return if path empty
        if (savedPath == null)
        {
            return;
        }
        //remove current position from path
        savedPath.RemoveAt(0);
               
        //move to next position on path
        StartCoroutine("MoveUnit");

        //clear moved path (now its one point only, can be easily changed to adjust for bigger movements)
        RemovePathPoints(constantPath, 1);
        
        //update unit position
        tileX = savedPath[0].x;
        tileY = savedPath[0].y;
        clearFogOfWar();
    }
    private IEnumerator MoveUnit()
    {
        mouse.GetComponent<Mouse>().UpdateIsNextTurnActive(1);
        //smooth movement
        Vector2 dirVector = map.GetComponent<MapCreator>().hexToWorldCoord(savedPath[0].x, savedPath[0].y) - transform.position;   
        while (dirVector.magnitude > 0.01)
        {
            //unparent dots so they dont move with the unit
            Transform[] dots = constantPath.GetComponentsInChildren<Transform>();
            Transform[] newDots = UnparentDots(dots);
            Vector2 veolcity = dirVector.normalized * speed * Time.deltaTime;
            veolcity = Vector2.ClampMagnitude(veolcity, dirVector.magnitude);
            transform.Translate(veolcity);
            dirVector = map.GetComponent<MapCreator>().hexToWorldCoord(savedPath[0].x, savedPath[0].y) - transform.position;
            ParentDots(newDots);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        mouse.GetComponent<Mouse>().UpdateIsNextTurnActive(-1);

        //if path has one node than after moving you reached target and can clear path
        if (savedPath.Count == 1)
        {
            savedPath = null;
            deleteConstantPath();
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
            newDots[i].parent = constantPath.transform;
        }
    }
    private void RemovePathPoints(LineRenderer path, int pointsToRemove)
    {
        Vector3[] positions = new Vector3[path.positionCount];
        Vector3[] newPositions = new Vector3[path.positionCount - pointsToRemove];
        path.GetPositions(positions);
        for (int i = 0; i < path.positionCount - pointsToRemove; i++)
        {
            newPositions[i] = positions[i + pointsToRemove];
        }
        path.SetPositions(newPositions);
        Transform[] dots = path.GetComponentsInChildren<Transform>();
        if (dots.Length > 1)
        {
            Destroy(dots[1].gameObject);
        }
    }

    public void SetPath()
    {
        isPathSet = true;
        savedPath = currentPath;
        currentPath = null;
        deleteConstantPath();
        ClonePath();
        DeleteTemporaryPath();
        NextMovement(); //moves first step
    }

   

    public void deleteConstantPath()
    {
        foreach (Transform child in constantPath.transform)
        {
            Destroy(child.gameObject);
        }
        constantPath.positionCount = 0;
    }

    public void DeleteTemporaryPath()
    {
        foreach (Transform child in temporaryPath.transform)
        {
            Destroy(child.gameObject);
        }
        temporaryPath.positionCount = 0;
    }
    public void UnSelectUnitWithOutPath()
    {
        currentPath = null;
        isSelected = false;
        changeSpriteAlpha(1f);
    }
    public void UnSelectUnitWithPath()
    {
        isSelected = false;
        changeSpriteAlpha(1f);
    }
    void ClonePath()
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
}
