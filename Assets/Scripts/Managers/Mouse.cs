using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Linq;
using UnityEngine.UI;

public class Mouse : MonoBehaviour
{

    //raycast
    private Ray mouseLocateRay;//used to locate mouse on screen
    private RaycastHit2D[] mouseHoveredRayCastResaults;

    //drag
    private Vector3 mouseDragNewPosition;
    private Vector3 mouseDragStartPosition;

    //marked colliders
    private Collider2D frontHoveredCollider;//collider that is below the mouse
    private Collider2D lastHoveredHex;
    private Collider2D clickedTile;
    private Collider2D clickedUnit;

    //references
    private TagMenager _tagMenager;
    private MapCreator map;
    [Space(2)]
    //parameters
    [Header("Parameter")]
    [SerializeField] private float mapZoomMultiplier = 0.2f;


    //button -- to move
    private bool isButtonHovered = false;//marked if button hovered - used to not hover below objects
    private int isNextTurnActive = 0;
    public Button nextTurnButton;

    void Start()
    {
        InitializeReferences();
    }

    private void InitializeReferences()
    {
        map = FindObjectOfType<MapCreator>();//get the map from the game
        _tagMenager = FindObjectOfType<TagMenager>();
    }

    void Update()
    {
        CheckHoverAndCLick();
        PanMap();
        ZoomMap();
        ReleaseSelection();
    }

    private void CheckHoverAndCLick()//checks if mouse is hovering over anything and triggers click input
    {
        mouseLocateRay = Camera.main.ScreenPointToRay(Input.mousePosition);//create ray using mouse position
        mouseHoveredRayCastResaults = Physics2D.RaycastAll
            (mouseLocateRay.origin, mouseLocateRay.direction, Mathf.Infinity);//gets collided things using mouse ray

        //Button[] allButtons = FindObjectsOfType<Button>();//gets buttons to prevent clicking tiles below
        //var allButtonTransforms = allButtons.Select(button => button.GetComponent<RectTransform>()).ToArray();
        //var mouse2D = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        if (mouseHoveredRayCastResaults.Length > 0)//check if something is hovered
        {
            // TODO: need to check if nost front collider is 0 in array
            frontHoveredCollider = mouseHoveredRayCastResaults[0].collider;//set hovered collider
        }
        if (frontHoveredCollider && !isButtonHovered)//check if hovering over collider and not over button
        {
            MouseOverHex(frontHoveredCollider);
            //TODO: move to input manager
            if (Input.GetMouseButtonDown(0))//check if left click
            {
                if(frontHoveredCollider.GetComponent<Tile>() != null)//check if hex clicked
                    MouseClickedTile(frontHoveredCollider);
                else if(frontHoveredCollider.GetComponent<MovingUnit>() != null)//check if moving unit clicked
                {
                    MouseClickedUnit(frontHoveredCollider);
                }
            }
        }
    }
    void MouseOverHex(Collider2D hoveredCollider)//functionallity of Hovering over Hex
    {

        if ((!clickedTile && !clickedUnit) ||
            (clickedTile && hoveredCollider.name != clickedTile.name) ||
            (clickedUnit && hoveredCollider.name != clickedUnit.name)) //check if hovering over not clicked hex
        {
            if (lastHoveredHex) //check if a hex is already hovered - to unhover
            {
                if (((clickedTile && clickedTile != lastHoveredHex) || !clickedTile) && 
                    ((clickedUnit && clickedUnit != lastHoveredHex) || !clickedUnit))//check if previous hovered hex is not clicked hex
                {
                    lastHoveredHex.GetComponent<Hex>().UnHoverHex();
                }
            }
            if (clickedUnit && clickedUnit.GetComponent<MovingUnit>() && hoveredCollider.transform.tag != _tagMenager.UNIT_TAG) //hovering over a hex while unit clicked - creating a temporary path
            {
                if (!clickedUnit.GetComponent<GeneralUnit>().IsMenuOpen()) //if menu is closed - print temp path
                {
                    map.GetComponent<MapCreator>().GeneratePathTo(hoveredCollider.GetComponent<Tile>(), clickedUnit.GetComponent<GeneralUnit>());
                    clickedUnit.GetComponent<MovingUnit>().createTemporaryPath();
                }
            }
            else//if not print path then mark hoveret hex
            {
                hoveredCollider.GetComponent<Hex>().HoverHex(); ;
                lastHoveredHex = hoveredCollider;
            }
        }
        if (clickedUnit == hoveredCollider)  // hovering over the clicked unit causes the temporary path to dissapear
        {
            hoveredCollider.GetComponent<MovingUnit>().DeleteTemporaryPath();
        }
    }
    void MouseClickedTile(Collider2D clickedTileCollider)//function of clicking tile
    {
        if (!clickedTile && !clickedUnit) //nothing is clicked - set clicked tile
        {
            clickedTile = clickedTileCollider;
            clickedTileCollider.GetComponent<Hex>().SelectHex();
        }
        else if (clickedTile && clickedTile == clickedTileCollider) //unclicking a tile
        {
            clickedTile = null;
            clickedTileCollider.GetComponent<Hex>().UnSelect();
        }
        else if (clickedUnit) //setting a path
        {
            clickedUnit.GetComponent<MovingUnit>().SetPath();//set temp path to constant path
            clickedUnit.GetComponent<MovingUnit>().UnSelect();
            clickedUnit = null;
        }
        else if (clickedTile && clickedTile != clickedTileCollider) // clicking a different tile while a tile is clicked
        {
            clickedTile.GetComponent<Hex>().UnSelect();
            clickedTileCollider.GetComponent<Hex>().SelectHex();
            clickedTile = clickedTileCollider;
        }
    }
    void MouseClickedUnit(Collider2D clickedUnitCollider)//function of clicking a unit
    {
        if (!clickedUnit) //no clicked unit - select unit
        {
            if (clickedTile) //there is a clicked tile - unclick it
            {
                clickedTile.GetComponent<Hex>().UnSelect();
                clickedTile = null;
            }
            clickedUnitCollider.GetComponent<Hex>().SelectHex();
            clickedUnit = clickedUnitCollider;
        }
        else if (clickedUnitCollider == clickedUnit) //clicked same unit 
        {
            if (!clickedUnitCollider.GetComponent<GeneralUnit>().IsMenuOpen()) //while menu is closed - open menu
            {
                clickedUnit.GetComponent<MovingUnit>().UnSelect();
                clickedUnitCollider.GetComponent<GeneralUnit>().OpenMenu();
            }
            else//while menu is open - close menu
            {
                clickedUnitCollider.GetComponent<GeneralUnit>().CloseMenu();
                clickedUnit = null;
            }
        }
        else if (clickedUnitCollider != clickedUnit && !clickedUnit.GetComponent<GeneralUnit>().IsMenuOpen()) //clicked on a different unit and not menu is open - click new unit
        {
            clickedUnit.GetComponent<MovingUnit>().UnSelect();
            clickedUnitCollider.GetComponent<Hex>().SelectHex();
            clickedUnit = clickedUnitCollider;
        }
    }


    //private void SetPathTrue()
    //{
    //    clickedUnit.GetComponent<MovingUnit>().isPathSet = true;
    //    print(clickedUnit.name + " + " + clickedUnit.GetComponent<MovingUnit>().isPathSet);
    //}
    //private IEnumerator SetPathFalse()
    //{
    //    clickedUnit.GetComponent<MovingUnit>().isPathSet = false;
    //    print(clickedUnit.name + " _ " + clickedUnit.GetComponent<MovingUnit>().isPathSet);
    //    yield return new WaitForSeconds(0.1f);
    //}

    
    private void PanMap()
    {

        if (Input.GetMouseButtonDown(1))//if right click mouse - save mouse origin position
        {
            mouseDragStartPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        //calculate difference between current mouse position and origin while mouse is dragging
        if (Input.GetMouseButton(1))
        {
            mouseDragNewPosition = mouseDragStartPosition - Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Camera.main.transform.Translate(mouseDragNewPosition);//move camera
            Camera.main.transform.position = ClampCamera(Camera.main.transform.position);
        }
    }
    private Vector3 ClampCamera(Vector3 targetPosition)//check if new camera position is valid
    {
        float camHeight = Camera.main.orthographicSize;
        float camWidth = Camera.main.orthographicSize * Camera.main.aspect;
        GameObject lowerLeftTile = GameObject.Find("Hex_0_0");
        string uperRightTileName = "Hex_" + (MapCreator.mapWidth - 1).ToString() + "_" + (MapCreator.mapHeight - 1).ToString();
        GameObject uperRightTile = GameObject.Find(uperRightTileName);
        float minX = lowerLeftTile.transform.position.x - MapCreator.xOffSet / 2 + camWidth;
        float minY = lowerLeftTile.transform.position.y - MapCreator.xOffSet / Mathf.Sqrt(3)  + camHeight;
        float maxX = uperRightTile.transform.position.x + MapCreator.xOffSet / 2 - camWidth;
        float maxY = uperRightTile.transform.position.y + MapCreator.xOffSet / Mathf.Sqrt(3) - camHeight;
        float newX = Mathf.Clamp(targetPosition.x, minX, maxX);
        float newY = Mathf.Clamp(targetPosition.y, minY, maxY);
        return new Vector3(newX, newY, targetPosition.z);
    }
    private void ZoomMap()//zooms the map
    {
        float newZoom = Camera.main.orthographicSize - Input.mouseScrollDelta.y * mapZoomMultiplier;
        float maxZoomOut = Mathf.Min(6f, MaxZoomFromMapSize());
        Camera.main.orthographicSize = Mathf.Clamp(newZoom, 3.8f, maxZoomOut);
        Camera.main.transform.position = ClampCamera(Camera.main.transform.position);
    }

    private float MaxZoomFromMapSize() //calculate max zoom
    {
        if (MapCreator.mapHeight % 2 == 0)
        {
            float twoRowsHeight = MapCreator.yOffSet + 1.5f * MapCreator.xOffSet / Mathf.Sqrt(3);
            return (((MapCreator.mapHeight / 2) * twoRowsHeight) + MapCreator.xOffSet * Mathf.Sqrt(1f / 12f)) / 2; //max camera size (ortho map size is half world measures...)
        } 
        else
        {
            float twoRowsHeight = MapCreator.yOffSet + 1.5f * MapCreator.xOffSet / Mathf.Sqrt(3);
            return (((MapCreator.mapHeight - 1) / 2) * twoRowsHeight + MapCreator.yOffSet + MapCreator.xOffSet * Mathf.Sqrt(1f / 12f)) / 2;
        }
    }
    private void ReleaseSelection()//right click to unselect
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (clickedTile)
            {
                clickedTile.GetComponent<Hex>().UnSelect();
                clickedTile = null;
                //isHexClicked = false;
            }
            if (clickedUnit)
            {
                clickedUnit.GetComponent<Hex>().UnSelect();
                clickedUnit = null;
                //isUnitClicked = false;
            }
        }
    }

    //TODO: move to GUI manager
    public void UpdateIsNextTurnActive(int diff)
    {
        isNextTurnActive += diff;
        if (isNextTurnActive > 0)
        {
            nextTurnButton.interactable = false;
        } 
        else
        {
            nextTurnButton.interactable = true;
        }
            
    }
    public void OnMouseEnterButton()
    {
        isButtonHovered = true;
    }
    public void OnMouseExitButton()
    {
        isButtonHovered = false;
    }
    
    public void nextMoveButtonClicked()//click on next turn button
    {
        var allUnits = GameObject.FindObjectsOfType<MovingUnit>();
        foreach (var unit in allUnits)
        {
            if (unit.GetComponent<MovingUnit>().IsMenuOpen())
            {
                unit.GetComponent<MovingUnit>().CloseMenu();
            }
            clickedUnit = null;
            unit.GetComponent<MovingUnit>().DeleteTemporaryPath();
            unit.GetComponent<MovingUnit>().NextMovement();
            unit.GetComponent<MovingUnit>().ResetMovementLeft();
        }
    }
}

