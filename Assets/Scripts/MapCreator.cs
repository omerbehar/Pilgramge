using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapCreator : MonoBehaviour
{
    //static
    //static public Tile[,] tiles;//array used to set neighbours of each tile
    static public int mapWidth = 25;
    static public int mapHeight = 12; //minimum 9 rows for zoom to work correctly!!!
    static public float xOffSet = 1f;
    static public float yOffSet = 0.88f;

    //tiles
    [SerializeField] private TileType[] tileTypes;//tile types
    //private int[,] tiles;//map tiles - a grid, contains the id of a tile type
    private Tile[,] tiles;
    private Tile _tilePrefab;
    
    void Start()
    {
        _tilePrefab = Resources.Load<Tile>("Prefabs/land1");
        CreateHexTileMap();
        initializeTileTypes();
        GenerateNeighbors();
        
    }
    void CreateHexTileMap()//instantiate the tiles
    {
        tiles = new Tile[mapWidth, mapHeight];
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                tiles[x, y] = Instantiate(_tilePrefab);//instantiate new tile
                if (y % 2 == 0)//set position with offsets
                {
                    tiles[x, y].transform.position = new Vector3(x * xOffSet, y * yOffSet, 0);
                }
                else
                {
                    tiles[x, y].transform.position = new Vector3((x + 0.5f) * xOffSet, y * yOffSet, 0);
                }

                tiles[x, y].SetTileInfo(x, y, transform);
            }
        }
    }
    void initializeTileTypes()//manually set the tile grid with tile types
    {
        //tiles = new int[mapWidth, mapHeight];
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                tiles[x, y].SetTileType(0);
            }
        }
        //river
        tiles[4, 4].SetTileType(2);
        tiles[5, 4].SetTileType(2);
        tiles[6, 4].SetTileType(2);
        tiles[7, 4].SetTileType(2);
        tiles[8, 4].SetTileType(2);
        tiles[9, 4].SetTileType(2);
        tiles[9, 5].SetTileType(2);
        tiles[10, 5].SetTileType(2);
        tiles[10, 6].SetTileType(2);

        //mountains
        for (int x = 3; x < 5; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                tiles[x, y].SetTileType(1);
            }
        }
    }
    //void GeneratePathFindingGraph()
    //{
    //    //initialize the array
    //    tiles = new Tile[mapWidth, mapHeight];
    //    //initialize each node in the array
    //    for (int x = 0; x < mapWidth; x++)
    //    {
    //        for (int y = 0; y < mapHeight; y++)
    //        {
    //            tiles[x, y] = new Tile();
    //            tiles[x, y].x = x;
    //            tiles[x, y].y = y;
    //        }
    //    }
    //    //populate neigbours
    //    populateNeigbours();
    //}
    private void GenerateNeighbors()//set each tile's neighbors
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (y % 2 != 0)
                {
                    if (x < mapWidth - 1 && y > 0)
                    {
                        tiles[x, y].neighbors.Add(tiles[x + 1, y - 1]);
                    }
                    if (x < mapWidth - 1 && y < mapHeight - 1)
                    {
                        tiles[x, y].neighbors.Add(tiles[x + 1, y + 1]);
                    }
                }
                else
                {
                    if (x > 0 && y > 0)
                    {
                        tiles[x, y].neighbors.Add(tiles[x - 1, y - 1]);
                    }
                    if (x > 0 && y < mapHeight - 1)
                    {
                        tiles[x, y].neighbors.Add(tiles[x - 1, y + 1]);
                    }
                }
                if (y > 0)
                {
                    tiles[x, y].neighbors.Add(tiles[x, y - 1]);
                }
                if (x > 0)
                {
                    tiles[x, y].neighbors.Add(tiles[x - 1, y]);
                }
                if (x < mapWidth - 1)
                {
                    tiles[x, y].neighbors.Add(tiles[x + 1, y]);
                }
                if (y < mapHeight - 1)
                {
                    tiles[x, y].neighbors.Add(tiles[x, y + 1]);
                }
            }
        }
    }
    
    public void GeneratePathTo(Tile tile, GeneralUnit selectedUnit)
    {
        MovingUnit unit = selectedUnit.GetComponent<MovingUnit>();
        if (!unit)//check that unit is moving unit
            return;
        
        unit.currentPath = null;//clear old path

        Dictionary<Tile, float> dist = new Dictionary<Tile, float>();
        Dictionary<Tile, Tile> prev = new Dictionary<Tile, Tile>();

        //create list of nodes which we have'nt checked yet
        List<Tile> unvisited = new List<Tile>();
        Tile source = tiles[(int)selectedUnit.GetComponent<MovingUnit>().GetTilePosition().x, (int)selectedUnit.GetComponent<MovingUnit>().GetTilePosition().y];
        Tile target = tiles[(int)tile.GetTilePosition().x, (int)tile.GetTilePosition().y];
        dist[source] = 0;
        prev[source] = null;

        //initialize to infinity distance 
        foreach (Tile v in tiles)
        {
            if (v != source)
            {
                dist[v] = Mathf.Infinity;
                prev[v] = null;
            }
            unvisited.Add(v);
        }


        while (unvisited.Count > 0)
        {
            //u will be the unvisited node with the smallest possible distance
            Tile u = null;
            foreach (Tile possibleU in unvisited)
            {
                if (u == null || dist[possibleU] < dist[u])
                {
                    u = possibleU;
                }
            }

            //exit loop if found target
            if (u == target)
            {
                break;
            }

            unvisited.Remove(u);

            foreach (Tile v in u.neighbors)
            {
                //float alt = dist[u] + u.DistanceTo(v);
                float alt = dist[u] + v.GetTileType().costToEnter;
                if (alt < dist[v])
                {
                    dist[v] = alt;
                    prev[v] = u;
                }
            }
        }

        //found shortest route or no route available
        if (prev[target] == null)
        {
            return; //no route to target
        }
        List<Tile> currentPath = new List<Tile>();
        Tile current = target;
        //step through prev chain and add to path
        while (prev[current] != null)
        {
            currentPath.Add(current);
            current = prev[current];
        }
        currentPath.Add(source);
        //inverting the path to the correct order
        currentPath.Reverse();
        //selectedUnit.GetComponent<MovingUnit>().SetCurrentPath()
        selectedUnit.GetComponent<MovingUnit>().currentPath.tilesInPath = currentPath;
    }
    public Vector3 hexToWorldCoord(int x, int y)
    {
        string hexName = "Hex_" + x + "_" + y;
        GameObject hex = GameObject.Find(hexName);
        return new Vector3(hex.transform.position.x, hex.transform.position.y, -1);
    }
}
