using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class GeneralUnit : Hex
{
    //menu
    protected GameObject[] menuOptionsPrefabs = new GameObject[6];//menu icons prefabs
    protected GameObject menuCirclePrefab;
    protected bool isMenuOpen;
    public void OpenMenu()// opens the menu
    {
        isMenuOpen = true;
        menuCirclePrefab = Instantiate(menuPrefab,transform.position,Quaternion.identity);
        PopulateMenu();
        
    }

    public virtual void PopulateMenu()//places menu options in place
    {
        if (menuOptionsPrefabs[0])
        {
            menuOptionsPrefabs[0].transform.localPosition = new Vector3(0.33f, 0, 0);
        }
        if (menuOptionsPrefabs[1])
        {
            menuOptionsPrefabs[1].transform.localPosition = new Vector3(0.165f, -0.3f, 0);
        }
        if (menuOptionsPrefabs[2])
        {
            menuOptionsPrefabs[2].transform.localPosition = new Vector3(-0.165f, -0.3f, 0);
        }
        if (menuOptionsPrefabs[3])
        {
            menuOptionsPrefabs[3].transform.localPosition = new Vector3(-0.33f, 0, 0);
        }
        if (menuOptionsPrefabs[4])
        {
            menuOptionsPrefabs[4].transform.localPosition = new Vector3(-0.165f, 0.3f, 0);
        }
        if (menuOptionsPrefabs[5])
        {
            menuOptionsPrefabs[5].transform.localPosition = new Vector3(0.165f, 0.3f, 0);
        }
    }

    public void CloseMenu()
    {
        isMenuOpen = false;
        isSelected = false;
        UnSelect();
        Destroy(menuCirclePrefab);
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


    //getters and setters
    public bool IsMenuOpen()
    {
        return isMenuOpen;
    }
}
