using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pilgrim : MovingUnit
{
    GameObject buildCityPrefab;
    GameObject cityPrefab;
    // Start is called before the first frame update
    void Start()
    {
        buildCityPrefab = Resources.Load<GameObject>("Prefabs/buildCityPrefab");
        cityPrefab = Resources.Load<GameObject>("Prefabs/cityPrefab");
        base.Start();
        movementPerTurn = 1;
        movementLeftThisTurn = movementPerTurn;
    }

    

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }

    public override void  PopulateMenu() //sets the menu options
    {
        
        menuOptionsPrefabs[0] = Instantiate(buildCityPrefab, menuCirclePrefab.transform);
        menuOptionsPrefabs[1] = Instantiate(buildCityPrefab, menuCirclePrefab.transform);
        menuOptionsPrefabs[2] = Instantiate(buildCityPrefab, menuCirclePrefab.transform);
        menuOptionsPrefabs[3] = Instantiate(buildCityPrefab, menuCirclePrefab.transform);
        menuOptionsPrefabs[4] = Instantiate(buildCityPrefab, menuCirclePrefab.transform);
        menuOptionsPrefabs[5] = Instantiate(buildCityPrefab, menuCirclePrefab.transform);
        base.PopulateMenu();
    }
    public void BuildCity()
    {
        GameObject city = Instantiate(cityPrefab);
        city.transform.position = transform.position;
        city.GetComponent<City>().SetTilePosotion (GetComponent<Pilgrim>().tileX, GetComponent<Pilgrim>().tileY);
        Destroy(this);
    }
}
