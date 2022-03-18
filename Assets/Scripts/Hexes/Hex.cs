using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Hex : MonoBehaviour
{
    //references
    protected MapCreator _map;
    protected GameObject menuPrefab;

    //position
    [SerializeField] protected int tileX;
    [SerializeField] protected int tileY;
    
    

    //state
    protected bool isSelected;
    protected bool isHovered;

    protected void Start()
    {
        InitializeReferences();
    }

    private void InitializeReferences()
    {
        menuPrefab = Resources.Load<GameObject>("Prefabs/MenuPrefab");
        _map = FindObjectOfType<MapCreator>();
    }

    protected void Update()
    {
        
    }

   
    public void SelectHex()//mark selected and change alpha
    {
        isSelected = true;
        changeSpriteAlpha(0.2f);
    }
    public void UnSelect()//unselects a hex
    {
        isSelected = false;
        changeSpriteAlpha(1f);
    }
    public void HoverHex()
    {
        isHovered = true;
        changeSpriteAlpha(0.5f);
    }
    public void UnHoverHex()
    {
        isHovered = false;
        changeSpriteAlpha(1f);
    }
    protected void changeSpriteAlpha(float alpha)//change alpha
    {
        Color color = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, alpha);
    }
   //geters and setters
    public void SetTilePosotion(int tileX,int tileY)
    {
        this.tileX = tileX;
        this.tileY = tileY;
    }
    public Vector2 GetTilePosition()
    {
        return new Vector2(tileX, tileY);
    }
}
