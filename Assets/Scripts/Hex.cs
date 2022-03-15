using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Hex : MonoBehaviour
{
    
    public GameObject map;

    public int tileX;
    public int tileY;
    protected GameObject menu;
    protected GameObject[] menuOption = new GameObject[6];
    GameObject menuPrefab;
    public bool isSelected;
    public bool isMenuOpen;
    public bool isHovered;

    // Start is called before the first frame update
    protected void Start()
    {
        menuPrefab = Resources.Load<GameObject>("Prefabs/MenuPrefab");
    }

    // Update is called once per frame
    protected void Update()
    {
        
    }

    public void OpenMenu()
    {
        isMenuOpen = true;
        menu = Instantiate(menuPrefab);
        menu.transform.position = transform.localPosition;
        GetComponent<Pilgrim>().PopulateMenu(); //to specific....
        if (menuOption[0])
        {
            menuOption[0].transform.localPosition = new Vector3(0.33f, 0, 0);
        }
        if (menuOption[1])
        {
            menuOption[1].transform.localPosition = new Vector3(0.165f, -0.3f, 0);
        }
        if (menuOption[2])
        {
            menuOption[2].transform.localPosition = new Vector3(-0.165f, -0.3f, 0);
        }
        if (menuOption[3])
        {
            menuOption[3].transform.localPosition = new Vector3(-0.33f, 0, 0);
        }
        if (menuOption[4])
        {
            menuOption[4].transform.localPosition = new Vector3(-0.165f, 0.3f, 0);
        }
        if (menuOption[5])
        {
            menuOption[5].transform.localPosition = new Vector3(0.165f, 0.3f, 0);
        }
    }
    public void CloseMenu()
    {
        isMenuOpen = false;
        isSelected = false;
        UnSelectUnit();
        Destroy(menu);
    }

    public void MenuOptionClicked(string name)
    {
        switch (name)
        {
            case ("buildCity"):
                GetComponent<Pilgrim>().BuildCity();
                break;
        }
    }

    public void SelectUnit()
    {
        isSelected = true;
        changeSpriteAlpha(0.2f);
    }
    public void UnSelectUnit()
    {
        if (GetComponent<MovingUnit>() && !GetComponent<MovingUnit>().isPathSet)
        {
            GetComponent<MovingUnit>().DeleteTemporaryPath();
            GetComponent<MovingUnit>().currentPath = null; //sure???
        } 
        else if(GetComponent<MovingUnit>() && GetComponent<MovingUnit>().isPathSet)
        {
            print("test");
            GetComponent<MovingUnit>().DeleteTemporaryPath();
        }
        isSelected = false;
        changeSpriteAlpha(1f);
    }
    public void HoverUnit()
    {
        isHovered = true;
        changeSpriteAlpha(0.5f);
    }
    public void UnHoverUnit()
    {
        isHovered = false;
        changeSpriteAlpha(1f);
    }
    protected void changeSpriteAlpha(float alpha)
    {
        Color color = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, alpha);
    }
}
