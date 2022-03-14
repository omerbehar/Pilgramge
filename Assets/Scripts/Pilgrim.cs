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
    }

    

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }

    public void PopulateMenu() //ask lior...
    {
        menuOption[0] = Instantiate(buildCityPrefab, menu.transform);
        menuOption[1] = Instantiate(buildCityPrefab, menu.transform);
        menuOption[2] = Instantiate(buildCityPrefab, menu.transform);
        menuOption[3] = Instantiate(buildCityPrefab, menu.transform);
        menuOption[4] = Instantiate(buildCityPrefab, menu.transform);
        menuOption[5] = Instantiate(buildCityPrefab, menu.transform);
    }
    public void BuildCity()
    {
        GameObject city = Instantiate(cityPrefab);
        city.transform.position = transform.position;
        city.GetComponent<City>().tileX = GetComponent<Pilgrim>().tileX;
        city.GetComponent<City>().tileY = GetComponent<Pilgrim>().tileY;
        Destroy(this);
    }
}
