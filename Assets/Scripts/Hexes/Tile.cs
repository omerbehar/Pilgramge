using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : Hex
{
    static public TileType[] tileTypes;
    private int tileTypeID;
    public List<Tile> neighbors = new List<Tile>();//the tile directly connected to this node

    void Start()
    {
        base.Start();
        SetTileTypesList();

    }

    private static void SetTileTypesList()
    {
        if (tileTypes != null)
        {
            tileTypes = Resources.LoadAll<TileType>("TileTypes");
        }
    }

    public float DistanceTo(Tile t)//returns virtual distance to tile on grid
    {
        return Vector2.Distance(new Vector2(tileX, tileY), new Vector2(t.tileX, t.tileY));
    }
    //getters and setters
    public void SetTileType(int type)
    {
        tileTypeID = type;
        GetComponent<SpriteRenderer>().sprite = GetTileTypeByID(tileTypeID).tileImage;
    }
    public TileType GetTileType()
    {
        return GetTileTypeByID(tileTypeID);
    }
    public void SetTileInfo(int column, int row, Transform parent)//set the tile parent, position and name
    {
        transform.parent = parent;
        name = "Hex_" + column.ToString() + "_" + row.ToString();
        SetTilePosotion(column, row);
    }
    public static TileType GetTileTypeByID( int typeID)
    {
        foreach (TileType type in tileTypes)
        {
            if (type.ID == typeID)
                return type;
        }
        return null;
    }
}
