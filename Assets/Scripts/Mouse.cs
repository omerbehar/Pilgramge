using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Linq;
using UnityEngine.UI;

public class Mouse : MonoBehaviour
{
    RaycastHit2D hit;
    Ray ray;
    Collider2D lastHoveredHex;
    Collider2D lastHoveredUnit;
    int button;
    Collider2D clickedHex;
    Collider2D clickedUnit;
    //bool isHexClicked = false;
    //bool isUnitClicked = false;
    float dist;
    Vector3 mouseStart;
    Vector3 mouseMove;
    bool dragging;
    public float scale = 0.2f;
    Collider2D call2D;
    public GameObject map;
    private bool isButtonHovered = false;
    RaycastHit2D[] raycastHit2D;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        HoverAndCLick();
        DragMap();
        ZoomMap();
        ReleaseSelection();
    }

    private void ReleaseSelection()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (clickedHex)
            {
                clickedHex.GetComponent<HexUnit>().UnSelectUnit();
                clickedHex = null;
                //isHexClicked = false;
            }
            if (clickedUnit)
            {
                clickedUnit.GetComponent<HexUnit>().UnSelectUnit();
                clickedUnit = null;
                //isUnitClicked = false;
            }
        }
    }

    private void HoverAndCLick()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        raycastHit2D = Physics2D.RaycastAll(ray.origin, ray.direction, Mathf.Infinity);

        var allButtons = FindObjectsOfType<Button>();

        var allButtonTransforms = allButtons.Select(button => button.GetComponent<RectTransform>()).ToArray();
        var mouse2D = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        if (raycastHit2D.Length > 0)
        {
            call2D = raycastHit2D[0].collider;
        }
        if (call2D && call2D.GetComponent<Hex>() != null && !isButtonHovered)
        {
            MouseOver_Hex(call2D);
            if (Input.GetMouseButtonDown(0))
            {
                MouseClickedHex(call2D);
            }
        }
        if (call2D && call2D.GetComponent<MovingUnit>() != null && !isButtonHovered)
        {
            MouseOver_Hex(call2D);
            if (Input.GetMouseButtonDown(0))
            {
                MouseClickedUnit(call2D);
            }
        }
    }

    //void changeSpriteAlpha(Collider2D collider2D, float alpha)
    //{
    //    Color color = collider2D.GetComponent<SpriteRenderer>().color;
    //    collider2D.GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, alpha);
    //}
    
    void MouseOver_Hex(Collider2D collider2D)
    {
       
        if ((!clickedHex && !clickedUnit) ||
            (clickedHex && collider2D.name != clickedHex.name) ||
            (clickedUnit && collider2D.name != clickedUnit.name)) //hex is clicked but we are pointing at a different hex
        {
            if (lastHoveredHex) //change previously hovered hex back to unhovered
            {
                if (((clickedHex && clickedHex != lastHoveredHex)||!clickedHex) && ((clickedUnit && clickedUnit != lastHoveredHex) || !clickedUnit))
                {
                    lastHoveredHex.GetComponent<HexUnit>().UnHoverUnit();
                }
            }
            if (clickedUnit && collider2D.transform.tag != "unit") //hovering over a hex while unit clicked - creating a temporary path
            {
                if (!clickedUnit.GetComponent<HexUnit>().isMenuOpen) //if menu is closed
                {
                    map.GetComponent<MapCreator>().GeneratePathTo(collider2D.GetComponent<Hex>().tileX, collider2D.GetComponent<Hex>().tileY, clickedUnit.gameObject);
                    clickedUnit.GetComponent<MovingUnit>().createTemporaryPath();
                }
                //else //there is a path set
                //{
                //    map.GetComponent<MapCreator>().GeneratePathTo(collider2D.GetComponent<Hex>().tileX, collider2D.GetComponent<Hex>().tileY, clickedUnit.gameObject);
                //    clickedUnit.GetComponent<MovingUnit>().createTemporaryPath();
                //}
            } 
            else
            {
                collider2D.GetComponent<HexUnit>().HoverUnit();;
                lastHoveredHex = collider2D;
            }
        }
        if (clickedUnit == collider2D)  // hovering over the clicked unit causes the temporary path to dissapear
        {
            collider2D.GetComponent<MovingUnit>().DeleteTemporaryPath();
        }
    }
    void MouseClickedHex(Collider2D collider2D)
    {
        if (!clickedHex && !clickedUnit) //nothing is clicked
        {
            //isHexClicked = true; //redundant
            clickedHex = collider2D;
            collider2D.GetComponent<HexUnit>().SelectUnit();
        }
        else if (clickedHex && clickedHex == collider2D) //unclicking a hex
        {
            //isHexClicked = false; //redundant
            clickedHex = null;
            collider2D.GetComponent<HexUnit>().UnSelectUnit();
        }
        else if (clickedUnit) //setting a path
        {
            //clickedUnit.GetComponent<MovingUnit>().isPathSet = true;
            //clickedUnit.GetComponent<MovingUnit>().deleteTemporaryPath();
            //map.GetComponent<MapCreator>().GeneratePathTo(collider2D.GetComponent<HexUnit>().tileX, collider2D.GetComponent<HexUnit>().tileY, clickedUnit.gameObject);
            clickedUnit.GetComponent<MovingUnit>().SetPath();
            lastHoveredHex = collider2D;
            collider2D.GetComponent<HexUnit>().UnHoverUnit();
            clickedUnit.GetComponent<MovingUnit>().NextMovement(); //moves first step
            clickedUnit.GetComponent<MovingUnit>().UnSelectUnitWithPath();
            clickedUnit = null;
            //isUnitClicked = false; //redundant
        }
        else if (clickedHex && clickedHex != collider2D) // clicking a different hex while a hex is clicked
        {
            clickedHex.GetComponent<HexUnit>().UnSelectUnit();
            collider2D.GetComponent<HexUnit>().SelectUnit();
            clickedHex = collider2D;
        }
    }
    
    //void MouseClickedHex(Collider2D collider2D)
    //{
    //    //var collidersName = raycastHit2D.Select(collider => collider.collider.name).ToArray();
    //    //Collider2D name = raycastHit2D.Where(collider => collider.collider.name == "buildCity").;

    //    //int pos = Array.IndexOf(collidersName, value);

    //    //if (raycastHit2D.Contains(collider2D => collider2D.collider.name == "");
    //    //    {

    //    //}
    //    if (collider2D == clickedHex) //unclicking a hex
    //    {
    //        changeSpriteAlpha(collider2D, 1f);
    //        isHexClicked = false;
    //        clickedHex = null;
    //    }
    //    else if (isUnitClicked && !clickedUnit.GetComponent<MovingUnit>().isPathSet) //clicking a hex while a unit is clicked (sets unit's path)
    //    {
    //        clickedUnit.GetComponent<HexUnit>().CloseMenu();
    //        clickedUnit.GetComponent<MovingUnit>().isPathSet = true;
    //        clickedUnit.GetComponent<MovingUnit>().deleteTemporaryPath();
    //        map.GetComponent<MapCreator>().GeneratePathTo(collider2D.GetComponent<Hex>().tileX, collider2D.GetComponent<Hex>().tileY, clickedUnit.gameObject);
    //        clickedUnit.GetComponent<MovingUnit>().destination = collider2D.transform.position;
    //        lastHoveredHex = collider2D;
    //        changeSpriteAlpha(collider2D, 1f);
    //        changeSpriteAlpha(clickedUnit, 1f);
    //        clickedUnit.GetComponent<MovingUnit>().NextMovement(); //moves first step
    //        clickedUnit = null;
    //        isUnitClicked = false;
    //    }
    //    //else if (clickedUnit.GetComponent<PlayerControl>().isPathSet) //there is already a path set 
    //    //{
    //    //    clickedUnit.GetComponent<PlayerControl>().deleteConstantPath();
    //    //    clickedUnit.GetComponent<PlayerControl>().isPathSet = false;
    //    //    print(clickedUnit.GetComponent<PlayerControl>().isPathSet);
    //    //    clickedUnit.GetComponent<PlayerControl>().deleteTemporaryPath();
    //    //    map.GetComponent<MapCreator>().GeneratePathTo(collider2D.GetComponent<Hex>().tileX, collider2D.GetComponent<Hex>().tileY, clickedUnit.gameObject);
    //    //    clickedUnit.GetComponent<PlayerControl>().destination = collider2D.transform.position;
    //    //    lastHoveredHex = collider2D;
    //    //    changeSpriteAlpha(collider2D, 1f);
    //    //    Invoke("SetPathTrue", 0.1f); //inorder to make sure everything happens before the isPathSet changes to true
    //    //}
    //    else
    //    {
    //        if (clickedHex)
    //        {
    //            changeSpriteAlpha(clickedHex, 1f);
    //        }
    //        changeSpriteAlpha(collider2D, 0.2f);
    //        isHexClicked = true;
    //        clickedHex = collider2D;
    //    }
        
    //}
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
            if (clickedHex) //there is a clicked hex - unclick it
            {
                clickedHex.GetComponent<HexUnit>().UnSelectUnit();
                clickedHex = null;
                //isHexClicked = false; //redundant
            }
            print("test");
            collider2D.GetComponent<HexUnit>().SelectUnit();
            //isUnitClicked = true; //redundent...
            clickedUnit = collider2D;
        } 
        else if (collider2D == clickedUnit && !collider2D.GetComponent<HexUnit>().isMenuOpen) //clicked same unit while menu is closed - open menu
        {
            if (clickedUnit.GetComponent<MovingUnit>().isPathSet)
            {
                clickedUnit.GetComponent<MovingUnit>().UnSelectUnitWithPath();
            }
            else
            {
                clickedUnit.GetComponent<MovingUnit>().UnSelectUnitWithOutPath();
            }
            collider2D.GetComponent<HexUnit>().OpenMenu();
        }
        else if (collider2D == clickedUnit && collider2D.GetComponent<HexUnit>().isMenuOpen) //clicked on a unit with an open menu - unclick unit
        {
            collider2D.GetComponent<HexUnit>().CloseMenu();
            //isUnitClicked = false; //redundent...
            clickedUnit = null;
        }
        else if (collider2D != clickedUnit && !clickedUnit.GetComponent<HexUnit>().isMenuOpen) //clicked on a different unit while menu is closed
        {
            clickedUnit.GetComponent<HexUnit>().UnSelectUnit();
            collider2D.GetComponent<HexUnit>().SelectUnit();
            clickedUnit = collider2D;
        }
    }
    //void MouseClickedUnit(Collider2D collider2D)
    //{
    //    if (collider2D.GetComponent<HexUnit>().isSelected && !collider2D.GetComponent<HexUnit>().isMenuOpen) //second click - open menu and close path
    //    {
    //        collider2D.GetComponent<HexUnit>().OpenMenu();
    //        if (!clickedUnit.GetComponent<MovingUnit>().isPathSet)
    //        {
    //            clickedUnit.GetComponent<MovingUnit>().deleteTemporaryPath();
    //            clickedUnit.GetComponent<MovingUnit>().Invoke("deleteConstantPath", 0.1f); //not working???
    //        }
    //        else
    //        {
    //            clickedUnit.GetComponent<MovingUnit>().deleteTemporaryPath();
    //        }
    //        isUnitClicked = false;
    //        clickedUnit = null;
    //    }
    //    else if (collider2D.GetComponent<HexUnit>().isSelected && collider2D.GetComponent<HexUnit>().isMenuOpen) //unclicking a unit
    //    {
    //        clickedUnit.GetComponent<HexUnit>().CloseMenu();
    //        changeSpriteAlpha(collider2D, 1f);
    //        isUnitClicked = false;
    //        clickedUnit = null;           
    //    }
    //    else if (!clickedUnit) //there is no clicked unit
    //    {
    //        collider2D.GetComponent<HexUnit>().isSelected = true;
    //        if (!clickedHex) //there is also no clicked hex
    //        {
    //            changeSpriteAlpha(collider2D, 0.2f);
    //            isUnitClicked = true;
    //            clickedUnit = collider2D;
    //            if (clickedUnit.GetComponent<MovingUnit>().isPathSet)
    //            {
    //                //clickedUnit.GetComponent<PlayerControl>().deleteTemporaryPath();
    //                clickedUnit.GetComponent<MovingUnit>().deleteConstantPath();
    //                clickedUnit.GetComponent<MovingUnit>().isPathSet = false;
    //            }
    //        }
    //        else //there is a clicked hex
    //        {
    //            changeSpriteAlpha(collider2D, 0.2f);
    //            isUnitClicked = true;
    //            clickedUnit = collider2D;
    //            isHexClicked = false;
    //            changeSpriteAlpha(clickedHex, 1f);
    //            clickedHex = null;
    //        }
    //    } 
    //    else //clicked unit is different (change clicked unit)
    //    {
    //        //clickedUnit.GetComponent<HexUnit>().CloseMenu();
    //        //collider2D.GetComponent<HexUnit>().OpenMenu();
    //        changeSpriteAlpha(clickedUnit, 1f);
    //        clickedUnit = collider2D;
    //        changeSpriteAlpha(clickedUnit, 0.2f);
    //    }
    //}    
    void DragMap()
    {
        //save mouse origin on drag
        if (Input.GetMouseButtonDown(1))
        {
            mouseStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        //calculate difference between current mouse position and origin while mouse is dragging
        if (Input.GetMouseButton(1))
        {
            mouseMove = mouseStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Camera.main.transform.Translate(mouseMove);
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
        float newZoom = Camera.main.orthographicSize - Input.mouseScrollDelta.y * scale;
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
        var allUnits = GameObject.FindObjectsOfType<MovingUnit>(); // GameObject.FindGameObjectsWithTag("unit");
        foreach (var unit in allUnits)
        {
            if (unit.GetComponent<MovingUnit>().isMenuOpen)
            {
                unit.GetComponent<MovingUnit>().CloseMenu();
            }
            unit.GetComponent<MovingUnit>().NextMovement();
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

