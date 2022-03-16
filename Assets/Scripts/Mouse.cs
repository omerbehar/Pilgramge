using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Linq;
using UnityEngine.UI;

public class Mouse : MonoBehaviour
{
    //float dist;
    //Collider2D lastHoveredUnit;
    //int button;
    //RaycastHit2D hit;
    //bool dragging;

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
    }

    void Update()
    {
        CheckHoverAndCLick();
        DragMap();
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
                    MouseClickedHex(frontHoveredCollider);
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
            if (lastHoveredHex) //check if a hex is already hovered
            {
                if (((clickedTile && clickedTile != lastHoveredHex) || !clickedTile) && 
                    ((clickedUnit && clickedUnit != lastHoveredHex) || !clickedUnit))//check if previous hovered hex is not clicked hex
                {
                    lastHoveredHex.GetComponent<Hex>().UnHoverUnit();
                }
            }
            if (clickedUnit && hoveredCollider.transform.tag != "unit") //hovering over a hex while unit clicked - creating a temporary path
            {
                if (!clickedUnit.GetComponent<Hex>().isMenuOpen) //if menu is closed
                {
                    map.GetComponent<MapCreator>().GeneratePathTo(hoveredCollider.GetComponent<Tile>().tileX, hoveredCollider.GetComponent<Tile>().tileY, clickedUnit.gameObject);
                    clickedUnit.GetComponent<MovingUnit>().createTemporaryPath();
                }
            }
            else
            {
                hoveredCollider.GetComponent<Hex>().HoverUnit(); ;
                lastHoveredHex = hoveredCollider;
            }
        }
        if (clickedUnit == hoveredCollider)  // hovering over the clicked unit causes the temporary path to dissapear
        {
            hoveredCollider.GetComponent<MovingUnit>().DeleteTemporaryPath();
        }
    }
    void MouseClickedHex(Collider2D collider2D)
    {
        if (!clickedTile && !clickedUnit) //nothing is clicked
        {
            clickedTile = collider2D;
            collider2D.GetComponent<Hex>().SelectUnit();
        }
        else if (clickedTile && clickedTile == collider2D) //unclicking a hex
        {
            clickedTile = null;
            collider2D.GetComponent<Hex>().UnSelectUnit();
        }
        else if (clickedUnit) //setting a path
        {
            clickedUnit.GetComponent<MovingUnit>().SetPath();
            lastHoveredHex = collider2D;
            collider2D.GetComponent<Hex>().UnHoverUnit();

            clickedUnit.GetComponent<MovingUnit>().UnSelectUnitWithPath();
            clickedUnit = null;
        }
        else if (clickedTile && clickedTile != collider2D) // clicking a different hex while a hex is clicked
        {
            clickedTile.GetComponent<Hex>().UnSelectUnit();
            collider2D.GetComponent<Hex>().SelectUnit();
            clickedTile = collider2D;
        }
    }
    private void ReleaseSelection()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (clickedTile)
            {
                clickedTile.GetComponent<Hex>().UnSelectUnit();
                clickedTile = null;
                //isHexClicked = false;
            }
            if (clickedUnit)
            {
                clickedUnit.GetComponent<Hex>().UnSelectUnit();
                clickedUnit = null;
                //isUnitClicked = false;
            }
        }
    }

    

    
    
 
    private void SetPathTrue()
    {
        clickedUnit.GetComponent<MovingUnit>().isPathSet = true;
        print(clickedUnit.name + " + " + clickedUnit.GetComponent<MovingUnit>().isPathSet);
    }
    private IEnumerator SetPathFalse()
    {
        clickedUnit.GetComponent<MovingUnit>().isPathSet = false;
        print(clickedUnit.name + " _ " + clickedUnit.GetComponent<MovingUnit>().isPathSet);
        yield return new WaitForSeconds(0.1f);
    }

    void MouseClickedUnit(Collider2D collider2D)
    {
        if (!clickedUnit) //no clicked unit - select unit
        {
            if (clickedTile) //there is a clicked hex - unclick it
            {
                clickedTile.GetComponent<Hex>().UnSelectUnit();
                clickedTile = null;
                //isHexClicked = false; //redundant
            }
            print("test");
            collider2D.GetComponent<Hex>().SelectUnit();
            //isUnitClicked = true; //redundent...
            clickedUnit = collider2D;
        } 
        else if (collider2D == clickedUnit && !collider2D.GetComponent<Hex>().isMenuOpen) //clicked same unit while menu is closed - open menu
        {
            if (clickedUnit.GetComponent<MovingUnit>().isPathSet)
            {
                clickedUnit.GetComponent<MovingUnit>().UnSelectUnitWithPath();
            }
            else
            {
                clickedUnit.GetComponent<MovingUnit>().UnSelectUnitWithOutPath();
            }
            collider2D.GetComponent<Hex>().OpenMenu();
        }
        else if (collider2D == clickedUnit && collider2D.GetComponent<Hex>().isMenuOpen) //clicked on a unit with an open menu - unclick unit
        {
            collider2D.GetComponent<Hex>().CloseMenu();
            //isUnitClicked = false; //redundent...
            clickedUnit = null;
        }
        else if (collider2D != clickedUnit && !clickedUnit.GetComponent<Hex>().isMenuOpen) //clicked on a different unit while menu is closed
        {
            clickedUnit.GetComponent<Hex>().UnSelectUnit();
            collider2D.GetComponent<Hex>().SelectUnit();
            clickedUnit = collider2D;
        }
    }
    
    void DragMap()
    {
        //save mouse origin on drag
        if (Input.GetMouseButtonDown(1))
        {
            mouseDragStartPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        //calculate difference between current mouse position and origin while mouse is dragging
        if (Input.GetMouseButton(1))
        {
            mouseDragNewPosition = mouseDragStartPosition - Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Camera.main.transform.Translate(mouseDragNewPosition);
            Camera.main.transform.position = ClampCamera(Camera.main.transform.position);
        }
    }
    Vector3 ClampCamera(Vector3 targetPosition)
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
    void ZoomMap()
    {
        float newZoom = Camera.main.orthographicSize - Input.mouseScrollDelta.y * mapZoomMultiplier;
        float maxZoomOut = Mathf.Min(6f, MaxZoomFromMapSize());
        Camera.main.orthographicSize = Mathf.Clamp(newZoom, 3.8f, maxZoomOut);
        Camera.main.transform.position = ClampCamera(Camera.main.transform.position);
    }

    private float MaxZoomFromMapSize() //fix later
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

    public void nextMoveButtonClicked()
    {
        var allUnits = GameObject.FindObjectsOfType<MovingUnit>();
        foreach (var unit in allUnits)
        {
            if (unit.GetComponent<MovingUnit>().isMenuOpen)
            {
                unit.GetComponent<MovingUnit>().CloseMenu();
            }
            clickedUnit = null;
            unit.GetComponent<MovingUnit>().DeleteTemporaryPath();
            unit.GetComponent<MovingUnit>().NextMovement();
            unit.GetComponent<MovingUnit>().ResetMovementLeft();
        }
    }
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
}

